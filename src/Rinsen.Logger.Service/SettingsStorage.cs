using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Rinsen.Logger.Service
{
    public class SettingsStorage : ISettingsStorage
    {
        private readonly LogServiceOptions _options;

        public SettingsStorage(LogServiceOptions options)
        {
            _options = options;
        }
       
        public async Task Create(Setting setting)
        {
            string insertSql = @"INSERT INTO Settings (
                                    Accessed, IdentityId, KeyField, ValueField) 
                                 VALUES (
                                    @accessed, @identityId, @keyfield, @valuefield);
                                 SELECT 
                                    CAST(SCOPE_IDENTITY() as int)";

            using (var connection = new SqlConnection(_options.ConnectionString))
            using (var command = new SqlCommand(insertSql, connection))
            {
                command.Parameters.Add(new SqlParameter("@accessed", setting.Accessed));
                command.Parameters.Add(new SqlParameter("@identityId", setting.IdentityId));
                command.Parameters.Add(new SqlParameter("@keyfield", setting.KeyField));
                command.Parameters.Add(new SqlParameter("@valuefield", setting.ValueField));

                await connection.OpenAsync();

                setting.Id = (int)await command.ExecuteScalarAsync();
            }
        }

        public async Task Update(Setting setting)
        {
            string insertSql = @"UPDATE Settings SET Accessed = @accessed, ValueField = @valuefield WHERE Id = @id";

            using (var connection = new SqlConnection(_options.ConnectionString))
            using (var command = new SqlCommand(insertSql, connection))
            {
                command.Parameters.Add(new SqlParameter("@id", setting.Id));
                command.Parameters.Add(new SqlParameter("@accessed", setting.Accessed));
                command.Parameters.Add(new SqlParameter("@valuefield", setting.ValueField));

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
            using (var command = new SqlCommand("SELECT Id, IdentityId, KeyField, ValueField, Accessed FROM Settings where KeyField = @keyfield AND IdentityId = @identityId", connection))
            {
                command.Parameters.Add(new SqlParameter($"@keyfield", key));
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
                            IdentityId = (Guid)reader["IdentityId"],
                            KeyField = (string)reader["KeyField"],
                            ValueField = (string)reader["ValueField"],
                            Accessed = (DateTimeOffset)reader["Accessed"]
                        };
                    }
                }
            }

            return default(Setting);
        }
    }
}