﻿CREATE TABLE [dbo].[HSE_WASTE_MANAGEMENT_NEW] (
    [Id]                                                    INT            IDENTITY (1, 1) NOT NULL,
    [OML_ID]                                                VARCHAR (200)  NULL,
    [OML_Name]                                              VARCHAR (200)  NULL,
    [CompanyName]                                           VARCHAR (500)  NULL,
    [Companyemail]                                          VARCHAR (500)  NULL,
    [Year_of_WP]                                            VARCHAR (100)  NULL,
    [ACTUAL_year]                                           VARCHAR (500)  NULL,
    [PROPOSED_year]                                         VARCHAR (500)  NULL,
    [Do_you_have_Waste_Management_facilities]               VARCHAR (3999) NULL,
    [If_YES_is_the_facility_registered]                     VARCHAR (3900) NULL,
    [If_NO_give_reasons_for_not_being_registered]           VARCHAR (3900) NULL,
    [Created_by]                                            VARCHAR (100)  NULL,
    [Updated_by]                                            VARCHAR (100)  NULL,
    [Date_Created]                                          DATETIME       NULL,
    [Date_Updated]                                          DATETIME       NULL,
    [Consession_Type]                                       VARCHAR (50)   NULL,
    [Terrain]                                               VARCHAR (50)   NULL,
    [Contract_Type]                                         VARCHAR (50)   NULL,
    [COMPANY_ID]                                            VARCHAR (100)  NULL,
    [CompanyNumber]                                         INT            NULL,
    [Field_ID]                                              INT            NULL,
    [Commitment_To_Waste_Management]                        VARCHAR (300)  NULL,
    [How_Much_Is_Budgeted_For_Waste_MGT_Plan]               FLOAT (53)     NULL,
    [Evidence_Of_Submission_Of_Journey_MGT_Plan]            VARCHAR (300)  NULL,
    [Are_Registered_Point_Sources_Valid]                    VARCHAR (100)  NULL,
    [Evidence_Of_Submission_Of_PreviousYears_Waste_Release] VARCHAR (300)  NULL,
    CONSTRAINT [PK_HSE_WASTE_MANAGEMENT_NEW] PRIMARY KEY CLUSTERED ([Id] ASC)
);

