﻿CREATE TABLE [dbo].[INITIAL_WELL_COMPLETION_JOBS] (
    [Id]                                        INT           IDENTITY (1, 1) NOT NULL,
    [OML_ID]                                    VARCHAR (200) NULL,
    [OML_Name]                                  VARCHAR (200) NULL,
    [CompanyName]                               VARCHAR (500) NULL,
    [Companyemail]                              VARCHAR (500) NULL,
    [Year_of_WP]                                VARCHAR (100) NULL,
    [Do_you_have_approval_to_complete_the_well] VARCHAR (500) NULL,
    [Current_year_Actual_Number]                VARCHAR (500) NULL,
    [Current_year_Budget_Allocation]            VARCHAR (500) NULL,
    [Proposed_year_data]                        VARCHAR (500) NULL,
    [Processing_Fees_paid]                      VARCHAR (500) NULL,
    [Remarks]                                   VARCHAR (MAX) NULL,
    [Actual_year]                               VARCHAR (300) NULL,
    [proposed_year]                             VARCHAR (300) NULL,
    [Created_by]                                VARCHAR (100) NULL,
    [Updated_by]                                VARCHAR (100) NULL,
    [Date_Created]                              DATETIME      NULL,
    [Date_Updated]                              DATETIME      NULL,
    [Budeget_Allocation_NGN]                    VARCHAR (50)  NULL,
    [Budeget_Allocation_USD]                    VARCHAR (50)  NULL,
    [Consession_Type]                           VARCHAR (50)  NULL,
    [Contract_Type]                             VARCHAR (50)  NULL,
    [Terrain]                                   VARCHAR (50)  NULL,
    [QUATER]                                    VARCHAR (50)  NULL,
    [oil_or_gas_wells]                          VARCHAR (20)  NULL,
    [Actual_Completion_Date]                    DATE          NULL,
    [Proposed_Completion_Date]                  DATE          NULL,
    [COMPANY_ID]                                VARCHAR (100) NULL,
    [CompanyNumber]                             INT           NULL,
<<<<<<< HEAD
    CONSTRAINT [PK_INITIAL_WELL_COMPLETION_JOBS] PRIMARY KEY CLUSTERED ([Id] ASC)
);

=======
    [Field_ID]                                  INT           NULL,
    CONSTRAINT [PK_INITIAL_WELL_COMPLETION_JOBS] PRIMARY KEY CLUSTERED ([Id] ASC)
);



>>>>>>> origin/main
