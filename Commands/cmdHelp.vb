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
		Dim desc

		For Each [module] In _service.Modules
			If Not [module].Name = "Music" Then

			Else
				Dim description As String = Nothing
				For Each cmd In [module].Commands
					Dim result = Await cmd.CheckPreconditionsAsync(Context)
					If result.IsSuccess Then
						description &= $"__*{cmd.Aliases.First}*__ - {cmd.Summary}" & vbLf
					End If
				Next cmd

				If Not String.IsNullOrWhiteSpace(description) Then
					desc = description
				End If
			End If
		Next [module]

		Dim embed = New EmbedBuilder() With {
			.Color = New Color(_utils.randomEmbedColor),
			.Title = "Music Commands",
			.Description = desc
		}

		Await ReplyAsync("", False, embed.Build())
	End Function

End Class
