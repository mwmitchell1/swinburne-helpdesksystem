using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Helpdesk.Common.Extensions;
using Helpdesk.Common.Responses;
using Helpdesk.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NLog;

namespace Helpdesk.Website.Controllers.api
{
    [ApiController]
    public class BaseApiController : ControllerBase
    {
        protected static Logger s_logger = LogManager.GetCurrentClassLogger();

        protected string BuildBadRequestMessage(BaseResponse response)
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

        protected bool IsAuthorized()
        {
            bool result = true;
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                string UserName = identity.Name;

                string id = identity.Claims.Where(c => c.Type.Contains(@"/sid")).FirstOrDefault().Value;

                if (!identity.IsAuthenticated)
                {
                    s_logger.Warn($"{identity.Name} IsAuthenticated = false");
                    return false;
                }

                var facade = new UsersFacade();
                result = facade.VerifyUser(UserName, id);
            }
            catch (NotFoundException ex)
            {
                s_logger.Warn(ex, "User does not exist in system.");
                result = false;
            }
            catch (Exception ex)
            {
                s_logger.Error(ex, "Unable to check user is authorised.");
                result = false;
            }
            return result;
        }
    }
}