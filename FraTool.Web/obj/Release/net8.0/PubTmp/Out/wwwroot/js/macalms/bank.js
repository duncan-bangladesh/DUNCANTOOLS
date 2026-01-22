$(document).ready(function () {
    initDataTable();
    bindSubmit();
});
function initDataTable() {
    $('#rTable').DataTable({
        ajax: {
            url: '/Macalms/GetBanks',
            dataSrc: ''
        },
        columns: [
            { data: 'bankName' },            
            {
                data: 'isActive',
                className: '',
                render: function (value, type, row, meta) {                    
                    return `<button type="button" class="btn btn1 btn-edit" data-id="${row.recordId}"><i class="fas fa-edit"></i></button>`;
                }
            }
        ],
        //order: [[0, 'asc']],
        pageLength: 30,
        responsive: true,
        lengthChange: false,
        autoWidth: false,
        info: true
    });
    $('#rTable').on('click', '.btn-edit', function () {
        fnEdit($(this).data('id'));
    });
}
function bindSubmit() {
    $('#btnSubmit').on('click', function () {
        let model = {
            RecordId: $('#RecordId').val(),
            BankName: $('#BankName').val()
        };
        if (model.RecordId > 0) {
            updateBank(model);
        } else {
            addBank(model);
        }
    });
}
function addBank(model) {
    $.post('/Macalms/AddBank', { model }, function (data) {
        if (data > 0) {
            toastr.success('Successfully saved.');
            setTimeout(function () {
                $('#rTable').DataTable().ajax.reload();
                resetForm();
            }, 1000);
        }
    });
}
function updateBank(model) {
    $.post('/Macalms/UpdateBank', { model }, function (data) {
        if (data > 0) {
            toastr.success('Successfully updated.');
            setTimeout(function () {
                $('#rTable').DataTable().ajax.reload();
                resetForm();
            }, 1000);
        }
    });
}
function resetForm() {
    if ($('#RecordId').val() > 0) {
        $('#btnSubmit').removeClass('btn-warning').addClass('btn-success').val('Save');
    }
    $('#RecordId').val(0);
    $('#BankName').val('');
}
function fnEdit(recordId) {
    $.get('/Macalms/BankEditView', { RecordId: recordId }, function (data) {
        $('#entry-ui').removeClass('collapsed-card').addClass('card card-info card-outline');
        $('#RecordId').val(data.recordId);
        $('#BankName').val(data.bankName);
        $('#btnSubmit').removeClass('btn-success').addClass('btn-warning').val('Update');
    });
}