﻿CREATE TABLE [dbo].[HSE_WASTE_MANAGEMENT_DZs] (
    [Id]                                               INT            IDENTITY (1, 1) NOT NULL,
    [OML_ID]                                           NVARCHAR (MAX) NULL,
    [OML_Name]                                         NVARCHAR (MAX) NULL,
    [CompanyName]                                      NVARCHAR (MAX) NULL,
    [Companyemail]                                     NVARCHAR (MAX) NULL,
    [Year_of_WP]                                       NVARCHAR (MAX) NULL,
    [COMPANY_ID]                                       NVARCHAR (MAX) NULL,
    [CompanyNumber]                                    INT            NULL,
    [Evidence_of_pay_of_DDCFilename]                   NVARCHAR (MAX) NULL,
    [Evidence_of_pay_of_DDCPath]                       NVARCHAR (MAX) NULL,
    [Waste_Contractor_Names]                           NVARCHAR (MAX) NULL,
    [Produce_Water_Manegent_Plan]                      NVARCHAR (MAX) NULL,
    [Evidence_of_Reinjection_Permit_Filename]          NVARCHAR (MAX) NULL,
    [Evidence_of_Reinjection_Permit_Path]              NVARCHAR (MAX) NULL,
    [Reason_for_No_Evidence_of_Reinjection]            NVARCHAR (MAX) NULL,
    [Do_You_Have_Previous_Year_Waste_Inventory_Report] NVARCHAR (MAX) NULL,
    [Evidence_of_EWD_Filename]                         NVARCHAR (MAX) NULL,
    [Evidence_of_EWD_Path]                             NVARCHAR (MAX) NULL,
    [Reason_for_No_Evidence_of_EWD]                    NVARCHAR (MAX) NULL,
    [ACTUAL_year]                                      NVARCHAR (MAX) NULL,
    [PROPOSED_year]                                    NVARCHAR (MAX) NULL,
    [Created_by]                                       NVARCHAR (MAX) NULL,
    [Updated_by]                                       NVARCHAR (MAX) NULL,
    [Date_Created]                                     DATETIME2 (7)  NULL,
    [Date_Updated]                                     DATETIME2 (7)  NULL,
    [Waste_Service_Permit_Filename]                    NVARCHAR (MAX) NULL,
    [Waste_Service_Permit_Path]                        FLOAT (53)     NULL,
    CONSTRAINT [PK_HSE_WASTE_MANAGEMENT_DZs] PRIMARY KEY CLUSTERED ([Id] ASC)
);

