using Remora.Commands.Attributes;
using Remora.Commands.Groups;
using Remora.Results;

namespace HelpSystemPOC;

public class Commands : CommandGroup
{
    [Command("top-level")]
    public async Task<IResult> TopLevelCommand() => default;
    
    [Command("group")]
    public async Task<IResult> GroupCommand() => default;
    
    [Group("group")]
    public class Group : CommandGroup
    {
        [Command("command")]
        public async Task<IResult> Command() => default;
    }
}