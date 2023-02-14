CREATE TABLE [dbo].[ApplicationProccesses] (
    [ProccessID]    INT            IDENTITY (1, 1) NOT NULL,
    [CategoryID]    INT            NOT NULL,
    [RoleID]        INT            NOT NULL,
    [SBU_ID]        INT            NOT NULL,
    [Sort]          INT            NOT NULL,
    [CreatedAt]     DATETIME       NOT NULL,
    [CreatedBy]     NVARCHAR (500) NULL,
    [UpdatedAt]     DATETIME       NULL,
    [UpdatedBy]     NVARCHAR (500) NULL,
    [DeleteStatus]  BIT            NOT NULL,
    [DeletedBy]     NVARCHAR (500) NULL,
    [DeletedAt]     DATETIME       NULL,
    [TriggeredBy]   NVARCHAR (500) NULL,
    [TargetTo]      NVARCHAR (500) NULL,
    [ProcessAction] NVARCHAR (100) NULL,
    [ProcessStatus] NVARCHAR (100) NULL,
    CONSTRAINT [PK_WorkProccess_] PRIMARY KEY CLUSTERED ([ProccessID] ASC)
);



