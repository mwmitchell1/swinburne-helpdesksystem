using Helpdesk.Common;
using Helpdesk.Common.DTOs;
using Helpdesk.Common.Extensions;
using Helpdesk.Common.Requests.Helpdesk;
using Helpdesk.Common.Responses;
using Helpdesk.Common.Responses.Helpdesk;
using Helpdesk.Common.Utilities;
using Helpdesk.DataLayer;
using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;

namespace Helpdesk.Services
{
    /// <summary>
    /// Used to handle business logic related to helpdesks and their report timespans
    /// </summary>
    public class HelpdeskFacade
    {
        private static Logger s_logger = LogManager.GetCurrentClassLogger();

        private readonly AppSettings _appSettings;

        public HelpdeskFacade()
        {
            _appSettings = new AppSettings();
        }

        /// <summary>
        /// This method is to handle adding helpdesk business logic
        /// </summary>
        /// <param name="request">This is the request with the info to add the helpdesk</param>
        /// <returns>Returns a response with the id and indications of success</returns>
        public AddHelpdeskResponse AddHelpdesk(AddHelpdeskRequest request)
        {
            var response = new AddHelpdeskResponse();

            try
            {
                response = (AddHelpdeskResponse)request.CheckValidation(response);

                if (response.Status == HttpStatusCode.BadRequest)
                    return response;

                var dataLayer = new HelpdeskDataLayer();
                int? helpdeskId = dataLayer.AddHelpdesk(request);
                if (helpdeskId.HasValue)
                {
                    response.HelpdeskID = helpdeskId.Value;
                    response.Status = HttpStatusCode.OK;
                }
            }
            catch (Exception ex)
            {
                response.Status = HttpStatusCode.InternalServerError;
                response.StatusMessages.Add(new StatusMessage(HttpStatusCode.InternalServerError, "Unable to add helpdesk"));
                s_logger.Error(ex, "Unable to add helpdesk.");
            }

            return response;
        }

        /// <summary>
        /// Used to return all of the active helpdesks
        /// </summary>
        /// <returns>A response indicating the success and a list of active helpdesks</returns>
        public GetHelpdesksResponse GetActiveHelpdesks()
        {
            var response = new GetHelpdesksResponse();

            try
            {
                var dataLayer = new HelpdeskDataLayer();
                response.Helpdesks = dataLayer.GetActiveHelpdesks();
                response.Status = HttpStatusCode.OK;
            }
            catch (NotFoundException ex)
            {
                response.Status = HttpStatusCode.NotFound;
                response.StatusMessages.Add(new StatusMessage(HttpStatusCode.NotFound, "Unable to get helpdesk"));
                s_logger.Warn(ex, "Unable to find helpdesks.");
            }
            catch (Exception ex)
            {
                response.Status = HttpStatusCode.InternalServerError;
                response.StatusMessages.Add(new StatusMessage(HttpStatusCode.InternalServerError, "Unable to get helpdesks"));
                s_logger.Error(ex, "Unable to get helpdesks.");
            }
            return response;
        }

        /// <summary>
        /// Used to return all of the helpdesks
        /// </summary>
        /// <returns>A response indicating the success and a list of helpdesks</returns>
        public GetHelpdesksResponse GetHelpdesks()
        {
            var response = new GetHelpdesksResponse();

            try
            {
                var dataLayer = new HelpdeskDataLayer();
                response.Helpdesks = dataLayer.GetHelpdesks();
                response.Status = HttpStatusCode.OK;
            }
            catch (NotFoundException ex)
            {
                response.Status = HttpStatusCode.NotFound;
                response.StatusMessages.Add(new StatusMessage(HttpStatusCode.NotFound, "Unable to get helpdesk"));
                s_logger.Warn(ex, "Unable to find helpdesks.");
            }
            catch (Exception ex)
            {
                response.Status = HttpStatusCode.InternalServerError;
                response.StatusMessages.Add(new StatusMessage(HttpStatusCode.InternalServerError, "Unable to get helpdesks"));
                s_logger.Error(ex, "Unable to get helpdesks.");
            }
            return response;
        }

        /// <summary>
        /// Used to get a helpdesk using it's id
        /// </summary>
        /// <param name="id">The id of the helpdesk to be retreived</param>
        /// <returns>A response indicating the success and helpdesk DTO or null</returns>
        public GetHelpdeskResponse GetHelpdesk(int id)
        {
            var response = new GetHelpdeskResponse();

            try
            {
                var dataLayer = new HelpdeskDataLayer();
                response.Helpdesk = dataLayer.GetHelpdesk(id);
                response.Status = HttpStatusCode.OK;
            }
            catch (NotFoundException ex)
            {
                response.Status = HttpStatusCode.NotFound;
                response.StatusMessages.Add(new StatusMessage(HttpStatusCode.NotFound, "Unable to get helpdesk"));
                s_logger.Error(ex, $"Unable to find helpdesk with id [{id}].");
            }
            catch (Exception ex)
            {
                response.Status = HttpStatusCode.InternalServerError;
                response.StatusMessages.Add(new StatusMessage(HttpStatusCode.InternalServerError, "Unable to get helpdesk"));
                s_logger.Error(ex, $"Unable to get helpdesk with id [{id}].");
            }
            return response;
        }

