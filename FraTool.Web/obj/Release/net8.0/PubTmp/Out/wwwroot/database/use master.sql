use master
GO
Create Database FraTool
GO
use FraTool
GO
CREATE SCHEMA [Shared]
GO
CREATE SCHEMA [Security]
GO
-----------Company-------------
Create Table [Shared].Company
(
	CompanyId bigint primary key,
	CompanyName nvarchar(255),
	CompanyCode nvarchar(20),
	ShortCode nvarchar(20),
	GardenId int,
	IsTeaEstate int,
	IsActive int,
	EntryBy nvarchar(80),
	EntryDate DateTime,
	ModifyBy nvarchar(80),
	ModifyDate DateTime
)
GO
Create Table Shared.FraLoginCompanies(
	RecordId bigint primary key,
	LoginEstateId bigint,
	LoginEstateName nvarchar(255),
	CompanyCode nvarchar(80),
	FraCompanyCode nvarchar(255),
	FraCompanyName nvarchar(255),
	EstateCode nvarchar(80),
	FraDivisionCode nvarchar(255),
	FraDivisionName nvarchar(255),
	IsActive int,
	EntryBy nvarchar(80),
	EntryDate DateTime,
	ModifyBy nvarchar(80),
	ModifyDate DateTime
)
GO
Create Procedure [Shared].AddCompany
(
	@CompanyName nvarchar(255),
	@EntryBy nvarchar(80)
)
AS
BEGIN
	declare @id bigint;
	select @id = ISNULL(Max(CompanyId),0) from [Shared].Company
	IF @id is not null
	BEGIN
		INSERT INTO [Shared].Company
		(
			CompanyId, CompanyName, IsActive, EntryBy, EntryDate
		)
		VALUES
		(
			(@id + 1), @CompanyName, 1, @EntryBy, GETDATE()
		)
	END
END
GO
Create Procedure [Shared].UpdateCompany
(
	@CompanyId BIGINT,
	@CompanyName NVARCHAR(255),
	@ModifyBy NVARCHAR(80)
)
AS
BEGIN
	DECLARE @isFound INT;
	SELECT @isFound = COUNT(*) FROM [Shared].Company WHERE CompanyId = @CompanyId
	IF @isFound > 0
	BEGIN
		UPDATE [Shared].Company
		SET 
		CompanyName = @CompanyName,
		ModifyBy = @ModifyBy,
		ModifyDate = GETDATE()
		WHERE 
		CompanyId = @CompanyId
	END
END
GO
Create Procedure [Shared].CheckCompanyName
(
	@CompanyName nvarchar(255)
)
AS
BEGIN
	IF @CompanyName is not null
	BEGIN
		Select count(*) IsFound from [Shared].Company Where CompanyName = @CompanyName
	END
END
GO
Create Procedure [Shared].AllCompanies
AS
BEGIN
	SELECT * FROM [Shared].Company
END
GO
Create Procedure [Shared].ChangeCompanyStatus
(
	@CompanyId bigint,
	@IsActive int,
	@ModifyBy nvarchar(80)
)
AS
BEGIN
	IF @CompanyId is not null
	BEGIN
		Update [Shared].Company
		Set
		IsActive = @IsActive,
		ModifyBy = @ModifyBy,
		ModifyDate = GETDATE()
		Where
		CompanyId = @CompanyId
	END
END
GO
-------------Users-------------
Create Table [Security].Users(
	UserId bigint primary key,
	FullName nvarchar(120),
	EmailAddress nvarchar(120),
	MobileNumber nvarchar(20),
	CompanyId bigint,
	UserName nvarchar(80),
	PasswordHash nvarchar(max),
	IsActive int,
	EntryBy nvarchar(80),
	EntryDate DateTime,
	ModifyBy nvarchar(80),
	ModifyDate DateTime
);
GO
Create Table [Security].UsersLog(
	LogId bigint primary key,
	UserId bigint,
	PasswordHash nvarchar(max),
	[NewPassword] nvarchar(255),
	[LastPassword] nvarchar(255),
	IsActive int,
	EntryBy nvarchar(80),
	EntryDate DateTime,
	ModifyBy nvarchar(80),
	ModifyDate DateTime
);
GO
Create Procedure [Security].AddUser
(
	@FullName nvarchar(120) = null,
	@EmailAddress nvarchar(120) = null,
	@MobileNumber nvarchar(20) = null,
	@CompanyId bigint = 0,
	@UserName nvarchar(80),
	@Password nvarchar(255),
	@PasswordHash nvarchar(max),
	@EntryBy nvarchar(80) = null
)
AS
BEGIN
	declare @id bigint;
	select @id = ISNULL(Max(UserId),0) from [Security].Users
	IF @id is not null
	BEGIN
		Insert Into [Security].Users
		(
			UserId, FullName, EmailAddress, MobileNumber, CompanyId, UserName, PasswordHash, IsActive, EntryBy, EntryDate
		)
		Values
		(
			(@id+1), @FullName, @EmailAddress, @MobileNumber, @CompanyId, @UserName, @PasswordHash, 1, @EntryBy, GETDATE()
		)

		IF @@ROWCOUNT > 0
		BEGIN
			declare @logId bigint;
			select @logId = ISNULL(MAX(LogId),0) from [Security].UsersLog
			IF @logId is not null
			BEGIN
				Insert Into [Security].UsersLog
				(
					LogId, UserId, PasswordHash, NewPassword, IsActive, EntryBy, EntryDate
				)
				Values
				(
					(@logId+1), (@id+1), @PasswordHash, @Password, 1, @EntryBy, GETDATE()
				)
			END
		END
	END
