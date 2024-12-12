$(document).ready(function () {
    getRoles();
    $.get('/Menus/ActiveMenus', function (data) {
        if (data.length > 0) {
            for (var i = 0; i < data.length; i++) {
                var html =
                    '<div class="icheck-primary offset-md-4 col-md-8"><input type="checkbox" class="mychk" value="'+data[i].menuId+'" id="MenuId' + data[i].menuId + '" name="MenuId' + data[i].menuId + '" >'
                    + '<label for="MenuId' + data[i].menuId + '">' + data[i].displayName+'</label></div>';
                var container = $('#menulist');
                $(html).appendTo(container);
            }
        }
    });
    $('#RoleId').change(function () {
        var RoleId = $('#RoleId').val();
        $("input[type='checkbox']").prop("checked", false);
        $.get('/Roles/MenusByRoleId', { RoleId: RoleId }, function (data) {
            if (data.length > 0) {
                for (var x = 0; x < data.length; x++) {
                    $("#MenuId" + data[x].menuId).prop("checked", true);
                }
            }
        });
    });
    $("#AllCheckBox").change(function () {
        if (this.checked) {
            $("input[type='checkbox']").prop("checked", true);
        }
        else {
            $("input[type='checkbox']").prop("checked", false);
        }
    });
    var getMenuList = function () {
        var list = [];
        $('.mychk:checkbox:checked').each(function () {
            list.push($(this).val());
        });
        return list;
    };
    $("#btnSubmit").click(function () {
        var status = false;
        var list = getMenuList();
        var roleId = $("#RoleId").val();
        if (roleId == 0) {
            toastr.warning('Please select role.');
            status = false;
        }
        else {
            status = true;
        }
        if (list.length == 0) {
            toastr.warning('Please select menu.');
            status = false;
        }
        else {
            status = true;
        }
        
        if (status == true) {            
            $.post('/Roles/SaveMenusInRole', { RoleId: roleId, MenuList: list }, function (data) {
                if (data > 0) {
                    toastr.success('Menus added to the role successfully');
                    $("input[type='checkbox']").prop("checked", false);
                    getRoles();
                }
            });
        }
    });
});
function getRoles() {
    $.get('/Roles/ActiveRoles', function (data) {
        if (data.length > 0) {
            $("#RoleId").empty();
            $("#RoleId").append('<option value="0">--Select--</option>');
            for (var i = 0; i < data.length; i++) {
                $('<option/>', {
                    value: data[i].roleId,
                    html: data[i].roleName
                }).appendTo('#RoleId');
            }
        }
    });
};