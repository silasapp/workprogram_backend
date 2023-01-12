CREATE TABLE [dbo].[MyDesks] (
    [DeskID]      INT           IDENTITY (1, 1) NOT NULL,
    [ProcessID]   INT           NOT NULL,
    [StaffID]     INT           NOT NULL,
    [AppId]       INT           NULL,
    [Sort]        INT           NOT NULL,
    [HasWork]     BIT           NOT NULL,
    [HasPushed]   BIT           NOT NULL,
    [FromStaffID] INT           NULL,
    [FromSBU]     VARCHAR (50)  NULL,
    [CreatedAt]   DATETIME      NOT NULL,
    [UpdatedAt]   DATETIME      NULL,
    [Comment]     VARCHAR (MAX) NULL,
    CONSTRAINT [PK_MyDesk_UT] PRIMARY KEY CLUSTERED ([DeskID] ASC)
);

