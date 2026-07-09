var eAttachmentRef = '';
$(document).ready(function () { 
    
    initializeTable();
    my_datepicker(IssueDate);
    $("#ExpiredDate").datepicker({
        dateFormat: "dd-mm-yy",
        todayHighlight: true,
        changemonth: true,
        changeyear: true,
        showOtherMonths: true,
        selectOtherMonths: true
    });
    loadAllDropdowns();
    $('#DocumentAttachment').on('change', function () {
        let fileName = this.files.length
            ? this.files[0].name
            : 'No file selected';

        $('#fileName').text(fileName);
    });
    $('#btnSave').click(function () {
        var isValid = true;
        if ($('#DocumentTypeId option:selected').text() === '--Select--' || $('#DocumentTypeId').val() === 0) {
            toastr.error('Document Type is required');
            isValid = false;
            return;
        }
        if ($('#VehicleId option:selected').text() === '--Select--' || $('#VehicleId').val() === 0) {
            toastr.error('Vehicle Name is required');
            isValid = false;
            return;
        }
        if ($('#IssueDate').val() === '' || $('#IssueDate').val() === null) {
            //$('#IssueDate').css("border-color", "red").focus();
            toastr.error('Issue date is required');
            isValid = false;
            return;
        }
        if ($('#ExpiredDate').val() === '' || $('#ExpiredDate').val() === null) {
            //$('#ExpiredDate').css("border-color", "red").focus();
            toastr.error('Expired date is required');
            isValid = false;
            return;
        }
        if ($('#DocumentAttachment')[0].files.length === 0) {
            //$('#ExpiredDate').css("border-color", "red").focus();
            toastr.error('Upload an attachment.');
            isValid = false;
            return;
        }

        //$('#VehicleName').css("border-color", "#ced4da");
        if (isValid == true) {
            var formData = new FormData();
            formData.append("RecordId", $('#RecordId').val());
            formData.append("DocumentTypeId", $('#DocumentTypeId').val());
            formData.append("DocumentTypeName", $('#DocumentTypeId option:selected').text());
            formData.append("VehicleId", $('#VehicleId').val());
            formData.append("VehicleName", $('#VehicleId option:selected').text());
            formData.append("IssueDate", toIsoDate($('#IssueDate').val()));
            formData.append("ExpiredDate", toIsoDate($('#ExpiredDate').val()));
            formData.append("FileUrl", eAttachmentRef);
            formData.append("Remarks", $('#Remarks').val());
            var file = $('#DocumentAttachment')[0].files[0];

            if (file != null) {
                formData.append("DocumentAttachment", file);
            }
            if ($('#RecordId').val() > 0) {
                $.ajax({
                    url: '/Vehicle/UpdateVehicleDocuments',
                    type: 'POST',
                    data: formData,
                    processData: false,
                    contentType: false,
                    success: function (response) {
                        toastr.success(response.message);
                        setTimeout(function () {
                            $('#rTable').DataTable().ajax.reload();
                            //resetForm();
                            window.location.reload();
                        }, 1000);
                    },
                    error: function (xhr) {
                        toastr.error(response.message);
                    }
                });
            }
            else {                
                $.ajax({
                    url: '/Vehicle/SaveDocument',
                    type: 'POST',
                    data: formData,
                    processData: false,
                    contentType: false,
                    success: function (response) {
                        toastr.success(response.message);
                        setTimeout(function () {
                            $('#rTable').DataTable().ajax.reload();
                            //resetForm();
                            window.location.reload();
                        }, 1000);
                    },
                    error: function (xhr) {
                        toastr.error(response.message);
                    }
                });
            }
        }
        function resetForm() {
            const recordId = $("#RecordId").val();
            if (recordId > 0) {
                $("#RecordId").val(0);
            }
            $('#DocumentTypeId').val(0).trigger('change');
            $('#VehicleId').val(0).trigger('change');
            $("#IssueDate").val('');
            $("#ExpiredDate").val('');
            $("#DocumentAttachment").val('');
            eAttachmentRef = '';
            $('#Remarks').val('');

            $("#btnSave").prop("value", "Save").removeClass('btn btn-warning').addClass('btn btn-success'); //.attr('disabled', 'disabled');
            $('#btntool').html('<i class="fas fa-plus"></i>&nbsp;&nbsp; Add Vehicle');
            $("#entry-ui").removeClass('card card-info card-outline').addClass('card card-info card-outline collapsed-card');
        }
    });
    function toIsoDate(date) {
        if (!date) return "";
        const parts = date.split('-');
        return `${parts[2]}-${parts[1]}-${parts[0]}`;
    }
});
function formatDate(dateString) {
    if (!dateString)
        return '';
    var date = new Date(dateString);
    if (isNaN(date.getTime()))
        return '';
    var day = String(date.getDate()).padStart(2, '0');
    var month = String(date.getMonth() + 1).padStart(2, '0');
    var year = date.getFullYear();
    return day + '-' + month + '-' + year;
}
function initializeTable() {
    var table = $('#rTable').DataTable({
        ajax: {
            url: "/Vehicle/VehicleDocumentList",
            dataSrc: ''
        },
        columns: [
            { data: 'vehicleName' },
            { data: 'documentTypeName' },
            {
                data: 'issueDate',
                render: function (data) {
                    return formatDate(data);
                }
            },
            {
                data: 'expiredDate',
                render: function (data) {
                    return formatDate(data);
                }
            },
            {
                data: 'fileUrl',
                render: function (data) {
                    if (!data) return '';
                    var ext = data.split('.').pop().toLowerCase();
                    var icon = '';
                    if (ext === 'pdf') {
                        icon = '<i class="fa fa-file-pdf" style="color:red;font-size:18px;"></i>';
                    }
                    else if (['jpg', 'jpeg', 'png', 'gif', 'bmp', 'webp'].includes(ext)) {
                        icon = '<i class="fa fa-file-image" style="color:green;font-size:18px;"></i>';
                    }
                    else {
                        icon = '<i class="fa fa-file-o" style="font-size:18px;"></i>';
                    }
                    return `<div style="text-align:center;"><a href="${data}" target="_blank">${icon}</a></div>`;
                }
            },
            { data: 'remarks' },
            {
                data: null,
                render: function (data) {
                    /*return `<button type="button" onclick="fnDetails(${data.recordId})" style="margin-top: -7px !important; margin-bottom: -7px !important;" class="btn text-default" data-toggle="modal" data-target="#modal-xl"><ion-icon name="eye"></ion-icon></button>  <button type="button" style="margin-top: -7px !important; margin-bottom: -7px !important;" onclick="fnEdit(${data.recordId})" class="btn text-primary"><ion-icon name="create"></ion-icon></button>`;*/

                    return `<button type="button" style="margin-top: -7px !important; margin-bottom: -7px !important;" onclick="fnEdit(${data.recordId})" class="btn text-primary"><ion-icon name="create"></ion-icon></button>`;
                }
            }
        ],
        info: true,
        order: [[0, "desc"]],
        pageLength: 30,
        lengthMenu: [[30, 50, 100, -1], [30, 50, 100, "All"]],
        responsive: false,
        autoWidth: false
    });
}
function loadAllDropdowns() {
    loadDropdown('/Vehicle/DocumentType_dd', '#DocumentTypeId', 'documentTypeName');
    loadDropdown('/Vehicle/Vehicle_dd', '#VehicleId', 'vehicleName');
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
function fnEdit(id) {
    $.get('/Vehicle/VehicleDocumentEditView', { id }, function (data) {
        console.log(data);
        if (data != null) {
            $("#entry-ui").removeClass('card card-info card-outline collapsed-card').addClass('card card-info card-outline');
            $("#RecordId").val(data.recordId);
            
            if (data.documentTypeId > 0) {
                $('#DocumentTypeId').val(data.documentTypeId).trigger('change');
            }
            if (data.vehicleId > 0) {
                $('#VehicleId').val(data.vehicleId).trigger('change');
            }
            $("#IssueDate").val(formatDate(data.issueDate));
            $("#ExpiredDate").val(formatDate(data.expiredDate));

            eAttachmentRef = data.fileUrl;
            $('#fileName').text('Uploaded file attached.');
            $("#Remarks").val(data.remarks);

            $('#btntool').html('<i class="fas fa-minus"></i>&nbsp;&nbsp; Change Vehicle Info');
            $("#btnSave").removeClass('btn-success').addClass('btn-warning').prop("value", "Update").removeAttr('disabled');
        }
    });
}
