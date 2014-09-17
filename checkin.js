function meetingSubmit() {
    if($("#newmeeting").val() === null || $("#newmeeting").val() === undefined || $("#newmeeting").val().trim() === "") {
        var newUrl = encodeURI($("#meetingDropdown option:selected").text());
        $("#meetingButton").attr("value", "checkin.php?meeting=" + newUrl);
        $("#meetingForm").attr("action", "checkin.php?meeting=" + newUrl);
    }
    else {
        var newUrl = encodeURI($("#newmeeting").val());
        $("#meetingButton").attr("value", "checkin.php?meeting=" + newUrl);
        $("#meetingForm").attr("action", "checkin.php?meeting=" + newUrl);
    }
    return true;
}