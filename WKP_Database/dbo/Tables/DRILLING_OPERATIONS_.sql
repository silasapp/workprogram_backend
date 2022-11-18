﻿CREATE TABLE [dbo].[DRILLING_OPERATIONS_] (
    [Id]                                        INT           IDENTITY (1, 1) NOT NULL,
    [OML_ID]                                    VARCHAR (200) NULL,
    [OML_Name]                                  VARCHAR (200) NULL,
    [CompanyName]                               VARCHAR (500) NULL,
    [Companyemail]                              VARCHAR (500) NULL,
    [Year_of_WP]                                VARCHAR (100) NULL,
    [Well_Name]                                 VARCHAR (500) NULL,
    [Current_year_Actual_Status_data]           VARCHAR (500) NULL,
    [Current_year_Actual_Net_Oil_Gas_sand_data] VARCHAR (500) NULL,
    [Proposed_year_data]                        VARCHAR (500) NULL,
    [Proposed_Budget_Allocation]                VARCHAR (500) NULL,
    [Remarks]                                   VARCHAR (500) NULL,
    [Do_you_have_approval_to_drill]             VARCHAR (500) NULL,
    [Give_reasons]                              VARCHAR (500) NULL,
    [Category]                                  VARCHAR (200) NULL,
    [Actual_No_Drilled_in_Current_Year]         VARCHAR (100) NULL,
    [Proposed_No_Drilled]                       VARCHAR (500) NULL,
    [Processing_Fees_Paid]                      VARCHAR (500) NULL,
    [Comments]                                  VARCHAR (MAX) NULL,
    [No_of_wells_cored]                         VARCHAR (500) NULL,
    [State_the_field_where_Discovery_was_made]  VARCHAR (MAX) NULL,
    [Any_New_Discoveries]                       VARCHAR (500) NULL,
    [Hydrocarbon_Counts]                        VARCHAR (500) NULL,
    [Actual_year]                               VARCHAR (300) NULL,
    [proposed_year]                             VARCHAR (300) NULL,
    [Created_by]                                VARCHAR (100) NULL,
    [Updated_by]                                VARCHAR (100) NULL,
    [Date_Created]                              DATETIME      NULL,
    [Date_Updated]                              DATETIME      NULL,
    [Budeget_Allocation_NGN]                    VARCHAR (50)  NULL,
    [Budeget_Allocation_USD]                    VARCHAR (50)  NULL,
    [Contract_Type]                             VARCHAR (50)  NULL,
    [Terrain]                                   VARCHAR (50)  NULL,
    [Consession_Type]                           VARCHAR (50)  NULL,
    [CompanyNumber]                             INT           NULL,
<<<<<<< HEAD
    CONSTRAINT [PK_Drilling_Operations_] PRIMARY KEY CLUSTERED ([Id] ASC)
);

=======
    [Field_ID]                                  INT           NULL,
    CONSTRAINT [PK_Drilling_Operations_] PRIMARY KEY CLUSTERED ([Id] ASC)
);



>>>>>>> origin/main