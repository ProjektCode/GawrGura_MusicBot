Imports Discord.WebSocket
Imports Discord.Commands
Imports Discord
Imports Victoria
Imports Microsoft.Extensions.DependencyInjection
Imports Microsoft.Extensions.Logging
Imports Victoria.EventArgs
Imports System.Media

NotInheritable Class eventManager
    Private Shared _lavaNode As LavaNode = serviceManager.provider.GetRequiredService(Of LavaNode)
    Private Shared _client As DiscordSocketClient = serviceManager.getService(Of DiscordSocketClient)
    Private Shared _cmdService As CommandService = serviceManager.getService(Of CommandService)
    Private config As configManager

    Public Function loadEvents() As Task
        config = configManager.Load
        AddHandler _client.Log, AddressOf logAsync
        AddHandler _cmdService.Log, AddressOf logAsync
        AddHandler _client.Ready, AddressOf onReady
        AddHandler _client.MessageReceived, AddressOf messageRecieved
        AddHandler _lavaNode.OnTrackEnded, AddressOf audioManager.trackEnded


        Return Task.CompletedTask
    End Function

#Region "Discord Events"

    Private Async Function logAsync(msg As LogMessage) As Task
        Await (loggingManager.LogAsync(msg.Source, msg.Severity, msg.Message))
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
        SystemSounds.Asterisk.Play()
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

End Class
