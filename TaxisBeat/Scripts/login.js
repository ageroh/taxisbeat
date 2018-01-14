var successfullLogin = function (response, alreadyLoggedIn) {
    $("#target-login-result").hide();
    $("#js-memberName").text(response.Data.MemberName);
    $("#js-member-loggedin .profilepage").attr("href", response.Data.MemberProfileUrl);
    $("#js-member-loggedin").show();
    $("#primary-menu").removeClass("nonLoggedIn");
    $("#primary-menu").addClass("loggedIn");

    // reload based on where member is ... maybe not?
    if (alreadyLoggedIn === false || alreadyLoggedIn === undefined) {
        if (document.location.href.indexOf("member-profile") > 1) { document.location.reload(); }
        if (document.location.href.indexOf("your-experience") > 1) { document.location.reload(); }
    }
    if (document.location.href.indexOf("email-verified") > 1) { document.location = "/"; }
    if (document.location.href.indexOf("reset-password") > 1) { document.location = "/"; }

    $('.modal-facebookregister-form').modal("hide");
    
    // refresh AF tokens hack
    $.post("/umbraco/Surface/AuthSurface/AntiFg", function (token) {
        $("input[name=__RequestVerificationToken").replaceWith(token);
        $("#js-logging-in").hide();
    });
};

var loginControl = function () {

    // Login form
    $("#login-form").bind("submit", function (e) {
        form = $(this);
        e.preventDefault();

        if (!form.valid()) {
            e.preventDefault();
            return false;
        }
        else {
            $(".modal-login-form").find(".js-process-registerlogin").fadeIn(400);
            if (form.data('requestRunning')) {
                return;
            }
            form.data('requestRunning', true);
            $.ajax({
                type: "POST",
                url: "/",
                dataType: "json",
                data: form.serialize(),
                success: function (response) {
                    if (response.Success && response.IsValidMember) {
                        successfullLogin(response);
                    }
                    else {
                        $("#target-login-result").html(response.ErrorMessage);
                        $("#target-login-result").show();
                    }
                },
                error: function (xhr, resp, text) {
                    console.log(xhr, resp, text);
                    $("#target-login-result").html("Critical Error.");
                    $("#target-login-result").show();
                    console.log("login data:", response);
                },
                complete: function () {
                    $(".modal-login-form").find(".js-process-registerlogin").fadeOut(400);
                    form.data('requestRunning', false);
                }
            });
        }
    });

    // close login modal, open passwordforget modal
    $(".modal-login-form").on("click", "#js-forgotpassword-btn", function (e) {
        $(".modal-login-form").modal("hide");
        $(".modal-forgotpassword").modal("show");
    });

    // close login modal, open register modal
    $(".modal-login-form").on("click", "#js-registerform-btn", function (e) {
        $(".modal-login-form").modal("hide");
        $(".modal-register-form").modal("show");
    });

    $("#js-forgot-password").bind("submit", function (e) {
        form = $(this);
        e.preventDefault();

        if (!form.valid()) {
            return false;
        }
        else {
            $(".icon-spin-forgot-form").show();
            if (form.data('requestRunning')) {
                return;
            }
            form.data('requestRunning', true);
            $.ajax({
                type: "POST",
                url: "/",
                dataType: "json",
                data: form.serialize(),
                success: function (response) {
                    if (response.Success) {
                        $("#target-forgot-result").hide();
                        $(form).hide();
                        $(form).parent().find("#js-success-forgot-pass").show();
                    }
                    else {
                        $("#target-forgot-result").html(response.ErrorMessage);
                        $("#target-forgot-result").show();
                    }
                },
                error: function (xhr, resp, text) {
                    console.log(xhr, resp, text);
                    $("#target-forgot-result").html("Critical Error.");
                    $("#target-forgot-result").show();
                },
                complete: function () {
                    $(".icon-spin-forgot-form").hide();
                    form.data('requestRunning', false);
                }
            });
        }
    });

    $("#js-form-container-partial").on('submit', "#js-reset-password-form", function (e) {
        form = $(this);
        e.preventDefault();

        $.ajax({
            url: "/umbraco/Surface/AuthSurface/HandleResetPassword/?resetGUID=" + getParameterByName("resetGUID") + "&cpid=" + currentPageId,
            type: "POST",
            data: form.serialize(),
            success: function (response) {
                $('#partial-replace-div').html(response);
            },
            error: function (xhr, resp, text) {
                console.log(xhr, resp, text);
            }
        });
    });

}