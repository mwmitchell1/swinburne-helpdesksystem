using Helpdesk.Common.Extensions;
using Helpdesk.Common.Requests.CheckIn;
using Helpdesk.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Helpdesk.DataLayer
{
    /// <summary>
    /// Used to handle any database interactions involving checking in and out of a helpdesk
    /// </summary>
    public class CheckInDataLayer
    {
        /// <summary>
        /// Checks a new item into the database
        /// </summary>
        /// <param name="request">Request containing the unit id of the check in item</param>
        /// <returns>The id of the new check in item</returns>
        public int CheckIn(CheckInRequest request)
        {
            int id = 0;

            using (helpdesksystemContext context = new helpdesksystemContext())
            {
                Checkinhistory checkIn = new Checkinhistory()
                {
                    UnitId = request.UnitID,
                    CheckInTime = DateTime.Now
                };

                if (!request.StudentID.HasValue)
                {
                    Nicknames student = new Nicknames()
                    {
                        Sid = request.SID,
                        NickName = request.Nickname
                    };

                    context.Nicknames.Add(student);
                    context.SaveChanges();

                    checkIn.StudentId = student.StudentId;
                }
                else
                {
                    checkIn.StudentId = request.StudentID;
                }

                context.Checkinhistory.Add(checkIn);
                context.SaveChanges();

                id = checkIn.CheckInId;
            }
            return id;
        }

        /// <summary>
        /// Checks a check in item out of the database
        /// </summary>
        /// <param name="id">CheckInID of the check in item to be checked out</param>
        /// <returns>A boolean indicating success or failure</returns>
        public bool CheckOut(int id)
        {
            using (helpdesksystemContext context = new helpdesksystemContext())
            {
                Checkinhistory checkOut = context.Checkinhistory.FirstOrDefault(co => co.CheckInId == id);

                if (checkOut == null)
                    return false;

                checkOut.CheckoutTime = DateTime.Now;
                checkOut.ForcedCheckout = 0;

                context.SaveChanges();
            }
            return true;
        }
    }
}
