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
INSERT INTO  HelpdeskSettings (Name, HasCheckIn, HasQueue) VALUES ('Test Helpdesk', 1, 1);
INSERT INTO Unit (Code, Name) VALUES ('COS00000', 'Test Unit');
INSERT INTO helpdeskunit (HelpdeskID, UnitID) VALUES(1,1);

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

CREATE PROCEDURE GetAllTiimespans
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