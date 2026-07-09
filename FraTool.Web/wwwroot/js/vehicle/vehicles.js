$(document).ready(function () {
    initializeTable();
    loadAllDropdowns();
    initRegistrationDate();
    $('#VehicleName').change(function () {
        var vahicleName = $('#VehicleName').val();        
        if (vahicleName === '' || vahicleName === null) {
            $('#VehicleName').css("border-color", "red").focus();
            toastr.error('Vehicle Name is required');
        }
        else {
            $('#VehicleName').css("border-color", "#ced4da");
        }        
    });
    $('#RegistrationDate').change(function () {
        var registrationDate = $('#RegistrationDate').val();
        if (registrationDate === '' || registrationDate === null) {
            $('#RegistrationDate').css("border-color", "red").focus();
            toastr.error('Registration Date is required');
        }
        else {
            $('#RegistrationDate').css("border-color", "#ced4da");
        }
    });
    $('#btnSave').click(function () {
        var model = {
            RecordId: $('#RecordId').val(),
            VehicleName: $('#VehicleName').val(),
            OwnerId: $('#OwnerId').val(),
            VehicleTypeId: $('#VehicleTypeId').val(),
            RegistrationDate: $('#RegistrationDate').val(),
            LicensePlate: $('#LicensePlate').val(),
            IssueLocationId: $('#IssueLocationId').val(),
            IssueToId: $('#IssueToId').val(),
            BRTAOfficeId: $('#BRTAOfficeId').val(),
            DriverId: $('#DriverId').val(),
            SeatCapacityWithDriver: $('#SeatCapacityWithDriver').val(),
            Remarks: $('#Remarks').val()
        }
        var isValid = true;
        if (model.VehicleName === '' || model.VehicleName === null) {
            $('#VehicleName').css("border-color", "red").focus();
            toastr.error('Vehicle Name is required');
            isValid = false;
            return;
        }
        if (model.OwnerId === '0' || model.OwnerId === null) {
            toastr.error('Vehicle Owner is required');
            isValid = false;
            return;
        }
        if (model.VehicleTypeId === '0' || model.VehicleTypeId === null) {
            toastr.error('Vehicle Type is required');
            isValid = false;
            return;
        }
        if (model.RegistrationDate === '' || model.RegistrationDate === null) {
            $('#RegistrationDate').css("border-color", "red").focus();
            toastr.error('Registration Date is required');
            isValid = false;
            return;
        }

        if (isValid == true) {
            //toastr.success('Vehicle information is valid. Ready to submit.');
            if (model.RecordId > 0) {
                $.post('/Vehicle/UpdateVehicle', { model }, function (data) {
                    if (data.success) {
                        toastr.success(data.message);
                        setTimeout(function () {
                            $('#rTable').DataTable().ajax.reload();
                            resetForm();
                        }, 1000);
                    }
                    else {
                        toastr.error(data.message);
                    }
                });
            }
            else {
                $.post('/Vehicle/SaveVehicle', { model }, function (data) {
                    if (data.success) {
                        toastr.success(data.message);
                        setTimeout(function () {
                            $('#rTable').DataTable().ajax.reload();
                            resetForm();
                        }, 1000);
                    }
                    else {
                        toastr.error(data.message);
                    }
                });
            }
        }
        function resetForm() {
            const recordId = $("#RecordId").val();
            if (recordId > 0) {
                $("#RecordId").val(0);                
            }
            $("#VehicleName").val('').css("border-color", "#ced4da");
            $('#OwnerId').val(0).trigger('change');
            $('#VehicleTypeId').val(0).trigger('change');
            $("#RegistrationDate").val('');
            $("#LicensePlate").val('');
            $('#IssueLocationId').val(0).trigger('change');
            $('#IssueToId').val(0).trigger('change');
            $('#BRTAOfficeId').val(0).trigger('change');
            $('#DriverId').val(0).trigger('change');
            $('#SeatCapacityWithDriver').val('');
            $('#Remarks').val('');

            $("#btnSave").prop("value", "Save").removeClass('btn btn-warning').addClass('btn btn-success'); //.attr('disabled', 'disabled');
            $('#btntool').html('<i class="fas fa-plus"></i>&nbsp;&nbsp; Add Vehicle');
            $("#entry-ui").removeClass('card card-info card-outline').addClass('card card-info card-outline collapsed-card');
        }
    });
});
function initializeTable() {
    var table = $('#rTable').DataTable({
        ajax: {
            url: "/Vehicle/VehicleList",
            dataSrc: ''
        },
        columns: [
            { data: 'vehicleName' },
            { data: 'brtaOfficeName' },
            { data: 'ownerName' },
            { data: 'issueLocationName' },
            { data: 'issueToName' },
            { data: 'remarks' },
            {
                data: null,
                render: function (data) {
                    //return data.isActive == 1 ? `<button type="button" onclick="fnEdit(${data.recordId})" class="btn btn1 text-primary"><i class="fas fa-edit"></i></button>` : '';
                    return `<button type="button" onclick="fnDetails(${data.recordId})" style="margin-top: -7px !important; margin-bottom: -7px !important;" class="btn text-default" data-toggle="modal" data-target="#modal-xl"><ion-icon name="eye"></ion-icon></button>  <button type="button" style="margin-top: -7px !important; margin-bottom: -7px !important;" onclick="fnEdit(${data.recordId})" class="btn text-primary"><ion-icon name="create"></ion-icon></button>`;

                    /* return `<button type="button" onclick="fnDetails(${data.recordId})" style="margin-top: -7px !important; margin-bottom: -7px !important;" class="btn btn-sm text-default" data-toggle="modal" data-target="#modal-xl"><i class="fas fa-eye"></i></button>  <button type="button" style="margin-top: -7px !important; margin-bottom: -7px !important;" onclick="fnEdit(${data.recordId})" class="btn btn-sm text-info"><i class="fas fa-edit"></i></button>`;*/

                    //return `<button type="button" onclick="fnEdit(${data.recordId})" class="btn btn1 text-primary"><i class="fas fa-edit"></i></button>`
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
function initRegistrationDate() {
    $('#RegistrationDate').one('focus', function () {
        registrationDate_datepicker(this);        
        $(this).datepicker('show');
    });
}
function loadAllDropdowns() {
    loadDropdown('/Vehicle/IssueLocation_dd', '#IssueLocationId', 'locationName');
    loadDropdown('/Vehicle/Owners_dd', '#OwnerId', 'ownerName');
    loadDropdown('/Vehicle/VehicleType_dd', '#VehicleTypeId', 'typeName');
    loadDropdown('/Vehicle/BRTAOffice_dd', '#BRTAOfficeId', 'officeName');
    loadDropdown('/Vehicle/Driver_dd', '#DriverId', 'driverName');
    loadDropdown('/Vehicle/IssueToUser_dd', '#IssueToId', 'receiverName');
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
    $.get('/Vehicle/VehiclesEditView', { id }, function (data) {
        if (data != null) {
            $("#entry-ui").removeClass('card card-info card-outline collapsed-card').addClass('card card-info card-outline');
            $("#RecordId").val(data.recordId);
                        
            $("#VehicleName").val(data.vehicleName);
            if (data.ownerId > 0) {
                $('#OwnerId').val(data.ownerId).trigger('change');
            }
            if (data.vehicleTypeId > 0) {
                $('#VehicleTypeId').val(data.vehicleTypeId).trigger('change');
            }
            $("#RegistrationDate").val(data.registrationDate);
            $("#LicensePlate").val(data.licensePlate);
            if (data.issueLocationId > 0) {
                $('#IssueLocationId').val(data.issueLocationId).trigger('change');
            }
            if (data.issueToId > 0) {
                $('#IssueToId').val(data.issueToId).trigger('change');
            }
            if (data.brtaOfficeId > 0) {
                $('#BRTAOfficeId').val(data.brtaOfficeId).trigger('change');
            }
            if (data.driverId > 0) {
                $('#DriverId').val(data.driverId).trigger('change');
            }
            $("#SeatCapacityWithDriver").val(data.seatCapacityWithDriver);
            $("#Remarks").val(data.remarks);

            $('#btntool').html('<i class="fas fa-minus"></i>&nbsp;&nbsp; Change Vehicle Info');
            $("#btnSave").removeClass('btn-success').addClass('btn-warning').prop("value", "Update").removeAttr('disabled');
        }
    });
}
function fnDetails(id) {
    if (id > 0) {
        $.get('/Vehicle/VehiclesEditView', { id }, function (data) {
            if (data != null) {
                $('#vehicleDetailsTable tbody').empty();
                var html = '';
                html += `<tr><td>${data.vehicleName}</td><td>${data.ownerName}</td><td>${data.registrationDate}</td><td>${data.licensePlate}</td><td>${data.issueLocationName}</td><td>${data.issueToName}</td><td>${data.vehicleTypeName}</td><td>${data.brtaOfficeName}</td><td>${data.driverName}</td><td>${data.seatCapacityWithDriver}</td></tr>`
                
                $('#vehicleDetailsTable tbody').append(html);
            }
        });
    }
}
