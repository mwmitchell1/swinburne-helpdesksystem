using Helpdesk.Common;
using Helpdesk.Common.DTOs;
using Helpdesk.Common.Requests.Students;
using Helpdesk.Common.Responses;
using Helpdesk.Common.Responses.Students;
using Helpdesk.Data.Models;
using Helpdesk.DataLayer;
using NLog;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Helpdesk.Services
{
    /// <summary>
    /// This class is used to handle the business logic of students
    /// </summary>
    public class StudentFacade
    {
        private static Logger s_logger = LogManager.GetCurrentClassLogger();

        private readonly AppSettings _appSettings;

        /// <summary>
        /// Used to get the studnet by their nickname
        /// </summary>
        /// <param name="nickname">The nickname of the student to be found</param>
        /// <returns>The student details</returns>
        public GetStudentResponse GetStudentByNickname(string nickname)
        {
            GetStudentResponse response = new GetStudentResponse();

            try
            {
                var dataLayer = new StudentDatalayer();
                var nicknameDTO = dataLayer.GetStudentNicknameByNickname(nickname);

                if (nicknameDTO == null)
                {
                    response.Status = HttpStatusCode.NotFound;
                }
                else
                {
                    response.Nickname = nicknameDTO;
                    response.Status = HttpStatusCode.OK;
                }
            }
            catch (Exception ex)
            {
                s_logger.Error(ex, "Unable to a get nickname");
                response.Status = HttpStatusCode.InternalServerError;
                response.StatusMessages.Add(new StatusMessage(HttpStatusCode.InternalServerError, "Unable to get nickname"));
            }

            return response;
        }

        /// <summary>
        /// Used to add a student nickname to the system
        /// </summary>
        /// <param name="request">The information required to add a student nickname</param>
        /// <returns>A response indictaes if the action was successful</returns>
        public AddStudentResponse AddStudentNickname(AddStudentRequest request)
        {
            AddStudentResponse response = new AddStudentResponse();

            try
            {
                response = (AddStudentResponse)request.CheckValidation(response);

                if (response.Status == HttpStatusCode.BadRequest)
                    return response;

                StudentDatalayer studentDatalayer = new StudentDatalayer();
                var nickname = studentDatalayer.GetStudentNicknameByNickname(request.Nickname);

                if (nickname != null)
                {
                    response.Status = HttpStatusCode.BadRequest;
                    response.StatusMessages.Add(new StatusMessage(HttpStatusCode.BadRequest, "This nickname is already being used."));
                    return response;
                }

                var dataLayer = new StudentDatalayer();
                int id = dataLayer.AddStudentNickname(request);

                response.StudentID = id;
                response.Status = HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                s_logger.Error(ex, "Unable to add a nickname");
                response.Status = HttpStatusCode.InternalServerError;
                response.StatusMessages.Add(new StatusMessage(HttpStatusCode.InternalServerError, "Unable to add a nickname"));
            }

            return response;
        }
    }
}
