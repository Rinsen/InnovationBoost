using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace Rinsen.IdentityProvider
{
    public class IdentityAttributeStorage : IIdentityAttributeStorage
    {
        private readonly IdentityOptions _identityOptions;

        private const string _getSql = @"SELECT 
                                            Attribute,
                                            ClusteredId,
                                            IdentityId
                                        FROM 
                                            IdentityAttributes 
                                        WHERE 
                                            IdentityId=@IdentityId";

        public IdentityAttributeStorage(IdentityOptions identityOptions)
        {
            _identityOptions = identityOptions;
        }

        public async Task<IEnumerable<IdentityAttribute>> GetIdentityAttributesAsync(Guid identityId)
        {
            var result = new List<IdentityAttribute>();

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
                            result.Add(new IdentityAttribute
                            {
                                Attribute = (string)reader["Attribute"],
                                ClusteredId = (int)reader["ClusteredId"],
                                IdentityId = (Guid)reader["IdentityId"],
                            });
                        }
                    }
                }
            }

            return result;
        }
    }
}
