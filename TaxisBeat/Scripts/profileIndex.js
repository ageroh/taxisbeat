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
$(document).ready(function () {
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
    });

    if ($("#js-profile-form-wrapper").length > 0) {
        loadDateOfBirth($("#js-profile-form-wrapper"));
    }
});
