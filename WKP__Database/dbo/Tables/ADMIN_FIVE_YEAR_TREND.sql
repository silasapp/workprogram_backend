﻿CREATE TABLE [dbo].[ADMIN_FIVE_YEAR_TREND] (
    [Id]   INT         IDENTITY (1, 1) NOT NULL,
    [Year] VARCHAR (6) NOT NULL,
    CONSTRAINT [PK_ADMIN_FIVE_YEAR_TREND] PRIMARY KEY CLUSTERED ([Id] ASC)
);

