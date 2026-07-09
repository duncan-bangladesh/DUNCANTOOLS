USE DBPROC
GO

--ALTER TABLE SupplierProfile ALTER COLUMN Country CHAR(50)
--ALTER TABLE SUPPLIERPROFILE ALTER COLUMN [Bank] [nvarchar](150)

ALTER TABLE SUPPLIERPROFILE ALTER COLUMN [Bank] [nvarchar](150)
GO
ALTER TABLE supplierprofile ALTER COLUMN City nvarchar(150)
GO
ALTER TABLE supplierprofile ALTER COLUMN Country nvarchar(150)
GO
ALTER TABLE supplierprofile ALTER COLUMN email nvarchar(80)
GO
ALTER TABLE supplierprofile ALTER COLUMN Company nvarchar(50)
GO
ALTER TABLE supplierprofile ALTER COLUMN AccountNo nvarchar(25)
GO
ALTER TABLE supplierprofile ALTER COLUMN RoutingNo nvarchar(25)
GO
ALTER TABLE supplierprofile ALTER COLUMN UpdateUser nvarchar(50)
GO
CREATE PROC sp_GetSupplierCodeBySupplierName
(
	@SupplierName NVARCHAR(255) = ''
)
AS
BEGIN
	IF @SupplierName != ''
	DECLARE @firstChar nvarchar(3) = UPPER(LEFT(@SupplierName, 1));
	DECLARE @count INT = 0, @code NVARCHAR(20);
	SELECT @count = count([Description]) FROM [dbo].[SupplierProfile] WHERE [Description] LIKE @firstChar + '%';
	IF LEN(@count) = 1
	BEGIN
		IF @count = 9
		BEGIN
			SET @code = @firstChar + '000' + CONVERT(NVARCHAR(15), (@count + 1))
		END
		ELSE
		BEGIN
			SET @code = @firstChar + '0000' + CONVERT(NVARCHAR(15), (@count + 1))
		END
	END
	ELSE IF LEN(@count) = 2
	BEGIN
		IF @count = 99
		BEGIN
			SET @code = @firstChar + '00' + CONVERT(NVARCHAR(15), (@count + 1))
		END
		ELSE
		BEGIN
			SET @code = @firstChar + '000' + CONVERT(NVARCHAR(15), (@count + 1))
		END
	END
	ELSE IF LEN(@count) = 3
	BEGIN
		IF @count = 999
		BEGIN
			SET @code = @firstChar + '0' + CONVERT(NVARCHAR(15), (@count + 1))
		END
		ELSE
		BEGIN
			SET @code = @firstChar + '00' + CONVERT(NVARCHAR(15), (@count + 1))
		END
	END
	ELSE IF LEN(@count) = 4
	BEGIN
		IF @count = 9999
		BEGIN
			SET @code = @firstChar + CONVERT(NVARCHAR(15), (@count + 1))
		END
		ELSE
		BEGIN
			SET @code = @firstChar + '0' + CONVERT(NVARCHAR(15), (@count + 1))
		END
	END
	ELSE
	BEGIN
		SET @code = @firstChar + CONVERT(NVARCHAR(15), (@count + 1))
	END
	SELECT @code AS SupplierCode
END
GO
CREATE FUNCTION dbo.NormalizeSupplierName (@text NVARCHAR(255))
RETURNS NVARCHAR(255)
AS
BEGIN
    SET @text = LOWER(LTRIM(RTRIM(@text)));
    SET @text = REPLACE(@text, '.', '');

    WHILE CHARINDEX('  ', @text) > 0
        SET @text = REPLACE(@text, '  ', ' ');

    RETURN @text;
END
GO
CREATE PROC sp_CheckDuplicateSupplierName
(
	@SupplierName NVARCHAR(255) = NULL
)
AS
BEGIN
	IF @SupplierName IS NOT NULL
	BEGIN
		SELECT COUNT(*) AS NoOfRecord  FROM SupplierProfile WHERE dbo.NormalizeSupplierName([Description]) = dbo.NormalizeSupplierName(@SupplierName);
	END
