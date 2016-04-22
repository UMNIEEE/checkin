﻿var successID;

$(document).ready(function () {
    setFocus();
    $("#meetingName").html($("#MainContent_MeetingName").val());
    // subscribe to the keydown events
    // move from firstname to lastname
    $("#firstname").keydown(function (e) {
        var charCode = e.charCode || e.keyCode || e.which;
        if (charCode == 13) {
            e.preventDefault();
            $("#lastname").focus();
            return false;
        }
    });
    // move from lastname to email
    $("#lastname").keydown(function (e) {
        var charCode = e.charCode || e.keyCode || e.which;
        if (charCode == 13) {
            e.preventDefault();
            $("#email").focus();
            return false;
        }
    });
    // submit the form
    $("#email, #cardtxt").keydown(function (e) {
        var charCode = e.charCode || e.keyCode || e.which;
        if (charCode == 13) {
            e.preventDefault();
            entrySubmit();
            return false;
        }
    });
});

function entrySubmit() {
    try {
        clearTimeout(successID);
        $('.alert-success').hide();

        // perform the parse function if the student id card input is not empty
        if (checkStr($("#cardtxt").val())) {
            // create/retrieve regular expression
            var regex = "^%(\\w+)\\^(\\d+)\\^{3}(\\d+)\\^(\\w+),\\s(?:([\\w\\s]+)\\s(\\w{1})\\?;|([\\w\\s]+)\\?;)(\\d+)=(\\d+)\\?$";
            var indices = {
                "firstName": "5,7",
                "lastName": "4",
                "middleName": "6",
                "studentId": "2",
                "email": "-1"
            };
            var re = $.cookie("card-regex");
            if (re != null && re != undefined) {
                var rejson = JSON.parse(re);
                if (checkStr(rejson["regex"]))
                    regex = rejson["regex"];
                if (rejson["indices"] != null && rejson["indices"] != undefined)
                    indices = rejson["indices"];
            }

            // parse the card
            var result = parseCard($("#cardtxt").val().trim(), regex, indices);
            if (result === null || result === undefined) {
                $('.alert-warning-msg').html("Failed to parse card data. Try again or use manual entry.")
                $('.alert-warning').show();
                setTimeout(function () { $('.alert-warning').hide(); }, 3000);
                clearForm();
                setFocus();
                return false;
            }

            // check to make sure first name, last name, and student id are present in the results
            var datas = ["firstName", "lastName", "studentId"];
            var datasReadable = ["First Name", "Last Name", "Student ID"];
            var index;
            for (index = 0; index < datas.length; index++) {
                if (!checkStr(result[datas[index]])) {
                    $('.alert-warning-msg').html("Missing " + datasReadable[index] + ".")
                    $('.alert-warning').show();
                    setTimeout(function () { $('.alert-warning').hide(); }, 3000);
                    clearForm();
                    setFocus();
                    return false;
                }
            }

            // check if email is present
            var emailVal = "";
            if (checkStr($("#email").val()))
                emailVal = $("#email").val().trim();

            // create new database entry
            var entry = {
                "firstname": result["firstName"],
                "lastname": result["lastName"],
                "studentid": result["studentId"],
                "email": emailVal,
                "meeting": $("#MainContent_MeetingName").val().trim(),
                "date": $("#MainContent_MeetingDate").val().trim()
            };

            // add data to the database
            if (!addData(entry)) {
                clearForm();
                setFocus();
                return false;
            }

            // clear the form, ready for adding the next entry
            clearForm();
            setFocus();

            $('.alert-success').show();
            successID = setTimeout(function () { $('.alert-success').hide(); }, 3000);
        }
            // student id card slot empty so check for valid manual entry
        else if (checkStr($("#firstname").val()) && checkStr($("#lastname").val())) {
            // check if email is present
            var emailVal = "";
            if (checkStr($("#email").val()))
                emailVal = $("#email").val().trim();
            // check if student id present (for whatever reason)
            var studentId = "";
            if (checkStr($("#studentid").val()))
                studentId = $("#studentid").val().trim();

            // create new database entry
            var entry = {
                "firstname": $("#firstname").val().trim(),
                "lastname": $("#lastname").val().trim(),
                "studentid": studentId,
                "email": emailVal,
                "meeting": $("#MainContent_MeetingName").val().trim(),
                "date": $("#MainContent_MeetingDate").val().trim()
            };

            // add data to the database
            if (!addData(entry)) {
                return false;
            }

            // clear the form, ready for adding the next entry
            clearForm();
            setFocus();

            $('.alert-success').show();
            successID = setTimeout(function () { $('.alert-success').hide(); }, 3000);
        }
            // both are invalid
        else {
            // card input is empty, but manual is not empty, but incomplete
            if (!checkStr($("#cardtxt").val()) && (checkStr($("#firstname").val()) || checkStr($("#lastname").val()))) {
                // firstname missing
                if (!checkStr($("#firstname").val()) && checkStr($("#lastname").val())) {
                    $('.alert-warning-msg').html("Missing First Name.")
                    $('.alert-warning').show();
                    setTimeout(function () { $('.alert-warning').hide(); }, 3000);
                    $("#firstname").focus();
                    return false;
                }
                    // lastname missing
                else if (!checkStr($("#lastname").val()) && checkStr($("#firstname").val())) {
                    $('.alert-warning-msg').html("Missing Last Name.")
                    $('.alert-warning').show();
                    setTimeout(function () { $('.alert-warning').hide(); }, 3000);
                    $("#lastname").focus();
                    return false;
                }
                    // something else missing
                else {
                    $('.alert-warning-msg').html("Missing Card or Manual Input Data.")
                    $('.alert-warning').show();
                    setTimeout(function () { $('.alert-warning').hide(); }, 3000);
                }
            }
                // card input and manual input are empty
            else {
                $('.alert-warning-msg').html("Missing Card or Manual Input Data.")
                $('.alert-warning').show();
                setTimeout(function () { $('.alert-warning').hide(); }, 3000);
            }

            setFocus();

            return false;
        }
    }
    catch (err) {
        setFocus();
    }

    return false;
}

// set focus based on saved cookies
function setFocus() {
    var us = $.cookie("use-swipe");
    if (us != null && us != undefined && us === "true") {
        $("#cardtxt").focus();
    }
    else {
        $("#firstname").focus();
    }
}

// clear the entries in the form
function clearForm() {
    $("input.clearable").each(function (index, element) {
        try {
            $(this).val("");
        }
        catch (err) {

        }
    });
}