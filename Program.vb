Imports System

#Region "To-Do List"
'Add custom Logging
'Use DraxCodes' way of sending messages using embeds instead of plain text
'Create help command
'Create clear queue command
'See if you can make the bot work via yt links instead of remember song name
#End Region

Module Program
    Sub Main()
        Console.Title = "Gawr Gura Music Bot"
        Call New bot().mainAsync().GetAwaiter().GetResult()
    End Sub
End Module
