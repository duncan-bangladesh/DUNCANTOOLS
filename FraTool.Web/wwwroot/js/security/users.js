$(document).ready(function () {
    var inc = 0;
    $('#userTable').DataTable({
        "ajax": {
            url: "/Users/GetAllUsers",
            dataSrc: ''
        },
        "columns": [
            { data: 'fullName' },
            { data: 'emailAddress' },
            { data: 'mobileNumber' },
            { data: 'companyName' },
            { data: "userName" },
            //{ data: "password" },
            {
                data: null,
                render: function (data) {
                    if (data.isActive == 1) {
                        inc++;
                        return '<div class="custom-control custom-switch"><input type="checkbox" onchange="changeStatus(' + data.userId + ')" class="custom-control-input" id="customSwitch' + inc + '" checked><label class="custom-control-label text-success" for="customSwitch' + inc + '">Active</label></div>';
                    } else if (data.isActive == 0) {
                        inc++;
                        return '<div class="custom-control custom-switch"><input type="checkbox" onchange="changeStatus(' + data.userId + ')" class="custom-control-input" id="customSwitch2' + inc + '" ><label class="custom-control-label text-danger" for="customSwitch2' + inc + '">Inactive</label></div>';
                    }
                }
            },
            {
                data: null,
                render: function (data) {
                    if (data.isActive == 1) {
                        return '<button type="button" class="btn btn1" onclick="fnEdit(' + data.userId + ')"><i class="fa fa-edit"</i></button>';
                    }
                    else {
                        //return '<button type="button" class="btn btn-default btn1 text-danger" disabled><i class="fas fa-eye-slash"></i></button>';
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
    $.get('/Company/ActiveCompanies', function (data) {
        if (data.length > 0) {
            $("#CompanyId").empty();
            $("#CompanyId").append('<option value="0">--Select--</option>');
            for (var i = 0; i < data.length; i++) {
                $('<option/>', {
                    value: data[i].companyId,
                    html: data[i].companyName
                }).appendTo('#CompanyId');
            }
        }
    });
    $("#FullName").change(function () {
        var FullName = $("#FullName").val();
        if (FullName == "") {
            $("#FullName").css("border-color", "red");
            $("#FullName").focus();
            toastr.warning('Name is required, please input.');
        }
        else {
            $("#FullName").css("border-color", "#ced4da");
        }
    });
    $("#EmailAddress").change(function () {
        var EmailAddress = $("#EmailAddress").val();
        if (EmailAddress != "") {
            if (IsEmail(EmailAddress) == false) {
                $("#EmailAddress").css("border-color", "red");
                $("#EmailAddress").focus();
                toastr.warning('Please input a valid email.');
                return false;
            }
            else {
                $("#EmailAddress").css("border-color", "#ced4da");
            }
        }
        else {
            $("#EmailAddress").css("border-color", "#ced4da");
        }
    });
    $("#UserName").change(function () {
        var userName = $("#UserName").val();
        if (userName == '') {
            $("#UserName").css("border-color", "red");
            $("#UserName").focus();
            toastr.warning('Username is required, please input.');
        }
        else {
            if (userName.length < 4) {
                $("#UserName").css("border-color", "red");
                $("#UserName").focus();
                toastr.warning('Username should contain 4 or more characters.');
            }
            else {
                $.get("/Accounts/CheckUser",
                {
                    name: userName
                },
                function (data) {
                    if (data == 0) {
                        $("#UserName").css("border-color", "green");
                        $("label[for='lblUserMessage']").css("color", "green");
                    }
                    else {
                        $("#UserName").css("border-color", "red");
                        $("#UserName").focus();
                        toastr.error('This username already used in, try another.');
                    }
                });
            }
        }
    });
    $("#Password").change(function () {
        var Password = $("#Password").val();
        if (Password == "") {
            $("#Password").css("border-color", "red");
            $("#Password").focus();
            toastr.warning('Password is required, please input.');
        }
        else {
            if (Password.length >= 8) {
                $("#Password").css("border-color", "#ced4da");
            }
            else {
                $("#Password").css("border-color", "red");
                $("#Password").focus();
                toastr.warning('Password should contain 8 or more characters.');
            }
        }
    });
    $("#btnSubmit").click(function () {
        var UserId = $("#UserId").val();
        var FullName = $("#FullName").val();
        var EmailAddress = $("#EmailAddress").val();
        var MobileNumber = $("#MobileNumber").val();
        var CompanyId = $("#CompanyId").val();
        var UserName = $("#UserName").val();
        var Password = $("#Password").val();
        //Update User Info
        if (UserId > 0) {
            var user = {
                UserId: UserId,
                FullName: FullName,
                EmailAddress: EmailAddress,
                MobileNumber: MobileNumber,
                CompanyId: CompanyId
            };
            $.post('/Users/Update',
                {
                    user: user
                },
                function (data) {
                    if (data > 0) {
                        toastr.success('User info updated successfully.');
                        setTimeout(function () {
                            $("#entry-ui").removeClass('card card-info card-outline');
                            $("#entry-ui").addClass('card card-info card-outline collapsed-card');
                            $('#userTable').DataTable().ajax.reload();
                            $("#FullName").val('');
                            $("#EmailAddress").val('');
                            $("#MobileNumber").val('');
                            $("#CompanyId").empty();
                            $("#CompanyId").append('<option value="0">--Select--</option>');
                            $.get('/Company/ActiveCompanies', function (data) {
                                if (data.length > 0) {
                                    for (var i = 0; i < data.length; i++) {
                                        $('<option/>', {
                                            value: data[i].companyId,
                                            html: data[i].companyName
                                        }).appendTo('#CompanyId');
                                    }
                                }
                            });
                            $("#UserName").val('');
                            $("#Password").val('');
                            $("#UserId").val('');
                            $("#btnSubmit").removeClass('btn-warning');
                            $("#btnSubmit").addClass('btn-success');
                            $("#btnSubmit").prop("value", "Save");
                            $("#divU").show();
                            $("#divP").show();
                            $("#cardTitle").text('Add New User');
                        }, 1000);
                    }
                }
            )
        }
        //Insert User Info
        else {
            var status = false;
            if (FullName == "") {
                $("#FullName").css("border-color", "red");
                $("#FullName").focus();
                toastr.warning('Name is required, please input.');
                status = false;
            }
            else {
                status = true;
            }
            if (status == true) {
                if (CompanyId == "0") {
                    $('#CompanyId').select();
                    toastr.warning('Company is required, please select.');
                    status = false;
                }
                else {
                    status = true;
                }
            }
            if (status == true) {
                if (UserName == "") {
                    $("#UserName").css("border-color", "red");
                    $("#UserName").focus();
                    toastr.warning('Username is required, please input.');
                    status = false;
                }
                else {
                    status = true;
                }
            }
            if (status == true) {
                if (Password == "") {
                    $("#Password").css("border-color", "red");
                    $("#Password").focus();
                    toastr.warning('Password is required, please input.');
                    status = false;
                }
                else {
                    status = true;
                }
            }
            if (status == true) {
                var user = {
                    FullName: FullName,
                    EmailAddress: EmailAddress,
                    MobileNumber: MobileNumber,
                    CompanyId: CompanyId,
                    UserName: UserName,
                    Password: Password
                };
                $.post("/Users/Create",
                    {
                        user: user
                    },
                    function (data) {
                        if (data > 0) {
                            toastr.success('User added successfully.');
                            setTimeout(function () {
                                $("#entry-ui").removeClass('card card-info card-outline');
                                $("#entry-ui").addClass('card card-info card-outline collapsed-card');
                                //location.reload();
                                $('#userTable').DataTable().ajax.reload();
                                $("#FullName").val('');
                                $("#EmailAddress").val('');
                                $("#MobileNumber").val('');
                                $("#CompanyId").empty();
                                $("#CompanyId").append('<option value="0">--Select--</option>');
                                $.get('/Company/ActiveCompanies', function (data) {
                                    if (data.length > 0) {
                                        for (var i = 0; i < data.length; i++) {
                                            $('<option/>', {
                                                value: data[i].companyId,
                                                html: data[i].companyName
                                            }).appendTo('#CompanyId');
                                        }
                                    }
                                });
                                $("#UserName").val('');
                                $("#UserName").css("border-color", "#ced4da");
                                $("#Password").val('');
                                $("#UserName").css("border-color", "#ced4da");
                            }, 1000);
                        }
                    });
            }
        }
    });
});
function changeStatus(userId) {
    $.post("/Users/ChangeStatus",
        {
            UserId: userId
        },
        function (data) {
            if (data > 0) {
                $('#userTable').DataTable().ajax.reload();
            }
            else {
                alert('try again');
            }
        });
}
function IsEmail(email) {
    var regex = /^([a-zA-Z0-9_\.\-\+])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$/;
    if (!regex.test(email)) {
        return false;
    } else {
        return true;
    }
}
function fnEdit(id) {
    $.get('/Users/EditView',
        {
            id: id
        },
        function (data) {
            $("#entry-ui").removeClass('card card-info card-outline collapsed-card');
            $("#entry-ui").addClass('card card-info card-outline');
            $("#UserId").val(data.userId);
            $("#FullName").val(data.fullName);
            $("#EmailAddress").val(data.emailAddress);
            $("#MobileNumber").val(data.mobileNumber);
            $("#UserName").val(data.userName);
            $("#Password").val(' ');
            $("#divU").hide();
            $("#divP").hide();
            var id = data.companyId;
            $.get('/Company/ActiveCompanies', function (x) {
                if (x.length > 0) {
                    $("#CompanyId").empty();
                    $("#CompanyId").append('<option value="0">--Select--</option>');
                    for (var i = 0; i < x.length; i++) {
                        $('<option/>', {
                            value: x[i].companyId,
                            html: x[i].companyName
                        }).appendTo('#CompanyId');
                    }
                    $("#CompanyId").val(id);
                }
            });
            $("#cardTitle").text('Change User Info');
            $("#btnSubmit").removeClass('btn-success');
            $("#btnSubmit").addClass('btn-warning');
            $("#btnSubmit").prop("value", "Update");
        }
    )
}