Imports Discord
Imports Discord.Commands

<Name("Help")>
Public Class cmdHelp
	'Customize help command
	'Check if the module is called admin and then check to see if they are able to use said commands, if not
	'Skip the module to prevent others from seeing those commands.
	Inherits ModuleBase(Of SocketCommandContext)
	Private ReadOnly _service As CommandService
	Private _utils As New Utilities

	Public Sub New(service As CommandService)
		_service = service
	End Sub

	<Command("help")>
	<Summary("Gives a short description of all commands")>
	Public Async Function HelpAsync() As Task
		Dim embed = New EmbedBuilder() With {
			.Color = New Color(_utils.randomEmbedColor),
			.Description = "These are the commands you can use"
		}

		For Each [module] In _service.Modules
			Dim description As String = Nothing
			For Each cmd In [module].Commands
				Dim result = Await cmd.CheckPreconditionsAsync(Context)
				If result.IsSuccess Then
					description &= $"__*{cmd.Aliases.First}*__ - {cmd.Summary}" & vbLf
				End If
			Next cmd

			If Not String.IsNullOrWhiteSpace(description) Then
				embed.AddField(Sub(x)
								   x.Name = [module].Name
								   x.Value = description
								   x.IsInline = True
							   End Sub)
			End If
		Next [module]

		Await ReplyAsync("", False, embed.Build())
	End Function

End Class
