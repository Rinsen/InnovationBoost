using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Text;
using Newtonsoft.Json;

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

        public async Task<Dictionary<string, int>> GetLogEnvironmentIdsAsync()
        {
            var results = new Dictionary<string, int>();

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
                            results.Add((string)reader["Name"], (int)reader["Id"]);
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

        public async Task<Dictionary<string, int>> GetLogSourceIdsAsync()
        {
            var results = new Dictionary<string, int>();

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
                            results.Add((string)reader["Name"], (int)reader["Id"]);
                        }
                    }
                }
            }

            return results;
        }

        public async Task<IEnumerable<LogView>> GetLogsAsync(DateTimeOffset from, DateTimeOffset to, IEnumerable<int> logApplications, IEnumerable<int> logEnvironments, IEnumerable<int> logSources, IEnumerable<int> logLevels, int take = 200)
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
                                ApplicationName = (string)reader["ExtName"],
                                EnvironmentName = (string)reader["EnvironmentName"],
                                LogLevel = (LogLevel)Enum.Parse(typeof(LogLevel), reader["LogLevel"].ToString()),
                                LogProperties = JsonConvert.DeserializeObject<IEnumerable<LogProperty>>((string)reader["LogProperties"]),
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
                                                        ExtApp.Name AS ExtName,
                                                        Env.Name AS EnvironmentName,
														Src.Name AS SourceName
	                                                FROM Logs
                                                        JOIN ExternalApplications ExtApp ON Logs.ApplicationId = ExtApp.Id
                                                        JOIN LogEnvironments Env ON Logs.EnvironmentId = Env.Id
														JOIN LogSources Src ON Logs.SourceId = Src.Id
                                                    WHERE Logs.Timestamp > @from 
                                                        AND Logs.Timestamp < @to 
                                                        AND Logs.ApplicationId IN (");

            var count = 0;
            foreach (var logApplication in logApplications)
            {
                command.Parameters.Add(new SqlParameter($"@la{count}", logApplication));
                if (count == 0)
                {
                    sql.Append($"@la{count}");
                }
                else
                {
                    sql.Append($", @la{count}");
                }
                count++;
            }

            count = 0;
            sql.Append(") AND Logs.EnvironmentId IN (");
            foreach (var logEnvironment in logEnvironments)
            {
                command.Parameters.Add(new SqlParameter($"@le{count}", logEnvironment));
                if (count == 0)
                {
                    sql.Append($"@le{count}");
                }
                else
                {
                    sql.Append($", @le{count}");
                }
                count++;
            }

            count = 0;
            sql.Append(") AND Logs.SourceId IN (");
            foreach (var sourceId in sourceIds)
            {
                command.Parameters.Add(new SqlParameter($"@si{count}", sourceId));
                if (count == 0)
                {
                    sql.Append($"@si{count}");
                }
                else
                {
                    sql.Append($", @si{count}");
                }
                count++;
            }

            count = 0;
            sql.Append(") AND Logs.LogLevel IN (");
            foreach (var logLevel in logLevels)
            {
                command.Parameters.Add(new SqlParameter($"@level{count}", logLevel));
                if (count == 0)
                {
                    sql.Append($"@level{count}");
                }
                else
                {
                    sql.Append($", @level{count}");
                }
                count++;
            }
            sql.Append(")");

            command.CommandText = sql.ToString();

        }
    }
}
