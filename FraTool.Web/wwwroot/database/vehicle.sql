CREATE SCHEMA Vehicle
GO
CREATE TABLE Vehicle.Owners
(
	RecordId BIGINT PRIMARY KEY,
	OwnerName NVARCHAR(120),
	OwnerDescription NVARCHAR(500),
	IsActive int,
	EntryBy nvarchar(80),
	EntryDate DateTime,
	ModifyBy nvarchar(80),
	ModifyDate DateTime
)
GO
Create Procedure Vehicle.GetOwners
AS
BEGIN
	Select * From Vehicle.Owners
END
GO
CREATE TABLE Vehicle.IssueLocation
(
	RecordId BIGINT PRIMARY KEY,
	LocationName NVARCHAR(120),
	LocationDescription NVARCHAR(500),
	IsActive int,
	EntryBy nvarchar(80),
	EntryDate DateTime,
	ModifyBy nvarchar(80),
	ModifyDate DateTime
)
GO
Create Procedure Vehicle.GetIssueLocation
AS
BEGIN
	Select * From Vehicle.IssueLocation
END
GO
CREATE TABLE Vehicle.IssueTo
(
	RecordId BIGINT PRIMARY KEY,
	ReceiverName NVARCHAR(120),
	MobileNo NVARCHAR(20),
	EmailAddress NVARCHAR(80),
	CurrentAddress NVARCHAR(255),
	IsActive int,
	EntryBy nvarchar(80),
	EntryDate DateTime,
	ModifyBy nvarchar(80),
	ModifyDate DateTime
)
GO
Create Procedure Vehicle.GetIssueTo
AS
BEGIN
	Select * From Vehicle.IssueTo
END
GO
CREATE TABLE Vehicle.VehicleType
(
	RecordId INT PRIMARY KEY,
	TypeName NVARCHAR(120),
	TypeDescription NVARCHAR(255),
	IsActive int,
	EntryBy nvarchar(80),
	EntryDate DateTime,
	ModifyBy nvarchar(80),
	ModifyDate DateTime
)
GO
Create Procedure Vehicle.GetVehicleType
AS
BEGIN
	Select * From Vehicle.VehicleType
END
GO
CREATE TABLE Vehicle.BRTAOffice
(
	RecordId INT PRIMARY KEY,
	OfficeName NVARCHAR(120),
	OfficeAddress NVARCHAR(255),
	IsActive int,
	EntryBy nvarchar(80),
	EntryDate DateTime,
	ModifyBy nvarchar(80),
	ModifyDate DateTime
)
GO
Create Procedure Vehicle.GetBRTAOffice
AS
BEGIN
	Select * From Vehicle.BRTAOffice
END
GO
CREATE TABLE Vehicle.Driver
(
	RecordId BIGINT PRIMARY KEY,
	DriverName NVARCHAR(120),
	CurrentAddress NVARCHAR(255),
	MobileNo NVARCHAR(20),
	IsActive int,
	EntryBy nvarchar(80),
	EntryDate DateTime,
	ModifyBy nvarchar(80),
	ModifyDate DateTime
)
GO
Create Procedure Vehicle.GetDrivers
AS
BEGIN
	Select * From Vehicle.Driver
