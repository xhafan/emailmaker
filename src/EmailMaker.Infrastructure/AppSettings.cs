using System.IO;
using Microsoft.Extensions.Configuration;

namespace EmailMaker.Infrastructure
{
    public static class AppSettings
    {
        static AppSettings()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public static IConfigurationRoot Configuration { get; }
    }
}