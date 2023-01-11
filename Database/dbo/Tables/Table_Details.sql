CREATE TABLE [dbo].[Table_Details] (
    [TableId]     INT           IDENTITY (1, 1) NOT NULL,
    [TableName]   VARCHAR (250) NULL,
    [TableSchema] VARCHAR (250) NULL,
    [SBU_ID]      VARCHAR (30)  NULL,
    CONSTRAINT [PK_Table_Details] PRIMARY KEY CLUSTERED ([TableId] ASC)
);

