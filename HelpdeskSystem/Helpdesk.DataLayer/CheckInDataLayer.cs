using Helpdesk.Common.Extensions;
using Helpdesk.Common.Requests.CheckIn;
using Helpdesk.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
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
                    CheckInTime = DateTime.Now,
                    StudentId = request.StudentID
                };

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
        public bool CheckOut(CheckOutRequest request, int id)
        {
            using (helpdesksystemContext context = new helpdesksystemContext())
            {
                Checkinhistory checkOut = context.Checkinhistory.FirstOrDefault(co => co.CheckInId == id);

                if (checkOut == null)
                    return false;

                checkOut.CheckoutTime = DateTime.Now;
                checkOut.ForcedCheckout = request.ForcedCheckout;

                context.SaveChanges();
            }
            return true;
        }

        /// Used to get a datatable with all of the checkin records
        /// </summary>
        /// <returns>Datatable with the checkin records</returns>
        public DataTable GetCheckInsAsDataTable()
        {
            DataTable checkIns = new DataTable();

            using (helpdesksystemContext context = new helpdesksystemContext())
            {
                DbConnection conn = context.Database.GetDbConnection();
                ConnectionState state = conn.State;

                try
                {
                    if (state != ConnectionState.Open)
                        conn.Open();

                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "GetAllCheckins";
                        cmd.CommandType = CommandType.StoredProcedure;
                        using (var reader = cmd.ExecuteReader())
                        {
                            checkIns.Load(reader);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    if (state != ConnectionState.Closed)
                        conn.Close();
                }
            }

            return checkIns;
        }

        /// Used to get a datatable with all of the checkinqueueitem records
        /// </summary>
        /// <returns>Datatable with the checkinqueueitem records</returns>
        public DataTable GetCheckInQueueItemsAsDataTable()
        {
            DataTable checkInQueueItems = new DataTable();

            using (helpdesksystemContext context = new helpdesksystemContext())
            {
                DbConnection conn = context.Database.GetDbConnection();
                ConnectionState state = conn.State;

                try
                {
                    if (state != ConnectionState.Open)
                        conn.Open();

                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "GetAllCheckInQueueItems";
                        cmd.CommandType = CommandType.StoredProcedure;
                        using (var reader = cmd.ExecuteReader())
                        {
                            checkInQueueItems.Load(reader);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    if (state != ConnectionState.Closed)
                        conn.Close();
                }
            }

            return checkInQueueItems;
        }
    }
}
