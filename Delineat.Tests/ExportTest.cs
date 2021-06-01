using Delineat.Assistan.Exports;
using Delineat.Assistant.Core.Data;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using System;
using System.IO;

namespace Delineat.Tests
{
    public class Tests
    {
        private DAAssistantDBContext dbContext = null;
        private string fileDirectoryPath = string.Empty;
        [SetUp]
        public void Setup()
        {
            var confBuilder = new ConfigurationBuilder();

            confBuilder.AddJsonFile("appsettings.json");
            var config = confBuilder.Build();

            this.dbContext = new DAAssistantDBContext(config.GetConnectionString("defaultConnection"));

            fileDirectoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Test");
            Directory.CreateDirectory(fileDirectoryPath);
        }

        [Test]
        public void MothlyHoursExportTest()
        {
            var export = new MothlyHoursExport(dbContext,null);
            string path = Path.Combine(fileDirectoryPath, "Export.xls");
            Assert.True(export.ExportToExcel(path, 1, 2021));
        }
        [TearDown]
        public void Clean()
        {
            Directory.Delete(fileDirectoryPath, true);
        }
    }
}