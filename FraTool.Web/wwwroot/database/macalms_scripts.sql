Use FraTool
GO
Create Schema Macalms
GO
Create Table Macalms.Departments(
	RecordId bigint Primary key,
	DepartmentName nvarchar(100),
	IsActive int,
	EntryBy nvarchar(80),
	EntryDate DateTime,
	ModifyBy nvarchar(80),
	ModifyDate DateTime
)
GO
Create Proc Macalms.GetDepartments
As
Begin
	Select * From Macalms.Departments
End
GO
Create Table Macalms.Designation(
	RecordId bigint Primary key,
	DesignationName nvarchar(100),
	IsActive int,
	EntryBy nvarchar(80),
	EntryDate DateTime,
	ModifyBy nvarchar(80),
	ModifyDate DateTime
)
GO
Create Proc Macalms.GetDesignation
As
Begin
	Select * From Macalms.Designation
End
GO
Create Table Macalms.WorkLocation(
	RecordId bigint Primary key,
	LocationName nvarchar(100),
	LocationTag nvarchar(50),
	IsActive int,
	EntryBy nvarchar(80),
	EntryDate DateTime,
	ModifyBy nvarchar(80),
	ModifyDate DateTime
)
GO
Create Proc Macalms.GetWorkLocation
As
Begin
	select * from Macalms.WorkLocation
End
GO
Create Table Macalms.EmployeeProfile(
	RecordId bigint Primary key,
	EmployeeCode nvarchar(20),
	EmployeeName nvarchar(100),
	DepartmentId bigint,
	DesignationId bigint,
	WorkLocationId bigint,
	EmailAddress nvarchar(100),
	ContactNumber nvarchar(20),
	ApplicableFrom nvarchar(20),
	ApplicableUpto nvarchar(20),
	IsActive int,
	EntryBy nvarchar(80),
	EntryDate DateTime,
	ModifyBy nvarchar(80),
	ModifyDate DateTime
)
GO
Create Proc Macalms.AddEmployeeProfile
( 
	@EmployeeCode nvarchar(20) = null,
	@EmployeeName nvarchar(100) = null, 
	@DepartmentId bigint = null, 
	@DesignationId bigint = null, 
	@WorkLocationId bigint = null,
	@EmailAddress nvarchar(100) = null, 
	@ContactNumber nvarchar(20) = null, 
	@ApplicableFrom nvarchar(20) = null, 
	@ApplicableUpto nvarchar(20) = null, 
	@EntryBy nvarchar(80) = null
)
AS
Begin
	Declare @Id Bigint;
	Select @Id = (ISNULL(Max(RecordId),0) + 1) From Macalms.EmployeeProfile
	Insert Into Macalms.EmployeeProfile
	(
		RecordId, EmployeeCode, EmployeeName, DepartmentId, DesignationId, WorkLocationId,
		EmailAddress, ContactNumber, ApplicableFrom, ApplicableUpto, IsActive, EntryBy, EntryDate
	)
	Values
	(
		@Id, @EmployeeCode, @EmployeeName, @DepartmentId, @DesignationId, @WorkLocationId,
		@EmailAddress, @ContactNumber, @ApplicableFrom, @ApplicableUpto, 1, @EntryBy, GETDATE()
	)
End
GO
Create Proc Macalms.UpdateEmployeeProfile
(
	@RecordId bigint = 0,
	@EmployeeCode nvarchar(20) = null,
	@EmployeeName nvarchar(100) = null, 
	@DepartmentId bigint = null, 
	@DesignationId bigint = null, 
	@WorkLocationId bigint = null,
	@EmailAddress nvarchar(100) = null, 
	@ContactNumber nvarchar(20) = null, 
	@ApplicableFrom nvarchar(20) = null, 
	@ApplicableUpto nvarchar(20) = null, 
	@ModifyBy nvarchar(80) = null
)
AS
BEGIN
	IF @RecordId > 0
	BEGIN
		Update Macalms.EmployeeProfile
		Set EmployeeCode = @EmployeeCode, EmployeeName = @EmployeeName, DepartmentId = @DepartmentId, 
		DesignationId = @DesignationId, WorkLocationId = @WorkLocationId, EmailAddress = @EmailAddress,
		ContactNumber = @ContactNumber, ApplicableFrom = @ApplicableFrom, ApplicableUpto = @ApplicableUpto,
		ModifyBy = @ModifyBy, ModifyDate = GETDATE()
		Where RecordId = @RecordId;
	END
END
GO
Create Procedure Macalms.GetEmployeeProfile
AS
BEGIN
	SET NOCOUNT ON;
	SELECT 
	p.RecordId, p.EmployeeCode, p.EmployeeName, p.DepartmentId, d.DepartmentName,
	p.DesignationId, ds.DesignationName, p.WorkLocationId, l.LocationName, 
	p.EmailAddress,	p.ContactNumber, p.ApplicableFrom, p.ApplicableUpto, p.IsActive
	FROM Macalms.EmployeeProfile p
	JOIN Macalms.Departments d ON p.DepartmentId = d.RecordId
	JOIN Macalms.Designation ds ON p.DesignationId = ds.RecordId
	JOIN Macalms.WorkLocation l ON p.WorkLocationId = l.RecordId