END
GO
declare
@fullName nvarchar(120) = 'System Admin',
@email nvarchar(120) = 'admin@duncan.com',
@mobile nvarchar(20) = '+8801700000001',
@user nvarchar(80) = 'admin',
@password nvarchar(255) = '1234Abcd#',
@passwordHash nvarchar(max) = 'YTRmYTlkMGZlMmJlYTExNDMwMTM5ZGQ1OGYzNTJmM2RmZmQ2YzA3OA==',
@entryBy nvarchar(80) = 'admin'
Exec [Security].AddUser @fullName, @email, @mobile, 1, @user, @password, @passwordHash, @entryBy
GO
Create procedure [Security].CheckUserName
(
	@UserName nvarchar(80)
)
AS
BEGIN
	IF @UserName is not null
	BEGIN
		Select count(*) IsFound from [Security].Users Where UserName = @UserName
	END
END
GO
Create Procedure [Security].GetAllUsers
AS
BEGIN
	DECLARE @data TABLE(
		LogId bigint,
		UserId bigint,
		FullName nvarchar(120),
		EmailAddress nvarchar(120),
		MobileNumber nvarchar(20),
		CompanyId bigint,
		UserName nvarchar(80),
		[Password] nvarchar(255),
		PasswordHash nvarchar(max),
		IsActive int,
		EntryBy nvarchar(80),
		EntryDate DateTime,
		ModifyBy nvarchar(80),
		ModifyDate DateTime
	)
	DECLARE @result TABLE(
		UserId bigint,
		FullName nvarchar(120),
		EmailAddress nvarchar(120),
		MobileNumber nvarchar(20),
		CompanyId bigint,
		CompanyName nvarchar(255),
		UserName nvarchar(80),
		[Password] nvarchar(255),
		PasswordHash nvarchar(max),
		IsActive int,
		EntryBy nvarchar(80),
		EntryDate DateTime,
		ModifyBy nvarchar(80),
		ModifyDate DateTime
	)
	INSERT INTO @data
	SELECT l.LogId, u.UserId, u.FullName, u.EmailAddress, u.MobileNumber, u.CompanyId, u.UserName, l.NewPassword, u.PasswordHash
	, l.IsActive, u.EntryBy, u.EntryDate, u.ModifyBy, u.ModifyDate
	FROM [Security].Users u
	INNER JOIN [Security].UsersLog l ON u.UserId = l.UserId

	DECLARE @count INT;
	SELECT @count = COUNT(*) FROM @data
	WHILE @count > 0
	BEGIN
		DECLARE @userId bigint;
		DECLARE @logId bigint;
		DECLARE @countC int;
		SELECT TOP 1 @userId = UserId FROM @data
		SELECT @countC = COUNT(*) FROM @data WHERE UserId = @userId
		SELECT TOP 1 @logId = LogId FROM @data WHERE UserId = @userId ORDER BY LogId DESC
		INSERT INTO @result
		SELECT d.UserId, d.FullName, d.EmailAddress, d.MobileNumber, d.CompanyId
		, c.CompanyName , d.UserName, d.[Password], d.PasswordHash
		, d.IsActive, d.EntryBy, d.EntryDate, d.ModifyBy, d.ModifyDate
		FROM @data d
		Inner Join [Shared].Company c on d.CompanyId = c.CompanyId
		WHERE LogId = @logId
		DELETE FROM @data WHERE UserId = @userId
		SET @count = @count - @countC
	END
	SELECT * FROM @result
END
go
Create Procedure [Security].ChangeUserStatus
(
	@UserId bigint,
	@IsActive int,
	@ModifyBy nvarchar(80)
)
AS
BEGIN
	IF @UserId is not null
	BEGIN
		Update [Security].Users 
		Set 
		IsActive = @IsActive, 
		ModifyBy = @ModifyBy, 
		ModifyDate = GETDATE() 
		Where 
		UserId = @UserId
		IF @@ROWCOUNT > 0
		BEGIN
			declare @logId bigint;
			select top 1 @logId = LogId from [Security].UsersLog Where UserId = @UserId Order by LogId desc
			IF @logId is not null
			BEGIN
				Update [Security].UsersLog
				SET
				IsActive = @IsActive,
				ModifyBy = @ModifyBy,
				ModifyDate = GETDATE()
				Where --UserId = @UserId And
				LogId = @logId
			END
		END
	END
END
GO
CREATE PROCEDURE [Security].UpdateUserInfo
(
	@UserId BIGINT,
	@EmailAddress NVARCHAR(120) = NULL,
	@MobileNumber NVARCHAR(20) = NULL,
	@CompanyId BIGINT = 0,
	@ModifyBy NVARCHAR(80) = NULL
)
AS
BEGIN
	IF @UserId IS NOT NULL
	BEGIN
		DECLARE @IsFound INT;
		SELECT @IsFound = COUNT(*) FROM [Security].Users WHERE UserId = @UserId
		IF @IsFound > 0
		BEGIN
			UPDATE [Security].Users 
			SET
			EmailAddress = @EmailAddress
			, MobileNumber = @MobileNumber
			, CompanyId = @CompanyId
			, ModifyBy = @ModifyBy
			, ModifyDate = GETDATE()
			WHERE UserId = @UserId
		END
	END
