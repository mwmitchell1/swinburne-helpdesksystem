using Helpdesk.Common;
using Helpdesk.Common.DTOs;
using Helpdesk.Common.Extensions;
using Helpdesk.Common.Requests.Students;
using Helpdesk.Common.Responses;
using Helpdesk.Common.Responses.Students;
using Helpdesk.Common.Utilities;
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
        /// Used to get all of the student nicknames
        /// </summary>
        /// <returns>The response with the nickname list</returns>
        public GetAllNicknamesResponse GetAllNicknames()
        {
            GetAllNicknamesResponse response = new GetAllNicknamesResponse();

            try
            {
                var dataLayer = new StudentDatalayer();
                response.Nicknames = dataLayer.GetAllNicknames();

                if (response.Nicknames.Count > 0)
                    response.Status = HttpStatusCode.OK;
                else
                    response.Status = HttpStatusCode.NotFound;
            }
            catch (Exception ex)
            {
                s_logger.Error(ex, "Unable to a get nicknames");
                response.Status = HttpStatusCode.InternalServerError;
                response.StatusMessages.Add(new StatusMessage(HttpStatusCode.InternalServerError, "Unable to get nicknames"));
            }
            return response;
        }

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

        /// <summary>
        /// This method is responsible for updating a specific students's nickname
        /// </summary>
        /// <param name="id">The StudentID of the student to be updated</param>
        /// <param name="request">The student's new nickname</param>
        /// <returns>The response that indicates if the update was successfull</returns>
        public EditStudentNicknameResponse EditStudentNickname(int id, EditStudentNicknameRequest request)
        {
            s_logger.Info("Editing student's nickname...");

            EditStudentNicknameResponse response = new EditStudentNicknameResponse();

            try
            {
                response = (EditStudentNicknameResponse)request.CheckValidation(response);

                if (response.Status == HttpStatusCode.BadRequest)
                    return response;

                var dataLayer = new StudentDatalayer();

                var nickname = dataLayer.GetStudentNicknameByNickname(request.Nickname);

                if (nickname != null)
                {
                    response.Status = HttpStatusCode.BadRequest;
                    response.StatusMessages.Add(new StatusMessage(HttpStatusCode.BadRequest, "This nickname is already being used."));
                    return response;
                }

                bool result = dataLayer.EditStudentNickname(id, request);

                if (result == false)
                    throw new NotFoundException("Unable to find student!");

                response.Result = result;
                response.Status = HttpStatusCode.OK;

            }
            catch (NotFoundException ex)
            {
                s_logger.Error(ex, "Unable to find student!");
                response.Status = HttpStatusCode.NotFound;
                response.StatusMessages.Add(new StatusMessage(HttpStatusCode.NotFound, "Unable to find student!"));
            }
            catch (Exception ex)
            {
                s_logger.Error(ex, "Unable to update student's nickname!");
                response.Status = HttpStatusCode.InternalServerError;
                response.StatusMessages.Add(new StatusMessage(HttpStatusCode.InternalServerError, "Unable to update student's nickname!"));
            }
            return response;
        }

        /// <summary>
        /// This method is responsible for validating a given student's nickname
        /// </summary>
        /// <param name="request">Request that contains the nickname information</param>
        /// <returns>Response which indicates success or failure</returns>
        public ValidateNicknameResponse ValidateNickname(ValidateNicknameRequest request)
        {
            var response = new ValidateNicknameResponse();

            try
            {
                var dataLayer = new StudentDatalayer();

                var existingNickname = dataLayer.GetStudentNicknameByNickname(request.Name);

                if (existingNickname == null)
                {
                    if (!string.IsNullOrEmpty(request.SID))
                    {
                        existingNickname = dataLayer.GetStudentNicknameByStudentID(request.SID);

                        if (existingNickname == null)
                        {
                            response.Status = HttpStatusCode.OK;
                        }
                        else
                        {
                            response.SID = existingNickname.ID;
                            response.StudentId = existingNickname.StudentID;
                            response.Nickname = existingNickname.Nickname;
                            response.Status = HttpStatusCode.Accepted;
                        }
                    }
                    else
                    {
                        response.Status = HttpStatusCode.NotFound;
                    }
                }
                else if (existingNickname.StudentID == request.SID || string.IsNullOrEmpty(request.SID))
                {
                    response.Nickname = existingNickname.Nickname;
                    response.StudentId = existingNickname.StudentID;
                    response.SID = existingNickname.ID;
                    response.Status = HttpStatusCode.Accepted;
                }
                else
                {
                    response.Status = HttpStatusCode.BadRequest;
                }
            }
            catch (Exception ex)
            {
                s_logger.Error(ex, "Unable to validate student's nickname!");
                response.Status = HttpStatusCode.InternalServerError;
                response.StatusMessages.Add(new StatusMessage(HttpStatusCode.InternalServerError, "Unable to validate student's nickname!"));
            }

            return response;
        }

        /// <summary>
        /// Used to generate a new random nickname for admin purposes
        /// </summary>
        /// <returns>The response containing the nickname</returns>
        public GenerateNicknameResponse GenerateNickname()
        {
            var response = new GenerateNicknameResponse();
            try
            {
                string nickname = AlphaNumericStringGenerator.GetString(20);

                var dataLayer = new StudentDatalayer();

                var existingUsername = dataLayer.GetStudentNicknameByNickname(nickname);

                while (existingUsername != null)
                {
                    nickname = AlphaNumericStringGenerator.GetString(20);
                    existingUsername = dataLayer.GetStudentNicknameByNickname(nickname);
                }

                response.Nickname = nickname;
                response.Status = HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                s_logger.Error(ex, "Unable to generate nickname!");
                response.Status = HttpStatusCode.InternalServerError;
                response.StatusMessages.Add(new StatusMessage(HttpStatusCode.InternalServerError, "Unable to generate nickname!"));
            }
            return response;
        }
    }
}
