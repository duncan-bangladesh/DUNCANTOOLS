$(document).ready(function () {
    $('#btnSubmit').attr('disabled', 'disabled');
    $('#UserName').change(function () {
        var UserName = $('#UserName').val();
        var Password = $('#Password').val();
        if (UserName == '') {
            $("#UserName").css("border-color", "red");
            $("#UserName").focus();
            toastr.warning('User name is required, please input.');
            $('#btnSubmit').attr('disabled', 'disabled');
        }
        else {
            if (Password != "") {
                $('#btnSubmit').removeAttr('disabled');
            }
            $("#UserName").css("border-color", "#ced4da");
        }
    });
    $('#Password').keyup(function () {
        var UserName = $('#UserName').val();
        var Password = $('#Password').val();
        if (Password == '') {
            //$("#Password").css("border-color", "red");
            $("#Password").focus();
            //toastr.warning('Password is required, please input.');
            $('#btnSubmit').attr('disabled', 'disabled');
        }
        else {
            if (UserName != "") {
                $('#btnSubmit').removeAttr('disabled');
            }
            $("#Password").css("border-color", "#ced4da");
        }
    });
});