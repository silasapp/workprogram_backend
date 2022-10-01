﻿CREATE TABLE [dbo].[CONCESSION_SITUATION] (
    [Id]                                            INT           IDENTITY (1, 1) NOT NULL,
    [OML_ID]                                        VARCHAR (200) NULL,
    [OML_Name]                                      VARCHAR (200) NULL,
    [CompanyName]                                   VARCHAR (500) NULL,
    [Companyemail]                                  VARCHAR (500) NULL,
    [Year]                                          VARCHAR (100) NULL,
    [Concession_Held]                               VARCHAR (500) NULL,
    [Area]                                          VARCHAR (500) NULL,
    [No_of_discovered_field]                        VARCHAR (500) NULL,
    [No_of_field_producing]                         VARCHAR (500) NULL,
    [Name_of_Company]                               VARCHAR (500) NULL,
    [Equity_distribution]                           VARCHAR (500) NULL,
    [Contract_Type]                                 VARCHAR (500) NULL,
    [Geological_location]                           VARCHAR (200) NULL,
    [Has_Signature_Bonus_been_paid]                 VARCHAR (100) NULL,
    [If_No_why_sig]                                 VARCHAR (500) NULL,
    [Has_the_Concession_Rentals_been_paid]          VARCHAR (5)   NULL,
    [If_No_why_concession]                          VARCHAR (MAX) NULL,
    [Is_there_an_application_for_renewal]           VARCHAR (500) NULL,
    [If_No_why_renewal]                             VARCHAR (MAX) NULL,
    [Budget_actual_for_license_or_lease]            VARCHAR (500) NULL,
    [proposed_budget_for_each_license_lease]        VARCHAR (500) NULL,
    [Five_year_proposal]                            VARCHAR (500) NULL,
    [Did_you_meet_the_minimum_work_programme]       VARCHAR (500) NULL,
    [Comment]                                       VARCHAR (MAX) NULL,
    [Created_by]                                    VARCHAR (100) NULL,
    [Updated_by]                                    VARCHAR (100) NULL,
    [Date_Created]                                  DATETIME      NULL,
    [Date_Updated]                                  DATETIME      NULL,
    [Date_of_Grant_Expiration]                      DATETIME      NULL,
    [Terrain]                                       VARCHAR (50)  NULL,
    [Consession_Type]                               VARCHAR (50)  NULL,
    [Date_of_Expiration]                            DATETIME      NULL,
    [How_Much_Signature_Bonus_have_been_paid_USD]   VARCHAR (50)  NULL,
    [How_Much_Concession_Rental_have_been_paid_USD] VARCHAR (50)  NULL,
    [How_Much_Renewal_Bonus_have_been_paid_USD]     VARCHAR (50)  NULL,
    [Has_Assignment_of_Interest_Fee_been_paid]      VARCHAR (50)  NULL,
    [relinquishment_retention]                      VARCHAR (500) NULL,
    [area_in_square_meter_based_on_company_records] VARCHAR (500) NULL,
    [COMPANY_ID]                                    VARCHAR (100) NULL,
    [CompanyNumber]                                 INT           NULL,
<<<<<<< HEAD
    CONSTRAINT [PK_Concession_Situation] PRIMARY KEY CLUSTERED ([Id] ASC)
);

=======
    [Field_Name]                                    VARCHAR (500) NULL,
    [AdminConcession_ID]                            INT           NULL,
    [Field_ID]                                      INT           NULL,
    CONSTRAINT [PK_Concession_Situation] PRIMARY KEY CLUSTERED ([Id] ASC)
);



>>>>>>> origin/main