END
GO
Create Procedure [Security].UserLogin
(
	@UserName nvarchar(80),
	@PasswordHash nvarchar(max)
)
AS
BEGIN
	IF @UserName is not null
	BEGIN
		select count(*) AS IsFound from [Security].Users 
		Where UserName = @UserName And PasswordHash = @PasswordHash
		And IsActive = 1
	END
END
GO
Create Procedure [Security].FraInfoForLoginByUserName
(
	@UserName Nvarchar(80) = Null
)
As
Begin
	If @UserName Is Not Null
	Begin
		Select 
		u.FullName, u.CompanyId, c.FraCompanyCode, c.FraCompanyName, c.FraDivisionCode, c.FraDivisionName
		From [Security].Users u 
		Inner Join [Shared].FraLoginCompanies c
		On c.LoginEstateId = u.CompanyId
		Where u.UserName = @UserName And u.IsActive = 1
	End
End
GO
Create Procedure [Security].CheckPassword
(
	@UserName nvarchar(80),
	@Password nvarchar(255)
)
AS
BEGIN
	IF @UserName IS NOT NULL
	BEGIN
		declare @UserId bigint;
		Select @UserId = UserId from [Security].Users Where UserName = @UserName
		IF @UserId IS NOT NULL
		BEGIN
			Select COUNT(*) IsFound from [Security].UsersLog 
			Where 
			UserId = @UserId 
			And NewPassword = @Password
			And IsActive = 1
		END
	END
END
GO
Create Procedure [Security].ChangePassword
(
	@UserName nvarchar(80),
	@OldPassword nvarchar(255),
	@NewPassword nvarchar(255),
	@PasswordHash nvarchar(max),
	@EntryBy nvarchar(80)
)
AS
BEGIN
	declare @UserId bigint;
	Update [Security].Users 
	Set
	PasswordHash = @PasswordHash,
	ModifyBy = @EntryBy,
	ModifyDate = GETDATE()
	Where UserName = @UserName
	IF @@ROWCOUNT > 0
	BEGIN
		select @UserId = UserId from [Security].Users where UserName = @UserName
		IF @UserId is not null
		BEGIN
			Update [Security].UsersLog 
			set 
			IsActive = 0,
			ModifyBy = @EntryBy,
			ModifyDate = GETDATE()
			Where
			UserId = @UserId
			And IsActive = 1
			IF @@ROWCOUNT > 0
			BEGIN
				declare @LogId bigint;
				select @LogId = ISNULL(Max(LogId),0) from [Security].UsersLog
				IF @LogId is not null
				BEGIN
					Insert into [Security].UsersLog
					(
						LogId, UserId, PasswordHash, NewPassword, LastPassword, IsActive, EntryBy, EntryDate
					)
					Values
					(
						(@LogId + 1), @UserId, @PasswordHash, @NewPassword, @OldPassword, 1, @EntryBy, GETDATE()
					)
				END
			END
		END
	END
END
GO
CREATE PROCEDURE Security.GetCompanyByUserId
(
	@UserName NVARCHAR(80)
)
AS
BEGIN
	IF @UserName IS NOT NULL
	BEGIN
		SELECT 
		CompanyId 
		FROM Security.Users 
		WHERE UserName = @UserName
		AND IsActive = 1;
	END
END
GO
----------Roles--------------
Create Table [Security].Roles(
	RoleId bigint primary key,
	RoleName nvarchar(80),	
	IsActive int,
	EntryBy nvarchar(80),
	EntryDate DateTime,
	ModifyBy nvarchar(80),
	ModifyDate DateTime
);
GO
Create Procedure [Security].CheckRoleName
(
	@RoleName nvarchar(80) 
)
AS
BEGIN
	IF @RoleName is not null
	BEGIN
		Select 
		count(*) IsFound 
		from [Security].Roles 
		Where 
		RoleName = @RoleName
	END
END
GO
Create Procedure [Security].AddRole
(
	@RoleName nvarchar(80),
	@EntryBy nvarchar(80)
)
AS
BEGIN
	declare @id bigint;
	select @id = ISNULL(Max(RoleId),0) from [Security].Roles
	IF @id is not null
	BEGIN
		Insert Into [Security].Roles
		(
			RoleId, RoleName, IsActive, EntryBy, EntryDate
		)
		Values
		(
			(@id+1), @RoleName, 1, @EntryBy, GETDATE()
		)
	END
END
GO
Create Procedure [Security].GetAllRoles
AS
BEGIN
	Select
	* From
	[Security].Roles
