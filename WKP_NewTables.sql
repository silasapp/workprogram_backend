USE [workprogram]
GO
/****** Object:  Table [dbo].[ApplicationCategories]    Script Date: 10/21/2022 1:28:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ApplicationCategories](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[FriendlyName] [nvarchar](50) NULL,
	[Price] [int] NULL,
	[CreatedAt] [datetime] NULL,
	[CreatedBy] [int] NULL,
	[UpdatedAt] [datetime] NULL,
	[UpdatedBy] [int] NULL,
	[DeleteStatus] [bit] NULL,
	[DeletedBy] [int] NULL,
	[DeletedAt] [datetime] NULL,
 CONSTRAINT [PK_category_id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ApplicationDeskHistories]    Script Date: 10/21/2022 1:28:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ApplicationDeskHistories](
	[Id] [int] IDENTITY(10849,1) NOT NULL,
	[AppId] [int] NOT NULL,
	[StaffID] [int] NULL,
	[Comment] [varchar](max) NOT NULL,
	[Status] [nvarchar](50) NOT NULL,
	[CreatedAt] [datetime2](0) NOT NULL,
 CONSTRAINT [PK_application_desk_history_id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ApplicationDocument]    Script Date: 10/21/2022 1:28:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ApplicationDocument](
	[CategoryId] [int] NULL,
	[AppDocID] [int] IDENTITY(1,1) NOT NULL,
	[ElpsDocTypeID] [int] NOT NULL,
	[DocName] [varchar](200) NOT NULL,
	[DocType] [varchar](8) NOT NULL,
	[CreatedAt] [datetime] NOT NULL,
	[UpdatedAt] [datetime] NULL,
	[DeleteStatus] [bit] NOT NULL,
	[DeletedBy] [int] NULL,
	[DeletedAt] [datetime] NULL,
 CONSTRAINT [PK_ApplicationDocuments_UT] PRIMARY KEY CLUSTERED 
(
	[AppDocID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ApplicationProccesses]    Script Date: 10/21/2022 1:28:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ApplicationProccesses](
	[ProccessID] [int] IDENTITY(1,1) NOT NULL,
	[CategoryID] [int] NOT NULL,
	[RoleID] [int] NOT NULL,
	[SBU_ID] [int] NOT NULL,
	[Sort] [int] NOT NULL,
	[CreatedAt] [datetime] NOT NULL,
	[CreatedBy] [int] NULL,
	[UpdatedAt] [datetime] NULL,
	[UpdatedBy] [int] NULL,
	[DeleteStatus] [bit] NOT NULL,
	[DeletedBy] [int] NULL,
	[DeletedAt] [datetime] NULL,
 CONSTRAINT [PK_WorkProccess_] PRIMARY KEY CLUSTERED 
(
	[ProccessID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Applications]    Script Date: 10/21/2022 1:28:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Applications](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ReferenceNo] [varchar](50) NOT NULL,
	[CompanyID] [int] NOT NULL,
	[ConcessionID] [int] NULL,
	[FieldID] [int] NULL,
	[CategoryID] [int] NOT NULL,
	[YearOfWKP] [int] NOT NULL,
	[Status] [nvarchar](25) NOT NULL,
	[PaymentStatus] [nvarchar](25) NOT NULL,
	[CurrentDesk] [int] NULL,
	[Submitted] [bit] NULL,
	[ApprovalRef] [nvarchar](50) NULL,
	[CreatedAt] [datetime] NULL,
	[SubmittedAt] [datetime] NULL,
	[UpdatedAt] [datetime] NULL,
	[DeletedBy] [int] NULL,
	[DeleteStatus] [bit] NULL,
	[DeletedAt] [datetime] NULL,
 CONSTRAINT [PK_applications_id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AuditTrails]    Script Date: 10/21/2022 1:28:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AuditTrails](
	[AuditLogID] [int] IDENTITY(1,1) NOT NULL,
	[AuditAction] [varchar](max) NOT NULL,
	[CreatedAt] [datetime] NOT NULL,
	[UserID] [varchar](80) NULL,
 CONSTRAINT [PK_AuditTrail] PRIMARY KEY CLUSTERED 
(
	[AuditLogID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DecommissioningAbandonment]    Script Date: 10/21/2022 1:28:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DecommissioningAbandonment](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Decommissioning] [varchar](500) NULL,
	[Abandonment] [varchar](500) NULL,
	[Cumulative_DA_Annual_Payment] [varchar](500) NULL,
	[Accrued_DA_Annual_Payment] [varchar](500) NULL,
	[CAPEX] [varchar](500) NULL,
	[OPEX] [varchar](500) NULL,
	[CompanyNumber] [int] NULL,
	[ConcessionID] [int] NULL,
	[FieldID] [int] NULL,
	[DateCreated] [date] NULL,
	[Year] [int] NULL,
 CONSTRAINT [PK_DecommissioningAbandonment] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Development_And_Productions]    Script Date: 10/21/2022 1:28:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Development_And_Productions](
	[Id] [int] NOT NULL,
	[FieldHistory] [varchar](300) NULL,
	[Do_You_Have_Any_SubsurfacePlan] [varchar](10) NULL,
	[Type_Of_SubsurfacePlan] [varchar](100) NULL,
	[FiveYears_Projection_Of_Anticipated_Dev_Schemes] [varchar](500) NULL,
	[Have_You_Submitted_AnnualReseves_BookingStatus] [nchar](10) NULL,
	[Do_You_Have_Reserves_Growth_Strategy_Plan] [nchar](10) NULL,
	[Number_Of_Shut_In_Wells] [int] NULL,
	[How_Many_ShutIn_Wells_Are_Planned_To_Reactivate] [int] NULL,
	[How_Many_Wells_Do_You_Plan_To_Complete] [int] NULL,
	[How_Many_Wells_Do_You_Plan_To_Abandon] [int] NULL,
	[Number_Of_Well_Interventions_Planned_For_The_Year] [int] NULL,
	[CompanyNumber] [int] NULL,
	[ConcessionID] [int] NULL,
	[FieldID] [int] NULL,
	[DateCreated] [date] NULL,
 CONSTRAINT [PK_Development_And_Productions] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


/****** Object:  Table [dbo].[DevelopmentDrilling]    Script Date: 10/21/2022 1:28:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DevelopmentDrilling](
	[Development_DrillingId] [int] IDENTITY(1,1) NOT NULL,
	[Actual_Year] [varchar](5) NULL,
	[Proposed_Year] [varchar](5) NULL,
	[Budget_Allocation] [bigint] NULL,
	[Remarks] [varchar](max) NULL,
 CONSTRAINT [PK_DevelopmentDrilling] PRIMARY KEY CLUSTERED 
(
	[Development_DrillingId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Messages]    Script Date: 10/21/2022 1:28:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Messages](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[subject] [varchar](250) NULL,
	[content] [varchar](max) NULL,
	[read] [int] NULL,
	[companyID] [int] NULL,
	[sender_id] [nvarchar](200) NULL,
	[date] [datetime2](7) NULL,
	[AppId] [int] NULL,
	[staffID] [int] NULL,
	[UserType] [nchar](10) NULL,
 CONSTRAINT [PK_message_id] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MyDesks]    Script Date: 10/21/2022 1:28:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MyDesks](
	[DeskID] [int] IDENTITY(1,1) NOT NULL,
	[ProcessID] [int] NOT NULL,
	[StaffID] [int] NOT NULL,
	[AppId] [int] NULL,
	[Sort] [int] NOT NULL,
	[HasWork] [bit] NOT NULL,
	[HasPushed] [bit] NOT NULL,
	[FromStaffID] [int] NULL,
	[FromSBU] [varchar](50) NULL,
	[CreatedAt] [datetime] NOT NULL,
	[UpdatedAt] [datetime] NULL,
	[Comment] [varchar](max) NULL,
 CONSTRAINT [PK_MyDesk_UT] PRIMARY KEY CLUSTERED 
(
	[DeskID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PermitApprovals]    Script Date: 10/21/2022 1:28:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PermitApprovals](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PermitNo] [varchar](50) NOT NULL,
	[AppID] [int] NOT NULL,
	[CompanyID] [int] NOT NULL,
	[ElpsID] [int] NULL,
	[DateIssued] [datetime2](0) NOT NULL,
	[DateExpired] [datetime2](0) NOT NULL,
	[IsRenewed] [nvarchar](130) NULL,
	[Printed] [bit] NOT NULL,
	[ApprovedBy] [int] NULL,
	[CreatedAt] [datetime] NULL,
 CONSTRAINT [PK_permit_id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Planning_MinimumRequirement]    Script Date: 10/21/2022 1:28:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Planning_MinimumRequirement](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ReservesRevenue_GrossProduction] [varchar](50) NULL,
	[ReservesRevenue_RemainingReserves] [varchar](50) NULL,
	[CompanyNumber] [int] NULL,
	[ConcessionID] [int] NULL,
	[FieldID] [int] NULL,
	[DateCreated] [date] NULL,
	[Year] [int] NULL,
 CONSTRAINT [PK_Planning_MinimumRequirement] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Role]    Script Date: 10/21/2022 1:28:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Role](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[RoleId] [varchar](20) NOT NULL,
	[Description] [varchar](50) NULL,
	[RoleName] [varchar](100) NULL,
 CONSTRAINT [PK_Role] PRIMARY KEY CLUSTERED 
(
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ROLES_]    Script Date: 10/21/2022 1:28:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ROLES_](
	[SN] [int] IDENTITY(1,1) NOT NULL,
	[RoleId] [varchar](50) NULL,
	[Description] [varchar](500) NULL,
	[RoleName] [varchar](100) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[staff]    Script Date: 10/21/2022 1:28:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[staff](
	[StaffID] [int] IDENTITY(1,1) NOT NULL,
	[StaffElpsID] [varchar](100) NULL,
	[Staff_SBU] [int] NULL,
	[RoleID] [int] NULL,
	[LocationID] [int] NULL,
	[StaffEmail] [varchar](50) NULL,
	[FirstName] [varchar](30) NULL,
	[LastName] [varchar](30) NULL,
	[CreatedAt] [datetime] NULL,
	[ActiveStatus] [bit] NULL,
	[UpdatedAt] [datetime] NULL,
	[DeleteStatus] [bit] NULL,
	[DeletedBy] [int] NULL,
	[DeletedAt] [datetime] NULL,
	[CreatedBy] [int] NULL,
	[UpdatedBy] [int] NULL,
	[SignaturePath] [varchar](1000) NULL,
	[SignatureName] [varchar](100) NULL,
 CONSTRAINT [PK_Staff_UT] PRIMARY KEY CLUSTERED 
(
	[StaffID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[StrategicBusinessUnits]    Script Date: 10/21/2022 1:28:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StrategicBusinessUnits](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SBU_Name] [varchar](100) NULL,
	[SBU_Code] [nvarchar](10) NULL,
 CONSTRAINT [PK_StrategicBusinessUnit] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SubmittedDocuments]    Script Date: 10/21/2022 1:28:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SubmittedDocuments](
	[SubDocID] [int] IDENTITY(1,1) NOT NULL,
	[AppID] [int] NULL,
	[LocalDocID] [int] NULL,
	[ElpsDocID] [int] NULL,
	[YearOfWKP] [int] NULL,
	[DocSource] [varchar](max) NULL,
	[DocumentCategory] [varchar](100) NULL,
	[DocumentName] [varchar](150) NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedAt] [datetime] NULL,
	[UpdatedAt] [datetime] NULL,
 CONSTRAINT [PK_SubmittedDocuments] PRIMARY KEY CLUSTERED 
(
	[SubDocID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

CREATE TABLE [dbo].[HSE_MinimumRequirement](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Is_Company_ISO45001_Certified] [varchar](20) NULL,
	[Provide_ISO45001_Certification_No] [varchar](100) NULL,
	[CompanyNumber] [int] NULL,
	[DateCreated] [date] NULL,
	[ConcessionID] [int] NULL,
	[FieldID] [int] NULL,
	[Year] [int] NULL,
 CONSTRAINT [PK_HSE_MinimumRequirement] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[OilSpill_Incident] ADD [Proof_Of_Submission_Of_Valid_OSCP] varchar(300) NULL;
ALTER TABLE [dbo].[OilSpill_Incident] ADD [Evidence_Of_QuaterlySubmissions_Of_OilField_Chemicals] varchar(500) NULL;
ALTER TABLE [dbo].[OilSpill_Incident] ADD [Evidence_Of_MOUs_With_CAN] varchar(500) NULL;
ALTER TABLE [dbo].[HSE_WASTE_MANAGEMENT_NEW] ADD [Commitment_To_Waste_Management] varchar(300) NULL;
ALTER TABLE [dbo].[HSE_WASTE_MANAGEMENT_NEW] ADD [How_Much_Is_Budgeted_For_Waste_MGT_Plan] float NULL;
ALTER TABLE [dbo].[HSE_WASTE_MANAGEMENT_NEW] ADD [Evidence_Of_Submission_Of_Journey_MGT_Plan] varchar(300) NULL;
ALTER TABLE [dbo].[HSE_WASTE_MANAGEMENT_NEW] ADD [Are_Registered_Point_Sources_Valid] varchar(100) NULL;
ALTER TABLE [dbo].[HSE_WASTE_MANAGEMENT_NEW] ADD [Evidence_Of_Submission_Of_PreviousYears_Waste_Release] varchar(300) NULL;
