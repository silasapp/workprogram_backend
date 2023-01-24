CREATE TABLE [dbo].[HSE_REMEDIATION_FUND] (
    [Id]                        INT            IDENTITY (1, 1) NOT NULL,
    [OML_Name]                  VARCHAR (3000) NULL,
    [OML_ID]                    VARCHAR (MAX)  NULL,
    [CompanyName]               VARCHAR (MAX)  NULL,
    [Company_Email]             VARCHAR (MAX)  NULL,
    [Year_of_WP]                VARCHAR (MAX)  NULL,
    [Company_Number]            VARCHAR (MAX)  NULL,
    [Company_ID]                VARCHAR (MAX)  NULL,
    [evidenceOfPaymentFilename] VARCHAR (MAX)  NULL,
    [evidenceOfPaymentPath]     VARCHAR (MAX)  NULL,
    [reasonForNoRemediation]    VARCHAR (MAX)  NULL,
    [areThereRemediationFund]   NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_HSE_REMEDIATION_FUND] PRIMARY KEY CLUSTERED ([Id] ASC)
);

