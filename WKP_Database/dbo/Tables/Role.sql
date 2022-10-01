CREATE TABLE [dbo].[Role] (
    [id]          INT           IDENTITY (1, 1) NOT NULL,
    [RoleId]      VARCHAR (20)  NOT NULL,
    [Description] VARCHAR (50)  NULL,
    [RoleName]    VARCHAR (100) NULL,
    CONSTRAINT [PK_Role] PRIMARY KEY CLUSTERED ([RoleId] ASC)
);

