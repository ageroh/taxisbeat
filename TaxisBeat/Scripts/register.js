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
                    //loadDateOfBirth(form);
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
                        loadDateOfBirth(form);
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

    var dtTemp = "";
    $("#register-form-wrapper,#register-facebook-wrapper").on("change", ".js-day, .js-month, .js-year", function () {
        let form = $(this).parent().parent().parent();
        let day = $(form).find(".js-day").val();
        let month = $(form).find(".js-month").val();
        let year = $(form).find(".js-year").val();
        dtTemp = day + "/" + month + "/" + year;

        if (dtTemp !== "" && dtTemp.match(/^(\d\d\/\d\d\/\d\d\d\d)$/)) {
            $(".js-date-of-birth").val(dtTemp);
        }
        else {
            $(".js-date-of-birth").val("");
        }
        if (day != "" && month != "" && year != "") {
            $(".js-date-of-birth").valid();
        }

        //isDateBirthValid(form);
    });

    if ($("#register-form-wrapper").length > 0 || $("#register-facebook-wrapper").length > 0) {
        loadDateOfBirthInit();
    }
};
var loadDateOfBirthInit = function () {
    loadDateOfBirth($("#register-form-wrapper"));
    loadDateOfBirth($("#register-facebook-wrapper"));
}
// load values if exists
var loadDateOfBirth = function (form) {
    if ($(form).find(".js-date-of-birth").val() !== "") {
        var split = $(form).find(".js-date-of-birth").val().split("/");
        $(form).find(".js-day").val(split[0]);
        $(form).find(".js-month").val(split[1]);
        $(form).find(".js-year").val(split[2]);
        //isDateBirthValid(form);
    }
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