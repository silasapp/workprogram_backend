﻿CREATE TABLE [dbo].[HSE_OCCUPATIONAL_HEALTH_MANAGEMENT] (
    [Id]                                    INT            IDENTITY (1, 1) NOT NULL,
    [OML_ID]                                VARCHAR (200)  NULL,
    [OML_Name]                              VARCHAR (200)  NULL,
    [CompanyName]                           VARCHAR (500)  NULL,
    [Companyemail]                          VARCHAR (500)  NULL,
    [Year_of_WP]                            VARCHAR (100)  NULL,
    [OHMplanFilePath]                       VARCHAR (3000) NULL,
    [OHMplanCommunicationFilePath]          VARCHAR (500)  NULL,
    [Created_by]                            VARCHAR (100)  NULL,
    [Updated_by]                            VARCHAR (100)  NULL,
    [Date_Created]                          DATETIME       NULL,
    [Date_Updated]                          DATETIME       NULL,
    [Consession_Type]                       VARCHAR (50)   NULL,
    [Terrain]                               VARCHAR (50)   NULL,
    [Contract_Type]                         VARCHAR (50)   NULL,
    [OHMplanFilename]                       VARCHAR (100)  NULL,
    [OHMplanCommunicationFilename]          VARCHAR (100)  NULL,
    [SMSFileUploadname]                     VARCHAR (100)  NULL,
    [COMPANY_ID]                            VARCHAR (100)  NULL,
    [CompanyNumber]                         INT            NULL,
    [Field_ID]                              INT            NULL,
    [ReasonWhyOhmWasNotCommunicatedToStaff] VARCHAR (1000) NULL,
    [WasOhmPolicyCommunicatedToStaff]       VARCHAR (10)   NULL,
    [ReasonForNoOhm]                        VARCHAR (1000) NULL,
    [DoYouHaveAnOhm]                        VARCHAR (10)   NULL,
    CONSTRAINT [PK_HSE_OCCUPATIONAL_HEALTH_MANAGEMENT] PRIMARY KEY CLUSTERED ([Id] ASC)
);







