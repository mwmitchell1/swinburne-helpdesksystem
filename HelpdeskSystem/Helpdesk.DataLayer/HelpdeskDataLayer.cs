using Helpdesk.Common.DTOs;
using Helpdesk.Common.Requests.Helpdesk;
using System;
using Helpdesk.Data.Models;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Helpdesk.DataLayer
{
    /// <summary>
    /// Used to handle any databases interactions for helpdesks including CRUD & timespans
    /// </summary>
    public class HelpdeskDataLayer
    {
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
