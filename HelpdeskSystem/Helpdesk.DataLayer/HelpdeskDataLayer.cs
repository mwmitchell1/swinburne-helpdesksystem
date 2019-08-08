using Helpdesk.Common.DTOs;
using Helpdesk.Common.Requests.Helpdesk;
using System;
using Helpdesk.Data.Models;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Helpdesk.DataLayer
{
    public class HelpdeskDataLayer
    {
        public int? AddTimeSpan(AddTimeSpanRequest request)
        {
            throw new NotImplementedException();
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
