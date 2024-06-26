USE [master]
GO
/****** Object:  Database [MyRentalHubDB]    Script Date: 18/04/2024 18:35:33 ******/
CREATE DATABASE [MyRentalHubDB]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'MyRentalHubDB', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.MSSQLSERVER\MSSQL\DATA\MyRentalHubDB.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'MyRentalHubDB_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.MSSQLSERVER\MSSQL\DATA\MyRentalHubDB_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT, LEDGER = OFF
GO
ALTER DATABASE [MyRentalHubDB] SET COMPATIBILITY_LEVEL = 160
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [MyRentalHubDB].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [MyRentalHubDB] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [MyRentalHubDB] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [MyRentalHubDB] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [MyRentalHubDB] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [MyRentalHubDB] SET ARITHABORT OFF 
GO
ALTER DATABASE [MyRentalHubDB] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [MyRentalHubDB] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [MyRentalHubDB] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [MyRentalHubDB] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [MyRentalHubDB] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [MyRentalHubDB] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [MyRentalHubDB] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [MyRentalHubDB] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [MyRentalHubDB] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [MyRentalHubDB] SET  DISABLE_BROKER 
GO
ALTER DATABASE [MyRentalHubDB] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [MyRentalHubDB] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [MyRentalHubDB] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [MyRentalHubDB] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [MyRentalHubDB] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [MyRentalHubDB] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [MyRentalHubDB] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [MyRentalHubDB] SET RECOVERY FULL 
GO
ALTER DATABASE [MyRentalHubDB] SET  MULTI_USER 
GO
ALTER DATABASE [MyRentalHubDB] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [MyRentalHubDB] SET DB_CHAINING OFF 
GO
ALTER DATABASE [MyRentalHubDB] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [MyRentalHubDB] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [MyRentalHubDB] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [MyRentalHubDB] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
EXEC sys.sp_db_vardecimal_storage_format N'MyRentalHubDB', N'ON'
GO
ALTER DATABASE [MyRentalHubDB] SET QUERY_STORE = ON
GO
ALTER DATABASE [MyRentalHubDB] SET QUERY_STORE (OPERATION_MODE = READ_WRITE, CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 30), DATA_FLUSH_INTERVAL_SECONDS = 900, INTERVAL_LENGTH_MINUTES = 60, MAX_STORAGE_SIZE_MB = 1000, QUERY_CAPTURE_MODE = AUTO, SIZE_BASED_CLEANUP_MODE = AUTO, MAX_PLANS_PER_QUERY = 200, WAIT_STATS_CAPTURE_MODE = ON)
GO
USE [MyRentalHubDB]
GO
/****** Object:  Table [dbo].[__EFMigrationsHistory]    Script Date: 18/04/2024 18:35:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[__EFMigrationsHistory](
	[MigrationId] [nvarchar](150) NOT NULL,
	[ProductVersion] [nvarchar](32) NOT NULL,
 CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY CLUSTERED 
(
	[MigrationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Apartments]    Script Date: 18/04/2024 18:35:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Apartments](
	[ApartmentId] [int] IDENTITY(1,1) NOT NULL,
	[PropertyId] [int] NOT NULL,
	[ApartmentNumber] [varchar](10) NOT NULL,
	[NbOfBeds] [int] NOT NULL,
	[NbOfBaths] [int] NOT NULL,
	[NbOfParkingSpots] [int] NOT NULL,
	[PriceAnnounced] [numeric](8, 2) NOT NULL,
	[AnimalsAccepted] [bit] NOT NULL,
 CONSTRAINT [PK_Apartments] PRIMARY KEY CLUSTERED 
(
	[ApartmentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Appointments]    Script Date: 18/04/2024 18:35:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Appointments](
	[AppointmentId] [int] IDENTITY(1,1) NOT NULL,
	[TenantId] [int] NOT NULL,
	[ApartmentId] [int] NOT NULL,
	[VisitDate] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_Appointments] PRIMARY KEY CLUSTERED 
(
	[AppointmentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Conversations]    Script Date: 18/04/2024 18:35:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Conversations](
	[ConversationId] [int] IDENTITY(1,1) NOT NULL,
	[TenantId] [int] NOT NULL,
	[ApartmentId] [int] NOT NULL,
 CONSTRAINT [PK_Conversations] PRIMARY KEY CLUSTERED 
(
	[ConversationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[EventsInProperties]    Script Date: 18/04/2024 18:35:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EventsInProperties](
	[EventId] [int] IDENTITY(1,1) NOT NULL,
	[PropertyId] [int] NOT NULL,
	[ApartmentId] [int] NULL,
	[EventTitle] [varchar](30) NOT NULL,
	[EventDescription] [text] NULL,
	[ReportDate] [date] NOT NULL,
 CONSTRAINT [PK_EventsInProperties] PRIMARY KEY CLUSTERED 
(
	[EventId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ManagerSlots]    Script Date: 18/04/2024 18:35:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ManagerSlots](
	[SlotId] [int] IDENTITY(1,1) NOT NULL,
	[ManagerId] [int] NOT NULL,
	[AvailableSlot] [datetime2](7) NOT NULL,
	[IsAlreadyScheduled] [bit] NOT NULL,
 CONSTRAINT [PK_ManagerSlots] PRIMARY KEY CLUSTERED 
(
	[SlotId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MessagesFromTenants]    Script Date: 18/04/2024 18:35:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MessagesFromTenants](
	[MessageId] [int] IDENTITY(1,1) NOT NULL,
	[ConversationId] [int] NOT NULL,
	[MessageSent] [text] NULL,
	[DateSent] [datetime2](7) NOT NULL,
	[AuthorType] [int] NOT NULL,
	[AuthorName] [nvarchar](max) NULL,
 CONSTRAINT [PK_MessagesFromTenants] PRIMARY KEY CLUSTERED 
(
	[MessageId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Properties]    Script Date: 18/04/2024 18:35:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Properties](
	[PropertyId] [int] IDENTITY(1,1) NOT NULL,
	[PropertyName] [nvarchar](max) NOT NULL,
	[AddressNumber] [nvarchar](max) NOT NULL,
	[AddressStreet] [nvarchar](max) NOT NULL,
	[PostalCode] [nvarchar](max) NOT NULL,
	[City] [nvarchar](max) NOT NULL,
	[Neighbourhood] [nvarchar](max) NULL,
	[ManagerId] [int] NOT NULL,
 CONSTRAINT [PK_Properties] PRIMARY KEY CLUSTERED 
(
	[PropertyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Rentals]    Script Date: 18/04/2024 18:35:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Rentals](
	[RentalId] [int] IDENTITY(1,1) NOT NULL,
	[TenantId] [int] NOT NULL,
	[ApartmentId] [int] NOT NULL,
	[FirstDayRental] [date] NOT NULL,
	[LastDayRental] [date] NOT NULL,
	[PriceRent] [numeric](8, 2) NOT NULL,
	[RentalStatus] [int] NOT NULL,
 CONSTRAINT [PK_Rentals] PRIMARY KEY CLUSTERED 
(
	[RentalId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserAccounts]    Script Date: 18/04/2024 18:35:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserAccounts](
	[UserId] [int] IDENTITY(1,1) NOT NULL,
	[UserType] [int] NOT NULL,
	[UserName] [nvarchar](max) NOT NULL,
	[UserPassword] [nvarchar](max) NOT NULL,
	[DateCreated] [datetime2](7) NOT NULL,
	[FirstName] [nvarchar](max) NOT NULL,
	[LastName] [nvarchar](max) NOT NULL,
	[UserStatus] [int] NOT NULL,
 CONSTRAINT [PK_UserAccounts] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20240418220048_Initial', N'8.0.4')
GO
SET IDENTITY_INSERT [dbo].[Apartments] ON 

INSERT [dbo].[Apartments] ([ApartmentId], [PropertyId], [ApartmentNumber], [NbOfBeds], [NbOfBaths], [NbOfParkingSpots], [PriceAnnounced], [AnimalsAccepted]) VALUES (1, 1, N'101', 3, 2, 1, CAST(2400.00 AS Numeric(8, 2)), 1)
INSERT [dbo].[Apartments] ([ApartmentId], [PropertyId], [ApartmentNumber], [NbOfBeds], [NbOfBaths], [NbOfParkingSpots], [PriceAnnounced], [AnimalsAccepted]) VALUES (2, 1, N'102', 2, 1, 0, CAST(1900.00 AS Numeric(8, 2)), 1)
INSERT [dbo].[Apartments] ([ApartmentId], [PropertyId], [ApartmentNumber], [NbOfBeds], [NbOfBaths], [NbOfParkingSpots], [PriceAnnounced], [AnimalsAccepted]) VALUES (3, 2, N'10A', 1, 1, 0, CAST(1500.00 AS Numeric(8, 2)), 0)
SET IDENTITY_INSERT [dbo].[Apartments] OFF
GO
SET IDENTITY_INSERT [dbo].[Appointments] ON 

INSERT [dbo].[Appointments] ([AppointmentId], [TenantId], [ApartmentId], [VisitDate]) VALUES (1, 4, 1, CAST(N'2024-04-23T10:30:00.0000000' AS DateTime2))
SET IDENTITY_INSERT [dbo].[Appointments] OFF
GO
SET IDENTITY_INSERT [dbo].[Conversations] ON 

INSERT [dbo].[Conversations] ([ConversationId], [TenantId], [ApartmentId]) VALUES (1, 4, 1)
SET IDENTITY_INSERT [dbo].[Conversations] OFF
GO
SET IDENTITY_INSERT [dbo].[ManagerSlots] ON 

INSERT [dbo].[ManagerSlots] ([SlotId], [ManagerId], [AvailableSlot], [IsAlreadyScheduled]) VALUES (1, 3, CAST(N'2024-04-22T08:30:00.0000000' AS DateTime2), 0)
INSERT [dbo].[ManagerSlots] ([SlotId], [ManagerId], [AvailableSlot], [IsAlreadyScheduled]) VALUES (2, 3, CAST(N'2024-04-22T09:30:00.0000000' AS DateTime2), 0)
INSERT [dbo].[ManagerSlots] ([SlotId], [ManagerId], [AvailableSlot], [IsAlreadyScheduled]) VALUES (3, 3, CAST(N'2024-04-22T10:30:00.0000000' AS DateTime2), 0)
INSERT [dbo].[ManagerSlots] ([SlotId], [ManagerId], [AvailableSlot], [IsAlreadyScheduled]) VALUES (4, 3, CAST(N'2024-04-23T08:30:00.0000000' AS DateTime2), 0)
INSERT [dbo].[ManagerSlots] ([SlotId], [ManagerId], [AvailableSlot], [IsAlreadyScheduled]) VALUES (5, 3, CAST(N'2024-04-23T09:30:00.0000000' AS DateTime2), 0)
INSERT [dbo].[ManagerSlots] ([SlotId], [ManagerId], [AvailableSlot], [IsAlreadyScheduled]) VALUES (6, 3, CAST(N'2024-04-23T10:30:00.0000000' AS DateTime2), 1)
INSERT [dbo].[ManagerSlots] ([SlotId], [ManagerId], [AvailableSlot], [IsAlreadyScheduled]) VALUES (7, 3, CAST(N'2024-04-24T08:30:00.0000000' AS DateTime2), 0)
INSERT [dbo].[ManagerSlots] ([SlotId], [ManagerId], [AvailableSlot], [IsAlreadyScheduled]) VALUES (8, 3, CAST(N'2024-04-24T09:30:00.0000000' AS DateTime2), 0)
INSERT [dbo].[ManagerSlots] ([SlotId], [ManagerId], [AvailableSlot], [IsAlreadyScheduled]) VALUES (9, 3, CAST(N'2024-04-24T10:30:00.0000000' AS DateTime2), 0)
INSERT [dbo].[ManagerSlots] ([SlotId], [ManagerId], [AvailableSlot], [IsAlreadyScheduled]) VALUES (10, 3, CAST(N'2024-04-25T08:30:00.0000000' AS DateTime2), 0)
INSERT [dbo].[ManagerSlots] ([SlotId], [ManagerId], [AvailableSlot], [IsAlreadyScheduled]) VALUES (11, 3, CAST(N'2024-04-25T09:30:00.0000000' AS DateTime2), 0)
INSERT [dbo].[ManagerSlots] ([SlotId], [ManagerId], [AvailableSlot], [IsAlreadyScheduled]) VALUES (12, 3, CAST(N'2024-04-25T10:30:00.0000000' AS DateTime2), 0)
INSERT [dbo].[ManagerSlots] ([SlotId], [ManagerId], [AvailableSlot], [IsAlreadyScheduled]) VALUES (13, 3, CAST(N'2024-04-26T08:30:00.0000000' AS DateTime2), 0)
INSERT [dbo].[ManagerSlots] ([SlotId], [ManagerId], [AvailableSlot], [IsAlreadyScheduled]) VALUES (14, 3, CAST(N'2024-04-26T09:30:00.0000000' AS DateTime2), 0)
INSERT [dbo].[ManagerSlots] ([SlotId], [ManagerId], [AvailableSlot], [IsAlreadyScheduled]) VALUES (15, 3, CAST(N'2024-04-26T10:30:00.0000000' AS DateTime2), 0)
SET IDENTITY_INSERT [dbo].[ManagerSlots] OFF
GO
SET IDENTITY_INSERT [dbo].[MessagesFromTenants] ON 

INSERT [dbo].[MessagesFromTenants] ([MessageId], [ConversationId], [MessageSent], [DateSent], [AuthorType], [AuthorName]) VALUES (1, 1, N'Hello. My name is Bernardo, and I am really interested in this apartment. I would like to talk to the manager.', CAST(N'2024-04-18T18:17:06.4843241' AS DateTime2), 3, N'Bernardo Araujo')
INSERT [dbo].[MessagesFromTenants] ([MessageId], [ConversationId], [MessageSent], [DateSent], [AuthorType], [AuthorName]) VALUES (2, 1, N'Hello Bernardo. I opened some appointment slots for visitation. Would you please schedule an appointment and we can discuss.', CAST(N'2024-04-18T18:19:28.6622262' AS DateTime2), 2, N'Peter White')
SET IDENTITY_INSERT [dbo].[MessagesFromTenants] OFF
GO
SET IDENTITY_INSERT [dbo].[Properties] ON 

INSERT [dbo].[Properties] ([PropertyId], [PropertyName], [AddressNumber], [AddressStreet], [PostalCode], [City], [Neighbourhood], [ManagerId]) VALUES (1, N'Côte-Saint-Luc', N'2345', N'Avenue De Lorimier', N'H4D 5S5', N'Montréal', N'La Salle', 3)
INSERT [dbo].[Properties] ([PropertyId], [PropertyName], [AddressNumber], [AddressStreet], [PostalCode], [City], [Neighbourhood], [ManagerId]) VALUES (2, N'Côte-du-Soleil', N'8765', N'Rue Professeur Kaiser', N'H2A 4F5', N'Montréal', N'Rosemont', 2)
SET IDENTITY_INSERT [dbo].[Properties] OFF
GO
SET IDENTITY_INSERT [dbo].[Rentals] ON 

INSERT [dbo].[Rentals] ([RentalId], [TenantId], [ApartmentId], [FirstDayRental], [LastDayRental], [PriceRent], [RentalStatus]) VALUES (1, 4, 1, CAST(N'2024-07-01' AS Date), CAST(N'2025-06-30' AS Date), CAST(2300.00 AS Numeric(8, 2)), 0)
SET IDENTITY_INSERT [dbo].[Rentals] OFF
GO
SET IDENTITY_INSERT [dbo].[UserAccounts] ON 

INSERT [dbo].[UserAccounts] ([UserId], [UserType], [UserName], [UserPassword], [DateCreated], [FirstName], [LastName], [UserStatus]) VALUES (1, 0, N'clara@owner.com', N'123456', CAST(N'2024-04-18T00:00:00.0000000' AS DateTime2), N'Clara', N'Carvalho', 0)
INSERT [dbo].[UserAccounts] ([UserId], [UserType], [UserName], [UserPassword], [DateCreated], [FirstName], [LastName], [UserStatus]) VALUES (2, 2, N'mary@manager.com', N'123456', CAST(N'2024-04-18T00:00:00.0000000' AS DateTime2), N'Mary', N'Brown', 0)
INSERT [dbo].[UserAccounts] ([UserId], [UserType], [UserName], [UserPassword], [DateCreated], [FirstName], [LastName], [UserStatus]) VALUES (3, 2, N'peter@manager.com', N'123456', CAST(N'2024-04-18T00:00:00.0000000' AS DateTime2), N'Peter', N'White', 0)
INSERT [dbo].[UserAccounts] ([UserId], [UserType], [UserName], [UserPassword], [DateCreated], [FirstName], [LastName], [UserStatus]) VALUES (4, 3, N'bgparaujo@gmail.com', N'123456', CAST(N'2024-04-18T00:00:00.0000000' AS DateTime2), N'Bernardo', N'Araujo', 0)
SET IDENTITY_INSERT [dbo].[UserAccounts] OFF
GO
/****** Object:  Index [IX_Apartments_PropertyId]    Script Date: 18/04/2024 18:35:33 ******/
CREATE NONCLUSTERED INDEX [IX_Apartments_PropertyId] ON [dbo].[Apartments]
(
	[PropertyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Appointments_ApartmentId]    Script Date: 18/04/2024 18:35:33 ******/
CREATE NONCLUSTERED INDEX [IX_Appointments_ApartmentId] ON [dbo].[Appointments]
(
	[ApartmentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Appointments_TenantId]    Script Date: 18/04/2024 18:35:33 ******/
CREATE NONCLUSTERED INDEX [IX_Appointments_TenantId] ON [dbo].[Appointments]
(
	[TenantId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Conversations_ApartmentId]    Script Date: 18/04/2024 18:35:33 ******/
CREATE NONCLUSTERED INDEX [IX_Conversations_ApartmentId] ON [dbo].[Conversations]
(
	[ApartmentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Conversations_TenantId]    Script Date: 18/04/2024 18:35:33 ******/
CREATE NONCLUSTERED INDEX [IX_Conversations_TenantId] ON [dbo].[Conversations]
(
	[TenantId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_EventsInProperties_ApartmentId]    Script Date: 18/04/2024 18:35:33 ******/
CREATE NONCLUSTERED INDEX [IX_EventsInProperties_ApartmentId] ON [dbo].[EventsInProperties]
(
	[ApartmentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_EventsInProperties_PropertyId]    Script Date: 18/04/2024 18:35:33 ******/
CREATE NONCLUSTERED INDEX [IX_EventsInProperties_PropertyId] ON [dbo].[EventsInProperties]
(
	[PropertyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_ManagerSlots_ManagerId]    Script Date: 18/04/2024 18:35:33 ******/
CREATE NONCLUSTERED INDEX [IX_ManagerSlots_ManagerId] ON [dbo].[ManagerSlots]
(
	[ManagerId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_MessagesFromTenants_ConversationId]    Script Date: 18/04/2024 18:35:33 ******/
CREATE NONCLUSTERED INDEX [IX_MessagesFromTenants_ConversationId] ON [dbo].[MessagesFromTenants]
(
	[ConversationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Properties_ManagerId]    Script Date: 18/04/2024 18:35:33 ******/
CREATE NONCLUSTERED INDEX [IX_Properties_ManagerId] ON [dbo].[Properties]
(
	[ManagerId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Rentals_ApartmentId]    Script Date: 18/04/2024 18:35:33 ******/
CREATE NONCLUSTERED INDEX [IX_Rentals_ApartmentId] ON [dbo].[Rentals]
(
	[ApartmentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Rentals_TenantId]    Script Date: 18/04/2024 18:35:33 ******/
CREATE NONCLUSTERED INDEX [IX_Rentals_TenantId] ON [dbo].[Rentals]
(
	[TenantId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Apartments]  WITH CHECK ADD  CONSTRAINT [FK_Apartments_Properties_PropertyId] FOREIGN KEY([PropertyId])
REFERENCES [dbo].[Properties] ([PropertyId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Apartments] CHECK CONSTRAINT [FK_Apartments_Properties_PropertyId]
GO
ALTER TABLE [dbo].[Appointments]  WITH CHECK ADD  CONSTRAINT [FK_Appointments_Apartments_ApartmentId] FOREIGN KEY([ApartmentId])
REFERENCES [dbo].[Apartments] ([ApartmentId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Appointments] CHECK CONSTRAINT [FK_Appointments_Apartments_ApartmentId]
GO
ALTER TABLE [dbo].[Appointments]  WITH CHECK ADD  CONSTRAINT [FK_Appointments_UserAccounts_TenantId] FOREIGN KEY([TenantId])
REFERENCES [dbo].[UserAccounts] ([UserId])
GO
ALTER TABLE [dbo].[Appointments] CHECK CONSTRAINT [FK_Appointments_UserAccounts_TenantId]
GO
ALTER TABLE [dbo].[Conversations]  WITH CHECK ADD  CONSTRAINT [FK_Conversations_Apartments_ApartmentId] FOREIGN KEY([ApartmentId])
REFERENCES [dbo].[Apartments] ([ApartmentId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Conversations] CHECK CONSTRAINT [FK_Conversations_Apartments_ApartmentId]
GO
ALTER TABLE [dbo].[Conversations]  WITH CHECK ADD  CONSTRAINT [FK_Conversations_UserAccounts_TenantId] FOREIGN KEY([TenantId])
REFERENCES [dbo].[UserAccounts] ([UserId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Conversations] CHECK CONSTRAINT [FK_Conversations_UserAccounts_TenantId]
GO
ALTER TABLE [dbo].[EventsInProperties]  WITH CHECK ADD  CONSTRAINT [FK_EventsInProperties_Apartments_ApartmentId] FOREIGN KEY([ApartmentId])
REFERENCES [dbo].[Apartments] ([ApartmentId])
GO
ALTER TABLE [dbo].[EventsInProperties] CHECK CONSTRAINT [FK_EventsInProperties_Apartments_ApartmentId]
GO
ALTER TABLE [dbo].[EventsInProperties]  WITH CHECK ADD  CONSTRAINT [FK_EventsInProperties_Properties_PropertyId] FOREIGN KEY([PropertyId])
REFERENCES [dbo].[Properties] ([PropertyId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[EventsInProperties] CHECK CONSTRAINT [FK_EventsInProperties_Properties_PropertyId]
GO
ALTER TABLE [dbo].[ManagerSlots]  WITH CHECK ADD  CONSTRAINT [FK_ManagerSlots_UserAccounts_ManagerId] FOREIGN KEY([ManagerId])
REFERENCES [dbo].[UserAccounts] ([UserId])
GO
ALTER TABLE [dbo].[ManagerSlots] CHECK CONSTRAINT [FK_ManagerSlots_UserAccounts_ManagerId]
GO
ALTER TABLE [dbo].[MessagesFromTenants]  WITH CHECK ADD  CONSTRAINT [FK_MessagesFromTenants_Conversations_ConversationId] FOREIGN KEY([ConversationId])
REFERENCES [dbo].[Conversations] ([ConversationId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[MessagesFromTenants] CHECK CONSTRAINT [FK_MessagesFromTenants_Conversations_ConversationId]
GO
ALTER TABLE [dbo].[Properties]  WITH CHECK ADD  CONSTRAINT [FK_Properties_UserAccounts_ManagerId] FOREIGN KEY([ManagerId])
REFERENCES [dbo].[UserAccounts] ([UserId])
GO
ALTER TABLE [dbo].[Properties] CHECK CONSTRAINT [FK_Properties_UserAccounts_ManagerId]
GO
ALTER TABLE [dbo].[Rentals]  WITH CHECK ADD  CONSTRAINT [FK_Rentals_Apartments_ApartmentId] FOREIGN KEY([ApartmentId])
REFERENCES [dbo].[Apartments] ([ApartmentId])
GO
ALTER TABLE [dbo].[Rentals] CHECK CONSTRAINT [FK_Rentals_Apartments_ApartmentId]
GO
ALTER TABLE [dbo].[Rentals]  WITH CHECK ADD  CONSTRAINT [FK_Rentals_UserAccounts_TenantId] FOREIGN KEY([TenantId])
REFERENCES [dbo].[UserAccounts] ([UserId])
GO
ALTER TABLE [dbo].[Rentals] CHECK CONSTRAINT [FK_Rentals_UserAccounts_TenantId]
GO
USE [master]
GO
ALTER DATABASE [MyRentalHubDB] SET  READ_WRITE 
GO
