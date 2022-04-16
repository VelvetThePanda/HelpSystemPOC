using Remora.Commands.Services;
using Remora.Commands.Trees.Nodes;
using Remora.Results;

namespace HelpSystemPOC;

public class HelpGenerator
{
    private readonly CommandService _commands;
    
    public HelpGenerator(CommandService commands)
    {
        _commands = commands;
    }

    public Result<IEnumerable<IChildNode>> GetMatchingCommands(string command, string? treeName = null)
    {
        if (!_commands.TreeAccessor.TryGetNamedTree(treeName, out var tree))
            return Result<IEnumerable<IChildNode>>.FromError(new NotFoundError());

        if (string.IsNullOrEmpty(command))
        {
            var res = FindCommandsByName(new(), tree.Root).ToArray();
            
            if (!res.Any())
                return Result<IEnumerable<IChildNode>>.FromError(new NotFoundError());
            
            return Result<IEnumerable<IChildNode>>.FromSuccess(res);
        }
        else
        {
            var tokenized = new Stack<string>(command.Split(' ').Reverse());
            var res = FindCommandsByName(tokenized, tree.Root).ToArray();
            
            if (!res.Any())
                return Result<IEnumerable<IChildNode>>.FromError(new NotFoundError());
            
            return Result<IEnumerable<IChildNode>>.FromSuccess(res);
        }
        
        return default;
    }

    private IEnumerable<IChildNode> FindCommandsByName(Stack<string> tokens, IParentNode parent)
    {
        if (!tokens.TryPop(out var token)) //We don't return an empty stack, so this must be from the caller
            foreach (var child in parent.Children)
                yield return child;

        IParentNode? nested = null;
        
        foreach (var child in parent.Children)
        {
            if (
                child.Key.Equals(token, StringComparison.OrdinalIgnoreCase) ||
                child.Aliases.Any(a => a.Equals(token, StringComparison.OrdinalIgnoreCase))
               )
            {
                if (TokensRemain() && child is IParentNode nestedParent)
                {
                    nested = nestedParent;
                    break;
                }
                else
                {
                    yield return child;
                }
            }
        }

        if (!TokensRemain() || nested is null)
            yield break;

        foreach (var child in FindCommandsByName(tokens, nested!))
            yield return child;
        
        yield break;
        
        bool TokensRemain() => tokens.TryPeek(out _);
    }
    
}