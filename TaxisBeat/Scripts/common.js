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
