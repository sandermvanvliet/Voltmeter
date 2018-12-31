﻿using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Voltmeter.UI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var logger = ConfigureLogger();

            CreateWebHostBuilder(args, logger)
                .Build()
                .Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args, ILogger logger)
        {
            var environment = System.Environment.GetEnvironmentVariable("ENVIRONMENT") ?? "Development";

            return new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(System.Environment.CurrentDirectory)
                .ConfigureAppConfiguration(
                    configurationBuilder => configurationBuilder
                        .AddJsonFile("appsettings.json")
                        .AddJsonFile($"appsettings.{environment}.json", optional:true)
                        .AddEnvironmentVariables())
                .ConfigureServices(serviceCollection => serviceCollection.AddSingleton(logger))
                .UseStartup<Startup>();
        }

        private static ILogger ConfigureLogger()
        {
            var logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();

            Log.Logger = logger;

            return logger;
        }
    }
}