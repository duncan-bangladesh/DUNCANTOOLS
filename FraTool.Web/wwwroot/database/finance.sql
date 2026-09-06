--Need to create a stored procedure to get wages report data from all DLR database based on the given date range.
-- EXEC spGetWagesReportData '2026-06-01', '2026-06-30'
CREATE PROC spGetWagesReportData
(
	@FromDate DATETIME = NULL,
	@ToDate DATETIME = NULL
)
AS
BEGIN
	IF @FromDate IS NOT NULL AND @ToDate IS NOT NULL
	BEGIN
		SELECT 
		SUM(CASE WHEN PTX = 'P' THEN NUM ELSE 0 END) AS PermanentAttendance,
		SUM(CASE WHEN PTX = 'T' THEN NUM ELSE 0 END) AS TemporaryAttendance,
		SUM(ISNULL(DoubleHNum, 0)) AS DoubleHazira,
		(
			SUM(CASE WHEN PTX = 'P' THEN NUM ELSE 0 END) +
			SUM(CASE WHEN PTX = 'T' THEN NUM ELSE 0 END) +
			SUM(ISNULL(DoubleHNum, 0))
		) AS TotalAttendance,
		dlr.ACOD AS SubCode,
		ac.[DESC] AS SubCodeDescription,
		dlr.Catg AS AccountsCategory, ac.Head AS AccountsHead,
		SUM(CASE WHEN PTX = 'P' THEN (dlr.TAKA - dlr.DoubleHTk) ELSE 0 END) AS PermanentAttendanceWages,
		SUM(CASE WHEN PTX = 'T' THEN (dlr.TAKA - dlr.DoubleHTk) ELSE 0 END) AS TemporaryAttendanceWages,
		SUM(ISNULL(DoubleHTk, 0)) AS DoubleHaziraWages, 
		(
			SUM(CASE WHEN PTX = 'P' THEN TAKA ELSE 0 END) 
			+ SUM(CASE WHEN PTX = 'T' THEN TAKA ELSE 0 END)
		) AS TotalAttendanceWages
		FROM tblDLR dlr
		JOIN tblDLRAC ac ON dlr.ACOD = ac.REF AND ac.LAVEL = 2
		WHERE VDAT BETWEEN @FromDate AND @ToDate
		GROUP BY dlr.ACOD, ac.[DESC], dlr.Catg, ac.Head
		ORDER BY dlr.Catg DESC, dlr.ACOD ASC
	END