END
GO
Create Procedure Macalms.ChangeEmployeeStatus
(
	@RecordId bigint,
	@IsActive int,
	@ModifyBy nvarchar(80)
)
AS
BEGIN
	IF @RecordId is not null
	BEGIN
		Update Macalms.EmployeeProfile
		Set
		IsActive = @IsActive,
		ModifyBy = @ModifyBy,
		ModifyDate = GETDATE()
		Where
		RecordId = @RecordId

		declare @EmployeeCode nvarchar(20);
		Select @EmployeeCode = EmployeeCode From Macalms.EmployeeProfile Where RecordId = RecordId
		Update Macalms.StudentProfile 
		Set IsActive = @IsActive,
		ModifyBy = @ModifyBy,
		ModifyDate = GETDATE()
		Where EmployeeRefCode = @EmployeeCode
	END
END
GO
Create Table Macalms.AllBanks(
	RecordId bigint Primary key,
	BankName nvarchar(250),
	ShortCode nvarchar(20),
	IsActive int,
	EntryBy nvarchar(80),
	EntryDate DateTime,
	ModifyBy nvarchar(80),
	ModifyDate DateTime
)
GO
CREATE PROC Macalms.GetAllBanks
AS
BEGIN
	SET NOCOUNT ON;
	SELECT RecordId, BankName, ShortCode, IsActive FROM Macalms.AllBanks
END
GO
CREATE PROC Macalms.AddBank
(
	@BankName nvarchar(250) = null,
	@EntryBy nvarchar(80) = null
)
AS
BEGIN
	IF @BankName IS NOT NULL
	BEGIN
		DECLARE @Id BIGINT = 0;
		SELECT @Id = (ISNULL(MAX(RecordId),0) + 1) FROM Macalms.AllBanks 
		IF @Id > 0
		BEGIN
			INSERT INTO Macalms.AllBanks (RecordId, BankName, IsActive, EntryBy, EntryDate)
			VALUES (@Id, @BankName, 1, @EntryBy, GETDATE())
		END
	END
END
GO
Create Proc Macalms.UpdateBankName
(
	@RecordId bigint = 0,
	@BankName nvarchar(250) = null, 
	@ModifyBy nvarchar(80) = null
)
AS
BEGIN
	IF @RecordId > 0
	BEGIN
		Update Macalms.AllBanks
		Set BankName = @BankName, ModifyBy = @ModifyBy, ModifyDate = GETDATE()
		Where RecordId = @RecordId;
	END
END
GO
Create Table Macalms.StudentProfile(
	RecordId bigint Primary key,
	ParentId bigint,
	StudentCode nvarchar(20),
	StudentName nvarchar(100),
	DateOfBirth nvarchar(20),
	Gender nvarchar(20),
	BankName nvarchar(100),
	BankAccountNo nvarchar(50),
	BankBranch nvarchar(100),
	BankRoutingNo nvarchar(50),
	EmployeeRefCode nvarchar(20),
	IsActive int,
	EntryBy nvarchar(80),
	EntryDate DateTime,
	ModifyBy nvarchar(80),
	ModifyDate DateTime
)
GO
CREATE PROC Macalms.GetStudentCodeByParentCode
(
	@EmployeeRefCode nvarchar(20) = ''
)
AS
BEGIN
	IF @EmployeeRefCode IS NOT NULL AND @EmployeeRefCode != ''
	BEGIN
		Declare @NoOfRecord Int, @StudentCode NVARCHAR(50) = '';
		Select @NoOfRecord = (ISNULL(COUNT(RecordId),0)+1) From Macalms.StudentProfile Where EmployeeRefCode = @EmployeeRefCode
		If @NoOfRecord > 9
		BEGIN
			SET @StudentCode = @EmployeeRefCode + '-' + CONVERT(NVARCHAR(10), @NoOfRecord)
		END
		ELSE
		BEGIN
			SET @StudentCode = @EmployeeRefCode + '-0' + CONVERT(NVARCHAR(10), @NoOfRecord)
		END
		SELECT @StudentCode AS StudentCode
	END
END
GO
CREATE PROC Macalms.AddStudentProfile
(
	@ParentId BIGINT = 0,
	@StudentCode NVARCHAR(20),
	@StudentName NVARCHAR(100),
	@DateOfBirth NVARCHAR(20),
	@Gender NVARCHAR(20),
	@BankName NVARCHAR(100),
	@BankAccountNo NVARCHAR(50),
	@BankBranch NVARCHAR(100),
	@BankRoutingNo NVARCHAR(50),	
	@EntryBy NVARCHAR(80)
)
AS
BEGIN
	IF @ParentId > 0
	BEGIN
		DECLARE @EmployeeRefCode NVARCHAR(20);
		SELECT TOP 1 @EmployeeRefCode = EmployeeCode FROM Macalms.EmployeeProfile WHERE RecordId = @ParentId
		IF @EmployeeRefCode IS NOT NULL
		BEGIN
			DECLARE @Id BIGINT;
			SELECT @Id = (ISNULL(MAX(RecordId),0) + 1) FROM Macalms.StudentProfile
			INSERT INTO Macalms.StudentProfile
			(
				RecordId, ParentId, StudentCode, StudentName, DateOfBirth, Gender, 
				BankName, BankAccountNo, BankBranch, BankRoutingNo, EmployeeRefCode,
				IsActive, EntryBy, EntryDate
			)
			VALUES
			(
				@Id, @ParentId, @StudentCode, @StudentName, @DateOfBirth, @Gender, @BankName,
				@BankAccountNo, @BankBranch, @BankRoutingNo, @EmployeeRefCode, 1, @EntryBy, GETDATE()
			)
		END
	END
