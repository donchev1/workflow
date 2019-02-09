IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
);
END

GO

IF  OBJECT_ID('Departments','U') IS NULL
BEGIN
CREATE TABLE [Departments] (
    [DepartmentId] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NULL,
    [UserId] int NOT NULL,
    CONSTRAINT [PK_Departments] PRIMARY KEY ([DepartmentId])
);
END

GO

IF  OBJECT_ID('NewMessagesMonitor','U') IS NULL
BEGIN
CREATE TABLE [NewMessagesMonitor] (
    [NewMessagesMonitorId] int NOT NULL IDENTITY,
    [Folirung] bit NOT NULL,
    [Handarbeit] bit NOT NULL,
    [Inkchet] bit NOT NULL,
    [Falcen] bit NOT NULL,
    [Covertirung] bit NOT NULL,
    [Lager] bit NOT NULL,
    [Fahrer] bit NOT NULL,
    CONSTRAINT [PK_NewMessagesMonitor] PRIMARY KEY ([NewMessagesMonitorId])
);
END

GO
    
IF  OBJECT_ID('Notes','U') IS NULL
BEGIN
CREATE TABLE [Notes] (
    [NoteId] int NOT NULL IDENTITY,
    [Content] nvarchar(max) NULL,
    [Location] int NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [Author] nvarchar(max) NULL,
    CONSTRAINT [PK_Notes] PRIMARY KEY ([NoteId])
);
END

GO

IF  OBJECT_ID('Orders','U') IS NULL
BEGIN
CREATE TABLE [Orders] (
    [OrderId] int NOT NULL IDENTITY,
    [OrderNumber] nvarchar(max) NULL,
    [EntityType] nvarchar(max) NULL,
    [EntityCount] int NOT NULL,
    [EntitiesInProgress] int NOT NULL,
    [EntitiesCompleted] int NOT NULL,
    [EntitiesNotProcessed] int NOT NULL,
    [Status] nvarchar(max) NULL,
    [CreatedAt] datetime2 NOT NULL,
    [StartedAt] datetime2 NOT NULL,
    [FinshedAt] datetime2 NOT NULL,
    [DeadLineDate] datetime2 NOT NULL,
    [Customer] nvarchar(max) NULL,
    CONSTRAINT [PK_Orders] PRIMARY KEY ([OrderId])
);
END

GO

IF  OBJECT_ID('Users','U') IS NULL
BEGIN
CREATE TABLE [Users] (
    [UserId] int NOT NULL IDENTITY,
    [UserName] nvarchar(max) NULL,
    [Password] nvarchar(max) NULL,
    [ConfirmPassword] nvarchar(max) NULL,
    [IsAdmin] bit NOT NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY ([UserId])
);
END

GO


IF  OBJECT_ID('DepartmentStates','U') IS NULL
BEGIN
CREATE TABLE [DepartmentStates] (
    [DepartmentStateId] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NULL,
    [EntitiesPassed] int NOT NULL,
    [EntitiesInProgress] int NOT NULL,
    [EntitiesRFC] int NOT NULL,
    [TotalEntityCount] int NOT NULL,
    [Start] datetime2 NOT NULL,
    [Finish] datetime2 NOT NULL,
    [LocationPosition] int NOT NULL,
    [Status] nvarchar(max) NULL,
    [OrderId] int NOT NULL,
    CONSTRAINT [PK_DepartmentStates] PRIMARY KEY ([DepartmentStateId]),
    CONSTRAINT [FK_DepartmentStates_Orders_OrderId] FOREIGN KEY ([OrderId]) REFERENCES [Orders] ([OrderId]) ON DELETE CASCADE
);
END

GO

IF  OBJECT_ID('Logs','U') IS NULL
BEGIN
CREATE TABLE [Logs] (
    [LogId] int NOT NULL IDENTITY,
    [ActionRecord] nvarchar(max) NULL,
    [CreatedAt] datetime2 NOT NULL,
    [OrderNumber] nvarchar(max) NULL,
    [UserName] nvarchar(max) NULL,
    [UserId] int NULL,
    CONSTRAINT [PK_Logs] PRIMARY KEY ([LogId]),
    CONSTRAINT [FK_Logs_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([UserId]) ON DELETE NO ACTION
);
END

GO

IF  OBJECT_ID('UserRoles','U') IS NULL
BEGIN
CREATE TABLE [UserRoles] (
    [UserRoleId] int NOT NULL IDENTITY,
    [Role] int NOT NULL,
    [UserId] int NOT NULL,
    CONSTRAINT [PK_UserRoles] PRIMARY KEY ([UserRoleId]),
    CONSTRAINT [FK_UserRoles_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([UserId]) ON DELETE CASCADE
);
END

GO


IF  OBJECT_ID('__EFMigrationsHistory','U') IS NOT NULL
AND NOT EXISTS(SELECT * FROM dbo.__EFMigrationsHistory WHERE MigrationId='20190209175335_MyFirstMigration') 
BEGIN
INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20190209175335_MyFirstMigration', N'2.2.1-servicing-10028');
END

GO

