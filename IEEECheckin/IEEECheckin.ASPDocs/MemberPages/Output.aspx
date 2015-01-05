﻿<%@ Page Title="Output" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Output.aspx.cs" Inherits="IEEECheckin.ASPDocs.MemberPages.Output" %>
<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
    <style>
        td, th {
            padding-left: 10px;
            padding-right: 10px;
            width: 230px;
        }
        th {
            padding-bottom: 10px;
        }
        td {
            padding-bottom: 5px;
        }
        .col-lg-4 {
            width: 50.0%;
            margin-left: 25.0%;
        }
        #output {
            background: white;
            color: black;
            border: 1px solid;
        }
        .boxed-section {
            width: 50.0%;
            margin-left: 25.0%;
        }
    </style>
</asp:Content>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <h1 class="post-header">Meeting Output</h1>

    <p class="section-header">Filter Meeting Attendance <i class="fa fa-file-text"></i></p> 
    <div class="boxed-section margin-lg-after">
        <select class="selectpicker" id="ouputDropdown" onchange="selectionChanged()"></select>
    </div>

    <p class="section-header">Export <i class="fa fa-download"></i></p> 
    <div class="boxed-section margin-lg-after">
        <asp:HiddenField ID="SubmitData" runat="server" />
        <asp:HiddenField ID="MeetingName" runat="server" />
        <asp:HiddenField ID="MeetingDate" runat="server" />
        <asp:Button CssClass="form-control input-lg btn btn-info check-in" OnClick="SubmitCsv" Text="Get CSV" id="CsvButton" runat="server" />
        <br /><br />
        <asp:Button CssClass="form-control input-lg btn btn-info check-in" OnClick="SubmitJson" Text="Get JSON" id="JsonButton" runat="server" />
        <br /><br />
        <asp:Button CssClass="form-control input-lg btn btn-info check-in" PostBackUrl="~/MemberPages/GoogleDocLoading.aspx" Text="Add to Google" id="GoogleButton" runat="server" />
    </div>

    <p class="section-header">Clear Attendance <i class="fa fa-eraser"></i></p>
    <div class="boxed-section margin-lg-after">
        <button class="form-control input-lg btn btn-info check-in" onclick="return clearSubmit();" type="submit" id="Button1" name="outputbutton"><i class="fa fa-check"></i>Clear</button>
    </div>

    <div id="outputDiv">
        <table id="output">
            <tbody>
                <tr id="headerRow">
                    <th>First Name</th>
                    <th>Last Name</th>
                    <th>Student ID</th>
                    <th>Email</th>
                    <th>Meeting</th>
                    <th>Date</th>
                </tr>
            </tbody>
        </table>
    </div>
</asp:Content>
<asp:Content ID="JavaScriptContent" ContentPlaceHolderID="JavaScripts" runat="server">
    <script type="text/javascript">
        function dbOpenCallback() {
            try {
                getMeetings();
            }
            catch (err) {
                alert(err.message);
            }
        }
        function outputCallback(outputVal) {
            if (outputVal != null && outputVal.trim() != "") {
                outputVal = outputVal.replace('/"', '/');
                outputVal = "First Name,Last Name,Student ID,Email,Meeting,Date\n" + outputVal;
            }
        }
        function jsonCallback(outputVal) {
            var decoded = decodeMeeting($("#ouputDropdown option:selected").val());
            var data = { "data": outputVal };
            var dataVal = JSON.stringify(data);
            $("#MainContent_SubmitData").val(dataVal);
            $("#MainContent_MeetingName").val(decoded["name"]);
            $("#MainContent_MeetingDate").val(decoded["date"]);
        }
        function meetingCallback(meetings) {
            var htmlVal = "";
            for (var i = 0; i < meetings.length; i++) {
                htmlVal = htmlVal + "<option value='" + "meeting=" + meetings[i].meeting + "&" + "date=" + meetings[i].date + "'>" + meetings[i].meeting + " " + meetings[i].date + "</option>";
            }

            htmlVal = "<option value='meeting=&date='>All Meetings</option>" + htmlVal;
            $("#ouputDropdown").html(htmlVal);

            var decoded = decodeMeeting($("#ouputDropdown option:selected").val());
            createOutput("#output tr:last", decoded["name"], decoded["date"]);
            getJson("", "");
        }

        function selectionChanged() {
            var decoded = decodeMeeting($("#ouputDropdown option:selected").val());

            $("#output tr:not(#headerRow)").remove();
            createOutput("#output tr:last", decoded["name"], decoded["date"]);
            getJson(decoded["name"], decoded["date"]);
        }

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
                    alert(err.message);
                }
            }

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
    </script>
</asp:Content>
