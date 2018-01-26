var registerControl = function () {
    // Register form
    $("#register-form-wrapper").bind("submit", "register-form", function (e) {
        form = $(this).find("form");
        e.preventDefault();

        if (!form.valid()) {
            e.preventDefault();
            return false;
        }
        else {
            //$(".modal-register-form").find(".js-process-registerlogin").show();
            if (form.data('requestRunning')) {
                return;
            }
            form.data('requestRunning', true);

            $.ajax({
                url: "/",
                type: "POST",
                data: form.serialize(),
                success: function (response) {
                    $('#register-form-wrapper').html(response);
                },
                error: function (xhr, resp, text) {
                    console.log(xhr, resp, text);
                },
                complete: function () {
                    $(".modal-register-form").find(".js-process-registerlogin").hide();
                    form.data('requestRunning', false);
                }
            });
        }
    });

    // Register with FB form
    $("#register-form-wrapper").on("click", "#js-register-with-facebook", function (e) {
        $(".modal-register-form").find(".js-process-registerlogin").show();
        FB.login(function (response) {
            $(".modal-register-form").find(".js-process-registerlogin").hide();
            if (response.authResponse) {
                statusChangeCallback(response);
            }
        }, { scope: 'email,user_birthday,public_profile', return_scopes: true });
    });

    // Register Through Facebook form
    $("#register-facebook-wrapper").bind("submit", "register-fb-form", function (e) {
        form = $(this).find("form");
        e.preventDefault();

        if (!form.valid()) {
            e.preventDefault();
            return false;
        }
        else {
            $(".modal-facebookregister-form").find(".js-process-registerlogin").show();
            if (form.data('requestRunning')) {
                return;
            }
            form.data('requestRunning', true);
            $.ajax({
                url: "/",
                type: "POST",
                data: form.serialize(),
                success: function (response) {
                    if (response.Success !== undefined && response.Success === true) {
                        successfullLogin(response);
                    }
                    else {
                        $('#register-facebook-wrapper').html(response);
                    }
                },
                error: function (xhr, resp, text) {
                    console.log(xhr, resp, text);
                },
                complete: function () {
                    $(".modal-facebookregister-form").find(".js-process-registerlogin").hide();
                    form.data('requestRunning', false);
                }
            });
        }
    });

};
var checkIfLoggedIn = function () {
    $.get("/umbraco/Surface/AuthSurface/HomeLoggedIn?cpid=" + currentPageId,
        function (data) {
            if (data) {
                successfullLogin(data, true);
                bindEggLinks();
            }
            else {
                $("#primary-menu").addClass("nonLoggedIn");
            }

            //// check experiences page when loggedin
            //if (data && $("#js-myprofilepage-experiences").length > 0) {
            //    $.get("/umbraco/Surface/ExperiencesSurface/HasMemberExperience",
            //        function (data) {
            //            $("#js-myprofilepage-experiences").show();
            //        }
            //    );
            //}

            // resolove tab-profile if logged in.
            if (data && $("#tab-profile").length > 0 && document.location.hash === "" && $("#js-memberName").text() !== "") {
                resolveTab("profile");
            }

            // Check if in member profile page, if not logged in, force open login modal
            if (!data && document.location.href.indexOf("member-profile") > 1) {
                $('.modal-login-form').modal({ show: "fade" });
            }
        })
        .always(function () {
            $("#js-logging-in").hide();
        });
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