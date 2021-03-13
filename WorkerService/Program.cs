using System;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace WorkerService
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            const string logFileLocation = @"logs\log.txt";
            try
            {
                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Information()
                    .WriteTo.File(logFileLocation, rollingInterval: RollingInterval.Day)
                    .WriteTo.Console()
                    .CreateLogger();
                CreateHostBuilder(args).Build().Run();
                return 0;
            }
            catch (Exception e)
            {
                Log.Fatal(e, "Critical error");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .UseWindowsService()
                .ConfigureServices((_, services) => services.AddHostedService<Worker>());
    }
}