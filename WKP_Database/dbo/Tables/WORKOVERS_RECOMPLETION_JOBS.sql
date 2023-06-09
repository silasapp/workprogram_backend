﻿CREATE TABLE [dbo].[WORKOVERS_RECOMPLETION_JOBS] (
    [Id]                                                 INT            IDENTITY (1, 1) NOT NULL,
    [OML_ID]                                             VARCHAR (200)  NULL,
    [OML_Name]                                           VARCHAR (200)  NULL,
    [CompanyName]                                        VARCHAR (500)  NULL,
    [Companyemail]                                       VARCHAR (500)  NULL,
    [Year_of_WP]                                         VARCHAR (100)  NULL,
    [Current_year_Actual_Number_data]                    VARCHAR (500)  NULL,
    [Proposed_year_data]                                 VARCHAR (500)  NULL,
    [Current_year_Budget_Allocation]                     VARCHAR (500)  NULL,
    [Remarks]                                            VARCHAR (MAX)  NULL,
    [Processing_Fees_paid]                               VARCHAR (500)  NULL,
    [Do_you_have_approval_for_the_workover_recompletion] VARCHAR (500)  NULL,
    [Actual_year]                                        VARCHAR (300)  NULL,
    [proposed_year]                                      VARCHAR (300)  NULL,
    [Created_by]                                         VARCHAR (100)  NULL,
    [Updated_by]                                         VARCHAR (100)  NULL,
    [Date_Created]                                       DATETIME       NULL,
    [Date_Updated]                                       DATETIME       NULL,
    [Budeget_Allocation_NGN]                             VARCHAR (50)   NULL,
    [Budeget_Allocation_USD]                             VARCHAR (50)   NULL,
    [Consession_Type]                                    VARCHAR (50)   NULL,
    [Terrain]                                            VARCHAR (50)   NULL,
    [Contract_Type]                                      VARCHAR (50)   NULL,
    [QUATER]                                             VARCHAR (50)   NULL,
    [oil_or_gas_wells]                                   VARCHAR (20)   NULL,
    [COMPANY_ID]                                         VARCHAR (100)  NULL,
    [CompanyNumber]                                      INT            NULL,
    [Field_ID]                                           INT            NULL,
    [DaysForCompletion]                                  VARCHAR (20)   NULL,
    [CompletionWellName]                                 NVARCHAR (MAX) NULL,
    [Proposed_Workover_Date]                             DATETIME2 (7)  NULL,
    CONSTRAINT [PK_WORKOVERS_RECOMPLETION_JOBS] PRIMARY KEY CLUSTERED ([Id] ASC)
);

