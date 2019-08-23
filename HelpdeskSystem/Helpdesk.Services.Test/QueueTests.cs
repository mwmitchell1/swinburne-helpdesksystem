using Helpdesk.Common.Requests.Queue;
using Helpdesk.Common.Responses.Queue;
using Helpdesk.Common.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using Helpdesk.Data.Models;
using System.Net;
using System.Linq;

namespace Helpdesk.Services.Test
{
    /// <summary>
    /// Used to test queue related code
    /// </summary>
    [TestClass]
    public class QueueTests
    {
        /// <summary>
        /// Used to ensure that joining the queue works
        /// </summary>
        [TestMethod]
        public void JoinQueueNewStudentNoCheckInNewStudent()
        {
            Helpdesksettings helpdesk = new Helpdesksettings()
            {
                HasQueue = true,
                HasCheckIn = false,
                IsDeleted = false,
                Name = AlphaNumericStringGenerator.GetString(10),
            };

            Unit unit = new Unit()
            {
                Code = AlphaNumericStringGenerator.GetString(8),
                IsDeleted = false,
                Name = AlphaNumericStringGenerator.GetString(10),
            };

            Topic topic = new Topic()
            {
                IsDeleted = false,
                Name = AlphaNumericStringGenerator.GetString(10),
            };

            using (helpdesksystemContext context = new helpdesksystemContext())
            {
                context.Helpdesksettings.Add(helpdesk);
                context.Unit.Add(unit);
                context.SaveChanges();

                Helpdeskunit helpdeskunit = new Helpdeskunit()
                {
                    HelpdeskId = helpdesk.HelpdeskId,
                    UnitId = unit.UnitId
                };

                context.SaveChanges();

                topic.UnitId = unit.UnitId;
                context.Topic.Add(topic);
                context.SaveChanges();
            }

            AddToQueueRequest request = new AddToQueueRequest()
            {
                Nickname = AlphaNumericStringGenerator.GetString(10),
                SID = AlphaNumericStringGenerator.GetStudentIDString(),
                TopicID = topic.TopicId,
            };

            QueueFacade facade = new QueueFacade();

            AddToQueueResponse response = facade.AddToQueue(request);

            Assert.AreEqual(HttpStatusCode.OK, response.Status);
            Assert.IsTrue(response.ItemId > 0);

            using (helpdesksystemContext context = new helpdesksystemContext())
            {
                Queueitem queueitem = context.Queueitem.FirstOrDefault(qi => qi.ItemId == response.ItemId);

                Assert.AreEqual(request.TopicID, queueitem.TopicId);
                Assert.IsTrue(DateTime.Now.Date < queueitem.TimeAdded);

                Nicknames nicknames = context.Nicknames.FirstOrDefault(n => n.StudentId == queueitem.StudentId);

                Assert.AreEqual(request.Nickname, nicknames.NickName);
                Assert.AreEqual(request.SID, nicknames.Sid);
            }
        }
    }
}
