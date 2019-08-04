using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Helpdesk.Common.Requests.Helpdesk;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Helpdesk.Website.Controllers.api
{
    //[Authorize]
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

            throw new NotImplementedException();
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