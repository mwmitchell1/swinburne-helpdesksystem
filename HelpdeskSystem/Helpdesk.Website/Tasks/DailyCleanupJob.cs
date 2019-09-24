using System;
using System.Threading.Tasks;
using Helpdesk.Services;
using Microsoft.Extensions.Logging;
using NLog;
using Quartz;

namespace Helpdesk.Website
{
    /// <summary>
    /// Used to force check-out and remove remaining queue items at the end of the day.
    /// </summary>
    public class DailyCleanupJob: IJob
    {
        private static Logger s_logger = LogManager.GetCurrentClassLogger();

        public Task Execute(IJobExecutionContext context)
        {
            try
            {
                var facade = new HelpdeskFacade();
                DateTime dateTime = DateTime.Now;
                if (!facade.ForceCheckoutQueueRemove(dateTime).Result)
                {
                    s_logger.Error("Unable to remove queue items and check-ins.");
                }
            }
            catch (Exception ex)
            {
                s_logger.Error(ex, "Unable to remove queue items and check-ins.");
            }
            return Task.CompletedTask;
        }
    }
}
