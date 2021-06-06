Imports System.Collections.Concurrent
Imports System.Text
Imports System.Threading
Imports Discord
Imports Discord.Commands
Imports Discord.WebSocket
Imports Microsoft.Extensions.DependencyInjection
Imports Microsoft.Extensions.Logging
Imports Victoria
Imports Victoria.Enums
Imports Victoria.EventArgs

NotInheritable Class audioManager
    Private Shared _lavaNode As LavaNode = serviceManager.provider.GetRequiredService(Of LavaNode)
    Private Shared _disconnectTokens = New ConcurrentDictionary(Of ULong, CancellationTokenSource)()


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

    Public Shared Async Function leaveAsync(guild As IGuild) As Task(Of String)
        Try
            Dim player = _lavaNode.GetPlayer(guild)
            If player.PlayerState = PlayerState.Playing Or player.PlayerState = PlayerState.Connected Then
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

    Public Shared Async Function listTracks(guild As IGuild) As Task(Of String) 'When converting to embed add track.url back
        Try
            Dim descBuilder = New StringBuilder
            Dim player = _lavaNode.GetPlayer(guild)
            If player Is Nothing Then 'Check if player is null
                Return "Player could not be found"
            End If
            If player.PlayerState = PlayerState.Playing Then
                If player.Queue.Count < 1 And player.Track IsNot Nothing Then
                    Return $"**Now Playing:** {player.Track.Title} - Nothing else is queued"

                Else
                    Dim trackNum = 2
                    For Each track As LavaTrack In player.Queue
                        descBuilder.Append($"{trackNum}: [{track.Title}] {Environment.NewLine}")
                        trackNum += 1
                    Next
                    Return descBuilder.ToString

                End If
            End If

        Catch ex As Exception
            Return ex.Message
        End Try
    End Function

    Public Shared Async Function clearTracks(guild As IGuild) As Task(Of String)
        'Find out why it's only removing the first entry in the queue

        Try
            Dim player = _lavaNode.GetPlayer(guild)
            Dim descBuilder = New StringBuilder
            If player Is Nothing Then
                Return "Player could not be found"
            End If
            If player.PlayerState = PlayerState.Playing Then
                For Each track As LavaTrack In player.Queue
                    player.Queue.TryDequeue(track)
                    Console.WriteLine($"{track.Title} has been removed")
                Next

            End If

            If player.Queue.Count = 0 Then
                Return "queue has been cleared"
            Else
                Dim tracknum = player.Queue.Count
                For Each track As LavaTrack In player.Queue
                    descBuilder.Append($"{tracknum}: [{track.Title}] - could not be cleared {Environment.NewLine}")
                    tracknum += 1
                Next
                Return descBuilder.ToString
            End If
        Catch ex As Exception
            Return ex.Message
        End Try
    End Function

    Public Shared Async Function stopAsync(guild As IGuild) As Task(Of String)

        Try

            Dim player = _lavaNode.GetPlayer(guild)
            If player Is Nothing Then
                Return "Could not find player."
            End If

            If player.PlayerState = PlayerState.Playing Then
                Await player.StopAsync
            Else
                Return "You have to be playing something in order to use this command"
            End If
            Await loggingManager.LogInformationAsync("audio", $"Bot has stopped playback.")
            Return "Playback has stopped and queue has been cleared"

        Catch ex As Exception
            Return ex.Message
        End Try

    End Function

    Public Shared Async Function restartAsync(guild As IGuild) As Task(Of String)

        Try

            Dim player = _lavaNode.GetPlayer(guild)
            If player Is Nothing Then
                Return "Could not find player."
            End If
            If Not player.PlayerState = PlayerState.Playing Then
                Return "I need to be playing something in order to repeat."
            End If

            Dim timespan As TimeSpan = TimeSpan.Zero
            Await player.SeekAsync(timespan)
            Await loggingManager.LogInformationAsync("audio", $"{player.Track.Title} has been repeated.")
            Return $"I have repeated {player.Track.Title}"


        Catch ex As Exception
            loggingManager.LogCriticalAsync("audio", ex.Message)
            Return ex.Message

        End Try


    End Function

    Public Shared Async Function seekAsync(guild As IGuild, timeSpan As TimeSpan) As Task(Of String) 'Broken - figure out why it is not working

        Try

            Dim player = _lavaNode.GetPlayer(guild)
            If player Is Nothing Then
                Return "Could not find player."
            End If
            If Not player.PlayerState = PlayerState.Playing Then
                Return "I need to be playing something in order to repeat."
            End If
            Await loggingManager.LogInformationAsync("audio", $"{player.Track.Title} has been seeked to {timeSpan}.")
            Await player.SeekAsync(timeSpan)
            Return $"**{player.Track.Title}** has been seeked to *{timeSpan}*."


        Catch ex As Exception
            loggingManager.LogCriticalAsync("audio", ex.Message)
            Return ex.Message
        End Try


    End Function

    Public Shared Async Function shuffleAsync(guild As IGuild, userMessage As SocketUserMessage, voiceState As IVoiceState) As Task(Of String)
        Try
            Dim player = _lavaNode.GetPlayer(guild)
            If player.Queue.Count = 0 Or player.Queue.Count = 1 Then
                Return "Not enough tracks for a shuffle"
            End If
            Dim users As Integer = player.VoiceChannel.GetUsersAsync.FlattenAsync.Result.Count(Function(x) Not x.IsBot)
            If voiceState.VoiceChannel Is player.VoiceChannel Or ((voiceState.VoiceChannel IsNot player.VoiceChannel) And (users = 0 Or player.Track Is Nothing)) Then
                player.Queue.Shuffle()
                Dim emo As Emoji = New Emoji(":melondio:")
                Return "Queue has been shuffled"
            End If

        Catch ex As InvalidOperationException
            Return $"Error: {ex.Message}"
        End Try
    End Function

#Region "Audio Events"
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
        Await player.TextChannel.SendMessageAsync($"Now Playing *{track.Title}* - **{track.Author}**")

    End Function

    Public Shared Async Function trackStart(ByVal args As TrackStartEventArgs) As Task
        Dim value As Object
        If Not _disconnectTokens.TryGetValue(args.Player.VoiceChannel.Id, value) Then
            Return
        End If

        If value.IsCancellationRequested Then
            Return
        End If
        value.Cancel(True)
        Await loggingManager.LogInformationAsync("audio", "Auto disconnect has been cancelled.")

    End Function


#End Region

End Class
