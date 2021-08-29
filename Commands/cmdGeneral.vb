Imports Discord.Commands
Imports Discord

<Name("General")>
Public Class cmdGeneral
	'Customize help command
	Inherits ModuleBase(Of SocketCommandContext)
	Private ReadOnly _service As CommandService
	Private ReadOnly _config As configManager

	Public Sub New(ByVal service As CommandService)
		_service = service
		_config = configManager.Load
	End Sub

	<Command("help")>
	<Summary("Gives a short description of all commands")>
	Public Async Function HelpAsync() As Task
		Dim builder = New EmbedBuilder() With {
			.Color = New Color(114, 137, 218),
			.Description = "These are the commands you can use"
		}

		For Each [module] In _service.Modules
			Dim description As String = Nothing
			For Each cmd In [module].Commands
				Dim result = Await cmd.CheckPreconditionsAsync(Context)
				If result.IsSuccess Then
					description &= $"__*{cmd.Aliases.First()}*__ - {cmd.Summary}" & vbLf
				End If
			Next cmd

			If Not String.IsNullOrWhiteSpace(description) Then
				builder.AddField(Sub(x)
									 x.Name = [module].Name
									 x.Value = description
									 x.IsInline = True
								 End Sub)
			End If
		Next [module]

		Await ReplyAsync("", False, builder.Build())
	End Function

End Class
