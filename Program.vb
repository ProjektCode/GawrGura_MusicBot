Imports System

Module Program
    Sub Main()
        Call New bot().mainAsync().GetAwaiter().GetResult()
    End Sub
End Module
