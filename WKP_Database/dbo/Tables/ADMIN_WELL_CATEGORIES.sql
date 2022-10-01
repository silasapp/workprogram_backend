CREATE TABLE [dbo].[ADMIN_WELL_CATEGORIES] (
    [Id]              INT           IDENTITY (1, 1) NOT NULL,
    [welltype]        VARCHAR (200) NULL,
    [categories_Desc] VARCHAR (MAX) NULL,
    [Created_by]      VARCHAR (100) NULL,
    [Updated_by]      VARCHAR (100) NULL,
    [Date_Created]    DATETIME      NULL,
    [Date_Updated]    DATETIME      NULL,
<<<<<<< HEAD
=======
    [CompanyNumber]   INT           NULL,
>>>>>>> origin/main
    CONSTRAINT [PK_ADMIN_WELL_CATEGORIES] PRIMARY KEY CLUSTERED ([Id] ASC)
);

