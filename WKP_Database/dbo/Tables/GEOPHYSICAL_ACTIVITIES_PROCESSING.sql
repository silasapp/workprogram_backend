﻿CREATE TABLE [dbo].[GEOPHYSICAL_ACTIVITIES_PROCESSING] (
    [Geophysical_Activities_ProcessingId] INT            IDENTITY (1, 1) NOT NULL,
    [Geo_Any_Ongoing_Processing_Project]  VARCHAR (300)  NULL,
    [Geo_Type_of_Data_being_Processed]    VARCHAR (300)  NULL,
    [Geo_Quantum_of_Data]                 VARCHAR (300)  NULL,
    [Geo_Quantum_of_Data_carry_over]      VARCHAR (300)  NULL,
    [Geo_Completion_Status]               VARCHAR (300)  NULL,
    [Geo_Activity_Timeline]               VARCHAR (300)  NULL,
    [Remarks]                             VARCHAR (MAX)  NULL,
    [Actual_year_aquired_data]            VARCHAR (300)  NULL,
    [proposed_year_data]                  VARCHAR (300)  NULL,
    [Budeget_Allocation]                  VARCHAR (300)  NULL,
    [Actual_year]                         VARCHAR (300)  NULL,
    [proposed_year]                       VARCHAR (300)  NULL,
    [Created_by]                          VARCHAR (100)  NULL,
    [Updated_by]                          VARCHAR (100)  NULL,
    [Date_Created]                        DATETIME       NULL,
    [Date_Updated]                        DATETIME       NULL,
    [OML_ID]                              VARCHAR (200)  NULL,
    [OML_Name]                            VARCHAR (200)  NULL,
    [CompanyName]                         VARCHAR (500)  NULL,
    [Companyemail]                        VARCHAR (500)  NULL,
    [Year_of_WP]                          VARCHAR (100)  NULL,
    [Budeget_Allocation_USD]              VARCHAR (50)   NULL,
    [Budeget_Allocation_NGN]              VARCHAR (50)   NULL,
    [Processed_Actual]                    VARCHAR (50)   NULL,
    [Processed_Proposed]                  VARCHAR (50)   NULL,
    [Reprocessed_Actual]                  VARCHAR (50)   NULL,
    [Reprocessed_Proposed]                VARCHAR (50)   NULL,
    [Interpreted_Actual]                  VARCHAR (50)   NULL,
    [Interpreted_Proposed]                VARCHAR (50)   NULL,
    [Name_of_Contractor]                  VARCHAR (3999) NULL,
    [Quantum_Approved]                    VARCHAR (50)   NULL,
    [Contract_Type]                       VARCHAR (50)   NULL,
    [Terrain]                             VARCHAR (50)   NULL,
    [Quantum_Planned]                     VARCHAR (50)   NULL,
    [Consession_Type]                     VARCHAR (50)   NULL,
    [QUATER]                              VARCHAR (50)   NULL,
    [COMPANY_ID]                          VARCHAR (100)  NULL,
    [CompanyNumber]                       INT            NULL,
<<<<<<< HEAD
    CONSTRAINT [PK_Geophysical_Activities_Processing] PRIMARY KEY CLUSTERED ([Geophysical_Activities_ProcessingId] ASC)
);

=======
    [Field_ID]                            INT            NULL,
    CONSTRAINT [PK_Geophysical_Activities_Processing] PRIMARY KEY CLUSTERED ([Geophysical_Activities_ProcessingId] ASC)
);



>>>>>>> origin/main
