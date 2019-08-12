using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Helpdesk.Common.Requests.Helpdesk;
using Helpdesk.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Helpdesk.Website.Controllers.api
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/helpdesk")]
    [ApiController]
    public class HelpdeskController : BaseApiController
    {
        [HttpGet]
        [Route("timespans")]
        public IActionResult GetTimeSpans()
        {
            if (!IsAuthorized())
                return Unauthorized();

            throw new NotImplementedException();
        }

        [HttpGet]
        [Route("timespan/{id}")]
        public IActionResult GetTimeSpan([FromRoute] int id)
        {
            if (!IsAuthorized())
                return Unauthorized();

            throw new NotImplementedException();
        }

        [HttpPost]
        [Route("timespan")]
        public IActionResult AddTimeSpan([FromBody] AddTimeSpanRequest request)
        {
            if (!IsAuthorized())
                return Unauthorized();

            try
            {
                var facade = new HelpdeskFacade();
                var result = facade.AddTimeSpan(request);

                switch (result.Status)
                {
                    case HttpStatusCode.OK:
                        return Ok();
                    case HttpStatusCode.BadRequest:
                        return BadRequest(BuildBadRequestMessage(result));
                    case HttpStatusCode.InternalServerError:
                        return StatusCode(StatusCodes.Status500InternalServerError);
                    case HttpStatusCode.NotFound:
                        return NotFound();
                }
                s_logger.Fatal("This code should be unreachable, unknown result has occured.");
            }
            catch (Exception ex)
            {
                s_logger.Error(ex, "Unable to add timespan.");
            }
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        [HttpPatch]
        [Route("timespan/{id}")]
        public IActionResult UpdateTimeSpan([FromRoute] int id, [FromBody] UpdateTimeSpanRequest request)
        {
            if (!IsAuthorized())
                return Unauthorized();

            throw new NotImplementedException();
        }

        [HttpDelete]
        [Route("timespan/{id}")]
        public IActionResult DeleteTimeSpan([FromRoute] int id)
        {
            if (!IsAuthorized())
                return Unauthorized();

            throw new NotImplementedException();
        }
    }
}