$(document).ready(function () {
    initializeTable();
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
                    return `<button type="button" onclick="fnDetails(${data.recordId})" style="margin-top: -7px !important; margin-bottom: -7px !important;" class="btn text-default" data-toggle="modal" data-target="#modal-xl"><ion-icon name="eye"></ion-icon></button>`;                    
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
