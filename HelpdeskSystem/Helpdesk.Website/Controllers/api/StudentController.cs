using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Helpdesk.Common.Requests.Students;
using Helpdesk.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Helpdesk.Website.Controllers.api
{
    /// <summary>
    /// Used as the access point for any features relating to students
    /// </summary>
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/student")]
    [ApiController]
    public class StudentController : BaseApiController
    {
        /// <summary>
        /// Edits a specific student's nickname with the given information
        /// </summary>
        /// <param name="id">ID of the student to be updated</param>
        /// <param name="request">Request that contains the new nickname</param>
        /// <returns>A response which indicates success or failure</returns>
        [HttpPatch]
        [Route("nickname/{id}")]
        public IActionResult EditStudentNickname([FromRoute] int id, [FromBody] EditStudentNicknameRequest request)
        {
            if (!IsAuthorized())
                return Unauthorized();

            try
            {
                var facade = new StudentFacade();
                var response = facade.EditStudentNickname(id, request);

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
                s_logger.Error(ex, "Unable to edit student's nickname.");
            }
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("")]
        public IActionResult ValidateNickname([FromBody] ValidateNicknameRequest request)
        {
            try
            {
                var facade = new StudentFacade();
                var response = facade.ValidateNickname(request);

                switch (response.Status)
                {
                    case HttpStatusCode.OK:
                        return Ok(response);
                    case HttpStatusCode.Accepted:
                        return Accepted(response);
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
                s_logger.Error(ex, "Unable to validate student's nickname.");
            }
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}
