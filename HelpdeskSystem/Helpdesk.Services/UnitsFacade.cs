using System;
using System.Net;
using Helpdesk.Common;
using Helpdesk.Common.DTOs;
using Helpdesk.Common.Extensions;
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
    }
}
