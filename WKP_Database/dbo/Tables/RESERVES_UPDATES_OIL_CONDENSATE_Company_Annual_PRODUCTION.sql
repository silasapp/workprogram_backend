﻿CREATE TABLE [dbo].[RESERVES_UPDATES_OIL_CONDENSATE_Company_Annual_PRODUCTION] (
    [Id]                        INT           IDENTITY (1, 1) NOT NULL,
    [OML_ID]                    VARCHAR (200) NULL,
    [OML_Name]                  VARCHAR (200) NULL,
    [CompanyName]               VARCHAR (500) NULL,
    [Companyemail]              VARCHAR (500) NULL,
    [Year_of_WP]                VARCHAR (100) NULL,
    [Company_Annual_Year]       VARCHAR (500) NULL,
    [Company_Annual_Oil]        VARCHAR (500) NULL,
    [Company_Annual_Condensate] VARCHAR (500) NULL,
    [Company_Annual_AG]         VARCHAR (100) NULL,
    [Company_Annual_NAG]        VARCHAR (500) NULL,
    [Created_by]                VARCHAR (100) NULL,
    [Updated_by]                VARCHAR (100) NULL,
    [Date_Created]              DATETIME      NULL,
    [Date_Updated]              DATETIME      NULL,
    [Terrain]                   VARCHAR (50)  NULL,
    [Contract_Type]             VARCHAR (50)  NULL,
    [Consession_Type]           VARCHAR (50)  NULL,
    [COMPANY_ID]                VARCHAR (100) NULL,
    [CompanyNumber]             INT           NULL,
    CONSTRAINT [PK_RESERVES_UPDATES_OIL_CONDENSATE_Company_Annual_PRODUCTION] PRIMARY KEY CLUSTERED ([Id] ASC)
);

