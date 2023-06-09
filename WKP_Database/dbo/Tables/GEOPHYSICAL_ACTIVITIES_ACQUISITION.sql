﻿CREATE TABLE [dbo].[GEOPHYSICAL_ACTIVITIES_ACQUISITION] (
    [Geophysical_ActivitiesId]      INT            IDENTITY (1, 1) NOT NULL,
    [Geo_acquired_geophysical_data] VARCHAR (300)  NULL,
    [Geo_area_of_coverage]          VARCHAR (300)  NULL,
    [Geo_method_of_acquisition]     VARCHAR (300)  NULL,
    [Geo_type_of_data_acquired]     VARCHAR (300)  NULL,
    [Geo_Record_Length_of_Data]     VARCHAR (300)  NULL,
    [Geo_Completion_Status]         VARCHAR (300)  NULL,
    [Quantum]                       VARCHAR (300)  NULL,
    [Quantum_carry_forward]         VARCHAR (300)  NULL,
    [Geo_Activity_Timeline]         VARCHAR (300)  NULL,
    [Remarks]                       VARCHAR (MAX)  NULL,
    [Actual_year_aquired_data]      VARCHAR (300)  NULL,
    [proposed_year_data]            VARCHAR (300)  NULL,
    [Budeget_Allocation]            VARCHAR (300)  NULL,
    [Actual_year]                   VARCHAR (300)  NULL,
    [proposed_year]                 VARCHAR (300)  NULL,
    [Created_by]                    VARCHAR (100)  NULL,
    [Updated_by]                    VARCHAR (100)  NULL,
    [Date_Created]                  DATETIME       NULL,
    [Date_Updated]                  DATETIME       NULL,
    [OML_ID]                        VARCHAR (200)  NULL,
    [OML_Name]                      VARCHAR (200)  NULL,
    [CompanyName]                   VARCHAR (500)  NULL,
    [Companyemail]                  VARCHAR (500)  NULL,
    [Year_of_WP]                    VARCHAR (100)  NULL,
    [Budeget_Allocation_NGN]        VARCHAR (50)   NULL,
    [Budeget_Allocation_USD]        VARCHAR (50)   NULL,
    [Name_of_Contractor]            VARCHAR (3999) NULL,
    [Quantum_Approved]              VARCHAR (50)   NULL,
    [Contract_Type]                 VARCHAR (50)   NULL,
    [Terrain]                       VARCHAR (50)   NULL,
    [Consession_Type]               VARCHAR (50)   NULL,
    [Quantum_Planned]               VARCHAR (50)   NULL,
    [Gas_flare_Royalty_payment]     VARCHAR (50)   NULL,
    [Gas_Sales_Royalty_Payment]     VARCHAR (50)   NULL,
    [QUATER]                        VARCHAR (50)   NULL,
    [COMPANY_ID]                    VARCHAR (100)  NULL,
    [CompanyNumber]                 INT            NULL,
    [Field_ID]                      INT            NULL,
    [No_of_Folds]                   INT            NULL,
    CONSTRAINT [PK_Geophysical_Activities] PRIMARY KEY CLUSTERED ([Geophysical_ActivitiesId] ASC)
);

