﻿using System;
using System.Collections.Generic;
using System.Net;
using Helpdesk.Common;
using Helpdesk.Common.DTOs;
using Helpdesk.Common.Extensions;
using Helpdesk.Common.Responses;
using Helpdesk.Common.Responses.Units;
using Helpdesk.DataLayer;
using NLog;

namespace Helpdesk.Services
{
    public class UnitsFacade : ILoginClass
    {
        private static Logger s_logger = LogManager.GetCurrentClassLogger();

        private readonly AppSettings _appSettings;

        public UnitsFacade()
        {
            _appSettings = new AppSettings();
        }

        /// <summary>
        /// Attempt to retrieve a unit from the database matching the provided id.
        /// </summary>
        /// <param name="id">The id of the unit to retrieve from the database.</param>
        /// <returns></returns>
        public GetUnitResponse GetUnit(int id)
        {
            s_logger.Info("Getting unit by id...");

            var response = new GetUnitResponse();

            try
            {
                var dataLayer = new UnitsDataLayer();
                UnitDTO result = dataLayer.GetUnit(id);

                if (result != null)
                    response.Unit = result;
                    response.Status = HttpStatusCode.OK;
            }
            catch (NotFoundException ex)
            {
                s_logger.Warn(ex, $"Unable to find the unit with id [{id}]");
                response.Status = HttpStatusCode.NotFound;
            }

            return response;
        }

        /// <summary>
        /// Attempts to retrieve all units under a specific helpdesk id from the database
        /// </summary>
        /// <param name="id">ID of the helpdesk to be retrieved from</param>
        /// <returns>A response containing the list of units and status code representing the result</returns>
        public GetUnitsByHelpdeskIDResponse GetUnitsByHelpdeskID(int id)
        {
            s_logger.Info("Getting units by helpdesk id...");

            var response = new GetUnitsByHelpdeskIDResponse();

            try
            {
                var dataLayer = new UnitsDataLayer();
                List<UnitDTO> units = dataLayer.GetUnitsByHelpdeskID(id);

                if(units.Count==0)
                {
                    throw new NotFoundException("No units found under helpdesk "+id);
                }

                response.Units = units;
                response.Status = HttpStatusCode.OK;
            }
            catch(NotFoundException ex)
            {
                s_logger.Error(ex, "No units found!");
                response.Status = HttpStatusCode.NotFound;
                response.StatusMessages.Add(new StatusMessage(HttpStatusCode.NotFound, "No units found!"));
            }
            catch(Exception ex)
            {
                s_logger.Error(ex, "Unable to get units!");
                response.Status = HttpStatusCode.InternalServerError;
                response.StatusMessages.Add(new StatusMessage(HttpStatusCode.InternalServerError, "Unable to get units!"));
            }

            return response;
        }

        /// <summary>
        /// Attempts to delete a specific unit from the database
        /// </summary>
        /// <param name="id">ID of the unit to be deleted</param>
        /// <returns>A response indicating the result of the operation</returns>
        public DeleteUnitResponse DeleteUnit(int id)
        {
            var response = new DeleteUnitResponse();

            try
            {
                var dataLayer = new UnitsDataLayer();
                bool result = dataLayer.DeleteUnit(id);

                if (result)
                    response.Status = HttpStatusCode.OK;
            }
            catch(NotFoundException ex)
            {
                s_logger.Warn($"Unable to find the unit with id [{id}]");
                response.Status = HttpStatusCode.NotFound;
            }
            catch (Exception ex)
            {
                s_logger.Error(ex, "Unable to delete the unit.");
                response.Status = HttpStatusCode.InternalServerError;
            }

            return response;
        }
    }
}