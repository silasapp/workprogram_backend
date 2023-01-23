﻿CREATE TABLE [dbo].[OIL_CONDENSATE_PRODUCTION_ACTIVITIES_Operating_Facilities] (
    [Id]                   INT           IDENTITY (1, 1) NOT NULL,
    [OML_ID]               VARCHAR (200) NULL,
    [OML_Name]             VARCHAR (200) NULL,
    [CompanyName]          VARCHAR (500) NULL,
    [Companyemail]         VARCHAR (500) NULL,
    [Year_of_WP]           VARCHAR (100) NULL,
    [Operating_Facilities] VARCHAR (500) NULL,
    [Created_by]           VARCHAR (100) NULL,
    [Updated_by]           VARCHAR (100) NULL,
    [Date_Created]         DATETIME      NULL,
    [Date_Updated]         DATETIME      NULL,
    [Contract_Type]        VARCHAR (50)  NULL,
    [Terrain]              VARCHAR (50)  NULL,
    [Consession_Type]      VARCHAR (50)  NULL,
    [CompanyNumber]        INT           NULL,
    [Field_ID]             INT           NULL,
    CONSTRAINT [PK_OIL_CONDENSATE_PRODUCTION_ACTIVITIES_Operating_Facilities] PRIMARY KEY CLUSTERED ([Id] ASC)
);

