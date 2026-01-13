$(document).ready(function () {
    $('#displayUploadedData').hide();
    $('#filtredData').hide();
    var transferData = [];
    $.get('/Epm/GetYears', function (data) {
        if (data.length > 0) {
            $('#ddYear').empty();
            $("#ddYear").append('<option value="0">--Select--</option>');
            for (var i = 0; i < data.length; i++) {
                $('<option/>',
                    {
                        value: data[i].year,
                        html: data[i].year
                    }
                ).appendTo("#ddYear");
            }
        }
    });
    $.get('/Epm/GetMonths', function (data) {
        if (data.length > 0) {
            $('#ddMonth').empty();
            $("#ddMonth").append('<option value="0">--Select--</option>');
            for (var i = 0; i < data.length; i++) {
                $('<option/>',
                    {
                        value: data[i].monthCode,
                        html: data[i].monthName
                    }
                ).appendTo("#ddMonth");
            }
        }
    });
    LoadCompany();
    $.get('/Company/GetTranCompanies', function (data) {
        if (data.length > 0) {
            $('#ddCompanyId').empty();
            $("#ddCompanyId").append('<option value="0">--Select--</option>');
            for (var i = 0; i < data.length; i++) {
                $('<option/>',
                    {
                        value: data[i].companyId,
                        html: data[i].companyName
                    }
                ).appendTo("#ddCompanyId");
            }
        }
    });
    $('#CompanyId').change(function () {
        var companyId = $('#CompanyId').val();
        if (companyId > 0) {
            $('#excelFile').prop('disabled', false);
        }
        else {
            $('#excelFile').prop('disabled', true);
            $('#excelDataTable tbody').empty();
            $('#displayUploadedData').hide();

        }
    });
    $('#excelFile').change(function (e) {
        var reader = new FileReader();
        reader.readAsArrayBuffer(e.target.files[0]);
        reader.onload = function (e) {
            var data = new Uint8Array(reader.result);
            var workbook = XLSX.read(data, { type: 'array' });
            var sheetName = workbook.SheetNames[0];
            var worksheet = workbook.Sheets[sheetName];
            var jsonData = XLSX.utils.sheet_to_json(worksheet, { header: 1, defval: "" });
            transferData = jsonData;
            if (jsonData.length > 0) {
                $('#excelDataTable tbody').empty();
                var rows = '<tbody>';
                var total = 0;
                for (var x = 1; x < jsonData.length; x++) {
                    var chAmount;
                    if (jsonData[x][5] == 'NULL' || jsonData[x][5] == null || jsonData[x][5] == '') {
                        chAmount = 0;
                    }
                    else {
                        chAmount = jsonData[x][5];
                    }
                    var value = parseFloat(chAmount);
                    total += value;
                    rows += `<tr>
                                <td>${jsonData[x][0]}</td>
                                <td>${jsonData[x][1]}</td>
                                <td>${jsonData[x][2]}</td>
                                <td>${jsonData[x][3]}</td>
                                <td>${jsonData[x][4] != 'NULL' ? jsonData[x][4] : ''}</td>
                                <td style="text-align: right;">${parseFloat(chAmount).toFixed(2) }</td>
                            </tr>`;
                }
                if (total.toFixed(2) == -0.00) {
                    total = 0;
                }
                else {
                    total = total;
                }
                rows += `<tr style="background-color: #ebebe0;"><td colspan="5" style="text-align: left; font-weight: bold;">Total</td><td style="text-align: right; font-weight: bold;">${total.toFixed(2)}</td ></tr >`;
                $('#excelDataTable').append(rows);
                $('#displayUploadedData').show();
            }
            else {
                $('#excelDataTable tbody').empty();
                $('#displayUploadedData').hide();
            }
        };
    });
    $('#btnSave').click(function () {
        var companyId = $('#CompanyId').val();
        if (transferData.length === 0) {
            toastr.error("No data to send.");
            return;
        }
        if (companyId > 0) {
            let jsonData = [];
            let headers = transferData[0];

            for (let i = 1; i < transferData.length; i++) {
                let rowObj = {};
                for (let j = 0; j < headers.length; j++) {
                    rowObj[headers[j]] = transferData[i][j];
                }
                jsonData.push(rowObj);
            }
            $.ajax({
                url: '/Epm/UploadExcelData',
                type: 'POST',
                contentType: 'application/json',
                data: JSON.stringify({
                    companyId: companyId,
                    excelRows: jsonData // your Excel data array
                }),
                success: function (response) {
                    if (response.success) {
                        setTimeout(function () {
                            toastr.success("Data uploaded successfully.");
                            $('#excelFile').prop('disabled', true);
                            $('#excelDataTable tbody').empty();
                            $('#displayUploadedData').hide();
                            $('#excelFile').val('');
                            LoadCompany();
                            transferData = [];
                        }, 2000);                        
                    } else {
                        toastr.error("Error saving data.");
                    }
                },
                error: function (xhr, status, error) {
                    console.error(error);
                }
            });
        }
    });
    $("#btnSearch").click(function () {
        var status = false, companyId, year, month;
        
        if ($('#ddCompanyId').val() > 0) {
            status = true;
            companyId = $('#ddCompanyId').val();
        }
        else {
            status = false;
            toastr.error("Please select a company.");
        }
        if (status == true) {
            if ($('#ddYear').val() > 0) {
                status = true;
                year = $('#ddYear').val();
            }
            else {
                status = false;
                toastr.error("Please select a year.");
            }
        }
        if (status == true) {
            if ($('#ddMonth').val() == 0) {
                status = false;
                toastr.error("Please select a month.");                
            }
            else {
                status = true;
                month = $('#ddMonth').val();
            }
        }
        if (status == true) {
            $.get("/Epm/Search"
                , { companyId: companyId, year: year, month: month }
                , function (data) {
                    if (data.length > 0) {
                        $('#displayTable tbody').empty();
                        var rows = '<tbody>';
                        var total = 0;
                        for (var x = 0; x < data.length; x++) {
                            var value = parseFloat(data[x].amount);
                            total += value;
                            
                            rows += `<tr>
                                <td>${data[x].year}</td>
                                <td>${data[x].month}</td>
                                <td>${data[x].accountNo}</td>
                                <td>${data[x].description}</td>
                                <td>${data[x].crop != 'NULL' ? data[x].crop : ''}</td>
                                <td style="text-align: right;">${parseFloat(data[x].amount).toFixed(2)}</td>
                            </tr>`;
                        }
                        if (total.toFixed(2) == -0.00) {
                            total = 0;
                        }
                        else {
                            total = total;
                        }
                        rows += `<tr style="background-color: #ebebe0;"><td colspan="5" style="text-align: left; font-weight: bold;">Total</td><td style="text-align: right; font-weight: bold;">${total.toFixed(2)}</td ></tr >`;
                        $('#displayTable').append(rows);
                        $('#filtredData').show();
                    }
                    else {
                        $('#displayTable tbody').empty();
                        $('#filtredData').hide();
                    }
                    
                }
            );
        }
    });
});
function LoadCompany() {
    $.get('/Company/GetTranCompanies', function (data) {
        if (data.length > 0) {
            $('#CompanyId').empty();
            $("#CompanyId").append('<option value="0">--Select--</option>');
            for (var i = 0; i < data.length; i++) {
                $('<option/>',
                    {
                        value: data[i].companyId,
                        html: data[i].companyName
                    }
                ).appendTo("#CompanyId");
            }
        }
    });
}