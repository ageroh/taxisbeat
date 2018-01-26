document.getElementById('fb-login-form').addEventListener('submit', function (e) {
    e.preventDefault();
    FB.login(function (response) {
        if (response.authResponse) {
            statusChangeCallback(response);
        }
    }, { scope: 'email,user_birthday,public_profile', return_scopes: true });
}, false);

document.getElementById("js-close").addEventListener('click',
    function (e) {
        $(this).parent().find("#js-message").html("");
    },
    false);

var showMessageFacebookModal = function (message) {
    $("#js-facebookmodal-message").find("#js-message").html(message);
    $("#js-facebookmodal-message").modal("show");
};

var facebookRegister = function (userDetail) {
    $('.js-process-registerlogin').hide();
    fillInRegisterFacebook(userDetail, $('.modal-facebookregister-form'));
    $('.modal-facebookregister-form').modal("show");
};
var fillInRegisterFacebook = function (userDetail, form) {
    $(form).find("#Name").val(userDetail.Name);
    $(form).find("#Surname").val(userDetail.Surname);
    $(form).find("#Username").val(userDetail.Username);
    $(form).find("#js-register-email").val(userDetail.Email);
    $(form).find("#AccessToken").val(userDetail.AccessToken);
    $(form).find("#FacebookUserId").val(userDetail.UserId);
    if (userDetail.BirthDateFormat !== "") {
        $(form).find(".js-date-of-birth").val(userDetail.BirthDateFormat);
        loadDateOfBirth($(form));
    }
};
