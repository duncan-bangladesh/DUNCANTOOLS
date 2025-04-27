$(document).ready(function () {
    $('#displayUploadedData').hide();
    var transferData = [];
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
            var jsonData = XLSX.utils.sheet_to_json(worksheet, { header: 1 });
            if (jsonData.length > 0) {
                $('#excelDataTable tbody').empty();
                var rows = '<tbody>';
                var total = 0;
                for (var x = 1; x < jsonData.length; x++) {
                    var value = parseFloat(jsonData[x][6]);
                    total += value;
                    var model = {
                        Id: 0,
                        Year: jsonData[x][1],
                        Month: jsonData[x][2],
                        AccountNo: jsonData[x][3],
                        Description: jsonData[x][4],
                        Crop: jsonData[x][5],
                        Amount: jsonData[x][6]
                    };
                    transferData.push(model);
                    rows += `<tr>
                                <td>${jsonData[x][1]}</td>
                                <td>${jsonData[x][2]}</td>
                                <td>${jsonData[x][3]}</td>
                                <td>${jsonData[x][4]}</td>
                                <td>${jsonData[x][5] != 'NULL' ? jsonData[x][5] : ''}</td>
                                <td style="text-align: right;">${parseFloat(jsonData[x][6]).toFixed(2) }</td>
                            </tr>`;
                }
                console.log(total.toFixed(2));
                if (total.toFixed(2) == -0.00) {
                    total = 0;
                }
                else {
                    total = total;
                }
                rows += `<tr style="background-color: #ebebe0;"><td colspan="5" style="text-align: left; font-weight: bold;">Total</td><td style="text-align: right; font-weight: bold;">${total.toFixed(2)}</td ></tr >`;
                $('#excelDataTable').append(rows);
                console.log(transferData);
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
        if (companyId > 0) {
            $.post('/Transfer/SaveTransferData'
                , { companyId: companyId, transferData: transferData }
                , function (data) {
                    if (data.success == true) {
                        toastr.success(data.message);
                        setTimeout(function () {
                            window.location.reload();
                        }, 2000);
                    }
                }
            );
        }
    });
});