using System;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace PVDevelop.ReminderBot.Microservice
{
    public static class ConfigurtionHelper
    {
        public static TConfig GetConfiguration<TConfig>(
            this IConfigurationRoot configurationRoot,
            string sectionName)
            where TConfig : new()
        {
            if (configurationRoot == null) throw new ArgumentNullException(nameof(configurationRoot));
            if (sectionName == null) throw new ArgumentNullException(nameof(sectionName));

            var config = new TConfig();

            foreach (var property in configurationRoot.GetSection(sectionName).GetChildren())
            {
                SetValue(config, property);
            }

            return config;
        }

        private static void SetValue<TConfig>(
            TConfig target,
            IConfigurationSection configurationSection)
        {
            var property = typeof(TConfig).GetRuntimeProperty(configurationSection.Key);

            var value =
                property.PropertyType == typeof(string) ?
                configurationSection.Value :
                JsonConvert.DeserializeObject(configurationSection.Value, property.PropertyType);

            property.SetValue(target, value);
        }
    }
}