END
GO
Create Procedure [Security].UpdateRoles
(
	@RoleId bigint,
	@RoleName nvarchar(80),
	@ModifyBy nvarchar(80) 
)
AS
BEGIN
	declare @IsFound int;
	Select 
	@IsFound = COUNT(*) 
	From [Security].Roles
	Where
	RoleId = @RoleId
	IF @IsFound > 0
	BEGIN
		Update [Security].Roles
		Set
		RoleName = @RoleName,
		ModifyBy = @ModifyBy,
		ModifyDate = GETDATE()
		Where
		RoleId = @RoleId
	END
END
GO
Create Procedure [Security].ChangeRoleStatus
(
	@RoleId bigint,
	@IsActive int,
	@ModifyBy nvarchar(80)
)
AS
BEGIN
	IF @RoleId is not null
	BEGIN
		Update [Security].Roles 
		Set 
		IsActive = @IsActive, 
		ModifyBy = @ModifyBy, 
		ModifyDate = GETDATE() 
		Where 
		RoleId = @RoleId
	END
END
GO
---------------Menu---------------
Create Table [Security].Menus(
	MenuId bigint primary key,
	DisplayName nvarchar(60),
	ControllerName nvarchar(50),
	ActionName nvarchar(50),
	MenuUrl nvarchar(255),
	IsParentMenu int,
	ParentMenuId int,
	IconTag nvarchar(50),
	IsActive int,
	EntryBy nvarchar(80),
	EntryDate DateTime,
	ModifyBy nvarchar(80),
	ModifyDate DateTime
);
GO
Create Procedure [Security].GetAllMenu
AS
BEGIN
	Select
	* From
	[Security].Menus
END
GO
Create Procedure [Security].[AddMenu]
(
	@DisplayName nvarchar(60)= null,
	@ControllerName nvarchar(50)= null,
	@ActionName nvarchar(50)= null,
	@MenuUrl nvarchar(255)= null,
	@IsParentMenu int= null,
	@ParentMenuId int= null,
	@IconTag nvarchar(50)= null,
	@EntryBy nvarchar(80)
)
AS
BEGIN
	declare @id bigint;
	select @id = ISNULL(Max(MenuId),0) from [Security].Menus
	IF @id is not null
	BEGIN
		Insert Into [Security].Menus
		(
			MenuId, DisplayName, ControllerName, ActionName, MenuUrl, IsParentMenu, ParentMenuId, IconTag, IsActive, EntryBy, EntryDate
		)
		Values
		(
			(@id+1), @DisplayName, @ControllerName, @ActionName, @MenuUrl, @IsParentMenu, @ParentMenuId, @IconTag, 1, @EntryBy, GETDATE()
		)
	END
END
GO
Create Procedure [Security].CheckMenuDisplayName
(
	@DisplayName nvarchar(60)
)
AS
BEGIN
	IF @DisplayName is not null
	BEGIN
		Select 
		count(*) IsFound 
		from [Security].Menus 
		Where 
		DisplayName = @DisplayName
	END
END
GO
Create Procedure [Security].[UpdateMenus]
(
	@MenuId bigint,
	@DisplayName nvarchar(60)= null,
	@ControllerName nvarchar(50)= null,
	@ActionName nvarchar(50)= null,
	@MenuUrl nvarchar(255)= null,
	@IsParentMenu int= null,
	@ParentMenuId int= null,
	@IconTag nvarchar(50)= null,
	@ModifyBy nvarchar(80) 
)
AS
BEGIN
	IF @MenuId is not null
	begin
		declare @IsFound int;
		Select @IsFound = COUNT(*) From [Security].Menus Where MenuId = @MenuId
		IF @IsFound > 0
		BEGIN
			Update [Security].Menus
			Set
			DisplayName = @DisplayName,
			ControllerName = @ControllerName,
			ActionName = @ActionName,
			MenuUrl = @MenuUrl,
			IsParentMenu = @IsParentMenu,
			ParentMenuId = @ParentMenuId,
			IconTag = @IconTag,
			ModifyBy = @ModifyBy,
			ModifyDate = GETDATE()
			Where MenuId = @MenuId
		END
	end
END
GO
Create Procedure [Security].ChangeMenuStatus
(
	@MenuId bigint,
	@IsActive int,
	@ModifyBy nvarchar(80)
)
AS
BEGIN
	IF @MenuId is not null
	BEGIN
		Update [Security].Menus 
		Set 
		IsActive = @IsActive, 
		ModifyBy = @ModifyBy, 
		ModifyDate = GETDATE() 
		Where 
		MenuId = @MenuId
	END
END
GO
Create Procedure [Security].GetParentMenu
AS
BEGIN
	select * from [Security].Menus Where IsParentMenu = 1 And IsActive = 1
END
GO
CREATE PROCEDURE [Security].[GetMenusByUser]
(
	@UserName NVARCHAR(80)
)
AS
BEGIN
	IF @UserName IS NOT NULL
	BEGIN
		SELECT * FROM [Security].Menus WHERE MenuId IN (
			SELECT MenuId FROM [Security].MenusInRole WHERE RoleId IN (
				SELECT RoleId FROM [security].UsersInRole WHERE UserId = (
					SELECT UserId FROM [Security].Users WHERE UserName = @UserName AND IsActive = 1
				) AND IsActive = 1
			) AND IsActive = 1
		) AND IsActive = 1
	END
