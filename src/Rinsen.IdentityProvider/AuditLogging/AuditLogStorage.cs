using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Rinsen.IdentityProvider.AuditLogging
{
    public class AuditLogStorage
    {
        private readonly IdentityOptions _identityOptions;

        const string _insertSql = @"INSERT INTO AuditLogItems 
                                        (Details,
                                        EventType,
                                        IpAddress,
                                        Timestamp) 
                                    VALUES 
                                        (@Details,
                                        @EventType,
                                        @IpAddress,
                                        @Timestamp); 
                                    SELECT CAST(SCOPE_IDENTITY() as int)";

        public AuditLogStorage(IdentityOptions identityOptions)
        {
            _identityOptions = identityOptions;
        }

        internal async Task LogAsync(AuditLogItem auditLogItem)
        {
            using (var connection = new SqlConnection(_identityOptions.ConnectionString))
            {
                using (var command = new SqlCommand(_insertSql, connection))
                {
                    command.Parameters.Add(new SqlParameter("@Details", auditLogItem.Details));
                    command.Parameters.Add(new SqlParameter("@EventType", auditLogItem.EventType));
                    command.Parameters.Add(new SqlParameter("@IpAddress", auditLogItem.IpAddress));
                    command.Parameters.Add(new SqlParameter("@Timestamp", auditLogItem.Timestamp));

                    await connection.OpenAsync();

                    auditLogItem.Id = (int)await command.ExecuteScalarAsync();
                }
            }
        }
    }
}
