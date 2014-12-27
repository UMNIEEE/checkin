$(document).ready(function () {
    updateFormat();
    var dt = new Date();
    $(".footer").html("Powered by the U of M IEEE Tech Subcommittee. &copy; " + dt.getFullYear() + " - University of Minnesota IEEE Student Branch");
});
function updateFormat() {
    var bbc = $.cookie("body-background-color");
    if (bbc != null && bbc != undefined)
        $("body").css("background-color", "#" + bbc);

    var bubc = $.cookie("button-background-color");
    if (bubc != null && bubc != undefined)
        $("button").css("background-color", "#" + bubc);

    var bc = $.cookie("body-color"); // text color
    if (bc != null && bc != undefined) {
        $("body").css("color", "#" + bc);
        $("button").css("color", "#" + bc);
    }

    var iu = $.cookie("image-url");
    if (iu != null && iu != undefined)
        $("#logoImage").attr("src", iu);

    var ht = $.cookie("header-text");
    if (ht != null && ht != undefined)
        $("#topHeader").html(ht);

    var us = $.cookie("use-swipe");
    if (us != null && us != undefined && us === "false") {
        $("#swipe-section").attr("style", "display: none;");
    }
}