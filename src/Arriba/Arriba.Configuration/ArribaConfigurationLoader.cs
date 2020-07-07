// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Configuration;
using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace Arriba.Configuration
{
    public class ArribaConfigurationLoader : IArribaConfigurationLoader
    {
        private const int MaximumSymbolNesting = 10;

        private readonly IConfigurationRoot configuration;

        public ArribaConfigurationLoader(IConfigurationRoot configuration)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            this.configuration = configuration;
        }

        public ArribaConfigurationLoader(string[] args, string basePath)
        {
            if (args == null) throw new ArgumentNullException(nameof(args));

            if (string.IsNullOrWhiteSpace(basePath))
                basePath = Directory.GetCurrentDirectory();
            else
            {
                if (!Directory.Exists(basePath))
                    throw new DirectoryNotFoundException(basePath);
            }

            var builder = new ConfigurationBuilder();
            builder.SetBasePath(basePath);

            configuration = builder.AddJsonFile("appsettings.json", true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", true)
                .AddEnvironmentVariables()
                .AddCommandLine(args)
                .Build();
        }

        /// <summary>
        /// Loads the configuration using the Configuration Name folder existent in ./Arriba/Databases
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configurationName">Folder name that holds the appsettings.json</param>
        /// <param name="sectionName">Name of the section that holds the Arriba configuration, empty string to bind the json to the type</param>
        /// <returns>Object that implements IArribaConfiguration</returns>
        static public T LoadByConfigurationName<T>(string configurationName, string sectionName) where T : class, IArribaConfiguration, new()
        {
            if (string.IsNullOrWhiteSpace(configurationName)) throw new ArgumentException("Not Provided!", nameof(configurationName));

            var configPath = Path.Combine(GetBasePath(), "Arriba", "Databases", configurationName, "appsettings.json");
            return LoadByPath<T>(configPath, sectionName);
        }

        /// <summary>
        /// Loads the configuration using the full path 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonConfigurationPath">full path for the configuration json</param>
        /// <param name="sectionName">Name of the section that holds the Arriba configuration, empty string to bind the json to the type</param>
        /// <returns>Object that implements IArribaConfiguration</returns>
        /// <returns></returns>
        static public T LoadByPath<T>(string jsonConfigurationPath, string sectionName = "") where T : class, IArribaConfiguration, new()
        {
            if (string.IsNullOrWhiteSpace(jsonConfigurationPath)) throw new ArgumentException("Not Provided!", nameof(jsonConfigurationPath));

            if (!File.Exists(jsonConfigurationPath))
                throw new FileNotFoundException(jsonConfigurationPath);

            var builder = new ConfigurationBuilder()
            .AddJsonFile(jsonConfigurationPath, optional: true)
            .AddEnvironmentVariables();

            var config = new T();
            if (string.IsNullOrWhiteSpace(sectionName))
                builder.Build().Bind(config);
            else
                builder.Build().GetSection(sectionName).Bind(config);
            return config;
        }

        private static string GetBasePath()
        {
            var configSrcFolder = "src";
            var basePath = Directory.GetCurrentDirectory();
            basePath = basePath.Substring(0, basePath.IndexOf(configSrcFolder) + configSrcFolder.Length);
            return basePath;
        }

        public T Bind<T>(string sectionName) where T : IArribaConfiguration, new()
        {
            var config = new T();
            if (string.IsNullOrWhiteSpace(sectionName))
                configuration.Bind(config);
            else
            {
                var section = configuration.GetSection(sectionName);
                if (section == null)
                    throw new Exception($"Section {sectionName} not found!");
                section.Bind(config);
            }
            return config;
        }

        public string GetStringValue(string keyName, string defaultValue = null)
        {
            return GetConfigurationValue(keyName, defaultValue);
        }

        public int GetIntValue(string keyName, int defaultValue = 0)
        {
            string value = GetConfigurationValue(keyName, defaultValue.ToString());
            int numericValue;
            if (int.TryParse(value, out numericValue))
                return numericValue;
            else
            {
                Debug.WriteLine($"Configuration value was not an integer as expected. Falling back to default, {defaultValue}");
                return defaultValue;
            }
        }

        public bool GetBoolValue(string keyName, bool defaultValue = false)
        {
            string value = GetConfigurationValue(keyName, defaultValue.ToString());
            bool boolValue;
            if (bool.TryParse(value, out boolValue))
                return boolValue;
            else
            {
                Debug.WriteLine($"Configuration value was not a boolean as expected. Falling back to default, {defaultValue}.");
                return defaultValue;
            }
        }

        /// <summary>
        /// Retreives the pre-parsed value for a config option, returning the default string if it is missing.  Any replacements in the token
        /// will be unpacked from this call
        /// </summary>
        /// <param name="keyName">config option name</param>
        /// <param name="defaultString">default value if the option is missing</param>
        /// <param name="depth">current unroll depth, used to restrict recursion depth</param>
        /// <returns>The unrolled value for a config setting</returns>
        private string GetConfigurationValue(string keyName, string defaultString, int depth = 0)
        {
            if (depth > MaximumSymbolNesting) throw new InvalidOperationException(String.Format("GetConfigurationValue doesn't support symbols nested more than {0} deep. Ensure you don't have symbols referencing each other circularly.", MaximumSymbolNesting));

            // Get the value for this key, or use the default
            string value = GetRawConfigurationValue(keyName, defaultString);

            // Now, *if the setting was found*, look for symbols in the value. Symbols look like {{SettingName}}.
            if (value != defaultString)
            {
                MatchCollection matches = Regex.Matches(value, @"\{\{([^\{\}]+)\}\}");
                foreach (Match m in matches)
                {
                    // Lookup the other symbol and replace the symbol with the value
                    // If the symbol isn't defined, leave it in as-is.
                    string innerSetting = m.Groups[1].Value;
                    string innerValue = GetConfigurationValue(innerSetting, m.Value, depth + 1);
                    value = value.Replace(m.Value, innerValue);
                }
            }
            Debug.WriteLine($"Configuration[\"{keyName}\"] = \"{value}\" ({new string(' ', depth * 2)}");


            return value;
        }

        private string GetRawConfigurationValue(string keyName, string defaultValue)
        {
            var value = configuration[keyName];

            if (value == null)
            {
                return defaultValue;
            }

            return value;
        }
    }
}