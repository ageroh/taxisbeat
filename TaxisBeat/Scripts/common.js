var toLocaleDateTime = function (parent) {
    let base = parent !== null ? parent : "body";
    $(base).find(".js-localdate").each(function () {
        let format = $(this).data("format");
        let locale = $(this).data("locale");
        let ms = $(this).data("localms");
        if (locale !== "en" || locale !== "") {
            moment.locale(locale);
        }
        var dt = moment(new Date(ms)).format(format);
        $(this).text(dt);
    });
};
var getParameterByName = function (name, url) {
    if (!url) url = window.location.href;
    name = name.replace(/[\[\]]/g, "\\$&");
    var regex = new RegExp("[?&]" + name + "(=([^&#]*)|&|#|$)"),
        results = regex.exec(url);
    if (!results) return null;
    if (!results[2]) return '';
    return decodeURIComponent(results[2].replace(/\+/g, " "));
};
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