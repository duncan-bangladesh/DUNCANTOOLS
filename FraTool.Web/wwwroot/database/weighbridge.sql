USE DUNCAN_TOOLS
GO
--EXEC Scale.FilterScaleData NULL, '2026-07-01', '2026-07-31'
CREATE PROC Scale.FilterScaleData
( 
	@EstateCode NVARCHAR(20) = NULL, 
	@FromDate NVARCHAR(20) = NULL, 
	@ToDate NVARCHAR(20) = NULL
)
AS
BEGIN
	SELECT 
		CONVERT(NVARCHAR(20), CAST(RecordDateTime AS DATE)) AS [RecordDate],
		FORMAT( CAST(RecordDateTime AS DATETIME), 'hh:mm tt') AS [RecordTime],
		VehicleNumber AS [Vehicle],
		TRY_CONVERT(DECIMAL(18,0),Gross) AS [LoadedVehicle],
		TRY_CONVERT(DECIMAL(18,0),Tare) AS [TareWeight],
		TRY_CONVERT(DECIMAL(18,0),RealNet) AS [NetWeight],
		c.CompanyName AS TeaEstate
	FROM Scale.ScaleData d
	JOIN Shared.Company c ON d.SourceName = c.EstateCodeForScale
	WHERE 
		Material IN ('GREEN LEAF') 
		AND VehicleNumber NOT IN('TEST') 
		--AND SourceName = @EstateCode
		AND (@EstateCode IS NULL OR d.SourceName = @EstateCode)
		AND CAST(d.RecordDateTime AS DATETIME) BETWEEN
			DATEADD(DAY, DATEDIFF(DAY, '19000101', @FromDate), '19000101')
			AND DATEADD(DAY, DATEDIFF(DAY, '18991231', @ToDate), '19000101')
	ORDER BY CAST(RecordDateTime AS DATE), VehicleNumber;
END
GO
ALTER PROC [Scale].[FilterScaleData]
( 
	@EstateCode NVARCHAR(20) = NULL, 
	@FromDate NVARCHAR(20) = NULL, 
	@ToDate NVARCHAR(20) = NULL
)
AS
BEGIN
	SELECT 
		CONVERT(NVARCHAR(20), CAST(RecordDateTime AS DATE)) AS [RecordDate],
		FORMAT( CAST(RecordDateTime AS DATETIME), 'hh:mm tt') AS [RecordTime],
		VehicleNumber AS [Vehicle],
		TRY_CONVERT(DECIMAL(18,0),Gross) AS [LoadedVehicle],
		TRY_CONVERT(DECIMAL(18,0),Tare) AS [TareWeight],
		TRY_CONVERT(DECIMAL(18,0),RealNet) AS [NetWeight],
		c.CompanyName AS TeaEstate
	FROM Scale.ScaleData d
	JOIN Shared.Company c ON d.SourceName = c.EstateCodeForScale
	WHERE 
		Material IN ('GREEN LEAF', 'GL') 
		AND VehicleNumber NOT IN('TEST') 
		--AND SourceName = @EstateCode
		AND (@EstateCode IS NULL OR d.SourceName = @EstateCode)
		AND CAST(d.RecordDateTime AS DATETIME) BETWEEN
			DATEADD(DAY, DATEDIFF(DAY, '19000101', @FromDate), '19000101')
			AND DATEADD(DAY, DATEDIFF(DAY, '18991231', @ToDate), '19000101')
	ORDER BY CAST(RecordDateTime AS DATE)
	, VehicleNumber
	, FORMAT( CAST(RecordDateTime AS DATETIME), 'hh:mm tt')
	, c.CompanyName;
END