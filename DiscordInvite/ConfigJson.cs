using System.Collections.Generic;
using DSharpPlus.Entities;
using Newtonsoft.Json;

namespace DiscordInvite
{
    public struct ConfigJson
    {
        [JsonProperty("token")]
        public string Token { get; private set; }
        [JsonProperty("prefix")]
        public string Prefix { get; private set; }

        [JsonProperty("inviteRole")]
        public Dictionary<string, ulong> InviteRole { get; set;}
    }
}