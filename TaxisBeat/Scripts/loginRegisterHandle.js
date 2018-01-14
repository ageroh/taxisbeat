$(document).ready(function () {
    // Initialize Registration Forms
    registerControl();
    loginControl();

    $('.modal-facebookregister-form').on('show.bs.modal', function (e) {
        console.log("FB register shown");
        $('.modal-login-form').modal("hide");
        $('.modal-register-form').modal("hide");
    });

});