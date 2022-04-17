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

        [Command("command")]
        public async Task<IResult> CommandOverload() => default;
    }
    
    [Group("g-1")]
    public class Group1 : CommandGroup
    {
        [Command("c-1")]
        public async Task<IResult> Command1() => default;
        
        [Command("c-2")]
        public async Task<IResult> Command2() => default;
    }
    
    [Group("g-1")]
    public class Group2 : CommandGroup
    {
        [Command("c-3")]
        public async Task<IResult> Command3() => default;
        
        [Command("c-4")]
        public async Task<IResult> Command4() => default;
    }
}