END
GO
ALTER PROC spGetWagesReportData
(
	@FromDate DATETIME = NULL,
	@ToDate DATETIME = NULL
)
AS
BEGIN	
	IF @FromDate IS NOT NULL AND @ToDate IS NOT NULL
	BEGIN
		DECLARE 
		@YTDStartDate DATETIME,
		@LastYearStartDate DATETIME,
		@LastYearToDate DATETIME;
		SET @YTDStartDate = DATEFROMPARTS(YEAR(@FromDate), 1, 1);
		SET @LastYearStartDate = DATEFROMPARTS(YEAR(@ToDate) - 1, 1, 1);
		SET @LastYearToDate = DATEADD(YEAR, -1, @ToDate);

		SELECT
            SUM(CASE WHEN VDAT BETWEEN @FromDate AND @ToDate AND PTX = 'P' THEN ISNULL(NUM, 0) ELSE 0 END) AS PermanentAttendance,
            SUM(CASE WHEN VDAT BETWEEN @FromDate AND @ToDate AND PTX = 'T' THEN ISNULL(NUM, 0) ELSE 0 END) AS TemporaryAttendance,
            SUM(CASE WHEN VDAT BETWEEN @FromDate AND @ToDate THEN ISNULL(DoubleHNum, 0) ELSE 0 END) AS DoubleHazira,
            (
                SUM(CASE WHEN VDAT BETWEEN @FromDate AND @ToDate AND PTX = 'P' THEN ISNULL(NUM, 0) ELSE 0 END)
                + SUM(CASE WHEN VDAT BETWEEN @FromDate AND @ToDate AND PTX = 'T' THEN ISNULL(NUM, 0) ELSE 0 END)
                + SUM(CASE WHEN VDAT BETWEEN @FromDate AND @ToDate THEN ISNULL(DoubleHNum, 0) ELSE 0 END)
            ) AS TotalAttendance,
            (
                SUM(CASE WHEN VDAT BETWEEN @YTDStartDate AND @ToDate AND PTX = 'P' THEN ISNULL(NUM, 0) ELSE 0 END)
                + SUM(CASE WHEN VDAT BETWEEN @YTDStartDate AND @ToDate AND PTX = 'T' THEN ISNULL(NUM, 0) ELSE 0 END)
                + SUM(CASE WHEN VDAT BETWEEN @YTDStartDate AND @ToDate THEN ISNULL(DoubleHNum, 0) ELSE 0 END)
            ) 
            -
            (
                SUM(CASE WHEN VDAT BETWEEN @FromDate AND @ToDate AND PTX = 'P' THEN ISNULL(NUM, 0) ELSE 0 END)
                + SUM(CASE WHEN VDAT BETWEEN @FromDate AND @ToDate AND PTX = 'T' THEN ISNULL(NUM, 0) ELSE 0 END)
                + SUM(CASE WHEN VDAT BETWEEN @FromDate AND @ToDate THEN ISNULL(DoubleHNum, 0) ELSE 0 END)
            ) 
            AS YTDPreviousMonth
            ,
            (
                SUM(CASE WHEN VDAT BETWEEN @YTDStartDate AND @ToDate AND PTX = 'P' THEN ISNULL(NUM, 0) ELSE 0 END)
                + SUM(CASE WHEN VDAT BETWEEN @YTDStartDate AND @ToDate AND PTX = 'T' THEN ISNULL(NUM, 0) ELSE 0 END)
                + SUM(CASE WHEN VDAT BETWEEN @YTDStartDate AND @ToDate THEN ISNULL(DoubleHNum, 0) ELSE 0 END)
            ) AS YTDAttendanceThisYear,
            (
                SUM(CASE WHEN VDAT BETWEEN @LastYearStartDate AND @LastYearToDate AND PTX = 'P' THEN ISNULL(NUM, 0) ELSE 0 END)
                + SUM(CASE WHEN VDAT BETWEEN @LastYearStartDate AND @LastYearToDate AND PTX = 'T' THEN ISNULL(NUM, 0) ELSE 0 END)
                + SUM(CASE WHEN VDAT BETWEEN @LastYearStartDate AND @LastYearToDate THEN ISNULL(DoubleHNum, 0) ELSE 0 END)
            ) AS YTDAttendanceLastYear,
            dlr.ACOD AS SubCode,
            ac.[DESC] AS SubCodeDescription,
            dlr.Catg AS AccountsCategory,
            CASE 
                WHEN ac.Head = 'ADV' THEN 'Advanced' 
                WHEN ac.Head = 'REV' THEN 'Revenue expenditure' 
                WHEN ac.Head = 'DEV' THEN 'Development Work' 
                WHEN ac.Head = 'WIP' THEN 'Asset Under Construction' 
                WHEN ac.Head = 'YMT' THEN 'Young Tea/Rubber Maintenance' 
            END AS AccountsHead,
            SUM(CASE WHEN VDAT BETWEEN @FromDate AND @ToDate AND PTX = 'P' THEN ISNULL(dlr.TAKA, 0) - ISNULL(dlr.DoubleHTk, 0) ELSE 0 END) AS PermanentAttendanceWages,
            SUM(CASE WHEN VDAT BETWEEN @FromDate AND @ToDate AND PTX = 'T' THEN ISNULL(dlr.TAKA, 0) - ISNULL(dlr.DoubleHTk, 0) ELSE 0 END) AS TemporaryAttendanceWages,
            SUM(CASE WHEN VDAT BETWEEN @FromDate AND @ToDate THEN ISNULL(DoubleHTk, 0) ELSE 0 END) AS DoubleHaziraWages,
            (
                SUM(CASE WHEN VDAT BETWEEN @FromDate AND @ToDate AND PTX = 'P' THEN ISNULL(TAKA, 0) ELSE 0 END)
                + SUM(CASE WHEN VDAT BETWEEN @FromDate AND @ToDate AND PTX = 'T' THEN ISNULL(TAKA, 0) ELSE 0 END)
            ) AS TotalAttendanceWages,
            (
                SUM(CASE WHEN VDAT BETWEEN @YTDStartDate AND @ToDate AND PTX = 'P' THEN ISNULL(TAKA, 0) - ISNULL(DoubleHTk, 0) ELSE 0 END)
                + SUM(CASE WHEN VDAT BETWEEN @YTDStartDate AND @ToDate AND PTX = 'T' THEN ISNULL(TAKA, 0) - ISNULL(DoubleHTk, 0) ELSE 0 END)
                + SUM(CASE WHEN VDAT BETWEEN @YTDStartDate AND @ToDate THEN ISNULL(DoubleHTk, 0) ELSE 0 END)
            ) 
            -
            (
                SUM(CASE WHEN VDAT BETWEEN @FromDate AND @ToDate AND PTX = 'P' THEN ISNULL(TAKA, 0) ELSE 0 END)
                + SUM(CASE WHEN VDAT BETWEEN @FromDate AND @ToDate AND PTX = 'T' THEN ISNULL(TAKA, 0) ELSE 0 END)
            )
            AS YTDWagesPreviousMonth,
            (
                SUM(CASE WHEN VDAT BETWEEN @YTDStartDate AND @ToDate AND PTX = 'P' THEN ISNULL(TAKA, 0) - ISNULL(DoubleHTk, 0) ELSE 0 END)
                + SUM(CASE WHEN VDAT BETWEEN @YTDStartDate AND @ToDate AND PTX = 'T' THEN ISNULL(TAKA, 0) - ISNULL(DoubleHTk, 0) ELSE 0 END)
                + SUM(CASE WHEN VDAT BETWEEN @YTDStartDate AND @ToDate THEN ISNULL(DoubleHTk, 0) ELSE 0 END)
            ) AS YTDWagesThisYear,
            (
                SUM(CASE WHEN VDAT BETWEEN @LastYearStartDate AND @LastYearToDate AND PTX = 'P' THEN ISNULL(TAKA, 0) - ISNULL(DoubleHTk, 0) ELSE 0 END)
                + SUM(CASE WHEN VDAT BETWEEN @LastYearStartDate AND @LastYearToDate AND PTX = 'T' THEN ISNULL(TAKA, 0) - ISNULL(DoubleHTk, 0) ELSE 0 END)
                + SUM(CASE WHEN VDAT BETWEEN @LastYearStartDate AND @LastYearToDate THEN ISNULL(DoubleHTk, 0) ELSE 0 END)
            ) AS YTDWagesLastYear
        FROM tblDLR dlr
        JOIN tblDLRAC ac ON dlr.ACOD = ac.REF AND ac.LAVEL = 2
        AND ac.[DESC] NOT LIKE '%PHC%'
        WHERE VDAT BETWEEN @LastYearStartDate AND @ToDate
        GROUP BY dlr.ACOD, ac.[DESC], dlr.Catg, ac.Head    
        ORDER BY dlr.Catg DESC, dlr.ACOD ASC;
	END
