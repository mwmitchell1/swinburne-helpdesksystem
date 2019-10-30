using System;
using System.Collections.Generic;
using Helpdesk.Common.Requests;
using Helpdesk.Common.Requests.CheckIn;
using Helpdesk.Common.Requests.Helpdesk;
using Helpdesk.Common.Requests.Queue;
using Helpdesk.Common.Requests.Students;
using Helpdesk.Common.Requests.Units;
using Helpdesk.Common.Responses;
using Helpdesk.Common.Responses.CheckIn;
using Helpdesk.Common.Responses.Helpdesk;
using Helpdesk.Common.Responses.Queue;
using Helpdesk.Common.Responses.Students;
using Helpdesk.Common.Responses.Units;
using Helpdesk.Common.Utilities;

namespace Helpdesk.Services.Test
{
    /// <summary>
    /// Helpdesk test data to be returned by TestEntityFactory. Includes Request and Response used for DB entity creation.
    /// </summary>
    public class TestDataHelpdesk
    {
        public TestDataHelpdesk (AddHelpdeskRequest request, AddHelpdeskResponse response)
        {
            Request = request;
            Response = response;
        }
        public AddHelpdeskRequest Request { get; }
        public AddHelpdeskResponse Response { get; }
    }

    /// <summary>
    /// TimeSpan test data to be returned by TestEntityFactory. Includes Request and Response used for DB entity creation.
    /// </summary>
    public class TestDataTimeSpan
    {
        public TestDataTimeSpan (AddTimeSpanRequest request, AddTimeSpanResponse response)
        {
            Request = request;
            Response = response;
        }
        public AddTimeSpanRequest Request { get; }
        public AddTimeSpanResponse Response { get; }
    }

    /// <summary>
    /// Unit test data to be returned by TestEntityFactory. Includes Request and Response used for DB entity creation.
    /// </summary>
    public class TestDataUnit
    {
        public TestDataUnit(AddUpdateUnitRequest request, AddUpdateUnitResponse response)
        {
            Request = request;
            Response = response;
        }
        public AddUpdateUnitRequest Request { get; }
        public AddUpdateUnitResponse Response { get; }
    }

    /// <summary>
    /// Queue test data to be returned by TestEntityFactory. Includes Request and Response used for DB entity creation.
    /// </summary>
    public class TestDataQueue
    {
        public TestDataQueue(AddToQueueRequest request, AddToQueueResponse response)
        {
            Request = request;
            Response = response;
        }
        public AddToQueueRequest Request { get; }
        public AddToQueueResponse Response { get; }
    }

    /// <summary>
    /// Check in test data to be returned by TestEntityFactory. Includes Request and Response used for DB entity creation.
    /// </summary>
    public class TestDataCheckIn
    {
        public TestDataCheckIn(CheckInRequest request, CheckInResponse response)
        {
            Request = request;
            Response = response;
        }
        public CheckInRequest Request { get; }
        public CheckInResponse Response { get; }
    }

    /// <summary>
    /// Nickname test data to be returned by TestEntityFactory. Includes Request and Response used for DB entity creation.
    /// </summary>
    public class TestDataNickname
    {
        public TestDataNickname(AddStudentRequest request, AddStudentResponse response)
        {
            Request = request;
            Response = response;
        }
        public AddStudentRequest Request { get; }
        public AddStudentResponse Response { get; }
    }

    /// <summary>
    /// Class with methods for generating test entities on the database.
    /// </summary>
    public class TestEntityFactory
    {
        public TestEntityFactory(bool populateEmptyStrings = true)
        {
            PopulateEmptyStrings = populateEmptyStrings;
        }

        public TestEntityFactory()
        {
            HelpdeskFacade = new HelpdeskFacade();
            UnitsFacade = new UnitsFacade();
            TopicsFacade = new TopicsFacade();
            QueueFacade = new QueueFacade();
            CheckInFacade = new CheckInFacade();
            StudentFacade = new StudentFacade();
        }

        /// <summary>
        /// Adds a test helpdesk in the database.
        /// </summary>
        /// <param name="name">Name of the Helpdesk (auto-generates if not provided, or empty string is provided).</param>
        /// <param name="hasCheckin">Determines if the helpdesk utilises the check-in/check-out functionality.</param>
        /// <param name="hasQueue">Determines if the helpdesk utilises the queue functionality.</param>
        /// <returns></returns>
        public TestDataHelpdesk AddHelpdesk(string name = "", bool? hasCheckin = true, bool? hasQueue = true)
        {
            var request = new AddHelpdeskRequest();
            if (name == "" && PopulateEmptyStrings) request.Name = AlphaNumericStringGenerator.GetString(10); else request.Name = name;

            request.Name = string.IsNullOrEmpty(name) && PopulateEmptyStrings ? AlphaNumericStringGenerator.GetString(10) : name;

            request.HasCheckIn = (bool)hasCheckin;
            request.HasQueue = (bool)hasQueue;

            var response = HelpdeskFacade.AddHelpdesk(request);

            TestDataHelpdesk data = new TestDataHelpdesk(request, response);
            return data;
        }

