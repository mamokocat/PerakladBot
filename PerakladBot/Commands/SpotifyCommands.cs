using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using TwitchLib.Client.Models;
using TwitchLib.Client;
using PerakladBot.Constants;
using System.Text.RegularExpressions;

namespace PerakladBot.Commands
{
    public static class SpotifyCommands
    {
        public async static Task Skip(HttpClient client)
        {
            await client.PostAsync(Utils.SpotifyApiEndpoint + "me/player/next", null);
        }

        public async static Task Previous(HttpClient client)
        {
            await client.PostAsync(Utils.SpotifyApiEndpoint + "me/player/previous", null);

        }

        public async static Task Play(HttpClient client, string songLink)
        {
            var songId = Regex.Match(songLink, @"https://open.spotify.com/track/([\w]+)").Groups[1].Value;

            await client.PostAsync(Utils.SpotifyApiEndpoint + $"me/player/queue?uri={Uri.EscapeDataString(songId)}", null);

        }
    }
}