END
GO
CREATE TABLE Vehicle.Vehicles
(
	RecordId BIGINT,
	VehicleName NVARCHAR(120),
	OwnerId BIGINT,
	RegistrationDate Date,
	LicensePlate NVARCHAR(50),
	IssueLocationId BIGINT,
	IssueToId BIGINT,
	VehicleTypeId INT,
	BRTAOfficeId INT,
	DriverId INT,
	SeatCapacityWithDriver INT,
	Remarks NVARCHAR(500),
	IsActive int,
	EntryBy nvarchar(80),
	EntryDate DateTime,
	ModifyBy nvarchar(80),
	ModifyDate DateTime
)
GO
Create Proc Vehicle.AddVehicle
(
	@VehicleName NVARCHAR(120) = NULL,
	@OwnerId BIGINT = NULL,
	@RegistrationDate nvarchar(20) = NULL,
	@LicensePlate NVARCHAR(50) = NULL,
	@IssueLocationId BIGINT = NULL,
	@IssueToId BIGINT = NULL,
	@VehicleTypeId INT = NULL,
	@BRTAOfficeId INT = NULL,
	@DriverId INT = NULL,
	@SeatCapacityWithDriver INT = NULL,
	@Remarks NVARCHAR(500) = NULL,
	@EntryBy nvarchar(80) = NULL	
)
AS
BEGIN
	IF @VehicleName IS NOT NULL	   
	BEGIN
		DECLARE @newId BIGINT = 0;
		SELECT @newId = (ISNULL(Max(RecordId),0) + 1) From Vehicle.Vehicles
		IF @newId > 0
		BEGIN
			INSERT INTO Vehicle.Vehicles
			(
				RecordId, VehicleName, OwnerId, RegistrationDate, LicensePlate, IssueLocationId,
				IssueToId, VehicleTypeId, BRTAOfficeId, DriverId, SeatCapacityWithDriver, Remarks,
				IsActive, EntryBy, EntryDate
			)
			VALUES
			(
				@newId, @VehicleName, @OwnerId, CONVERT(DATE, @RegistrationDate, 105) , @LicensePlate, @IssueLocationId,
				@IssueToId, @VehicleTypeId, @BRTAOfficeId, @DriverId, @SeatCapacityWithDriver,
				@Remarks, 1, @EntryBy, GETDATE()
			)
		END
	END
END
GO
CREATE PROC Vehicle.GetVehicles
AS
BEGIN
	SELECT
	v.RecordId, v.VehicleName, 
	CONVERT(NVARCHAR(20), v.RegistrationDate, 105) RegistrationDate, 
	v.LicensePlate,
	v.OwnerId, ISNULL(o.OwnerName,'') OwnerName, 
	v.IssueLocationId, ISNULL(l.LocationName,'') LocationName, 
	v.IssueToId, ISNULL(i.ReceiverName,'') ReceiverName, 
	v.VehicleTypeId, ISNULL(t.TypeName,'') TypeName, 
	v.BRTAOfficeId, ISNULL(b.OfficeName,'') OfficeName, 
	v.DriverId, ISNULL(d.DriverName,'') DriverName, 
	v.SeatCapacityWithDriver, v.Remarks, 
	v.IsActive
	FROM Vehicle.Vehicles v
	LEFT JOIN Vehicle.Owners o ON v.OwnerId = o.RecordId
	LEFT JOIN Vehicle.IssueLocation l ON v.IssueLocationId = l.RecordId
	LEFT JOIN Vehicle.IssueTo i ON v.IssueToId = i.RecordId
	LEFT JOIN Vehicle.VehicleType t ON v.VehicleTypeId = t.RecordId
	LEFT JOIN Vehicle.BRTAOffice b ON v.BRTAOfficeId = b.RecordId
	LEFT JOIN Vehicle.Driver d ON v.DriverId = d.RecordId
