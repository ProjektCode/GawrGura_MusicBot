Imports System
Imports System.Threading.Thread

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
    Sub Main()
        Console.Title = "Gawr Gura"

        process.EnableRaisingEvents = False
        process.StartInfo.FileName = "java.exe"
        process.StartInfo.Arguments = "-jar " + """" + path + "Lavalink.jar"
        process.Start()
        Sleep(5000)
        Console.Clear()
        Call New bot().mainAsync().GetAwaiter().GetResult()
    End Sub
End Module
