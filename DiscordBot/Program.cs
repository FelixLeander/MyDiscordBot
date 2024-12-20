﻿using System.Reflection;
using DiscordBot.Business.Bots;
using DiscordBot.Database;
using DiscordBot.Models.Database;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;

namespace DiscordBot;

internal static class Program
{
    private static async Task<int> Main()
    {
        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

        var audioClip = new AudioClip
        {
            DiscordUserId = 229720939078615040,
            CallCode = "xcode",
            FileName = @"E:\aras\ara-471.mp3",
        };
        var context = new DatabaseContext();
        context.Database.EnsureCreated();
        var data = context.AudioClips.FirstOrDefault(f => f.CallCode.Equals(audioClip.CallCode));
        if (data == null)
            context.AudioClips.Add(audioClip);
        context.SaveChanges();

        try
        {
            Log.Logger = new LoggerConfiguration()
            .WriteTo.Console(LogEventLevel.Verbose) //Default: Verbose
            .WriteTo.File("Log/log.txt", LogEventLevel.Information)
            .CreateLogger();

            Log.Information("Initialized logging.");

            var runningSpaceNamespace = typeof(RunningSpace.RunningSpace).Namespace?.Split('.').Last();
            if (string.IsNullOrWhiteSpace(runningSpaceNamespace))
            {
                Log.Warning("Invalid namespace.");
                return -3;
            }

            var runningSpace = Path.Combine(Environment.CurrentDirectory, runningSpaceNamespace);
            if (!Directory.Exists(runningSpace))
            {
                Log.Fatal("Invalid running space: '{invalidRunningSpace}'.", runningSpace);
                return -2;
            }

            Environment.CurrentDirectory = runningSpace;
            Log.Information($"Running in '{Environment.CurrentDirectory}'.");


            var configuration = new ConfigurationBuilder()
                .AddUserSecrets(Assembly.GetExecutingAssembly())
                .Build();

            await using var serviceProvider = new ServiceCollection()
                .AddSingleton<IConfiguration>(configuration)
                .AddScoped<TestingBot>()
                .AddScoped<KumoBot>()
                .BuildServiceProvider();


            //var testingBot = serviceProvider.GetRequiredService<TestingBot>();
            //var testing = await testingBot.StartAsync(serviceProvider);

            var testingBot = serviceProvider.GetRequiredService<TestingBot>();
            var resultTesting = await testingBot.StartAsync(serviceProvider, configuration["DiscordBot:Token"], configuration["DiscordBot:Name"]);

            //var kumoBot = serviceProvider.GetRequiredService<KumoBot>();
            //var resultKumo = await kumoBot.StartAsync(serviceProvider, configuration["DiscordBotTesting:Token"], configuration["DiscordBotTesting:Name"]);

            await Task.Delay(Timeout.Infinite);
            //do
            //{
            //    Console.WriteLine("Press 'q' to quit.");
            //} while (Console.ReadKey(true).Key != ConsoleKey.Q);
            Log.Information("Shutting down...");

            await testingBot.StopAsync();
            //await kumoBot.StopAsync();

            Log.Information("Ran to completion.");
            return 0;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Unexpected error. Ending application.");
            return -1;
        }
    }

    private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        if (e.ExceptionObject is Exception ex)
            Log.Error(ex, "Unhandled Exception.");
    }
}