﻿using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Rinsen.Logger
{
    [ProviderAlias("QueueLogger")]
    public class QueueLoggerProvider : ILoggerProvider
    {
        private readonly Func<string, LogLevel, bool> _filter;
        private readonly ILogQueue _logQueue;
        private readonly ILogServiceClient _logServiceClient;
        private readonly LogOptions _options;

        private CancellationTokenSource _cancellationTokenSource;
        private Task _logHandlerTask;

        public QueueLoggerProvider(ILogQueue logQueue, ILogServiceClient logServiceClient, LogOptions options)
        {
            _logQueue = logQueue;
            _logServiceClient = logServiceClient;
            _options = options;
            _filter = (category, logLevel) => logLevel >= options.MinLevel && category.StartsWith("");
            _cancellationTokenSource = new CancellationTokenSource();
            _logHandlerTask = Task.Factory.StartNew(ProcessLogQueue, null, TaskCreationOptions.LongRunning);
        }

        public ILogger CreateLogger(string name)
        {
            return new Logger(name, _filter, _logQueue);
        }

        public void Dispose()
        {
            StopProcessing();

            _cancellationTokenSource?.Dispose();
        }

        private void StopProcessing()
        {
            _cancellationTokenSource.Cancel();

            try
            {
                _logHandlerTask.Wait(_options.TimeToSleepBetweenBatches);
            }
            catch (TaskCanceledException)
            {
            }
            catch (AggregateException ex) when (ex.InnerExceptions.Count == 1 && ex.InnerExceptions[0] is TaskCanceledException)
            {
            }
        }

        internal async Task ProcessLogQueue(object state)
        {
            while (!_cancellationTokenSource.IsCancellationRequested)
            {
                
                try
                {
                    var logs = _logQueue.GetReportedLogs();
                    if (logs.Any())
                    {
                        await _logServiceClient.ReportAsync(logs);
                    }
                }
                catch (Exception e)
                {
                    CreateLogger(GetType().FullName).LogError(e, "Failed to send log report");
                }

                await Task.Delay(_options.TimeToSleepBetweenBatches, _cancellationTokenSource.Token);
            }
        }
    }
}
