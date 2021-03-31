Imports Discord.WebSocket
Imports Discord.Commands
Imports Discord
Imports Victoria
Imports Microsoft.Extensions.DependencyInjection
Imports Microsoft.Extensions.Logging
Imports Victoria.EventArgs

NotInheritable Class eventManager
    Private Shared _lavaNode As LavaNode = serviceManager.provider.GetRequiredService(Of LavaNode)
    Private Shared _client As DiscordSocketClient = serviceManager.getService(Of DiscordSocketClient)
    Private Shared _cmdService As CommandService = serviceManager.getService(Of CommandService)
    Private config As configManager
    Private _logger As LoggerMessage

    Public Function loadEvents() As Task
        config = configManager.Load
        AddHandler _client.Log, AddressOf clientLog
        AddHandler _cmdService.Log, AddressOf commandLog 'Not working as intended figure out why
        AddHandler _client.Ready, AddressOf onReady
        AddHandler _client.MessageReceived, AddressOf messageRecieved
        AddHandler _lavaNode.OnTrackEnded, AddressOf trackEnded
        AddHandler _lavaNode.OnLog, AddressOf lava_onLog


        Return Task.CompletedTask
    End Function

#Region "Discord Events"
    Private Function clientLog(msg As LogMessage) As Task
        Console.WriteLine($"(GATEWAY) {vbTab} {msg}")
        Return Task.CompletedTask
    End Function
    Private Function commandLog(msg As LogMessage) As Task
        Console.WriteLine($"(COMMAND) {vbTab} {msg}")
        Return Task.CompletedTask
    End Function
    Private Async Function onReady() As Task 'Place custom game event here once masterClass is made
        If Not _lavaNode.IsConnected Then
            Try
                Await _lavaNode.ConnectAsync
            Catch ex As Exception
                Throw ex
            End Try
        End If

        Await _client.SetStatusAsync(UserStatus.Online)
        Await _client.SetGameAsync($"prefix is {config.prefix}", type:=ActivityType.Listening)
        Console.WriteLine($"(READY EVENT FINISHED) - Bot is ready")
    End Function
    Private Async Function messageRecieved(arg As SocketMessage) As Task
        Dim message = TryCast(arg, SocketUserMessage)
        Dim context = New SocketCommandContext(_client, message)
        config = configManager.Load

        If message.Author.IsBot OrElse TypeOf message.Channel Is IDMChannel Then
            Return
        End If

        Dim argPos As Integer = 0

        If Not (message.HasStringPrefix(config.prefix, argPos) OrElse message.HasMentionPrefix(_client.CurrentUser, argPos)) Then
            Return
        End If

        Dim result = Await _cmdService.ExecuteAsync(context, argPos, serviceManager.provider)

        If Not result.IsSuccess Then
            If result.Error = CommandError.UnknownCommand Then
                Return
            End If
        End If


    End Function

#End Region


#Region "Victoria Events"
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

    Private Function lava_onLog(arg As LogMessage) As Task
        Console.WriteLine($"(VICTORIA) {vbTab} {arg}")
        Return Task.CompletedTask
    End Function


#End Region


End Class
