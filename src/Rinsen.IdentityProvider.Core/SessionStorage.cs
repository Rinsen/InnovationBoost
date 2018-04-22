using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Rinsen.IdentityProvider.Core
{
    public class SessionStorage : ISessionStorage
    {
        private string _connectionString;

        private const string _insertSql = @"INSERT INTO Sessions (
                                                SessionId,
                                                IdentityId,
                                                CorrelationId,
                                                LastAccess,
                                                Expires,
                                                SerializedTicket) 
                                            VALUES (
                                                @SessionId,
                                                @IdentityId,
                                                @CorrelationId,
                                                @LastAccess,
                                                @Expires,
                                                @SerializedTicket); 
                                            SELECT CAST(SCOPE_IDENTITY() as int)";

        private const string _getSql = @"SELECT 
                                            ClusteredId,
                                            SessionId,
                                            IdentityId,
                                            CorrelationId,
                                            LastAccess,
                                            Expires,
                                            SerializedTicket
                                        FROM 
                                            Sessions 
                                        WHERE 
                                            SessionId=@SessionId";

        private const string _deleteSql = @"DELETE FROM Sessions WHERE SessionId = @SessionId";

        private const string _updateSql = @"UPDATE Sessions SET
                                                LastAccess = @LastAccess,
                                                Expires = @Expires,
                                                SerializedTicket = @SerializedTicket
                                            WHERE
                                                SessionId=@SessionId AND IdentityId = @IdentityId";

        public SessionStorage(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task CreateAsync(Session session)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                try
                {
                    using (var command = new SqlCommand(_insertSql, connection))
                    {
                        command.Parameters.Add(new SqlParameter("@SessionId", session.SessionId));
                        command.Parameters.Add(new SqlParameter("@IdentityId", session.IdentityId));
                        command.Parameters.Add(new SqlParameter("@CorrelationId", session.CorrelationId));
                        command.Parameters.Add(new SqlParameter("@LastAccess", session.LastAccess));
                        command.Parameters.Add(new SqlParameter("@Expires", session.Expires));
                        command.Parameters.Add(new SqlParameter("@SerializedTicket", session.SerializedTicket));
                        connection.Open();

                        session.ClusteredId = (int)await command.ExecuteScalarAsync();
                    }
                }
                catch (SqlException ex)
                {
                    // 2601 - Violation in unique index
                    // 2627 - Violation in unique constraint
                    if (ex.Number == 2601 || ex.Number == 2627)
                    {
                        throw new SessionAlreadyExistException($"Session {session.SessionId} already exist while trying to create for user {session.IdentityId}", ex);
                    }
                    throw;
                }
            }
        }

        public async Task DeleteAsync(string sessionId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(_getSql, connection))
                {
                    command.Parameters.Add(new SqlParameter("@SessionId", sessionId));
                    connection.Open();
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<Session> GetAsync(string sessionId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(_getSql, connection))
                {
                    command.Parameters.Add(new SqlParameter("@SessionId", sessionId));
                    connection.Open();
                    var reader = await command.ExecuteReaderAsync();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            return MapSession(reader);
                        }
                    }
                }
            }

            return default(Session);
        }

        public async Task<IEnumerable<Session>> GetAsync(Guid identityId)
        {
            var result = new List<Session>();

            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(_getSql, connection))
                {
                    command.Parameters.Add(new SqlParameter("@SessionId", identityId));
                    connection.Open();
                    var reader = await command.ExecuteReaderAsync();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            result.Add(MapSession(reader));
                        }
                    }
                }
            }

            return result;
        }

        private static Session MapSession(SqlDataReader reader)
        {
            return new Session
            {
                ClusteredId = (int)reader["ClusteredId"],
                SessionId = (string)reader["SessionId"],
                IdentityId = (Guid)reader["IdentityId"],
                CorrelationId = (Guid)reader["CorrelationId"],
                LastAccess = (DateTimeOffset)reader["LastAccess"],
                Expires = (DateTimeOffset)reader["Expires"],
                SerializedTicket = (byte[])reader["SerializedTicket"]
            };
        }

        public async Task UpdateAsync(Session session)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(_updateSql, connection))
                {
                    command.Parameters.Add(new SqlParameter("@SessionId", session.SessionId));
                    command.Parameters.Add(new SqlParameter("@IdentityId", session.IdentityId));
                    command.Parameters.Add(new SqlParameter("@LastAccess", session.LastAccess));
                    command.Parameters.Add(new SqlParameter("@Expires", session.Expires));
                    command.Parameters.Add(new SqlParameter("@SerializedTicket", session.SerializedTicket));
                    connection.Open();

                    await command.ExecuteNonQueryAsync();
                }
            }
        }
    }
}