END
GO
-------------Menus In Role-------------------
Create Table [Security].MenusInRole
(
	RecordId bigint primary key IDENTITY,
	RoleId bigint,
	MenuId bigint,
	ApprovalStatus nvarchar(20),
	IsActive int,
	EntryBy nvarchar(80),
	EntryDate datetime,
	ApprovedBy nvarchar(80),
	ApprovedDate datetime
)
GO
Create Table [Security].MenusInRoleLog
(
	LogId bigint primary key IDENTITY,
	RecordId bigint,
	RoleId bigint,
	MenuId bigint,
	ApprovalStatus nvarchar(20),
	IsActive int,
	EntryBy nvarchar(80),
	EntryDate datetime,
	ApprovedBy nvarchar(80),
	ApprovedDate datetime,
	LogBy nvarchar(80),
	LogDate datetime
)
GO
Create Procedure [Security].AddMenusInRole
(
	@RoleId bigint,
	@MenuId bigint,
	@EntryBy nvarchar(80)
)
AS
BEGIN
	IF @RoleId IS NOT NULL
	BEGIN
		INSERT INTO [Security].MenusInRole
		(
			RoleId, MenuId, IsActive, EntryBy, EntryDate
		)
		VALUES
		(
			@RoleId, @MenuId, 1, @EntryBy, GETDATE()
		)
	END
END
GO
Create Procedure [Security].CheckMenusInRole
(
	@RoleId bigint,
	@EntryBy nvarchar(80)
)
AS
BEGIN
	IF @RoleId IS NOT NULL
	BEGIN
		DECLARE @count int;
		SELECT @count = COUNT(*) FROM [Security].MenusInRole WHERE RoleId = @RoleId
		IF @count > 0
		BEGIN
			INSERT INTO [Security].MenusInRoleLog
			SELECT RecordId, RoleId, MenuId, ApprovalStatus, 0, EntryBy, EntryDate, ApprovedBy, ApprovedDate, @EntryBy, GETDATE()
			FROM [Security].MenusInRole WHERE RoleId = @RoleId
			IF @@ROWCOUNT > 0
			BEGIN
				DELETE FROM [Security].MenusInRole WHERE RoleId = @RoleId
			END
		END
	END
END
GO
Create Procedure [Security].AllMenusInRole
AS
BEGIN
	SELECT * FROM [Security].MenusInRole WHERE IsActive = 1;
END
GO
Create Procedure [Security].MenusByRoleId
(
	@RoleId bigint
)
AS
BEGIN
	SELECT * FROM [Security].MenusInRole WHERE RoleId = @RoleId AND IsActive = 1;
END
GO
--------------Users In Role ----------------------
Create Table [Security].UsersInRole
(
	RecordId bigint primary key IDENTITY,
	RoleId bigint,
	UserId bigint,
	ApprovalStatus nvarchar(20),
	IsActive int,
	EntryBy nvarchar(80),
	EntryDate datetime,
	ApprovedBy nvarchar(80),
	ApprovedDate datetime
)
GO
Create Table [Security].UsersInRoleLog
(
	LogId bigint primary key IDENTITY,
	RecordId bigint,
	RoleId bigint,
	UserId bigint,
	ApprovalStatus nvarchar(20),
	IsActive int,
	EntryBy nvarchar(80),
	EntryDate datetime,
	ApprovedBy nvarchar(80),
	ApprovedDate datetime,
	LogBy nvarchar(80),
	LogDate datetime
)
GO
Create Procedure [Security].AddUsersInRole
(
	@RoleId bigint,
	@UserId bigint,
	@EntryBy nvarchar(80)
)
AS
BEGIN
	IF @RoleId IS NOT NULL
	BEGIN
		INSERT INTO [Security].UsersInRole
		(
			RoleId, UserId, IsActive, EntryBy, EntryDate
		)
		VALUES
		(
			@RoleId, @UserId, 1, @EntryBy, GETDATE()
		)
	END
END
GO
Create Procedure [Security].CheckUsersInRole
(
	@RoleId bigint,
	@EntryBy nvarchar(80)
)
AS
BEGIN
	IF @RoleId IS NOT NULL
	BEGIN
		DECLARE @count int;
		SELECT @count = COUNT(*) FROM [Security].UsersInRole WHERE RoleId = @RoleId
		IF @count > 0
		BEGIN
			INSERT INTO [Security].UsersInRoleLog
			SELECT RecordId, RoleId, UserId, ApprovalStatus, IsActive, EntryBy, EntryDate, ApprovedBy, ApprovedDate, @EntryBy, GETDATE()
			FROM [Security].UsersInRole WHERE RoleId = @RoleId
			IF @@ROWCOUNT > 0
			BEGIN
				DELETE FROM [Security].UsersInRole WHERE RoleId = @RoleId
			END
		END
	END
END
GO
Create Procedure [Security].AllUsersInRole
AS
BEGIN
	SELECT * FROM [Security].UsersInRole WHERE IsActive = 1;
END
GO
Create Procedure [Security].UsersByRoleId
(
	@RoleId bigint
)
AS
BEGIN
	SELECT * FROM [Security].UsersInRole WHERE RoleId = @RoleId AND IsActive = 1;
