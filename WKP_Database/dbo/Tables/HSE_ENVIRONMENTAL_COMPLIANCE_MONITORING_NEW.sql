﻿CREATE TABLE [dbo].[HSE_ENVIRONMENTAL_COMPLIANCE_MONITORING_NEW] (
    [Id]                                                            INT            IDENTITY (1, 1) NOT NULL,
    [OML_ID]                                                        VARCHAR (200)  NULL,
    [OML_Name]                                                      VARCHAR (200)  NULL,
    [CompanyName]                                                   VARCHAR (500)  NULL,
    [Companyemail]                                                  VARCHAR (500)  NULL,
    [Year_of_WP]                                                    VARCHAR (100)  NULL,
    [ACTUAL_year]                                                   VARCHAR (500)  NULL,
    [PROPOSED_year]                                                 VARCHAR (500)  NULL,
    [Are_you_a_Producing_or_Non_Producing_Company]                  VARCHAR (3999) NULL,
    [If_YES_have_you_registered_your_Point_Sources]                 VARCHAR (3900) NULL,
    [If_NO_give_reasons_for_not_registering_your_Point_Sources]     VARCHAR (3999) NULL,
    [Have_you_submitted_your_Environmental_Compliance_Report]       VARCHAR (3999) NULL,
    [If_NO_Give_reasons_for_non_SUBMISSION]                         VARCHAR (3900) NULL,
    [Have_you_submitted_your_Chemical_Usage_Inventorization_Report] VARCHAR (3999) NULL,
    [If_NO_Give_reasons_for_non_submission_2]                       VARCHAR (3900) NULL,
    [Created_by]                                                    VARCHAR (100)  NULL,
    [Updated_by]                                                    VARCHAR (100)  NULL,
    [Date_Created]                                                  DATETIME       NULL,
    [Date_Updated]                                                  DATETIME       NULL,
    [Terrain]                                                       VARCHAR (50)   NULL,
    [Contract_Type]                                                 VARCHAR (50)   NULL,
    [Consession_Type]                                               VARCHAR (50)   NULL,
    [COMPANY_ID]                                                    VARCHAR (100)  NULL,
    [CompanyNumber]                                                 INT            NULL,
    [Field_ID]                                                      INT            NULL,
    CONSTRAINT [PK_ENVIRONMENTAL_COMPLIANCE_MONITORING_NEW] PRIMARY KEY CLUSTERED ([Id] ASC)
);



