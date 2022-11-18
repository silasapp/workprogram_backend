﻿CREATE TABLE [dbo].[HSE_COMMUNITY_DISTURBANCES_AND_OIL_SPILL_COST_NEW] (
    [Id]                                                                                 INT            IDENTITY (1, 1) NOT NULL,
    [OML_ID]                                                                             VARCHAR (200)  NULL,
    [OML_Name]                                                                           VARCHAR (200)  NULL,
    [CompanyName]                                                                        VARCHAR (500)  NULL,
    [Companyemail]                                                                       VARCHAR (500)  NULL,
    [Year_of_WP]                                                                         VARCHAR (100)  NULL,
    [ACTUAL_year]                                                                        VARCHAR (500)  NULL,
    [PROPOSED_year]                                                                      VARCHAR (500)  NULL,
    [Was_there_any_Community_Related_Disturbances_within_your_operational_area]          VARCHAR (3999) NULL,
    [If_YES_Give_details_on_Community_Related_Disturbances_within_your_operational_area] VARCHAR (MAX)  NULL,
    [Was_any_Oil_Spill_recorded_within_your_operational_area]                            VARCHAR (3999) NULL,
    [Possible_causes]                                                                    VARCHAR (3900) NULL,
    [Effect_on_your_operations]                                                          VARCHAR (3999) NULL,
    [Cost_involved]                                                                      VARCHAR (3900) NULL,
    [Total_days_lost]                                                                    VARCHAR (3999) NULL,
    [No_of_casual_Fatality]                                                              VARCHAR (3900) NULL,
    [Action_Plan_for_]                                                                   VARCHAR (3999) NULL,
    [Created_by]                                                                         VARCHAR (100)  NULL,
    [Updated_by]                                                                         VARCHAR (100)  NULL,
    [Date_Created]                                                                       DATETIME       NULL,
    [Date_Updated]                                                                       DATETIME       NULL,
    [Terrain]                                                                            VARCHAR (50)   NULL,
    [Contract_Type]                                                                      VARCHAR (50)   NULL,
    [Consession_Type]                                                                    VARCHAR (50)   NULL,
    [Oil_spill_reported]                                                                 VARCHAR (50)   NULL,
    [COMPANY_ID]                                                                         VARCHAR (100)  NULL,
    [CompanyNumber]                                                                      INT            NULL,
<<<<<<< HEAD
    CONSTRAINT [PK_COMMUNITY_DISTURBANCES_AND_OIL_SPILL_COST_NEW] PRIMARY KEY CLUSTERED ([Id] ASC)
);

=======
    [Field_ID]                                                                           INT            NULL,
    CONSTRAINT [PK_COMMUNITY_DISTURBANCES_AND_OIL_SPILL_COST_NEW] PRIMARY KEY CLUSTERED ([Id] ASC)
);



>>>>>>> origin/main