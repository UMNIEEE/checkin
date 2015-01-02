<%@ Page Title="Menu" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Menu.aspx.cs" Inherits="IEEECheckin.ASPDocs.MemberPages.Menu" %>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <p class="section-header">Meeting Options <i class="fa fa-list"></i></p>  
    <div class="boxed-section margin-lg-after">
        <p class="section-label">Select Existing Meeting<i class="fa fa-calendar-o"></i></p>
        <select class="selectpicker" id="meetingDropdown" name="meeting"></select>
        <p class="section-label">Create New Meeting<i class="fa fa-pencil"></i></p>
        <input class="form-control input-lg margin-sm-after" type="text" id="newmeeting" placeholder="Meeting Name">
        <button class="form-control input-lg btn btn-info check-in" id="meetingButton" onclick="return meetingSubmit();" type="submit" ><i class="fa fa-check"></i>New Meeting</button>
    </div>

    <p class="section-header">View Meeting Attendance <i class="fa fa-file-text"></i></p> 
    <div class="boxed-section margin-lg-after">
        <p class="section-label">Filter Meetings<i class="fa fa-filter"></i></p>
        <select class="selectpicker" id="ouputDropdown" name="meeting"></select>
        <button class="form-control input-lg btn btn-info check-in" onclick="return outputSubmit();" type="submit" id="outputbutton" name="outputbutton"><i class="fa fa-check"></i>View</button>
    </div>

    <p class="section-header">Clear Attendance <i class="fa fa-eraser"></i></p>
    <div class="boxed-section margin-lg-after">
        <p class="section-label">Filter Meetings<i class="fa fa-filter"></i></p>
        <select class="selectpicker" id="clearDropdown" name="meeting"></select>
        <button class="form-control input-lg btn btn-info check-in" onclick="return clearSubmit();" type="submit" id="Button1" name="outputbutton"><i class="fa fa-check"></i>Clear</button>
    </div>

    <p class="section-header">Format Appearance <i class="fa fa-cogs"></i></p>
    <div class="boxed-section margin-lg-after">
        <button class="form-control input-lg btn btn-info check-in" onclick="return formatSubmit();" type="submit" id="Button2" name="outputbutton"><i class="fa fa-check"></i>Format</button>
    </div>

    <p class="section-header">About <i class="fa fa-info-circle"></i></p>
    <div class="boxed-section margin-lg-after">
        <button class="form-control input-lg btn btn-info check-in" onclick="return aboutSubmit();" type="submit" id="Button3" name="outputbutton"><i class="fa fa-check"></i>About</button>
    </div>

</asp:Content>
<asp:Content ID="JavaScriptContent" ContentPlaceHolderID="JavaScripts" runat="server">
    <script type="text/javascript">
        $(document).ready(function () {
            $("#newmeeting").keydown(function (event) {
                if (event.keyCode == 13) {
                    meetingSubmit()
                }
            });
            $("#meetingSearch").keydown(function (event) {
                if (event.keyCode == 13) {
                    outputSubmit()
                }
            });
            $("#meetingClear").keydown(function (event) {
                if (event.keyCode == 13) {
                    clearSubmit()
                }
            });
                    
        });
        function dbOpenCallback() {
            try {
                getMeetings();
            }
            catch (err) {
                alert(err.message);
            }
        }
        function meetingCallback(meetings) {
            var htmlVal = "";
            for(var i = 0; i < meetings.length; i++) {
                htmlVal = htmlVal + "<option value='" + "meeting=" + meetings[i].meeting + "&" + "date=" + meetings[i].date + "'>" + meetings[i].meeting + " " + meetings[i].date + "</option>";
            }

            $("#meetingDropdown").html(htmlVal);

            var htmlVal = "<option value='meeting=&date='>All Meetings</option>" + htmlVal;
            $("#ouputDropdown").html(htmlVal);
            $("#clearDropdown").html(htmlVal);
        }

        function meetingSubmit() {
            var newUrl = "";
            var meeting = $("#newmeeting").val();
            var meetingDrop = $("#meetingDropdown option:selected").val()
            if (meeting === null || meeting === undefined || meeting.trim() === "" && meetingDrop !== null && meetingDrop !== undefined && meetingDrop.trim() !== "") {
                var meetUrl = $("#meetingDropdown option:selected").val().trim();
                var meet = "";
                var date = "";

                var sURLVariables = meetUrl.split('&');
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
            else if($("#newmeeting").val() !== null && $("#newmeeting").val() !== undefined && $("#newmeeting").val().trim() !== ""){
                var dt = new Date();
                var month = dt.getMonth() + 1;
                if (month < 10)
                    month = "0" + month;
                var day = dt.getDate();
                if (day < 10)
                    day = "0" + day;
                date = dt.getFullYear() + "-" + month + "-" + day;
                meet = $("#newmeeting").val().trim();
            }
            else {
                alert("Must select a meeting or create a new meeting.");
                return false;
            }

            newUrl = encodeURI("meeting=" + meet + "&date=" + date);
            $("#newmeeting").val("");
            window.location.href = "Checkin?" + newUrl;

            return false;
        }
        function outputSubmit() {
            var meet = encodeURI($("#ouputDropdown option:selected").val().trim());
            var newUrl = "Output";
            if (meet != null && meet.trim() != "")
                newUrl = newUrl + "?" + meet;

            window.location.href = newUrl;

            return false;
        }
        function clearSubmit() {
            var meetUrl = $("#clearDropdown option:selected").val().trim();
            var meet = "";
            var date = "";

            var sURLVariables = meetUrl.split('&');
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

            var msg = "Are you sure you want to clear '";
            if (meet != null && meet.trim() != "")
                msg = msg + meet.trim() + "'?";
            else
                msg = msg + "All'?"

            var r = confirm(msg);
            if (r == true) {
                try {
                    if ((meet != null && date != null) && (meet.trim() != "" && date.trim() != ""))
                        deleteItems(meet.trim(), date.trim());
                    else
                        clearData();
                }
                catch (err) {
                    alert(err.message);
                }
            }

            return false;
        }
        function formatSubmit() {
            window.location.href = "Format";
            return false;
        }
        function aboutSubmit() {
            window.location.href = "About";
            return false;
        }
    </script>
</asp:Content>
