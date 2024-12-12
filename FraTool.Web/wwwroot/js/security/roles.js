$(document).ready(function () {
    var inc = 0;
    $('#roleTable').DataTable({
        "ajax": {
            url: "/Roles/GetAllRole",
            dataSrc: ''
        },
        "columns": [
            { data: "roleName" },
            {
                data: null,
                render: function (data) {
                    if (data.isActive == 1) {
                        inc++;
                        return '<div class="custom-control custom-switch"><input type="checkbox" onchange="changeStatus(' + data.roleId + ')" class="custom-control-input" id="customSwitch' + inc + '" checked><label class="custom-control-label text-success" for="customSwitch' + inc + '">Active</label></div>';
                    } else if (data.isActive == 0) {
                        inc++;
                        return '<div class="custom-control custom-switch"><input type="checkbox" onchange="changeStatus(' + data.roleId + ')" class="custom-control-input" id="customSwitch2' + inc + '" ><label class="custom-control-label text-danger" for="customSwitch2' + inc + '">Inactive</label></div>';
                    }
                }
            },
            {
                data: null,
                render: function (data) {
                    if (data.isActive == 1) {
                        return '<button type="button" class="btn btn1" onclick="fnEdit(' + data.roleId + ')"><i class="fas fa-edit"></i></button>';
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
        pageLength: 10,
        responsive: true,
        lengthChange: false,
        autoWidth: false
    });
    $("#RoleName").change(function () {
        var RoleName = $("#RoleName").val();
        if (RoleName == '') {
            $("#RoleName").css("border-color", "red");
            $("#RoleName").focus();
            toastr.warning('Role name is required, please input.');
        }
        else {
            if (RoleName.length < 4) {
                $("#RoleName").css("border-color", "red");
                $("#RoleName").focus();
                toastr.warning('Role name should contain 4 or more characters.');
            }
            else {
                $.get("/Roles/CheckRole",
                    {
                        name: RoleName
                    },
                    function (data) {
                        if (data == 0) {
                            $("#RoleName").css("border-color", "#ced4da");
                            //toastr.success('This role name is available, you can use.');
                        }
                        else {
                            $("#RoleName").css("border-color", "red");
                            $("#RoleName").focus();
                            toastr.error('This role name already exist, try another.');
                        }
                    }
                );
            }
        }
    });
    $("#btnSubmit").click(function () {
        var RoleId = $("#RoleId").val();
        var RoleName = $("#RoleName").val();
        var status = false;
        if (RoleName == "") {
            $("#RoleName").css("border-color", "red");
            $("#RoleName").focus();
            toastr.warning('Role name is required, please input.');
            status = false;
        }
        else {
            status = true;
        }

        if (status == true) {
            var model = {
                RoleId: RoleId,
                RoleName: RoleName
            };
            $.post("/Roles/AddOrUpdate", { model: model }, function (data) {
                if (data != "") {
                    toastr.success(data);
                    setTimeout(function () {
                        //location.reload();
                        $('#roleTable').DataTable().ajax.reload();
                        $("#RoleName").css("border-color", "#ced4da");
                        $("#RoleId").val(0);
                        $("#RoleName").val('');
                    }, 1000);
                }
            });
        }
    });
});
function changeStatus(roleId) {
    $.post("/Roles/ChangeStatus",
        {
            RoleId: roleId
        },
        function (data) {
            if (data > 0) {
                $('#roleTable').DataTable().ajax.reload();
                $("#RoleName").css("border-color", "#ced4da");
                $("#RoleId").val(0);
                $("#RoleName").val('');
            }
            else {
                alert('try again');
            }
        }
    );
}
function fnEdit(id) {
    $.get('/Roles/EditView',
        {
            id: id
        },
        function (data) {
            $("#RoleId").val(data.roleId);
            $("#RoleName").val(data.roleName);
        }
    );
}