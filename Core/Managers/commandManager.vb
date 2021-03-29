Imports System.Reflection
Imports Discord.Commands

NotInheritable Class commandManager

    Private Shared cmdService = serviceManager.getService(Of CommandService)

    Public Shared Async Function loadCommandsAsync() As Task
        Await cmdService.AddModulesAsync(Assembly.GetEntryAssembly, serviceManager.provider)

    End Function

End Class
