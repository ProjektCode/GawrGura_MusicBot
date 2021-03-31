Imports System
Imports System.Drawing
Imports System.IO
Imports System.Media
Imports System.Threading.Thread
Imports Figgle

#Region "To-Do List"
'Add custom Logging
'Use DraxCodes' way of sending messages using embeds instead of plain text
'Create help command
'Create clear queue command
'See if you can make the bot work via yt links instead of remember song name
#End Region

Module Program
    Dim path = AppDomain.CurrentDomain.BaseDirectory
    Dim process As Process = New Process
    Dim lavalink = $"{path}" + "Lavalink.jar"
    Dim app = $"{path}" + "application.yaml"
    Sub Main()
        Console.Title = "Gawr Gura"
        setBanner("/ Gawr Gura \", ConsoleColor.Cyan, ConsoleColor.Red)
        consoleTextColor("Looking for Lavalink server....", ConsoleColor.DarkRed)
        If Not File.Exists(lavalink) Or Not File.Exists(app) Then
            Console.WriteLine("After the program closes please add your Lavalink.jar and application.yml file into the bin\netcoreapp3.1 folder.")
            Sleep(3000)
        Else
            consoleTextColor("Lavalink server has been found now starting server...", ConsoleColor.Yellow)
            process.EnableRaisingEvents = False
            process.StartInfo.FileName = "javaw.exe"
            process.StartInfo.Arguments = "-jar " + """" + path + "Lavalink.jar"
            process.Start()
            Sleep(5000)
            consoleTextColor("Sever is setup now starting bot", ConsoleColor.Green)
            Sleep(1500)
            Console.Clear()
            setBanner("/ Gawr Gura \", ConsoleColor.Cyan, ConsoleColor.Green)
            Call New bot().mainAsync().GetAwaiter().GetResult()

        End If
    End Sub

    Private Sub setBanner(text As String, color As ConsoleColor, _color As ConsoleColor)
        Console.ForegroundColor = color
        Console.Write(FiggleFonts.Standard.Render(text), color)
        Console.ForegroundColor = _color
        Console.WriteLine("===================================================================", _color)
    End Sub

    Private Sub consoleTextColor(text As String, color As ConsoleColor)
        Console.ForegroundColor = color
        Console.WriteLine(vbTab + text, color)
    End Sub
End Module
