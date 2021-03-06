Imports System
Imports System.Drawing
Imports System.IO
Imports System.Media
Imports System.Threading.Thread
Imports Figgle

#Region "To-Do List"
'Add custom Logging
'Use DraxCodes' way of sending messages using embeds instead of plain text - Some works and some does not not fully implemented yet
'Figure out how to make multi-colored ascii text for the banner
'Leave command gets a null exception error when not connected to a voice channel even with attempted error handling in place
#End Region

Module Program
    Dim path = AppDomain.CurrentDomain.BaseDirectory
    Dim process As Process = New Process
    Dim lavalink = $"{path}" + "Lavalink.jar"
    Dim app = $"{path}" + "application.yaml"
    Dim utils As New Utilities
    Sub Main()
        Call setUp().GetAwaiter.GetResult()
    End Sub

    Private Async Function setUp() As Task
        Console.Title = "Gawr Gura"
        utils.setBanner("/ Gawr Gura \", ConsoleColor.Cyan, ConsoleColor.Green)
        Await (loggingManager.LogSetupAsync("setup", "Looking for Lavalink server..."))
        If Not File.Exists(lavalink) Or Not File.Exists(app) Then
            Await loggingManager.LogCriticalAsync("setup", "After the program closes please add your Lavalink.jar and application.yml file into the bin\netcoreapp3.1 folder.")
            Console.WriteLine()
            Sleep(3000)
            Environment.Exit(0)
        Else
            Await loggingManager.LogSetupAsync("setup", "Lavalink server has been found now starting server...")
            process.EnableRaisingEvents = False
            process.StartInfo.FileName = "javaw.exe"
            process.StartInfo.Arguments = "-jar " + """" + path + "Lavalink.jar"
            process.Start()
            Sleep(5000)
            Await loggingManager.LogSetupAsync("setup", "Sever is setup now starting bot")
            Sleep(1500)
            Call New bot().mainAsync().GetAwaiter().GetResult()

        End If
    End Function


End Module
