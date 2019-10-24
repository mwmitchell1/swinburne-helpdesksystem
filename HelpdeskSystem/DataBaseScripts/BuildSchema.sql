drop database HelpdeskSystem
create database HelpdeskSystem

use HelpdeskSystem

CREATE TABLE [User] (
    UserID INT IDENTITY PRIMARY KEY NOT NULL,
    Username VARCHAR(20) UNIQUE NOT NULL,
    Password TEXT NOT NULL,
    FirstTime BIT NOT NULL
);

CREATE TABLE HelpdeskSettings (
    HelpdeskID INT IDENTITY PRIMARY KEY NOT NULL,
    Name VARCHAR(50) NOT NULL,
    HasCheckIn BIT NOT NULL,
    HasQueue BIT NOT NULL,
    IsDeleted BIT NOT NULL DEFAULT 0
);

CREATE TABLE Unit (
    UnitID INT IDENTITY PRIMARY KEY NOT NULL,
    Code VARCHAR(8) NOT NULL,
    Name VARCHAR(50) NOT NULL,
    IsDeleted BIT NOT NULL DEFAULT 0
);

CREATE TABLE NickNames (
    StudentID INT IDENTITY PRIMARY KEY NOT NULL,
    [SID] VARCHAR(20) NOT NULL,
    NickName VARCHAR(20) UNIQUE NOT NULL
);

CREATE TABLE Topic (
    TopicID INT IDENTITY PRIMARY KEY NOT NULL,
    UnitID INT NOT NULL,
    [Name] VARCHAR(50) NOT NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,
    FOREIGN KEY (UnitID)
        REFERENCES Unit (UnitID)
);

CREATE TABLE QueueItem (
    ItemID INT IDENTITY PRIMARY KEY NOT NULL,
    StudentID INT NOT NULL,
    TopicID INT NOT NULL,
	[Description] VARCHAR(50) NOT NULL,
    TimeAdded DATETIME NOT NULL,
    TimeHelped DATETIME,
    TimeRemoved DATETIME,
    FOREIGN KEY (StudentID)
        REFERENCES NickNames (StudentID),
    FOREIGN KEY (TopicID)
        REFERENCES Topic (TopicID)
);

CREATE TABLE CheckInHistory (
    CheckInID INT IDENTITY PRIMARY KEY NOT NULL,
	StudentID INT NOT NULL,
    UnitID INT NOT NULL,
    CheckInTime DATETIME NOT NULL,
    CheckoutTime DATETIME,
    ForcedCheckout BIT,
	FOREIGN KEY (StudentID)
		REFERENCES NickNames (StudentID),
    FOREIGN KEY (UnitID)
        REFERENCES Unit (UnitID)
);

CREATE TABLE CheckInQueueItem (
    ID INT IDENTITY PRIMARY KEY NOT NULL,
    QueueItemID INT NOT NULL,
    CheckInID INT NOT NULL,
    FOREIGN KEY (QueueItemID)
        REFERENCES QueueItem (ItemID),
    FOREIGN KEY (CheckInID)
        REFERENCES CheckInHistory (CheckInID)
);

CREATE TABLE HelpdeskUnit (
    HelpdeskUnitID INT IDENTITY PRIMARY KEY NOT NULL,
    HelpdeskID INT NOT NULL,
    UnitID INT NOT NULL,
    FOREIGN KEY (HelpdeskID)
        REFERENCES HelpdeskSettings (HelpdeskID),
    FOREIGN KEY (UnitID)
        REFERENCES Unit (UnitID)
);

CREATE TABLE TimeSpans (
    SpanID INT IDENTITY PRIMARY KEY NOT NULL,
    HelpdeskID INT NOT NULL,
    Name VARCHAR(200),
    StartDate DATETIME,
    EndDate DATETIME,
    foreign key (HelpdeskID)
    references HelpdeskSettings (HelpdeskID)
);

INSERT INTO [User] (Username, Password, FirstTime) VALUES ('Admin', 'cMzZAHM41tgd07YnFiG5z5qX6gA=', 0);
--INSERT INTO  HelpdeskSettings (Name, HasCheckIn, HasQueue) VALUES ('Test Helpdesk', 1, 1);
--INSERT INTO Unit (Code, Name) VALUES ('COS00000', 'Test Unit');

--DECLARE @UnitID INT = @@IDENTITY
--INSERT INTO Topic (UnitID, Name, IsDeleted) VALUES (1, 'Test Topic', 0);
--INSERT INTO Topic (UnitID, Name, IsDeleted) VALUES (1, 'Test Topic 2', 0);
--INSERT INTO helpdeskunit (HelpdeskID, UnitID) VALUES(1,1);

GO
CREATE PROCEDURE GetAllHelpdesks
AS
-- =============================================
-- Author:		Wade Russell
-- Create date: 2019-09-18 12:24
-- Description:	Used to get all helpdeks for exporting
-- =============================================
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT *
	FROM HelpdeskSettings
END
GO