END
GO
CREATE PROC sp_SaveSupplierProfile
(
	@Code nvarchar(20)= null, 
	@Description nvarchar(255)= null, 
	@Address nvarchar(256)= null, 
	@City nvarchar(150)= null,
	@Country nvarchar(150)= null, 
	@Bank nvarchar(150)= null, 
	@AccountNo nvarchar(25)= null, 
	@RoutingNo nvarchar(25)= null, 
	@Taxgroup int= null,
	@TIN nvarchar(30)= null, 
	@BIN nvarchar(25)= null, 
	@Phone nvarchar(50)= null, 
	@email nvarchar(80)= null, 
	@CreateUser nvarchar(50)= null
)
AS
BEGIN
	Insert Into SupplierProfile
	(
		Code, [Description], [Address], City,
		Country, Bank, AccountNo, RoutingNo, Taxgroup,
		TIN, BIN, Phone, email, CreateUser, CreateDate
	)
	VALUES
	(
		@Code, @Description, @Address, @City,
		@Country, @Bank, @AccountNo, @RoutingNo, @Taxgroup,
		@TIN, @BIN, @Phone, @email, @CreateUser, GETDATE()
	)
END
GO
CREATE PROC sp_GetAllSupplierProfile
AS
BEGIN
	SET NOCOUNT ON;
	SELECT 
		ISNULL(SLNo, 0) AS SLNo,
		ISNULL(Code, '') AS Code,
		ISNULL(Description, '') AS Description,
		ISNULL(Address, '') AS Address,
		ISNULL(City, '') AS City,
		ISNULL(Country, '') AS Country,
		ISNULL(Phone, '') AS Phone,
		ISNULL(Fax, '') AS Fax,
		ISNULL(email, '') AS email,
		ISNULL(ConPerson, '') AS ConPerson,
		ISNULL([Group], '') AS [Group],
		ISNULL(Company, '') AS Company,
		ISNULL(CreateDate, '') AS CreateDate,
		ISNULL(CreateUser, '') AS CreateUser,
		ISNULL(UpdateDate, '') AS UpdateDate,
		ISNULL(UpdateUser, '') AS UpdateUser,
		ISNULL(TIN, '') AS TIN,
		ISNULL(Taxgroup, 0) AS Taxgroup,
		ISNULL(BIN, '') AS BIN,
		ISNULL(Bank, '') AS Bank,
		ISNULL(AccountNo, '') AS AccountNo,
		ISNULL(RoutingNo, '') AS RoutingNo
	FROM dbo.SupplierProfile 
	WHERE [Description] IS NOT NULL 
	ORDER BY [Description] ASC
END
GO
CREATE PROC sp_UpdateSupplierProfile
(
	@SLNo int = 0,
	--@Code nvarchar(20)= null, 
	@Description nvarchar(255)= null, 
	@Address nvarchar(256)= null, 
	@City nvarchar(150)= null,
	@Country nvarchar(150)= null, 
	@Bank nvarchar(150)= null, 
	@AccountNo nvarchar(25)= null, 
	@RoutingNo nvarchar(25)= null, 
	@Taxgroup int= null,
	@TIN nvarchar(30)= null, 
	@BIN nvarchar(25)= null, 
	@Phone nvarchar(50)= null, 
	@email nvarchar(80)= null, 
	@UpdateUser nvarchar(50)= null
)
AS
BEGIN
	IF @SLNo > 0
	BEGIN
		UPDATE SupplierProfile
		SET
			[Description] = @Description,
			[Address] = @Address, City = @City, Country = @Country,
			Bank = @Bank, AccountNo = @AccountNo, RoutingNo = @RoutingNo,
			Taxgroup = @Taxgroup, TIN = @TIN, BIN = @BIN,
			Phone = @Phone, email = @email,
			UpdateUser = @UpdateUser, UpdateDate = GETDATE()
		WHERE SLNo = @SLNo
	END
END
GO
