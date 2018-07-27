using System;
using System.Threading.Tasks;

namespace Rinsen.Logger.Service
{
    public class SettingsManager : ISettingsManager
    {

        private readonly SettingsStorage _settingsStorage;

        public SettingsManager(SettingsStorage settingsStorage)
        {
            _settingsStorage = settingsStorage;
        }

        public async Task<T> GetValueOrDefault<T>(string key, Guid identityId)
        {
            var setting = await _settingsStorage.Get(key, identityId);

            if (setting == default(Setting))
            {
                return default(T);
            }

            if (setting.Accessed < DateTimeOffset.Now.AddDays(-1))
            {
                setting.Accessed = DateTimeOffset.Now;

                await _settingsStorage.Update(setting);
            }

            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(setting.Value);
        }

        public async Task Set<T>(string key, Guid identityId, T value)
        {
            var settingString = Newtonsoft.Json.JsonConvert.SerializeObject(value);

            var setting = await _settingsStorage.Get(key, identityId);

            if (setting == default(Setting))
            {
                setting = new Setting
                {
                    IdentityId = identityId,
                    Key = key,
                    Value = settingString,
                    Accessed = DateTimeOffset.Now
                };

                await _settingsStorage.Create(setting);

                return;
            }

            if (setting.Value.Equals(settingString))
            {
                // No need to update when the value is the same
                return; 
            }

            setting.Accessed = DateTimeOffset.Now;
            setting.Value = settingString;

            await _settingsStorage.Update(setting);
        }
    }
}
