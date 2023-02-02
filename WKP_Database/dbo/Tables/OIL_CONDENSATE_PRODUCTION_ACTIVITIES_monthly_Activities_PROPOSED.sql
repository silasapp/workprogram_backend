﻿CREATE TABLE [dbo].[OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities_PROPOSED] (
    [Id]                   INT            IDENTITY (1, 1) NOT NULL,
    [OML_ID]               VARCHAR (200)  NULL,
    [OML_Name]             VARCHAR (200)  NULL,
    [CompanyName]          VARCHAR (500)  NULL,
    [Companyemail]         VARCHAR (500)  NULL,
    [Year_of_WP]           VARCHAR (100)  NULL,
    [Production_month_id]  INT            NULL,
    [Production_month]     VARCHAR (100)  NULL,
    [Production]           VARCHAR (50)   NULL,
    [Avg_Daily_Production] VARCHAR (50)   NULL,
    [Created_by]           VARCHAR (100)  NULL,
    [Updated_by]           VARCHAR (100)  NULL,
    [Date_Created]         DATETIME       NULL,
    [Date_Updated]         DATETIME       NULL,
    [Contract_Type]        VARCHAR (50)   NULL,
    [Terrain]              VARCHAR (50)   NULL,
    [Consession_Type]      VARCHAR (50)   NULL,
    [Gas_AG]               VARCHAR (20)   NULL,
    [Gas_NAG]              VARCHAR (20)   NULL,
    [COMPANY_ID]           VARCHAR (100)  NULL,
    [CompanyNumber]        INT            NULL,
    [Field_ID]             INT            NULL,
    [CondensateProd]       NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities_PROPOSED] PRIMARY KEY CLUSTERED ([Id] ASC)
);



