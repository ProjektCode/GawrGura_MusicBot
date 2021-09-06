Imports Discord
Imports Discord.Commands
Imports Discord.WebSocket

<Name("Admin")>
<Group("admin")>
Public Class cmdAdmin
    Inherits ModuleBase(Of SocketCommandContext)
    ReadOnly _utils As Utilities = New Utilities
    Dim start As New System.Diagnostics.ProcessStartInfo

    <Command("hide")>
    <Summary("Hides command window.")>
    Public Async Function cmdHide() As Task
        Dim author = Context.Message.Author

        If DirectCast(author, SocketGuildUser).GuildPermissions.Administrator Then
            _utils.winHide()

            Await Context.Channel.SendMessageAsync($"I'll be watching you.")
        Else
            Await Context.Channel.SendMessageAsync("This is only for the bot owner.")
        End If
    End Function

    <Command("show")>
    <Summary("Shows command window.")>
    Public Async Function cmdShow() As Task
        Dim author = Context.Message.Author

        If DirectCast(author, SocketGuildUser).GuildPermissions.Administrator Then
            _utils.winShow()

            Await Context.Channel.SendMessageAsync($"HA {author.Mention} I have returned.")
        Else
            Await Context.Channel.SendMessageAsync("This is only for the bot owner.")
        End If
    End Function

    <Command("kill")>
    <Summary("Kills the bot process.")>
    Public Async Function cmdKill() As Task
        Dim author = Context.Message.Author
        If DirectCast(author, SocketGuildUser).GuildPermissions.Administrator Then
            For Each p As Process In Process.GetProcesses
                If p.ProcessName = "Gawr_Gura" Then
                    Try
                        Await Context.Channel.SendMessageAsync("Going for a swim.")
                        p.Kill()
                    Catch ex As Exception
                        Continue For
                    End Try
                End If
            Next
        End If
    End Function

End Class
