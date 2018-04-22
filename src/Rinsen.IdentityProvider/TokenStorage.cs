using Rinsen.IdentityProvider;
using Rinsen.IdentityProvider.ExternalApplications;
using System;
using System.Data.SqlClient;
using System.Security.Authentication;
using System.Threading.Tasks;

namespace Rinsen.IdentityProvider
{
    public class TokenStorage : ITokenStorage
    {
        private const string _createSql = @"INSERT INTO Tokens (
                                                Created,
                                                CorrelationId,
                                                ExternalApplicationId,
                                                Expiration,
                                                IdentityId,
                                                TokenId) 
                                            VALUES (
                                                @Created,
                                                @CorrelationId,
                                                @ExternalApplicationId,
                                                @Expiration,
                                                @IdentityId,
                                                @TokenId); 
                                            SELECT CAST(SCOPE_IDENTITY() as int)";

        private const string _oneTimeGetSql = @"DELETE FROM Tokens OUTPUT
                                            DELETED.*
                                        WHERE 
                                            TokenId=@TokenId";

        private readonly IdentityOptions _identityOptions;

        public TokenStorage(IdentityOptions identityOptions)
        {
            _identityOptions = identityOptions;
        }

        public async Task CreateAsync(Token token)
        {
            using (var connection = new SqlConnection(_identityOptions.ConnectionString))
            {
                try
                {
                    using (var command = new SqlCommand(_createSql, connection))
                    {
                        command.Parameters.Add(new SqlParameter("@Created", token.Created));
                        command.Parameters.Add(new SqlParameter("@CorrelationId", token.CorrelationId));
                        command.Parameters.Add(new SqlParameter("@ExternalApplicationId", token.ExternalApplicationId));
                        command.Parameters.Add(new SqlParameter("@Expiration", token.Expiration));
                        command.Parameters.Add(new SqlParameter("@IdentityId", token.IdentityId));
                        command.Parameters.Add(new SqlParameter("@TokenId", token.TokenId));

                        connection.Open();

                        token.ClusteredId = (int)await command.ExecuteScalarAsync();
                    }
                }
                catch (SqlException ex)
                {
                    if (ex.Number == 2601 || ex.Number == 2627)
                    {
                        throw new TokenAlreadyExistException($"Identity {token.IdentityId} already exist", ex);
                    }
                    throw;
                }
            }
        }

        public async Task<Token> GetAndDeleteAsync(string tokenId)
        {
            using (var connection = new SqlConnection(_identityOptions.ConnectionString))
            {
                using (var command = new SqlCommand(_oneTimeGetSql, connection))
                {
                    command.Parameters.Add(new SqlParameter("@TokenId", tokenId));
                    connection.Open();
                    var reader = await command.ExecuteReaderAsync();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            return new Token
                            {
                                ClusteredId = (int)reader["ClusteredId"],
                                Created = (DateTimeOffset)reader["Created"],
                                CorrelationId = (Guid)reader["CorrelationId"],
                                ExternalApplicationId = (Guid)reader["ExternalApplicationId"],
                                Expiration = (bool)reader["Expiration"],
                                IdentityId = (Guid)reader["IdentityId"],
                                TokenId = (string)reader["TokenId"]
                            };
                        }
                    }
                }
            }

            throw new AuthenticationException($"Invalid token id {tokenId}");
        }
    }
}
