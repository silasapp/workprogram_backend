﻿CREATE TABLE [dbo].[OIL_CONDENSATE_PRODUCTION_ACTIVITIES_UNITIZATION] (
    [Id]                                             INT           IDENTITY (1, 1) NOT NULL,
    [OML_ID]                                         VARCHAR (200) NULL,
    [OML_Name]                                       VARCHAR (200) NULL,
    [CompanyName]                                    VARCHAR (500) NULL,
    [Companyemail]                                   VARCHAR (500) NULL,
    [Year_of_WP]                                     VARCHAR (100) NULL,
    [Current_year_Actual]                            VARCHAR (500) NULL,
    [Deferment]                                      VARCHAR (500) NULL,
    [Forecast]                                       VARCHAR (500) NULL,
    [Remarks]                                        VARCHAR (MAX) NULL,
    [Cost_Barrel]                                    VARCHAR (500) NULL,
    [Company_Timeline]                               VARCHAR (500) NULL,
    [Company_Oil]                                    VARCHAR (500) NULL,
    [Company_Condensate]                             VARCHAR (500) NULL,
    [Company_AG]                                     VARCHAR (100) NULL,
    [Company_NAG]                                    VARCHAR (500) NULL,
    [Fiveyear_Timeline]                              VARCHAR (500) NULL,
    [Fiveyear_Oil]                                   VARCHAR (500) NULL,
    [Fiveyear_Condensate]                            VARCHAR (500) NULL,
    [Fiveyear_AG]                                    VARCHAR (100) NULL,
    [Fiveyear_NAG]                                   VARCHAR (100) NULL,
    [Did_you_carry_out_any_well_test]                VARCHAR (500) NULL,
    [Type_of_Test]                                   VARCHAR (500) NULL,
    [Maximum_Efficiency_Rate]                        VARCHAR (500) NULL,
    [Number_of_Test_Carried_out]                     VARCHAR (100) NULL,
    [Number_of_Producing_Wells]                      VARCHAR (100) NULL,
    [Daily_Production_]                              VARCHAR (100) NULL,
    [Is_any_of_your_field_straddling]                VARCHAR (500) NULL,
    [How_many_fields_straddle]                       VARCHAR (500) NULL,
    [Straddling_Fields_OC]                           VARCHAR (500) NULL,
    [Prod_Status_OC]                                 VARCHAR (100) NULL,
    [Straddling_Field_OP]                            VARCHAR (100) NULL,
    [Company_Name_OP]                                VARCHAR (100) NULL,
    [Prod_Status_OP]                                 VARCHAR (100) NULL,
    [Has_DPR_been_notified]                          VARCHAR (500) NULL,
    [Has_the_other_party_been_notified]              VARCHAR (500) NULL,
    [Has_the_CA_been_signed]                         VARCHAR (500) NULL,
    [Committees_been_inaugurated]                    VARCHAR (100) NULL,
    [Participation_been_determined]                  VARCHAR (100) NULL,
    [Has_the_PUA_been_signed]                        VARCHAR (100) NULL,
    [Is_there_a_Joint_Development]                   VARCHAR (100) NULL,
    [Has_the_UUOA_been_signed]                       VARCHAR (100) NULL,
    [Actual_year]                                    VARCHAR (300) NULL,
    [proposed_year]                                  VARCHAR (300) NULL,
    [Created_by]                                     VARCHAR (100) NULL,
    [Updated_by]                                     VARCHAR (100) NULL,
    [Date_Created]                                   DATETIME      NULL,
    [Date_Updated]                                   DATETIME      NULL,
    [Total_Reconciled_National_Crude_Oil_Production] VARCHAR (100) NULL,
    [Contract_Type]                                  VARCHAR (50)  NULL,
    [Terrain]                                        VARCHAR (50)  NULL,
    [Consession_Type]                                VARCHAR (50)  NULL,
    [Oil_Royalty_Payment]                            VARCHAR (50)  NULL,
    [straddle_field_producing]                       VARCHAR (50)  NULL,
    [what_concession_field_straddling]               VARCHAR (50)  NULL,
    [PUAUploadFilePath]                              VARCHAR (100) NULL,
    [UUOAUploadFilePath]                             VARCHAR (100) NULL,
    [PUAUploadFilename]                              VARCHAR (100) NULL,
    [UUOAUploadFilename]                             VARCHAR (100) NULL,
    [COMPANY_ID]                                     VARCHAR (100) NULL,
    [CompanyNumber]                                  INT           NULL,
<<<<<<< HEAD
    CONSTRAINT [PK_OIL_CONDENSATE_PRODUCTION_ACTIVITIES_UNITIZATION] PRIMARY KEY CLUSTERED ([Id] ASC)
);

=======
    [Field_ID]                                       INT           NULL,
    CONSTRAINT [PK_OIL_CONDENSATE_PRODUCTION_ACTIVITIES_UNITIZATION] PRIMARY KEY CLUSTERED ([Id] ASC)
);



>>>>>>> origin/main
