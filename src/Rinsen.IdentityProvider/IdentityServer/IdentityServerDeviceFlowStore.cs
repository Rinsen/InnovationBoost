using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using IdentityServer4.Stores.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Rinsen.IdentityProvider.IdentityServer.Entities;

namespace Rinsen.IdentityProvider.IdentityServer
{
    public class IdentityServerDeviceFlowStore : IDeviceFlowStore
    {
        private readonly IdentityServerDbContext _identityServerDbContext;
        private readonly IPersistentGrantSerializer _persistentGrantSerializer;
        private readonly ILogger<IdentityServerDeviceFlowStore> _logger;

        public IdentityServerDeviceFlowStore(IdentityServerDbContext identityServerDbContext,
           IPersistentGrantSerializer persistentGrantSerializer,
           ILogger<IdentityServerDeviceFlowStore> logger)
        {
            _identityServerDbContext = identityServerDbContext;
            _persistentGrantSerializer = persistentGrantSerializer;
            _logger = logger;
        }

        public async Task<DeviceCode> FindByDeviceCodeAsync(string deviceCode)
        {
            var deviceFlowCodes = await _identityServerDbContext.IdentityServerDeviceFlowCodes.AsNoTracking().FirstOrDefaultAsync(x => x.DeviceCode == deviceCode);

            var model = ToModel(deviceFlowCodes?.SerializedDeviceFlowCode);

            _logger.LogDebug("{deviceCode} found in database: {deviceCodeFound}", deviceCode, model != null);

            return model;
        }

        public async Task<DeviceCode> FindByUserCodeAsync(string userCode)
        {
            var deviceFlowCodes = await _identityServerDbContext.IdentityServerDeviceFlowCodes.AsNoTracking().FirstOrDefaultAsync(x => x.UserCode == userCode);

            if (deviceFlowCodes == default)
            {
                _logger.LogWarning("{userCode} not found in database", userCode);

                return null;
            }

            var model = ToModel(deviceFlowCodes?.SerializedDeviceFlowCode);

            _logger.LogDebug("{userCode} found in database: {userCodeFound}", userCode, model != null);

            return model;
        }

        public async Task RemoveByDeviceCodeAsync(string deviceCode)
        {
            var identityServerDeviceFlowCodes = await _identityServerDbContext.IdentityServerDeviceFlowCodes.FirstOrDefaultAsync(x => x.DeviceCode == deviceCode);

            if (identityServerDeviceFlowCodes != null)
            {
                _logger.LogDebug("removing {deviceCode} device code from database", deviceCode);

                _identityServerDbContext.IdentityServerDeviceFlowCodes.Remove(identityServerDeviceFlowCodes);

                try
                {
                    await _identityServerDbContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    throw new InvalidOperationException($"Exception removing {deviceCode} device code from database", ex);
                }
            }
            else
            {
                _logger.LogDebug("no {deviceCode} device code found in database", deviceCode);
            }
        }

        public Task StoreDeviceAuthorizationAsync(string deviceCode, string userCode, DeviceCode data)
        {
            _identityServerDbContext.IdentityServerDeviceFlowCodes.Add(ToEntity(data, deviceCode, userCode));

            return _identityServerDbContext.SaveChangesAsync();
        }

        public async Task UpdateByUserCodeAsync(string userCode, DeviceCode deviceCode)
        {
            var existing = await _identityServerDbContext.IdentityServerDeviceFlowCodes.SingleOrDefaultAsync(x => x.UserCode == userCode);

            if (existing == null)
            {
                throw new InvalidOperationException($"Could not update device code with user code {userCode}");
            }

            var entity = ToEntity(deviceCode, existing.DeviceCode, userCode);

            _logger.LogDebug("{userCode} found in database", userCode);

            existing.IdentityId = deviceCode.Subject != null ? Guid.Parse(deviceCode.Subject.FindFirst(JwtClaimTypes.Subject).Value) : (Guid?)null;
            existing.SerializedDeviceFlowCode = entity.SerializedDeviceFlowCode;

            try
            {
                await _identityServerDbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new InvalidOperationException($"Exception updating {userCode} user code in database", ex);
            }
        }

        private IdentityServerDeviceFlowCode ToEntity(DeviceCode model, string deviceCode, string userCode)
        {
            if (model == null || deviceCode == null || userCode == null) return null;

            return new IdentityServerDeviceFlowCode
            {
                DeviceCode = deviceCode,
                UserCode = userCode,
                ClientId = model.ClientId,
                IdentityId = model.Subject != null ? Guid.Parse(model.Subject.FindFirst(JwtClaimTypes.Subject).Value) : (Guid?)null,
                CreationTime = model.CreationTime,
                Expiration = model.CreationTime.AddSeconds(model.Lifetime),
                SerializedDeviceFlowCode = _persistentGrantSerializer.Serialize(model)
            };
        }

        private DeviceCode ToModel(string serializedDeviceFlowCode)
        {
            if (serializedDeviceFlowCode == null) return null;

            return _persistentGrantSerializer.Deserialize<DeviceCode>(serializedDeviceFlowCode);
        }
    }
}