END
GO
CREATE PROC Macalms.UpdateStudentProfile
(
	@RecordId BIGINT = 0,
	@ParentId BIGINT = null,
	@StudentCode NVARCHAR(20) = null,
	@StudentName NVARCHAR(100) = null,
	@DateOfBirth NVARCHAR(20) = null,
	@Gender NVARCHAR(20) = null,
	@BankName NVARCHAR(100) = null,
	@BankAccountNo NVARCHAR(50) = null,
	@BankBranch NVARCHAR(100) = null,
	@BankRoutingNo NVARCHAR(50) = null,	
	@ModifyBy NVARCHAR(80) = null
)
AS
BEGIN
	IF @RecordId > 0
	BEGIN
		Update Macalms.StudentProfile
		SET
		ParentId = @ParentId, StudentCode = @StudentCode, StudentName = @StudentName, DateOfBirth = @DateOfBirth, Gender = @Gender,
		BankName = @BankName, BankAccountNo = @BankAccountNo, BankBranch = @BankBranch, BankRoutingNo = @BankRoutingNo, ModifyBy = @ModifyBy,
		ModifyDate = GETDATE()
		WHERE RecordId = @RecordId
	END
END
GO
CREATE PROC Macalms.GetStudentProfile
AS
BEGIN
	SET NOCOUNT ON;
	SELECT 
	s.RecordId,
	--e.EmployeeCode + ' - ' + e.EmployeeName ParentName,
	s.ParentId,
	e.EmployeeName AS ParentName,
	s.StudentCode, s.StudentName, s.DateOfBirth, s.Gender, 
	b.RecordId AS BankId,
	s.BankName, s.BankAccountNo, s.BankBranch, s.BankRoutingNo, s.IsActive,
	CASE WHEN s.isactive = 0 THEN 'Inactive' ELSE 'Active' END AS ScholarshipStatus,
	s.EmployeeRefCode
	FROM Macalms.StudentProfile s
	JOIN Macalms.EmployeeProfile e ON s.EmployeeRefCode = e.EmployeeCode
	JOIN Macalms.AllBanks b ON s.BankName = b.BankName
END
GO
Create Procedure Macalms.ChangeStudentStatus
(
	@RecordId bigint,
	@IsActive int,
	@ModifyBy nvarchar(80)
)
AS
BEGIN
	IF @RecordId is not null
	BEGIN
		Update Macalms.StudentProfile
		Set
		IsActive = @IsActive,
		ModifyBy = @ModifyBy,
		ModifyDate = GETDATE()
		Where
		RecordId = @RecordId
	END
END
GO
CREATE TABLE Macalms.StudentResults
(
	RecordId bigint Primary key,
	StudentCode nvarchar(20),
	ClassStudied nvarchar(100),
	NameOfTheInstitution nvarchar(200),
	StudyMedium nvarchar(20),
	AcademyType nvarchar(60),
	ExamResult nvarchar(50),
	AssessmentYear nvarchar(50),
	IsActive int,
	EntryBy nvarchar(80),
	EntryDate DateTime,
	ModifyBy nvarchar(80),
	ModifyDate DateTime
)
GO
CREATE PROC Macalms.SaveStudentResult
(
	@StudentId bigint = 0,
	@ClassStudied nvarchar(100) = null,
	@NameOfTheInstitution nvarchar(200) = null,
	@StudyMedium nvarchar(20) = null,
	@AcademyType nvarchar(60) = null,
	@ExamResult nvarchar(50) = null,
	@AssessmentYear nvarchar(50) = null,
	@EntryBy nvarchar(80) = null
)
AS
BEGIN
	IF @StudentId > 0
	BEGIN
		DECLARE @StudentCode NVARCHAR(20);
		SELECT TOP 1 @StudentCode = StudentCode FROM Macalms.StudentProfile WHERE RecordId = @StudentId
		IF @StudentCode IS NOT NULL
		BEGIN
			DECLARE @Id bigint;
			SELECT @Id = (ISNULL(MAX(RecordId),0) + 1) FROM Macalms.StudentResults
			IF @Id > 0
			BEGIN
				INSERT INTO Macalms.StudentResults
				(
					RecordId, StudentCode, ClassStudied, NameOfTheInstitution, StudyMedium, AcademyType, ExamResult, AssessmentYear, IsActive, EntryBy, EntryDate
				)
				VALUES(
					@Id, @StudentCode, @ClassStudied, @NameOfTheInstitution, @StudyMedium, @AcademyType, @ExamResult, @AssessmentYear, 1, @EntryBy, GETDATE()
				)
			END
		END
	END
