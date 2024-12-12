$(document).ready(function () {
    var inc = 0;
    $('#rTable').DataTable({
        "ajax": {
            url: "/Company/AllCompanies",
            dataSrc: ''
        },
        "columns": [
            { data: "companyName" },
            { data: "shortCode" },
            {
                data: null,
                render: function (data) {
                    if (data.isActive == 1) {
                        inc++;
                        return '<div class="custom-control custom-switch"><input type="checkbox" onchange="changeStatus(' + data.companyId + ')" class="custom-control-input" id="customSwitch' + inc + '" checked><label class="custom-control-label text-success" for="customSwitch' + inc + '">Active</label></div>';
                    } else if (data.isActive == 0) {
                        inc++;
                        return '<div class="custom-control custom-switch"><input type="checkbox" onchange="changeStatus(' + data.companyId + ')" class="custom-control-input" id="customSwitch2' + inc + '" ><label class="custom-control-label text-danger" for="customSwitch2' + inc + '">Inactive</label></div>';
                    }
                }
            },
            {
                data: null,
                render: function (data) {
                    if (data.isActive == 1) {
                        return '<button type="button" class="btn btn1" onclick="fnEdit(' + data.companyId + ');"><i class="fas fa-edit"></i></button>';
                    }
                    else {
                        return '';
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
    $("#CompanyName").change(function () {
        var CompanyName = $("#CompanyName").val();
        if (CompanyName == '') {
            $("#CompanyName").css("border-color", "red");
            $("#CompanyName").focus();
            toastr.warning('Company name is required, please input.');
        }
        else {
            $.get("/Company/CheckCompanyName",
            {
                name: CompanyName
            },
            function (data) {
                if (data == 0) {
                    $("#CompanyName").css("border-color", "#ced4da");
                }
                else {
                    $("#CompanyName").css("border-color", "red");
                    $("#CompanyName").focus();
                    toastr.error('This company name already exist, try another.');
                }
            });
        }
    });
    $("#btnSubmit").click(function () {
        var CompanyId = $("#CompanyId").val();
        var CompanyName = $("#CompanyName").val();
        var status = false;
        if (CompanyName == "") {
            $("#CompanyName").css("border-color", "red");
            $("#CompanyName").focus();
            toastr.warning('Company name is required, please input.');
            status = false;
        }
        else {
            status = true;
        }
        if (status == true) {
            var model = {
                CompanyId: CompanyId,
                CompanyName: CompanyName
            };
            $.post("/Company/AddOrUpdate", { model: model }, function (data) {
                if (data != "") {
                    toastr.success(data);
                    setTimeout(function () {
                        //location.reload();
                        $('#rTable').DataTable().ajax.reload();
                        $("#CompanyName").css("border-color", "#ced4da");
                        $("#CompanyName").val('');
                    }, 1000);
                }
            });
        }
    });
});
function changeStatus(CompanyId) {
    $.post("/Company/ChangeStatus",
        {
            CompanyId: CompanyId
        },
        function (data) {
            if (data > 0) {
                $('#rTable').DataTable().ajax.reload();
            }
            else {
                alert('Try again');
            }
        });
}
function fnEdit(id) {
    $.get('/Company/EditView',
        {
            id: id
        },
        function (data) {
            $("#entry-ui").removeClass('card card-info card-outline collapsed-card');
            $("#entry-ui").addClass('card card-info card-outline');
            $("#CompanyId").val(data.companyId);
            $("#CompanyName").val(data.companyName);
        }
    );
}