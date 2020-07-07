using Arriba.Configuration;
using Arriba.TfsWorkItemCrawler;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Reflection;

namespace Arriba.Test.Configuration
{
    [TestClass]
    public class ArribaConfigurationTest
    {
        private IArribaConfiguration _configuration;

        [TestMethod]
        public void LoadExistentConfigurationUsingConfigurationName()
        {
            _configuration = ArribaConfigurationLoader.LoadByConfigurationName<CrawlerConfiguration>("Louvau", "Arriba");
            Assert.IsNotNull(_configuration);
            Assert.AreEqual("Louvau", _configuration.ArribaTable);
        }

        [TestMethod]
        public void ThrowFileNotFoundExceptionForAnonExistenceConfiguration()
        {
            Assert.ThrowsException<FileNotFoundException>( 
                () => ArribaConfigurationLoader.LoadByConfigurationName<CrawlerConfiguration>("foo","Arriba"));
        }

        [TestMethod]
        public void LoadExistentConfigurationUsingJsonFullPath()
        {
            var configJsonPath = Path.Combine(Assembly.GetEntryAssembly().Location, 
                                            @"../../../../Databases", 
                                            "Louvau", 
                                            "config.json");

            Assert.ThrowsException<FileNotFoundException>(
                () => ArribaConfigurationLoader.LoadByPath<CrawlerConfiguration>(configJsonPath, string.Empty));
        }
    }
}