END
GO
CREATE PROC Macalms.UpdateExamResult
(
	@RecordId bigint = 0,
	@StudentId bigint = 0,
	@ClassStudied nvarchar(100) = null,
	@NameOfTheInstitution nvarchar(200) = null,
	@StudyMedium nvarchar(20) = null,
	@AcademyType nvarchar(60) = null,
	@ExamResult nvarchar(50) = null,
	@AssessmentYear nvarchar(50) = null,
	@ModifyBy nvarchar(80) = null
)
AS
BEGIN
	IF @RecordId > 0
	BEGIN
		DECLARE @StudentCode NVARCHAR(20);
		SELECT TOP 1 @StudentCode = StudentCode FROM Macalms.StudentProfile WHERE RecordId = @StudentId
		IF @StudentCode IS NOT NULL
		BEGIN
			UPDATE Macalms.StudentResults
			SET StudentCode = @StudentCode, ClassStudied = @ClassStudied,
			NameOfTheInstitution = @NameOfTheInstitution, StudyMedium = @StudyMedium,
			AcademyType = @AcademyType, ExamResult = @ExamResult, AssessmentYear = @AssessmentYear,
			ModifyBy = @ModifyBy, ModifyDate = GETDATE()
			Where RecordId = @RecordId
		END
	END
END
GO
CREATE PROC Macalms.GetStudentResults
AS
BEGIN
	SET NOCOUNT ON;
	SELECT r.RecordId, P.ParentId, p.RecordId AS StudentId, r.StudentCode, p.StudentName, 
	r.ClassStudied, r.NameOfTheInstitution, r.StudyMedium, r.AcademyType, 
	r.ExamResult, y.RecordId AS AssessmentYearId, r.AssessmentYear
	FROM Macalms.StudentResults r
	JOIN Macalms.StudentProfile p ON r.StudentCode = p.StudentCode
	JOIN Macalms.AssessmentYears y ON r.AssessmentYear = y.YearName
END
GO
----------------*******************Assessment Years******************--------------------
Create table Macalms.AssessmentYears
(
	RecordId bigint,
	YearName nvarchar(20),
	ShortCode nvarchar(20),
	IsActive int,
	EntryBy nvarchar(80)
)
GO
Insert Into Macalms.AssessmentYears Values(1, '2025', '25', 1, 'admin')
GO
Insert Into Macalms.AssessmentYears Values(1, '2026', '26', 0, 'admin')
GO
Create Procedure Macalms.GetAssessmentYear
AS
Begin
	Select RecordId, YearName, ShortCode From Macalms.AssessmentYears Where IsActive = 1
