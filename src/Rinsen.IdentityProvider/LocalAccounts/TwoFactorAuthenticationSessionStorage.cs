using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace Rinsen.IdentityProvider.LocalAccounts
{
    public class TwoFactorAuthenticationSessionStorage : ITwoFactorAuthenticationSessionStorage
    {
        private readonly IdentityOptions _identityOptions;

        const string _insertSql = @"INSERT INTO TwoFactorAuthenticationSessions 
                                        (Created,
                                        Deleted,
                                        IdentityId,
                                        KeyCode,
                                        SessionId,
                                        Type,
                                        Updated) 
                                    VALUES 
                                        (@Created,
                                        @Deleted,
                                        @IdentityId,
                                        @KeyCode,
                                        @SessionId,
                                        @Type,
                                        @Updated); 
                                    SELECT CAST(SCOPE_IDENTITY() as int)";

        const string _selectWithSessionId = @"SELECT Id,
                                        Created,
                                        Deleted,
                                        IdentityId,
                                        KeyCode,
                                        SessionId,
                                        Type,
                                        Updated
                                    FROM 
                                        TwoFactorAuthenticationSessions 
                                    WHERE 
                                        SessionId = @SessionId";

        const string _update = @"UPDATE 
                                    TwoFactorAuthenticationSessions 
                                SET 
                                    Deleted = @Deleted,
                                    KeyCode = @KeyCode,
                                    Type = @Type,
                                    Updated = @Updated
                                WHERE 
                                    Id = @Id";

        public TwoFactorAuthenticationSessionStorage(IdentityOptions identityOptions)
        {
            _identityOptions = identityOptions;
        }

        public async Task Create(TwoFactorAuthenticationSession twoFactorAuthenticationSession)
        {
            using (var connection = new SqlConnection(_identityOptions.ConnectionString))
            using (var command = new SqlCommand(_insertSql, connection))
            {
                command.Parameters.AddWithValue("@Created", twoFactorAuthenticationSession.Created);
                command.Parameters.AddWithNullableValue("@Deleted", twoFactorAuthenticationSession.Deleted);
                command.Parameters.AddWithValue("@IdentityId", twoFactorAuthenticationSession.IdentityId);
                command.Parameters.AddWithValue("@KeyCode", twoFactorAuthenticationSession.KeyCode);
                command.Parameters.AddWithValue("@SessionId", twoFactorAuthenticationSession.SessionId);
                command.Parameters.AddWithValue("@Type", twoFactorAuthenticationSession.Type);
                command.Parameters.AddWithValue("@Updated", twoFactorAuthenticationSession.Updated);

                await connection.OpenAsync();

                twoFactorAuthenticationSession.Id = (int)await command.ExecuteScalarAsync();
            }
        }

        public Task Delete(TwoFactorAuthenticationSession twoFactorAuthenticationSession)
        {
            twoFactorAuthenticationSession.Deleted = DateTimeOffset.Now;

            return Update(twoFactorAuthenticationSession);
        }

        public async Task<TwoFactorAuthenticationSession> Get(string sessionId)
        {
            using (var connection = new SqlConnection(_identityOptions.ConnectionString))
            using (var command = new SqlCommand(_selectWithSessionId, connection))
            {
                command.Parameters.AddWithValue("@SessionId", sessionId);

                await connection.OpenAsync();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (reader.HasRows)
                    {
                        while (await reader.ReadAsync())
                        {
                            return new TwoFactorAuthenticationSession
                            {
                                Created = (DateTimeOffset)reader["Created"],
                                IdentityId = (Guid)reader["IdentityId"],
                                Id = (int)reader["Id"],
                                Updated = (DateTimeOffset)reader["Updated"],
                                Deleted = reader.GetValueOrDefault<DateTimeOffset?>("Deleted"),
                                KeyCode = (string)reader["KeyCode"],
                                SessionId = (string)reader["SessionId"],
                                Type = (TwoFactorType)Enum.Parse(typeof(TwoFactorType), Convert.ToString(reader["Type"]))
                            };
                        }
                    }
                }
            }

            return default;
        }

        public async Task Update(TwoFactorAuthenticationSession twoFactorAuthenticationSession)
        {
            using (var connection = new SqlConnection(_identityOptions.ConnectionString))
            using (var command = new SqlCommand(_update, connection))
            {
                command.Parameters.AddWithValue("@Deleted", twoFactorAuthenticationSession.Deleted);
                command.Parameters.AddWithValue("@KeyCode", twoFactorAuthenticationSession.KeyCode);
                command.Parameters.AddWithValue("@Type", twoFactorAuthenticationSession.Type);
                command.Parameters.AddWithValue("@Updated", twoFactorAuthenticationSession.Updated);
                command.Parameters.AddWithValue("@Id", twoFactorAuthenticationSession.Id);

                await connection.OpenAsync();

                var result = await command.ExecuteNonQueryAsync();
            }
        }
    }
}
