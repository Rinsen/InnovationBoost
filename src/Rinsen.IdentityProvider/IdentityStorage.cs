using Rinsen.IdentityProvider;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Rinsen.IdentityProvider
{
    public class IdentityStorage : IIdentityStorage
    {
        private readonly IdentityOptions _identityOptions;

        private const string _createSql = @"INSERT INTO Identities (
                                                Created,
                                                Email,
                                                EmailConfirmed,
                                                GivenName,
                                                IdentityId,
                                                Surname,
                                                PhoneNumber,
                                                PhoneNumberConfirmed,
                                                Updated) 
                                            VALUES (
                                                @Created,
                                                @Email,
                                                @EmailConfirmed,
                                                @GivenName,
                                                @IdentityId,
                                                @Surname,
                                                @PhoneNumber,
                                                @PhoneNumberConfirmed,
                                                @Updated); 
                                            SELECT CAST(SCOPE_IDENTITY() as int)";

        private const string _getSql = @"SELECT 
                                            Created,
                                            Email,
                                            EmailConfirmed,
                                            GivenName, 
                                            ClusteredId, 
                                            IdentityId, 
                                            Surname,
                                            PhoneNumber, 
                                            PhoneNumberConfirmed, 
                                            Updated 
                                        FROM 
                                            Identities 
                                        WHERE 
                                            IdentityId=@IdentityId";

        public IdentityStorage(IdentityOptions identityOptions)
        {
            _identityOptions = identityOptions;
        }

        public async Task CreateAsync(Identity identity)
        {
            using (var connection = new SqlConnection(_identityOptions.ConnectionString))
            {
                try
                {
                    using (var command = new SqlCommand(_createSql, connection))
                    {
                        command.Parameters.Add(new SqlParameter("@Created", identity.Created));
                        command.Parameters.Add(new SqlParameter("@Email", identity.Email));
                        command.Parameters.Add(new SqlParameter("@EmailConfirmed", identity.EmailConfirmed));
                        command.Parameters.Add(new SqlParameter("@GivenName", identity.GivenName));
                        command.Parameters.Add(new SqlParameter("@IdentityId", identity.IdentityId));
                        command.Parameters.Add(new SqlParameter("@Surname", identity.Surname));
                        command.Parameters.Add(new SqlParameter("@PhoneNumber", identity.PhoneNumber));
                        command.Parameters.Add(new SqlParameter("@PhoneNumberConfirmed", identity.PhoneNumberConfirmed));
                        command.Parameters.Add(new SqlParameter("@Updated", identity.Updated));
                        
                        connection.Open();

                        identity.ClusteredId = (int)await command.ExecuteScalarAsync();
                    }
                }
                catch (SqlException ex)
                {
                    if (ex.Number == 2601 || ex.Number == 2627)
                    {
                        throw new IdentityAlreadyExistException($"Identity {identity.IdentityId} already exist", ex);
                    }
                    throw;
                }
            }
        }
        
        public async Task<Identity> GetAsync(Guid identityId)
        {
            using (var connection = new SqlConnection(_identityOptions.ConnectionString))
            {
                using (var command = new SqlCommand(_getSql, connection))
                {
                    command.Parameters.Add(new SqlParameter("@IdentityId", identityId));
                    connection.Open();
                    var reader = await command.ExecuteReaderAsync();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            return new Identity
                            {
                                Created = (DateTimeOffset)reader["Created"],
                                Email = (string)reader["Email"],
                                EmailConfirmed = (bool)reader["EmailConfirmed"],
                                GivenName = (string)reader["GivenName"],
                                Surname = (string)reader["Surname"],
                                ClusteredId = (int)reader["ClusteredId"],
                                IdentityId = (Guid)reader["IdentityId"],
                                PhoneNumber = (string)reader["PhoneNumber"],
                                PhoneNumberConfirmed = (bool)reader["PhoneNumberConfirmed"],
                                Updated = (DateTimeOffset)reader["Updated"]
                            };
                        }
                    }
                }
            }

            return default(Identity);
        }

        private Identity MapIdentityFromReader(SqlDataReader reader)
        {
            return new Identity
            {
                Created = (DateTimeOffset)reader["Created"],
                Email = (string)reader["Email"],
                EmailConfirmed = (bool)reader["EmailConfirmed"],
                GivenName = (string)reader["GivenName"],
                Surname = (string)reader["Surname"],
                ClusteredId = (int)reader["ClusteredId"],
                IdentityId = (Guid)reader["IdentityId"],
                PhoneNumber = (string)reader["PhoneNumber"],
                PhoneNumberConfirmed = (bool)reader["PhoneNumberConfirmed"],
                Updated = (DateTimeOffset)reader["Updated"]
            };
        }
    }
}
