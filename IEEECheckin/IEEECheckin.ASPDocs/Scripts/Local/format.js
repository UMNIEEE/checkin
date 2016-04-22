$(document).ready(function () {
    pageLoaded();
});

function pageLoaded() {

    for (var i = 0; i < predefThemes.length; i++) {
        var opt = $("<option></option>");
        opt.attr("value", predefThemes[i].theme);
        opt.html(predefThemes[i].name);
        $("#MainContent_themeDropdown").append(opt);
    }

    for (var i = 0; i < predefRegex.length; i++) {
        var opt = $("<option></option>");
        opt.attr("value", predefRegex[i].regex);
        opt.html(predefRegex[i].name);
        $("#MainContent_regexDropdown").append(opt);
    }

    var us = $.cookie("use-swipe");
    if (us != null && us != undefined && us === "true") {
        $("#swipeCheck").prop("checked", true);
    }
    else {
        $("#swipeCheck").prop("checked", false);
    }

    try {
        var re = $.cookie("card-regex");
        if (re != null && re != undefined) {
            var rejson = JSON.parse(re);
            if (checkStr(rejson["name"])) {
                var regName = rejson["name"];
                $("#MainContent_regexDropdown option").filter(function () { return $(this).html() == regName }).attr('selected', 'selected');
            }
        }
        else {
            $("#MainContent_regexDropdown option").filter(function () { return $(this).html() == "No Card Format" }).attr('selected', 'selected');
        }
    }
    catch (err) {

    }

    try {
        var tn = $.cookie("theme-name");
        if (checkStr(tn)) {
            $("#MainContent_themeDropdown option").filter(function () { return $(this).html() == tn }).attr('selected', 'selected');
        }
        else {
            $("#MainContent_themeDropdown option").filter(function () { return $(this).html() == "Custom Theme" }).attr('selected', 'selected');
        }
    }
    catch (err) {

    }

    var ts = $.cookie("theme-shade");
    if (ts != null && ts != undefined && ts === "light") {
        $("#themeShade").prop("checked", false);
        $("#themeShadeText").html("Using Light Theme");
    }
    else {
        $("#themeShade").prop("checked", true);
        $("#themeShadeText").html("Using Dark Theme");
    }

    var bbc = $.cookie("body-background-color");
    if (bbc == null || bbc == undefined)
        bbc = "ffffff";

    var bubc = $.cookie("button-background-color");
    if (bubc == null || bubc == undefined)
        bubc = "ffffff";

    var bc = $.cookie("body-color"); // text color
    if (bc == null || bc == undefined)
        bc = "000000";

    $('#colorSelector').ColorPicker({ // body background
        color: bbc,
        onShow: function (colpkr) {
            $(colpkr).fadeIn(500);
            return false;
        },
        onHide: function (colpkr) {
            $(colpkr).fadeOut(500);
            return false;
        },
        onChange: function (hsb, hex, rgb) {
            $("body").css("background-color", "#" + hex);
            $.cookie("body-background-color", hex, { expires: 365, path: "/" });
            $("#colorSelector").css("background-color", "#" + hex);
            themeUpdated();
        }
    });
    $('#colorSelector2').ColorPicker({ // button color
        color: bubc,
        onShow: function (colpkr) {
            $(colpkr).fadeIn(500);
            return false;
        },
        onHide: function (colpkr) {
            $(colpkr).fadeOut(500);
            return false;
        },
        onChange: function (hsb, hex, rgb) {
            $("button").css("background-color", "#" + hex);
            $.cookie("button-background-color", hex, { expires: 365, path: "/" });
            reduced = parseInt(hex, 16);
            if (hex & 0x30 >= 0x0A0000)
                reduced = reduced - 0x0A0000;
            if (hex & 0x0C >= 0x000A00)
                reduced = reduced - 0x000A00;
            if (hex & 0x03 >= 0x00000A)
                reduced = reduced - 0x00000A;
            var temp = ("000000" + reduced.toString(16)).substr(-6)
            $("button").css("border-color", "#" + temp);
            $("#colorSelector2").css("background-color", "#" + hex);
            themeUpdated();
        }
    });
    $('#colorSelector3').ColorPicker({ // body (font) color
        color: bc,
        onShow: function (colpkr) {
            $(colpkr).fadeIn(500);
            return false;
        },
        onHide: function (colpkr) {
            $(colpkr).fadeOut(500);
            return false;
        },
        onChange: function (hsb, hex, rgb) {
            $("body").css("color", "#" + hex);
            $("button").css("color", "#" + hex);
            $.cookie("body-color", hex, { expires: 365, path: "/" });
            $("#colorSelector3").css("background-color", "#" + hex);
            $("[class*='boxed-section']").css("border", "1px dashed #" + bc);
            themeUpdated();
        }
    });

    themeUpdated();
}

function updateImage() {
    var value = $("#imageUrl").val();
    if (value != null && value.toLowerCase().trim() === "ieee") {
        $("#logoImage").attr("src", "../Images/logo.svg");
        $.cookie("image-url", "../Images/logo.svg", { expires: 365, path: "/" });
    }
    else if (value != null) {
        $("#logoImage").attr("src", value);
        $.cookie("image-url", value, { expires: 365, path: "/" });
    }

    themeUpdated();
    return false;
}

function updateTopText() {
    var value = $("#topText").val();
    if (value != null) {
        $("#topHeader").html(value);
        $.cookie("header-text", value, { expires: 365, path: "/" });
    }
    themeUpdated();
    return false;
}

