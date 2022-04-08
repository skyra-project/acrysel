using System;
using System.Threading.Tasks;
using Acrysel.Extensions;
using Acrysel.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Remora.Commands.Extensions;
using Remora.Discord.API;
using Remora.Discord.Commands.Extensions;
using Remora.Discord.Commands.Services;
using Remora.Discord.Hosting.Extensions;
using Remora.Rest.Core;

var app = CreateApplication(args);

await UpdateSlashCommandsAsync(app);

app.UseHttpsRedirection();

app.MapControllers();

app.Run();

WebApplication CreateApplication(string[] arguments)
{
    var builder = WebApplication.CreateBuilder(arguments);

    builder.Services.AddControllers();
    builder.Services.AddSingleton<IYoutubeApiClient, YoutubeApiClient>();

    var host = builder.Configuration.GetValue<string?>("HOST") ?? "localhost";
    var port = builder.Configuration.GetValue<uint?>("PORT") ?? 5001;

    builder.WebHost.UseUrls($"http://{host}:{port}");

    builder.Host.AddDiscordService(services =>
        {
            var config = services.GetRequiredService<IConfiguration>();

            var token = config.GetValue<string?>("DISCORD_TOKEN");

            if (token is null)
            {
                throw new ArgumentException("Missing DISCORD_TOKEN in configuration.");
            }

            return token;
        })
        .ConfigureServices((_, services) =>
        {
            services
                .AddDiscordCommands(true)
                .AddCommandTree()
                .AddCommandsFromAssembly();
        })
        .ConfigureLogging(loggingConfig =>
        {
            loggingConfig
                .AddConsole()
                .AddFilter("System.Net.Http.HttpClient.*.LogicalHandler", LogLevel.Warning)
                .AddFilter("System.Net.Http.HttpClient.*.ClientHandler", LogLevel.Warning);
        });

    return builder.Build();
}

async Task UpdateSlashCommandsAsync(WebApplication application)
{
    var services = application.Services;

    var configuration = services.GetRequiredService<IConfiguration>();
    var slashService = services.GetRequiredService<SlashService>();
    var logger = services.GetRequiredService<ILogger<Program>>();

    var debugServer = default(Snowflake?);

#if DEBUG
    var debugServerString = configuration.GetValue<string?>("DEBUG_SERVER");

    if (debugServerString is null)
    {
        logger.LogWarning("No debug server specified when running in development mode");
    }
    else
    {
        if (DiscordSnowflake.TryParse(debugServerString, out debugServer))
        {
            logger.LogInformation("Debug server set to {DebugServer}", debugServer);
        }
        else
        {
            logger.LogWarning("Invalid snowflake for debug server specified: {DebugServer}", debugServer);
        }
    }
#endif

    var updateSlashCommandsResult = await slashService.UpdateSlashCommandsAsync(debugServer);

    if (!updateSlashCommandsResult.IsSuccess)
    {
        logger.LogError("Failed to update slash commands: {UpdateSlashCommandsError}",
            updateSlashCommandsResult.Inner!.Error);
    }
}