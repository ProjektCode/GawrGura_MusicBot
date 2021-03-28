Imports Discord
Imports Discord.WebSocket
Imports Microsoft.Extensions.DependencyInjection
Imports Victoria
Imports Victoria.Enums
Imports Victoria.EventArgs
Imports System

NotInheritable Class audioManager
    Private Shared ReadOnly _lavaNode As LavaNode = serviceManager.provider.GetRequiredService(Of LavaNode)

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
            Console.WriteLine($"Now playing: {track.Title} ")
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

    Public Shared Async Function trackEnded(args As TrackEndedEventArgs) As Task

        If Not args.Reason.ShouldPlayNext Then
            Return
        End If

        Dim queueable As LavaTrack
        If Not args.Player.Queue.TryDequeue(queueable) Then

            Return
        End If
        Dim tempVar As Boolean = TypeOf queueable Is LavaTrack
        Dim track As LavaTrack = If(tempVar, CType(queueable, LavaTrack), Nothing)
        If Not tempVar Then
            Await args.Player.TextChannel.SendMessageAsync("Next item in the queue is not a track")
            Return
        End If
        Await args.Player.PlayAsync(track)
        Await args.Player.TextChannel.SendMessageAsync($"Now Playing *{track.Title} - {track.Author}*")

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

End Class
