﻿using Discord;
using Discord.Commands;

namespace DiscordBot.Business.Helpers;

internal static class DiscordExtensions
{
    internal static IVoiceChannel? GetVoiceChannel(this ModuleBase<SocketCommandContext> moduleBase)
        => (moduleBase.Context.User as IGuildUser)?.VoiceChannel;
}
