using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Text;
using System.Text.Json;

namespace Rinsen.Logger.Service
{
    public class DatabaseLogReader : ILogReader
    {
        private readonly LogServiceOptions _options;

        public DatabaseLogReader(LogServiceOptions options)
        {
            _options = options;
        }
        
        public async Task<List<LogEnvironment>> GetLogEnvironmentsAsync()
        {
            var results = new List<LogEnvironment>();

            using (var connection = new SqlConnection(_options.ConnectionString))
            using (var command = new SqlCommand("SELECT Id, Name FROM LogEnvironments", connection))
            {
                await connection.OpenAsync();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (reader.HasRows)
                    {
                        while (await reader.ReadAsync())
                        {
                            results.Add(new LogEnvironment
                            {
                                Id = (int)reader["Id"],
                                Name = (string)reader["Name"]
                            });
                        }
                    }
                }
            }

            return results;
        }

        public async Task<List<LogSource>> GetLogSourcesAsync()
        {
            var results = new List<LogSource>();

            using (var connection = new SqlConnection(_options.ConnectionString))
            using (var command = new SqlCommand("SELECT Id, Name FROM LogSources", connection))
            {
                await connection.OpenAsync();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (reader.HasRows)
                    {
                        while (await reader.ReadAsync())
                        {
                            results.Add(new LogSource
                            {
                                Id = (int)reader["Id"],
                                Name = (string)reader["Name"]
                            });
                        }
                    }
                }
            }

            return results;
        }

        public async Task<List<LogApplication>> GetLogApplicationsAsync()
        {
            var results = new List<LogApplication>();

            using (var connection = new SqlConnection(_options.ConnectionString))
            using (var command = new SqlCommand("SELECT Id, ApplicationId, DisplayName FROM LogApplications", connection))
            {
                await connection.OpenAsync();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (reader.HasRows)
                    {
                        while (await reader.ReadAsync())
                        {
                            results.Add(new LogApplication
                            {
                                Id = (int)reader["Id"],
                                ApplicationId = (string)reader["ApplicationId"],
                                DisplayName = (string)reader["DisplayName"]
                            });
                        }
                    }
                }
            }

            return results;
        }

        public async Task<LogApplication> GetLogApplicationAsync(string applicationId)
        {
            using (var connection = new SqlConnection(_options.ConnectionString))
            using (var command = new SqlCommand("SELECT Id, ApplicationId, DisplayName FROM LogApplications WHERE ApplicationId = @applicationId", connection))
            {
                command.Parameters.Add(new SqlParameter("applicationId", applicationId));

                await connection.OpenAsync();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (reader.HasRows)
                    {
                        await reader.ReadAsync();

                        return new LogApplication
                        {
                            Id = (int)reader["Id"],
                            ApplicationId = (string)reader["ApplicationId"],
                            DisplayName = (string)reader["DisplayName"]
                        };
                    }

                    return default;
                }
            }
        }

        public async Task<IEnumerable<LogView>> GetLogsAsync(DateTimeOffset from, DateTimeOffset to, IEnumerable<int> logApplications, IEnumerable<int> logEnvironments, IEnumerable<int> logSources, IEnumerable<int> logLevels, int take = 10000)
        {
            var logs = new List<LogView>();
            var taken = 0;

            using (var connection = new SqlConnection(_options.ConnectionString))
            using (var command = new SqlCommand("", connection))
            {
                CreateCommandSqlAndParameters(command, from, to, logApplications, logEnvironments, logSources, logLevels);

                connection.Open();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (reader.HasRows)
                    {
                        while (await reader.ReadAsync() && taken < take)
                        {
                            logs.Add(new LogView
                            {
                                Id = (int)reader["Id"],
                                ApplicationName = (string)reader["DisplayName"],
                                EnvironmentName = (string)reader["EnvironmentName"],
                                LogLevel = (LogLevel)Enum.Parse(typeof(LogLevel), reader["LogLevel"].ToString()),
                                LogProperties = JsonSerializer.Deserialize<IEnumerable<LogProperty>>((string)reader["LogProperties"]),
                                MessageFormat = (string)reader["MessageFormat"],
                                RequestId = (string)reader["RequestId"],
                                SourceName = (string)reader["SourceName"],
                                Timestamp = (DateTimeOffset)reader["Timestamp"]
                            });
                            taken++;
                        }
                    }
                }
            }

            return logs;
        }

        private void CreateCommandSqlAndParameters(SqlCommand command, DateTimeOffset from, DateTimeOffset to, IEnumerable<int> logApplications, IEnumerable<int> logEnvironments, IEnumerable<int> sourceIds, IEnumerable<int> logLevels)
        {
            command.Parameters.Add(new SqlParameter("@from", from));
            command.Parameters.Add(new SqlParameter("@to", to));

            var sql = new StringBuilder(@"SELECT Logs.Id,
                                                        Logs.LogLevel,
                                                        Logs.LogProperties,
                                                        Logs.MessageFormat,
                                                        Logs.RequestId,
                                                        Logs.Timestamp,
                                                        App.DisplayName AS DisplayName,
                                                        Env.Name AS EnvironmentName,
														Src.Name AS SourceName
	                                                FROM Logs
                                                        JOIN LogApplications App ON Logs.ApplicationId = App.Id
                                                        JOIN LogEnvironments Env ON Logs.EnvironmentId = Env.Id
														JOIN LogSources Src ON Logs.SourceId = Src.Id
                                                    WHERE Logs.Timestamp > @from 
                                                        AND Logs.Timestamp < @to");

            var logAppCount = 0;
            foreach (var logApplication in logApplications)
            {
                command.Parameters.Add(new SqlParameter($"@la{logAppCount}", logApplication));

                if (logAppCount == 0)
                {
                    sql.Append($" AND Logs.ApplicationId IN (@la0");
                }
                else
                {
                    sql.Append($", @la{logAppCount}");
                }

                logAppCount++;
            }

            if (logAppCount > 0)
            {
                sql.Append(")");
            }

            var logEnvCount = 0;
            foreach (var logEnvironment in logEnvironments)
            {
                command.Parameters.Add(new SqlParameter($"@le{logEnvCount}", logEnvironment));

                if (logEnvCount == 0)
                {
                    sql.Append($" AND Logs.EnvironmentId IN (@le0");
                }
                else
                {
                    sql.Append($", @le{logEnvCount}");
                }

                logEnvCount++;
            }

            if (logEnvCount > 0)
            {
                sql.Append(")");
            }

            var sourceIdCount = 0;
            foreach (var sourceId in sourceIds)
            {
                command.Parameters.Add(new SqlParameter($"@si{sourceIdCount}", sourceId));

                if (sourceIdCount == 0)
                {
                    sql.Append($" AND Logs.SourceId IN (@si0");
                }
                else
                {
                    sql.Append($", @si{sourceIdCount}");
                }

                sourceIdCount++;
            }

            if (sourceIdCount > 0)
            {
                sql.Append(")");
            }

            var logLevelCount = 0;
            foreach (var logLevel in logLevels)
            {
                command.Parameters.Add(new SqlParameter($"@level{logLevelCount}", logLevel));

                if (logLevelCount == 0)
                {
                    sql.Append($" AND Logs.LogLevel IN (@level0");
                }
                else
                {
                    sql.Append($", @level{logLevelCount}");
                }

                logLevelCount++;
            }

            if (logLevelCount > 0)
            {
                sql.Append(")");
            }

            command.CommandText = sql.ToString();

        }
    }
}
