using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

namespace DiscordInvite.Commands
{
    public class DummyCommands : BaseCommandModule
    {
        [Command("ping")]
        public async Task Ping(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync("Pong").ConfigureAwait(false);
        }

        [Command("add")]
        public async Task Add(CommandContext ctx, int a, int b)
        {
            await ctx.Channel.SendMessageAsync($"{a} + {b} = {a + b}")
                .ConfigureAwait(false);
        }
        
        [Command("getRoleID")]
        public async Task GetRoleID(CommandContext ctx, string role)
        {
            await ctx.Channel.SendMessageAsync(ctx.Guild.Roles.First(x => x.Value.Name == role).Key.ToString())
                .ConfigureAwait(false);
        }
        
        [Command("AddInvite")]
        public async Task AddRole(CommandContext ctx, DiscordRole role, string invite)
        {
            var config = await Bot.GetConfig();
            config.InviteRole ??= new Dictionary<string, ulong>();
            if(role != null)
            {
                if(!config.InviteRole.TryAdd(invite,role.Id)){
                    config.InviteRole[invite] = role.Id;
                }
            }

            await Bot.UpdateConfig(config);
            await ctx.Channel.SendMessageAsync("Added role");
        }
    }
}