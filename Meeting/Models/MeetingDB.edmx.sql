
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 10/13/2016 17:30:22
-- Generated from EDMX file: C:\Users\chustgg\Source\Repos\EJIKmeeting\Meeting\Models\MeetingDB.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [MeetingDB];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_MessageFile]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[FileSet] DROP CONSTRAINT [FK_MessageFile];
GO
IF OBJECT_ID(N'[dbo].[FK_MessageUser]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[MessageSet] DROP CONSTRAINT [FK_MessageUser];
GO
IF OBJECT_ID(N'[dbo].[FK_UserChat]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[UserSet] DROP CONSTRAINT [FK_UserChat];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[ChatSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ChatSet];
GO
IF OBJECT_ID(N'[dbo].[FileSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[FileSet];
GO
IF OBJECT_ID(N'[dbo].[MessageSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[MessageSet];
GO
IF OBJECT_ID(N'[dbo].[UserSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[UserSet];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'ChatSet'
CREATE TABLE [dbo].[ChatSet] (
    [ID] uniqueidentifier  NOT NULL,
    [StartingTime] datetime  NOT NULL,
    [EndingTime] datetime  NULL,
    [Description] nvarchar(max)  NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [Status] nvarchar(8)  NOT NULL
);
GO

-- Creating table 'MessageSet'
CREATE TABLE [dbo].[MessageSet] (
    [ID] uniqueidentifier  NOT NULL,
    [Content] nvarchar(max)  NOT NULL,
    [SendingTime] datetime  NOT NULL,
    [Type] nvarchar(8)  NOT NULL,
    [User_ID] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'FileSet'
CREATE TABLE [dbo].[FileSet] (
    [ID] uniqueidentifier  NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [Message_ID] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'UserSet'
CREATE TABLE [dbo].[UserSet] (
    [ID] uniqueidentifier  NOT NULL,
    [Name] nvarchar(max)  NULL,
    [Nick] nvarchar(max)  NOT NULL,
    [EJIK_ID] uniqueidentifier  NULL,
    [Type] nvarchar(8)  NOT NULL,
    [Password] nvarchar(max)  NOT NULL,
    [Chat_ID] uniqueidentifier  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [ID] in table 'ChatSet'
ALTER TABLE [dbo].[ChatSet]
ADD CONSTRAINT [PK_ChatSet]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'MessageSet'
ALTER TABLE [dbo].[MessageSet]
ADD CONSTRAINT [PK_MessageSet]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'FileSet'
ALTER TABLE [dbo].[FileSet]
ADD CONSTRAINT [PK_FileSet]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'UserSet'
ALTER TABLE [dbo].[UserSet]
ADD CONSTRAINT [PK_UserSet]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [Message_ID] in table 'FileSet'
ALTER TABLE [dbo].[FileSet]
ADD CONSTRAINT [FK_MessageFile]
    FOREIGN KEY ([Message_ID])
    REFERENCES [dbo].[MessageSet]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_MessageFile'
CREATE INDEX [IX_FK_MessageFile]
ON [dbo].[FileSet]
    ([Message_ID]);
GO

-- Creating foreign key on [User_ID] in table 'MessageSet'
ALTER TABLE [dbo].[MessageSet]
ADD CONSTRAINT [FK_MessageUser]
    FOREIGN KEY ([User_ID])
    REFERENCES [dbo].[UserSet]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_MessageUser'
CREATE INDEX [IX_FK_MessageUser]
ON [dbo].[MessageSet]
    ([User_ID]);
GO

-- Creating foreign key on [Chat_ID] in table 'UserSet'
ALTER TABLE [dbo].[UserSet]
ADD CONSTRAINT [FK_UserChat]
    FOREIGN KEY ([Chat_ID])
    REFERENCES [dbo].[ChatSet]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_UserChat'
CREATE INDEX [IX_FK_UserChat]
ON [dbo].[UserSet]
    ([Chat_ID]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------