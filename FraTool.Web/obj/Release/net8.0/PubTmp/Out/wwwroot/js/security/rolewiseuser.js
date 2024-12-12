$(document).ready(function () {
    getRoles();
    $.get('/Users/ActiveUsers', function (data) {
        if (data.length > 0) {
            for (var i = 0; i < data.length; i++) {
                var html =
                    '<div class="icheck-primary offset-md-4 col-md-8"><input type="checkbox" class="mychk" value="' + data[i].userId + '" id="UserId' + data[i].userId + '" name="UserId' + data[i].userId + '" >'
                    + '<label for="UserId' + data[i].userId + '">' + data[i].userName + '</label></div>';
                var container = $('#userlist');
                $(html).appendTo(container);
            }
        }
    });
    $("#AllCheckBox").change(function () {
        if (this.checked) {
            $("input[type='checkbox']").prop("checked", true);
        }
        else {
            $("input[type='checkbox']").prop("checked", false);
        }
    });
    $('#RoleId').change(function () {
        var RoleId = $('#RoleId').val();
        $("input[type='checkbox']").prop("checked", false);
        $.get('/Roles/UsersByRoleId', { RoleId: RoleId }, function (data) {
            if (data.length > 0) {
                for (var x = 0; x < data.length; x++) {
                    $("#UserId" + data[x].userId).prop("checked", true);
                }
            }
        });
    });
    var getUserList = function () {
        var list = [];
        $('.mychk:checkbox:checked').each(function () {
            list.push($(this).val());
        });
        return list;
    };
    $("#btnSubmit").click(function () {
        var status = false;
        var datalist = getUserList();
        var roleId = $("#RoleId").val();
        if (roleId == 0) {
            toastr.warning('Please select role.');
            status = false;
        }
        else {
            status = true;
        }
        //if (datalist.length == 0) {
        //    toastr.warning('Please select user.');
        //    status = false;
        //}
        //else {
        //    status = true;
        //}

        if (status == true) {
            $.post('/Roles/SaveUsersInRole', { RoleId: roleId, UserList: datalist }, function (data) {
                if (data > 0) {
                    toastr.success('User added to the role successfully');
                    //setTimeout(function () {
                    //    location.reload();
                    //}, 3000);
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