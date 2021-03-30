Imports Discord
Imports Discord.WebSocket
Imports Discord.Commands
Imports Microsoft.Extensions.DependencyInjection
Imports Victoria
Imports System.Threading
Imports Discord.Net.Providers.WS4Net

Public Class bot
    Private _client As DiscordSocketClient
    Private _cmdService As CommandService
    Private _config As configManager
    Private ReadOnly eManager As New eventManager

    Public Sub bot()
        _client = New DiscordSocketClient(New DiscordSocketConfig() With {
            .LogLevel = LogSeverity.Info,
            .DefaultRetryMode = Discord.RetryMode.AlwaysRetry,
            .WebSocketProvider = WS4NetProvider.Instance
         })

        _cmdService = New CommandService(New CommandServiceConfig() With {
            .LogLevel = LogSeverity.Info,
            .CaseSensitiveCommands = False,
            .DefaultRunMode = RunMode.Async,
            .IgnoreExtraArgs = True
        })

        'Setting up services
        Dim collection = New ServiceCollection
        collection.AddSingleton(_client)
        collection.AddSingleton(_cmdService)
        collection.AddLavaNode(Sub(LavaConfig)
                                   LavaConfig.SelfDeaf = False
                               End Sub)
        serviceManager.setProvider(collection)

    End Sub

    Public Async Function mainAsync() As Task
        Console.WriteLine($"What are you doing waking me up? {Environment.NewLine}")

        _config = configManager.Load
        bot()

        Await commandManager.loadCommandsAsync
        Await eManager.loadEvents()
        Await _client.LoginAsync(TokenType.Bot, _config.token)
        Await _client.StartAsync
        Await Task.Delay(Timeout.Infinite)
    End Function




End Class
