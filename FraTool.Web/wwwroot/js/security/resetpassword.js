$(document).ready(function () {
    $("#OldPassword").change(function () {
        var OldPassword = $("#OldPassword").val();
        if (OldPassword == '') {
            $("#OldPassword").css("border-color", "red");
            $("#OldPassword").focus();
            toastr.warning('Old password is required, please input.');
        }
        else {
            if (OldPassword.length < 8) {
                $("#OldPassword").css("border-color", "red");
                $("#OldPassword").focus();
                toastr.warning('Password should contain 8 or more characters.');
            }
            else {
                //$.post("@Url.Action("CheckUser","Accounts")",
                $.get("/Accounts/CheckPassword",
                    {
                        password: OldPassword
                    },
                    function (data) {
                        if (data > 0) {
                            $("#OldPassword").css("border-color", "green");
                            //toastr.success('This username is available, you can use.');
                        }
                        else {
                            $("#OldPassword").css("border-color", "red");
                            //$("#UserName").val("");
                            $("#OldPassword").focus();
                            toastr.error('Password is not match, try again.');
                        }
                    });
            }
        }
    });
    $("#NewPassword").change(function () {
        var OldPassword = $("#OldPassword").val();
        var Password = $("#NewPassword").val();
        if (Password == "") {
            $("#NewPassword").css("border-color", "red");
            $("#NewPassword").focus();
            toastr.warning('New password is required, please input.');
        }
        else {
            if (Password == OldPassword) {
                $("#NewPassword").css("border-color", "red");
                $("#NewPassword").focus();
                toastr.warning('New password should not same as old, try another.');
            }
            else {
                if (Password.length >= 8) {
                    $("#NewPassword").css("border-color", "green");
                }
                else {
                    $("#NewPassword").css("border-color", "red");
                    $("#NewPassword").focus();
                    toastr.warning('Password should contain 8 or more characters.');
                }
            }
        }
    });
    $("#ConfirmPassword").change(function () {
        var NewPassword = $("#NewPassword").val();
        var Password = $("#ConfirmPassword").val();
        if (Password == "") {
            $("#ConfirmPassword").css("border-color", "red");
            $("#ConfirmPassword").focus();
            toastr.warning('Confirmation password is required, please input.');
        }
        else {
            if (Password.length < 8) {
                $("#ConfirmPassword").css("border-color", "red");
                $("#ConfirmPassword").focus();
                toastr.warning('Password should contain 8 or more characters.');
            }
            else {
                if (Password != NewPassword) {
                    $("#ConfirmPassword").css("border-color", "red");
                    $("#ConfirmPassword").focus();
                    toastr.warning('Confirmation password should be same as new password.');
                }
                else {
                    $("#ConfirmPassword").css("border-color", "green");
                }
            }
        }
    });
    $("#btnSubmit").click(function () {
        var OldPassword = $("#OldPassword").val();
        var NewPassword = $("#NewPassword").val();
        var ConfirmPassword = $("#ConfirmPassword").val();
        var UserName = $("#UserName").val();
        var status = false;
        if (OldPassword == "") {
            $("#OldPassword").css("border-color", "red");
            $("#OldPassword").focus();
            toastr.warning('Old password is required, please input.');
            status = false;
        }
        else {
            status = true;
        }
        if (NewPassword == "") {
            $("#NewPassword").css("border-color", "red");
            $("#NewPassword").focus();
            toastr.warning('New password is required, please input.');
            status = false;
        }
        else {
            status = true;
        }
        if (ConfirmPassword == "") {
            $("#ConfirmPassword").css("border-color", "red");
            $("#ConfirmPassword").focus();
            toastr.warning('Confirmation password is required, please input.');
            status = false;
        }
        else {
            status = true;
        }
        if (status == true) {
            var user = {
                UserName: UserName,
                Password: NewPassword,
                OldPassword: OldPassword
            };
            $.post("/Users/ResetPassword", { user: user }, function (data) {
                if (data > 0) {
                    toastr.success('Password successfully updated, please login');
                    setTimeout(function () {
                        location.href = '/Accounts/Login';
                    }, 3000);
                }
            });
        }
    });
});