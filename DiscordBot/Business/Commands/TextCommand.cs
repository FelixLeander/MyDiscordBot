﻿using Discord.Commands;
using DiscordBot.Business.Manager;
using DiscordBot.Models.Internal.Configs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Serilog;

namespace DiscordBot.Business.Commands;

public sealed class TextCommand([FromServices] IOptions<Configuration> options) : ModuleBase<SocketCommandContext>
{
    [Command("holobots")]
    public async Task HoloBotsAsync()
    {
        try
        {
            Log.Debug("Executing HoloBots.");

            var botId = Context.Client.CurrentUser.Id;

            if (botId == 1302467929761120347) // Kumo
            {
                var (imageUrl, imageIndex) = await new DanbooruManager(options.Value.Danbooru).GetRandomImageByTagAsync("+shiraori", "+solo");
                if (imageUrl == null)
                    await Context.Channel.SendMessageAsync("YOU WORM! You won't receive ANY images from me!");
                else
                    await Context.Channel.SendMessageAsync(imageUrl);
            }
            else if (botId == 995955672934006784) //Ina
            {
                var (imageUrl, imageIndex) = await new DanbooruManager(options.Value.Danbooru).GetRandomImageByTagAsync("+ninomae_ina'nis", "+solo");
                if (imageUrl == null)
                    await Context.Channel.SendMessageAsync("Sowy Tako <3, I, couldn't fetch any images, maybe next time^^");
                else
                    await Context.Channel.SendMessageAsync(imageUrl);
            }
            else
                throw new Exception($"Bot's don't match any id: {botId}");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Could not execute HoloBots.");
        }
    }

    private static int _punCounter = -1;
    [Command("InaPun")]
    public async Task InaPanAsync()
    {
        try
        {
            Log.Debug("Executing InaPun.");

            if (Context.Client.CurrentUser.Id != 995955672934006784) //Ina
                return;

            var (imageUrl, imageIndex) = await new DanbooruManager(options.Value.Danbooru).GetRandomImageByTagAsync(_punCounter, "+ninomae_ina'nis", "+pun");
            if (imageUrl == null)
            {
                _punCounter = ++imageIndex;
                await Context.Channel.SendMessageAsync("I'm not felling *pun*Tastic.");
            }
            else
                await Context.Channel.SendMessageAsync(imageUrl);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Could not execute InaPun.");
        }
    }
}