End
GO
/*
----------------*******************Calculate Eligible Months Example Query******************--------------------
-------Old Script for reference only.-------

DECLARE @AssessmentYear INT = 2025;

DECLARE @YearStart DATE = DATEFROMPARTS(@AssessmentYear, 1, 1);
DECLARE @YearEnd   DATE = DATEFROMPARTS(@AssessmentYear, 12, 31);

declare @EmpDates table
(
    EmployeeCode   NVARCHAR(20),
	EmployeeName NVARCHAR(100),
    ApplicableFrom DATE,
    ApplicableUpto DATE
);
INSERT INTO @EmpDates (EmployeeCode, EmployeeName, ApplicableFrom, ApplicableUpto)
SELECT e.EmployeeCode, e.EmployeeName,
case when e.ApplicableFrom is null or e.ApplicableFrom = '' then TRY_CONVERT(date, getdate(),105) else TRY_CONVERT(DATE, e.ApplicableFrom, 105) end,
case when e.ApplicableUpto is null or e.ApplicableUpto = '' then TRY_CONVERT(date, getdate(),105) else TRY_CONVERT(DATE, e.ApplicableUpto, 105) end
FROM Macalms.EmployeeProfile e;

-- 2. Calculate Eligible Months ONLY using DATEs
SELECT s.StudentName, e.EmployeeName ParentName, s.DateOfBirth, r.StudyMedium,
DATEDIFF(YEAR, TRY_CONVERT(DATE, s.DateOfBirth, 105), @YearEnd) - CASE WHEN DATEADD(YEAR, DATEDIFF(YEAR, TRY_CONVERT(DATE, s.DateOfBirth, 105), @YearEnd), 
	TRY_CONVERT(DATE, s.DateOfBirth, 105)) > @YearEnd THEN 1 ELSE 0 END AS AgeYears,
DATEDIFF(MONTH, DATEADD(YEAR, DATEDIFF(YEAR, TRY_CONVERT(DATE, s.DateOfBirth, 105), @YearEnd), TRY_CONVERT(DATE, s.DateOfBirth, 105)), @YearEnd) % 12 AS AgeMonths,
DATEDIFF(DAY, DATEADD(MONTH, DATEDIFF(MONTH, DATEADD(YEAR, DATEDIFF(YEAR, TRY_CONVERT(DATE, s.DateOfBirth, 105), @YearEnd), 
	TRY_CONVERT(DATE, s.DateOfBirth, 105)), @YearEnd), DATEADD(YEAR, DATEDIFF(YEAR, TRY_CONVERT(DATE, s.DateOfBirth, 105), @YearEnd), 
	TRY_CONVERT(DATE, s.DateOfBirth, 105))), @YearEnd) AS AgeDays,
s.BankName, s.BankBranch, s.BankAccountNo, s.BankRoutingNo, CASE WHEN Calc.StartDate > Calc.EndDate THEN 0 
	ELSE DATEDIFF(MONTH, Calc.StartDate, Calc.EndDate) + 1 END AS EligibleMonths
FROM Macalms.StudentProfile s
JOIN @EmpDates e ON s.EmployeeRefCode = e.EmployeeCode
JOIN Macalms.StudentResults r ON r.StudentCode = s.StudentCode AND r.AssessmentYear = CAST(@AssessmentYear AS NVARCHAR(4)) AND r.IsActive = 1
CROSS APPLY
(
    SELECT StartDate = CASE WHEN ISNULL(e.ApplicableFrom, @YearStart) < @YearStart THEN @YearStart ELSE ISNULL(e.ApplicableFrom, @YearStart) END,
	EndDate = CASE WHEN ISNULL(e.ApplicableUpto, @YearEnd) > @YearEnd THEN @YearEnd ELSE ISNULL(e.ApplicableUpto, @YearEnd) END
) Calc
ORDER BY s.StudentName;


----------------Final Script for the Stored Procedure******************--------------------
DECLARE @AssessmentYear INT = 2025;

DECLARE @YearStart DATE = DATEFROMPARTS(@AssessmentYear, 1, 1);
DECLARE @YearEnd   DATE = DATEFROMPARTS(@AssessmentYear, 12, 31);
-----Temp dataset
DECLARE @EmpDates TABLE
(
    EmployeeCode   NVARCHAR(20),
    EmployeeName   NVARCHAR(100),
    ApplicableFrom DATE,
    ApplicableUpto DATE
);
-----Insert Into Temp
INSERT INTO @EmpDates (EmployeeCode, EmployeeName, ApplicableFrom, ApplicableUpto)
SELECT e.EmployeeCode, e.EmployeeName,
    CASE WHEN e.ApplicableFrom IS NULL OR e.ApplicableFrom = '' THEN CAST(GETDATE() AS DATE) ELSE TRY_CONVERT(DATE, e.ApplicableFrom, 105) END,
    CASE WHEN e.ApplicableUpto IS NULL OR e.ApplicableUpto = '' THEN NULL ELSE TRY_CONVERT(DATE, e.ApplicableUpto, 105) END
FROM Macalms.EmployeeProfile e Where e.IsActive = 1;

-----Select Final Data
SELECT s.StudentName, e.EmployeeName AS ParentName, s.DateOfBirth, r.StudyMedium,
    AgeCalc.AgeYears AS StAgeYears, AgeCalc.AgeMonths AS StAgeMonths, AgeCalc.AgeDays AS StAgeDays, s.BankName, s.BankBranch, s.BankAccountNo, s.BankRoutingNo,

    /* Eligible Months (only if resigned) */
    CASE WHEN Calc.StartDate > Calc.EndDate THEN 0 ELSE DATEDIFF(MONTH, Calc.StartDate, Calc.EndDate) 
		- CASE WHEN DATEADD(MONTH, DATEDIFF(MONTH, Calc.StartDate, Calc.EndDate), Calc.StartDate) > Calc.EndDate THEN 1 ELSE 0 END + 1 END AS EmpEligibleMonths,

    /* Eligible Days (only if resigned) */
    CASE WHEN e.ApplicableUpto IS NULL THEN 0 WHEN Calc.StartDate > Calc.EndDate THEN 0 
		ELSE DATEDIFF(DAY, DATEADD(MONTH, DATEDIFF(MONTH, Calc.StartDate, Calc.EndDate)
		- CASE WHEN DATEADD(MONTH, DATEDIFF(MONTH, Calc.StartDate, Calc.EndDate), Calc.StartDate) > 
		Calc.EndDate THEN 1 ELSE 0 END, Calc.StartDate), Calc.EndDate) + 1 END AS EmpEligibleDays
FROM Macalms.StudentProfile s
JOIN @EmpDates e ON s.EmployeeRefCode = e.EmployeeCode
JOIN Macalms.StudentResults r ON r.StudentCode = s.StudentCode AND r.AssessmentYear = CAST(@AssessmentYear AS NVARCHAR(4)) AND r.IsActive = 1

/* Accurate age calculation for Student */
CROSS APPLY
	(SELECT AgeYears = DATEDIFF(YEAR, TRY_CONVERT(DATE, s.DateOfBirth, 105), @YearEnd) 
		- CASE WHEN DATEADD(YEAR, DATEDIFF(YEAR, TRY_CONVERT(DATE, s.DateOfBirth, 105), @YearEnd), 
		TRY_CONVERT(DATE, s.DateOfBirth, 105)) > @YearEnd THEN 1 ELSE 0 END,
    AgeMonths = DATEDIFF(MONTH, DATEADD(YEAR, DATEDIFF(YEAR, TRY_CONVERT(DATE, s.DateOfBirth, 105), @YearEnd), 
		TRY_CONVERT(DATE, s.DateOfBirth, 105)), @YearEnd) % 12,
    AgeDays = DATEDIFF(DAY, DATEADD(MONTH, DATEDIFF(MONTH, DATEADD(YEAR, DATEDIFF(YEAR, TRY_CONVERT(DATE, s.DateOfBirth, 105), @YearEnd), 
		TRY_CONVERT(DATE, s.DateOfBirth, 105)), @YearEnd), DATEADD(YEAR, DATEDIFF(YEAR, TRY_CONVERT(DATE, s.DateOfBirth, 105), @YearEnd), 
		TRY_CONVERT(DATE, s.DateOfBirth, 105))), @YearEnd)) AgeCalc
CROSS APPLY
	(SELECT StartDate = CASE WHEN e.ApplicableFrom < @YearStart THEN @YearStart ELSE e.ApplicableFrom END,
    EndDate = CASE WHEN e.ApplicableUpto IS NOT NULL AND e.ApplicableUpto < @YearEnd THEN e.ApplicableUpto ELSE @YearEnd END) Calc