        /// <summary>
        /// This method is to handle updating helpdesk business logic
        /// </summary>
        /// <param name="id">The id of the helpdesk to be updated</param>
        /// <param name="request">This is the request with the info to update the helpdesk</param>
        /// <returns>Returns a response which indicate the result</returns>
        public UpdateHelpdeskResponse UpdateHelpdesk(int id, UpdateHelpdeskRequest request)
        {
            var response = new UpdateHelpdeskResponse();

            try
            {
                response = (UpdateHelpdeskResponse)request.CheckValidation(response);

                if (response.Status == HttpStatusCode.BadRequest)
                    return response;

                var dataLayer = new HelpdeskDataLayer();
                bool result = dataLayer.UpdateHelpdesk(id, request);

                if (result)
                    response.Status = HttpStatusCode.OK;
                else
                {
                    response.Status = HttpStatusCode.BadRequest;
                    response.StatusMessages.Add(new StatusMessage(HttpStatusCode.BadRequest, "Unable to update helpdesk."));
                }
            }
            catch (NotFoundException ex)
            {
                response.Status = HttpStatusCode.NotFound;
                response.StatusMessages.Add(new StatusMessage(HttpStatusCode.NotFound, "Unable to update to find helpdesk"));
                s_logger.Error(ex, "Unable to find helpdesk.");
            }
            catch (Exception ex)
            {
                response.Status = HttpStatusCode.InternalServerError;
                response.StatusMessages.Add(new StatusMessage(HttpStatusCode.InternalServerError, "Unable to update helpdesk"));
                s_logger.Error(ex, "Unable to update helpdesk.");
            }

            return response;
        }

        /// <summary>
        /// This method is responsible for retrieving all timespans from the helpdesk system
        /// </summary>
        /// <returns>The response that indicates if the operation was a success,
        /// and the list of timespans</returns>
        public GetTimeSpansResponse GetTimeSpans()
        {
            s_logger.Info("Getting timespans...");

            GetTimeSpansResponse response = new GetTimeSpansResponse();

            try
            {
                var dataLayer = new HelpdeskDataLayer();

                List<TimeSpanDTO> timespans = dataLayer.GetTimeSpans();

                response.Timespans = timespans;
                response.Status = HttpStatusCode.OK;
            }
            catch (NotFoundException ex)
            {
                s_logger.Error(ex, "No timespans found!");
                response.Status = HttpStatusCode.NotFound;
                response.StatusMessages.Add(new StatusMessage(HttpStatusCode.NotFound, "No timespans found!"));
            }
            catch (Exception ex)
            {
                s_logger.Error(ex, "Unable to get timespans!");
                response.Status = HttpStatusCode.InternalServerError;
                response.StatusMessages.Add(new StatusMessage(HttpStatusCode.InternalServerError, "Unable to get timespans!"));
            }
            return response;
        }

        /// <summary>
        /// This method is responsible for getting a specific timespan from the helpdesk system
        /// </summary>
        /// <param name="id">The SpanId of the specific timespan to be retrieved</param>
        /// <returns>The response that indicates if the operation was a success,
        /// and the details of the retrieved timespan if it was</returns>
        public GetTimeSpanResponse GetTimeSpan(int id)
        {
            s_logger.Info("Getting timespan...");

            GetTimeSpanResponse response = new GetTimeSpanResponse();

            try
            {
                var dataLayer = new HelpdeskDataLayer();

                TimeSpanDTO timespan = dataLayer.GetTimeSpan(id);
                response.Timespan = timespan ?? throw new NotFoundException("Unable to find timespan!");
                response.Status = HttpStatusCode.OK;
            }
            catch (NotFoundException ex)
            {
                s_logger.Error(ex, "Unable to find timespan!");
                response.Status = HttpStatusCode.NotFound;
                response.StatusMessages.Add(new StatusMessage(HttpStatusCode.NotFound, "Unable to find timespan!"));
            }
            catch (Exception ex)
            {
                s_logger.Error(ex, "Unable to get timespan!");
                response.Status = HttpStatusCode.InternalServerError;
                response.StatusMessages.Add(new StatusMessage(HttpStatusCode.InternalServerError, "Unable to get timespan!"));
            }
            return response;
        }

