Imports Discord.WebSocket
Imports Discord.Commands
Imports Discord
Imports Victoria
Imports Microsoft.Extensions.DependencyInjection

NotInheritable Class eventManager
    Private Shared _lavaNode As LavaNode = serviceManager.provider.GetRequiredService(Of LavaNode)
    Private Shared _client As DiscordSocketClient = serviceManager.getService(Of DiscordSocketClient)
    Private Shared _cmdService As CommandService = serviceManager.getService(Of CommandService)
    Private config As configManager

    Public Function loadEvents() As Task
        config = configManager.Load
        AddHandler _client.Log, AddressOf clientLog
        AddHandler _cmdService.Log, AddressOf commandLog
        AddHandler _client.Ready, AddressOf onReady
        AddHandler _client.MessageReceived, AddressOf messageRecieved
        AddHandler _lavaNode.OnTrackEnded, AddressOf audioManager.trackEnded

        Return Task.CompletedTask
    End Function

    Private Function clientLog(msg As LogMessage) As Task
        Console.WriteLine($"[{msg}]")
        Return Task.CompletedTask
    End Function
    Private Function commandLog(msg As LogMessage) As Task
        Console.WriteLine($"[{msg}] has been used.")
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

        'Console.WriteLine($"[{Date.Now}]{vbTab}(READY) - Bot is ready")
        Await _client.SetStatusAsync(UserStatus.Online)
        Await _client.SetGameAsync($"prefix is {config.prefix}", type:=ActivityType.Listening)
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


End Class
