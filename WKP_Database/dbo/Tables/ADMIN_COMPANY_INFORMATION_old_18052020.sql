﻿CREATE TABLE [dbo].[ADMIN_COMPANY_INFORMATION_old_18052020] (
    [Id]                   INT           IDENTITY (1, 1) NOT NULL,
    [COMPANY_NAME]         VARCHAR (200) NULL,
    [EMAIL]                VARCHAR (200) NULL,
    [PASSWORDS]            VARCHAR (500) NULL,
    [Created_by]           VARCHAR (100) NULL,
    [Date_Created]         DATETIME      NULL,
    [LAST_LOGIN_DATE]      DATETIME      NULL,
    [STATUS_]              VARCHAR (200) NULL,
    [FLAG_PASSWORD_CHANGE] VARCHAR (500) NULL,
    [CATEGORY]             VARCHAR (50)  NULL,
    [NAME]                 VARCHAR (50)  NULL,
    [DESIGNATION]          VARCHAR (50)  NULL,
    [PHONE_NO]             VARCHAR (50)  NULL,
    [CompanyNumber]        INT           NULL,
    CONSTRAINT [PK_ADMIN_COMPANY_INFORMATION_old_18052020] PRIMARY KEY CLUSTERED ([Id] ASC)
);