        /// <summary>
        /// This method is responsible for adding a new timespan.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public AddTimeSpanResponse AddTimeSpan(AddTimeSpanRequest request)
        {
            s_logger.Info("Adding timespan...");

            AddTimeSpanResponse response = new AddTimeSpanResponse();

            try
            {
                response = (AddTimeSpanResponse)request.CheckValidation(response);
                if (response.Status == HttpStatusCode.BadRequest)
                {
                    return response;
                }

                var dataLayer = new HelpdeskDataLayer();

                int result = dataLayer.AddTimeSpan(request);

                response.SpanId = result;
                response.Status = HttpStatusCode.OK;
            }
            catch (DuplicateNameException ex)
            {
                s_logger.Error(ex, "Timespan name already exists!");
                response.Status = HttpStatusCode.BadRequest;
                response.StatusMessages.Add(new StatusMessage(HttpStatusCode.BadRequest, "Timespan name already exists!"));
            }
            catch (Exception ex)
            {
                s_logger.Error(ex, "Unable to add timespan!");
                response.Status = HttpStatusCode.InternalServerError;
                response.StatusMessages.Add(new StatusMessage(HttpStatusCode.InternalServerError, "Unable to add timespan!"));
            }
            return response;
        }

        /// <summary>
        /// This method is responsible for updating a specific timespan's information
        /// </summary>
        /// <param name="id">The SpanId of the timespan to be updated</param>
        /// <param name="request">The timespan's new information</param>
        /// <returns>The response that indicates if the operation was a success</returns>
        public UpdateTimeSpanResponse UpdateTimeSpan(int id, UpdateTimeSpanRequest request)
        {
            s_logger.Info("Updating timespan...");

            UpdateTimeSpanResponse response = new UpdateTimeSpanResponse();

            try
            {
                response = (UpdateTimeSpanResponse)request.CheckValidation(response);

                if (response.Status == HttpStatusCode.BadRequest)
                    return response;

                var dataLayer = new HelpdeskDataLayer();

                //will implement unique check when get timespan by name method is implemented

                bool result = dataLayer.UpdateTimeSpan(id, request);

                if (result == false)
                    throw new NotFoundException("Unable to find timespan!");

                response.Result = result;
                response.Status = HttpStatusCode.OK;
            }
            catch (NotFoundException ex)
            {
                s_logger.Error(ex, "Unable to find timespan!");
                response.Status = HttpStatusCode.NotFound;
                response.StatusMessages.Add(new StatusMessage(HttpStatusCode.NotFound, "Unable to find timespan!"));
            }
            catch (DuplicateNameException ex)
            {
                s_logger.Error(ex, "Timespan name already exists!");
                response.Status = HttpStatusCode.BadRequest;
                response.StatusMessages.Add(new StatusMessage(HttpStatusCode.BadRequest, "Timespan name already exists!"));
            }
            catch (Exception ex)
            {
                s_logger.Error(ex, "Unable to update timespan!");
                response.Status = HttpStatusCode.InternalServerError;
                response.StatusMessages.Add(new StatusMessage(HttpStatusCode.InternalServerError, "Unable to update timespan!"));
            }
            return response;
        }


        /// <summary>
        /// Use to generate a database export and return it as a stream
        /// </summary>
        /// <returns>Response containing the stream of the file</returns>
        public DatabaseExportResponse ExportDatabaseManual()
        {
            var response = new DatabaseExportResponse();

            try
            {
                response = ExportDatabase();

                if (response.Status != HttpStatusCode.OK)
                    return response;

                response.File = FileToBytes(response.Path);
            }
            catch (Exception ex)
            {
                s_logger.Error(ex, "Unable to perform database export");
                response.Status = HttpStatusCode.InternalServerError;
            }

            return response;
        }

