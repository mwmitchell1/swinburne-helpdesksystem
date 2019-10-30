using System;
using System.Net;
using Helpdesk.Common.Requests.Units;
using Helpdesk.Common.Responses.Units;
using Helpdesk.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Helpdesk.Website.Controllers.api
{
    /// <summary>
    /// Used as the access point for any features relating to units
    /// </summary>
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/units")]
    [ApiController]
    public class UnitsController : BaseApiController
    {
        /// <summary>
        /// Used to either add a new unit, or update an existing unit, in the database
        /// </summary>
        /// <param name="id">The id of the unit if this is used to update a unit</param>
        /// <param name="request">Request that contains the new unit information</param>
        /// <returns>Response which indicates success or failure</returns>
        [HttpPost]
        [Route("{id}")]
        public IActionResult AddOrUpdateUnit([FromRoute] int id, [FromBody] AddUpdateUnitRequest request)
        {
            if (request == null)
                return BadRequest();

            if (!IsAuthorized())
                return Unauthorized();

            try
            {
                var facade = new UnitsFacade();
                var response = facade.AddOrUpdateUnit(id, request);

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
                s_logger.Error(ex, "Unable to add or update unit.");
            }
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        /// <summary>
        /// Retrieve a unit from the database by id.
        /// </summary>
        /// <param name="id">The id of the unit to retrieve from the database.</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}")]
        public IActionResult GetUnit([FromRoute] int id)
        {
            if (!IsAuthorized())
                return Unauthorized();

            try
            {
                var facade = new UnitsFacade();
                var response = facade.GetUnit(id);

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
                s_logger.Error(ex, "Unable to get unit.");
            }
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        /// <summary>
        /// Retrieve units from the database by helpdesk ID
        /// </summary>
        /// <param name="id">The ID of the helpdesk</param>
        /// <returns>Response which indicates success or failure</returns>
        [HttpGet]
        [Route("helpdesk/{id}")]
        public IActionResult GetUnitsByHelpdeskID([FromRoute] int id)
        {
            if (!IsAuthorized())
                return Unauthorized();

            try
            {
                var facade = new UnitsFacade();
                var response = facade.GetUnitsByHelpdeskID(id, false);

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
                s_logger.Error(ex, "Unable to get units with helpdesk id "+id+".");
            }
            return StatusCode(StatusCodes.Status500InternalServerError);
        }


        /// <summary>
        /// Retrieve the active units from the database by helpdesk ID
        /// </summary>
        /// <param name="id">The ID of the helpdesk</param>
        /// <returns>Response which indicates success or failure</returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("helpdesk/{id}/active")]
        public IActionResult GetActiveUnitsByHelpdeskID([FromRoute] int id)
        {
            try
            {
                var facade = new UnitsFacade();
                var response = facade.GetUnitsByHelpdeskID(id, true);

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
                s_logger.Error(ex, "Unable to get units with helpdesk id " + id + ".");
            }
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        /// <summary>
        /// Delete a unit from the database using its UnitID
        /// </summary>
        /// <param name="id">ID of the unit to be deleted</param>
        /// <returns>Response which indicates success or failure</returns>
        [HttpDelete]
        [Route("{id}")]
        public IActionResult DeleteUnit([FromRoute] int id)
        {
            if (!IsAuthorized())
                return Unauthorized();

            try
            {
                var facade = new UnitsFacade();
                var response = facade.DeleteUnit(id);

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
                s_logger.Error(ex, "Unable to delete unit.");
            }
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}
