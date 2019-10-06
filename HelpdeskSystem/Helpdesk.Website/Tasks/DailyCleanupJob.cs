using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Helpdesk.Common.Responses;
using Helpdesk.Common.Responses.Helpdesk;
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
                var helpdeskIds = facade.GetHelpdesks().Helpdesks.Select(h => h.HelpdeskID).ToList();

                foreach(int id in helpdeskIds)
                {
                    ForceCheckoutQueueRemoveResponse result = facade.ForceCheckoutQueueRemove(id);
                    if (result.Status != HttpStatusCode.OK)
                    {
                        foreach (StatusMessage message in result.StatusMessages)
                        {
                            s_logger.Error(message);
                        }
                    }
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
