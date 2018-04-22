using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Rinsen.IdentityProvider.ExternalApplications
{
    public class ExternalSessionStorage : IExternalSessionStorage
    {
        private readonly IdentityOptions _identityOptions;

        private const string _createSql = @"INSERT INTO ExternalSessions (
                                                CorrelationId,
                                                Created,
                                                ExternalApplicationId,
                                                IdentityId) 
                                            VALUES (
                                                @CorrelationId,
                                                @Created,
                                                @ExternalApplicationId,
                                                @IdentityId); 
                                            SELECT CAST(SCOPE_IDENTITY() as int)";

        public ExternalSessionStorage(IdentityOptions identityOptions)
        {
            _identityOptions = identityOptions;
        }

        public async Task Create(ExternalSession externalSession)
        {
            using (var connection = new SqlConnection(_identityOptions.ConnectionString))
            {
                using (var command = new SqlCommand(_createSql, connection))
                {
                    command.Parameters.Add(new SqlParameter("@CorrelationId", externalSession.CorrelationId));
                    command.Parameters.Add(new SqlParameter("@Created", externalSession.Created));
                    command.Parameters.Add(new SqlParameter("@ExternalApplicationId", externalSession.ExternalApplicationId));
                    command.Parameters.Add(new SqlParameter("@IdentityId", externalSession.IdentityId));
                    connection.Open();

                    externalSession.Id = (int)await command.ExecuteScalarAsync();
                }
            }
        }
    }
}
