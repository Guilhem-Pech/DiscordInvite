using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using DiscordInvite.Commands;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace DiscordInvite
{
    public class Bot
    {
        public DiscordClient Client { get; private set; }
        public CommandsNextExtension Commands { get; private set; }

        public Dictionary<string, int> invitesCount; 
            
        public async Task RunAsync()
        {
            ConfigJson configJson = await GetConfig();
            
            DiscordConfiguration config = new DiscordConfiguration
            {
                Token = configJson.Token, 
                TokenType = TokenType.Bot,
                AutoReconnect = true,
                MinimumLogLevel = LogLevel.Debug,
            };
            Client = new DiscordClient(config);
            Client.Ready += OnClientReady;
            Client.GuildMemberAdded += OnMemberAdded;
            
            CommandsNextConfiguration commandsConfig = new CommandsNextConfiguration
            {
                StringPrefixes = new string[] {configJson.Prefix},
                EnableMentionPrefix = true,
                EnableDms = false
            };

            Commands = Client.UseCommandsNext(commandsConfig);
            Commands.RegisterCommands<DummyCommands>();
            
            await Client.ConnectAsync();
            await Task.Delay(-1);
        }

        private async Task OnMemberAdded(DiscordClient sender, GuildMemberAddEventArgs e)
        {
            ConfigJson config = await GetConfig();
            foreach (DSharpPlus.Entities.DiscordInvite discordInvite in await e.Guild.GetInvitesAsync())
            {
                if (discordInvite.Uses != invitesCount[discordInvite.Code])
                {
                    await e.Member.GrantRoleAsync(e.Guild.GetRole(config.InviteRole[discordInvite.Code]));
                    invitesCount[discordInvite.Code] = discordInvite.Uses;
                    return;
                }
            }
        }

        public static async Task<ConfigJson> GetConfig()
        {
            var json = string.Empty;
            await using(FileStream fs = File.OpenRead("config.json"))
            using (StreamReader sr = new StreamReader(fs, new UTF8Encoding(false))) 
                json = await sr.ReadToEndAsync().ConfigureAwait(false);
            return JsonConvert.DeserializeObject<ConfigJson>(json);
        }
        public static async Task UpdateConfig(ConfigJson configJson)
        {
            string t = JsonConvert.SerializeObject(configJson);
            await File.WriteAllTextAsync("config.json", t);
        }
        private async Task OnClientReady(DiscordClient sender, ReadyEventArgs e)
        {
            invitesCount = new Dictionary<string, int>();
            foreach (var guild in sender.Guilds)
            {
                IReadOnlyList<DSharpPlus.Entities.DiscordInvite> invites = await guild.Value.GetInvitesAsync();
                foreach (var discordInvite in invites)
                {
                    if (!invitesCount.TryAdd(discordInvite.Code, discordInvite.Uses))
                    {
                        invitesCount[discordInvite.Code] = discordInvite.Uses;
                    }
                }
            }
        }
    }
}