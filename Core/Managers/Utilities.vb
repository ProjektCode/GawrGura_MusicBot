Imports System.Runtime.InteropServices

Public Class Utilities


#Region "Command Window Options"
    <DllImport("Kernel32.dll")>
    Public Shared Function GetConsoleWindow() As IntPtr

    End Function
    <DllImport("User32.dll")>
    Private Shared Function ShowWindow(hwd As IntPtr, cmdShow As Integer) As Boolean

    End Function

    Dim hwd As IntPtr = GetConsoleWindow()

    'Hides Console Window
    Public Function winHide()
        Return ShowWindow(hwd, 0)
    End Function

    'Exposes Console Window
    Public Function winShow()
        Return ShowWindow(hwd, 1)
    End Function
#End Region

End Class
