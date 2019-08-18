using System;
using System.Net;
using Helpdesk.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Helpdesk.Website.Controllers.api
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/units")]
    [ApiController]
    public class UnitsController : BaseApiController
    {
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
    }
}
