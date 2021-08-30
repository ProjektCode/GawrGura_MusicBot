Imports System.Runtime.InteropServices
Imports Figgle

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

#Region "Command Window Banner Options"
    Public Sub setBanner(text As String, color As ConsoleColor, _color As ConsoleColor)
        Console.ForegroundColor = color
        Console.Write(FiggleFonts.Standard.Render(text), color)
        Console.ForegroundColor = _color
        Console.WriteLine("===================================================================", _color)
    End Sub

    Public Sub consoleTextColor(text As String, color As ConsoleColor)
        Console.ForegroundColor = color
        Console.WriteLine(vbTab + text, color)
    End Sub
#End Region

#Region "Color Options"

    Public Function randomEmbedColor()
        Dim rand As New Random
        Dim colors() As Integer =
        {
            16711680, 'Red
            65280,    ' Green
            255,      ' Blue
            16777215, ' White
            10617087, ' Purple
            65535,    ' Light Blue
            16776960, ' Yellow
            16711935, ' Light Purple
            16751104, ' Orange
            16751586, ' Light Pink
            10682267, ' Light Green
            14423100, ' Crimson
            9055202,  ' Blue Violet
            15132410  ' Lavendar
        }

        Dim colorPicker As Integer = colors(rand.Next(0, colors.Length))
        Dim colour As UInteger = Convert.ToInt32(colorPicker)

        Return colour
    End Function
#End Region

End Class
