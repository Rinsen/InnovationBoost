using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Rinsen.IdentityProvider.Core
{
    public class SessionStorage : ISessionStorage
    {
        private string _connectionString;

        private const string InsertSql = @"INSERT INTO Sessions (
                                                SessionId,
                                                IdentityId,
                                                CorrelationId,
                                                IpAddress,
                                                UserAgent,
                                                Created,
                                                Updated,
                                                Deleted,
                                                Expires,
                                                SerializedTicket) 
                                            VALUES (
                                                @SessionId,
                                                @IdentityId,
                                                @CorrelationId,
                                                @IpAddress,
                                                @UserAgent,
                                                @Created,
                                                @Updated,
                                                @Deleted,
                                                @Expires,
                                                @SerializedTicket); 
                                            SELECT CAST(SCOPE_IDENTITY() as int)";

        private const string GetSql = @"SELECT 
                                            ClusteredId,
                                            SessionId,
                                            IdentityId,
                                            CorrelationId,
                                            IpAddress,
                                            UserAgent,
                                            Created,
                                            Updated,
                                            Deleted,
                                            Expires,
                                            SerializedTicket
                                        FROM 
                                            Sessions 
                                        WHERE 
                                            SessionId=@SessionId
                                            AND Deleted is null";

        private const string DeleteSql = @"UPDATE Sessions SET Deleted = @Deleted WHERE SessionId = @SessionId";

        private const string UpdateSql = @"UPDATE Sessions SET
                                                Updated = @Updated,
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
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                using (var command = new SqlCommand(InsertSql, connection))
                {
                    command.Parameters.Add(new SqlParameter("@SessionId", session.SessionId));
                    command.Parameters.Add(new SqlParameter("@IdentityId", session.IdentityId));
                    command.Parameters.Add(new SqlParameter("@CorrelationId", session.CorrelationId));
                    command.Parameters.Add(new SqlParameter("@IpAddress", session.IpAddress));
                    command.Parameters.Add(new SqlParameter("@UserAgent", session.UserAgent));
                    command.Parameters.Add(new SqlParameter("@Created", session.Created));
                    command.Parameters.Add(new SqlParameter("@Updated", session.Updated));
                    command.Parameters.Add(new SqlParameter("@Deleted", System.Data.SqlDbType.DateTimeOffset));
                    if (session.Deleted == null)
                    {
                        command.Parameters["@Deleted"].Value = DBNull.Value;
                    }
                    else
                    {
                        command.Parameters["@Deleted"].Value = session.Deleted;
                    }
                    command.Parameters.Add(new SqlParameter("@Expires", session.Expires));
                    command.Parameters.Add(new SqlParameter("@SerializedTicket", session.SerializedTicket));
                    await connection.OpenAsync();

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

        public async Task DeleteAsync(string sessionId)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(DeleteSql, connection))
            {
                command.Parameters.Add(new SqlParameter("@SessionId", sessionId));
                command.Parameters.Add(new SqlParameter("@Deleted", DateTimeOffset.Now));

                await connection.OpenAsync();
                var count = await command.ExecuteNonQueryAsync();
            }
        }

        public async Task<Session> GetAsync(string sessionId)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(GetSql, connection))
            {
                command.Parameters.Add(new SqlParameter("@SessionId", sessionId));

                await connection.OpenAsync();

                var reader = await command.ExecuteReaderAsync();

                if (reader.HasRows)
                {
                    while (await reader.ReadAsync())
                    {
                        return MapSession(reader);
                    }
                }
            }

            return default;
        }

        public async Task<IEnumerable<Session>> GetAsync(Guid identityId)
        {
            var result = new List<Session>();

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(GetSql, connection))
            {
                command.Parameters.Add(new SqlParameter("@SessionId", identityId));

                await connection.OpenAsync();

                var reader = await command.ExecuteReaderAsync();

                if (reader.HasRows)
                {
                    while (await reader.ReadAsync())
                    {
                        result.Add(MapSession(reader));
                    }
                }
            }

            return result;
        }

        private static Session MapSession(SqlDataReader reader)
        {
            DateTimeOffset? deleted = null;
            if (reader["Deleted"] != DBNull.Value)
            {
                deleted = (DateTimeOffset?)reader["Deleted"];
            }

            return new Session
            {
                ClusteredId = (int)reader["ClusteredId"],
                SessionId = (string)reader["SessionId"],
                IdentityId = (Guid)reader["IdentityId"],
                CorrelationId = (Guid)reader["CorrelationId"],
                IpAddress = (string)reader["IpAddress"],
                UserAgent = (string)reader["UserAgent"],
                Created = (DateTimeOffset)reader["Created"],
                Updated = (DateTimeOffset)reader["Updated"],
                Deleted = deleted,
                Expires = (DateTimeOffset)reader["Expires"],
                SerializedTicket = (byte[])reader["SerializedTicket"]
            };
        }

        public async Task UpdateAsync(Session session)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(UpdateSql, connection))
            {
                command.Parameters.Add(new SqlParameter("@SessionId", session.SessionId));
                command.Parameters.Add(new SqlParameter("@IdentityId", session.IdentityId));
                command.Parameters.Add(new SqlParameter("@Updated", session.Updated));
                command.Parameters.Add(new SqlParameter("@Expires", session.Expires));
                command.Parameters.Add(new SqlParameter("@SerializedTicket", session.SerializedTicket));
                await connection.OpenAsync();

                await command.ExecuteNonQueryAsync();
            }
        }
    }
}
