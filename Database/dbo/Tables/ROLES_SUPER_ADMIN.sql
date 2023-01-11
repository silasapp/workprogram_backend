﻿CREATE TABLE [dbo].[ROLES_SUPER_ADMIN] (
    [SN]             INT           IDENTITY (1, 1) NOT NULL,
    [Role_]          VARCHAR (50)  NULL,
    [Email_]         VARCHAR (100) NULL,
    [Created_by]     VARCHAR (100) NULL,
    [Updated_by]     VARCHAR (100) NULL,
    [Date_Created]   DATETIME      NULL,
    [Date_Updated]   DATETIME      NULL,
    [Deleted_by]     VARCHAR (100) NULL,
    [Deleted_Date]   VARCHAR (100) NULL,
    [Deleted_status] VARCHAR (100) NULL,
    [Description]    VARCHAR (500) NULL
);

