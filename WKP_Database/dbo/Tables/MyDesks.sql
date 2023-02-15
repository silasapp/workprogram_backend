CREATE TABLE [dbo].[MyDesks] (
    [DeskID]      INT            IDENTITY (1, 1) NOT NULL,
    [ProcessID]   INT            NULL,
    [StaffID]     INT            NOT NULL,
    [AppId]       INT            NULL,
    [Sort]        INT            NULL,
    [HasWork]     BIT            NOT NULL,
    [HasPushed]   BIT            NOT NULL,
    [FromStaffID] NVARCHAR (MAX) NULL,
    [FromSBU]     INT            DEFAULT ((0)) NOT NULL,
    [CreatedAt]   DATETIME       NULL,
    [UpdatedAt]   DATETIME       NULL,
    [Comment]     VARCHAR (MAX)  NULL,
    [FromRoleId]  INT            NULL,
    CONSTRAINT [PK_MyDesk_UT] PRIMARY KEY CLUSTERED ([DeskID] ASC)
);



