﻿CREATE TABLE [dbo].[ADMIN_CONCESSIONS_INFORMATION] (
    [Consession_Id]        INT            IDENTITY (1, 1) NOT NULL,
    [Company_ID]           VARCHAR (200)  NULL,
    [CompanyName]          VARCHAR (3900) NULL,
<<<<<<< HEAD
=======
    [COMPANY_EMAIL]        VARCHAR (3900) NULL,
    [Concession_Unique_ID] VARCHAR (50)   NULL,
    [Consession_Type]      VARCHAR (50)   NULL,
>>>>>>> origin/main
    [Equity_distribution]  VARCHAR (MAX)  NULL,
    [Concession_Held]      VARCHAR (3900) NULL,
    [Area]                 VARCHAR (3900) NULL,
    [Contract_Type]        VARCHAR (3900) NULL,
    [Year_of_Grant_Award]  VARCHAR (3900) NULL,
    [Date_of_Expiration]   DATETIME       NULL,
    [Geological_location]  VARCHAR (3900) NULL,
    [Comment]              VARCHAR (3900) NULL,
    [Status_]              VARCHAR (3900) NULL,
    [Flag1]                VARCHAR (3900) NULL,
    [Flag2]                VARCHAR (3900) NULL,
<<<<<<< HEAD
    [Created_by]           VARCHAR (100)  NULL,
    [Updated_by]           VARCHAR (100)  NULL,
    [Date_Created]         DATETIME       NULL,
    [Date_Updated]         DATETIME       NULL,
    [COMPANY_EMAIL]        VARCHAR (3900) NULL,
    [Terrain]              VARCHAR (3900) NULL,
    [Year]                 VARCHAR (50)   NULL,
    [submitted]            VARCHAR (3900) NULL,
    [Consession_Type]      VARCHAR (50)   NULL,
    [Concession_Unique_ID] VARCHAR (50)   NULL,
=======
    [Terrain]              VARCHAR (3900) NULL,
    [Year]                 VARCHAR (50)   NULL,
    [submitted]            VARCHAR (3900) NULL,
>>>>>>> origin/main
    [OPEN_DATE]            VARCHAR (100)  NULL,
    [CLOSE_DATE]           VARCHAR (100)  NULL,
    [DELETED_STATUS]       VARCHAR (100)  NULL,
    [DELETED_BY]           VARCHAR (200)  NULL,
    [DELETED_DATE]         VARCHAR (100)  NULL,
    [EMAIL_REMARK]         VARCHAR (100)  NULL,
<<<<<<< HEAD
    [CompanyNumber]        INT            NULL
);

=======
    [CompanyNumber]        INT            NULL,
    [ConcessionName]       VARCHAR (50)   NULL,
    [Field_Name]           VARCHAR (100)  NULL,
    [Created_by]           VARCHAR (100)  NULL,
    [Updated_by]           VARCHAR (100)  NULL,
    [Date_Created]         DATETIME       NULL,
    [Date_Updated]         DATETIME       NULL,
    CONSTRAINT [PK_ADMIN_CONCESSIONS_INFORMATIONs] PRIMARY KEY CLUSTERED ([Consession_Id] ASC)
);



>>>>>>> origin/main
