using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Helpdesk.Common
{
    public class AppSettings
    {
        public string DefaultConnection { get; } = string.Empty;

        //public string AppSecret { get; set; }

        public AppSettings()
        {
            var configurationBuilder = new ConfigurationBuilder();
            var path = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
            configurationBuilder.AddJsonFile(path, false);

            var root = configurationBuilder.Build();

            DefaultConnection = root.GetConnectionString("DefaultConnection");
            //AppSecret = root.GetSection("AppSettings").GetSection("Secret").Value;

            var appSetting = root.GetSection("ApplicationSettings");
        }

    }
}