WHERE s.IsActive = 1 AND AgeCalc.AgeYears BETWEEN 6 AND 22
ORDER BY s.StudentName;

*/
CREATE PROC Macalms.GetAllEligibleStudent
(
	@AssessmentYear INT = 0
)
AS
BEGIN
	IF @AssessmentYear > 0
	BEGIN
		DECLARE @YearStart DATE = DATEFROMPARTS(@AssessmentYear, 1, 1);
		DECLARE @YearEnd   DATE = DATEFROMPARTS(@AssessmentYear, 12, 31);
		-----Temp dataset
		DECLARE @EmpDates TABLE
		(
			EmployeeCode   NVARCHAR(20),
			EmployeeName   NVARCHAR(100),
			ApplicableFrom DATE,
			ApplicableUpto DATE
		);
		-----Insert Into Temp
		INSERT INTO @EmpDates (EmployeeCode, EmployeeName, ApplicableFrom, ApplicableUpto)
		SELECT e.EmployeeCode, e.EmployeeName,
			CASE WHEN e.ApplicableFrom IS NULL OR e.ApplicableFrom = '' THEN CAST(GETDATE() AS DATE) ELSE TRY_CONVERT(DATE, e.ApplicableFrom, 105) END,
			CASE WHEN e.ApplicableUpto IS NULL OR e.ApplicableUpto = '' THEN NULL ELSE TRY_CONVERT(DATE, e.ApplicableUpto, 105) END
		FROM Macalms.EmployeeProfile e Where e.IsActive = 1;

		-----Select Final Data
		SELECT s.StudentName, e.EmployeeName AS ParentName, s.DateOfBirth, r.StudyMedium,
			AgeCalc.AgeYears AS StAgeYears, AgeCalc.AgeMonths AS StAgeMonths, AgeCalc.AgeDays AS StAgeDays, s.BankName, s.BankBranch, s.BankAccountNo, s.BankRoutingNo,

			/* Eligible Months (only if resigned) */
			CASE WHEN Calc.StartDate > Calc.EndDate THEN 0 ELSE DATEDIFF(MONTH, Calc.StartDate, Calc.EndDate) 
				- CASE WHEN DATEADD(MONTH, DATEDIFF(MONTH, Calc.StartDate, Calc.EndDate), Calc.StartDate) > Calc.EndDate THEN 1 ELSE 0 END + 1 END AS EmpEligibleMonths,

			/* Eligible Days (only if resigned) */
			CASE WHEN e.ApplicableUpto IS NULL THEN 0 WHEN Calc.StartDate > Calc.EndDate THEN 0 
				ELSE DATEDIFF(DAY, DATEADD(MONTH, DATEDIFF(MONTH, Calc.StartDate, Calc.EndDate)
				- CASE WHEN DATEADD(MONTH, DATEDIFF(MONTH, Calc.StartDate, Calc.EndDate), Calc.StartDate) > 
				Calc.EndDate THEN 1 ELSE 0 END, Calc.StartDate), Calc.EndDate) + 1 END AS EmpEligibleDays
		FROM Macalms.StudentProfile s
		JOIN @EmpDates e ON s.EmployeeRefCode = e.EmployeeCode
		JOIN Macalms.StudentResults r ON r.StudentCode = s.StudentCode AND r.AssessmentYear = CAST(@AssessmentYear AS NVARCHAR(4)) AND r.IsActive = 1

		/* Accurate age calculation for Student */
		CROSS APPLY
			(SELECT AgeYears = DATEDIFF(YEAR, TRY_CONVERT(DATE, s.DateOfBirth, 105), @YearEnd) 
				- CASE WHEN DATEADD(YEAR, DATEDIFF(YEAR, TRY_CONVERT(DATE, s.DateOfBirth, 105), @YearEnd), 
				TRY_CONVERT(DATE, s.DateOfBirth, 105)) > @YearEnd THEN 1 ELSE 0 END,
			AgeMonths = DATEDIFF(MONTH, DATEADD(YEAR, DATEDIFF(YEAR, TRY_CONVERT(DATE, s.DateOfBirth, 105), @YearEnd), 
				TRY_CONVERT(DATE, s.DateOfBirth, 105)), @YearEnd) % 12,
			AgeDays = DATEDIFF(DAY, DATEADD(MONTH, DATEDIFF(MONTH, DATEADD(YEAR, DATEDIFF(YEAR, TRY_CONVERT(DATE, s.DateOfBirth, 105), @YearEnd), 
				TRY_CONVERT(DATE, s.DateOfBirth, 105)), @YearEnd), DATEADD(YEAR, DATEDIFF(YEAR, TRY_CONVERT(DATE, s.DateOfBirth, 105), @YearEnd), 
				TRY_CONVERT(DATE, s.DateOfBirth, 105))), @YearEnd)) AgeCalc
		CROSS APPLY
			(SELECT StartDate = CASE WHEN e.ApplicableFrom < @YearStart THEN @YearStart ELSE e.ApplicableFrom END,
			EndDate = CASE WHEN e.ApplicableUpto IS NOT NULL AND e.ApplicableUpto < @YearEnd THEN e.ApplicableUpto ELSE @YearEnd END) Calc
		WHERE s.IsActive = 1 AND AgeCalc.AgeYears BETWEEN 6 AND 22
		ORDER BY s.StudentName;
	END
