using Microsoft.AspNetCore.SignalR;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
}

builder.Services.AddCors(options =>
{
    options.AddPolicy(MyAllowSpecificOrigins, policy =>
    {
        //policy.WithOrigins(builder.Configuration["AlertPageUrl"] ?? string.Empty)
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});



var app = builder.Build();

app.UseCors(MyAllowSpecificOrigins);

app.MapHub<NotificationHub>("/notificationHub");

void StartTwitchChatMonitoring(IHubContext<NotificationHub> hubContext)
{
    var credentials = new ConnectionCredentials(builder.Configuration["TwitchClientUsername"], builder.Configuration["TwitchClientToken"]);
    var client = new TwitchClient();
    client.Initialize(credentials, builder.Configuration["TargetChannelName"]);

    client.OnMessageReceived += (sender, e) =>
    {
        var message = $"{e.ChatMessage.DisplayName}: {e.ChatMessage.Message}";
        _ = hubContext.Clients.All.SendAsync("ReceiveMessage", message);
    };

    client.Connect();
}

StartTwitchChatMonitoring(app.Services.GetRequiredService<IHubContext<NotificationHub>>());

app.Run();

public class NotificationHub : Hub
{
    public async Task SendMessage(string message)
    {
        await Clients.All.SendAsync("ReceiveMessage", message);
    }
}

