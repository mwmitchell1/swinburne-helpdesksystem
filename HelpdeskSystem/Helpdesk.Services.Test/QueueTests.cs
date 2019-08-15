using Helpdesk.Common.Requests.Queue;
using Helpdesk.Common.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Helpdesk.Services.Test
{
    [TestClass]
    public class QueueTests
    {
        [TestMethod]
        public void JoinQueueNewStudentNoCheckIn()
        {
            var request = new AddToQueueRequest()
            {
                Nickname = AlphaNumericStringGenerator.GetString(10),
                SID = AlphaNumericStringGenerator.GetStudentIDString(),
                TopicID = 1
            };
        }
    }
}
