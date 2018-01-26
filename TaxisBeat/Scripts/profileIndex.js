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
        if (day !== "" && month !== "" && year !== "") {
            $(".js-date-of-birth").valid();
        }
    });

    if ($("#js-profile-form-wrapper").length > 0) {
        loadDateOfBirth($("#js-profile-form-wrapper"));
    }
});
