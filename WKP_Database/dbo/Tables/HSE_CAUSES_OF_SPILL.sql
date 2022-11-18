﻿CREATE TABLE [dbo].[HSE_CAUSES_OF_SPILL] (
    [Id]                            INT            IDENTITY (1, 1) NOT NULL,
    [OML_ID]                        VARCHAR (200)  NULL,
    [OML_Name]                      VARCHAR (200)  NULL,
    [CompanyName]                   VARCHAR (500)  NULL,
    [Companyemail]                  VARCHAR (500)  NULL,
    [Year_of_WP]                    VARCHAR (100)  NULL,
    [no_of_spills_reported]         VARCHAR (300)  NULL,
    [Total_Quantity_Spilled]        VARCHAR (100)  NULL,
    [Total_Quantity_Recovered]      VARCHAR (500)  NULL,
    [Corrosion]                     VARCHAR (500)  NULL,
    [Equipment_Failure]             VARCHAR (3999) NULL,
    [Erossion_waves_sand]           VARCHAR (500)  NULL,
    [Human_Error]                   VARCHAR (300)  NULL,
    [Mystery]                       VARCHAR (100)  NULL,
    [Operational_Maintenance_Error] VARCHAR (500)  NULL,
    [Sabotage]                      VARCHAR (500)  NULL,
    [YTBD]                          VARCHAR (3999) NULL,
    [Updated_by]                    VARCHAR (100)  NULL,
    [Date_Created]                  DATETIME       NULL,
    [Date_Updated]                  DATETIME       NULL,
    [Contract_Type]                 VARCHAR (500)  NULL,
    [Terrain]                       VARCHAR (50)   NULL,
    [Consession_Type]               VARCHAR (50)   NULL,
    [Created_by]                    VARCHAR (50)   NULL,
    [COMPANY_ID]                    VARCHAR (100)  NULL,
    [CompanyNumber]                 INT            NULL,
<<<<<<< HEAD
    CONSTRAINT [PK_HSE_CAUSES_OF_SPILL] PRIMARY KEY CLUSTERED ([Id] ASC)
);

=======
    [Field_ID]                      INT            NULL,
    CONSTRAINT [PK_HSE_CAUSES_OF_SPILL] PRIMARY KEY CLUSTERED ([Id] ASC)
);



>>>>>>> origin/main