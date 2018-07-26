using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using System;
//using Microsoft.AspNetCore.Builder;

namespace Rinsen.Logger.Service
{
    public static class LogServiceExtensions
    {
        public static void AddLoggerService(this ILoggingBuilder loggingBuilder, IConfiguration configuration, string environmentName)
        {
            loggingBuilder.AddLoggerService(options =>
            {
                options.ApplicationLogKey = configuration["Rinsen:ApplicationKey"];
                options.LogServiceUri = configuration["Rinsen:InnovationBoost"];
                options.ConnectionString = configuration["Rinsen:ConnectionString"];
                options.EnvironmentName = environmentName;
            });
        }

        public static void AddLoggerService(this ILoggingBuilder loggingBuilder, Action<LogServiceOptions> logServiceOptionsAction)
        {
            var logOptions = new LogServiceOptions();

            logServiceOptionsAction.Invoke(logOptions);
                        
            loggingBuilder.Services.AddSingleton(logOptions);
            loggingBuilder.Services.AddSingleton<LogOptions>(logOptions);

            loggingBuilder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILogQueue, LogQueue>());
            loggingBuilder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, QueueLoggerProvider>());
            loggingBuilder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILogServiceClient, LogServiceClient>());

            loggingBuilder.Services.AddScoped<ILogReader, DatabaseLogReader>();
            loggingBuilder.Services.AddScoped<ILogWriter, DatabaseLogWriter>();
            loggingBuilder.Services.AddScoped<LogHandler, LogHandler>();
            loggingBuilder.Services.AddScoped<ILogEnvironmentHandler, LogEnvironmentHandler>();
            loggingBuilder.Services.AddScoped<ISourceHandler, SourceHandler>();
        }
    }
}
