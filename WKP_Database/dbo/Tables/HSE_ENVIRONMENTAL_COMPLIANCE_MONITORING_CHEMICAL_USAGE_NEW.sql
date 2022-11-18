﻿CREATE TABLE [dbo].[HSE_ENVIRONMENTAL_COMPLIANCE_MONITORING_CHEMICAL_USAGE_NEW] (
    [Id]               INT            IDENTITY (1, 1) NOT NULL,
    [OML_ID]           VARCHAR (200)  NULL,
    [OML_Name]         VARCHAR (200)  NULL,
    [CompanyName]      VARCHAR (500)  NULL,
    [Companyemail]     VARCHAR (500)  NULL,
    [Year_of_WP]       VARCHAR (100)  NULL,
    [ACTUAL_year]      VARCHAR (500)  NULL,
    [PROPOSED_year]    VARCHAR (500)  NULL,
    [Name_of_Chemical] VARCHAR (3999) NULL,
    [DPR_Approved]     VARCHAR (3999) NULL,
    [Quarter_1]        VARCHAR (3900) NULL,
    [Quarter_2]        VARCHAR (3999) NULL,
    [Quarter_3]        VARCHAR (3999) NULL,
    [Quarter_4]        VARCHAR (3900) NULL,
    [Created_by]       VARCHAR (100)  NULL,
    [Updated_by]       VARCHAR (100)  NULL,
    [Date_Created]     DATETIME       NULL,
    [Date_Updated]     DATETIME       NULL,
    [Contract_Type]    VARCHAR (50)   NULL,
    [Consession_Type]  VARCHAR (50)   NULL,
    [Terrain]          VARCHAR (50)   NULL,
    [COMPANY_ID]       VARCHAR (100)  NULL,
    [CompanyNumber]    INT            NULL,
<<<<<<< HEAD
    CONSTRAINT [PK_HSE_ENVIRONMENTAL_COMPLIANCE_MONITORING_CHEMICAL_USAGE_NEW] PRIMARY KEY CLUSTERED ([Id] ASC)
);

=======
    [Field_ID]         INT            NULL,
    CONSTRAINT [PK_HSE_ENVIRONMENTAL_COMPLIANCE_MONITORING_CHEMICAL_USAGE_NEW] PRIMARY KEY CLUSTERED ([Id] ASC)
);



>>>>>>> origin/main