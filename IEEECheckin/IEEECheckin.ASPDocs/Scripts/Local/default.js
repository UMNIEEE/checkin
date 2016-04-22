$(document).ready(function () {
    $("#MainContent_MeetingName").keydown(function (e) {
        var charCode = e.charCode || e.keyCode || e.which;
        if (charCode == 13) {
            meetingSubmit();
        }
    });
});

function dbOpenCallback() {
    try {
        getMeetings();
    }
    catch (err) {
    }
}

function meetingCallback(meetings) {
    var htmlVal = "";
    for (var i = 0; i < meetings.length; i++) {
        htmlVal = htmlVal + "<option value='" + "meeting=" + meetings[i].meeting + "&" + "date=" + meetings[i].date + "'>" + meetings[i].meeting + " " + meetings[i].date + "</option>";
    }

    $("#meetingDropdown").html("<option value=''>New Meetings</option>" + htmlVal);
}

function meetingSubmit() {
    var dropdown = $("#meetingDropdown option:selected").val();
    var meeting = $("#MainContent_MeetingName").val();
    $("#MainContent_DropdownValue").val(dropdown);
    if ((dropdown == null || dropdown == undefined || dropdown.trim() === "") && (meeting == null || meeting == undefined || meeting.trim() === "")) {
        $('.alert-info').show();
        setTimeout(function () { $('.alert-info').hide(); }, 3000);
        return false;
    }

    $("#MainContent_MeetingButtonHidden").click();
    return false;
}

function decodeMeeting(meeting) {
    var meet = "", date = "";

    if (meeting !== null && meeting !== undefined && meeting.trim() !== "") {
        var sURLVariables = meeting.split('&');
        for (var i = 0; i < sURLVariables.length; i++) {
            var sParameterName = sURLVariables[i].split('=');
            if (sParameterName[0] == "meeting") {
                meet = sParameterName[1];
            }
        }
        for (var i = 0; i < sURLVariables.length; i++) {
            var sParameterName = sURLVariables[i].split('=');
            if (sParameterName[0] == "date") {
                date = sParameterName[1];
            }
        }
    }

    return { "name": meet, "date": date };
}