using Helpdesk.Common.Requests.Users;
using Helpdesk.Common.Responses;
using Helpdesk.Common.Responses.Users;
using Helpdesk.DataLayer;
using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text;

namespace Helpdesk.Services
{
    /// <summary>
    /// This class is used to handle the business logic of users
    /// </summary>
    public class UsersFacade
    {
        private static Logger s_Logger = LogManager.GetCurrentClassLogger();

        public GetUsersResponse GetUsers()
        {
            throw new NotImplementedException();
        }

        public GetUserResponse GetUser(int id)
        {
            throw new NotImplementedException();
        }

        public AddUserResponse AddUser(AddUserRequest request)
        {
            s_Logger.Info("Adding user...");

            AddUserResponse response = new AddUserResponse();

            try
            {
                response = (AddUserResponse)request.CheckValidation(response);
                if (response.Status == HttpStatusCode.BadRequest)
                {
                    return response;
                }

                var dataLayer = new DataLayer.UsersDataLayer();
                int? result = dataLayer.AddUser(request);

                if (result == null)
                {
                    throw new Exception("Unable to add user!");
                }

                response.UserId = (int)result;
                response.Status = HttpStatusCode.OK;
            }
            catch(Exception ex)
            {
                s_Logger.Error(ex, "Unable to add user!");
                response.Status = HttpStatusCode.InternalServerError;
                response.StatusMessages.Add(new StatusMessage(HttpStatusCode.InternalServerError, "Unable to add user!"));
            }
            return response;
        }

        public UpdateUserResponse UpdateUser(int id, UpdateUserRequest request)
        {
            throw new NotImplementedException();
        }

        public DeleteUserResponse DeleteUser(int id)
        {
            throw new NotImplementedException();
        }
    }
}
