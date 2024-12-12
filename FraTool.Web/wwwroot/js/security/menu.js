$(document).ready(function () {
    var inc = 0;
    $('#menuTable').DataTable({
        "ajax": {
            url: "/Menus/GetAllMenu",
            dataSrc: ''
        },
        "columns": [
            { data: "displayName" },
            { data: "controllerName" },
            { data: "actionName" },
            { data: "menuUrl" },
            { data: 'isParentMenu' },
            { data: "parentMenuId" },
            {
                data: null,
                render: function (data) {
                    return '<i class="'+data.iconTag+'"></i>';
                }
            },
            {
                data: null,
                render: function (data) {
                    if (data.isActive == 1) {
                        inc++;
                        return '<div class="custom-control custom-switch"><input type="checkbox" onchange="changeStatus(' + data.menuId + ')" class="custom-control-input" id="customSwitch' + inc + '" checked><label class="custom-control-label text-success" for="customSwitch' + inc + '">Active</label></div>';
                    } else if (data.isActive == 0) {
                        inc++;
                        return '<div class="custom-control custom-switch"><input type="checkbox" onchange="changeStatus(' + data.menuId + ')" class="custom-control-input" id="customSwitch2' + inc + '" ><label class="custom-control-label text-danger" for="customSwitch2' + inc + '">Inactive</label></div>';
                    }
                }
            },
            {
                data: null,
                render: function (data) {
                    if (data.isActive == 1) {
                        return '<button type="button" class="btn btn1" onclick="fnEdit(' + data.menuId + ');"><i class="fas fa-edit"></i></button>';
                    }
                    else if (data.isActive == 0) {
                        //return '<button type="button" class="btn btn-default btn1 text-danger" disabled><i class="fas fa-eye-slash"></i></button>';
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
    
    $('#ParentMenuId').attr("disabled", "disabled");
    $("#DisplayName").change(function () {
        var DisplayName = $("#DisplayName").val();
        if (DisplayName == '') {
            $("#DisplayName").css("border-color", "red");
            $("#DisplayName").focus();
            toastr.warning('Menu display name is required, please input.');
        }
        else {
            $.get("/Menus/CheckMenu",
            {
                name: DisplayName
            },
            function (data) {
                if (data == 0) {
                    $("#DisplayName").css("border-color", "green");
                    //toastr.success('This menu name is available, you can use.');
                }
                else {
                    $("#DisplayName").css("border-color", "red");
                    $("#DisplayName").focus();
                    toastr.error('This menu name already exist, try another.');
                }
            });
        }
    });
    $("#ControllerName").change(function () {
        var ControllerName = $("#ControllerName").val();
        if (ControllerName == '') {
            $("#ControllerName").css("border-color", "red");
            $("#ControllerName").focus();
            toastr.warning('Controller name is required, please input.');
        }
        else {
            $("#ControllerName").css("border-color", "#ced4da");
        }
    });
    $("#ActionName").change(function () {
        var ControllerName = $("#ControllerName").val();
        var ActionName = $("#ActionName").val();
        if (ControllerName == '') {
            $("#ControllerName").css("border-color", "red");
            $("#ControllerName").focus();
            toastr.warning('Controller name is required, please input.');
        }
        else {
            if (ActionName == '') {
                $("#ActionName").css("border-color", "red");
                $("#ActionName").focus();
                toastr.warning('Action name is required, please input.');
                $("#MenuUrl").removeAttr("disabled");
                $("#MenuUrl").val('');
            }
            else {
                $("#ActionName").css("border-color", "#ced4da");
                $("#MenuUrl").attr("disabled", "disabled");
                $("#MenuUrl").val('/' + ControllerName + '/' + ActionName);
            }
        }
    });
    $("input[name='IsParentMenu']").click(function () {
        IsParentMenu = $("input[name='IsParentMenu']:checked").val();
        if (IsParentMenu == "Yes") {
            $('#ParentMenuId').attr("disabled", "disabled");
            $("#ParentMenuId").empty();
            $("#ParentMenuId").append('<option value="0">--Select--</option>');
        }
        else {
            $.get("/Menus/GetParentMenu", function (data) {
                if (data.length > 0) {
                    $('#ParentMenuId').removeAttr("disabled");
                    $("#ParentMenuId").empty();
                    $("#ParentMenuId").append('<option value="0">--Select--</option>');
                    for (var i = 0; i < data.length; i++) {
                        $('<option/>', {
                            value: data[i].menuId,
                            html: data[i].displayName
                        }).appendTo('#ParentMenuId');
                    }
                }
            });
        }
    });
    $("#btnSubmit").click(function () {
        var MenuId = $("#MenuId").val();
        var DisplayName = $("#DisplayName").val();
        var ControllerName = $("#ControllerName").val();
        var ActionName = $("#ActionName").val();
        var MenuUrl = $("#MenuUrl").val();
        var IsParentMenu = $("input[name='IsParentMenu']:checked").val();
        var ParentMenuId = $("#ParentMenuId").val();
        var IconTag = $("#IconTag").val();

        var status = false;
        if (DisplayName == "") {
            $("#DisplayName").css("border-color", "red");
            $("#DisplayName").focus();
            toastr.warning('Menu displany name is required, please input.');
            status = false;
        }
        else {
            $("#DisplayName").css("border-color", "green");
            status = true;
        }
        if (IsParentMenu == "") {
            $("#IsParentMenu").css("border-color", "red");
            //$("#Password").focus();
            toastr.warning('please select IsParentMenu.');
            status = false;
        }
        else {
            $("#IsParentMenu").css("border-color", "green");
            if (IsParentMenu == "Yes") {
                IsParentMenu = 1;
            }
            else {
                IsParentMenu = 0;
            }
            status = false;
        }
        if (IconTag == "") {
            $("#IconTag").css("border-color", "red");
            $("#IconTag").focus();
            toastr.warning('Icon-Tag is required, please input.');
            status = false;
        }
        else {
            $("#IconTag").css("border-color", "#ced4da");
            status = true;
        }
        if (status == true) {
            var model = {
                MenuId: MenuId,
                DisplayName: DisplayName,
                ControllerName: ControllerName,
                ActionName: ActionName,
                MenuUrl: MenuUrl,
                IsParentMenu: IsParentMenu,
                ParentMenuId: ParentMenuId,
                IconTag: IconTag
            };
            $.post("/Menus/AddOrUpdate", { model: model }, function (data) {
                if (data != "") {
                    toastr.success(data);
                    setTimeout(function () {
                        $('#menuTable').DataTable().ajax.reload();
                        $("#MenuId").val(0);
                        $("#DisplayName").css("border-color", "#ced4da");
                        $("#DisplayName").val('');
                        $("#ControllerName").css("border-color", "#ced4da");
                        $("#ControllerName").val('');
                        $("#ActionName").css("border-color", "#ced4da");
                        $("#ActionName").val('');
                        $("#MenuUrl").val('');
                        $('input[name="IsParentMenu"]').prop('checked', false);
                        $("#ParentMenuId").empty();
                        $("#ParentMenuId").append('<option value="0">--Select--</option>');
                        $('#ParentMenuId').attr("disabled", "disabled");
                        $("#IconTag").css("border-color", "#ced4da");
                        $("#IconTag").val('');

                        $("#cardTitle").text('Add Menu');
                        $("#btnSubmit").removeClass('btn-warning');
                        $("#btnSubmit").addClass('btn-success');
                        $("#btnSubmit").prop("value", "Save");
                    }, 1000);
                }
            });
        }
    });
});
function changeStatus(menuId) {
    $.post("/Menus/ChangeStatus",
    {
        MenuId: menuId
    },
    function (data) {
        if (data > 0) {
            $('#menuTable').DataTable().ajax.reload();
        }
        else {
            alert('Try again');
        }
    });
}
function fnEdit(id) {
    $.get('/Menus/EditView',
        {
            id: id
        },
        function (data) {
            $("#entry-ui").removeClass('card card-info card-outline collapsed-card');
            $("#entry-ui").addClass('card card-info card-outline');
            $("#MenuId").val(data.menuId);
            $("#ActionName").val(data.actionName);
            $("#ControllerName").val(data.controllerName);
            $("#DisplayName").val(data.displayName);
            $("#IconTag").val(data.iconTag);
            if (data.menuUrl != "") {
                $("#MenuUrl").val(data.menuUrl);
                $("#MenuUrl").attr("disabled", "disabled");
            }
            else {
                $("#MenuUrl").val('');
                $("#MenuUrl").removeAttr("disabled");
            }
            if (data.isParentMenu == 0) {
                $('input[id="radioPrimary2"]').prop('checked', true);
                var id = data.parentMenuId;
                $.get("/Menus/GetParentMenu", function (data) {
                    if (data.length > 0) {
                        $('#ParentMenuId').removeAttr("disabled");
                        $("#ParentMenuId").empty();
                        $("#ParentMenuId").append('<option value="0">--Select--</option>');
                        for (var i = 0; i < data.length; i++) {
                            $('<option/>', {
                                value: data[i].menuId,
                                html: data[i].displayName
                            }).appendTo('#ParentMenuId');
                        }
                        $("#ParentMenuId").val(id);
                    }
                });
            }
            else {
                $('input[id="radioPrimary1"]').prop('checked', true);
                $('#ParentMenuId').attr("disabled", "disabled");
                $("#ParentMenuId").empty();
                $("#ParentMenuId").append('<option value="0">--Select--</option>');
            }
            $("#cardTitle").text('Update Menu');
            $("#btnSubmit").removeClass('btn-success');
            $("#btnSubmit").addClass('btn-warning');
            $("#btnSubmit").prop("value", "Update");
        }
    );
}