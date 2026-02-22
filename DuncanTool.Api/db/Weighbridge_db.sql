Use DUNCAN_TOOLS
GO
Create Schema Scale
GO
Create Table Scale.ScaleData(
	RecordId BIGINT PRIMARY KEY,
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
	SourceName NVARCHAR(20),
	EntryDate DATETIME
)
GO
CREATE PROC Scale.InsertScaleData
(
	@SerialNo NVARCHAR(50),
	@VehicleId NVARCHAR(20),
	@VehicleNumber NVARCHAR(150),
	@MaterialId NVARCHAR(20),
	@Material NVARCHAR(250),
	@CustomerId NVARCHAR(20),
	@Customer NVARCHAR(250),
	@Gross NVARCHAR(20),
	@Tare NVARCHAR(20),
	@Net NVARCHAR(20),
	@RealNet NVARCHAR(20),
	@RecordDateTime NVARCHAR(20),
	@SourceName NVARCHAR(20)
)
AS
BEGIN
	DECLARE @Id BIGINT = 0;
	Select @Id = (ISNULL(Max(RecordId),0) + 1) From Scale.ScaleData
	IF @Id > 0
	BEGIN
		INSERT INTO Scale.ScaleData
		(
			RecordId, SerialNo, VehicleId, VehicleNumber, MaterialId, Material, CustomerId, Customer,
			Gross, Tare, Net, RealNet, RecordDateTime, SourceName, EntryDate
		)
		VALUES
		(
			@Id, @SerialNo, @VehicleId, @VehicleNumber, @MaterialId, @Material, @CustomerId, @Customer,
			@Gross, @Tare, @Net, @RealNet, @RecordDateTime, @SourceName, GETDATE()
		)
	END
END
GO
/*
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
        @Url NVARCHAR(500) = 'http://localhost:5067/api/WeighbridgeScale/SaveScaleData/',
        @SerialNo NVARCHAR(50),
        @HttpStatus INT;

    WHILE EXISTS (SELECT 1 FROM @temp)
    BEGIN
        SELECT TOP (1) @SerialNo = SerialNo FROM @temp ORDER BY SerialNo; -- deterministic
        SELECT @JsonBody = (
			SELECT SerialNo, VehicleId, VehicleNumber, MaterialId, Material, CustomerId, Customer, Gross, Tare, Net, RealNet, RecordDateTime, SourceName
            FROM @temp WHERE SerialNo = @SerialNo FOR JSON PATH, WITHOUT_ARRAY_WRAPPER);

        EXEC sp_OACreate 'MSXML2.ServerXMLHTTP', @Object OUT;
        EXEC sp_OAMethod @Object, 'open', NULL, 'POST', @Url, false;
        EXEC sp_OAMethod @Object, 'setRequestHeader', NULL, 'Content-Type', 'application/json';
        EXEC sp_OAMethod @Object, 'send', NULL, @JsonBody;
        EXEC sp_OAGetProperty @Object, 'status', @HttpStatus OUT;
        EXEC sp_OAGetProperty @Object, 'responseText', @ResponseText OUT;
        EXEC sp_OADestroy @Object;

        IF @HttpStatus = 200 --AND @ResponseText LIKE '%success%'
        BEGIN
            UPDATE dbo.zy_jl SET IsSent = 1 WHERE Czbhs = @SerialNo;   -- ✅ FIXED COLUMN
        END

        DELETE FROM @temp WHERE SerialNo = @SerialNo;
    END
END
*/
GO

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
		SELECT @JsonBody = (
			SELECT SerialNo, VehicleId, VehicleNumber, MaterialId, Material, CustomerId, Customer, Gross, Tare, Net, RealNet, RecordDateTime, SourceName
			FROM @temp WHERE SerialNo = @SerialNo FOR JSON PATH, WITHOUT_ARRAY_WRAPPER);

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