END
GO
Create Procedure Security.IsAuthUrl
(
	@UserName nvarchar(80),
	@MenuUrl nvarchar(255)
)
AS
BEGIN
	IF @UserName IS NOT NULL
	BEGIN
	select count(*) IsFound from Security.Menus Where MenuId in 
		(
		select MenuId from Security.MenusInRole 
		Where RoleId =
			(
			select RoleId from Security.UsersInRole 
			where UserId = 
				(
				select UserId from Security.Users 
				Where UserName = @UserName and IsActive = 1
				) 
			and IsActive = 1
			) 
		and IsActive = 1
		)
	and IsActive = 1 And MenuUrl = @MenuUrl
	END
END
GO
Create PROCEDURE [Security].[GetRoleByUser]
(
	@UserName NVARCHAR(80)
)
AS
BEGIN
	IF @UserName IS NOT NULL
	BEGIN
		SELECT RoleId FROM [security].UsersInRole WHERE UserId = (
			SELECT UserId FROM [Security].Users WHERE UserName = @UserName AND IsActive = 1
		) AND IsActive = 1
	END
END
GO
------------***********Voucher************-----------
Create Schema Voucher
GO
Create table Voucher.VoucherMaster
(
	RecordId bigint primary key,
	VoucherDate nvarchar(50),
	CompanyName nvarchar(120),
	FraDivisionCode nvarchar(255),
	EstateName nvarchar(120),
	DivisionName nvarchar(80),
	VoucherDescription nvarchar(max),
	VoucherType nvarchar(50),
	EntryDate datetime,
	IsSent int,
	IsActive int,
	VoucherNo nvarchar(50)
)
GO
Create Proc Voucher.CheckIfExistVoucher
(
	@date nvarchar(20) = null,
	@estate nvarchar(120) = null,
	@division nvarchar(80) = null,
	@type nvarchar(50) = null
)
As
Begin
	declare @count int = 0;
	If @date is not null And @estate is not null And 
	@division is not null And @type is not null
	Begin
		Select @count = Count(*) from Voucher.VoucherMaster
		Where VoucherDate = @date and
		EstateName = @estate and
		DivisionName = @division and 
		VoucherType = @type;
	End
	Else
	Begin
		Set @count = -1
	End
	Select @count [IsExist]
End
GO
Create Proc Voucher.SaveVoucherMaster(
	@date nvarchar(50) = '',
	@company nvarchar(120) = '',
	@divisionCode nvarchar(255) = '',
	@estate nvarchar(120) = '',
	@division nvarchar(80),
	@description nvarchar(max) = '',
	@voucher_type nvarchar(50) = '',
	@MasterId bigint out
)
As
Begin
	Declare @id bigint;
	Select @id = ISNULL(Max(RecordId),0) + 1 from Voucher.VoucherMaster
	If @id is not null
	Begin
		Insert Into Voucher.VoucherMaster
		(
			RecordId, VoucherDate, CompanyName, FraDivisionCode, EstateName, DivisionName, VoucherDescription, VoucherType, EntryDate, IsSent, IsActive
		)
		Values
		(
			@id, @date, @company, @divisionCode, @estate, @division, @description, @voucher_type, getdate(), 0, 1
		)
		If @@ROWCOUNT > 0
			set @MasterId = @id
	End
	Select @MasterId MasterId;
End
Go
Create table Voucher.VoucherData
(
	RecordId bigint primary key,
	MasterId bigint,
	AccountCode nvarchar(120),
	AccountHead nvarchar(120),
	DataDescription nvarchar(max),
	Amount money,
	EntryDate datetime,
	IsSent int,
	IsActive int
)
GO
Create Proc Voucher.SaveVoucherData(
	@MasterId bigint,
	@account_code nvarchar(120) = '',
	@head_name nvarchar(120) = '',
	@description nvarchar(max) = '',
	@amount money = 0,
	@DataId bigint out
)
As
Begin
	Declare @id bigint;
	Select @id = ISNULL(Max(RecordId),0) + 1 from Voucher.VoucherData
	If @id is not null
	Begin
		Insert Into Voucher.VoucherData
		(
			RecordId, MasterId, AccountCode, AccountHead, DataDescription, Amount, EntryDate, IsSent, IsActive
		)
		Values
		(
			@id, @MasterId, @account_code, @head_name, @description, @amount, getdate(), 0, 1
		)
		If @@ROWCOUNT > 0
			set @DataId = @id
	End
	Select @DataId DataId;
End
GO
Create table Voucher.VoucherDetails
(
	RecordId bigint primary key,
	DataId bigint,
	AccountCode nvarchar(120),
	AccountHead nvarchar(120),
	DataDescription nvarchar(max),
	Amount money,
	EntryDate datetime,
	IsSent int,
	IsActive int
)
GO
Create  Proc Voucher.SaveVoucherDetails(
	@DataId bigint,
	@account_code nvarchar(120) = '',
	@head_name nvarchar(120) = '',
	@description nvarchar(max) = '',
	@amount money = 0
)
As
Begin
	Declare @id bigint;
	Select @id = ISNULL(Max(RecordId),0) + 1 from VoucherDetails
	If @id is not null
	Begin
		Insert Into Voucher.VoucherDetails
		(
			RecordId, DataId, AccountCode, AccountHead, DataDescription, Amount, EntryDate, IsSent, IsActive
		)
		Values
		(
			@id, @DataId, @account_code, @head_name, @description, @amount, getdate(), 0, 1
		)
	End
