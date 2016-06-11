use DarkServerUsers

IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.ROUTINES WHERE SPECIFIC_NAME=N'CreateUserAccount')
	DROP PROCEDURE CreateUserAccount
GO

CREATE PROCEDURE CreateUserAccount
	@USERNAME		varchar(20),
	@ENABLED		bit,
	@DATECREATED	datetime,
	@PASSHASH		varbinary(128),
	@PASSSALT		varbinary(64),
	@ACCTYPE		varchar(20),
	@ANSHASH		varbinary(128),
	@ANSSALT		varbinary(64),
	@QUESTYPE		varchar(100),
	@RVAL			INT OUTPUT
AS
BEGIN
	IF NOT EXISTS(SELECT UserAccountID FROM UserAccount WHERE Username = @USERNAME)
	BEGIN

		-- Grab UserAccountTypeID and SecurityQuestionTypeID for later use
		DECLARE @ACCTYPEID		int = (SELECT UserAccountTypeID FROM UserAccountType WHERE TypeName = @ACCTYPE)
		DECLARE @QUESTTYPEID	int = (SELECT SecurityQuestionTypeID FROM SecurityQuestionType WHERE QuestionText = @QUESTYPE)

		-- Create UserAccount record
		INSERT INTO UserAccount (Username, IsEnabled, DateCreated, UserAccountTypeID)
			VALUES (@USERNAME, @ENABLED, @DATECREATED, @ACCTYPEID)

		-- Grab UserAccountID of UserAccount just created
		DECLARE @USERID	INT = (SELECT UserAccountID FROM UserAccount WHERE Username = @USERNAME)

		-- Create UserPassword record
		INSERT INTO UserPassword (UserAccountID, PasswordHash, PasswordSalt)
			VALUES (@USERID, @PASSHASH, @PASSSALT)

		-- Create SecurityQuestion record
		INSERT INTO SecurityQuestion (UserAccountID, SecurityQuestionTypeID)
			VALUES (@USERID, @QUESTTYPEID)

		-- Grab SecurityQuestionID from SecurityQuestion just created
		DECLARE @QUESID INT = (SELECT SecurityQuestionID FROM SecurityQuestion WHERE UserAccountID = @USERID)

		-- Create SecurityQuestionAnswer record
		INSERT INTO SecurityQuestionAnswer (SecurityQuestionID, AnswerHash, AnswerSalt)
			VALUES (@QUESID, @ANSHASH, @ANSSALT)

		SET @RVAL = 1
	END
	ELSE SET @RVAL = 0
END
GO
