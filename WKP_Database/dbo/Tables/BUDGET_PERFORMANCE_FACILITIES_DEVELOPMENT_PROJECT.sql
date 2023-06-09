﻿CREATE TABLE [dbo].[BUDGET_PERFORMANCE_FACILITIES_DEVELOPMENT_PROJECT] (
    [Id]                               INT           IDENTITY (1, 1) NOT NULL,
    [OML_ID]                           VARCHAR (200) NULL,
    [OML_Name]                         VARCHAR (200) NULL,
    [CompanyName]                      VARCHAR (500) NULL,
    [Companyemail]                     VARCHAR (500) NULL,
    [Year_of_WP]                       VARCHAR (100) NULL,
    [CONCEPT_planned]                  VARCHAR (500) NULL,
    [CONCEPT_Actual]                   VARCHAR (500) NULL,
    [FEED_planned]                     VARCHAR (500) NULL,
    [FEED_COST_Actual]                 VARCHAR (500) NULL,
    [DETAILED_ENGINEERING_planned]     VARCHAR (500) NULL,
    [DETAILED_ENGINEERING_Actual]      VARCHAR (500) NULL,
    [PROCUREMENT_planned]              VARCHAR (500) NULL,
    [PROCUREMENT_Actual]               VARCHAR (500) NULL,
    [CONSTRUCTION_FABRICATION_planned] VARCHAR (500) NULL,
    [CONSTRUCTION_FABRICATION_Actual]  VARCHAR (500) NULL,
    [INSTALLATION_planned]             VARCHAR (500) NULL,
    [INSTALLATION_Actual]              VARCHAR (500) NULL,
    [UPGRADE_MAINTENANCE_planned]      VARCHAR (500) NULL,
    [UPGRADE_MAINTENANCE_Actual]       VARCHAR (500) NULL,
    [DECOMMISSIONING_ABANDONMENT]      VARCHAR (500) NULL,
    [Created_by]                       VARCHAR (100) NULL,
    [Updated_by]                       VARCHAR (100) NULL,
    [Date_Created]                     DATETIME      NULL,
    [Date_Updated]                     DATETIME      NULL,
    [Contract_Type]                    VARCHAR (50)  NULL,
    [Terrain]                          VARCHAR (50)  NULL,
    [Consession_Type]                  VARCHAR (50)  NULL,
    [COMPANY_ID]                       VARCHAR (100) NULL,
    [CompanyNumber]                    INT           NULL,
    [Field_ID]                         INT           NULL,
    CONSTRAINT [PK_BUDGET_PERFORMANCE_FACILITIES_DEVELOPMENT_PROJECT] PRIMARY KEY CLUSTERED ([Id] ASC)
);

