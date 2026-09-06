var eTaxGroup = null;
$(document).ready(function () {    
    $('#BIN').on('keypress', function (event) {
        var value = $(this).val();
        // Allow control keys
        if (event.which === 8 || event.which === 0) return;
        // Allow digits
        if (event.which >= 48 && event.which <= 57) return;
        // Allow only one hyphen
        if (event.which === 45) {
            if (value.indexOf('-') !== -1) {
                event.preventDefault();
            }
            return;
        }
        event.preventDefault();
    });
    $('#Taxgroup').change(function () {
        var selectedTaxGroup = $(this).val();
        if (eTaxGroup !== null) {
            if (selectedTaxGroup == eTaxGroup) {
                $('#btnSave').prop('disabled', false);
                $('#btnSave').fadeIn();  // Shows smoothly
            }
            else {
                isServerCall = 'Yes';
                var eSupplierName = $('#SupplierName').val();
                $.get('/Procurement/IsBillExistForThisFinancialYear', { SupplierName: eSupplierName }, function (data) {
                    if (data > 0) {
                        toastr.error('You can not change TAX Group for this financial year, Supplier already in another TAX Group.');
                        $('#btnSave').prop('disabled', true);
                        $('#btnSave').fadeOut(); // Hides smoothly
                    }
                });
            }
        }
    });
    initDataTable();
    $('#SupplierName').on('change', function () {
        var supplierName = $(this).val();
        if (supplierName) {
            var isValid = true;
            $.ajax({
                url: '/Procurement/CheckSupplierName',
                type: 'GET',
                data: { SupplierName: supplierName },
                success: function (data) {
                    if (data > 0) {
                        toastr.error("Supplier name already exists.");
                        $('#SupplierName').addClass("input-error").focus();
                        $('#SupplierCode').val('');
                        isValid = false;
                    }
                },
                error: function () {
                    console.error('Error checking supplier name');
                }
            });

            if (!isValid) return;

            $.ajax({
                url: '/Procurement/GetNewSupplierCode',
                type: 'GET',
                data: { SupplierName: supplierName },
                success: function (data) {
                    $('#SupplierCode').val(data);
                },
                error: function () {
                    console.error('Error fetching supplier code');
                }
            });
        } else {
            $('#SupplierCode').val('');
        }
    });
    $('#btnSave').on('click', function () {
        var isValid = true;
        $(".input-error").removeClass("input-error");
        function validateField(selector, message) {
            var value = $(selector).val().trim();
            if (value === '') {
                toastr.error(message);
                $(selector).addClass("input-error").focus();
                isValid = false;
                return false;
            }
            return true;
        }

        // Validation (stop at first error)
        if (!validateField('#SupplierName', 'Please input a Supplier Name')) return;
        if ($('#SupplierCode').val().trim() === '') {
            toastr.error('Please input a valid Supplier.');
            isValid = false;
            return false;
        };
        if (!validateField('#Taxgroup', 'Please input Tax Group')) return;
        if (!validateField('#TIN', 'Please input a TIN no.')) return;
        if (!validateField('#BIN', 'Please input a BIN no.')) return;

        if (!isValid) return;

        var model = {
            SLNo: parseInt($('#SLNo').val() || '0'),
            Code: $('#SupplierCode').val(),
            Description: $('#SupplierName').val(),
            Address: $('#Address').val(),
            City: $('#City').val(),
            Country: $('#Country').val(),
            Bank: $('#Bank').val(),
            AccountNo: $('#AccountNo').val(),
            RoutingNo: $('#RoutingNo').val(),
            Taxgroup: $('#Taxgroup').val(),
            TIN: $('#TIN').val(),
            BIN: $('#BIN').val(),
            Phone: $('#Phone').val(),
            email: $('#Email').val()
        };

        if (model.SLNo > 0) {
            $.post('/Procurement/UpdateSupplierProfile', { model: model }, function (data) {
                if (data > 0) {                    
                    toastr.success("Data updated successfully.");
                    setTimeout(function () {
                        $("#entry-ui").removeClass('card card-info card-outline').addClass('card card-info card-outline collapsed-card');
                        $('#rTable').DataTable().ajax.reload();
                        $('#SLNo').val(0);
                        resetForm();
                        $("#btnSave").removeClass('btn-warning').addClass('btn-success').prop("value", "Save");
                        $('#btntool').html('<i class="fas fa-plus"></i>&nbsp;&nbsp; Add New Supplier.');
                    }, 1000);                        
                } else {
                    toastr.error("Something went wrong, Please try again later.");
                }
            });
        } else {
            $.post('/Procurement/SaveSupplierProfile', { model: model }, function (data) {
                if (data > 0) {                    
                    toastr.success("Data saved successfully.");
                    setTimeout(function () {
                        $("#entry-ui").removeClass('card card-info card-outline').addClass('card card-info card-outline collapsed-card');
                        $('#rTable').DataTable().ajax.reload();
                        $('#SLNo').val(0);
                        resetForm();
                        $("#btnSave").removeClass('btn-warning').addClass('btn-success').prop("value", "Save");
                        $('#btntool').html('<i class="fas fa-plus"></i>&nbsp;&nbsp; Add New Supplier.');
                    }, 1000);
                }
                else {
                    toastr.error("Something went wrong, Please try again later.");
                }
            });
        }
    });
    $(document).on("input change", "input", function () {
        $(this).removeClass("input-error");
    });
    function resetForm() {
        // Clear all input, textarea, select
        eTaxGroup = null;
        $('#SupplierCode, #SupplierName, #Address, #City, #Country, #Bank, #AccountNo, #RoutingNo, #Taxgroup, #TIN, #BIN, #Phone, #Email').val('');
        $('#SLNo').val('0');
        $('.input-error').removeClass('input-error');
    }
});
function initDataTable() {
    if ($.fn.DataTable.isDataTable('#rTable')) {
        $('#rTable').DataTable().destroy();
        $('#rTable tbody').empty(); // Important when reloading columns
    }
    $('#rTable').DataTable({
        ajax: {
            url: '/Procurement/GetSupplierProfileList',
            dataSrc: ''
        },
        columns: [
            { data: 'code' },
            { data: 'description' },
            { data: 'taxgroup' },
            { data: 'tin' },
            { data: 'bin' },
            { data: 'city' },            
            { data: 'phone' },
            {
                data: 'slNo',
                className: '',
                render: function (value, type, row, meta) {
                    return `<button type="button" class="btn btn1 btn-edit" data-id="${row.slNo}"><i class="fas fa-edit"></i></button>`;
                }
            }
        ],
        info: true,
        order: [[1, "asc"]],
        pageLength: 30,
        lengthMenu: [[30, 50, 100, -1], [30, 50, 100, "All"]],
        responsive: false,
        autoWidth: false
    });
    $('#rTable').on('click', '.btn-edit', function () {
        fnEdit($(this).data('id'));
    });
}
function fnEdit(recordId) {
    $.get('/Procurement/GetSupplierProfileBySlNo', { SlNo: recordId }, function (data) {
        eTaxGroup = data[0].taxgroup;
        $("#entry-ui").removeClass('card card-info card-outline collapsed-card').addClass('card card-info card-outline');
        $('#SLNo').val(data[0].slNo);
        $('#SupplierCode').val(data[0].code);
        $('#SupplierName').val(data[0].description);
        $('#Address').val(data[0].address);
        $('#City').val(data[0].city);
        $('#Country').val(data[0].country);
        $('#Bank').val(data[0].bank);
        $('#AccountNo').val(data[0].accountNo);
        $('#RoutingNo').val(data[0].routingNo);
        $('#Taxgroup').val(data[0].taxgroup);
        $('#TIN').val(data[0].tin);
        $('#BIN').val(data[0].bin);
        $('#Phone').val(data[0].phone);
        $('#Email').val(data[0].email);        
        $("#btnSave").removeClass('btn-success').addClass('btn-warning').prop("value", "Update");
        $('#btntool').html('<i class="fas fa-minus"></i>&nbsp;&nbsp; Update Supplier Profile.');
    });
}