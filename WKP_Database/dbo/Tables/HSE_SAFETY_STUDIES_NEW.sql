﻿CREATE TABLE [dbo].[HSE_SAFETY_STUDIES_NEW] (
    [Id]                                                                 INT            IDENTITY (1, 1) NOT NULL,
    [OML_ID]                                                             VARCHAR (200)  NULL,
    [OML_Name]                                                           VARCHAR (200)  NULL,
    [CompanyName]                                                        VARCHAR (500)  NULL,
    [Companyemail]                                                       VARCHAR (500)  NULL,
    [Year_of_WP]                                                         VARCHAR (100)  NULL,
    [ACTUAL_year]                                                        VARCHAR (500)  NULL,
    [PROPOSED_year]                                                      VARCHAR (500)  NULL,
    [Did_you_carry_out_safety_studies]                                   VARCHAR (3999) NULL,
    [State_Project_Name_for_which_studies_was_carried_out]               VARCHAR (3900) NULL,
    [List_the_studies]                                                   VARCHAR (3999) NULL,
    [List_identified_Major_Accident_Hazards_for_the_study]               VARCHAR (3999) NULL,
    [List_the_Safeguards_based_on_the_identified_Major_Accident_Hazards] VARCHAR (3999) NULL,
    [Created_by]                                                         VARCHAR (100)  NULL,
    [Updated_by]                                                         VARCHAR (100)  NULL,
    [Date_Created]                                                       DATETIME       NULL,
    [Date_Updated]                                                       DATETIME       NULL,
    [Contract_Type]                                                      VARCHAR (50)   NULL,
    [Terrain]                                                            VARCHAR (50)   NULL,
    [Consession_Type]                                                    VARCHAR (50)   NULL,
    [SMSFileUploadPath]                                                  VARCHAR (500)  NULL,
    [DoyouhaveSMSinPlace]                                                VARCHAR (500)  NULL,
    [COMPANY_ID]                                                         VARCHAR (100)  NULL,
    [CompanyNumber]                                                      INT            NULL,
<<<<<<< HEAD
    CONSTRAINT [PK_HSE_SAFETY_STUDIES_NEW] PRIMARY KEY CLUSTERED ([Id] ASC)
);

=======
    [Field_ID]                                                           INT            NULL,
    CONSTRAINT [PK_HSE_SAFETY_STUDIES_NEW] PRIMARY KEY CLUSTERED ([Id] ASC)
);



>>>>>>> origin/main
