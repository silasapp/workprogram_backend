﻿CREATE TABLE [dbo].[HSE_ASSET_REGISTER_TEMPLATE_RBI_EQUIPMENT_INSPECTION_STRATEGY_NEW] (
    [Id]                                  INT            IDENTITY (1, 1) NOT NULL,
    [OML_ID]                              VARCHAR (200)  NULL,
    [OML_Name]                            VARCHAR (200)  NULL,
    [CompanyName]                         VARCHAR (500)  NULL,
    [Companyemail]                        VARCHAR (500)  NULL,
    [Year_of_WP]                          VARCHAR (100)  NULL,
    [facility]                            VARCHAR (500)  NULL,
    [Equipment_type]                      VARCHAR (500)  NULL,
    [Equipment_description]               VARCHAR (3999) NULL,
    [Equipment_serial_number]             VARCHAR (3900) NULL,
    [Equipment_tag_number]                VARCHAR (3999) NULL,
    [Equipment_manufacturer]              VARCHAR (500)  NULL,
    [Equipment_Installation_date]         DATETIME       NULL,
    [Last_inspection_date]                DATETIME       NULL,
    [Last_Inspection_Type_Performed]      VARCHAR (3999) NULL,
    [Likelihood_of_Failure]               VARCHAR (3999) NULL,
    [Consequence_of_Failure]              VARCHAR (3999) NULL,
    [Maximum_Inspection_Interval]         VARCHAR (3999) NULL,
    [Next_Inspection_Date]                DATETIME       NULL,
    [Proposed_Inspection_Type]            VARCHAR (3999) NULL,
    [RBI_Assessment_Date]                 DATETIME       NULL,
    [Equipment_Inspected_as_and_when_due] VARCHAR (3999) NULL,
    [State_reason]                        VARCHAR (3999) NULL,
    [Condition_of_Equipment]              VARCHAR (3999) NULL,
    [Function_Test_Result]                VARCHAR (3999) NULL,
    [Inspection_Report_Review]            VARCHAR (3999) NULL,
    [Created_by]                          VARCHAR (100)  NULL,
    [Updated_by]                          VARCHAR (100)  NULL,
    [Date_Created]                        DATETIME       NULL,
    [Date_Updated]                        DATETIME       NULL,
    [Contract_Type]                       VARCHAR (50)   NULL,
    [Terrain]                             VARCHAR (50)   NULL,
    [Consession_Type]                     VARCHAR (50)   NULL,
    [COMPANY_ID]                          VARCHAR (100)  NULL,
    [CompanyNumber]                       INT            NULL,
    [Field_ID]                            INT            NULL,
    CONSTRAINT [PK_HSE_ASSET_REGISTER_TEMPLATE_RBI_EQUIPMENT_INSPECTION_STRATEGY_NEW] PRIMARY KEY CLUSTERED ([Id] ASC)
);

