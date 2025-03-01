using Microsoft.Extensions.Configuration;
using System.Text.Json;
using PerakladBot.Commands;
using PerakladBot.Constants;
using TwitchLib.Client;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Interfaces;
using System.Net.Http.Headers;
using SpotifyAPI.Web;
using System.Web;
using System.Text;
using System.Text.RegularExpressions;


var basePath = System.IO.Directory.GetCurrentDirectory();
var configuration = new ConfigurationBuilder()
    .SetBasePath(basePath)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

var accessToken = String.Empty;
var expireTokenTimestamp = DateTime.UtcNow;

var httpClient = new HttpClient();

await RefreshAccessToken();



#region RefreshAccessToken method
async Task RefreshAccessToken()
{
    string tokenUrl = "https://accounts.spotify.com/api/token";

    var requestBody = new Dictionary<string, string>
    {
        { "grant_type", "refresh_token" },
        { "refresh_token", configuration["SpotifyRefreshToken"] },
        { "client_id", configuration["SpotifyClientID"] },
        { "client_secret", configuration["SpotifyClientSecret"] }
    };

    var content = new FormUrlEncodedContent(requestBody);

    try
    {
        HttpResponseMessage response = await httpClient.PostAsync(tokenUrl, content);

        if (response.IsSuccessStatusCode)
        {
            var responseBody = await response.Content.ReadAsStringAsync();
            using JsonDocument doc = JsonDocument.Parse(responseBody);
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", doc.RootElement.GetProperty("access_token").GetString());
            expireTokenTimestamp = DateTime.UtcNow.AddSeconds(3600);
        }
        else
        {
            Console.WriteLine($"Error refreshing token: {response.StatusCode}");
        }
    }
    catch (HttpRequestException e)
    {
        Console.WriteLine($"Request exception: {e.Message}");
    }
}

#endregion

void StartTwitchChatMonitoring()
{
    var credentials = new ConnectionCredentials(configuration["TwitchClientUsername"], configuration["TwitchClientToken"]);
    var client = new TwitchClient();
    client.Initialize(credentials, configuration["TargetChannelName"]);

    client.OnMessageReceived += async (sender, e) =>
    {
        if(DateTime.UtcNow > expireTokenTimestamp)
        {
            RefreshAccessToken();
        }

        var commandMatch = Regex.Match(e.ChatMessage.Message, @"!(\w+)(?:\s(.+))?");


        switch (commandMatch.Groups[1].Value)
        {
            case CommandNames.TranslationBelRu:
                //TranslationCommand.Translate(driver, client, e.ChatMessage, message[1], TranslationTypes.RusBel);
                client.SendMessage(client.JoinedChannels.First(), "не працуе пакуль ЛМАО :D :D :D :D");
                break;
            case CommandNames.SpotifySkip:
                await SpotifyCommands.Skip(httpClient);
                break;
            case CommandNames.SpotifyPrevious:
                await SpotifyCommands.Previous(httpClient);
                break;
            case CommandNames.SpotifyPlay:
                await SpotifyCommands.Play(httpClient, commandMatch.Groups[2].Value);
                break;

        }
    };

    client.Connect();
}


StartTwitchChatMonitoring();

Console.ReadKey();
