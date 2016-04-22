// callback for when database is opened
function dbOpenCallback() {
    try {
        getMeetings(); // get the meetings in the database
    }
    catch (err) {
    }
}

// callback for when the json is ready
function jsonCallback(outputVal) {
    var decoded = decodeMeeting($("#ouputDropdown option:selected").val());
    var data = { "data": outputVal };
    var dataVal = JSON.stringify(data);
    $("#MainContent_SubmitData").val(dataVal);
    $("#MainContent_MeetingName").val(decoded["name"]);
    $("#MainContent_MeetingDate").val(decoded["date"]);
}

// callback for when the list of meetings is ready, populate meeting selection dropdown
function meetingCallback(meetings) {
    var htmlVal = "";
    for (var i = 0; i < meetings.length; i++) {
        htmlVal = htmlVal + "<option value='" + "meeting=" + meetings[i].meeting + "&" + "date=" + meetings[i].date + "'>" + meetings[i].meeting + " " + meetings[i].date + "</option>";
    }

    htmlVal = "<option value='meeting=&date='>All Meetings</option>" + htmlVal;
    $("#ouputDropdown").html(htmlVal);

    // initially populated data with all meetings and dates
    var decoded = decodeMeeting($("#ouputDropdown option:selected").val());
    createOutput("#output tr:last", decoded["name"], decoded["date"]);
    getJson("", "");
}

// event handler for when meeting dropdown selection is changed
function selectionChanged() {
    var decoded = decodeMeeting($("#ouputDropdown option:selected").val());
    numRows = 1;
    $("#output tr:not(#headerRow)").remove();
    createOutput("#output tr:last", decoded["name"], decoded["date"]);
    getJson(decoded["name"], decoded["date"]);
}

// event handler for when clear button is clicked
function clearSubmit() {
    var decoded = decodeMeeting($("#ouputDropdown option:selected").val());

    var msg = "Are you sure you want to clear '";
    if (decoded["name"] != null && decoded["name"].trim() != "")
        msg = msg + decoded["name"].trim() + "'?";
    else
        msg = msg + "All'?"

    var r = confirm(msg);
    if (r == true) {
        try {
            if ((decoded["name"] != null && decoded["date"] != null) && (decoded["name"].trim() != "" && decoded["date"].trim() != ""))
                deleteItems(decoded["name"].trim(), decoded["date"].trim());
            else
                clearData();
        }
        catch (err) {
        }
    }

    return false;
}

// gets the meeting and date from the query encoded data string
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

function lotteryRoll() {
    var roll = Math.floor((Math.random() * (numRows - 1)) + 1);
    var firstName = $("#row" + roll + " td:nth-child(2)").text();
    var lastName = $("#row" + roll + " td:nth-child(3)").text();
    $("#WinnerBox").html("<h2>" + firstName + " " + lastName + "!</h2>");
    return false;
}