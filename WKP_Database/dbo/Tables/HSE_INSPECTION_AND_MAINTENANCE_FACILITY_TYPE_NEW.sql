﻿CREATE TABLE [dbo].[HSE_INSPECTION_AND_MAINTENANCE_FACILITY_TYPE_NEW] (
    [Id]                                 INT            IDENTITY (1, 1) NOT NULL,
    [OML_ID]                             VARCHAR (200)  NULL,
    [OML_Name]                           VARCHAR (200)  NULL,
    [CompanyName]                        VARCHAR (500)  NULL,
    [Companyemail]                       VARCHAR (500)  NULL,
    [Year_of_WP]                         VARCHAR (100)  NULL,
    [ACTUAL_year]                        VARCHAR (500)  NULL,
    [PROPOSED_year]                      VARCHAR (500)  NULL,
    [Facility_Type]                      VARCHAR (3999) NULL,
    [Type_of_Inspection_and_Maintenance] VARCHAR (3900) NULL,
    [When_was_it_carried_out]            VARCHAR (3999) NULL,
    [Created_by]                         VARCHAR (100)  NULL,
    [Updated_by]                         VARCHAR (100)  NULL,
    [Date_Created]                       DATETIME       NULL,
    [Date_Updated]                       DATETIME       NULL,
    [Consession_Type]                    VARCHAR (50)   NULL,
    [Contract_Type]                      VARCHAR (50)   NULL,
    [Terrain]                            VARCHAR (50)   NULL,
    [Name_of_facility]                   VARCHAR (50)   NULL,
    [was_the_inspection_and_maintenemce] VARCHAR (50)   NULL,
    [If_RBI_was_approval_granted]        VARCHAR (50)   NULL,
    [If_No_Give_reasonS]                 VARCHAR (3999) NULL,
    [COMPANY_ID]                         VARCHAR (100)  NULL,
        [CompanyNumber]      INT        NULL          

    CONSTRAINT [PK_HSE_INSPECTION_AND_MAINTENANCE_FACILITY_TYPE_NEW] PRIMARY KEY CLUSTERED ([Id] ASC)
);

