﻿CREATE TABLE [dbo].[OIL_AND_GAS_FACILITY_MAINTENANCE_PROJECTS] (
    [Id]                                          INT           IDENTITY (1, 1) NOT NULL,
    [OML_ID]                                      VARCHAR (200) NULL,
    [OML_Name]                                    VARCHAR (200) NULL,
    [CompanyName]                                 VARCHAR (500) NULL,
    [Companyemail]                                VARCHAR (500) NULL,
    [Year_of_WP]                                  VARCHAR (100) NULL,
    [Major_Projects]                              VARCHAR (500) NULL,
    [Name]                                        VARCHAR (MAX) NULL,
    [Objective_Drivers_]                          VARCHAR (MAX) NULL,
    [Approval_License_Permits]                    VARCHAR (500) NULL,
    [CAPEX_Oversight]                             VARCHAR (500) NULL,
    [Budget_Performance]                          VARCHAR (500) NULL,
    [Completion_Status]                           VARCHAR (100) NULL,
    [Conceptual]                                  VARCHAR (500) NULL,
    [FEED]                                        VARCHAR (500) NULL,
    [Detailed_Engineering]                        VARCHAR (500) NULL,
    [Construction_Commissioning_]                 VARCHAR (500) NULL,
    [Production_Product_Offtakers]                VARCHAR (500) NULL,
    [Challenges]                                  VARCHAR (100) NULL,
    [Project_Timeline]                            VARCHAR (500) NULL,
    [Conformity_Assessment]                       VARCHAR (500) NULL,
    [New_Technology_]                             VARCHAR (500) NULL,
    [Has_it_been_adopted_by_DPR_]                 VARCHAR (500) NULL,
    [Comment_]                                    VARCHAR (MAX) NULL,
    [Planned_ongoing_and_routine_maintenance]     VARCHAR (100) NULL,
    [Actual_year]                                 VARCHAR (500) NULL,
    [Proposed_year]                               VARCHAR (500) NULL,
    [Created_by]                                  VARCHAR (100) NULL,
    [Updated_by]                                  VARCHAR (100) NULL,
    [Date_Created]                                DATETIME      NULL,
    [Date_Updated]                                DATETIME      NULL,
    [Actual_capital_expenditure_Current_year_NGN] VARCHAR (500) NULL,
    [Actual_capital_expenditure_Current_year_USD] VARCHAR (500) NULL,
    [Proposed_Capital_Expenditure_NGN]            VARCHAR (500) NULL,
    [Proposed_Capital_Expenditure_USD]            VARCHAR (500) NULL,
    [Project_Stage]                               VARCHAR (500) NULL,
    [Nigerian_Content_Value]                      VARCHAR (500) NULL,
    [Product_Off_takers]                          VARCHAR (500) NULL,
    [Actual_Proposed_year]                        VARCHAR (50)  NULL,
    [Consession_Type]                             VARCHAR (50)  NULL,
    [Terrain]                                     VARCHAR (50)  NULL,
    [Contract_Type]                               VARCHAR (50)  NULL,
    [Actual_Proposed]                             VARCHAR (50)  NULL,
    [COMPANY_ID]                                  VARCHAR (100) NULL,
    [CompanyNumber]                               INT           NULL,
<<<<<<< HEAD
    CONSTRAINT [PK_OIL_AND_GAS_FACILITY_MAINTENANCE_PROJECTS] PRIMARY KEY CLUSTERED ([Id] ASC)
);

=======
    [Field_ID]                                    INT           NULL,
    CONSTRAINT [PK_OIL_AND_GAS_FACILITY_MAINTENANCE_PROJECTS] PRIMARY KEY CLUSTERED ([Id] ASC)
);



>>>>>>> origin/main
