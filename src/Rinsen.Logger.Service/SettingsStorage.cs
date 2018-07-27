using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Rinsen.Logger.Service
{
    public class SettingsStorage
    {
        private readonly LogServiceOptions _options;

        public SettingsStorage(LogServiceOptions options)
        {
            _options = options;
        }
       
        public async Task Create(Setting setting)
        {
            string insertSql = @"INSERT INTO Settings (
                                    Accessed, IdentityId, Key, Value) 
                                 VALUES (
                                    @accessed, @identityId, @key, @value);
                                 SELECT 
                                    CAST(SCOPE_IDENTITY() as int)";

            using (var connection = new SqlConnection(_options.ConnectionString))
            using (var command = new SqlCommand(insertSql, connection))
            {
                command.Parameters.Add(new SqlParameter("@accessed", setting.Accessed));
                command.Parameters.Add(new SqlParameter("@identityId", setting.IdentityId));
                command.Parameters.Add(new SqlParameter("@key", setting.Key));
                command.Parameters.Add(new SqlParameter("@value", setting.Value));

                await connection.OpenAsync();

                setting.Id = (int)await command.ExecuteScalarAsync();
            }
        }

        public async Task Update(Setting setting)
        {
            string insertSql = @"UPDATE Settings SET Accessed = @accessed, Value = @value WHERE Id = @id";

            using (var connection = new SqlConnection(_options.ConnectionString))
            using (var command = new SqlCommand(insertSql, connection))
            {
                command.Parameters.Add(new SqlParameter("@id", setting.Id));
                command.Parameters.Add(new SqlParameter("@accessed", setting.Accessed));
                command.Parameters.Add(new SqlParameter("@value", setting.Value));

                await connection.OpenAsync();

                var count = await command.ExecuteNonQueryAsync();

                if (count != 1)
                {
                    throw new Exception($"Failed to update id {setting.Id} with count {count}");
                }
            }
        }

        public async Task<Setting> Get(string key, Guid identityId)
        {
            using (var connection = new SqlConnection(_options.ConnectionString))
            using (var command = new SqlCommand("SELECT Id, IdentityId, Key, Value, Accessed FROM Settings where Key = @key AND IdentityId = @identityId", connection))
            {
                command.Parameters.Add(new SqlParameter($"@key", key));
                command.Parameters.Add(new SqlParameter($"@identityId", identityId));

                await connection.OpenAsync();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (reader.HasRows)
                    {
                        await reader.ReadAsync();

                        return new Setting
                        {
                            Id = (int)reader["Id"],
                            IdentityId = Guid.Parse((string)reader["IdentityId"]),
                            Key = (string)reader["Key"],
                            Value = (string)reader["Value"],
                            Accessed = (DateTimeOffset)reader["Accessed"]
                        };
                    }
                }
            }

            return default(Setting);
        }
    }
}