End
GO
Create Proc Voucher.GetVoucherMaster
(
	@FraDivisionCode nvarchar(255) = null
)
As
Begin
	If @FraDivisionCode is not null
	Begin
		Select RecordId, VoucherDate, CompanyName, EstateName, 
		DivisionName, VoucherDescription, VoucherType, IsSent
		From Voucher.VoucherMaster 
		Where FraDivisionCode = @FraDivisionCode
		order by VoucherDate desc
	End
End
GO
Create Proc Voucher.GetVoucher
(
	@MasterId bigint = 0
)
AS
Begin
	If @MasterId > 0
	Begin
		declare @vData table(
			Id bigint,
			AccountCode nvarchar(120),
			AccountHead nvarchar(120),
			DataDescription nvarchar(max),
			Amount money
		)
		declare @vDetails table(
			Id bigint,
			DataId bigint,
			AccountCode nvarchar(120),
			AccountHead nvarchar(120),
			DataDescription nvarchar(max),
			Amount money
		)
		declare @temp table(	
			AccountCode nvarchar(120),
			AccountHead nvarchar(120),
			DataDescription nvarchar(max),
			Amount money
		)
		Insert Into @vData
		Select RecordId, AccountCode, AccountHead, DataDescription, Amount from Voucher.VoucherData
		Where MasterId = @masterId
		Insert Into @vDetails
		Select d.RecordId, d.DataId, d.AccountCode, d.AccountHead, d.DataDescription, d.Amount from @vData v
		Inner Join Voucher.VoucherDetails d on v.Id = d.DataId

		declare @vCount int;
		Select @vCount = Count(*) from @vData
		While @vCount > 0
		Begin
			declare @dataId bigint;
			Select top 1 @dataId = Id from @vData
			Insert Into @temp
			Select AccountCode, AccountHead, DataDescription, 0 from @vData Where Id = @dataId
			Insert Into @temp
			Select AccountCode, AccountHead, DataDescription, Amount from @vDetails Where DataId = @dataId 
			Delete @vData Where Id = @dataId;
			Delete @vDetails Where DataId = @dataId;
			Set @vCount = @vCount - 1;
		End
		Select * from @temp
	End
End
GO
Create Function Voucher.fn_GenVoucherNo
(
	@SerialNo int,
	@EstateCode nvarchar(20),
	@VoucherInit nvarchar(10)
)
Returns nvarchar(50)
As
Begin
	declare @Sequence nvarchar(50), @Voucher nvarchar(50);
	If Len(@SerialNo) = 1
	Begin
		Set @Sequence = '00000' + Convert(nvarchar(20),@SerialNo);
	End
	Else If Len(@SerialNo) = 2
	Begin
		Set @Sequence = '0000' + Convert(nvarchar(20),@SerialNo);
	End
	Else If Len(@SerialNo) = 3
	Begin
		Set @Sequence = '000' + Convert(nvarchar(20),@SerialNo);
	End
	Else If Len(@SerialNo) = 4
	Begin
		Set @Sequence = '00' + Convert(nvarchar(20),@SerialNo);
	End
	Else If Len(@SerialNo) = 5
	Begin
		Set @Sequence = '0' + Convert(nvarchar(20),@SerialNo);
	End
	Else If Len(@SerialNo) = 6
	Begin
		Set @Sequence = Convert(nvarchar(20),@SerialNo);
	End
	Set @Voucher = @EstateCode + '-' + @VoucherInit + '-' + @Sequence + '-' 
		+ RIGHT((Convert(nvarchar(20), YEAR(GetDate()))), LEN(Convert(nvarchar(20), YEAR(GetDate())))-2)
	Return @voucher
End
GO
Create Function fn_GetBatchSerialNo()
Returns int
As
Begin
	declare @SerialNo int
	Select @SerialNo = (count(*) + 1)  from TranMaster Where EntryDate between 
	DATEADD(DAY, DATEDIFF(DAY, '19000101', GETDATE()), '19000101')
	AND DATEADD(DAY, DATEDIFF(DAY, '18991231', GETDATE()), '19000101')	
	Return @SerialNo
End
GO
Create Function fn_GetVoucherSerialNo()
Returns int
As
Begin
	declare @SerialNo int
	Select @SerialNo = (max(VoucherSerialNo) + 1) from TranMaster 
	Where TranDate between (dateadd(yy, datediff(yy, 0, GETDATE()), 0)) and GETDATE()
	AND VoucherNo like '%AM-JT-0%'OR VoucherNo like '%AM-JR-0%'	
	Return @SerialNo
