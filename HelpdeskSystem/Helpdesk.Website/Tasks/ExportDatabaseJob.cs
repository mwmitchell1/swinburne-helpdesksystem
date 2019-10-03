using Helpdesk.Services;
using NLog;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Helpdesk.Website
{
    /// <summary>
    /// Job used to export the system database to a ZIP file with CSVs.
    /// </summary>
    public class ExportDatabaseJob : IJob
    {
        private static Logger s_logger = LogManager.GetCurrentClassLogger();

        public Task Execute(IJobExecutionContext context)
        {
            try
            {
                var facade = new HelpdeskFacade();
                if(facade.ExportDatabase().Status != HttpStatusCode.OK)
                {
                    s_logger.Error("Unable to export database.");
                }
            }
            catch (Exception ex)
            {
                s_logger.Error(ex, "Unable to export database.");
            }
            return Task.CompletedTask;
        }
    }
}