END
GO




alter PROC Macalms.GetAllEligibleStudent
(
	@AssessmentYear INT = 0
)
AS
BEGIN
	IF @AssessmentYear > 0
	BEGIN
		DECLARE @YearStart DATE = DATEFROMPARTS(@AssessmentYear, 1, 1);
		DECLARE @YearEnd   DATE = DATEFROMPARTS(@AssessmentYear, 12, 31);

		/* Temp Employee Dataset */
		DECLARE @EmpDates TABLE
		(
			EmployeeCode   NVARCHAR(20),
			EmployeeName   NVARCHAR(100),
			ApplicableFrom DATE,
			ApplicableUpto DATE
		);
		INSERT INTO @EmpDates (EmployeeCode, EmployeeName, ApplicableFrom, ApplicableUpto)
		SELECT e.EmployeeCode, e.EmployeeName, TRY_CONVERT(DATE, e.ApplicableFrom, 105), CASE WHEN e.ApplicableUpto IS NULL OR e.ApplicableUpto = '' THEN NULL ELSE TRY_CONVERT(DATE, e.ApplicableUpto, 105) END
		FROM Macalms.EmployeeProfile e WHERE e.IsActive = 1;

		SELECT s.StudentName, e.EmployeeName AS ParentName, s.DateOfBirth, r.StudyMedium, AgeCalc.AgeYears  AS StAgeYears, AgeCalc.AgeMonths AS StAgeMonths, AgeCalc.AgeDays   AS StAgeDays,
			s.BankName, s.BankBranch, s.BankAccountNo, s.BankRoutingNo, ISNULL(e.ApplicableUpto, @YearEnd) AS ApplicableUpto,
			/* FINAL — Correct Month & Day Result */
			M.FullMonths AS EmpEligibleMonths, CASE WHEN M.FullMonths = 12 THEN 0 ELSE DATEDIFF(DAY, DATEADD(MONTH, M.FullMonths, Calc.StartDate), Calc.EndDate) + 1 END AS EmpEligibleDays
		FROM Macalms.StudentProfile s
		JOIN @EmpDates e ON s.EmployeeRefCode = e.EmployeeCode
		JOIN Macalms.StudentResults r ON r.StudentCode = s.StudentCode AND r.AssessmentYear = CAST(@AssessmentYear AS NVARCHAR(4)) AND r.IsActive = 1

		/* Accurate Student Age */
		CROSS APPLY
		(SELECT AgeYears = DATEDIFF(YEAR, TRY_CONVERT(DATE, s.DateOfBirth, 105), @YearEnd)
					- CASE WHEN DATEADD(YEAR, DATEDIFF(YEAR, TRY_CONVERT(DATE, s.DateOfBirth, 105), @YearEnd), TRY_CONVERT(DATE, s.DateOfBirth, 105)) > @YearEnd THEN 1 ELSE 0 END,
				AgeMonths = DATEDIFF(MONTH, DATEADD(YEAR, DATEDIFF(YEAR, TRY_CONVERT(DATE, s.DateOfBirth, 105), @YearEnd), TRY_CONVERT(DATE, s.DateOfBirth, 105)), @YearEnd ) % 12,
				AgeDays = DATEDIFF(DAY,DATEADD(MONTH, DATEDIFF(MONTH, DATEADD(YEAR, DATEDIFF(YEAR, TRY_CONVERT(DATE, s.DateOfBirth, 105), @YearEnd), 
					TRY_CONVERT(DATE, s.DateOfBirth, 105)), @YearEnd), DATEADD(YEAR, DATEDIFF(YEAR, TRY_CONVERT(DATE, s.DateOfBirth, 105), @YearEnd), 
					TRY_CONVERT(DATE, s.DateOfBirth, 105))), @YearEnd)) AgeCalc

		/* Normalize Applicable Period */
		CROSS APPLY
		(SELECT
			StartDate = CASE WHEN e.ApplicableFrom < @YearStart THEN @YearStart ELSE e.ApplicableFrom END,
			EndDate = CASE WHEN e.ApplicableUpto IS NOT NULL 
			--AND e.ApplicableUpto < @YearEnd 
			THEN e.ApplicableUpto ELSE @YearEnd END) Calc

		/* FINAL — Month Calculation (No Overflows) */
		CROSS APPLY (SELECT BaseMonths = DATEDIFF(MONTH, Calc.StartDate, Calc.EndDate)) BM
		CROSS APPLY (SELECT FullMonths = CASE
			/* Not resigned + full year */
			WHEN e.ApplicableUpto IS NULL AND DATEDIFF(DAY, Calc.StartDate, Calc.EndDate) + 1 >= 365 THEN 12
			/* Adjust if adding months overshoots */
			WHEN DATEADD(MONTH, BM.BaseMonths, Calc.StartDate) > Calc.EndDate THEN BM.BaseMonths - 1 ELSE BM.BaseMonths END) M
		WHERE s.IsActive = 1 AND AgeCalc.AgeYears BETWEEN 6 AND 22
		ORDER BY s.StudentName;
	END
END

