using System;
using System.Collections.Generic;
using System.IO;
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
    /// <summary>
    /// Used as the access point for any features relating to helpdesks and timespans
    /// </summary>
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/helpdesk")]
    [ApiController]
    public class HelpdeskController : BaseApiController
    {

        /// <summary>
        /// Used to retreive all of the helpdesks
        /// </summary>
        /// <returns>A reponse to indictae whether or not it was a success 
        /// with a list of helpdesks</returns>
        [HttpGet]
        [Route("")]
        public IActionResult GetHelpdesks()
        {
            try
            {
                var facade = new HelpdeskFacade();
                var response = facade.GetHelpdesks();

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
                s_logger.Error(ex, "Unable to get helpdesks.");
            }
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        /// <summary>
        /// Used to retreive all of the active helpdesks
        /// </summary>
        /// <returns>A reponse to indictae whether or not it was a success 
        /// with a list of active helpdesks</returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("active")]
        public IActionResult GetActiveHelpdesks()
        {
            try
            {
                var facade = new HelpdeskFacade();
                var response = facade.GetActiveHelpdesks();

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
                s_logger.Error(ex, "Unable to get active helpdesks.");
            }
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        /// <summary>
        /// Used to get the information for a specific helpdesk
        /// </summary>
        /// <param name="id">The id of the helpdesk to be retreived</param>
        /// <returns>A response indicating the success and helpdesk DTO or null</returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("{id}")]
        public IActionResult GetHelpdesk([FromRoute] int id)
        {
            try
            {
                var facade = new HelpdeskFacade();
                var response = facade.GetHelpdesk(id);

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
                s_logger.Error(ex, "Unable to get helpdesk.");
            }
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

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

        /// <summary>
        /// Gets every timespan from the database
        /// </summary>
        /// <returns>Response which indicates success or failure</returns>
        [HttpGet]
        [Route("timespan")]
        public IActionResult GetTimeSpans()
        {
            if (!IsAuthorized())
                return Unauthorized();

            try
            {
                var facade = new HelpdeskFacade();
                var response = facade.GetTimeSpans();

                switch (response.Status)
                {
                    case HttpStatusCode.OK:
                        return Ok(response);
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
                s_logger.Error(ex, "Unable to get timespans.");
            }
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        /// <summary>
        /// Gets a specific timespan from the database
        /// </summary>
        /// <param name="id">ID of the specific timespan</param>
        /// <returns>Response which indicates success or failure</returns>
        [HttpGet]
        [Route("timespan/{id}")]
        public IActionResult GetTimeSpan([FromRoute] int id)
        {
            if (!IsAuthorized())
                return Unauthorized();

            try
            {
                var facade = new HelpdeskFacade();
                var response = facade.GetTimeSpan(id);

                switch (response.Status)
                {
                    case HttpStatusCode.OK:
                        return Ok(response);
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
                s_logger.Error(ex, "Unable to get timespan.");
            }
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        /// <summary>
        /// Adds a new timespan into the database
        /// </summary>
        /// <param name="request">The request with the timespan information</param>
        /// <returns>Response which indicates success or failure</returns>
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

        /// <summary>
        /// Updates a specific timespan with the given information
        /// </summary>
        /// <param name="id">ID of the timespan to be updated</param>
        /// <param name="request">Request containing the new timespan information</param>
        /// <returns>Response which indicates success or failure</returns>
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
                s_logger.Error(ex, "Unable to update timespan.");
            }
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        /// <summary>
        /// Deletes a specific timespan from the database
        /// </summary>
        /// <param name="id">SpanID of the timespan to be deleted</param>
        /// <returns>Response which indicates success or failure</returns>
        [HttpDelete]
        [Route("timespan/{id}")]
        public IActionResult DeleteTimeSpan([FromRoute] int id)
        {
            if (!IsAuthorized())
                return Unauthorized();

            try
            {
                var facade = new HelpdeskFacade();
                var response = facade.DeleteTimeSpan(id);

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
                s_logger.Error(ex, "Unable to delete timespan.");
            }
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        /// <summary>
        /// Used to get an export of the helpdesk as a zip of CSVs
        /// </summary>
        /// <returns>The zip file</returns>
        [HttpGet]
        [Route("~/api/exportdatabase")]
        public IActionResult GetFullDatabaseBackup()
        {
            if (!IsAuthorized())
                return Unauthorized();

            try
            {
                var facade = new HelpdeskFacade();
                var response = facade.ExportDatabaseManual();
                var contentType = "application/zip";
                Response.ContentType = contentType;

                switch (response.Status)
                {
                    case HttpStatusCode.OK:
                        return new FileContentResult(response.File, contentType)
                        {
                            FileDownloadName = Path.GetFileName(response.Path),
                        };
                    case HttpStatusCode.InternalServerError:
                        return StatusCode(StatusCodes.Status500InternalServerError);
                }
                s_logger.Fatal("This code should be unreachable, unknown result has occured.");
            }
            catch (Exception ex)
            {
                s_logger.Error(ex, "Unable to export database timespan.");
            }
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
        
        /// <summary>
        /// Used to force checkout students and clear queue
        /// </summary>
        /// <param name="id">The hepdesk of the ID</param>
        /// <returns>A response that indicates the result</returns>
        [HttpDelete]
        [Route("{id}/clear")]
        public IActionResult ClearHelpdesk([FromRoute]int id)
        {
            if (!IsAuthorized())
                return Unauthorized();

            try
            {
                var facade = new HelpdeskFacade();
                var response = facade.ForceCheckoutQueueRemove(id);

                switch (response.Status)
                {
                    case HttpStatusCode.OK:
                        return Ok(response);
                    case HttpStatusCode.InternalServerError:
                        return StatusCode(StatusCodes.Status500InternalServerError);
                }
                s_logger.Fatal("This code should be unreachable, unknown result has occured.");
            }
            catch (Exception ex)
            {
                s_logger.Error(ex, "Unable to clear helpdesk queue and/or check ins.");
            }
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}