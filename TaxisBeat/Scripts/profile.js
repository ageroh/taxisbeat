var bindEggLinks = function () {
    $(".profilepage").bind("click", function (e) {
        e.preventDefault();
        window.location = $(this).attr("href") + "#" + $(this).data("hash");
        var hasHash = $.trim(window.location.hash);
        if (hasHash) {
            location.reload();
        }
    });
};


var bindForms = function () {

    $("#js-profile-form-wrapper").on('submit', "#js-profile-form", function (e) {
        form = $(this);
        e.preventDefault();
        if (!form.valid()) {
            e.stopPropagation();
            return false;
        }
        $("#js-profile-process").fadeIn(400);
        $.post("/umbraco/Surface/AuthSurface/IsLoggedIn", function (data) {
            if (data) {
                $.ajax({
                    url: "/",
                    type: "POST",
                    data: form.serialize(),
                    success: function (response) {
                        $('#js-profile-form-wrapper').html(response);

                        if ($(".js-errormsg").text().length <= 0 && $(".input-validation-error").length === 0) {
                            $("#js-update-success").fadeIn('fast');
                            setTimeout(function () {
                                $('#js-update-success').fadeOut('fast');
                            }, 3000);
                        }
                    },
                    error: function (xhr, resp, text) {
                        console.log(xhr, resp, text);
                    },
                    complete: function (x) {
                        $("#js-profile-process").fadeOut(300);
                    }
                });
            }
            else {
                document.location = "/signin";
            }
        });
    });

    // Upadate Participation
    $(".js-update-participation-wrapper").on("submit", ".js-update-participant", function (e) {
        var form = $(this);
        let wrapper = $(this).parent();
        let updatemessage = $(this).parent().parent().find(".js-update-part-success");
        let formprocess = $(this).parent().find(".form-process");
        e.preventDefault();
        if (!form.valid()) {
            return false;
        }
        $(formprocess).fadeIn(400);

        $.post("/umbraco/Surface/AuthSurface/IsLoggedIn", function (data) {
            if (data) {
                $.ajax({
                    url: "/",
                    type: "POST",
                    data: form.serialize(),
                    success: function (response) {
                        $(wrapper).html(response);

                        if ($(".js-errormsg").text().length <= 0 && $(".input-validation-error").length === 0) {
                            $(updatemessage).fadeIn('fast');
                            setTimeout(function () {
                                $(updatemessage).fadeOut('fast');
                            }, 3000);
                        }
                    },
                    error: function (xhr, resp, text) {
                        console.log(xhr, resp, text);
                    },
                    complete: function (x) {
                        toLocaleDateTime(wrapper);
                        bindProfileTabLink();
                        $(formprocess).fadeOut(300);
                    }
                });
            }
            else {
                // trigger login modal
                if ($("#js-modal-login-button").length > 0) {
                    $('.modal-login-form').modal({ show: "fade" });
                    $(formprocess).fadeOut(300);
                }
                else {
                    document.location = "/";
                }
            }
        });
    });
    
    $("#js-profile-upload-wrapper").on("click", "#js-upload-img", function (e) {
        e.preventDefault();
        if (!(document.getElementById("js-upload-file").files.length > 0
            && document.getElementById("js-upload-file").files[0].size > 50000)) {
            alert("Too small Image!");
            return;
        }
        $("#js-process-uploadphoto").show();
        var wrapper = $("#js-profile-upload-wrapper");
        var af = $(this).parent().find("#input[name=__RequestVerificationToken]").val();
        var imageData = $('.image-editor').cropit('export');
        $.post("/umbraco/Surface/AuthSurface/IsLoggedIn", function (data) {
            if (data) {
                if ($(wrapper).find("form").length > 0) {
                    var formData = new FormData();
                    formData.append('data', $("#js-upload-file")[0].files[0]);

                    $.ajax({
                        type: "POST",
                        url: "/umbraco/Surface/ProfileSurface/UploadExperienceMobile?compid=" + compid + "&expid=" + expid,
                        data: formData,
                        dataType: "json",
                        contentType: false,
                        processData: false,
                        success: experienceUploaded,
                        error: function (error) {
                            alert("Some Error occured, while upload photo.");
                        }
                    });
                }
                else {
                    $.post("/umbraco/Surface/ProfileSurface/UploadExperience?compid=" + compid + "&expid=" + expid,
                        {
                            data: imageData,
                            __RequestVerificationToken: af
                        }, experienceUploaded);
                }
            }
            else {
                // trigger login modal
                if ($("#js-modal-login-button").length > 0) {
                    $('.modal-login-form').modal({ show: "fade" });
                    $(btn).prop("disabled", false);
                    $("#js-process-uploadphoto").hide();
                }
                else {
                    document.location = "/";
                }
            }
        });
    });

    // is used to get related competition and experience ids in meber profile page.
    $('#upload-competition').on('show.bs.modal', function (e) {
        compid = e.relatedTarget.dataset.competitionid;
        expid = e.relatedTarget.dataset.experienceid;
    });

    // fix js.. plz god..
    bindProfileTabLink();
};

