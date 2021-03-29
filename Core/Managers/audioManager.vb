Imports System.Text
Imports Discord
Imports Discord.WebSocket
Imports Microsoft.Extensions.DependencyInjection
Imports Microsoft.Extensions.Logging
Imports Victoria
Imports Victoria.Enums
Imports Victoria.EventArgs

NotInheritable Class audioManager
    Private Shared _lavaNode As LavaNode = serviceManager.provider.GetRequiredService(Of LavaNode)
    Private Shared _logger As ILogger

    Public Sub New(ByVal lavaNode As LavaNode, loggerFactory As ILoggerFactory)
        _lavaNode = lavaNode
        _logger = loggerFactory.CreateLogger(Of LavaNode)()

        AddHandler _lavaNode.OnTrackEnded, AddressOf trackEnded
        AddHandler _lavaNode.OnLog, Function(arg)
                                        _logger.Log(arg.Severity, arg.Exception, arg.Message)
                                        Return Task.CompletedTask
                                    End Function
    End Sub

    Public Shared Async Function joinAsync(ByVal guild As IGuild, ByVal voiceState As IVoiceState, ByVal channel As ITextChannel) As Task(Of String)

        If _lavaNode.HasPlayer(guild) Then
            Return "I'm already connected to a voice channel."
        End If
        If voiceState.VoiceChannel Is Nothing Then
            Return "You must be connected to a voice channel"
        End If

        Try
            Await _lavaNode.JoinAsync(voiceState.VoiceChannel, channel)
            Return $"Joined {voiceState.VoiceChannel.Name}"
        Catch ex As Exception
            Return $"ERROR: {ex.Message}"
        End Try

    End Function

    Public Shared Async Function playAsync(user As SocketGuildUser, guild As IGuild, query As String) As Task(Of String)

        'Error handling
        If user.VoiceChannel Is Nothing Then
            Return "You must join a voice channel!"
        End If
        If Not _lavaNode.HasPlayer(guild) Then
            Return "I'm not connected to a voice channel."
        End If
        Try
            Dim player = _lavaNode.GetPlayer(guild)
            Dim track As LavaTrack
            Dim search = If(Uri.IsWellFormedUriString(query, UriKind.Absolute), Await _lavaNode.SearchAsync(query), Await _lavaNode.SearchYouTubeAsync(query))

            If search.LoadStatus = LoadStatus.NoMatches Then
                Return $"{query} could not be found"
            End If
            track = search.Tracks.FirstOrDefault

            If player.Track IsNot Nothing AndAlso player.PlayerState = PlayerState.Playing OrElse player.PlayerState = PlayerState.Paused Then
                player.Queue.Enqueue(track)
                Console.WriteLine($"[{Date.Now}] (AUDIO) - Track was added to queue.")
                Return $"{track.Title} has been added to queue"
            End If

            Await player.PlayAsync(track)
            Console.WriteLine($"[{Date.Now}] (AUDIO) [Now playing] - {track.Title}")
            Return $"Now playing: {track.Title}"
        Catch ex As Exception
            Return $"ERROR: {ex.Message}"
        End Try

    End Function

    Public Shared Async Function leaveAsync(guild As IGuild) As Task(Of String)

        Try
            Dim player = _lavaNode.GetPlayer(guild)
            If player.PlayerState = PlayerState.Playing Then
                Await player.StopAsync
                Await _lavaNode.LeaveAsync(player.VoiceChannel)
                Console.WriteLine($"[{Date.Now}] {vbTab} (AUDIO) {vbTab} Bot has left a voice channel")
                Return "Thank you for listening!"
            End If
        Catch ex As InvalidOperationException
            Return $"Error: {ex.Message}"
        End Try

    End Function

    Public Shared Async Function setVolumeAsync(guild As IGuild, vol As Integer) As Task(Of String)
        If vol > 150 Or vol <= 0 Then 'Limits the volume to only be between 1 and 150
            Return $"Volume can only be set between 1 and 150."
        End If

        Try
            Dim player = _lavaNode.GetPlayer(guild)

            Await player.UpdateVolumeAsync(Math.Truncate(vol))
            Console.WriteLine($"[{Date.Now}] {vbTab} (AUDIO) - Bot volume was set to {vol}")
            Return $"Volume has been set to {vol}."

        Catch ex As Exception
            Return ex.Message
        End Try

    End Function

    Public Shared Async Function togglePauseAsync(guild As IGuild) As Task(Of String)
        Try
            Dim player = _lavaNode.GetPlayer(guild)

            If Not player.PlayerState = PlayerState.Playing And player.Track Is Nothing Then
                Return "There is nothing playing"
            End If
            If Not player.PlayerState = PlayerState.Playing Then
                Await player.ResumeAsync
                Return $"**Resumed:** {player.Track.Title}"
            End If
            Await player.PauseAsync
            Return $"***Paused:*** {player.Track.Title}"


        Catch ex As InvalidOperationException
            Return ex.Message
        End Try

    End Function

    Public Shared Async Function skipTrack(guild As IGuild) As Task(Of String)
        Try
            Dim player = _lavaNode.GetPlayer(guild)
            If player Is Nothing Then 'Check is the player is null
                Return "Could not find player"
            End If
            If player.Queue.Count < 1 Then 'Checking queue to see if less than 1 if true skip
                Return "No more tracks to skip to"

            Else
                Try
                    'Save current song for use
                    Dim currentTrack = player.Track
                    'Skip current track
                    Await player.SkipAsync
                    Return $"The song {currentTrack.Title} has been skipped"

                Catch ex As Exception
                    Return ex.Message
                End Try
            End If

        Catch ex As Exception
            Return ex.Message
        End Try
    End Function

    Public Shared Async Function listTracks(guild As IGuild) As Task(Of String)
        Try
            Dim descBuilder = New StringBuilder
            Dim player = _lavaNode.GetPlayer(guild)
            If player Is Nothing Then 'Chek if player is null
                Return "Player could not be found"
            End If
            If player.PlayerState = PlayerState.Playing Then
                If player.Queue.Count < 1 And player.Track IsNot Nothing Then
                    Return $"**Now Playing:** {player.Track.Title} - Nothing else is queued"

                Else
                    Dim trackNum = 2
                    For Each track As LavaTrack In player.Queue
                        descBuilder.Append($"{trackNum}: [{track.Title}]({track.Url}) - {track.Id} {Environment.NewLine}")
                        trackNum += 1
                    Next
                    Return descBuilder.ToString

                End If
            End If

        Catch ex As Exception
            Return ex.Message
        End Try
    End Function

    Public Shared Async Function trackEnded(args As TrackEndedEventArgs) As Task

        If Not args.Reason.ShouldPlayNext Then
            Return
        End If

        Dim player = args.Player
        Dim queueable As LavaTrack
        If Not player.Queue.TryDequeue(queueable) Then
            Await args.Player.TextChannel.SendMessageAsync("Playback Finished")
            Return
        End If
        Dim tempVar As Boolean = TypeOf queueable Is LavaTrack
        Dim track As LavaTrack = If(tempVar, queueable, Nothing)
        If Not tempVar Then
            Await player.TextChannel.SendMessageAsync("Next item in the queue is not a track")
            Return
        End If
        Await player.PlayAsync(track)
        Await player.TextChannel.SendMessageAsync($"Now Playing *{track.Title} - {track.Author}*")

    End Function

End Class
