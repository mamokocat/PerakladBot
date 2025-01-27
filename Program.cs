using TwitchLib.Client;
using TwitchLib.Client.Models;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using PerakladBot.Constants;

var options = new ChromeOptions();
options.AddArgument("--no-sandbox");
options.AddArgument("--disable-dev-shm-usage");
options.AddArgument("--remote-allow-origins=*");
options.AddArgument("--headless");

using IWebDriver driver = new ChromeDriver(options);

driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(120); 
driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(120); 

driver.Navigate().GoToUrl("https://www.skarnik.by/");

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddUserSecrets<Program>();

var app = builder.Build();

void StartTwitchChatMonitoring()
{
    var credentials = new ConnectionCredentials(builder.Configuration["TwitchClientUsername"], builder.Configuration["TwitchClientToken"]);
    var client = new TwitchClient();
    client.Initialize(credentials, builder.Configuration["TargetChannelName"]);

    client.OnMessageReceived += (sender, e) =>
    {
        var message = e.ChatMessage.Message.Split(' ');

        if (message.Length < 2 || !message[0].StartsWith('!'))
        {
            return;
        }

        switch (message[0].Substring(1))
        {
            case CommandNames.TranslationBelRu:
                //TranslationCommand.Translate(driver, client, e.ChatMessage, message[1], TranslationTypes.RusBel);
                client.SendMessage(client.JoinedChannels.First(), "lmao");
                break;

        }
    };

    client.Connect();
}

StartTwitchChatMonitoring();

app.Run();

