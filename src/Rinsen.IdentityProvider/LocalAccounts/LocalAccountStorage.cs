﻿using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Rinsen.IdentityProvider;

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
                                        Updated,
                                        SharedTotpSecret,
                                        TwoFactorTotpEnabled,
                                        TwoFactorSmsEnabled,
                                        TwoFactorEmailEnabled,
                                        TwoFactorAppNotificationEnabled,
                                        Deleted) 
                                    VALUES 
                                        (@Created,
                                        @FailedLoginCount,
                                        @IdentityId,
                                        @IsDisabled,
                                        @IterationCount,
                                        @LoginId,
                                        @PasswordHash,
                                        @PasswordSalt,
                                        @Updated,
                                        @SharedTotpSecret,
                                        @TwoFactorTotpEnabled,
                                        @TwoFactorSmsEnabled,
                                        @TwoFactorEmailEnabled,
                                        @TwoFactorAppNotificationEnabled,
                                        @Deleted); 
                                    SELECT CAST(SCOPE_IDENTITY() as int)";

        const string _selectWithIdentityId = @"SELECT Id,
                                        IdentityId,
                                        Created,
                                        FailedLoginCount,
                                        IsDisabled,
                                        IterationCount,
                                        LoginId,
                                        PasswordHash,
                                        PasswordSalt,
                                        Updated,
                                        SharedTotpSecret,
                                        TwoFactorTotpEnabled,
                                        TwoFactorSmsEnabled,
                                        TwoFactorEmailEnabled,
                                        TwoFactorAppNotificationEnabled,
                                        Deleted 
                                    FROM 
                                        LocalAccounts 
                                    WHERE 
                                        IdentityId=@IdentityId";

        const string _selectWithLoginId = @"SELECT Id,
                                        IdentityId,
                                        Created,
                                        FailedLoginCount,
                                        IsDisabled,
                                        IterationCount,
                                        LoginId,
                                        PasswordHash,
                                        PasswordSalt,
                                        Updated,
                                        SharedTotpSecret,
                                        TwoFactorTotpEnabled,
                                        TwoFactorSmsEnabled,
                                        TwoFactorEmailEnabled,
                                        TwoFactorAppNotificationEnabled,
                                        Deleted
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

        const string _update = @"UPDATE
                                    LocalAccounts
                                SET
                                    Created = @Created,
                                    FailedLoginCount = @FailedLoginCount,
                                    IdentityId = @IdentityId,
                                    IsDisabled = @IsDisabled,
                                    IterationCount = @IterationCount,
                                    LoginId = @LoginId,
                                    PasswordHash = @PasswordHash,
                                    PasswordSalt = @PasswordSalt,
                                    Updated = @Updated,
                                    SharedTotpSecret = @SharedTotpSecret,
                                    TwoFactorTotpEnabled = @TwoFactorTotpEnabled,
                                    TwoFactorSmsEnabled = @TwoFactorSmsEnabled,
                                    TwoFactorEmailEnabled = @TwoFactorEmailEnabled,
                                    TwoFactorAppNotificationEnabled = @TwoFactorAppNotificationEnabled,
                                    Deleted = @Deleted
                                WHERE 
                                    Id = @Id";


        public LocalAccountStorage(IdentityOptions identityOptions)
        {
            _identityOptions = identityOptions;
        }

        public async Task CreateAsync(LocalAccount localAccount)
        {
            localAccount.Created = DateTimeOffset.Now;
            localAccount.Updated = DateTimeOffset.Now;

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
                        command.Parameters.AddWithNullableValue("@SharedTotpSecret", localAccount.SharedTotpSecret);
                        command.Parameters.AddWithNullableValue("@TwoFactorAppNotificationEnabled", localAccount.TwoFactorAppNotificationEnabled);
                        command.Parameters.AddWithNullableValue("@TwoFactorEmailEnabled", localAccount.TwoFactorEmailEnabled);
                        command.Parameters.AddWithNullableValue("@TwoFactorSmsEnabled", localAccount.TwoFactorSmsEnabled);
                        command.Parameters.AddWithNullableValue("@TwoFactorTotpEnabled", localAccount.TwoFactorTotpEnabled);
                        command.Parameters.AddWithNullableValue("@Deleted", localAccount.Deleted);

                        await connection.OpenAsync();

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
            localAccount.Deleted = DateTimeOffset.Now;

            return UpdateAsync(localAccount);
        }

        public async Task<LocalAccount> GetAsync(string loginId)
        {
            using (var connection = new SqlConnection(_identityOptions.ConnectionString))
            {
                using (var command = new SqlCommand(_selectWithLoginId, connection))
                {
                    command.Parameters.AddWithValue("@LoginId", loginId);

                    await connection.OpenAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    { 
                        if (reader.HasRows)
                        {
                            while (await reader.ReadAsync())
                            {
                                return MapLocalAccountFromReader(reader);
                            }
                        }
                    }
                }
            }

            return default;
        }

        public async Task<LocalAccount> GetAsync(Guid identityId)
        {
            using (var connection = new SqlConnection(_identityOptions.ConnectionString))
            using (var command = new SqlCommand(_selectWithIdentityId, connection))
            {
                command.Parameters.Add(new SqlParameter("@IdentityId", identityId));

                await connection.OpenAsync();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (reader.HasRows)
                    {
                        while (await reader.ReadAsync())
                        {
                            return MapLocalAccountFromReader(reader);
                        }
                    }
                }
            }

            return default;
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
                Updated = (DateTimeOffset)reader["Updated"],
                Deleted = reader.GetValueOrDefault<DateTimeOffset?>("Deleted"),
                SharedTotpSecret = (byte[])reader["SharedTotpSecret"],
                TwoFactorAppNotificationEnabled = reader.GetValueOrDefault<DateTimeOffset?>("TwoFactorAppNotificationEnabled"),
                TwoFactorEmailEnabled = reader.GetValueOrDefault<DateTimeOffset?>("TwoFactorEmailEnabled"),
                TwoFactorSmsEnabled = reader.GetValueOrDefault<DateTimeOffset?>("TwoFactorSmsEnabled"),
                TwoFactorTotpEnabled = reader.GetValueOrDefault<DateTimeOffset?>("TwoFactorTotpEnabled")
            };
        }

        public async Task UpdateAsync(LocalAccount localAccount)
        {
            localAccount.Updated = DateTimeOffset.Now;

            using (var connection = new SqlConnection(_identityOptions.ConnectionString))
            using (var command = new SqlCommand(_update, connection))
            {
                command.Parameters.AddWithValue("@Id", localAccount.Id);
                command.Parameters.AddWithValue("@Created", localAccount.Created);
                command.Parameters.AddWithValue("@FailedLoginCount", localAccount.FailedLoginCount);
                command.Parameters.AddWithValue("@IdentityId", localAccount.IdentityId);
                command.Parameters.AddWithValue("@IsDisabled", localAccount.IsDisabled);
                command.Parameters.AddWithValue("@IterationCount", localAccount.IterationCount);
                command.Parameters.AddWithValue("@LoginId", localAccount.LoginId);
                command.Parameters.AddWithValue("@PasswordHash", localAccount.PasswordHash);
                command.Parameters.AddWithValue("@PasswordSalt", localAccount.PasswordSalt);
                command.Parameters.AddWithValue("@Updated", localAccount.Updated);
                command.Parameters.AddWithValue("@SharedTotpSecret", localAccount.SharedTotpSecret);
                command.Parameters.AddWithNullableValue("@TwoFactorAppNotificationEnabled", localAccount.TwoFactorAppNotificationEnabled);
                command.Parameters.AddWithNullableValue("@TwoFactorEmailEnabled", localAccount.TwoFactorEmailEnabled);
                command.Parameters.AddWithNullableValue("@TwoFactorSmsEnabled", localAccount.TwoFactorSmsEnabled);
                command.Parameters.AddWithNullableValue("@TwoFactorTotpEnabled", localAccount.TwoFactorTotpEnabled);
                command.Parameters.AddWithNullableValue("@Deleted", localAccount.Deleted);
                await connection.OpenAsync();

                var result = await command.ExecuteNonQueryAsync();
            }
        }

        public async Task UpdateFailedLoginCountAsync(LocalAccount localAccount)
        {
            using (var connection = new SqlConnection(_identityOptions.ConnectionString))
            using (var command = new SqlCommand(_updateFailedLoginCount, connection))
            {
                command.Parameters.AddWithValue("@FailedLoginCount", localAccount.FailedLoginCount);
                command.Parameters.AddWithValue("@Updated", localAccount.Updated);
                command.Parameters.AddWithValue("@IdentityId", localAccount.IdentityId);
                await connection.OpenAsync();

                var result = await command.ExecuteNonQueryAsync();
            }
        }
    }
}
