USE master

IF EXISTS(SELECT name from sys.databases where name=N'DarkServerUsers')
	DROP DATABASE DarkServerUsers
GO
CREATE DATABASE DarkServerUsers
GO

USE DarkServerUsers

/* --------DROP AND CREATE TABLES-------- */

/* UserAccount */
IF OBJECT_ID('dbo.UserAccount','U') IS NOT NULL
	DROP TABLE UserAccount
GO
CREATE TABLE UserAccount
(
	UserAccountID		INT IDENTITY PRIMARY KEY NOT NULL,
	Username			VARCHAR(20) NOT NULL,
	IsEnabled			BIT NULL,
	DateCreated			DATETIME NULL,
	UserAccountTypeID	INT NOT NULL,
)
GO

/* UserPassword */
IF OBJECT_ID('dbo.UserPassword','U') IS NOT NULL
	DROP TABLE UserPassword
GO
CREATE TABLE UserPassword
(
	UserPasswordID	INT IDENTITY PRIMARY KEY NOT NULL,
	UserAccountID	INT NOT NULL,
	PasswordHash	VARBINARY(128) NOT NULL,
	PasswordSalt	VARBINARY(64) NOT NULL
)
GO

/* ContactInfo */
IF OBJECT_ID('dbo.ContactInfo','U') IS NOT NULL
	DROP TABLE ContactInfo
GO
CREATE TABLE ContactInfo
(
	ContactInfoID	INT IDENTITY PRIMARY KEY NOT NULL,
	UserAccountID	INT NOT NULL,
	Nickname		VARCHAR(20) NOT NULL,
	Email			VARCHAR(50) NOT NULL,
	BirthDate		DATETIME NULL,
	EmailVerified	BIT NULL
)
GO

/* UserAccountType */
IF OBJECT_ID('dbo.UserAccountType','U') IS NOT NULL
	DROP TABLE UserAccountType
GO
CREATE TABLE UserAccountType
(
	UserAccountTypeID	INT IDENTITY PRIMARY KEY NOT NULL,
	TypeName			VARCHAR(20) NOT NULL,
	TypeDesc			VARCHAR(255) NULL
)
GO

/* SecurityQuestion */
IF OBJECT_ID('dbo.SecurityQuestion','U') IS NOT NULL
	DROP TABLE SecurityQuestion
GO
CREATE TABLE SecurityQuestion
(
	SecurityQuestionID		INT IDENTITY PRIMARY KEY NOT NULL,
	UserAccountID			INT NOT NULL,
	SecurityQuestionTypeID	INT NOT NULL,
)
GO

/* SecurityQuestionAnswer */
IF OBJECT_ID('dbo.SecurityQuestionAnswer','U') IS NOT NULL
	DROP TABLE SecurityQuestionAnswer
GO
CREATE TABLE SecurityQuestionAnswer
(
	SecurityQuestionAnswerID	INT IDENTITY PRIMARY KEY NOT NULL,
	SecurityQuestionID			INT NOT NULL,
	AnswerHash					VARBINARY(128),
	AnswerSalt					VARBINARY(64)
)
GO

/* SecurityQuestionType */
IF OBJECT_ID('dbo.SecurityQuestionType','U') IS NOT NULL
	DROP TABLE SecurityQuestionType
GO
CREATE TABLE SecurityQuestionType
(
	SecurityQuestionTypeID	INT IDENTITY PRIMARY KEY NOT NULL,
	QuestionText			VARCHAR(100) NOT NULL
)
GO

/* LoginSession */
IF OBJECT_ID('dbo.LoginSession','U') IS NOT NULL
	DROP TABLE LoginSession
GO
CREATE TABLE LoginSession
(
	LoginSessionID	INT IDENTITY PRIMARY KEY NOT NULL,
	UserAccountID	INT NOT NULL,
	LoginDeviceID	INT NOT NULL,
	LoginAppID		INT NOT NULL,
	SessionBegin	DATETIME NULL,
	SessionEnd		DATETIME NULL,
)
GO

/* ActiveLoginSession */
IF OBJECT_ID('dbo.ActiveLoginSession','U') IS NOT NULL
	DROP TABLE ActiveLoginSession
