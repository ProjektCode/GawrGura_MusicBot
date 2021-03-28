Imports System.Reflection
Imports Discord.Commands

NotInheritable Class commandManager

    Private Shared cmdService = serviceManager.getService(Of CommandService)

    Public Shared Async Function loadCommandsAsync() As Task
        Await cmdService.AddModulesAsync(Assembly.GetEntryAssembly, serviceManager.provider)
        For Each command In cmdService.Commands
            Console.WriteLine($"Command {command.name} has loaded.")
        Next

    End Function

End Class
