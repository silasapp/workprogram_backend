﻿CREATE TABLE [dbo].[ProcessActions] (
    [Id]         INT            IDENTITY (1, 1) NOT NULL,
    [ActionName] NVARCHAR (MAX) NULL,
    [CreatedBy]  NVARCHAR (MAX) NULL,
    [CreateOn]   DATETIME2 (7)  NOT NULL,
    [UpdatedBy]  NVARCHAR (MAX) NULL,
    [UpdatedOn]  DATETIME2 (7)  NOT NULL,
    CONSTRAINT [PK_ProcessActions] PRIMARY KEY CLUSTERED ([Id] ASC)
);

