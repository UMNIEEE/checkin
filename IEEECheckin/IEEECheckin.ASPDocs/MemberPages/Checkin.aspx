<%@ Page Title="Check-in" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Checkin.aspx.cs" Inherits="IEEECheckin.ASPDocs.MemberPages.Checkin" %>

<%@ PreviousPageType VirtualPath="~/MemberPages/Menu.aspx" %> 

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <h1 class="post-header">Meeting Check-In for:</h1>
    <h2 id="meetingName" class="post-header"></h2>

    <div class="overlay" id="umnOverlay" style="display: none;">
        <div class="modal">
            <asp:Button runat="server" ID="overlayClose" Text="X" OnClientClick="return hideOverlay('#umnOverlay')" style="float: right;" />
            <div id="links" style="margin-top: 45px;"></div>
        </div>
    </div>
    <div id="swipe-section">
        <p class="section-label">U-Card Swipe Card Entry <i class="fa fa-credit-card"></i></p>
        <div class="boxed-section margin-lg-after">
            <div class="form-group no-margin-after">
                <asp:HiddenField ID="MeetingName" runat="server" />
                <asp:HiddenField ID="MeetingDate" runat="server" />
                <input class="form-control input-lg margin-sm-after" autofocus="autofocus" type="password" id="cardtxt" placeholder="Click here, then swipe your card">
            </div>
        </div>
    </div>
    <p class="section-label">Manual Entry <i class="fa fa-pencil"></i></p>
    <div class="boxed-section margin-lg-after">
        <input class="form-control input-lg margin-sm-after" type="text" id="firstname" placeholder="First Name">
        <input class="form-control input-lg margin-sm-after" type="text" id="lastname" placeholder="Last Name">
        <input class="form-control input-lg margin-sm-after" type="text" id="email" placeholder="Email">
        <input class="form-control input-lg margin-sm-after" type="hidden" id="studentid" placeholder="Student ID">
        <button class="form-control input-lg btn btn-info check-in" onclick="return entrySubmit();" type="submit" id="checkinbutton" name="checkinbutton"><i class="fa fa-check"></i> Check In</button>
    </div>
</asp:Content>
<asp:Content ID="JavaScriptContent" ContentPlaceHolderID="JavaScripts" runat="server">
    <script type="text/javascript">
        $(document).ready(function () {

            var us = $.cookie("use-swipe");
            if (us != null && us != undefined && us === "false") {
                $("#firstname").focus();
            }
            else {
                $("#cardtxt").focus();
            }

            $("#meetingName").html($("#MainContent_MeetingName").val());
            // subscribe to the keydown for the U-card input
            $("#cardtxt, #firstname, #lastname, #email").keydown(function (event) {
                if (event.keyCode == 13) {
                    entrySubmit()
                }
            });
        });
        // called when U-card parsing has completed
        function parseUCardComplete(uCard) {
            if (uCard === null || uCard === undefined)
                return false;

            $("#firstname").val(uCard["firstName"]);
            $("#lastname").val(uCard["lastName"]);
            $("#studentid").val(uCard["studentId"]);
        }
        // called when crawl parsing of a single person has completed
        function parseCrawlComplete(uPerson) {
            if (uPerson === null || uPerson === undefined)
                return;
            // add post crawl code here
        }
        // called when crawl parsing of multiple persons returned has completed
        // html - raw HTML of the search results that can be displayed in an overlay
        function searchResultSelected(html) {

        }
        function entrySubmit() {
            try {
                // perform the parse function if the U-card slot is not empty
                if ($("#cardtxt").val() !== null && $("#cardtxt").val() !== undefined && $("#cardtxt").val().trim() !== "") {
                    parse("#cardtxt");
                }

                // Change this to true to crawl the U of M directory for people
                var crawlUMN = false;
                if (crawlUMN) {
                    // create U of M people search URL query.
                    var urlPath = "http://www.umn.edu/lookup?SET_INSTITUTION=UMNTC&type=name&CN=" +
                        $("#firstname").val() + "+" + $("#lastname").val() + "&campus=a&role=any";

                    var returnVal = crawl(urlPath); // returns raw HTML of the search results that can be displayed in an overlay

                    $("#links").append(returnVal);

                    $("#crawlConfirm").focus();
                    $("#crawlConfirm").keyup(function () {
                        parseData();
                        return false;
                    });

                    var w = $("#modalDiv").width();
                    $("#modalDiv").css("margin-left", w / -2);
                    showOverlay("#umnOverlay");
                }

                var data = {
                    "firstname": $("#firstname").val().trim(),
                    "lastname": $("#lastname").val().trim(),
                    "studentid": $("#studentid").val().trim(),
                    "email": $("#email").val().trim(),
                    "meeting": $("#MainContent_MeetingName").val().trim(),
                    "date": $("#MainContent_MeetingDate").val().trim()
                };

                addData(data);

                // clear the form, ready for adding the next entry
                $("#firstname").val("");
                $("#lastname").val("");
                $("#studentid").val("");
                $("#email").val("");
            }
            catch (err) {
                alert(err.message);
            }

            var us = $.cookie("use-swipe");
            if (us != null && us != undefined && us === "false") {
                $("#firstname").focus();
            }
            else {
                $("#cardtxt").focus();
            }

            window.location.href = "Confirm.aspx";
            return false;
        }
    </script>
</asp:Content>