GO
CREATE TABLE ActiveLoginSession
(
	ActiveLoginSessionID	INT IDENTITY PRIMARY KEY NOT NULL,
	UserAccountID			INT NOT NULL,
	LoginDeviceID			INT NOT NULL,
	LoginAppID				INT NOT NULL,
	SessionBegin			DATETIME NULL
)
GO

/* LoginDevice */
IF OBJECT_ID('dbo.LoginDevice','U') IS NOT NULL
	DROP TABLE LoginDevice
GO
CREATE TABLE LoginDevice
(
	LoginDeviceID	INT IDENTITY PRIMARY KEY NOT NULL,
	DeviceName		VARCHAR(20) NOT NULL,
	DeviceDesc		VARCHAR(255) NULL
)
GO

/* LoginApp */
IF OBJECT_ID('dbo.LoginApp','U') IS NOT NULL
	DROP TABLE LoginApp
GO
CREATE TABLE LoginApp
(
	LoginAppID	INT IDENTITY PRIMARY KEY NOT NULL,
	AppName		VARCHAR(20) NOT NULL,
	AppDesc		VARCHAR(255) NULL
)
GO

/* --------CREATE FOREIGN KEY CONSTRAINTS-------- */

/* UserAccount */
ALTER TABLE UserAccount
	ADD CONSTRAINT UsrAccTypId_fk FOREIGN KEY (UserAccountTypeID) REFERENCES UserAccountType(UserAccountTypeID)
GO

/* UserPassword */
ALTER TABLE UserPassword
	ADD CONSTRAINT UsrPassUsrId_fk FOREIGN KEY (UserAccountID) REFERENCES UserAccount(UserAccountID)
GO

/* ContactInfo */
ALTER TABLE ContactInfo
	ADD CONSTRAINT ContUsrId_fk FOREIGN KEY (UserAccountID) REFERENCES UserAccount(UserAccountID)
GO

/* SecurityQuestion */
ALTER TABLE SecurityQuestion
	ADD CONSTRAINT SecUsrId_fk FOREIGN KEY (UserAccountID) REFERENCES UserAccount(UserAccountID),
	CONSTRAINT SecQuesId_fk FOREIGN KEY (SecurityQuestionTypeID) REFERENCES SecurityQuestionType(SecurityQuestionTypeID)
GO

/* SecurityQuestionAnswer */
ALTER TABLE SecurityQuestionAnswer
	ADD CONSTRAINT SecAnsQuesId_fk FOREIGN KEY (SecurityQuestionID) REFERENCES SecurityQuestion(SecurityQuestionID)
GO

/* LoginSession */
ALTER TABLE LoginSession
	ADD CONSTRAINT LogUsrId_fk FOREIGN KEY (UserAccountID) REFERENCES UserAccount(UserAccountID),
	CONSTRAINT LogDevId_fk FOREIGN KEY (LoginDeviceID) REFERENCES LoginDevice(LoginDeviceID),
	CONSTRAINT LogAppId_fk FOREIGN KEY (LoginAppID) REFERENCES LoginApp(LoginAppID)
GO

/* ActiveLoginSession */
ALTER TABLE ActiveLoginSession
	ADD CONSTRAINT ActLogUsrId_fk FOREIGN KEY (UserAccountID) REFERENCES UserAccount(UserAccountID),
	CONSTRAINT ActLogDevId_fk FOREIGN KEY (LoginDeviceID) REFERENCES LoginDevice(LoginDeviceID),
	CONSTRAINT ActLogAppId_fk FOREIGN KEY (LoginAppID) REFERENCES LoginApp(LoginAppID)
GO

/* --------INSERT DEFAULT TEST TYPES, DEVICES, AND APPS-------- */
INSERT INTO UserAccountType (TypeName, TypeDesc)
	VALUES ('Test', 'Account type used for testing')
GO

INSERT INTO SecurityQuestionType (QuestionText)
	VALUES ('What is your favorite city or town?')
GO

INSERT INTO LoginDevice (DeviceName,DeviceDesc)
	VALUES ('Test', 'Device used for testing')
GO

INSERT INTO LoginApp (AppName, AppDesc)
	VALUES ('Test', 'App used for testing')
GO

-- Create DarkClient user
CREATE USER DarkClient FOR LOGIN DarkClient
GO

ALTER ROLE [db_owner] ADD MEMBER [DarkClient]
GO
