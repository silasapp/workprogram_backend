﻿CREATE TABLE [dbo].[HSE_COMMUNITY_DISTURBANCES_AND_OIL_SPILL_COST] (
    [Id]                            INT           IDENTITY (1, 1) NOT NULL,
    [OML_ID]                        VARCHAR (200) NULL,
    [OML_Name]                      VARCHAR (200) NULL,
    [CompanyName]                   VARCHAR (500) NULL,
    [Companyemail]                  VARCHAR (500) NULL,
    [Year_of_WP]                    VARCHAR (100) NULL,
    [ACTUAL_year]                   VARCHAR (500) NULL,
    [PROPOSED_year]                 VARCHAR (500) NULL,
    [Possible_causes_]              VARCHAR (MAX) NULL,
    [Effect_on_your_operations_]    VARCHAR (MAX) NULL,
    [Cost_involved_]                VARCHAR (MAX) NULL,
    [Total_days_lost_]              VARCHAR (MAX) NULL,
    [No_of_casual_Fatality_]        VARCHAR (MAX) NULL,
    [Action_plan_for_PROPOSED_YEAR] VARCHAR (MAX) NULL,
    [Created_by]                    VARCHAR (100) NULL,
    [Updated_by]                    VARCHAR (100) NULL,
    [Date_Created]                  DATETIME      NULL,
    [Date_Updated]                  DATETIME      NULL,
    [Consession_Type]               VARCHAR (50)  NULL,
    [Contract_Type]                 VARCHAR (50)  NULL,
    [Terrain]                       VARCHAR (50)  NULL,
    [CompanyNumber]                 INT           NULL,
    [Field_ID]                      INT           NULL,
    CONSTRAINT [PK_HSE_COMMUNITY_DISTURBANCES_AND_OIL_SPILL_COST] PRIMARY KEY CLUSTERED ([Id] ASC)
);



