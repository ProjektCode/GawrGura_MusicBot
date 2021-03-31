Imports Discord
Imports System.Threading.Tasks
Imports System.Text
Imports Discord.WebSocket
Imports System
Imports System.Collections.Generic

NotInheritable Class loggingManager

	Public Shared Async Function LogAsync(ByVal src As String, ByVal severity As LogSeverity, ByVal message As String, Optional ByVal exception As Exception = Nothing) As Task
		If severity.Equals(Nothing) Then
			severity = LogSeverity.Warning
		End If
		Await Append($"{GetSeverityString(severity)}", GetConsoleColor(severity))
		Await Append($" [{SourceToString(src)}] ", ConsoleColor.DarkGray)

		If Not String.IsNullOrWhiteSpace(message) Then
			Await Append($"{message}" & vbLf, ConsoleColor.White)
		ElseIf exception Is Nothing Then
			Await Append("Uknown Exception. Exception Returned Null." & vbLf, ConsoleColor.DarkRed)
		ElseIf exception.Message Is Nothing Then
			Await Append($"Unknown {exception.StackTrace}" & vbLf, GetConsoleColor(severity))
		Else
			Console.WriteLine("I don't know the syntax translation of th")
			'Await Append($"{If(exception.Message, "Unknown")} {If(exception.StackTrace, "Unknown")}, {GetConsoleColor(severity)}, {Color.LightOrange}")
		End If
	End Function


	' The Way To Log Critical Errors
	Public Shared Function LogCriticalAsync(ByVal source As String, ByVal message As String, Optional ByVal exc As Exception = Nothing) As Task
		Return LogAsync(source, LogSeverity.Critical, message, exc)
	End Function

	' The Way To Log Basic Infomation 
	Public Shared Function LogInformationAsync(ByVal source As String, ByVal message As String) As Task
		Return LogAsync(source, LogSeverity.Info, message)
	End Function

	' Format The Output 
	Private Shared Async Function Append(ByVal message As String, ByVal color As ConsoleColor) As Task
		Await Task.Run(Sub()
						   Console.ForegroundColor = color
						   Console.Write(message)
					   End Sub)
	End Function

	' Swap The Normal Source Input To Something Neater 
	Private Shared Function SourceToString(ByVal src As String) As String
		Select Case src.ToLower()
			Case "discord"
				Return "DISCD"
			Case "victoria"
				Return "VICTR"
			Case "audio"
				Return "AUDIO"
			Case "admin"
				Return "ADMIN"
			Case "gateway"
				Return "GTWAY"
			Case "blacklist"
				Return "BLAKL"
			Case "lavanode_0_socket"
				Return "LAVAS"
			Case "lavanode_0"
				Return "LAVA#"
			Case "bot"
				Return "BOTWN"
			Case Else
				Return src
		End Select
	End Function

	' Swap The Severity To a String So We Can Output It To The Console 
	Private Shared Function GetSeverityString(ByVal severity As LogSeverity) As String
		Select Case severity
			Case LogSeverity.Critical
				Return "CRIT"
			Case LogSeverity.Debug
				Return "DBUG"
			Case LogSeverity.Error
				Return "EROR"
			Case LogSeverity.Info
				Return "INFO"
			Case LogSeverity.Verbose
				Return "VERB"
			Case LogSeverity.Warning
				Return "WARN"
			Case Else
				Return "UNKN"
		End Select
	End Function

	' Return The Console Color Based On Severity Selected 
	Private Shared Function GetConsoleColor(ByVal severity As LogSeverity) As ConsoleColor
		Select Case severity
			Case LogSeverity.Critical
				Return ConsoleColor.Red
			Case LogSeverity.Debug
				Return ConsoleColor.Magenta
			Case LogSeverity.Error
				Return ConsoleColor.DarkRed
			Case LogSeverity.Info
				Return ConsoleColor.Green
			Case LogSeverity.Verbose
				Return ConsoleColor.DarkCyan
			Case LogSeverity.Warning
				Return ConsoleColor.Yellow
			Case Else
				Return ConsoleColor.White
		End Select
	End Function


End Class
