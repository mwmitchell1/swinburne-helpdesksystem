using Helpdesk.Common.DTOs;
using Helpdesk.Common.Requests.Helpdesk;
using System;
using Helpdesk.Data.Models;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Helpdesk.Common.Extensions;

namespace Helpdesk.DataLayer
{
    /// <summary>
    /// This is used to before CRUD actions for the helpdesks and timespans
    /// </summary>
    public class HelpdeskDataLayer
    {
        /// <summary>
        /// This method is used to add a new helpdesk to the database
        /// </summary>
        /// <param name="request">The information of the helpdesk</param>
        /// <returns>The id of the helpdesk that was added</returns>
        public int? AddHelpdesk(AddHelpdeskRequest request)
        {
            int? helpdeskId = null;

            Helpdesksettings helpdesk = new Helpdesksettings();
            helpdesk.Name = request.Name;
            helpdesk.HasCheckIn = request.HasCheckIn;
            helpdesk.HasQueue = request.HasQueue;

            using (helpdesksystemContext context = new helpdesksystemContext())
            {
                context.Add(helpdesk);
                context.SaveChanges();
                helpdeskId = helpdesk.HelpdeskId;
            }

            return helpdeskId;
        }
   
        /// <summary>
        /// This method is used to update the relevent helpdesk
        /// </summary>
        /// <param name="id">The id of the helpdesk to be updated</param>
        /// <param name="request">The information to update the helpdesk</param>
        /// <returns>Result the indicates whether or not the update was successful</returns>
        public bool UpdateHelpdesk(int id, UpdateHelpdeskRequest request)
        {
            bool result = false;

            using (helpdesksystemContext context = new helpdesksystemContext())
            {
                Helpdesksettings helpdesk = context.Helpdesksettings.FirstOrDefault(p => p.HelpdeskId == id);

                if (helpdesk == null)
                    throw new NotFoundException($"Helpdesk with id [{id}] not found.");

                helpdesk.Name = request.Name;
                helpdesk.HasCheckIn = request.HasCheckIn;
                helpdesk.HasQueue = request.HasQueue;

                context.SaveChanges();
                result = true;
            }

            return result;
        }

        /// <summary>
        /// This method adds a timespan to the database.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public int? AddTimeSpan(AddTimeSpanRequest request)
        {
            int? spanId = null;

            Timespans timespan = new Timespans();
            timespan.Name = request.Name;
            timespan.StartDate = request.StartDate;
            timespan.EndDate = request.EndDate;
            using (var context = new helpdesksystemContext())
            {
                context.Timespans.Add(timespan);
                context.SaveChanges();
                spanId = timespan.SpanId;
            }
            return spanId;
        }

        public TimeSpanDTO GetTimeSpan(int id)
        {
            throw new NotImplementedException();
        }

        public List<TimeSpanDTO> GetTimeSpans()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// This method updates a specified timespan's information in the database
        /// </summary>
        /// <param name="id">The SpanId of the timespan to be updated</param>
        /// <param name="request">The request that contains the timespan's new information</param>
        /// <returns>A bool indicating whether the operation was a success</returns>
        public bool UpdateTimeSpan(int id, UpdateTimeSpanRequest request)
        {

            using (helpdesksystemContext context = new helpdesksystemContext())
            {
                Timespans timespan = context.Timespans.Single(t => t.SpanId == id);

                if (timespan == null)
                {
                    return false;
                }

                timespan.Name = request.Name;
                timespan.StartDate = request.StartDate;
                timespan.EndDate = request.EndDate;

                context.SaveChanges();
            }
            return true;
        }

        public bool DeleteTimeSpan(int id)
        {
            throw new NotImplementedException();
        }
    }
}
