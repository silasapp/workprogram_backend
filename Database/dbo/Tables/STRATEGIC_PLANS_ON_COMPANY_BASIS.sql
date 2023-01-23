﻿CREATE TABLE [dbo].[STRATEGIC_PLANS_ON_COMPANY_BASIS] (
    [Id]              INT           IDENTITY (1, 1) NOT NULL,
    [OML_ID]          VARCHAR (200) NULL,
    [OML_Name]        VARCHAR (200) NULL,
    [CompanyName]     VARCHAR (500) NULL,
    [Companyemail]    VARCHAR (500) NULL,
    [Year_of_WP]      VARCHAR (100) NULL,
    [ACTIVITIES]      VARCHAR (500) NULL,
    [N_1_Q1]          VARCHAR (100) NULL,
    [N_1_Q2]          VARCHAR (100) NULL,
    [N_1_Q3]          VARCHAR (100) NULL,
    [N_1_Q4]          VARCHAR (100) NULL,
    [N_2_Q1]          VARCHAR (100) NULL,
    [N_2_Q2]          VARCHAR (100) NULL,
    [N_2_Q3]          VARCHAR (100) NULL,
    [N_2_Q4]          VARCHAR (100) NULL,
    [N_3_Q1]          VARCHAR (100) NULL,
    [N_3_Q2]          VARCHAR (100) NULL,
    [N_3_Q3]          VARCHAR (100) NULL,
    [N_3_Q4]          VARCHAR (100) NULL,
    [N_4_Q1]          VARCHAR (100) NULL,
    [N_4_Q2]          VARCHAR (100) NULL,
    [N_4_Q3]          VARCHAR (100) NULL,
    [N_4_Q4]          VARCHAR (100) NULL,
    [N_5_Q1]          VARCHAR (100) NULL,
    [N_5_Q2]          VARCHAR (100) NULL,
    [N_5_Q3]          VARCHAR (100) NULL,
    [N_5_Q4]          VARCHAR (100) NULL,
    [Created_by]      VARCHAR (100) NULL,
    [Updated_by]      VARCHAR (100) NULL,
    [Date_Created]    DATETIME      NULL,
    [Date_Updated]    DATETIME      NULL,
    [Contract_Type]   VARCHAR (50)  NULL,
    [Terrain]         VARCHAR (50)  NULL,
    [Consession_Type] VARCHAR (50)  NULL,
    [COMPANY_ID]      VARCHAR (100) NULL,
    [CompanyNumber]   INT           NULL,
    [Field_ID]        INT           NULL,
    CONSTRAINT [PK_STRATEGIC_PLANS_ON_COMPANY_BASIS] PRIMARY KEY CLUSTERED ([Id] ASC)
);

