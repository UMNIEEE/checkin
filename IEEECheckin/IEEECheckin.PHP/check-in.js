// Clears a clearable text field given by objName.
function clearBox(objName) {
    if (objName !== null && objName !== undefined)
        $("#MainContent_" + objName).val("");
    return false;
}
// Changes the table section visibility corresponding with the option checkbox provided.
function tableVisibility(checkBox) {
    if (checkBox === null || checkBox === undefined || checkBox == 0)
        return;
    var tableName = "#MainContent_" + $(checkBox).val();
    var isChecked = $(checkBox).attr("checked");
    if (isChecked === undefined) {
        $(tableName).attr("style", "visibility: collapse; display: none;");
    }
    else {
        $(tableName).attr("style", "visibility: visible; display: inherit;");
    }
}
// Hides or shows the options panel.
function showOptions(obj) {
    if (obj === null || obj === undefined)
        return;

    if ($(obj).attr("data-show") == "true") {
        $("#CheckBoxDiv").attr("style", "visibility: collapse; display: none;");
        $(obj).attr("data-show", false);
    }
    else {
        $("#CheckBoxDiv").attr("style", "visibility: visible; display: inherit;");
        $(obj).attr("data-show", true);
    }
}
// Shows a page overlay.
function showOverlay(obj) {
    if (obj === null || obj === undefined)
        return;
    $(obj).attr("style", "display: inherit;");
    $(".modal").each(function (idx, item) {
        var w = $(this).width();
        $(this).css("margin-left", w / -2);
    });
    return false;
}
// Hides a page overlay.
function hideOverlay(obj) {
    $(obj).attr("style", "display: none;");
    return false;
}
function clearHideOverlay(parent, overlay, errorOutput, validatable) {
    try {
        if (overlay !== null && overlay !== undefined)
            hideOverlay(overlay);
        if (errorOutput !== null && errorOutput !== undefined)
            $(errorOutput).empty();
        if (parent !== null && parent !== undefined) {
            $(parent).find("input[type!='submit']").each(function (idx, item) {
                try {
                    $(this).val("");
                }
                catch (err) {
                }
            });
            $(parent).find("select").each(function (idx2, item2) {
                try {
                    $(this).val("0");
                }
                catch (err) {
                }
            });
        }
        if (validatable !== null && validatable !== undefined) {
            var validate = $(validatable);
            $(validatable).each(function (idx3, item3) {
                try {
                    $(this).css("border", "none");
                }
                catch (err) {
                }
            });
        }
    }
    catch (err) {
    }
    return false;
}
$(document).ready(function () {
    // prevent form submit due to an enter key press.
    $(window).keydown(function (event) {
        if (event.keyCode == 13) {
            event.preventDefault();
            return false;
        }
    });
});