using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rinsen.IdentityProvider.ExternalApplications
{
    public interface IExternalApplicationStorage
    {
        Task CreateAsync(ExternalApplication externalApplication);
        Task<ExternalApplication> GetFromApplicationNameAndHostAsync(string applicationName, string host);
        Task<ExternalApplication> GetFromApplicationKeyAsync(string applicationKey);
        Task<ExternalApplication> GetFromExternalApplicationIdAsync(Guid externalApplicationId);
        Task<IEnumerable<ExternalApplication>> GetAllAsync();
        Task UpdateAsync(ExternalApplication externalApplication);
        Task DeleteAsync(ExternalApplication externalApplication);
    }
}