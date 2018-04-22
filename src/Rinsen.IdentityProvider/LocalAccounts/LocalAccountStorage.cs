using Rinsen.IdentityProvider;
using Rinsen.IdentityProvider.LocalAccounts;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Rinsen.IdentityProvider.LocalAccounts
{

    public class LocalAccountStorage : ILocalAccountStorage
    {
        private readonly IdentityOptions _identityOptions;

        const string _insertSql = @"INSERT INTO LocalAccounts 
                                        (Created,
                                        FailedLoginCount,
                                        IdentityId,
                                        IsDisabled,
                                        IterationCount,
                                        LoginId,
                                        PasswordHash,
                                        PasswordSalt,
                                        Updated) 
                                    VALUES 
                                        (@Created,
                                        @FailedLoginCount,
                                        @IdentityId,
                                        @IsDisabled,
                                        @IterationCount,
                                        @LoginId,
                                        @PasswordHash,
                                        @PasswordSalt,
                                        @Updated); 
                                    SELECT CAST(SCOPE_IDENTITY() as int)";

        const string _selectWithIdentityId = @"SELECT Id,
                                        Created,
                                        FailedLoginCount,
                                        IdentityId,
                                        IsDisabled,
                                        IterationCount,
                                        LoginId,
                                        PasswordHash,
                                        PasswordSalt,
                                        Updated 
                                    FROM 
                                        LocalAccounts 
                                    WHERE 
                                        IdentityId=@IdentityId";

        const string _selectWithLoginId = @"SELECT Id,
                                        Created,
                                        FailedLoginCount,
                                        IdentityId,
                                        IsDisabled,
                                        IterationCount,
                                        LoginId,
                                        PasswordHash,
                                        PasswordSalt,
                                        Updated 
                                    FROM 
                                        LocalAccounts 
                                    WHERE 
                                        LoginId=@LoginId";

        const string _updateFailedLoginCount = @"UPDATE 
                                                    LocalAccounts 
                                                SET 
                                                    FailedLoginCount = @FailedLoginCount,
                                                    Updated = @Updated 
                                                WHERE 
                                                    IdentityId=@IdentityId";


        public LocalAccountStorage(IdentityOptions identityOptions)
        {
            _identityOptions = identityOptions;
        }

        public async Task CreateAsync(LocalAccount localAccount)
        {
            using (var connection = new SqlConnection(_identityOptions.ConnectionString))
            {
                try
                {
                    using (var command = new SqlCommand(_insertSql, connection))
                    {
                        command.Parameters.Add(new SqlParameter("@Created", localAccount.Created));
                        command.Parameters.Add(new SqlParameter("@FailedLoginCount", localAccount.FailedLoginCount));
                        command.Parameters.Add(new SqlParameter("@IdentityId", localAccount.IdentityId));
                        command.Parameters.Add(new SqlParameter("@IsDisabled", localAccount.IsDisabled));
                        command.Parameters.Add(new SqlParameter("@IterationCount", localAccount.IterationCount));
                        command.Parameters.Add(new SqlParameter("@LoginId", localAccount.LoginId));
                        command.Parameters.Add(new SqlParameter("@PasswordHash", localAccount.PasswordHash));
                        command.Parameters.Add(new SqlParameter("@PasswordSalt", localAccount.PasswordSalt));
                        command.Parameters.Add(new SqlParameter("@Updated", localAccount.Updated));

                        connection.Open();

                        localAccount.Id = (int)await command.ExecuteScalarAsync();
                    }
                }
                catch (SqlException ex)
                {
                    // 2601 - Violation in unique index
                    // 2627 - Violation in unique constraint
                    if (ex.Number == 2601 || ex.Number == 2627)
                    {
                        throw new LocalAccountAlreadyExistException($"Identity {localAccount.LoginId} already exist for user {localAccount.IdentityId}", ex);
                    }
                    throw;
                }
            }
        }

        public Task DeleteAsync(LocalAccount localAccount)
        {
            throw new NotImplementedException();
        }

        public async Task<LocalAccount> GetAsync(string loginId)
        {
            using (var connection = new SqlConnection(_identityOptions.ConnectionString))
            {
                using (var command = new SqlCommand(_selectWithLoginId, connection))
                {
                    command.Parameters.Add(new SqlParameter("@LoginId", loginId));
                    connection.Open();
                    var reader = await command.ExecuteReaderAsync();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            return MapLocalAccountFromReader(reader);
                        }
                    }
                }
            }

            return default(LocalAccount);
        }

        public async Task<LocalAccount> GetAsync(Guid identityId)
        {
            using (var connection = new SqlConnection(_identityOptions.ConnectionString))
            {
                using (var command = new SqlCommand(_selectWithIdentityId, connection))
                {
                    command.Parameters.Add(new SqlParameter("@IdentityId", identityId));
                    connection.Open();
                    var reader = await command.ExecuteReaderAsync();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            return MapLocalAccountFromReader(reader);
                        }
                    }
                }
            }

            return default(LocalAccount);
        }

        private LocalAccount MapLocalAccountFromReader(SqlDataReader reader)
        {
            return new LocalAccount
            {
                Created = (DateTimeOffset)reader["Created"],
                FailedLoginCount = (int)reader["FailedLoginCount"],
                IdentityId = (Guid)reader["IdentityId"],
                IsDisabled = (bool)reader["IsDisabled"],
                IterationCount = (int)reader["IterationCount"],
                Id = (int)reader["Id"],
                LoginId = (string)reader["LoginId"],
                PasswordHash = (byte[])reader["PasswordHash"],
                PasswordSalt = (byte[])reader["PasswordSalt"],
                Updated = (DateTimeOffset)reader["Updated"]
            };
        }

        public Task UpdateAsync(LocalAccount localAccount)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateFailedLoginCountAsync(LocalAccount localAccount)
        {
            using (var connection = new SqlConnection(_identityOptions.ConnectionString))
            {
                using (var command = new SqlCommand(_updateFailedLoginCount, connection))
                {
                    command.Parameters.Add(new SqlParameter("@FailedLoginCount", localAccount.FailedLoginCount));
                    command.Parameters.Add(new SqlParameter("@Updated", localAccount.Updated));
                    command.Parameters.Add(new SqlParameter("@IdentityId", localAccount.IdentityId));
                    connection.Open();

                    var result = await command.ExecuteNonQueryAsync();
                }
            }
        }
    }
}