var bindProfileTabLink = function () {
    $(".js-profiletab").bind("click", function (e) {
        e.preventDefault();
        e.stopPropagation();
        if (window.location.hash !== "#tab-profile") {
            window.location = window.location.origin + window.location.pathname + "#tab-profile";
            location.reload();
            return;
        }
        location.reload();
    });
}

// Profile Page - Lazy Load Tabs
var resolveTab = function (tabname) {
    let lang = $(".tab-container").data("lang");
    $("#js-profile-tab-loader").fadeIn(200);
    $.get("/umbraco/Surface/ProfileSurface/" + tabname + "?lang=" + lang + "&cpid=" + currentPageId, )
        .done(function (data) {
            $("#tab-" + tabname).html(data);
            toLocaleDateTime(null);
            bindForms();
            $("#js-profile-tab-loader").fadeOut(200);
        })
        .error(function (e) {
            document.location = "/";
        });
};

var experienceUploaded = function (response) {
    $("#js-process-uploadphoto").hide();
    if (response.Status === "Success") {
        //$("#js-process-competitions").fadeIn(200);
        document.location = response.RedirectUrl;
        location.reload();
        //$("#tab-experiences")
        //    .load("/umbraco/Surface/ProfileSurface/Competitions", function (e) {
        //        $('#upload-competition').modal("hide");
        //        $("#js-process-competitions").fadeOut(200);
        //    });
    }
    else {
        $("#js-error").text(response.Message);
        $("#js-error").show();
        $("#js-process-competitions").fadeOut(200);
    }
};

$(document).ready(function () {

    $(".tab-nav a").on("click", function (e) {
        resolveTab($(this).data("tab"));
    });
    if (document.location.hash === "" && $("#js-memberName").text() !== "") {
        resolveTab("profile");
    }

    bindEggLinks();

    // handle reload in profile page with hash
    var hasHash = $.trim(window.location.hash);
    if (hasHash) {
        $('#tabs-profile a[href$="' + hasHash + '"]').trigger('click');
    }

    var dtTemp = "";
    $("#js-profile-form-wrapper").on("change", ".js-day, .js-month, .js-year", function () {
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

    if ($("#js-profile-form-wrapper").length > 0) {
        loadDateOfBirthInit();
    }
});
var loadDateOfBirthInit = function () {
    loadDateOfBirth($("#js-profile-form-wrapper"));
}
// load values if exists
var loadDateOfBirth = function (form) {
    if ($(form).find(".js-date-of-birth").length > 0) {
        if ($(form).find(".js-date-of-birth").val() !== "") {
            var split = $(form).find(".js-date-of-birth").val().split("/");
            $(form).find(".js-day").val(split[0]);
            $(form).find(".js-month").val(split[1]);
            $(form).find(".js-year").val(split[2]);
            //isDateBirthValid(form);
        }
    }
};
var compid = 0;
var expid = 0;