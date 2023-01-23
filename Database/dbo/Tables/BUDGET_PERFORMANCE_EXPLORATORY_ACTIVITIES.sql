﻿CREATE TABLE [dbo].[BUDGET_PERFORMANCE_EXPLORATORY_ACTIVITIES] (
    [Id]                   INT           IDENTITY (1, 1) NOT NULL,
    [OML_ID]               VARCHAR (200) NULL,
    [OML_Name]             VARCHAR (200) NULL,
    [CompanyName]          VARCHAR (500) NULL,
    [Companyemail]         VARCHAR (500) NULL,
    [Year_of_WP]           VARCHAR (100) NULL,
    [ACQUISITION_planned]  VARCHAR (500) NULL,
    [ACQUISITION_Actual]   VARCHAR (500) NULL,
    [PROCESSING_planned]   VARCHAR (500) NULL,
    [PROCESSING_Actual]    VARCHAR (500) NULL,
    [REPROCESSING_planned] VARCHAR (500) NULL,
    [REPROCESSING_Actual]  VARCHAR (500) NULL,
    [EXPLORATION_planned]  VARCHAR (500) NULL,
    [EXPLORATION_Actual]   VARCHAR (500) NULL,
    [APPRAISAL_planned]    VARCHAR (500) NULL,
    [APPRAISAL_Actual]     VARCHAR (500) NULL,
    [Created_by]           VARCHAR (100) NULL,
    [Updated_by]           VARCHAR (100) NULL,
    [Date_Created]         DATETIME      NULL,
    [Date_Updated]         DATETIME      NULL,
    [Consession_Type]      VARCHAR (50)  NULL,
    [Terrain]              VARCHAR (50)  NULL,
    [Contract_Type]        VARCHAR (50)  NULL,
    [COMPANY_ID]           VARCHAR (100) NULL,
    [CompanyNumber]        INT           NULL,
    [Field_ID]             INT           NULL,
    CONSTRAINT [PK_BUDGET_PERFORMANCE_EXPLORATORY_ACTIVITIES] PRIMARY KEY CLUSTERED ([Id] ASC)
);
