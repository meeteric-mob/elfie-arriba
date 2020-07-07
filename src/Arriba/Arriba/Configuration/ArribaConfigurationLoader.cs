// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Arriba.Configuration;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Xml;

namespace Arriba.TfsWorkItemCrawler
{
    public abstract class ArribaConfigurationLoader 
    {

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
        static public T LoadByPath<T>(string jsonConfigurationPath, string sectionName="") where T : class, IArribaConfiguration, new()
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

    }
}