GO
ALTER PROC Macalms.GetAllEligibleStudent
(
	@AssessmentYear INT = 0
)
AS
BEGIN
	IF @AssessmentYear > 0
	BEGIN
		DECLARE @YearStart DATE = DATEFROMPARTS(@AssessmentYear, 1, 1);
		DECLARE @YearEnd   DATE = DATEFROMPARTS(@AssessmentYear, 12, 31);

		/* Temp Employee Dataset */
		DECLARE @EmpDates TABLE
		(
			EmployeeCode   NVARCHAR(20),
			EmployeeName   NVARCHAR(100),
			ApplicableFrom DATE,
			ApplicableUpto DATE
		);
		INSERT INTO @EmpDates (EmployeeCode, EmployeeName, ApplicableFrom, ApplicableUpto)
		SELECT e.EmployeeCode, e.EmployeeName, TRY_CONVERT(DATE, e.ApplicableFrom, 105), 
		CASE WHEN e.ApplicableUpto IS NULL OR e.ApplicableUpto = '' THEN NULL ELSE TRY_CONVERT(DATE, e.ApplicableUpto, 105) END
		FROM Macalms.EmployeeProfile e WHERE e.IsActive = 1;

		SELECT s.StudentName, e.EmployeeName AS ParentName, 
			--FORMAT(CONVERT(date, s.DateOfBirth, 105), 'dd-MMM-yyyy', 'en-US') DateOfBirth, 
			(SELECT RIGHT('0' + CAST(DAY(d) AS varchar(2)), 2) + '-' + LEFT(DATENAME(month, d), 3) + '-' + RIGHT(CAST(YEAR(d) AS varchar(4)), 2) FROM (SELECT CONVERT(date, s.DateOfBirth, 105) AS d) x) AS DateOfBirth,
			r.StudyMedium, AgeCalc.AgeYears  AS StAgeYears, AgeCalc.AgeMonths AS StAgeMonths, AgeCalc.AgeDays   AS StAgeDays,
			s.BankName, s.BankBranch, s.BankAccountNo, s.BankRoutingNo, ISNULL(e.ApplicableUpto, @YearEnd) AS ApplicableUpto,
			/* FINAL — Correct Month & Day Result */
			M.FullMonths AS EmpEligibleMonths, CASE WHEN M.FullMonths = 12 THEN 0 ELSE DATEDIFF(DAY, DATEADD(MONTH, M.FullMonths, Calc.StartDate), Calc.EndDate) + 1 END AS EmpEligibleDays
		FROM Macalms.StudentProfile s
		JOIN @EmpDates e ON s.EmployeeRefCode = e.EmployeeCode
		JOIN Macalms.StudentResults r ON r.StudentCode = s.StudentCode AND r.AssessmentYear = CAST(@AssessmentYear AS NVARCHAR(4)) AND r.IsActive = 1

		/* Accurate Student Age */
		CROSS APPLY
		(SELECT AgeYears = DATEDIFF(YEAR, TRY_CONVERT(DATE, s.DateOfBirth, 105), @YearEnd)
				- CASE WHEN DATEADD(YEAR, DATEDIFF(YEAR, TRY_CONVERT(DATE, s.DateOfBirth, 105), @YearEnd), TRY_CONVERT(DATE, s.DateOfBirth, 105)) > @YearEnd THEN 1 ELSE 0 END,
				AgeMonths = DATEDIFF(MONTH, DATEADD(YEAR, DATEDIFF(YEAR, TRY_CONVERT(DATE, s.DateOfBirth, 105), @YearEnd), TRY_CONVERT(DATE, s.DateOfBirth, 105)), @YearEnd ) % 12,
				AgeDays = DATEDIFF(DAY,DATEADD(MONTH, DATEDIFF(MONTH, DATEADD(YEAR, DATEDIFF(YEAR, TRY_CONVERT(DATE, s.DateOfBirth, 105), @YearEnd), 
				TRY_CONVERT(DATE, s.DateOfBirth, 105)), @YearEnd), DATEADD(YEAR, DATEDIFF(YEAR, TRY_CONVERT(DATE, s.DateOfBirth, 105), @YearEnd), 
				TRY_CONVERT(DATE, s.DateOfBirth, 105))), @YearEnd)) AgeCalc

		/* Normalize Applicable Period */
		CROSS APPLY
		(SELECT
			StartDate = CASE WHEN e.ApplicableFrom < @YearStart THEN @YearStart ELSE e.ApplicableFrom END,
			EndDate = CASE WHEN e.ApplicableUpto IS NOT NULL 
			--AND e.ApplicableUpto < @YearEnd 
			THEN e.ApplicableUpto ELSE @YearEnd END) Calc

		/* FINAL — Month Calculation (No Overflows) */
		CROSS APPLY (SELECT BaseMonths = DATEDIFF(MONTH, Calc.StartDate, Calc.EndDate)) BM
		CROSS APPLY (SELECT FullMonths = CASE
			/* Not resigned + full year */
			WHEN e.ApplicableUpto IS NULL AND DATEDIFF(DAY, Calc.StartDate, Calc.EndDate) + 1 >= 365 THEN 12
			/* Adjust if adding months overshoots */
			WHEN DATEADD(MONTH, BM.BaseMonths, Calc.StartDate) > Calc.EndDate THEN BM.BaseMonths - 1 ELSE BM.BaseMonths END) M
		WHERE s.IsActive = 1 AND AgeCalc.AgeYears BETWEEN 6 AND 22
		ORDER BY s.StudentName;
	END
END

