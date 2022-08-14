﻿CREATE TABLE [dbo].[NIGERIA_CONTENT_Upload_Succession_Plan] (
    [Id]                   INT           IDENTITY (1, 1) NOT NULL,
    [OML_ID]               VARCHAR (200) NULL,
    [OML_Name]             VARCHAR (200) NULL,
    [CompanyName]          VARCHAR (500) NULL,
    [Companyemail]         VARCHAR (500) NULL,
    [Year_of_WP]           VARCHAR (100) NULL,
    [Name_]                VARCHAR (500) NULL,
    [Understudy_]          VARCHAR (500) NULL,
    [Timeline_]            VARCHAR (500) NULL,
    [Position_Occupied_]   VARCHAR (500) NULL,
    [Created_by]           VARCHAR (100) NULL,
    [Updated_by]           VARCHAR (100) NULL,
    [Date_Created]         DATETIME      NULL,
    [Date_Updated]         DATETIME      NULL,
    [Actual_proposed]      VARCHAR (50)  NULL,
    [Actual_Proposed_Year] VARCHAR (50)  NULL,
    [Terrain]              VARCHAR (50)  NULL,
    [Contract_Type]        VARCHAR (50)  NULL,
    [Consession_Type]      VARCHAR (50)  NULL,
    [COMPANY_ID]           VARCHAR (100) NULL,
    [CompanyNumber]        INT           NULL,
    [Field_ID]             INT           NULL,
    CONSTRAINT [PK_NIGERIA_CONTENT_Upload_Succession_Plan] PRIMARY KEY CLUSTERED ([Id] ASC)
);



