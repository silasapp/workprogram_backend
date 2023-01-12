CREATE TABLE [dbo].[StrategicBusinessUnits] (
    [Id]       INT           IDENTITY (1, 1) NOT NULL,
    [SBU_Name] VARCHAR (100) NULL,
    [SBU_Code] NVARCHAR (10) NULL,
    CONSTRAINT [PK_StrategicBusinessUnit] PRIMARY KEY CLUSTERED ([Id] ASC)
);

