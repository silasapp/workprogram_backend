﻿CREATE TABLE [dbo].[OIL_AND_GAS_FACILITY_MAINTENANCE] (
    [Id]                                      INT           IDENTITY (1, 1) NOT NULL,
    [OML_ID]                                  VARCHAR (200) NULL,
    [OML_Name]                                VARCHAR (200) NULL,
    [CompanyName]                             VARCHAR (500) NULL,
    [Companyemail]                            VARCHAR (500) NULL,
    [Year_of_WP]                              VARCHAR (100) NULL,
    [Actual_capital_expenditure_Current_year] VARCHAR (500) NULL,
    [Proposed_Capital_Expenditure]            VARCHAR (500) NULL,
    [Remarks]                                 VARCHAR (500) NULL,
    [Major_Projects]                          VARCHAR (500) NULL,
    [Name]                                    VARCHAR (500) NULL,
    [Objective_Drivers_]                      VARCHAR (500) NULL,
    [Approval_License_Permits]                VARCHAR (500) NULL,
    [CAPEX_Oversight]                         VARCHAR (500) NULL,
    [Budget_Performance]                      VARCHAR (500) NULL,
    [Completion_Status]                       VARCHAR (100) NULL,
    [Conceptual]                              VARCHAR (500) NULL,
    [FEED]                                    VARCHAR (500) NULL,
    [Detailed_Engineering]                    VARCHAR (500) NULL,
    [Construction_Commissioning_]             VARCHAR (500) NULL,
    [Production_Product_Offtakers]            VARCHAR (500) NULL,
    [Challenges]                              VARCHAR (100) NULL,
    [Project_Timeline]                        VARCHAR (500) NULL,
    [Conformity_Assessment]                   VARCHAR (500) NULL,
    [New_Technology_]                         VARCHAR (500) NULL,
    [Has_it_been_adopted_by_DPR_]             VARCHAR (500) NULL,
    [Comment_]                                VARCHAR (MAX) NULL,
    [Planned_ongoing_and_routine_maintenance] VARCHAR (100) NULL,
    [Consession_Type]                         VARCHAR (50)  NULL,
    [Terrain]                                 VARCHAR (50)  NULL,
    [Contract_Type]                           VARCHAR (50)  NULL,
    [CompanyNumber]                           INT           NULL,
    [Field_ID]                                INT           NULL,
    CONSTRAINT [PK_OIL_AND_GAS_FACILITY_MAINTENANCE] PRIMARY KEY CLUSTERED ([Id] ASC)
);

