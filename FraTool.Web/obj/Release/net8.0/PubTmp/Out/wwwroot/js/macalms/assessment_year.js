$(document).ready(function () {
    var inc = 0;
    $('#rTable').DataTable({
        "ajax": {
            url: "/Macalms/AssessmentYears",
            dataSrc: ''
        },
        "columns": [
            { data: "yearName" },
            {
                data: null,
                render: function (data) {
                    if (data.isActive == 1) {
                        inc++;
                        return '<div class="custom-control custom-switch"><input type="checkbox" onchange="changeStatus(' + data.recordId + ')" class="custom-control-input" id="customSwitch' + inc + '" checked><label class="custom-control-label text-success" for="customSwitch' + inc + '">Active</label></div>';
                    } else if (data.isActive == 0) {
                        inc++;
                        return '<div class="custom-control custom-switch"><input type="checkbox" onchange="changeStatus(' + data.recordId + ')" class="custom-control-input" id="customSwitch2' + inc + '" ><label class="custom-control-label text-danger" for="customSwitch2' + inc + '">Inactive</label></div>';
                    }
                }
            }
        ],
        orderCellsTop: true,
        "info": true,
        order: [[0, "asc"]],
        columnDefs: [
            //{ orderable: false, targets: [1] }
        ],
        pageLength: 10,
        responsive: true,
        lengthChange: false,
        autoWidth: false
    });
    $("#btnSave").click(function () {
        var YearName = $("#YearName").val();
        var Status = false;
        if (YearName == "") {
            $("#YearName").css("border-color", "red");
            $("#YearName").focus();
            toastr.error("Input assessment year.");
            Status = false;
        }
        else {
            Status = true;
        }
        if (Status == true) {
            var model = {
                YearName: YearName
            };
            $.post("/Macalms/SaveAssessmentYear", { model: model }, function (data) {
                if (data == 1) {
                    toastr.success('Successfully Saved.');
                    setTimeout(function () {
                        $('#rTable').DataTable().ajax.reload();
                        $("#YearName").css("border-color", "#ced4da");
                        $("#YearName").val('');
                    }, 1000);
                }
                else {
                    toastr.error('You are trying to save a duplicate year.');
                }
            });
        }
    });
});
function changeStatus(id) {
    $.post("/Macalms/ChangeYearStatus",
        {
            id: id
        },
        function (data) {
            if (data > 0) {
                $('#rTable').DataTable().ajax.reload();
                $("#YearName").css("border-color", "#ced4da");
                $("#YearName").val('');
                $("#cardTitle").text('Add Assessment Year)');
                $("#btnSave").removeClass('btn-warning');
                $("#btnSave").addClass('btn-success');
                $("#btnSave").prop("value", "Save");
            }
            else {
                alert('try again');
            }
        }
    );
}
