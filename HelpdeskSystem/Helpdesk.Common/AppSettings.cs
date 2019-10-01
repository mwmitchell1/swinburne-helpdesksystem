using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Helpdesk.Common
{
    /// <summary>
    /// This is used to load the web apps settings file so that it can't be used
    /// througout the code.
    /// </summary>
    public class AppSettings
    {
        public string WindowsConnectionString { get; } = string.Empty;

        public string MacConnectionString { get; set; }

        public string AppSecret { get; set; }

        public string DatabaseBackupDestination { get; set; }

        public Dictionary<string, string> Jobs { get; set; } = new Dictionary<string, string>();

        public AppSettings()
        {
            var configurationBuilder = new ConfigurationBuilder();
            var path = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
            configurationBuilder.AddJsonFile(path, false);

            var root = configurationBuilder.Build();

            WindowsConnectionString = root.GetConnectionString("Windows");
            MacConnectionString = root.GetConnectionString("Mac");

            AppSecret = root.GetSection("AppSettings").GetSection("Secret").Value;
            DatabaseBackupDestination = root.GetSection("AppSettings").GetSection("DatabaseBackupDestination").Value;

            var appSetting = root.GetSection("ApplicationSettings");
            Jobs.Add("ExportDatabaseJob", root.GetSection("Jobs").GetSection("ExportDatabaseJob").Value);
            Jobs.Add("DailyCleanupJob", root.GetSection("Jobs").GetSection("DailyCleanupJob").Value);
        }

    }
}
