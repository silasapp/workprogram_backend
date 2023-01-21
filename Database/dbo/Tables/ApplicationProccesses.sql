CREATE TABLE [dbo].[ApplicationProccesses] (
    [ProccessID]   INT      IDENTITY (1, 1) NOT NULL,
    [CategoryID]   INT      NOT NULL,
    [RoleID]       INT      NOT NULL,
    [SBU_ID]       INT      NOT NULL,
    [Sort]         INT      NOT NULL,
    [CreatedAt]    DATETIME NOT NULL,
    [CreatedBy]    INT      NULL,
    [UpdatedAt]    DATETIME NULL,
    [UpdatedBy]    INT      NULL,
    [DeleteStatus] BIT      NOT NULL,
    [DeletedBy]    INT      NULL,
    [DeletedAt]    DATETIME NULL,
    CONSTRAINT [PK_WorkProccess_] PRIMARY KEY CLUSTERED ([ProccessID] ASC)
);