End
GO
--Confirm Check / Sent data to CHARMS database : based on login estates
Create Procedure Voucher.GetSentVoucherByMasterId
(
	@MasterId bigint = 0,
	@FraDivisionCode nvarchar(255) = null
)
As
Begin
	If @MasterId > 0 AND @FraDivisionCode is not null
	Begin
		Select CONVERT(VARCHAR(10), CONVERT(DATE, VoucherDate, 103), 120) TranDate,
		5 TranType,
		VoucherDescription Narration,
		fc.CompanyCode CompanyCode,
		fc.EstateCode EstateCode,
		AMTE.dbo.fn_GetBatchSerialNo() SerialNo,
		('J/' +fc.CompanyCode +'/' + fc.EstateCode + '/' + (CONVERT(nvarchar(20), GETDATE(),23)) + '/' + Convert(nvarchar(10), AMTE.dbo.fn_GetBatchSerialNo())) BatchNo,
		AMTE.dbo.fn_GetVoucherSerialNo() VoucherSerialNo,
		dbo.fn_GenVoucherNo (AMTE.dbo.fn_GetVoucherSerialNo(), fc.EstateCode, (Case When VoucherType = 'TEA' Then 'JT' Else 'JR' End)) VoucherNo
		from Voucher.VoucherMaster m
		Inner Join Shared.FraLoginCompanies fc on m.FraDivisionCode = fc.FraDivisionCode
		Where m.RecordId = @masterId and fc.FraDivisionCode = @FraDivisionCode
	End
End
GO
Create Proc InsertTranMaster_FraTool_Confirmation
( 
	@TranDate datetime, 
	@TranType int, 
	@BatchNo nvarchar(50), 
	@SerialNo int,  
	@VoucherNo nvarchar(50), 
	@Narration nvarchar(300),
	@EntryUserID int,
	@VoucherSerialNo int,
	@TranId bigint out
)
As
Begin
	Declare @id bigint;
	Select @id = ISNULL(Max(TranID),0) + 1 from TranMaster
	Insert Into TranMaster
	(TranID, TranDate, TranType, BatchNo, SerialNo, VoucherNo, Narration, ChequeNo, EntryUserID, EntryDate, UpdateUserID, UpdateDate, 
	IsAuthenticate, TranStatus, OnLocationID, ForLocationID, ChequeDate, SourceBatchNo, VoucherSerialNo, IsCanceled, EntryMethod, VoucherType, IsForigenCurency,
	CurrencyID, ConversionRate)
	Values
	(
		@id, @TranDate, @TranType, @BatchNo, @SerialNo, @VoucherNo, @Narration, '', @EntryUserID, Getdate(), null, null,
		1, 0, 2, 2, null, null, @VoucherSerialNo, 0, 1, 3, 0, null, null
	)
	If @@ROWCOUNT > 0
		Set @TranId = @id
	Select @TranId As TranId
End
GO
Create Procedure Voucher.GetSentVoucherDataByMasterId
(
	@MasterId bigint = 0
)
AS
Begin
	If @MasterId > 0
	Begin
		Select RecordId, AccountCode, AccountHead, DataDescription, Amount
		from Voucher.VoucherData
		Where MasterId = @MasterId
	End
End
GO
Create Procedure GetGLCodeByCodeName
(
	@CodeName nvarchar(50) = null
)
As
Begin
	If @CodeName is not null
	Begin
		Select GLID From GL Where code = @CodeName
	End
End
GO
Create  Procedure InsertTranDetail_FraTool_Confirmation
(
	@TranId int,
	@GlId int,
	@TranSide int,
	@Amount money,
	@Description nvarchar(250),
	@TranDetailId int out
)
AS
Begin
	declare @DetailId int
	Select @DetailId = ISNULL(Max(DetailID),0) + 1 from TranDetail
	Insert Into TranDetail
	(DetailID, TranID, GLID, ContraGLID, CostCenterID, TranSide, Amount, [Description], IsParentItem,CategoryID, RespondingGLID, 
	BillNo, BillDate, PersonID, RespondingUOID, RespondingURID, TranDetailStatus, OperationalStatus, ForLocationID, FCAmount)
	Values
	(
		@DetailId, @TranId, @GlId, null, null, @TranSide, @Amount, @Description, 0, 2, null, null, null, null, null, null, 2, 0, 2, 0
	)
	If @@ROWCOUNT > 0
		Set @TranDetailId = @DetailId
	Select @TranDetailId As DetailId
End
GO
Create Procedure GetTaskCodeIdByCodeName
(
	@CodeName nvarchar(50)
)
As
Begin
	Select RecordID, ObjectID from UserRecord Where Code = @CodeName
End
GO
Create Procedure Voucher.GetSentVoucherDetailsByVoucherDataId
(
	@DataId bigint = 0
)
AS
Begin
	If @DataId > 0
	Begin
		Select AccountCode, AccountHead, DataDescription, Amount
		from Voucher.VoucherDetails
		Where DataId = @DataId
	End
End
GO
Create Procedure InsertTranUserRecord_FraTool_Confirmation
(
	@TranId int = 0,
	@DetailId int,
	@GlId int,
	@UooneId int,
	@UroneId int,
	@Amount money,
	@Description nvarchar(250)
)
AS
Begin
	If @TranId > 0
	Begin
		declare @RecordId int
		Select @RecordId = ISNULL(Max(RecordID),0) + 1 from TranUserRecord
		Insert Into TranUserRecord
		(RecordID, TranID, DetailID, GLID, UOOneID, UROneID, UOTwoID, URTwoID, Amount, [Description], OperationalStatus)
		Values(@RecordId, @TranId, @DetailId, @GlId, @UooneId, @UroneId, null, 2, @Amount, @Description, null)
	End
End
GO

