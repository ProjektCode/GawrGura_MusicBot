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
        Console.Title = "Looking for Lavalink server"
        Sleep(2000)
        If Not File.Exists(lavalink) Or Not File.Exists(app) Then
            Console.WriteLine("After the program closes please add your Lavalink.jar and application.yml file into the bin\netcoreapp3.1 folder.")
            Sleep(3000)
        Else
            Console.Title = "Starting Lavalink Server..."
            Console.WriteLine("Currently setting up the lavalink server...")
            process.EnableRaisingEvents = False
            process.StartInfo.FileName = "javaw.exe"
            process.StartInfo.Arguments = "-jar " + """" + path + "Lavalink.jar"
            process.Start()
            Sleep(5000)
            Console.Clear()
            Console.Write("Sever is setup now starting bot")
            Sleep(1500)
            Console.Clear()
            Console.Title = "Starting Gura"
            setBanner("/ Gawr Gura \", ConsoleColor.Cyan)
            Call New bot().mainAsync().GetAwaiter().GetResult()

        End If
    End Sub

    Private Sub setBanner(text As String, color As ConsoleColor)
        FiggleFonts.Standard.Render(text)
        Console.ForegroundColor = color
        Console.WriteLine(FiggleFonts.Standard.Render(text), color)
    End Sub
End Module
