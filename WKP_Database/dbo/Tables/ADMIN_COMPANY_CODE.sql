CREATE TABLE [dbo].[ADMIN_COMPANY_CODE] (
<<<<<<< HEAD
    [Id]           INT           IDENTITY (1, 1) NOT NULL,
    [CompanyCode]  VARCHAR (200) NULL,
    [CompanyName]  VARCHAR (200) NULL,
    [Email]        VARCHAR (200) NULL,
    [GUID]         VARCHAR (200) NULL,
    [IsActive]     VARCHAR (20)  NULL,
    [Created_by]   VARCHAR (100) NULL,
    [Updated_by]   VARCHAR (100) NULL,
    [Date_Created] DATETIME      NULL,
    [Date_Updated] DATETIME      NULL,
    [UserNumber]   INT           NULL
);

=======
    [Id]            INT           IDENTITY (1, 1) NOT NULL,
    [CompanyCode]   VARCHAR (200) NULL,
    [CompanyName]   VARCHAR (200) NULL,
    [Email]         VARCHAR (200) NULL,
    [GUID]          VARCHAR (200) NULL,
    [IsActive]      VARCHAR (20)  NULL,
    [Created_by]    VARCHAR (100) NULL,
    [Updated_by]    VARCHAR (100) NULL,
    [Date_Created]  DATETIME      NULL,
    [Date_Updated]  DATETIME      NULL,
    [UserNumber]    INT           NULL,
    [CompanyNumber] INT           NULL
);



>>>>>>> origin/main
