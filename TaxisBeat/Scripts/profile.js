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