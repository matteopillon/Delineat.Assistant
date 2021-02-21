using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;
namespace Delineat.Assistant.Core.Data
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<DAAssistantDBContext>
    {
        public DAAssistantDBContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var builder = new DbContextOptionsBuilder<DAAssistantDBContext>();

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            builder.UseSqlServer(connectionString);

            return new DAAssistantDBContext(builder.Options);
        }
    }
}
