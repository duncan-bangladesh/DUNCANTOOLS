USE balance2012
GO
--Step 1: Add a column to existing table
ALTER TABLE dbo.zy_jl Add IsSent int
GO
--Step 2: configure db server to execute HTTP request
EXEC sp_configure 'show advanced options', 1;
RECONFIGURE;
EXEC sp_configure 'Ole Automation Procedures', 1;
RECONFIGURE;
GO
--Step 3: run the script below to create store procedure to call api (make sure to change the garden code for each garden, here is 'ethte')
CREATE PROC sp_CallWeighbridgeDataToApi
AS
BEGIN
	SET NOCOUNT ON;
	DECLARE @temp TABLE (
		SerialNo NVARCHAR(50),
		VehicleId NVARCHAR(20),
		VehicleNumber NVARCHAR(150),
		MaterialId NVARCHAR(20),
		Material NVARCHAR(250),
		CustomerId NVARCHAR(20),
		Customer NVARCHAR(250),
		Gross NVARCHAR(20),
		Tare NVARCHAR(20),
		Net NVARCHAR(20),
		RealNet NVARCHAR(20),
		RecordDateTime NVARCHAR(20),
		SourceName NVARCHAR(20)
	);

	INSERT INTO @temp
	SELECT detail.Czbhs, detail.chbh, vehicle.Chmc, detail.Wzbh, material.Wzmc,
		detail.Khbh, customer.khmc, detail.Mzzl, detail.Pzzl, detail.Jzzl, detail.By12,
		CONVERT(CHAR(16), detail.Mzsj, 120), 'ethte'
	FROM dbo.zy_jl detail
	JOIN dbo.jb_ch vehicle  ON detail.Chbh = vehicle.chbh
	JOIN dbo.jb_wz material ON detail.Wzbh = material.Wzbh
	JOIN dbo.jb_kh customer ON detail.Khbh = customer.khbh
	WHERE detail.IsSent IS NULL;

	DECLARE 
		@Object INT,
		@ResponseText NVARCHAR(MAX),
		@JsonBody NVARCHAR(MAX),
		@Url NVARCHAR(500) = 'http://172.17.5.243:8090/api/WeighbridgeScale/SaveScaleData',
		@SerialNo NVARCHAR(50),
		@HttpStatus INT;

	WHILE EXISTS (SELECT 1 FROM @temp)
	BEGIN
		SELECT TOP (1) @SerialNo = SerialNo FROM @temp ORDER BY SerialNo;
		SELECT @JsonBody =
			'{' +
			'"SerialNo":"' + ISNULL(REPLACE(SerialNo,'"','\"'),'') + '",' +
			'"VehicleId":"' + ISNULL(REPLACE(VehicleId,'"','\"'),'') + '",' +
			'"VehicleNumber":"' + ISNULL(REPLACE(VehicleNumber,'"','\"'),'') + '",' +
			'"MaterialId":"' + ISNULL(REPLACE(MaterialId,'"','\"'),'') + '",' +
			'"Material":"' + ISNULL(REPLACE(Material,'"','\"'),'') + '",' +
			'"CustomerId":"' + ISNULL(REPLACE(CustomerId,'"','\"'),'') + '",' +
			'"Customer":"' + ISNULL(REPLACE(Customer,'"','\"'),'') + '",' +
			'"Gross":"' + ISNULL(REPLACE(Gross,'"','\"'),'') + '",' +
			'"Tare":"' + ISNULL(REPLACE(Tare,'"','\"'),'') + '",' +
			'"Net":"' + ISNULL(REPLACE(Net,'"','\"'),'') + '",' +
			'"RealNet":"' + ISNULL(REPLACE(RealNet,'"','\"'),'') + '",' +
			'"RecordDateTime":"' + ISNULL(REPLACE(RecordDateTime,'"','\"'),'') + '",' +
			'"SourceName":"' + ISNULL(REPLACE(SourceName,'"','\"'),'') + '"' +
			'}'
		FROM @temp
		WHERE SerialNo = @SerialNo;


		EXEC sp_OACreate 'MSXML2.ServerXMLHTTP', @Object OUT;
		EXEC sp_OAMethod @Object, 'open', NULL, 'POST', @Url, false;
		EXEC sp_OAMethod @Object, 'setRequestHeader', NULL, 'Content-Type', 'application/json';
		EXEC sp_OAMethod @Object, 'send', NULL, @JsonBody;
		EXEC sp_OAGetProperty @Object, 'status', @HttpStatus OUT;
		EXEC sp_OAGetProperty @Object, 'responseText', @ResponseText OUT;
		EXEC sp_OADestroy @Object;

		IF @HttpStatus = 200
		BEGIN
			UPDATE dbo.zy_jl SET IsSent = 1 WHERE Czbhs = @SerialNo;
		END

		DELETE FROM @temp WHERE SerialNo = @SerialNo;
	END
END


--Step 4: create a recurring job and run : "EXEC dbo.sp_CallWeighbridgeDataToApi" to the 1st step of the JOB
--jb_CallApi_SentScaleData
--stp_CallApi_SentScaleData
--sch_CallApi_SentScaleData
