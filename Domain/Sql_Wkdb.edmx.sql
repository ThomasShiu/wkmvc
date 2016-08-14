
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 08/10/2016 22:32:29
-- Generated from EDMX file: E:\CodeProjects\GitRepo\coding_up\asp.net\Domain\Sql_Wkdb.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [wkmvc_db];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_MAIL_ATTACHMENT_MAIL_OUTBOX]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[MAIL_ATTACHMENT] DROP CONSTRAINT [FK_MAIL_ATTACHMENT_MAIL_OUTBOX];
GO
IF OBJECT_ID(N'[dbo].[FK_MAIL_INBOX_MAIL_OUTBOX]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[MAIL_INBOX] DROP CONSTRAINT [FK_MAIL_INBOX_MAIL_OUTBOX];
GO
IF OBJECT_ID(N'[dbo].[FK_PRO_PROJECT_MESSAGE_PRO_PROJECTS]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[PRO_PROJECT_MESSAGE] DROP CONSTRAINT [FK_PRO_PROJECT_MESSAGE_PRO_PROJECTS];
GO
IF OBJECT_ID(N'[dbo].[FK_PRO_PROJECT_STAGES_PRO_PROJECTS]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[PRO_PROJECT_STAGES] DROP CONSTRAINT [FK_PRO_PROJECT_STAGES_PRO_PROJECTS];
GO
IF OBJECT_ID(N'[dbo].[FK_PRO_PROJECT_TEAMS_PRO_PROJECT_STAGES]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[PRO_PROJECT_TEAMS] DROP CONSTRAINT [FK_PRO_PROJECT_TEAMS_PRO_PROJECT_STAGES];
GO
IF OBJECT_ID(N'[dbo].[FK_SYS_MODULE_SYSTEMID]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SYS_MODULE] DROP CONSTRAINT [FK_SYS_MODULE_SYSTEMID];
GO
IF OBJECT_ID(N'[dbo].[FK_SYS_PERMISSION_MODULEID]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SYS_PERMISSION] DROP CONSTRAINT [FK_SYS_PERMISSION_MODULEID];
GO
IF OBJECT_ID(N'[dbo].[FK_SYS_POST_SYS_DEPARTMENT]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SYS_POST] DROP CONSTRAINT [FK_SYS_POST_SYS_DEPARTMENT];
GO
IF OBJECT_ID(N'[dbo].[FK_SYS_ROLE_PERMISSION_PERID]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SYS_ROLE_PERMISSION] DROP CONSTRAINT [FK_SYS_ROLE_PERMISSION_PERID];
GO
IF OBJECT_ID(N'[dbo].[FK_SYS_ROLE_PERMISSION_ROLEID]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SYS_ROLE_PERMISSION] DROP CONSTRAINT [FK_SYS_ROLE_PERMISSION_ROLEID];
GO
IF OBJECT_ID(N'[dbo].[FK_SYS_USER_ONLINE_SYS_USER_ONLINE]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SYS_USER_ONLINE] DROP CONSTRAINT [FK_SYS_USER_ONLINE_SYS_USER_ONLINE];
GO
IF OBJECT_ID(N'[dbo].[FK_SYS_USER_PERMISSION_PERID]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SYS_USER_PERMISSION] DROP CONSTRAINT [FK_SYS_USER_PERMISSION_PERID];
GO
IF OBJECT_ID(N'[dbo].[FK_SYS_USER_PERMISSION_USERID]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SYS_USER_PERMISSION] DROP CONSTRAINT [FK_SYS_USER_PERMISSION_USERID];
GO
IF OBJECT_ID(N'[dbo].[FK_SYS_USER_ROLE_ROLEID]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SYS_USER_ROLE] DROP CONSTRAINT [FK_SYS_USER_ROLE_ROLEID];
GO
IF OBJECT_ID(N'[dbo].[FK_SYS_USER_ROLE_USERID]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SYS_USER_ROLE] DROP CONSTRAINT [FK_SYS_USER_ROLE_USERID];
GO
IF OBJECT_ID(N'[dbo].[FK_SYS_USERINFO_SYSUSERID]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SYS_USERINFO] DROP CONSTRAINT [FK_SYS_USERINFO_SYSUSERID];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[COM_CONTENT]', 'U') IS NOT NULL
    DROP TABLE [dbo].[COM_CONTENT];
GO
IF OBJECT_ID(N'[dbo].[COM_DAILYS]', 'U') IS NOT NULL
    DROP TABLE [dbo].[COM_DAILYS];
GO
IF OBJECT_ID(N'[dbo].[COM_UPLOAD]', 'U') IS NOT NULL
    DROP TABLE [dbo].[COM_UPLOAD];
GO
IF OBJECT_ID(N'[dbo].[COM_WORKATTENDANCE]', 'U') IS NOT NULL
    DROP TABLE [dbo].[COM_WORKATTENDANCE];
GO
IF OBJECT_ID(N'[dbo].[MAIL_ATTACHMENT]', 'U') IS NOT NULL
    DROP TABLE [dbo].[MAIL_ATTACHMENT];
GO
IF OBJECT_ID(N'[dbo].[MAIL_INBOX]', 'U') IS NOT NULL
    DROP TABLE [dbo].[MAIL_INBOX];
GO
IF OBJECT_ID(N'[dbo].[MAIL_OUTBOX]', 'U') IS NOT NULL
    DROP TABLE [dbo].[MAIL_OUTBOX];
GO
IF OBJECT_ID(N'[dbo].[PRO_PROJECT_FILES]', 'U') IS NOT NULL
    DROP TABLE [dbo].[PRO_PROJECT_FILES];
GO
IF OBJECT_ID(N'[dbo].[PRO_PROJECT_MESSAGE]', 'U') IS NOT NULL
    DROP TABLE [dbo].[PRO_PROJECT_MESSAGE];
GO
IF OBJECT_ID(N'[dbo].[PRO_PROJECT_STAGES]', 'U') IS NOT NULL
    DROP TABLE [dbo].[PRO_PROJECT_STAGES];
GO
IF OBJECT_ID(N'[dbo].[PRO_PROJECT_TEAMS]', 'U') IS NOT NULL
    DROP TABLE [dbo].[PRO_PROJECT_TEAMS];
GO
IF OBJECT_ID(N'[dbo].[PRO_PROJECTS]', 'U') IS NOT NULL
    DROP TABLE [dbo].[PRO_PROJECTS];
GO
IF OBJECT_ID(N'[dbo].[SYS_BUSSINESSCUSTOMER]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SYS_BUSSINESSCUSTOMER];
GO
IF OBJECT_ID(N'[dbo].[SYS_CHATMESSAGE]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SYS_CHATMESSAGE];
GO
IF OBJECT_ID(N'[dbo].[SYS_CODE]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SYS_CODE];
GO
IF OBJECT_ID(N'[dbo].[SYS_CODE_AREA]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SYS_CODE_AREA];
GO
IF OBJECT_ID(N'[dbo].[SYS_DEPARTMENT]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SYS_DEPARTMENT];
GO
IF OBJECT_ID(N'[dbo].[SYS_LOG]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SYS_LOG];
GO
IF OBJECT_ID(N'[dbo].[SYS_MODULE]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SYS_MODULE];
GO
IF OBJECT_ID(N'[dbo].[SYS_PERMISSION]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SYS_PERMISSION];
GO
IF OBJECT_ID(N'[dbo].[SYS_POST]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SYS_POST];
GO
IF OBJECT_ID(N'[dbo].[SYS_POST_USER]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SYS_POST_USER];
GO
IF OBJECT_ID(N'[dbo].[SYS_ROLE]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SYS_ROLE];
GO
IF OBJECT_ID(N'[dbo].[SYS_ROLE_PERMISSION]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SYS_ROLE_PERMISSION];
GO
IF OBJECT_ID(N'[dbo].[SYS_SYSTEM]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SYS_SYSTEM];
GO
IF OBJECT_ID(N'[dbo].[SYS_USER]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SYS_USER];
GO
IF OBJECT_ID(N'[dbo].[SYS_USER_ONLINE]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SYS_USER_ONLINE];
GO
IF OBJECT_ID(N'[dbo].[SYS_USER_PERMISSION]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SYS_USER_PERMISSION];
GO
IF OBJECT_ID(N'[dbo].[SYS_USER_ROLE]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SYS_USER_ROLE];
GO
IF OBJECT_ID(N'[dbo].[SYS_USERINFO]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SYS_USERINFO];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'SYS_USER'
CREATE TABLE [dbo].[SYS_USER] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [NAME] nvarchar(50)  NULL,
    [ACCOUNT] nvarchar(20)  NULL,
    [PASSWORD] nvarchar(1000)  NULL,
    [ISCANLOGIN] int  NOT NULL,
    [SHOWORDER1] int  NULL,
    [SHOWORDER2] int  NULL,
    [PINYIN1] nvarchar(50)  NULL,
    [PINYIN2] nvarchar(50)  NULL,
    [FACE_IMG] nvarchar(max)  NULL,
    [LEVELS] nvarchar(36)  NULL,
    [DPTID] nvarchar(36)  NULL,
    [CREATEPER] nvarchar(36)  NULL,
    [CREATEDATE] datetime  NULL,
    [UPDATEUSER] nvarchar(36)  NULL,
    [UPDATEDATE] datetime  NULL,
    [LastLoginIP] nvarchar(50)  NULL
);
GO

-- Creating table 'SYS_CODE'
CREATE TABLE [dbo].[SYS_CODE] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [CODETYPE] nvarchar(50)  NULL,
    [NAMETEXT] nvarchar(200)  NULL,
    [CODEVALUE] nvarchar(100)  NULL,
    [SHOWORDER] int  NULL,
    [ISCODE] int  NOT NULL,
    [REMARK] nvarchar(2000)  NULL,
    [CREATEDATE] datetime  NULL,
    [CREATEUSER] nvarchar(36)  NULL,
    [UPDATEDATE] datetime  NULL,
    [UPDATEUSER] nvarchar(36)  NULL,
    [PARENTID] int  NULL
);
GO

-- Creating table 'SYS_CODE_AREA'
CREATE TABLE [dbo].[SYS_CODE_AREA] (
    [ID] varchar(50)  NOT NULL,
    [PID] varchar(50)  NOT NULL,
    [NAME] nvarchar(200)  NULL,
    [LEVELS] tinyint  NOT NULL
);
GO

-- Creating table 'SYS_DEPARTMENT'
CREATE TABLE [dbo].[SYS_DEPARTMENT] (
    [ID] nvarchar(36)  NOT NULL,
    [CODE] nvarchar(100)  NULL,
    [NAME] nvarchar(200)  NULL,
    [BUSINESSLEVEL] int  NULL,
    [SHOWORDER] int  NULL,
    [CREATEPERID] nvarchar(36)  NULL,
    [CREATEDATE] datetime  NULL,
    [PARENTID] nvarchar(36)  NULL,
    [UPDATEDATE] datetime  NULL,
    [UPDATEUSER] nvarchar(36)  NULL,
    [PARENTCODE] nvarchar(100)  NULL
);
GO

-- Creating table 'SYS_LOG'
CREATE TABLE [dbo].[SYS_LOG] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [DATES] datetime  NULL,
    [LEVELS] nvarchar(20)  NULL,
    [LOGGER] nvarchar(200)  NULL,
    [CLIENTUSER] nvarchar(100)  NULL,
    [CLIENTIP] nvarchar(20)  NULL,
    [REQUESTURL] nvarchar(500)  NULL,
    [ACTION] nvarchar(20)  NULL,
    [MESSAGE] nvarchar(4000)  NULL,
    [EXCEPTION] nvarchar(4000)  NULL
);
GO

-- Creating table 'SYS_MODULE'
CREATE TABLE [dbo].[SYS_MODULE] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [FK_BELONGSYSTEM] nvarchar(36)  NOT NULL,
    [PARENTID] int  NOT NULL,
    [NAME] nvarchar(50)  NULL,
    [ALIAS] nvarchar(50)  NULL,
    [MODULETYPE] int  NOT NULL,
    [ICON] nvarchar(200)  NULL,
    [MODULEPATH] nvarchar(500)  NULL,
    [ISSHOW] bit  NOT NULL,
    [SHOWORDER] int  NOT NULL,
    [LEVELS] int  NOT NULL,
    [IsVillage] bit  NOT NULL,
    [CREATEUSER] nvarchar(50)  NULL,
    [CREATEDATE] datetime  NULL,
    [UPDATEUSER] nvarchar(36)  NULL,
    [UPDATEDATE] datetime  NULL
);
GO

-- Creating table 'SYS_PERMISSION'
CREATE TABLE [dbo].[SYS_PERMISSION] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [MODULEID] int  NOT NULL,
    [NAME] nvarchar(36)  NULL,
    [PERVALUE] nvarchar(100)  NULL,
    [ICON] nvarchar(50)  NULL,
    [SHOWORDER] int  NULL,
    [CREATEDATE] datetime  NULL,
    [CREATEUSER] nvarchar(36)  NULL,
    [UPDATEDATE] datetime  NULL,
    [UPDATEUSER] nvarchar(36)  NULL
);
GO

-- Creating table 'SYS_POST'
CREATE TABLE [dbo].[SYS_POST] (
    [ID] nvarchar(36)  NOT NULL,
    [POSTNAME] nvarchar(100)  NULL,
    [POSTTYPE] nvarchar(36)  NOT NULL,
    [REMARK] nvarchar(500)  NULL,
    [SHOWORDER] int  NULL,
    [CREATEUSERID] int  NULL,
    [CREATEDATE] datetime  NOT NULL,
    [UPDATEDATE] datetime  NULL,
    [UPDATEUSER] nvarchar(36)  NULL,
    [FK_DEPARTID] nvarchar(36)  NOT NULL,
    [CREATEUSER] nvarchar(50)  NULL
);
GO

-- Creating table 'SYS_POST_DEPARTMENT'
CREATE TABLE [dbo].[SYS_POST_DEPARTMENT] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [FK_DEPARTMENT_ID] nvarchar(36)  NOT NULL,
    [FK_POST_ID] nvarchar(36)  NOT NULL
);
GO

-- Creating table 'SYS_POST_USER'
CREATE TABLE [dbo].[SYS_POST_USER] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [FK_USERID] int  NOT NULL,
    [FK_POST_DEPARTMENTID] int  NOT NULL,
    [FK_POSTID] nvarchar(36)  NOT NULL
);
GO

-- Creating table 'SYS_ROLE'
CREATE TABLE [dbo].[SYS_ROLE] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [ROLENAME] nvarchar(50)  NULL,
    [ISCUSTOM] int  NOT NULL,
    [ROLEDESC] nvarchar(1000)  NULL,
    [CREATEPERID] nvarchar(36)  NOT NULL,
    [CREATEDATE] datetime  NOT NULL,
    [UPDATEDATE] datetime  NOT NULL,
    [UPDATEUSER] nvarchar(36)  NULL,
    [FK_BELONGSYSTEM] nvarchar(36)  NOT NULL
);
GO

-- Creating table 'SYS_ROLE_PERMISSION'
CREATE TABLE [dbo].[SYS_ROLE_PERMISSION] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [ROLEID] int  NOT NULL,
    [PERMISSIONID] int  NOT NULL
);
GO

-- Creating table 'SYS_SYSTEM'
CREATE TABLE [dbo].[SYS_SYSTEM] (
    [ID] nvarchar(36)  NOT NULL,
    [NAME] nvarchar(200)  NULL,
    [SITEURL] nvarchar(500)  NULL,
    [IS_LOGIN] tinyint  NOT NULL,
    [DOCKUSER] nvarchar(100)  NULL,
    [DOCKPASS] nvarchar(200)  NULL,
    [CREATEDATE] datetime  NULL,
    [REMARK] nvarchar(2000)  NULL
);
GO

-- Creating table 'SYS_USER_DEPARTMENT'
CREATE TABLE [dbo].[SYS_USER_DEPARTMENT] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [USER_ID] int  NOT NULL,
    [DEPARTMENT_ID] nvarchar(36)  NOT NULL
);
GO

-- Creating table 'SYS_USER_PERMISSION'
CREATE TABLE [dbo].[SYS_USER_PERMISSION] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [FK_USERID] int  NOT NULL,
    [FK_PERMISSIONID] int  NOT NULL
);
GO

-- Creating table 'SYS_USER_ROLE'
CREATE TABLE [dbo].[SYS_USER_ROLE] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [FK_USERID] int  NOT NULL,
    [FK_ROLEID] int  NOT NULL
);
GO

-- Creating table 'SYS_USERINFO'
CREATE TABLE [dbo].[SYS_USERINFO] (
    [ID] int  NOT NULL,
    [USERID] int  NOT NULL,
    [POSTCODE] int  NULL,
    [PHONE] nvarchar(200)  NULL,
    [OFFICEPHONE] nvarchar(200)  NULL,
    [EMAILADDRESS] nvarchar(200)  NULL,
    [SECONDPHONE] nvarchar(200)  NULL,
    [WORKCODE] int  NULL,
    [SEXCODE] int  NULL,
    [BIRTHDAY] datetime  NULL,
    [NATIONCODE] int  NULL,
    [IDNUMBER] nvarchar(18)  NULL,
    [MARRYCODE] int  NULL,
    [IDENTITYCODE] int  NULL,
    [HomeTown] nvarchar(200)  NULL,
    [ACCOUNTLOCATION] nvarchar(200)  NULL,
    [XUELI] int  NULL,
    [ZHICHENG] int  NULL,
    [GRADUATIONSCHOOL] nvarchar(200)  NULL,
    [SPECIALTY] nvarchar(200)  NULL,
    [PHOTOOLDNAME] nvarchar(200)  NULL,
    [PHOTONEWNAME] nvarchar(200)  NULL,
    [PHOTOTYPE] nvarchar(200)  NULL,
    [RESUMEOLDNAME] nvarchar(200)  NULL,
    [RESUMENEWNAME] nvarchar(200)  NULL,
    [RESUMETYPE] nvarchar(200)  NULL,
    [HuJiSuoZaiDi] nvarchar(200)  NULL,
    [HUJIPAICHUSUO] nvarchar(200)  NULL,
    [WORKDATE] datetime  NULL,
    [JINRUDATE] datetime  NULL,
    [CARNUMBER] nvarchar(200)  NULL,
    [QQ] nvarchar(15)  NULL,
    [WEBCHATOPENID] nvarchar(200)  NULL,
    [CREATEDATE] datetime  NULL,
    [CREATEUSER] nvarchar(36)  NULL,
    [UPDATEDATE] datetime  NULL,
    [UPDATEUSER] nvarchar(36)  NULL
);
GO

-- Creating table 'COM_CONTENT'
CREATE TABLE [dbo].[COM_CONTENT] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [FK_RELATIONID] nvarchar(72)  NOT NULL,
    [CONTENT] nvarchar(max)  NULL,
    [CONTENTBLOB] varbinary(max)  NULL,
    [FK_TABLE] nvarchar(200)  NOT NULL,
    [CREATEDATE] datetime  NOT NULL
);
GO

-- Creating table 'COM_DAILYS'
CREATE TABLE [dbo].[COM_DAILYS] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [FK_USERID] int  NOT NULL,
    [FK_RELATIONID] nvarchar(72)  NULL,
    [AddDate] datetime  NOT NULL,
    [LastEditDate] datetime  NOT NULL,
    [DailySubIP] nvarchar(50)  NOT NULL
);
GO

-- Creating table 'COM_UPLOAD'
CREATE TABLE [dbo].[COM_UPLOAD] (
    [ID] nvarchar(36)  NOT NULL,
    [FK_USERID] nvarchar(72)  NULL,
    [UPOPEATOR] nvarchar(100)  NULL,
    [UPTIME] datetime  NULL,
    [UPOLDNAME] nvarchar(400)  NULL,
    [UPNEWNAME] nvarchar(400)  NULL,
    [UPFILESIZE] decimal(18,2)  NULL,
    [UPFILEUNIT] nvarchar(20)  NULL,
    [UPFILEPATH] nvarchar(2000)  NULL,
    [UPFILESUFFIX] nvarchar(40)  NULL,
    [UPFILETHUMBNAIL] nvarchar(2000)  NULL,
    [UPFILETHUMBNAILFORPAD] nvarchar(2000)  NULL,
    [UPFILETHUMBNAILFORPHONE] nvarchar(2000)  NULL,
    [UPFILEIP] nvarchar(40)  NULL,
    [UPFILEURL] nvarchar(1000)  NULL
);
GO

-- Creating table 'COM_WORKATTENDANCE'
CREATE TABLE [dbo].[COM_WORKATTENDANCE] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [FK_UserId] int  NOT NULL,
    [Is_SiginAM] bit  NOT NULL,
    [Is_SigOutAM] bit  NOT NULL,
    [SiginAMDate] datetime  NOT NULL,
    [SigOutAMDate] datetime  NOT NULL,
    [Is_SiginPM] bit  NOT NULL,
    [Is_SigOutPM] bit  NOT NULL,
    [SiginPM] datetime  NOT NULL,
    [SigOutPM] datetime  NOT NULL,
    [IsLateAM] bit  NOT NULL,
    [LateAMMinutes] int  NOT NULL,
    [IsEarlyOutAM] bit  NOT NULL,
    [EarlyOutAMMinutes] int  NOT NULL,
    [IsLatePM] bit  NOT NULL,
    [LatePMMinutes] int  NOT NULL,
    [IsEarlyOutPM] bit  NOT NULL,
    [EarlyOutPMMinutes] int  NOT NULL,
    [WorkHours] float  NOT NULL,
    [SigIP] nvarchar(50)  NOT NULL,
    [CreateDate] datetime  NOT NULL
);
GO

-- Creating table 'MAIL_ATTACHMENT'
CREATE TABLE [dbo].[MAIL_ATTACHMENT] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [FK_MailID] int  NOT NULL,
    [AttName] nvarchar(50)  NOT NULL,
    [AttNewName] nvarchar(50)  NOT NULL,
    [AttPath] nvarchar(500)  NOT NULL,
    [AttExt] nvarchar(10)  NOT NULL,
    [AttSize] nvarchar(50)  NOT NULL,
    [CreateIP] nvarchar(50)  NOT NULL,
    [UploadDate] datetime  NOT NULL
);
GO

-- Creating table 'MAIL_INBOX'
CREATE TABLE [dbo].[MAIL_INBOX] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [FK_MailID] int  NOT NULL,
    [Recipient] nvarchar(50)  NOT NULL,
    [MailType] int  NOT NULL,
    [ReadStatus] int  NOT NULL,
    [ReceivingTime] datetime  NOT NULL
);
GO

-- Creating table 'MAIL_OUTBOX'
CREATE TABLE [dbo].[MAIL_OUTBOX] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [FK_RELATIONID] nvarchar(72)  NOT NULL,
    [FK_UserId] nvarchar(50)  NOT NULL,
    [ToUser] nvarchar(500)  NOT NULL,
    [MailTitle] nvarchar(100)  NOT NULL,
    [SendStatus] int  NOT NULL,
    [MailType] int  NOT NULL,
    [SendDate] datetime  NOT NULL,
    [SaveDate] datetime  NOT NULL
);
GO

-- Creating table 'PRO_PROJECT_FILES'
CREATE TABLE [dbo].[PRO_PROJECT_FILES] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [DocStyle] nvarchar(50)  NOT NULL,
    [Fk_ForeignId] int  NOT NULL,
    [FK_UserId] int  NOT NULL,
    [DocName] nvarchar(50)  NOT NULL,
    [DocNewName] nvarchar(50)  NOT NULL,
    [DocPath] nvarchar(500)  NOT NULL,
    [DocFileExt] nvarchar(10)  NOT NULL,
    [DocSize] nvarchar(50)  NOT NULL,
    [UploadDate] datetime  NOT NULL,
    [CreateUser] nvarchar(50)  NOT NULL,
    [CreateIP] nvarchar(50)  NOT NULL,
    [AcceptanceStatus] int  NULL
);
GO

-- Creating table 'PRO_PROJECT_MESSAGE'
CREATE TABLE [dbo].[PRO_PROJECT_MESSAGE] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [FK_ProjectId] int  NOT NULL,
    [MessageContent] nvarchar(500)  NOT NULL,
    [UserName] nvarchar(50)  NOT NULL,
    [UserFace] nvarchar(max)  NULL,
    [CreateDate] datetime  NOT NULL
);
GO

-- Creating table 'PRO_PROJECT_STAGES'
CREATE TABLE [dbo].[PRO_PROJECT_STAGES] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [FK_ProjectId] int  NOT NULL,
    [StageTitle] nvarchar(50)  NOT NULL,
    [NeedNumber] int  NOT NULL,
    [StageTimeLimit] int  NOT NULL,
    [StartDate] datetime  NOT NULL,
    [EndDate] datetime  NOT NULL,
    [StageStatus] int  NOT NULL,
    [IsOverTime] bit  NOT NULL,
    [OverDays] int  NOT NULL,
    [OrderNumber] int  NOT NULL,
    [CreateUser] nvarchar(50)  NOT NULL,
    [CreateDate] datetime  NOT NULL,
    [UpdateUser] nvarchar(50)  NOT NULL,
    [UpdateDate] datetime  NOT NULL
);
GO

-- Creating table 'PRO_PROJECT_TEAMS'
CREATE TABLE [dbo].[PRO_PROJECT_TEAMS] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [FK_StageId] int  NOT NULL,
    [FK_UserId] int  NOT NULL,
    [ApplyReasons] nvarchar(300)  NULL,
    [JionStatus] int  NOT NULL,
    [RefuseReasons] nvarchar(300)  NULL,
    [JionDate] datetime  NOT NULL
);
GO

-- Creating table 'PRO_PROJECTS'
CREATE TABLE [dbo].[PRO_PROJECTS] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [ProjectTitle] nvarchar(50)  NOT NULL,
    [FK_RELATIONID] nvarchar(72)  NULL,
    [Fk_DepartId] nvarchar(36)  NULL,
    [Fk_UserId] int  NOT NULL,
    [ProjectStatus] int  NOT NULL,
    [Fk_BussinessCustomer] int  NOT NULL,
    [ProjectLevels] int  NOT NULL,
    [ProjectMoney] decimal(18,2)  NOT NULL,
    [ProjectTimeLimit] int  NOT NULL,
    [StartDate] datetime  NOT NULL,
    [EndDate] datetime  NOT NULL,
    [CreateDate] datetime  NOT NULL,
    [UpdateDate] datetime  NOT NULL,
    [ContractCode] nvarchar(50)  NOT NULL,
    [ContractFile] nvarchar(500)  NOT NULL,
    [SignPersion] nvarchar(50)  NOT NULL,
    [SignDate] datetime  NOT NULL
);
GO

-- Creating table 'SYS_BUSSINESSCUSTOMER'
CREATE TABLE [dbo].[SYS_BUSSINESSCUSTOMER] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [Fk_DepartId] nvarchar(36)  NULL,
    [FK_RELATIONID] nvarchar(72)  NULL,
    [CompanyName] nvarchar(50)  NOT NULL,
    [CompanyProvince] nvarchar(10)  NOT NULL,
    [CompanyCity] nvarchar(10)  NOT NULL,
    [CompanyArea] nvarchar(10)  NOT NULL,
    [CompanyAddress] nvarchar(500)  NULL,
    [CompanyTel] nvarchar(50)  NULL,
    [CompanyWebSite] nvarchar(100)  NULL,
    [ChargePersionName] nvarchar(50)  NULL,
    [ChargePersionSex] int  NOT NULL,
    [ChargePersionQQ] nvarchar(20)  NULL,
    [ChargePersionEmail] nvarchar(50)  NULL,
    [ChargePersionPhone] nvarchar(50)  NULL,
    [IsValidate] bit  NOT NULL,
    [CreateUser] nvarchar(50)  NOT NULL,
    [CreateDate] datetime  NOT NULL,
    [UpdateUser] nvarchar(50)  NOT NULL,
    [UpdateDate] datetime  NOT NULL,
    [CustomerStyle] int  NOT NULL
);
GO

-- Creating table 'SYS_CHATMESSAGE'
CREATE TABLE [dbo].[SYS_CHATMESSAGE] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [FromUser] int  NOT NULL,
    [MessageType] int  NOT NULL,
    [MessageContent] nvarchar(max)  NOT NULL,
    [ToGroup] nvarchar(50)  NULL,
    [MessageDate] datetime  NOT NULL,
    [MessageIP] nvarchar(50)  NOT NULL
);
GO

-- Creating table 'SYS_USER_ONLINE'
CREATE TABLE [dbo].[SYS_USER_ONLINE] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [ConnectId] nvarchar(500)  NULL,
    [FK_UserId] int  NOT NULL,
    [OnlineDate] datetime  NOT NULL,
    [OfflineDate] datetime  NULL,
    [IsOnline] bit  NOT NULL,
    [UserIP] nvarchar(50)  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [ID] in table 'SYS_USER'
ALTER TABLE [dbo].[SYS_USER]
ADD CONSTRAINT [PK_SYS_USER]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'SYS_CODE'
ALTER TABLE [dbo].[SYS_CODE]
ADD CONSTRAINT [PK_SYS_CODE]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'SYS_CODE_AREA'
ALTER TABLE [dbo].[SYS_CODE_AREA]
ADD CONSTRAINT [PK_SYS_CODE_AREA]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'SYS_DEPARTMENT'
ALTER TABLE [dbo].[SYS_DEPARTMENT]
ADD CONSTRAINT [PK_SYS_DEPARTMENT]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'SYS_LOG'
ALTER TABLE [dbo].[SYS_LOG]
ADD CONSTRAINT [PK_SYS_LOG]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'SYS_MODULE'
ALTER TABLE [dbo].[SYS_MODULE]
ADD CONSTRAINT [PK_SYS_MODULE]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'SYS_PERMISSION'
ALTER TABLE [dbo].[SYS_PERMISSION]
ADD CONSTRAINT [PK_SYS_PERMISSION]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'SYS_POST'
ALTER TABLE [dbo].[SYS_POST]
ADD CONSTRAINT [PK_SYS_POST]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'SYS_POST_DEPARTMENT'
ALTER TABLE [dbo].[SYS_POST_DEPARTMENT]
ADD CONSTRAINT [PK_SYS_POST_DEPARTMENT]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'SYS_POST_USER'
ALTER TABLE [dbo].[SYS_POST_USER]
ADD CONSTRAINT [PK_SYS_POST_USER]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'SYS_ROLE'
ALTER TABLE [dbo].[SYS_ROLE]
ADD CONSTRAINT [PK_SYS_ROLE]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'SYS_ROLE_PERMISSION'
ALTER TABLE [dbo].[SYS_ROLE_PERMISSION]
ADD CONSTRAINT [PK_SYS_ROLE_PERMISSION]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'SYS_SYSTEM'
ALTER TABLE [dbo].[SYS_SYSTEM]
ADD CONSTRAINT [PK_SYS_SYSTEM]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'SYS_USER_DEPARTMENT'
ALTER TABLE [dbo].[SYS_USER_DEPARTMENT]
ADD CONSTRAINT [PK_SYS_USER_DEPARTMENT]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'SYS_USER_PERMISSION'
ALTER TABLE [dbo].[SYS_USER_PERMISSION]
ADD CONSTRAINT [PK_SYS_USER_PERMISSION]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'SYS_USER_ROLE'
ALTER TABLE [dbo].[SYS_USER_ROLE]
ADD CONSTRAINT [PK_SYS_USER_ROLE]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'SYS_USERINFO'
ALTER TABLE [dbo].[SYS_USERINFO]
ADD CONSTRAINT [PK_SYS_USERINFO]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'COM_CONTENT'
ALTER TABLE [dbo].[COM_CONTENT]
ADD CONSTRAINT [PK_COM_CONTENT]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'COM_DAILYS'
ALTER TABLE [dbo].[COM_DAILYS]
ADD CONSTRAINT [PK_COM_DAILYS]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'COM_UPLOAD'
ALTER TABLE [dbo].[COM_UPLOAD]
ADD CONSTRAINT [PK_COM_UPLOAD]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'COM_WORKATTENDANCE'
ALTER TABLE [dbo].[COM_WORKATTENDANCE]
ADD CONSTRAINT [PK_COM_WORKATTENDANCE]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'MAIL_ATTACHMENT'
ALTER TABLE [dbo].[MAIL_ATTACHMENT]
ADD CONSTRAINT [PK_MAIL_ATTACHMENT]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'MAIL_INBOX'
ALTER TABLE [dbo].[MAIL_INBOX]
ADD CONSTRAINT [PK_MAIL_INBOX]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'MAIL_OUTBOX'
ALTER TABLE [dbo].[MAIL_OUTBOX]
ADD CONSTRAINT [PK_MAIL_OUTBOX]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'PRO_PROJECT_FILES'
ALTER TABLE [dbo].[PRO_PROJECT_FILES]
ADD CONSTRAINT [PK_PRO_PROJECT_FILES]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'PRO_PROJECT_MESSAGE'
ALTER TABLE [dbo].[PRO_PROJECT_MESSAGE]
ADD CONSTRAINT [PK_PRO_PROJECT_MESSAGE]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'PRO_PROJECT_STAGES'
ALTER TABLE [dbo].[PRO_PROJECT_STAGES]
ADD CONSTRAINT [PK_PRO_PROJECT_STAGES]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'PRO_PROJECT_TEAMS'
ALTER TABLE [dbo].[PRO_PROJECT_TEAMS]
ADD CONSTRAINT [PK_PRO_PROJECT_TEAMS]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'PRO_PROJECTS'
ALTER TABLE [dbo].[PRO_PROJECTS]
ADD CONSTRAINT [PK_PRO_PROJECTS]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'SYS_BUSSINESSCUSTOMER'
ALTER TABLE [dbo].[SYS_BUSSINESSCUSTOMER]
ADD CONSTRAINT [PK_SYS_BUSSINESSCUSTOMER]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'SYS_CHATMESSAGE'
ALTER TABLE [dbo].[SYS_CHATMESSAGE]
ADD CONSTRAINT [PK_SYS_CHATMESSAGE]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'SYS_USER_ONLINE'
ALTER TABLE [dbo].[SYS_USER_ONLINE]
ADD CONSTRAINT [PK_SYS_USER_ONLINE]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [FK_DEPARTMENT_ID] in table 'SYS_POST_DEPARTMENT'
ALTER TABLE [dbo].[SYS_POST_DEPARTMENT]
ADD CONSTRAINT [FK_SYS_POST_DEPARTMENT_DPTID]
    FOREIGN KEY ([FK_DEPARTMENT_ID])
    REFERENCES [dbo].[SYS_DEPARTMENT]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_SYS_POST_DEPARTMENT_DPTID'
CREATE INDEX [IX_FK_SYS_POST_DEPARTMENT_DPTID]
ON [dbo].[SYS_POST_DEPARTMENT]
    ([FK_DEPARTMENT_ID]);
GO

-- Creating foreign key on [DEPARTMENT_ID] in table 'SYS_USER_DEPARTMENT'
ALTER TABLE [dbo].[SYS_USER_DEPARTMENT]
ADD CONSTRAINT [FK_SYS_USER_DEPARTMENT_DPTID]
    FOREIGN KEY ([DEPARTMENT_ID])
    REFERENCES [dbo].[SYS_DEPARTMENT]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_SYS_USER_DEPARTMENT_DPTID'
CREATE INDEX [IX_FK_SYS_USER_DEPARTMENT_DPTID]
ON [dbo].[SYS_USER_DEPARTMENT]
    ([DEPARTMENT_ID]);
GO

-- Creating foreign key on [FK_BELONGSYSTEM] in table 'SYS_MODULE'
ALTER TABLE [dbo].[SYS_MODULE]
ADD CONSTRAINT [FK_SYS_MODULE_SYSTEMID]
    FOREIGN KEY ([FK_BELONGSYSTEM])
    REFERENCES [dbo].[SYS_SYSTEM]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_SYS_MODULE_SYSTEMID'
CREATE INDEX [IX_FK_SYS_MODULE_SYSTEMID]
ON [dbo].[SYS_MODULE]
    ([FK_BELONGSYSTEM]);
GO

-- Creating foreign key on [MODULEID] in table 'SYS_PERMISSION'
ALTER TABLE [dbo].[SYS_PERMISSION]
ADD CONSTRAINT [FK_SYS_PERMISSION_MODULEID]
    FOREIGN KEY ([MODULEID])
    REFERENCES [dbo].[SYS_MODULE]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_SYS_PERMISSION_MODULEID'
CREATE INDEX [IX_FK_SYS_PERMISSION_MODULEID]
ON [dbo].[SYS_PERMISSION]
    ([MODULEID]);
GO

-- Creating foreign key on [PERMISSIONID] in table 'SYS_ROLE_PERMISSION'
ALTER TABLE [dbo].[SYS_ROLE_PERMISSION]
ADD CONSTRAINT [FK_SYS_ROLE_PERMISSION_PERID]
    FOREIGN KEY ([PERMISSIONID])
    REFERENCES [dbo].[SYS_PERMISSION]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_SYS_ROLE_PERMISSION_PERID'
CREATE INDEX [IX_FK_SYS_ROLE_PERMISSION_PERID]
ON [dbo].[SYS_ROLE_PERMISSION]
    ([PERMISSIONID]);
GO

-- Creating foreign key on [FK_PERMISSIONID] in table 'SYS_USER_PERMISSION'
ALTER TABLE [dbo].[SYS_USER_PERMISSION]
ADD CONSTRAINT [FK_SYS_USER_PERMISSION_PERID]
    FOREIGN KEY ([FK_PERMISSIONID])
    REFERENCES [dbo].[SYS_PERMISSION]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_SYS_USER_PERMISSION_PERID'
CREATE INDEX [IX_FK_SYS_USER_PERMISSION_PERID]
ON [dbo].[SYS_USER_PERMISSION]
    ([FK_PERMISSIONID]);
GO

-- Creating foreign key on [FK_POST_ID] in table 'SYS_POST_DEPARTMENT'
ALTER TABLE [dbo].[SYS_POST_DEPARTMENT]
ADD CONSTRAINT [FK_SYS_POST_DEPARTMENT_POSTID]
    FOREIGN KEY ([FK_POST_ID])
    REFERENCES [dbo].[SYS_POST]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_SYS_POST_DEPARTMENT_POSTID'
CREATE INDEX [IX_FK_SYS_POST_DEPARTMENT_POSTID]
ON [dbo].[SYS_POST_DEPARTMENT]
    ([FK_POST_ID]);
GO

-- Creating foreign key on [FK_POST_DEPARTMENTID] in table 'SYS_POST_USER'
ALTER TABLE [dbo].[SYS_POST_USER]
ADD CONSTRAINT [FK_SYS_POST_USER_POSTDPTID]
    FOREIGN KEY ([FK_POST_DEPARTMENTID])
    REFERENCES [dbo].[SYS_POST_DEPARTMENT]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_SYS_POST_USER_POSTDPTID'
CREATE INDEX [IX_FK_SYS_POST_USER_POSTDPTID]
ON [dbo].[SYS_POST_USER]
    ([FK_POST_DEPARTMENTID]);
GO

-- Creating foreign key on [FK_USERID] in table 'SYS_POST_USER'
ALTER TABLE [dbo].[SYS_POST_USER]
ADD CONSTRAINT [FK_SYS_POST_USER_USERID]
    FOREIGN KEY ([FK_USERID])
    REFERENCES [dbo].[SYS_USER]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_SYS_POST_USER_USERID'
CREATE INDEX [IX_FK_SYS_POST_USER_USERID]
ON [dbo].[SYS_POST_USER]
    ([FK_USERID]);
GO

-- Creating foreign key on [ROLEID] in table 'SYS_ROLE_PERMISSION'
ALTER TABLE [dbo].[SYS_ROLE_PERMISSION]
ADD CONSTRAINT [FK_SYS_ROLE_PERMISSION_ROLEID]
    FOREIGN KEY ([ROLEID])
    REFERENCES [dbo].[SYS_ROLE]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_SYS_ROLE_PERMISSION_ROLEID'
CREATE INDEX [IX_FK_SYS_ROLE_PERMISSION_ROLEID]
ON [dbo].[SYS_ROLE_PERMISSION]
    ([ROLEID]);
GO

-- Creating foreign key on [FK_ROLEID] in table 'SYS_USER_ROLE'
ALTER TABLE [dbo].[SYS_USER_ROLE]
ADD CONSTRAINT [FK_SYS_USER_ROLE_ROLEID]
    FOREIGN KEY ([FK_ROLEID])
    REFERENCES [dbo].[SYS_ROLE]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_SYS_USER_ROLE_ROLEID'
CREATE INDEX [IX_FK_SYS_USER_ROLE_ROLEID]
ON [dbo].[SYS_USER_ROLE]
    ([FK_ROLEID]);
GO

-- Creating foreign key on [USER_ID] in table 'SYS_USER_DEPARTMENT'
ALTER TABLE [dbo].[SYS_USER_DEPARTMENT]
ADD CONSTRAINT [FK_SYS_USER_DEPARTMENT_USERID]
    FOREIGN KEY ([USER_ID])
    REFERENCES [dbo].[SYS_USER]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_SYS_USER_DEPARTMENT_USERID'
CREATE INDEX [IX_FK_SYS_USER_DEPARTMENT_USERID]
ON [dbo].[SYS_USER_DEPARTMENT]
    ([USER_ID]);
GO

-- Creating foreign key on [FK_USERID] in table 'SYS_USER_PERMISSION'
ALTER TABLE [dbo].[SYS_USER_PERMISSION]
ADD CONSTRAINT [FK_SYS_USER_PERMISSION_USERID]
    FOREIGN KEY ([FK_USERID])
    REFERENCES [dbo].[SYS_USER]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_SYS_USER_PERMISSION_USERID'
CREATE INDEX [IX_FK_SYS_USER_PERMISSION_USERID]
ON [dbo].[SYS_USER_PERMISSION]
    ([FK_USERID]);
GO

-- Creating foreign key on [FK_USERID] in table 'SYS_USER_ROLE'
ALTER TABLE [dbo].[SYS_USER_ROLE]
ADD CONSTRAINT [FK_SYS_USER_ROLE_USERID]
    FOREIGN KEY ([FK_USERID])
    REFERENCES [dbo].[SYS_USER]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_SYS_USER_ROLE_USERID'
CREATE INDEX [IX_FK_SYS_USER_ROLE_USERID]
ON [dbo].[SYS_USER_ROLE]
    ([FK_USERID]);
GO

-- Creating foreign key on [USERID] in table 'SYS_USERINFO'
ALTER TABLE [dbo].[SYS_USERINFO]
ADD CONSTRAINT [FK_SYS_USERINFO_SYSUSERID]
    FOREIGN KEY ([USERID])
    REFERENCES [dbo].[SYS_USER]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_SYS_USERINFO_SYSUSERID'
CREATE INDEX [IX_FK_SYS_USERINFO_SYSUSERID]
ON [dbo].[SYS_USERINFO]
    ([USERID]);
GO

-- Creating foreign key on [FK_MailID] in table 'MAIL_ATTACHMENT'
ALTER TABLE [dbo].[MAIL_ATTACHMENT]
ADD CONSTRAINT [FK_MAIL_ATTACHMENT_MAIL_OUTBOX]
    FOREIGN KEY ([FK_MailID])
    REFERENCES [dbo].[MAIL_OUTBOX]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_MAIL_ATTACHMENT_MAIL_OUTBOX'
CREATE INDEX [IX_FK_MAIL_ATTACHMENT_MAIL_OUTBOX]
ON [dbo].[MAIL_ATTACHMENT]
    ([FK_MailID]);
GO

-- Creating foreign key on [FK_MailID] in table 'MAIL_INBOX'
ALTER TABLE [dbo].[MAIL_INBOX]
ADD CONSTRAINT [FK_MAIL_INBOX_MAIL_OUTBOX]
    FOREIGN KEY ([FK_MailID])
    REFERENCES [dbo].[MAIL_OUTBOX]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_MAIL_INBOX_MAIL_OUTBOX'
CREATE INDEX [IX_FK_MAIL_INBOX_MAIL_OUTBOX]
ON [dbo].[MAIL_INBOX]
    ([FK_MailID]);
GO

-- Creating foreign key on [FK_ProjectId] in table 'PRO_PROJECT_MESSAGE'
ALTER TABLE [dbo].[PRO_PROJECT_MESSAGE]
ADD CONSTRAINT [FK_PRO_PROJECT_MESSAGE_PRO_PROJECTS]
    FOREIGN KEY ([FK_ProjectId])
    REFERENCES [dbo].[PRO_PROJECTS]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_PRO_PROJECT_MESSAGE_PRO_PROJECTS'
CREATE INDEX [IX_FK_PRO_PROJECT_MESSAGE_PRO_PROJECTS]
ON [dbo].[PRO_PROJECT_MESSAGE]
    ([FK_ProjectId]);
GO

-- Creating foreign key on [FK_ProjectId] in table 'PRO_PROJECT_STAGES'
ALTER TABLE [dbo].[PRO_PROJECT_STAGES]
ADD CONSTRAINT [FK_PRO_PROJECT_STAGES_PRO_PROJECTS]
    FOREIGN KEY ([FK_ProjectId])
    REFERENCES [dbo].[PRO_PROJECTS]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_PRO_PROJECT_STAGES_PRO_PROJECTS'
CREATE INDEX [IX_FK_PRO_PROJECT_STAGES_PRO_PROJECTS]
ON [dbo].[PRO_PROJECT_STAGES]
    ([FK_ProjectId]);
GO

-- Creating foreign key on [FK_StageId] in table 'PRO_PROJECT_TEAMS'
ALTER TABLE [dbo].[PRO_PROJECT_TEAMS]
ADD CONSTRAINT [FK_PRO_PROJECT_TEAMS_PRO_PROJECT_STAGES]
    FOREIGN KEY ([FK_StageId])
    REFERENCES [dbo].[PRO_PROJECT_STAGES]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_PRO_PROJECT_TEAMS_PRO_PROJECT_STAGES'
CREATE INDEX [IX_FK_PRO_PROJECT_TEAMS_PRO_PROJECT_STAGES]
ON [dbo].[PRO_PROJECT_TEAMS]
    ([FK_StageId]);
GO

-- Creating foreign key on [FK_DEPARTID] in table 'SYS_POST'
ALTER TABLE [dbo].[SYS_POST]
ADD CONSTRAINT [FK_SYS_POST_SYS_DEPARTMENT]
    FOREIGN KEY ([FK_DEPARTID])
    REFERENCES [dbo].[SYS_DEPARTMENT]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_SYS_POST_SYS_DEPARTMENT'
CREATE INDEX [IX_FK_SYS_POST_SYS_DEPARTMENT]
ON [dbo].[SYS_POST]
    ([FK_DEPARTID]);
GO

-- Creating foreign key on [FK_UserId] in table 'SYS_USER_ONLINE'
ALTER TABLE [dbo].[SYS_USER_ONLINE]
ADD CONSTRAINT [FK_SYS_USER_ONLINE_SYS_USER_ONLINE]
    FOREIGN KEY ([FK_UserId])
    REFERENCES [dbo].[SYS_USER]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_SYS_USER_ONLINE_SYS_USER_ONLINE'
CREATE INDEX [IX_FK_SYS_USER_ONLINE_SYS_USER_ONLINE]
ON [dbo].[SYS_USER_ONLINE]
    ([FK_UserId]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------