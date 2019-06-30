using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Helpdesk.Common.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NLog;

namespace Helpdesk.Website.Controllers.api
{
    [ApiController]
    public class BaseApiController : ControllerBase
    {
        protected static Logger s_logger = LogManager.GetCurrentClassLogger();

        protected string buildBadRequestMessage(BaseResponse response)
        {
            string message = string.Empty;
            try
            {
                foreach (StatusMessage statusMessage in response.StatusMessages)
                    message += statusMessage.Message + "\n";
            }
            catch (Exception ex)
            {
                s_logger.Error(ex, "Unable to build bad request message.");
                message = string.Empty;
            }

            return message;
        }
    }
}