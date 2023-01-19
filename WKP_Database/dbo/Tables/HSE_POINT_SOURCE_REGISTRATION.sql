CREATE TABLE [dbo].[HSE_POINT_SOURCE_REGISTRATION] (
    [Id]                            INT            IDENTITY (1, 1) NOT NULL,
    [OML_Name]                      VARCHAR (3000) NULL,
    [OML_ID]                        VARCHAR (MAX)  NULL,
    [CompanyName]                   VARCHAR (100)  NULL,
    [Company_Email]                 VARCHAR (100)  NULL,
    [Year_of_WP]                    VARCHAR (1)    NULL,
    [Company_Number]                VARCHAR (1)    NULL,
    [Company_ID]                    VARCHAR (1)    NULL,
    [are_there_point_source_permit] VARCHAR (1)    NULL,
    [evidence_of_PSP_filename]      VARCHAR (1)    NULL,
    [evidence_of_PSP_path]          VARCHAR (1)    NULL,
    [reason_for_no_PSP]             VARCHAR (1)    NULL,
    CONSTRAINT [PK_HSE_POINT_SOURCE_REGISTRATION] PRIMARY KEY CLUSTERED ([Id] ASC)
);