function updateSwipe() {
    if ($("#swipeCheck").prop("checked")) {
        $.cookie("use-swipe", true, { expires: 365, path: "/" });
        $("#swipe-section").attr("style", "display: inherit;");
    }
    else {
        $.cookie("use-swipe", false, { expires: 365, path: "/" });
        $("#swipe-section").attr("style", "display: none;");
    }
    themeUpdated();
    return false;
}

function updateThemeShade() {
    if ($("#themeShade").prop("checked")) {
        $.cookie("theme-shade", "dark", { expires: 365, path: "/" });
        $("#themeShadeText").html("Using Dark Theme");
    }
    else {
        $.cookie("theme-shade", "light", { expires: 365, path: "/" });
        $("#themeShadeText").html("Using Light Theme");
    }
    themeUpdated();
    return false;
}

function updateRegex() {
    var sel = $("#MainContent_regexDropdown option:selected").val();
    if (checkStr(sel))
        $.cookie("card-regex", sel, { expires: 365, path: "/" });
    else {
        $.removeCookie("card-regex", { path: '/' });
        $("#swipeCheck").prop("checked", false);
        updateSwipe();
    }
    return false;
}

function updatePreDefTheme() {
    var sel = $("#MainContent_themeDropdown option:selected").val();
    var name = $("#MainContent_themeDropdown option:selected").text();
    if (checkStr(sel)) {
        $.cookie("theme-name", name, { expires: 365, path: "/" });
        $("#export").val(sel);
        importTheme();
    }
    else {
        $.cookie("theme-name", name, { expires: 365, path: "/" });
        $.removeCookie("theme-name", { path: '/' });
    }
    return false;
}

function resetTheme() {
    $.removeCookie("body-background-color", { path: '/' });
    $.removeCookie("button-background-color", { path: '/' });
    $.removeCookie("background-color", { path: '/' });
    $.removeCookie("body-color", { path: '/' });
    $.removeCookie("theme-shade", { path: '/' });
    $.removeCookie("image-url", { path: '/' });
    $.removeCookie("header-text", { path: '/' });
    $.removeCookie("use-swipe", { path: "/" });
    $.removeCookie("theme-name", { path: '/' });
    pageLoaded();
    location.reload();
    return false;
}

function themeUpdated() {
    var bbc = $.cookie("body-background-color");
    if (bbc == null || bbc == undefined)
        bbc = "ffffff";
    $("#colorSelector").css("background-color", "#" + bbc);

    var bubc = $.cookie("button-background-color");
    if (bubc == null || bubc == undefined)
        bubc = "ffffff";
    $("#colorSelector2").css("background-color", "#" + bubc);

    var bc = $.cookie("body-color"); // text color
    if (bc == null || bc == undefined)
        bc = "000000";
    $("#colorSelector3").css("background-color", "#" + bc);

    var ts = $.cookie("theme-shade");
    if (ts == null || ts == undefined)
        ts = "dark";
    if (ts === "dark") {
        $("#themeShade").prop("checked", true);
        $("#themeShadeText").html("Using Dark Theme");
    }
    else {
        $("#themeShade").prop("checked", false);
        $("#themeShadeText").html("Using Light Theme");
    }

    var iu = $.cookie("image-url");
    if (iu == null || iu == undefined)
        iu = "";
    $("#imageUrl").val(iu);

    var ht = $.cookie("header-text");
    if (ht == null || ht == undefined)
        ht = "Meeting Check-in Web App";
    $("#topText").val(ht);

    var us = $.cookie("use-swipe");
    if (us == null || us == undefined)
        us = "false";
    if (us === "true") {
        $("#swipeCheck").prop("checked", true);
    }
    else {
        $("#swipeCheck").prop("checked", false);
    }

    var theme = {
        bodyBackgroundColor: bbc,
        buttonBackgroundColor: bubc,
        bodyColor: bc,
        themeShade: ts,
        imageUrl: iu,
        headerText: ht,
        useSwipe: us
    };
    $("#export").val(JSON.stringify(theme));

    updateFormat();
}

function importTheme() {
    try {
        var theme = jQuery.parseJSON($("#export").val());
        if (theme == null || theme == undefined)
            return false;

        var bbc = theme.bodyBackgroundColor;
        if (bbc != null && bbc != undefined)
            $.cookie("body-background-color", bbc, { expires: 365, path: "/" });

        var bubc = theme.buttonBackgroundColor;
        if (bubc != null && bubc != undefined)
            $.cookie("button-background-color", bubc, { expires: 365, path: "/" });

        var bc = theme.bodyColor;
        if (bc != null && bc != undefined)
            $.cookie("body-color", bc, { expires: 365, path: "/" });

        var ts = theme.themeShade;
        if (ts != null && ts != undefined)
            $.cookie("theme-shade", ts, { expires: 365, path: "/" });

        var iu = theme.imageUrl;
        if (iu != null && iu != undefined)
            $.cookie("image-url", iu, { expires: 365, path: "/" });

        var ht = theme.headerText;
        if (ht != null && ht != undefined)
            $.cookie("header-text", ht, { expires: 365, path: "/" });

        var us = theme.useSwipe;
        if (us != null && us != undefined)
            $.cookie("use-swipe", us, { expires: 365, path: "/" });

        themeUpdated();
    }
    catch (err) {
    }
}