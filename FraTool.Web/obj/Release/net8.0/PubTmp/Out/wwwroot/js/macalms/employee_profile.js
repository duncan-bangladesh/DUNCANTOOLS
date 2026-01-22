$(document).ready(function () {
    initApplicableFrom(ApplicableFrom);
    initApplicableFrom(ApplicableUpto);
    initDataTable();
    loadAllDropdowns();
    bindSubmit();
});
function initApplicableFrom(id) {
    $(id).one('focus', function () {
        init_datepicker(this);
        $(this).datepicker('show');
    });
}
function initDataTable() {
    $('#rTable').DataTable({
        ajax: {
            url: '/Macalms/GetEmployeeProfiles',
            dataSrc: ''
        },
        columns: [
            { data: 'employeeCode' },
            { data: 'employeeName' },
            { data: 'departmentName' },
            { data: 'designationName' },
            { data: 'workingIn' },
            { data: 'emailAddress' },
            { data: 'contactNumber' },
            {
                data: 'isActive',
                className: '',
                render: function (value, type, row, meta) {
                    let switchId = `empSwitch_${meta.row}`;
                    let checked = value == 1 ? 'checked' : '';
                    let switchClass = value == 1 ? 'switch-active' : 'switch-inactive';
                    let editButton = value == 1 ? `<button type="button" class="btn btn1 btn-edit" data-id="${row.recordId}"><i class="fas fa-edit"></i></button>` : '';
                    return `<div class="custom-control custom-switch ${switchClass}"><input type="checkbox" class="custom-control-input emp-status" id="${switchId}" data-id="${row.recordId}" ${checked}><label class="custom-control-label" for="${switchId}"></label>${editButton ? ' | ' + editButton : ''}</div>`;
                }
            }
        ],
        //order: [[0, 'asc']],
        pageLength: 30,
        lengthMenu: [[30, 50, 100, -1], [30, 50, 100, "All"]],
        responsive: true,
        autoWidth: false,
        info: true 
    });
    $('#rTable').on('change', '.emp-status', function () {
        changeStatus($(this).data('id'));
    }).on('click', '.btn-edit', function () {
        fnEdit($(this).data('id'));
    });
}
function loadAllDropdowns() {
    loadDropdown('/Macalms/EmpDepartment', '#DepartmentId', 'departmentName');
    loadDropdown('/Macalms/EmpDesignation', '#DesignationId', 'designationName');
    loadDropdown('/Macalms/EmpWorkLocation', '#WorkingIn', 'locationName');
}
function loadDropdown(url, selector, textField) {
    $.get(url, function (data) {
        let ddl = $(selector);
        ddl.empty().append('<option value="0">--Select--</option>');
        $.each(data, function (_, item) {
            ddl.append(
                $('<option>', {
                    value: item.recordId,
                    text: item[textField]
                })
            );
        });
    });
}
function bindSubmit() {
    $('#btnSubmit').on('click', function () {
        let model = {
            RecordId: $('#RecordId').val(),
            EmployeeCode: $('#EmployeeCode').val(),
            EmployeeName: $('#EmployeeName').val(),
            DepartmentId: $('#DepartmentId').val(),
            DesignationId: $('#DesignationId').val(),
            WorkLocationId: $('#WorkingIn').val(),
            EmailAddress: $('#EmailAddress').val(),
            ContactNumber: $('#ContactNumber').val(),
            ApplicableFrom: $('#ApplicableFrom').val(),
            ApplicableUpto: $('#ApplicableUpto').val()
        };
        if (model.RecordId > 0) {
            updateEmployee(model);
        } else {
            addEmployee(model);
        }
    });
}
function addEmployee(model) {
    $.post('/Macalms/AddEmployeeProfile', { model }, function (data) {
        if (data > 0) {
            toastr.success('Successfully saved.');
            setTimeout(function () {
                $('#rTable').DataTable().ajax.reload();
                resetForm();
            }, 1000);
        }
    });
}
function updateEmployee(model) {
    $.post('/Macalms/UpdateEmployeeProfile', { model }, function (data) {
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
    $('#EmployeeCode, #EmployeeName, #EmailAddress, #ContactNumber, #ApplicableFrom, #ApplicableUpto').val('');
    loadAllDropdowns();
}
function changeStatus(recordId) {
    $.post('/Macalms/ChangeEmployeeStatus', { RecordId: recordId }, function (data) {
        if (data > 0) {
            $('#rTable').DataTable().ajax.reload(null, false);
        } else {
            alert('Try again');
        }
    });
}
function fnEdit(recordId) {
    $.get('/Macalms/EmployeeEditView', { RecordId: recordId }, function (data) {
        $('#entry-ui').removeClass('collapsed-card').addClass('card card-info card-outline');
        $('#RecordId').val(data.recordId);
        $('#EmployeeCode').val(data.employeeCode);
        $('#EmployeeName').val(data.employeeName);
        if (data.departmentId != null) {
            $('#DepartmentId').val(data.departmentId).trigger('change');
        }
        if (data.designationId != null) {
            $('#DesignationId').val(data.designationId).trigger('change');
        }
        if (data.workLocationId != null) {
            $('#WorkingIn').val(data.workLocationId).trigger('change');
        }
        $('#EmailAddress').val(data.emailAddress);
        $('#ContactNumber').val(data.contactNumber);
        $('#ApplicableFrom').val(data.applicableFrom);
        $('#ApplicableUpto').val(data.applicableUpto);
        $('#btnSubmit').removeClass('btn-success').addClass('btn-warning').val('Update');
    });
}
