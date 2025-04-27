$(document).ready(function () {
    my_datepicker(VoucherDate);
    my_datepicker(FromDate);
    my_datepicker(ToDate);
    $('#voucherList').hide();
    $('#btnGetVoucher').click(function () {
        var VoucherDate = $("#VoucherDate").val();
        $.post("/Voucher/UpData", { VoucherDate: VoucherDate }, function (data) {
            if (data == 99) {
                toastr.error('FRA voucher may have some incorrect GL/Subsidiary code, please recheck.');
                location.href = '/Voucher/errorlog';
            }
            else {
                if (data == 1) {
                    toastr.success('Successfully received the voucher.');
                    setTimeout(function () {
                        location.reload();
                    }, 2000);
                }
                else {
                    if (data == 2) {
                        toastr.error('You have already received this voucher, Please try for other one.');
                    }
                    else {
                        toastr.error('Something went wrong, Please try again later.');
                    }
                }
            }
        });
    });
    $('#btnSearch').click(function () {
        var FromDate = $("#FromDate").val();
        var ToDate = $("#ToDate").val();
        $.post("/Voucher/SearchVoucher", { FromDate: FromDate, ToDate: ToDate }, function (data) {
            console.log(data);
        });
    });
    $('#journal_voucher').hide();
    $.get('/Voucher/GetVoucherMaster', function (data) {
        if (data.length > 0) {
            $('#voucherList').show();
            $("#mTable tbody").empty();
            var html = '';
            for (var i = 0; i < data.length; i++) {
                if (data[i].isSent == 1) {
                    html += '<tr><td>' + data[i].date + '</td><td>' + toTitleCase(data[i].company) + '</td><td>' + toTitleCase(data[i].estate) + '</td><td>' + toTitleCase(data[i].division) + '</td><td>' + toTitleCase(data[i].description) + '</td><td>' + toTitleCase(data[i].voucher_type)
                        + '</td><td><button type="button" class="btn btn-primary" data-toggle="modal" data-target="#modal-xl" onclick = "LoadJournalVoucher(' + data[i].recordId + ');" >show</button >'
                        + '</td><td><button disabled="disabled" type="button" class="btn btn-default text-success" >confirmed</button ></td></tr>';
                }
                else {
                    html += '<tr><td>' + data[i].date + '</td><td>' + toTitleCase(data[i].company) + '</td><td>' + toTitleCase(data[i].estate) + '</td><td>' + toTitleCase(data[i].division) + '</td><td>' + toTitleCase(data[i].description) + '</td><td>' + toTitleCase(data[i].voucher_type)
                        + '</td><td><button type="button" class="btn btn-primary" data-toggle="modal" data-target="#modal-xl" onclick = "LoadJournalVoucher(' + data[i].recordId + ');" >show</button >'
                        + '</td><td><button type="button" class="btn btn-success" onclick = "ConfirmJournalVoucher(' + data[i].recordId + ');" >sent</button ></td></tr>';
                }
            }
            $("#mTable tbody").append(html);
        }
    });
});
function LoadJournalVoucher(masterId) {
    $.get('/Voucher/GetVoucher', { MasterId: masterId }, function (data) {
        if (data.length > 0) {
            $("#vTable tbody").empty();
            var html = '', total = 0;
            for (var i = 0; i < data.length; i++) {
                total += parseFloat(data[i].amount);
                if (data[i].amount > 0) {
                    html += '<tr><td>' + data[i].account_code + '</td><td>' + toTitleCase(data[i].head_name) + '</td><td>' + toTitleCase(data[i].description) + '</td><td>' + numberWithCommas((Math.round(data[i].amount * 100) / 100).toFixed(2)) + '</td><td></td></tr>';
                }
                else {
                    html += '<tr style="font-weight: bold;"><td>' + data[i].account_code + '</td><td>' + toTitleCase(data[i].head_name) + '</td><td>' + toTitleCase(data[i].description) + '</td><td></td><td></td></tr>';
                }
            }
            html += '<tr><td></td><td></td><td></td><td></td><td>' + numberWithCommas((Math.round(total * 100) / 100).toFixed(2)) + '</td></tr>';
            html += '<tr style="font-weight: bold;"><td>Total</td><td></td><td></td><td>' + numberWithCommas((Math.round(total * 100) / 100).toFixed(2)) + '</td><td>' + numberWithCommas((Math.round(total * 100) / 100).toFixed(2)) + '</td></tr>';
            $("#vTable tbody").append(html);
        }
    });
}
function ConfirmJournalVoucher(masterId) {
    $.post('/Voucher/ConfirmVoucher', { MasterId: masterId }, function (data) {
        if (data == 100) {
            toastr.success('Successfully Sent to the CHARMS database.');
            setTimeout(function () {
                location.reload();
            }, 2000);
        }
    });
}
function toTitleCase(str) {
    return str.replace(
        /\w\S*/g,
        text => text.charAt(0).toUpperCase() + text.substring(1).toLowerCase()
    );
}
function numberWithCommas(x) {
    return x.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
}