END
GO
USE DUNCAN_TOOLS
GO
CREATE SCHEMA Finance 
GO
CREATE TABLE Finance.COA_DLR_SAGE_Map
(
	RecordId BIGINT PRIMARY KEY,
	SageAccountsId NVARCHAR(50),
	SageAccountsDescription NVARCHAR(250),
	CompanyCode NVARCHAR(20),
	AccountCode NVARCHAR(20),
	TaskCode NVARCHAR(20),
	LocationCode NVARCHAR(20),
	CostCenter NVARCHAR(10),
	AccountsGroupCode NVARCHAR(30),
	AccountsGroupDescription NVARCHAR(250),
	AccountsSubGroupCode NVARCHAR(20),
	TaskType NVARCHAR(20),
	Remarks NVARCHAR(500),
	IsActive INT,
	EntryBy NVARCHAR(80),
	EntryDate DATETIME,
	ModifyBy NVARCHAR(80),
	ModifyDate DATETIME
)
GO
--EXEC Finance.GetGardenWiseAccCodes 'MAZ'
CREATE PROC Finance.GetGardenWiseAccCodes
(
	@LocationCode NVARCHAR(20) = NULL
)
AS
BEGIN
	IF @LocationCode IS NOT NULL
	BEGIN
		SELECT SageAccountsId, SageAccountsDescription, 
		CASE 
			WHEN CostCenter = 'T' THEN 'Tea'
			WHEN CostCenter = 'R' THEN 'Rubber'
			ELSE '' 
		END AS CostCenter,
		LocationCode, AccountsGroupCode, AccountsGroupDescription,
		AccountsSubGroupCode
		FROM Finance.COA_DLR_SAGE_Map 
		WHERE LocationCode = @LocationCode
	END
END