        public TestDataTimeSpan AddTimeSpan(int? helpdeskID = null, string name = "", DateTime? startDate = null, DateTime? endDate = null)
        {
            var request = new AddTimeSpanRequest();

            if (helpdeskID != null) request.HelpdeskId = (int)helpdeskID;

            if (name == "" && PopulateEmptyStrings) request.Name = AlphaNumericStringGenerator.GetString(10); else request.Name = name;

            if (startDate == null) request.StartDate = DateTime.Now;

            if (endDate == null)  {
                request.EndDate = DateTime.Now;
                request.EndDate.AddDays(1);
            }

            var response = HelpdeskFacade.AddTimeSpan(request);

            TestDataTimeSpan data = new TestDataTimeSpan(request, response);
            return data;
        }

        /// <summary>
        /// Adds a test unit to the database.
        /// </summary>
        /// <param name="unitID">The ID of the unit to update. Should not be provided if adding a new helpdesk.</param>
        /// <param name="helpdeskID">The ID of the helpdesk that the unit is being added to.</param>
        /// <param name="name">Name of the unit (auto-generates if not provided, or empty string is provided).</param>
        /// <param name="code">Code of the unit (auto-generates if not provided, or empty string is provided).</param>
        /// <param name="isDeleted">Whether or not the helpdesk is removed from the system.</param>
        /// <param name="topics">A list of topics associated with the unit.</param>
        /// <returns></returns>
        public TestDataUnit AddUpdateUnit(int unitID = 0, int? helpdeskID = null, string name = "", string code = "", bool? isDeleted = false, List<string> topics = null)
        {
            var request = new AddUpdateUnitRequest();
            if (helpdeskID != null) request.HelpdeskID = (int)helpdeskID;
            if (name != null)
            {
                if (name == "" && PopulateEmptyStrings) request.Name = AlphaNumericStringGenerator.GetString(10); else request.Name = name;
            }
            if (code != null)
            {
                if (code == "" && PopulateEmptyStrings) request.Code = AlphaNumericStringGenerator.GetString(8); else request.Code = code;
            }
            if (isDeleted != null) request.IsDeleted = (bool)isDeleted;
            if (topics != null) request.Topics = topics;

            var facade = new UnitsFacade();
            var response = facade.AddOrUpdateUnit(unitID, request);

            TestDataUnit data = new TestDataUnit(request, response);
            return data;
        }

        public TestDataQueue AddQueueItem(int? studentID = null, int? topicID = null, int? checkInID = null, string nickname = "", string sID = "", string description = "")
        {
            var request = new AddToQueueRequest();

            if (studentID != null) request.StudentID = studentID;
            if (topicID != null) request.TopicID = (int)topicID;
            if (checkInID != null) request.CheckInID = (int)checkInID;
            if (nickname != null)
            {
                if (nickname == "" && PopulateEmptyStrings) request.Nickname = AlphaNumericStringGenerator.GetString(20); else request.Nickname = nickname;
            }
            if (sID != null)
            {
                if (sID == "" && PopulateEmptyStrings) request.SID = AlphaNumericStringGenerator.GetStudentIDString(); else request.SID = sID;
            }
            if (description != null)
            {
                if (description == "" && PopulateEmptyStrings) request.Description = "From Test Factory"; else request.Description = description;
            }

            var response = QueueFacade.AddToQueue(request);

            TestDataQueue data = new TestDataQueue(request, response);
            return data;
        }

        public TestDataCheckIn AddCheckIn(int? studentID = null, string nickname = "", string sID = "", int? unitID = null)
        {
            var request = new CheckInRequest();

            if (studentID != null) request.StudentID = studentID;
            if (nickname != null)
            {
                if (nickname == "" && PopulateEmptyStrings) request.Nickname = AlphaNumericStringGenerator.GetString(20); else request.Nickname = nickname;
            }
            if (sID != null)
            {
                if (sID == "" && PopulateEmptyStrings) request.SID = AlphaNumericStringGenerator.GetStudentIDString(); else request.SID = sID;
            }
            if (unitID != null) request.UnitID = (int)unitID;

            var response = CheckInFacade.CheckIn(request);

            TestDataCheckIn data = new TestDataCheckIn(request, response);
            return data;
        }

        public TestDataNickname AddNickname(string sID = "", string nickname = "")
        {
            var request = new AddStudentRequest();

            if (sID != null)
            {
                if (sID == "" && PopulateEmptyStrings) request.SID = AlphaNumericStringGenerator.GetStudentIDString(); else request.SID = sID;
            }
            if (nickname != null)
            {
                if (nickname == "" && PopulateEmptyStrings) request.Nickname = AlphaNumericStringGenerator.GetString(20); else request.Nickname = nickname;
            }

            var response = StudentFacade.AddStudentNickname(request);

            TestDataNickname data = new TestDataNickname(request, response);
            return data;
        }


        // GETTERS & SETTERS

        public bool PopulateEmptyStrings { get; set; }

        public HelpdeskFacade HelpdeskFacade { get; }
        public UnitsFacade UnitsFacade { get; }
        public QueueFacade QueueFacade { get; }
        public CheckInFacade CheckInFacade { get; }
        public StudentFacade StudentFacade { get; }
        public TopicsFacade TopicsFacade { get; }
    }
}
