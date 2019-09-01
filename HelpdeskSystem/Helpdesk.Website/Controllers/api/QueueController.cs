using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Helpdesk.Common.Requests.Queue;
using Helpdesk.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Helpdesk.Website.Controllers.api
{
    /// <summary>
    /// Used as the access point for any features relating to the queue
    /// </summary>
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/queue")]
    [ApiController]
    public class QueueController : BaseApiController
    {
        /// <summary>
        /// Gets every queue item from the database
        /// </summary>
        /// <returns>Response which indicates success or failure</returns>
        [HttpGet]
        [Route("")]
        public IActionResult GetQueueItems()
        {
            if (!IsAuthorized())
                return Unauthorized();

            try
            {
                var facade = new QueueFacade();
                var response = facade.GetQueueItems();

                switch (response.Status)
                {
                    case HttpStatusCode.OK:
                        return Ok();
                    case HttpStatusCode.BadRequest:
                        return BadRequest(BuildBadRequestMessage(response));
                    case HttpStatusCode.NotFound:
                        return NotFound();
                    case HttpStatusCode.InternalServerError:
                        return StatusCode(StatusCodes.Status500InternalServerError);
                }
                s_logger.Fatal("This code should be unreachable, unknown result has occured.");
            }
            catch (Exception ex)
            {
                s_logger.Error(ex, "Unable to get queue items.");
            }
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}