CREATE PROCEDURE GetAllTimespans
AS
-- =============================================
-- Author:		Wade Russell
-- Create date: 2019-09-18 12:24
-- Description:	Used to get all timespans for exporting
-- =============================================
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT *
	FROM TimeSpans
END
GO

CREATE PROCEDURE GetAllUsers
AS
-- =============================================
-- Author:		Wade Russell
-- Create date: 2019-09-18 12:40
-- Description:	Used to get all users for exporting
-- =============================================
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT *
	FROM [User]
END
GO

CREATE PROCEDURE GetAllTopics
AS
-- =============================================
-- Author:		Wade Russell
-- Create date: 2019-09-18 12:45
-- Description:	Used to get all topics for exporting
-- =============================================
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT *
	FROM [Topic]
END
GO

CREATE PROCEDURE GetAllNicknames
AS
-- =============================================
-- Author:		Wade Russell
-- Create date: 2019-09-18 12:46
-- Description:	Used to get all nicknames for exporting
-- =============================================
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT *
	FROM [NickNames]
END
GO

CREATE PROCEDURE GetAllCheckins
AS
-- =============================================
-- Author:		Wade Russell
-- Create date: 2019-09-18 12:56
-- Description:	Used to get all checkins for exporting
-- =============================================
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT *
	FROM CheckInHistory
END
GO

CREATE PROCEDURE GetAllQueueItems
AS
-- =============================================
-- Author:		Wade Russell
-- Create date: 2019-09-18 12:56
-- Description:	Used to get all queue items for exporting
-- =============================================
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT *
	FROM QueueItem
END
GO

CREATE PROCEDURE GetAllUnits
AS
-- =============================================
-- Author:		Wade Russell
-- Create date: 2019-09-18 13:03
-- Description:	Used to get all units for exporting
-- =============================================
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT *
	FROM Unit
END
GO

CREATE PROCEDURE GetAllHelpdeskUnits
AS
-- =============================================
-- Author:		Wade Russell
-- Create date: 2019-09-18 12:58
-- Description:	Used to get all helpdesk units for exporting
-- =============================================
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT *
	FROM HelpdeskUnit
END
GO

CREATE PROCEDURE GetAllCheckInQueueItems
AS
-- =============================================
-- Author:		Wade Russell
-- Create date: 2019-09-18 13:00
-- Description:	Used to get all check in queue items for exporting
-- =============================================
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT *
	FROM CheckInQueueItem
END
GO

/*
CREATE PROCEDURE GetRepeatVisitsSingle
    @pHelpdeskID INT,
    @pStartDate DATETIME,
    @pEndDate DATETIME
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @tHelpDeskCheckInJoin TABLE (HelpdeskID INT, CheckInID INT, CheckInTime DATETIME)
    DECLARE @tRepeats TABLE (HelpdeskID INT, StudentID INT, CheckInID INT, CheckInTime DATETIME)
    DECLARE @tRepeatCheck TABLE (StudentID INT)
    DECLARE @vStudentID INT
    DECLARE @vCheckInID INT
    DECLARE @vCheckInTime DATETIME

    DECLARE sorter CURSOR FOR
        SELECT *
        FROM @tHelpDeskCheckInJoin

    -- First get all checkins that match the requested helpdesk id and are within the start and end dates.
    INSERT INTO @tHelpDeskCheckInJoin(HelpdeskID, CheckInId, CheckInTime)
    SELECT      HelpdeskSettings.HelpdeskID,
                CheckInHistory.CheckInID, 
                CheckInHistory.CheckInTime
    FROM        CheckInHistory
                INNER JOIN HelpdeskUnit ON CheckInHistory.UnitID = HelpdeskUnit.UnitID
                INNER JOIN HelpdeskSettings ON HelpdeskUnit.HelpdeskID = HelpdeskSettings.HelpdeskID
                INNER JOIN NickNames ON NickNames.StudentID = CheckInHistory.StudentID
    WHERE       @pHelpdeskID = HelpdeskSettings.HelpdeskID
                AND CheckInHistory.CheckInTime >= @pStartDate
                AND CheckInHistory.CheckInTime <= @pEndDate;
    
    -- Loop through filtered temp table @tHelpDeskCheckInJoin and check if its CheckInId exists in @tRepeatCheck.
    -- If if does, add the row to tRepeats; if not, add it to tRepeatCheck.
    -- If it wasn't added the first time and the same CheckInId 
    WHILE 1=1
    BEGIN
        FETCH NEXT FROM sorter
            INTO @vCheckInID, @vCheckInTime
        IF @@FETCH_STATUS <> 0
        BEGIN
            -- Fetched all rows; break loop.
            BREAK
        END
        ELSE IF NOT EXISTS (SELECT StudentID FROM @tRepeatCheck WHERE StudentID = @vStudentID)
        BEGIN
            INSERT INTO @tRepeatCheck (StudentID)
            VALUES (@vStudentID)
        END
        ELSE
        BEGIN
            INSERT INTO @tRepeats (CheckInID, CheckInTime)
            VALUES (@vCheckInID, @vCheckInTime)
        END
    END

    SELECT * FROM @tRepeats
    RETURN
END
GO
*/