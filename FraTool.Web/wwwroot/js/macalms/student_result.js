$(document).ready(function () {
    initDataTable();
    initializeDropdowns();
    //$('#StudentId').change(function () {
    //    $('#StudentId').val();
    //    toastr.info(`Selected Student ID: ${$('#StudentId').val()}`);
    //});
    bindEvents();
    bindSubmit();
});
function initDataTable() {
    $('#rTable').DataTable({
        ajax: {
            url: '/Macalms/GetStudentResults',
            dataSrc: ''
        },
        columns: [
            { data: 'studentCode' },
            { data: 'studentName' },
            { data: 'assessmentYear' },
            { data: 'classStudied' },
            { data: 'nameOfTheInstitution' },
            { data: 'studyMedium' },
            { data: 'examResult' },
            { data: 'academyType' },
            {
                data: 'isActive',
                className: 'text-center',
                render: function (value, type, row) {
                    return `<button type="button" class="btn btn1 btn-edit" data-id="${row.recordId}"><i class="fas fa-edit"></i></button>`;
                }
            }
        ],
        //order: [[0, 'asc']],
        pageLength: 30,
        responsive: true,
        lengthChange: false,
        autoWidth: false
    });
    $('#rTable').on('click', '.btn-edit', function () {
        fnEdit($(this).data('id'));
    });
}
function bindSubmit() {
    $('#btnSubmit').on('click', function () {
        if (!validateFormWithToastr()) return;
        const model = {
            RecordId: parseInt($('#RecordId').val()) || 0,
            StudentId: parseInt($('#StudentId').val()) || 0,
            ClassStudied: $('#ClassStudied').val().trim(),
            NameOfTheInstitution: $('#InstitutionName').val().trim(),
            StudyMedium: $('#StudyMedium').val(),
            AcademyType: $('#AcademyType').val(),
            ExamResult: $('#ExamResult').val().trim(),
            AssessmentYear: $('#AssessmentYearId option:selected').text()
        };
        if (model.RecordId > 0) {
            updateResult(model);
        } else {
            addResult(model);
        }
    });
}
function addResult(model) {
    $.post('/Macalms/SaveStudentResult', { model }, function (data) {
        if (data > 0) {
            toastr.success('Successfully saved.');
            $('#rTable').DataTable().ajax.reload();
            resetForm();
        }
    });
}
function updateResult(model) {
    $.post('/Macalms/UpdateExamResult', { model }, function (data) {
        if (data > 0) {
            toastr.success('Successfully updated.');
            setTimeout(function () {
                $('#rTable').DataTable().ajax.reload();
                resetForm();
            }, 1000);
        }
    });
}
function fnEdit(recordId) {
    isEditMode = true;
    $.get('/Macalms/ResultEditView', { RecordId: recordId }, function (data) {
        $('#entry-ui').removeClass('collapsed-card').addClass('card card-info card-outline');

        $('#RecordId').val(data.recordId);
        if (data.assessmentYearId) {
            $('#AssessmentYearId').val(data.assessmentYearId).trigger('change');
        }
        if (data.parentId) {
            $('#EmployeeRefId').val(data.parentId).trigger('change');
            // Load students FIRST, then set student
            loadStudents(data.parentId, function () {
                const ddlStudent = $('#StudentId');
                // Inject option if missing
                if (ddlStudent.find(`option[value="${data.studentId}"]`).length === 0) {
                    ddlStudent.append(
                        new Option(data.studentName, data.studentId, true, true)
                    );
                }
                ddlStudent.val(data.studentId).trigger('change');
            });
        }
        $('#ClassStudied').val(data.classStudied || '');
        $('#InstitutionName').val(data.nameOfTheInstitution || '');
        $('#StudyMedium').val(data.studyMedium).trigger('change');
        $('#AcademyType').val(data.academyType).trigger('change');
        $('#ExamResult').val(data.examResult || '');
        $('#btnSubmit').removeClass('btn-success').addClass('btn-warning').val('Update');
    });
}
function validateFormWithToastr() {
    $('.is-invalid').removeClass('is-invalid');
    // Assessment Year
    if ($('#AssessmentYearId').val() === '0') {
        showError('#AssessmentYearId', 'Please select an assessment year.');
        return false;
    }
    // Student
    if ($('#StudentId').val() === '0') {
        showError('#StudentId', 'Please select a student.');
        return false;
    }
    // Class Studied
    if (!$('#ClassStudied').val().trim()) {
        showError('#ClassStudied', 'Class studied is required.');
        return false;
    }
    // Institution Name
    if (!$('#InstitutionName').val().trim()) {
        showError('#InstitutionName', 'Institution name is required.');
        return false;
    }
    // Study Medium
    if (!$('#StudyMedium').val()) {
        showError('#StudyMedium', 'Please select a study medium.');
        return false;
    }
    // Exam Result
    if (!$('#ExamResult').val().trim()) {
        showError('#ExamResult', 'Exam result is required.');
        return false;
    }
    return true;
}
function showError(selector, message) {
    $(selector).addClass('is-invalid').focus();
    toastr.error(message);
}
function bindEvents() {
    $('#EmployeeRefId').on('change', function () {
        const parentId = $(this).val();
        if (!parentId || parentId === '0') {
            resetDropdown('#StudentId');
            return;
        }
        loadStudents(parentId);
    });
}
function loadStudents(parentId, callback) {
    $.get('/Macalms/GetStudents', { ParentId: parentId }).done(function (data) {
        const ddl = $('#StudentId');
        resetDropdown(ddl);
        $.each(data, function (_, item) {
            ddl.append(
                $('<option>', {
                    value: item.recordId,
                    text: item.studentName
                })
            );
        });
        if (typeof callback === 'function') {
            callback();
        }
    });
}
function initializeDropdowns() {
    loadAssessmentYear('/Macalms/GetAssessmentYears', '#AssessmentYearId', 'yearName');
    loadParents('/Macalms/GetParents', '#EmployeeRefId', 'employeeName');
}
function loadAssessmentYear(url, selector, textField) {
    $.get(url).done(function (data) {
        const ddl = $(selector);
        resetDropdown(ddl);
        $.each(data, function (index, item) {
            ddl.append(
                $('<option>', {
                    value: item.recordId,
                    text: item[textField]
                })
            );
            // Select first item
            if (index === 0) {
                ddl.val(item.recordId);
            }
        });
    });
}
function loadParents(url, selector, textField) {
    $.get(url)
        .done(function (data) {
            const ddl = $(selector);
            resetDropdown(ddl);

            $.each(data, function (_, item) {
                ddl.append(
                    $('<option>', {
                        value: item.recordId,
                        text: item[textField]
                    })
                );
            });
        }
    );
}
function resetDropdown(selectorOrElement) {
    const ddl = typeof selectorOrElement === 'string'
        ? $(selectorOrElement)
        : selectorOrElement;
    ddl.empty().append('<option value="0">--Select--</option>');
}
function resetForm() {
    if ($('#RecordId').val() > 0) {
        $('#btnSubmit').removeClass('btn-warning').addClass('btn-success').val('Save');
    }    
    // Clear text inputs
    $('#ClassStudied').val('');
    $('#InstitutionName').val('');
    $('#ExamResult').val('');
    // Reset dropdowns
    $('#EmployeeRefId').val('0').trigger('change');
    $('#StudentId').val('0').trigger('change');
    $('#StudyMedium').val('').trigger('change');
    $('#AcademyType').val('').trigger('change');
    // Reset hidden / system fields
    $('#RecordId').val('0');
    // Remove validation styles
    $('.is-invalid').removeClass('is-invalid');
    // Clear toastr notifications
    if (typeof toastr !== 'undefined') {
        toastr.clear();
    }
}
