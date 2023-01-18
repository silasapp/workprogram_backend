CREATE TABLE [dbo].[HSE_REMEDIATION_FUND] (
    [Id]                        INT            IDENTITY (1, 1) NOT NULL,
    [OMN_Name]                  VARCHAR (3000) NULL,
    [OML_ID]                    VARCHAR (MAX)  NULL,
    [CompanyName]               VARCHAR (100)  NULL,
    [Company_Email]             VARCHAR (100)  NULL,
    [Year_of_WP]                VARCHAR (1)    NULL,
    [Company_Number]            VARCHAR (1)    NULL,
    [Company_ID]                VARCHAR (1)    NULL,
    [evidenceOfPaymentFilename] VARCHAR (1)    NULL,
    [evidenceOfPaymentPath]     VARCHAR (1)    NULL,
    [reasonForNoRemediation]    VARCHAR (1)    NULL,
    [OML_Name]                  VARCHAR (3000) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

