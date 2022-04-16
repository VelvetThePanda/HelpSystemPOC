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

    var commandList = commands.ToList();
    
    var executable = false;

    if (commands.Length is 2 && string.Equals(commands[0].Key, commands[1].Key, StringComparison.OrdinalIgnoreCase))
        executable = commands[0] is IParentNode ^ commands[1] is IParentNode;

    if (executable)
        commandList = (commands.First(c => c is IParentNode) as IParentNode).Children.ToList();

    Console.WriteLine($"Found {commandList.Distinct().Count()} command(s):");

    Console.WriteLine($"Parent: {(commandList[0].Parent as IChildNode)?.Key ?? "Root"}" + (executable ? " (executable)" : ""));

    for (var i = 0; i < commandList.Count; i++)
    {
        var foundCommand = commandList[i];
        var matching = commandList.Where(cn => cn.Key == foundCommand.Key);

        if (matching.Count() > 1)
        {
            if (matching.All(c => c is not IParentNode))
            {
                Console.WriteLine($"   {foundCommand.Key} - Command ({matching.Count()} overloads)");
            }
            else
            {
                var group = matching.First(c => c is IParentNode) as IParentNode;
                
                Console.WriteLine($"   {foundCommand.Key} - Group (executable) ({group.Children.Count()} children)");
            }
            
            i += matching.Count() - 1;
            continue;
        }
        
        Console.WriteLine($"   {foundCommand.Key} - {(foundCommand is IParentNode pn ? $"Group ({pn.Children.Count()} children)" : "Command")}");
    }

}