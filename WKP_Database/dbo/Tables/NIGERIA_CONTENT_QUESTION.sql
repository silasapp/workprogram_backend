﻿CREATE TABLE [dbo].[NIGERIA_CONTENT_QUESTION] (
    [Id]                                                          INT           IDENTITY (1, 1) NOT NULL,
    [OML_ID]                                                      VARCHAR (200) NULL,
    [OML_Name]                                                    VARCHAR (200) NULL,
    [CompanyName]                                                 VARCHAR (500) NULL,
    [Companyemail]                                                VARCHAR (500) NULL,
    [Year_of_WP]                                                  VARCHAR (100) NULL,
    [Do_you_have_a_valid_Expatriate_Quota_for_your_foreign_staff] VARCHAR (500) NULL,
    [If_NO_why]                                                   VARCHAR (500) NULL,
    [Is_there_a_succession_plan_in_place]                         VARCHAR (500) NULL,
    [Number_of_staff_released_within_the_year_]                   VARCHAR (500) NULL,
    [Created_by]                                                  VARCHAR (100) NULL,
    [Updated_by]                                                  VARCHAR (100) NULL,
    [Date_Created]                                                DATETIME      NULL,
    [Date_Updated]                                                DATETIME      NULL,
    [Contract_Type]                                               VARCHAR (50)  NULL,
    [Terrain]                                                     VARCHAR (50)  NULL,
    [Consession_Type]                                             VARCHAR (50)  NULL,
    [total_no_of_nigeria_senior_staff]                            VARCHAR (50)  NULL,
    [total_no_of_senior_staff]                                    VARCHAR (50)  NULL,
    [total_no_of_top_nigerian_management_staff]                   VARCHAR (50)  NULL,
    [total_no_of_top_management_staff]                            VARCHAR (50)  NULL,
    [COMPANY_ID]                                                  VARCHAR (100) NULL,
    [CompanyNumber]                                               INT           NULL,
<<<<<<< HEAD
    CONSTRAINT [PK_NIGERIA_CONTENT_QUESTION] PRIMARY KEY CLUSTERED ([Id] ASC)
);

=======
    [Field_ID]                                                    INT           NULL,
    CONSTRAINT [PK_NIGERIA_CONTENT_QUESTION] PRIMARY KEY CLUSTERED ([Id] ASC)
);



>>>>>>> origin/main