END
GO
CREATE TABLE Vehicle.VehiclesLog
(
	RecordId BIGINT PRIMARY KEY,
	VehicleId BIGINT,
	VehicleName NVARCHAR(120),
	OwnerId BIGINT,
	RegistrationDate Date,
	LicensePlate NVARCHAR(50),
	IssueLocationId BIGINT,
	IssueToId BIGINT,
	VehicleTypeId INT,
	BRTAOfficeId INT,
	DriverId INT,
	SeatCapacityWithDriver INT,
	Remarks NVARCHAR(500),
	IsActive int,
	EntryBy nvarchar(80),
	EntryDate DateTime,
	LastModifyBy nvarchar(80),
	LastModifyDate DateTime
)
GO
Create Proc Vehicle.UpdateVehicles
(
	@RecordId BIGINT = NULL,
	@VehicleName NVARCHAR(120) = NULL,
	@OwnerId BIGINT = NULL,
	@RegistrationDate nvarchar(20) = NULL,
	@LicensePlate NVARCHAR(50) = NULL,
	@IssueLocationId BIGINT = NULL,
	@IssueToId BIGINT = NULL,
	@VehicleTypeId INT = NULL,
	@BRTAOfficeId INT = NULL,
	@DriverId INT = NULL,
	@SeatCapacityWithDriver INT = NULL,
	@Remarks NVARCHAR(500) = NULL,
	@EntryBy nvarchar(80) = NULL
)
AS
BEGIN
	IF @RecordId > 0
	BEGIN
		DECLARE @isExist INT = 0;
		SELECT @isExist = COUNT(*) FROM Vehicle.Vehicles WHERE RecordId = @RecordId
		IF @isExist > 0
		BEGIN
			DECLARE @newId BIGINT = 0;
			SELECT @newId = (ISNULL(Max(RecordId),0) + 1) From Vehicle.VehiclesLog
			IF @newId > 0
			BEGIN
				BEGIN TRANSACTION;
				BEGIN TRY
				INSERT INTO Vehicle.VehiclesLog
				(
					RecordId, VehicleId, VehicleName, OwnerId, RegistrationDate, LicensePlate, IssueLocationId, 
					IssueToId, VehicleTypeId, BRTAOfficeId, DriverId, SeatCapacityWithDriver, Remarks, EntryBy, EntryDate, LastModifyBy, LastModifyDate
				)
				SELECT @newId, RecordId, VehicleName, OwnerId, RegistrationDate, LicensePlate, IssueLocationId, IssueToId,
				VehicleTypeId, BRTAOfficeId, DriverId, SeatCapacityWithDriver, Remarks, @EntryBy, GETDATE(), ModifyBy, ModifyDate
				FROM Vehicle.Vehicles WHERE RecordId = @RecordId
				IF @@ROWCOUNT > 0
				BEGIN
					--UPDATE HERE
					UPDATE Vehicle.Vehicles 
					SET VehicleName = @VehicleName, OwnerId = @OwnerId, RegistrationDate = CONVERT(DATE, @RegistrationDate, 105),
					LicensePlate = @LicensePlate, IssueLocationId = @IssueLocationId, IssueToId = @IssueToId,
					VehicleTypeId = @VehicleTypeId, BRTAOfficeId = @BRTAOfficeId, DriverId = @DriverId,
					SeatCapacityWithDriver = @SeatCapacityWithDriver, Remarks = @Remarks,
					ModifyBy = @EntryBy, ModifyDate = GETDATE() 
					WHERE RecordId = @RecordId
				END
				COMMIT TRANSACTION;
				END TRY
				BEGIN CATCH
					ROLLBACK TRANSACTION;
				END CATCH
			END
		END
	END
END
GO
----------Document------------
Create Table Vehicle.DocumentType
(
	RecordId INT PRIMARY KEY,
	DocumentTypeName NVARCHAR(255),
	DocumentTypeDescription NVARCHAR(500),
	IsActive int,
	EntryBy nvarchar(80),
	EntryDate DateTime,
	ModifyBy nvarchar(80),
	ModifyDate DateTime
)
GO
CREATE PROC Vehicle.GetDocumentType
AS
BEGIN
	SELECT * FROM Vehicle.DocumentType
END
GO
Create Table Vehicle.Documents
(
	RecordId INT PRIMARY KEY,
	DocumentTypeId INT,
	VehicleId BIGINT,
	IssueDate DATE,
	ExpiredDate DATE,
	FilePath NVARCHAR(MAX),
	Remarks NVARCHAR(500),
	IsActive int,
	EntryBy nvarchar(80),
	EntryDate DateTime,
	ModifyBy nvarchar(80),
	ModifyDate DateTime
)
GO
CREATE PROC Vehicle.SaveDocument
(
	@DocumentTypeId INT = NULL,
	@VehicleId BIGINT = NULL,
	@IssueDate DATE = NULL,
	@ExpiredDate DATE = NULL,
	@FilePath NVARCHAR(MAX) = NULL,
	@Remarks NVARCHAR(500) = NULL,
	@EntryBy nvarchar(80) = NULL
)
AS
BEGIN
	IF @DocumentTypeId IS NOT NULL AND @DocumentTypeId > 0 AND @VehicleId IS NOT NULL AND @VehicleId > 0 
	BEGIN
		DECLARE @newId BIGINT = 0;
		SELECT @newId = (ISNULL(MAX(RecordId),0) + 1) FROM Vehicle.Documents
		IF @newId > 0
		BEGIN
			INSERT INTO Vehicle.Documents
			(
				RecordId, DocumentTypeId, VehicleId, IssueDate, ExpiredDate, 
				FilePath, Remarks, IsActive, EntryBy, EntryDate
			)
			VALUES
			(
				@newId, @DocumentTypeId, @VehicleId, @IssueDate, @ExpiredDate,
				@FilePath, @Remarks, 1, @EntryBy, GETDATE()
			)
		END
	END
