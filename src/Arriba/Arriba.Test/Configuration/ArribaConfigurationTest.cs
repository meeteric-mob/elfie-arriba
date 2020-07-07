using Arriba.Configuration;
using Arriba.TfsWorkItemCrawler;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace Arriba.Test.Configuration
{
    [TestClass]
    public class ArribaConfigurationTest
    {
        private IArribaConfiguration _configuration;
        private IArribaConfigurationLoader _configurationLoader;        

        [TestMethod]
        public void LoadExistentConfigurationUsingConfigurationName()
        {
            var args = new string[] { };
            _configurationLoader = new ArribaConfigurationLoader(args, GetBasePath("Louvau"));
            _configuration = _configurationLoader.Bind<CrawlerConfiguration>("Arriba");
            Assert.IsNotNull(_configuration);
            Assert.AreEqual("Louvau", _configuration.ArribaTable);
        }

        [TestMethod]
        public void ThrowDirectoryNotFoundExceptionForANonExistenceBasePath()
        {
            Assert.ThrowsException<DirectoryNotFoundException>(
                () => new ArribaConfigurationLoader(new string[] { }, GetBasePath("foo")));
        }

        [TestMethod]
        public void LoadExistentConfigurationUsingConfigurationRoot()
        {
            var confRoot = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .Build();

            _configurationLoader = new ArribaConfigurationLoader(confRoot);
            Assert.IsNotNull(_configurationLoader);
        }

        [TestMethod]
        public void GetEnvironmentVariableValue()
        {
            System.Environment.SetEnvironmentVariable("ArribaLoaderTest", "true");

            var args = new string[] { };
            _configurationLoader = new ArribaConfigurationLoader(args);            
            Assert.IsNotNull(_configurationLoader);
            Assert.AreEqual("true", _configurationLoader.GetStringValue("ArribaLoaderTest"));
        }


        [TestMethod]
        public void AddNetJsonConfigurationFile()
        {
            var args = new string[] {"test=foo" };
            _configurationLoader = new ArribaConfigurationLoader(args);
            Assert.IsNotNull(_configurationLoader);
            Assert.AreEqual("foo", _configurationLoader.GetStringValue("test"));
            Assert.AreEqual("", _configurationLoader.GetStringValue("Arriba:arribaTable", ""));
            _configurationLoader.AddJsonSource(Path.Combine(GetBasePath("Louvau"),"appsettings.json"));
            Assert.AreEqual("Louvau", _configurationLoader.GetStringValue("Arriba:arribaTable"));
        }

        private string GetBasePath(string configurationName)
        {
            var configSrcFolder = "src";
            var basePath = Directory.GetCurrentDirectory();
            basePath = basePath.Substring(0, basePath.IndexOf(configSrcFolder) + configSrcFolder.Length);
            return Path.Combine(basePath, "Arriba", "Databases", configurationName);
        }
    }
}
