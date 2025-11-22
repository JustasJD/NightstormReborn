using Nightstorm.Bot;
using Nightstorm.Bot.Configuration;
using Nightstorm.Bot.Interfaces;
using Nightstorm.Bot.Services;

var builder = Host.CreateApplicationBuilder(args);

// Configure Discord bot options
builder.Services.Configure<DiscordBotOptions>(
    builder.Configuration.GetSection(DiscordBotOptions.SectionName));

// Register Discord bot service
builder.Services.AddSingleton<IDiscordBotService, DiscordBotService>();

// Register the worker service
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
