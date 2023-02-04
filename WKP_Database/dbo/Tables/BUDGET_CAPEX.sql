﻿CREATE TABLE [dbo].[BUDGET_CAPEX] (
    [ID]                        INT            IDENTITY (1, 1) NOT NULL,
    [OmL_Name]                  VARCHAR (1000) NULL,
    [OmL_ID]                    VARCHAR (400)  NULL,
    [CompanyName]               VARCHAR (2000) NULL,
    [Companyemail]              VARCHAR (200)  NULL,
    [Year_of_WP]                VARCHAR (300)  NULL,
    [Company_ID]                VARCHAR (200)  NULL,
    [CompanyNumber]             INT            NULL,
    [Acquisition]               VARCHAR (3000) NULL,
    [Processing]                VARCHAR (3000) NULL,
    [Reprocessing]              VARCHAR (2000) NULL,
    [Exploratory_Well_Drilling] VARCHAR (1000) NULL,
    [Appraisal_Well_Drilling]   VARCHAR (1000) NULL,
    [Development_Well_Drilling] VARCHAR (1000) NULL,
    [Workover_Operations]       VARCHAR (1000) NULL,
    [Completions]               VARCHAR (1000) NULL,
    [Flowlines]                 VARCHAR (1000) NULL,
    [Pipelines]                 VARCHAR (1000) NULL,
    [Generators]                VARCHAR (1000) NULL,
    [Turbines_Compressors]      VARCHAR (1000) NULL,
    [Buildings]                 VARCHAR (1000) NULL,
    [Other_Equipment]           VARCHAR (1000) NULL,
    [Civil_Works]               VARCHAR (1000) NULL,
    [Other_Costs]               VARCHAR (1000) NULL,
    [Created_by]                VARCHAR (3000) NULL,
    [Updated_by]                VARCHAR (300)  NULL,
    [Date_Created]              DATETIME       NULL,
    [Date_Updated]              DATETIME       NULL,
    CONSTRAINT [PK_BUDGET_CAPEX] PRIMARY KEY CLUSTERED ([ID] ASC)
);

