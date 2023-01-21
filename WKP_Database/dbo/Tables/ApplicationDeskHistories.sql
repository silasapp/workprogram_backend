CREATE TABLE [dbo].[ApplicationDeskHistories] (
    [Id]        INT           IDENTITY (10849, 1) NOT NULL,
    [AppId]     INT           NOT NULL,
    [StaffID]   INT           NULL,
    [Comment]   VARCHAR (MAX) NOT NULL,
    [Status]    NVARCHAR (50) NOT NULL,
    [CreatedAt] DATETIME2 (0) NOT NULL,
    CONSTRAINT [PK_ApplicationDeskHistories] PRIMARY KEY CLUSTERED ([Id] ASC)
);



