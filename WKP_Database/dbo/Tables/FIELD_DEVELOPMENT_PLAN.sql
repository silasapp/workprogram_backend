﻿CREATE TABLE [dbo].[FIELD_DEVELOPMENT_PLAN] (
    [Id]                                         INT           IDENTITY (1, 1) NOT NULL,
    [OML_ID]                                     VARCHAR (200) NULL,
    [OML_Name]                                   VARCHAR (200) NULL,
    [CompanyName]                                VARCHAR (500) NULL,
    [Companyemail]                               VARCHAR (500) NULL,
    [Year_of_WP]                                 VARCHAR (100) NULL,
    [How_many_fields_have_FDP]                   VARCHAR (500) NULL,
    [List_all_the_field_with_FDP]                VARCHAR (MAX) NULL,
    [Which_fields_do_you_plan_to_submit_an_FDP]  VARCHAR (MAX) NULL,
    [How_many_fields_have_approved_FDP]          VARCHAR (100) NULL,
    [Number_of_wells_proposed_in_the_FDP]        VARCHAR (500) NULL,
    [No_of_wells_drilled_in_current_year]        VARCHAR (500) NULL,
    [Proposed_number_of_wells_from_approved_FDP] VARCHAR (500) NULL,
    [Processing_Fees_paid]                       VARCHAR (500) NULL,
    [Actual_year]                                VARCHAR (300) NULL,
    [proposed_year]                              VARCHAR (300) NULL,
    [Created_by]                                 VARCHAR (100) NULL,
    [Updated_by]                                 VARCHAR (100) NULL,
    [Date_Created]                               DATETIME      NULL,
    [Date_Updated]                               DATETIME      NULL,
    [Consession_Type]                            VARCHAR (50)  NULL,
    [Terrain]                                    VARCHAR (50)  NULL,
    [Contract_Type]                              VARCHAR (50)  NULL,
    [how_many_fields_in_concession]              VARCHAR (50)  NULL,
    [Noof_Producing_Fields]                      VARCHAR (20)  NULL,
    [Uploaded_approved_FDP_Document]             VARCHAR (200) NULL,
    [Are_they_oil_or_gas_wells]                  VARCHAR (20)  NULL,
    [FDPDocumentFilename]                        VARCHAR (500) NULL,
    [COMPANY_ID]                                 VARCHAR (100) NULL,
    [CompanyNumber]                              INT           NULL,
    [Field_ID]                                   INT           NULL,
    CONSTRAINT [PK_FIELD_DEVELOPMENT_PLAN] PRIMARY KEY CLUSTERED ([Id] ASC)
);



