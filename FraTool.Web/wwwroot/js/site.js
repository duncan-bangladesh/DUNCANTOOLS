// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$(document).ready(function () {
    $('.select2').select2();
    $('.select2bs4').select2({
        theme: 'bootstrap4'
    });
    $(".number").keypress(function (e) {
        if (e.which < 48 || e.which > 57) {
            return false;
        }
    });
    $(".number-decimal").keypress(function (e) {
        if (e.which != 46 && (e.which < 48 || e.which > 57)) {
            return false;
        }
    });
    $.get('/Accounts/AppName', function (data) {
        $('#appName').text(data);
    });
    $.get('/Menus/MenusByUser', function (data) {
        var html = "";
        for (var i = 0; i < data.length; i++) {
            html += '<li class="nav-item"><a href="#" class="nav-link"><i class="' + data[i].iconTag + '"></i><p> ' + data[i].displayName + '<i class="right fas fa-angle-left"></i></p></a>';
            if (data[i].cMenus.length > 0) {
                html += '<ul class="nav nav-treeview">';
                for (var j = 0; j < data[i].cMenus.length; j++) {
                    html += '<li class="nav-item"><a style="padding-left:35px;" href="/' + data[i].cMenus[j].controllerName + '/' + data[i].cMenus[j].actionName + '" class="nav-link"><i class="' + data[i].cMenus[j].iconTag + '"></i><p> ' + data[i].cMenus[j].displayName + '</p></a></li>';
                }
                html += '</ul>';
            }
            html += '</li>';
        }
        $('#menus').append(html);
    });
    
});
function my_datepicker(picker_id) {
    $(picker_id).datepicker({
        dateFormat: "dd-mm-yy",
        todayHighlight: true,
        changemonth: true,
        changeyear: true,
        showOtherMonths: true,
        selectOtherMonths: true,
        maxDate: 0 
    });
    $(picker_id).datepicker('setDate', 'today');
}

function numberWithCommas(x) {
    return x.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
};