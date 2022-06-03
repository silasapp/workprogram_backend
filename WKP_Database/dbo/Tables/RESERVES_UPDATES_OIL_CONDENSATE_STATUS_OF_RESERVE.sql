﻿CREATE TABLE [dbo].[RESERVES_UPDATES_OIL_CONDENSATE_STATUS_OF_RESERVE] (
    [Id]                                          INT           IDENTITY (1, 1) NOT NULL,
    [OML_ID]                                      VARCHAR (200) NULL,
    [OML_Name]                                    VARCHAR (200) NULL,
    [CompanyName]                                 VARCHAR (500) NULL,
    [Companyemail]                                VARCHAR (500) NULL,
    [Year_of_WP]                                  VARCHAR (100) NULL,
    [Company_Reserves_Year]                       VARCHAR (500) NULL,
    [Company_Reserves_Oil]                        VARCHAR (500) NULL,
    [Company_Reserves_Condensate]                 VARCHAR (500) NULL,
    [Company_Reserves_AG]                         VARCHAR (100) NULL,
    [Company_Reserves_NAG]                        VARCHAR (500) NULL,
    [Company_Reserves_AnnualOilProduction]        VARCHAR (500) NULL,
    [FLAG1]                                       VARCHAR (500) NULL,
    [FLAG2]                                       VARCHAR (500) NULL,
    [Created_by]                                  VARCHAR (100) NULL,
    [Updated_by]                                  VARCHAR (100) NULL,
    [Date_Created]                                DATETIME      NULL,
    [Date_Updated]                                DATETIME      NULL,
    [Consession_Type]                             VARCHAR (50)  NULL,
    [Terrain]                                     VARCHAR (50)  NULL,
    [Contract_Type]                               VARCHAR (50)  NULL,
    [Company_Reserves_AnnualGasProduction]        VARCHAR (100) NULL,
    [Company_Reserves_AnnualCondensateProduction] VARCHAR (100) NULL,
    [Company_Reserves_AnnualGasAGProduction]      VARCHAR (100) NULL,
    [Company_Reserves_AnnualGasNAGProduction]     VARCHAR (100) NULL,
    [COMPANY_ID]                                  VARCHAR (100) NULL,
        [CompanyNumber]      INT        NULL          

    CONSTRAINT [PK_RESERVES_UPDATES_OIL_CONDENSATE_STATUS_OF_RESERVE] PRIMARY KEY CLUSTERED ([Id] ASC)
);

