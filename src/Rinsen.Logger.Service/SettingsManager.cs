﻿using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace Rinsen.Logger.Service
{
    public class SettingsManager : ISettingsManager
    {

        private readonly ISettingsStorage _settingsStorage;

        public SettingsManager(ISettingsStorage settingsStorage)
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

            return JsonSerializer.Deserialize<T>(setting.ValueField);
        }

        public async Task Set<T>(string key, Guid identityId, T value)
        {
            var settingString = JsonSerializer.Serialize(value);

            var setting = await _settingsStorage.Get(key, identityId);

            if (setting == default(Setting))
            {
                setting = new Setting
                {
                    IdentityId = identityId,
                    KeyField = key,
                    ValueField = settingString,
                    Accessed = DateTimeOffset.Now
                };

                await _settingsStorage.Create(setting);

                return;
            }

            if (setting.ValueField.Equals(settingString))
            {
                // No need to update when the value is the same
                return; 
            }

            setting.Accessed = DateTimeOffset.Now;
            setting.ValueField = settingString;

            await _settingsStorage.Update(setting);
        }
    }
}