END
GO
ALTER PROC dbo.sp_GetAllSupplierProfile
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
		ISNULL(TRIM(AccountNo), '') AS AccountNo,
		ISNULL(RoutingNo, '') AS RoutingNo
	FROM dbo.SupplierProfile 
	WHERE [Description] IS NOT NULL 
	ORDER BY [Description] ASC
END
GO
CREATE PROCEDURE Vehicle.GetVehicleDocuments
AS
BEGIN
	SELECT 
	d.RecordId, d.DocumentTypeId, T.DocumentTypeName,
	d.VehicleId, v.VehicleName, d.IssueDate, d.ExpiredDate,
	d.FilePath, d.Remarks, d.IsActive, d.EntryBy, d.EntryDate, 
	d.ModifyBy, d.ModifyDate
	FROM Vehicle.Documents d
	JOIN Vehicle.DocumentType t ON d.DocumentTypeId = t.RecordId
	JOIN Vehicle.Vehicles v ON d.VehicleId = v.RecordId
	ORDER BY v.VehicleName, t.DocumentTypeName
END
GO
CREATE PROC Vehicle.UpdateDocument
(
	@RecordId BIGINT = NULL,
	@DocumentTypeId INT = NULL,
	@VehicleId BIGINT = NULL,
	@IssueDate DATE = NULL,
	@ExpiredDate DATE = NULL,
	@FilePath NVARCHAR(MAX) = NULL,
	@Remarks NVARCHAR(500) = NULL,
	@ModifyBy nvarchar(80) = NULL
)
AS
BEGIN
	IF @RecordId > 0 
	BEGIN
		Update Vehicle.Documents 
		SET 
		DocumentTypeId = @DocumentTypeId
		, VehicleId = @VehicleId
		, IssueDate = @IssueDate
		, ExpiredDate = @ExpiredDate
		, FilePath = @FilePath
		, Remarks = @Remarks
		, ModifyBy = @ModifyBy
		, ModifyDate = GETDATE()
		WHERE RecordId = @RecordId
	END
END
GO
ALTER PROCEDURE Vehicle.GetVehicleDocuments
AS
BEGIN
	SELECT 
	d.RecordId, d.DocumentTypeId, T.DocumentTypeName,
	d.VehicleId, v.VehicleName, v.IssueLocationId, d.IssueDate, d.ExpiredDate,
	d.FilePath, d.Remarks, d.IsActive, d.EntryBy, d.EntryDate, 
	d.ModifyBy, d.ModifyDate
	FROM Vehicle.Documents d
	JOIN Vehicle.DocumentType t ON d.DocumentTypeId = t.RecordId
	JOIN Vehicle.Vehicles v ON d.VehicleId = v.RecordId
	ORDER BY v.VehicleName, t.DocumentTypeName
END
GO
ALTER TABLE Vehicle.IssueLocation ADD CompanyId INT null
GO
ALTER PROC Vehicle.GetVehicles
AS
BEGIN
	SELECT
	v.RecordId, v.VehicleName, 
	CONVERT(NVARCHAR(20), v.RegistrationDate, 105) RegistrationDate, 
	v.LicensePlate,
	v.OwnerId, ISNULL(o.OwnerName,'') OwnerName, 
	v.IssueLocationId, ISNULL(l.LocationName,'') LocationName, 
	ISNULL(l.CompanyId, 0) CompanyId,
	v.IssueToId, ISNULL(i.ReceiverName,'') ReceiverName, 
	v.VehicleTypeId, ISNULL(t.TypeName,'') TypeName, 
	v.BRTAOfficeId, ISNULL(b.OfficeName,'') OfficeName, 
	v.DriverId, ISNULL(d.DriverName,'') DriverName, 
	v.SeatCapacityWithDriver, v.Remarks, 
	v.IsActive
	FROM Vehicle.Vehicles v
	LEFT JOIN Vehicle.Owners o ON v.OwnerId = o.RecordId
	LEFT JOIN Vehicle.IssueLocation l ON v.IssueLocationId = l.RecordId
	LEFT JOIN Vehicle.IssueTo i ON v.IssueToId = i.RecordId
	LEFT JOIN Vehicle.VehicleType t ON v.VehicleTypeId = t.RecordId
	LEFT JOIN Vehicle.BRTAOffice b ON v.BRTAOfficeId = b.RecordId
	LEFT JOIN Vehicle.Driver d ON v.DriverId = d.RecordId
END