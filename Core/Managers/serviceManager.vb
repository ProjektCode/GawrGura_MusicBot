Imports Microsoft.Extensions.DependencyInjection

NotInheritable Class serviceManager
    Public Shared provider As IServiceProvider

    Public Shared Sub setProvider(collection As ServiceCollection)
        provider = collection.BuildServiceProvider()
    End Sub

    Public Shared Function getService(Of T As New)()
        Return provider.GetRequiredService(Of T)()
    End Function

End Class
