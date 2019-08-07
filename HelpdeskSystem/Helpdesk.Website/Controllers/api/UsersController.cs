using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Helpdesk.Common.Requests.Users;
using Helpdesk.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Helpdesk.Website.Controllers.api
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/users")]
    [ApiController]
    public class UsersController : BaseApiController
    {
        [HttpGet]
        [Route("{id}")]
        public IActionResult GetUser([FromRoute] int id)
        {
            if (!IsAuthorized())
                return Unauthorized();

            throw new NotImplementedException();
        }

        [HttpGet]
        [Route("")]
        public IActionResult GetUsers()
        {
            if (!IsAuthorized())
                return Unauthorized();

            throw new NotImplementedException();
        }

        [HttpPost]
        [Route("{id}")]
        public IActionResult AddUser([FromBody] AddUserRequest request)
        {
            if (!IsAuthorized())
                return Unauthorized();

            throw new NotImplementedException();
        }

        [HttpPatch]
        [Route("{id}")]
        public IActionResult UpdateUser([FromRoute] int id, [FromBody] UpdateUserRequest request)
        {
            if (!IsAuthorized())
                return Unauthorized();

            throw new NotImplementedException();
        }

        [HttpDelete]
        [Route("{id}")]
        public IActionResult DeleteUser ([FromRoute] int id)
        {
            if (!IsAuthorized())
                return Unauthorized();

            throw new NotImplementedException();
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            if (request == null)
                return BadRequest();

            try
            {
                var facade = new UsersFacade();
                var result = facade.LoginUser(request);

                switch (result.Status)
                {
                    case HttpStatusCode.OK:
                        {
                            CookieOptions cookie = new CookieOptions()
                            {
                                Expires = DateTime.Now.AddHours(4),
                                HttpOnly = false,
                                Domain = ".swin.helpdesk.edu.au",
                                IsEssential = true,
                                Path = "/",
                                Secure = false,
                                SameSite = SameSiteMode.Strict,
                            };

                            Response.Cookies.Append("AuthToken", result.Token, cookie);
                            return Ok();
                        }
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
                s_logger.Error(ex, "Unable to add galaxy.");
            }
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}