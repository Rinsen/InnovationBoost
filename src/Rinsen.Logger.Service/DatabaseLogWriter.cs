using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text.Json;
using System.Threading.Tasks;

namespace Rinsen.Logger.Service
{
    public class DatabaseLogWriter : ILogWriter
    {
        private readonly LogServiceOptions _options;

        public DatabaseLogWriter(LogServiceOptions options)
        {
            _options = options;
        }

        public async Task<LogApplication> CreateLogApplicationAsync(string applicationId, string displayName)
        {
            string insertSql = @"INSERT INTO LogApplications (
                                    ApplicationId, DisplayName) 
                                 VALUES (
                                    @applicationId, @displayName);
                                 SELECT 
                                    CAST(SCOPE_IDENTITY() as int)";


            using (var connection = new SqlConnection(_options.ConnectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(insertSql, connection))
                {
                    command.Parameters.Add(new SqlParameter("@applicationId", applicationId));
                    command.Parameters.Add(new SqlParameter("@displayName", displayName));

                    return new LogApplication
                    {
                        Id = (int)await command.ExecuteScalarAsync(),
                        ApplicationId = applicationId,
                        DisplayName = displayName
                    };
                }
            }
        }

        public async Task<LogEnvironment> CreateLogEnvironmentAsync(string environmentName)
        {
            string insertSql = @"INSERT INTO LogEnvironments (
                                    Name) 
                                 VALUES (
                                    @Name);
                                 SELECT 
                                    CAST(SCOPE_IDENTITY() as int)";
                                

            using (var connection = new SqlConnection(_options.ConnectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(insertSql, connection))
                {
                    command.Parameters.Add(new SqlParameter("@Name", environmentName));
                    return new LogEnvironment
                    {
                        Id = (int)await command.ExecuteScalarAsync(),
                        Name = environmentName
                    };
                }
            }
        }

        public async Task<LogSource> CreateLogSourceAsync(string sourceName)
        {
            string insertSql = @"INSERT INTO LogSources (
                                    Name) 
                                 VALUES (
                                    @Name);
                                 SELECT 
                                    CAST(SCOPE_IDENTITY() as int)";


            using (var connection = new SqlConnection(_options.ConnectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(insertSql, connection))
                {
                    command.Parameters.Add(new SqlParameter("@Name", sourceName));
                    return new LogSource
                    {
                        Id = (int)await command.ExecuteScalarAsync(),
                        Name = sourceName
                    };
                }
            }
        }

        public async Task UpdateLogApplicationAsync(LogApplication logApplication)
        {
            string insertSql = @"UPDATE LogApplications
                                    SET DisplayName = @displayName
                                 WHERE
                                    Id = @id";


            using (var connection = new SqlConnection(_options.ConnectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(insertSql, connection))
                {
                    command.Parameters.Add(new SqlParameter("@id", logApplication.Id));
                    command.Parameters.Add(new SqlParameter("@displayName", logApplication.DisplayName));

                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task WriteLogsAsync(IEnumerable<Log> logs)
        {
            string insertSql = @"
                                INSERT INTO Logs (
                                    ApplicationId,
                                    SourceId, 
                                    EnvironmentId, 
                                    RequestId, 
                                    LogLevel, 
                                    MessageFormat, 
                                    LogProperties, 
                                    Timestamp) 
                                VALUES (
                                    @ApplicationId,
                                    @SourceId, 
                                    @EnvironmentId, 
                                    @RequestId, 
                                    @LogLevel, 
                                    @MessageFormat, 
                                    @LogProperties,
                                    @Timestamp);
                                SELECT 
                                    CAST(SCOPE_IDENTITY() as int)";

            using (var connection = new SqlConnection(_options.ConnectionString))
            {
                await connection.OpenAsync();
                foreach (var item in logs)
                {
                    using (var command = new SqlCommand(insertSql, connection))
                    {
                        command.Parameters.Add(new SqlParameter("@ApplicationId", item.ApplicationId));
                        command.Parameters.Add(new SqlParameter("@SourceId", item.SourceId));
                        command.Parameters.Add(new SqlParameter("@EnvironmentId", item.EnvironmentId));
                        command.Parameters.Add(new SqlParameter("@RequestId", item.RequestId));
                        command.Parameters.Add(new SqlParameter("@LogLevel", item.LogLevel));
                        command.Parameters.Add(new SqlParameter("@MessageFormat", item.MessageFormat));
                        var properties = JsonSerializer.Serialize(item.LogProperties);
                        command.Parameters.Add(new SqlParameter("@LogProperties", properties));
                        command.Parameters.Add(new SqlParameter("@Timestamp", item.Timestamp));

                        item.Id = (int) await command.ExecuteScalarAsync();
                    }
                }
            }
        }
    }
}
