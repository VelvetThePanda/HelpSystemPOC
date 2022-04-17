using System.Text;
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

    var output = new StringBuilder();
    
    if (commands.Length is 1 && commands[0] is IParentNode pn)
    {
        output.AppendLine($"Found command group: {commands[0].Key} with {pn.Children.Count} children.");
        foreach (var child in pn.Children)
            output.AppendLine($"-   {child.Key} - {(child is IParentNode pnc ? $"Group ({pnc.Children.Count()} childen)" : "Command")}");
        
        output.AppendLine();
    }

    var groupedCommands = commands.GroupBy(c => c.Key).ToArray();

    if (groupedCommands.Length is 1)
    {
        if (groupedCommands[0].Count() > 1 && groupedCommands[0].FirstOrDefault(gc => gc is IParentNode) is IParentNode gpn)
        {
            for (var i = 0; i < gpn.Children.Count; i++)
            {
                var child = gpn.Children[i];
                output.AppendLine($"{child.Key} - Command (overload {i + 1} of {gpn.Children.Count})");
            }
        }
        else
        {
            foreach (var subCommand in groupedCommands[0])
            {
                output.AppendLine($"{subCommand.Key} - {(subCommand is IParentNode scpn ? $"Group ({scpn.Children.Count()} children)" : "Command")}");
            }
        }
    }
    else
    {
        foreach (var group in groupedCommands)
        {
            var groupArray = group.ToArray();
            
            if (groupArray.Length > 1 && groupArray.FirstOrDefault(g => g is IParentNode) is IParentNode gpn)
            {
                output.AppendLine($"{group.Key} - Group (executable) {gpn.Children.Count()} childen");
            }
            else if (groupArray.Length > 1)
            {
                for (int i = 0; i < groupArray.Length; i++)
                {
                    var child = groupArray[i];
                    output.AppendLine($"{child.Key} - Command (overload {i + 1} of {groupArray.Length})");
                }
            }
            else
            {
                output.AppendLine($"{group.Key} - {(groupArray[0] is IParentNode cpn ? $"Group ({cpn.Children.Count()} children)" : "Command")}");
            }
        }
    }

    
    Console.WriteLine(output);

}