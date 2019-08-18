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
        /// <summary>
        /// This method is the end point to be able to add a heldesk
        /// </summary>
        /// <param name="request">The request with the helpdesk information</param>
        /// <returns>A reponse to indictae whether or not it was a success</returns>
        [HttpPost]
        [Route("")]
        public IActionResult AddHelpdesk([FromBody] AddHelpdeskRequest request)
        {
            if (!IsAuthorized())
                return Unauthorized();

            if (request == null)
                return BadRequest();

            try
            {
                var facade = new HelpdeskFacade();
                var response = facade.AddHelpdesk(request);

                switch (response.Status)
                {
                    case HttpStatusCode.OK:
                        return Ok(response);
                    case HttpStatusCode.BadRequest:
                        return BadRequest(BuildBadRequestMessage(response));
                    case HttpStatusCode.InternalServerError:
                        return StatusCode(StatusCodes.Status500InternalServerError);
                    case HttpStatusCode.NotFound:
                        return NotFound();
                }
                s_logger.Fatal("This code should be unreachable, unknown result has occured.");
            }
            catch (Exception ex)
            {
                s_logger.Error(ex, "Unable to add helpdesk.");
            }
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        /// <summary>
        /// This method is the end point to be able to update a heldesk
        /// </summary>
        /// <param name="id">The id of the helpdesk to be update</param>
        /// <param name="request">The request with the helpdesk information</param>
        /// <returns>A reponse to indictae whether or not it was a success</returns>
        [HttpPatch]
        [Route("{id}")]
        public IActionResult AddHelpdesk([FromRoute] int id, [FromBody] UpdateHelpdeskRequest request)
        {
            if (!IsAuthorized())
                return Unauthorized();

            if (request == null)
                return BadRequest();

            try
            {
                var facade = new HelpdeskFacade();
                var response = facade.UpdateHelpdesk(id, request);

                switch (response.Status)
                {
                    case HttpStatusCode.OK:
                        return Ok(response);
                    case HttpStatusCode.BadRequest:
                        return BadRequest(BuildBadRequestMessage(response));
                    case HttpStatusCode.InternalServerError:
                        return StatusCode(StatusCodes.Status500InternalServerError);
                    case HttpStatusCode.NotFound:
                        return NotFound();
                }
                s_logger.Fatal("This code should be unreachable, unknown result has occured.");
            }
            catch (Exception ex)
            {
                s_logger.Error(ex, "Unable to update helpdesk.");
            }
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        [HttpGet]
        [Route("timespan")]
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
                var response = facade.AddTimeSpan(request);

                switch (response.Status)
                {
                    case HttpStatusCode.OK:
                        return Ok(response);
                    case HttpStatusCode.BadRequest:
                        return BadRequest(BuildBadRequestMessage(response));
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

            try
            {
                var facade = new HelpdeskFacade();
                var response = facade.UpdateTimeSpan(id, request);

                switch (response.Status)
                {
                    case HttpStatusCode.OK:
                        return Ok();
                    case HttpStatusCode.BadRequest:
                        return BadRequest(BuildBadRequestMessage(response));
                    case HttpStatusCode.InternalServerError:
                        return StatusCode(StatusCodes.Status500InternalServerError);
                    case HttpStatusCode.NotFound:
                        return NotFound();
                }
                s_logger.Fatal("This code should be unreachable, unknown result has occured.");
            }
            catch (Exception ex)
            {
                s_logger.Error(ex, "Unable to update timespan.");
            }
            return StatusCode(StatusCodes.Status500InternalServerError);
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