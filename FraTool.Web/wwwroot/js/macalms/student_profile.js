let isEditMode = false;
$(document).ready(function () {
    initDataTable();
    initDateOfBirth();
    loadAllDropdowns();
    $('#EmployeeRefId').change(function () {
        if (isEditMode) return; 
        let ParentId = $(this).val();
        if (!ParentId || ParentId == 0) return;
        $.get('/Macalms/GetStudentCodeByParentCode', { ParentId: ParentId }, function (data) {
            $('#StudentCode').val(data.studentCode);
        });
    });
    bindSubmit();
});
function initDataTable() {
    $('#rTable').DataTable({
        ajax: {
            url: '/Macalms/GetStudentProfiles',
            dataSrc: ''
        },
        columns: [
            { data: 'parentName' },
            //{
            //    data: null,
            //    render: function (data, type, row) {
            //        return row.studentCode + ' ' + row.studentName;
            //    },
            //},
            { data: 'studentName' },
            { data: 'dateOfBirth' },
            {
                data: null,
                className: 'text-left',
                //render: function (row) {
                //    const dob = row.dateOfBirth;
                //    const age = calculateAgeDetailed(dob);
                //    let years = age.years;
                //    let months = age.months;
                //    if (age.days >= 15) {
                //        months += 1;
                //    }
                //    // Handle month overflow
                //    if (months >= 12) {
                //        years += 1;
                //        months = months % 12;
                //    }
                //    return `${years} Years, ${months} Months`;
                //}
                render: function (row) {
                    const dob = row.dateOfBirth;
                    const age = calculateAgeDetailed(dob);
                    return `${age.years} Years`;
                    //return `${age.years} Years, ${age.months} Months, ${age.days} Days`;
                    //return `${age.years}-Y, ${age.months}-M, ${age.days}-D`;
                }
            },
            { data: 'bankAccountNo' },
            { data: 'bankName' },
            { data: 'bankBranch' },
            { data: 'bankRoutingNo' },
            {
                data: 'isActive',
                className: 'text-center',
                render: function (value, type, row, meta) {
                    let switchId = `empSwitch_${meta.row}`;
                    let checked = value == 1 ? 'checked' : '';
                    let statusText = value == 1 ? 'Active' : 'Inactive';
                    let statusClass = value == 1 ? 'text-success' : 'text-danger';
                    return `<div class="custom-control custom-switch"><input type="checkbox" class="custom-control-input emp-status" id="${switchId}" data-id="${row.recordId}"${checked}><label class="custom-control-label ${statusClass}" for="${switchId}">${statusText}</label> | <button type="button" class="btn btn1 btn-edit" data-id="${row.recordId}"><i class="fas fa-edit"></i></button></div>`;
                }
            }
        ],
        order: [[0, 'asc']],
        pageLength: 30,
        responsive: true,
        lengthChange: false,
        autoWidth: false,
        info: true
    });
    $('#rTable').on('change', '.emp-status', function () {
        changeStatus($(this).data('id'));
    }).on('click', '.btn-edit', function () {
        fnEdit($(this).data('id'));
    });
}
function initDateOfBirth() {
    $('#DateOfBirth').one('focus', function () {
        init_datepicker(this);
        $(this).datepicker('show');
    });
}
function bindSubmit() {
    $('#btnSubmit').on('click', function () {
        let model = {
            RecordId: parseInt($('#RecordId').val()) || 0,
            ParentId: $('#EmployeeRefId').val() || 0,
            StudentCode: $('#StudentCode').val(),
            StudentName: $('#StudentName').val(),
            DateOfBirth: $('#DateOfBirth').val(),
            Gender: $('#Gender').val(),
            BankAccountNo: $('#BankAccountNo').val(),
            BankName: $('#BankId option:selected').text(),
            BankBranch: $('#BankBranch').val(),
            BankRoutingNo: $('#BankRoutingNo').val()
        };
        if (model.ParentId == 0) {
            toastr.error('Employee reference is required.');
            return;
        }
        if (!model.StudentCode) {
            toastr.error('Student code is required.');
            $('#StudentCode').focus();
            return;
        }
        if (!model.StudentName) {
            toastr.error('Student name is required.');
            $('#StudentName').focus();
            return;
        }
        if (!model.DateOfBirth) {
            toastr.error('Date of birth is required.');
            $('#DateOfBirth').focus();
            return;
        }
        if (model.RecordId > 0) {
            updateStudent(model);
        } else {
            addStudent(model);
        }
    });
}
function addStudent(model) {
    $.post('/Macalms/AddStudentProfile', { model }, function (data) {
        if (data > 0) {
            toastr.success('Successfully saved.');
            setTimeout(function () {
                $('#rTable').DataTable().ajax.reload();
                resetForm();
            }, 1000);
        }
    });
}
function updateStudent(model) {
    $.post('/Macalms/UpdateStudentProfile', { model }, function (data) {
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
    isEditMode = false;
    if ($('#RecordId').val() > 0) {
        $('#btnSubmit').removeClass('btn-warning').addClass('btn-success').val('Save');
    }
    $('#RecordId').val(0);
    $('#StudentCode, #StudentName, #DateOfBirth, #BankAccountNo, #BankBranch, #BankRoutingNo').val('');
    $('#Gender').val('--Select--').trigger('change');
    loadAllDropdowns();
}
function loadAllDropdowns() {
    loadDropdown('/Macalms/GetParents', '#EmployeeRefId', 'employeeName');
    loadDropdown('/Macalms/GetBanks', '#BankId', 'bankName');
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
function fnEdit(recordId) {
    isEditMode = true;
    $.get('/Macalms/StudentEditView', { RecordId: recordId }, function (data) {
        $('#entry-ui').removeClass('collapsed-card').addClass('card card-info card-outline');
        $('#RecordId').val(data.recordId);
        if (data.parentId != null) {
            $('#EmployeeRefId').val(data.parentId).trigger('change');
        }
        $('#StudentCode').val(data.studentCode);
        $('#StudentName').val(data.studentName);
        $('#DateOfBirth').val(data.dateOfBirth);
        if (data.gender != null) {
            $('#Gender').val(data.gender).trigger('change');
        }
        $('#BankAccountNo').val(data.bankAccountNo);
        if (data.bankId != null) {
            $('#BankId').val(data.bankId).trigger('change');
        }
        $('#BankBranch').val(data.bankBranch);
        $('#BankRoutingNo').val(data.bankRoutingNo);

        $('#btnSubmit').removeClass('btn-success').addClass('btn-warning').val('Update');
    });
}
function changeStatus(recordId) {
    $.post('/Macalms/ChangeStudentStatus', { RecordId: recordId }, function (data) {
        if (data == -99) {
            toastr.error('Parent is not active in the Job');
            $('#rTable').DataTable().ajax.reload(null, false);
        }
        else {
            if (data > 0) {
                $('#rTable').DataTable().ajax.reload(null, false);
            } else {
                alert('Try again');
            }
        }
    });
}
function calculateAgeDetailed(dob) {
    // dob format: dd-MM-yyyy
    const parts = dob.split('-');
    let birthDate = new Date(
        parseInt(parts[2]),        // year
        parseInt(parts[1]) - 1,    // month (0-based)
        parseInt(parts[0])         // day
    );

    let today = new Date();
    let years = today.getFullYear() - birthDate.getFullYear();
    let months = today.getMonth() - birthDate.getMonth();
    let days = today.getDate() - birthDate.getDate();

    // Adjust days and months if negative
    if (days < 0) {
        months--;
        // Get days in previous month
        const prevMonth = new Date(today.getFullYear(), today.getMonth(), 0);
        days += prevMonth.getDate();
    }

    // Adjust years if months negative
    if (months < 0) {
        years--;
        months += 12;
    }
    return {
        years: years,
        months: months,
        days: days
    };
}
