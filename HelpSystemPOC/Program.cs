using HelpSystemPOC;
using Microsoft.Extensions.DependencyInjection;
using Remora.Commands.Extensions;
using Remora.Commands.Trees.Nodes;

var services = new ServiceCollection();

services.AddSingleton<HelpGenerator>();
services.AddCommands();

services.AddCommandTree().WithCommandGroup<Commands>();

var provider = services.BuildServiceProvider();

var help = provider.GetRequiredService<HelpGenerator>();

var exit = false;

Console.CancelKeyPress += (_, _) => exit = true;

while (!exit)
{
    Console.Write("Looking for command: ");
    
    var command = Console.ReadLine();
    
    var result = help.GetMatchingCommands(command);

    if (!result.IsDefined(out var commands))
    {
        Console.Error.WriteLine("No dice, sorry.");
        
        continue;
    }
    
    Console.WriteLine($"Found {commands.Count()} command(s):");
    
    foreach (var foundCommand in commands)
        Console.WriteLine($"  {foundCommand.Key} - {(foundCommand is IParentNode ? "Group" : "Command")}");
}