        /// <summary>
        /// Used to get a Zip file of all of the database tables as CSVs
        /// </summary>
        public DatabaseExportResponse ExportDatabase()
        {
            var response = new DatabaseExportResponse();

            try
            {
                FileProccessing proccessing = new FileProccessing();

                DateTime now = DateTime.Now;

                string exportName = $"databaseexport_{now.ToString("yyyyddMM_HHmmss")}";

                string fullZipPath = proccessing.CreateZip(_appSettings.DatabaseBackupDestination, exportName);

                if (string.IsNullOrEmpty(fullZipPath))
                {
                    s_logger.Error("Unable to create empty zip");
                    response.Status = HttpStatusCode.InternalServerError;
                    return response;
                }
                else
                {
                    var helpdeskDataLayer = new HelpdeskDataLayer();
                    var unitDataLayer = new UnitsDataLayer();
                    var usersDataLayer = new UsersDataLayer();
                    var topicsDataLayer = new TopicsDataLayer();
                    var studentDataLayer = new StudentDatalayer();
                    var queueDataLayer = new QueueDataLayer();
                    var checkInDataLayer = new CheckInDataLayer();

                    DataTable helpdesks = helpdeskDataLayer.GetHelpdesksAsDataTable();
                    proccessing.SaveToZIPAsCSV(fullZipPath, "helpdesks", helpdesks);

                    DataTable timespans = helpdeskDataLayer.GetTimeSpansAsDataTable();
                    proccessing.SaveToZIPAsCSV(fullZipPath, "timespans", timespans);

                    DataTable helpdeskUnits = helpdeskDataLayer.GetHelpdeskUnitsAsDataTable();
                    proccessing.SaveToZIPAsCSV(fullZipPath, "helpdeskunits", helpdeskUnits);

                    DataTable users = usersDataLayer.GetUsersAsDataTable();
                    proccessing.SaveToZIPAsCSV(fullZipPath, "users", users);

                    DataTable units = unitDataLayer.GetUnitsAsDataTable();
                    proccessing.SaveToZIPAsCSV(fullZipPath, "units", units);

                    DataTable topics = topicsDataLayer.GetTopicsAsDataTable();
                    proccessing.SaveToZIPAsCSV(fullZipPath, "topics", topics);

                    DataTable students = studentDataLayer.GetStudentsAsDataTable();
                    proccessing.SaveToZIPAsCSV(fullZipPath, "students", students);

                    DataTable queuesItems = queueDataLayer.GetQueueItemsAsDataTable();
                    proccessing.SaveToZIPAsCSV(fullZipPath, "queueItems", queuesItems);

                    DataTable checkIns = checkInDataLayer.GetCheckInsAsDataTable();
                    proccessing.SaveToZIPAsCSV(fullZipPath, "checkInHistory", checkIns);

                    DataTable checkInQueueItems = checkInDataLayer.GetCheckInQueueItemsAsDataTable();
                    proccessing.SaveToZIPAsCSV(fullZipPath, "checkinqueueitem", checkInQueueItems);

                    response.Path = fullZipPath;
                    response.Status = HttpStatusCode.OK;
                }
            }
            catch (Exception ex)
            {
                s_logger.Error(ex, "Unable to generate database export");
                response.Status = HttpStatusCode.InternalServerError;
            }
            return response;
        }

        /// <summary>
        /// Used to load the file at the path specified into a stream
        /// </summary>
        /// <param name="path">The full location of the file</param>
        /// <returns>The file as a stream</returns>
        public byte[] FileToBytes(string path)
        {
            byte[] file = null;

            try
            {
                file = File.ReadAllBytes(path);
            }
            catch (Exception ex)
            {
                s_logger.Error(ex, "Unable to load file to stream");
                file = null;
            }

            return file;
        }

        /// <summary>
        /// Used to force-checkout users and remove queue items.
        /// Takes optional DateTime parameter. If not provided, data layer will use DateTime.Now.
        /// Used by DailyCleanupJob.
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public ForceCheckoutQueueRemoveResponse ForceCheckoutQueueRemove(int id)
        {
            ForceCheckoutQueueRemoveResponse response = new ForceCheckoutQueueRemoveResponse();

            try
            {
                HelpdeskDataLayer dataLayer = new HelpdeskDataLayer();

                response.Result = dataLayer.ForceCheckoutQueueRemove(id);

                if (response.Result == true)
                    response.Status = HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                s_logger.Error(ex, "Unable to force checkout and remove queue items for Helpdesk " + id);
                response.Status = HttpStatusCode.InternalServerError;
                response.StatusMessages.Add(new StatusMessage(HttpStatusCode.InternalServerError, "Unable to force checkout and remove queue items for Helpdesk " + id));
            }

            return response;
        }

        /// <summary>
        /// This method is responsible for handling the deletion of a timespan from the system
        /// </summary>
        /// <param name="id">The SpanID of the timespan to be deleted</param>
        /// <returns>A response indicating success or failure</returns>
        public DeleteTimeSpanResponse DeleteTimeSpan(int id)
        {
            DeleteTimeSpanResponse response = new DeleteTimeSpanResponse();

            try
            {
                var dataLayer = new HelpdeskDataLayer();

                bool result = dataLayer.DeleteTimeSpan(id);

                if (result == false)
                    throw new NotFoundException("Unable to find timespan with id " + id);

                response.Status = HttpStatusCode.OK;
            }
            catch (NotFoundException ex)
            {
                s_logger.Warn($"Unable to find the timespan with id [{id}]");
                response.Status = HttpStatusCode.NotFound;
            }
            catch (Exception ex)
            {
                s_logger.Error(ex, "Unable to delete the timespan.");
                response.Status = HttpStatusCode.InternalServerError;
            }

            return response;
        }
    }
}
