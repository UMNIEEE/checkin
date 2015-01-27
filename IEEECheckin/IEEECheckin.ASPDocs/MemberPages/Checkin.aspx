<%@ Page Title="Check-in" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Checkin.aspx.cs" Inherits="IEEECheckin.ASPDocs.MemberPages.Checkin" %>

<%@ PreviousPageType VirtualPath="~/MemberPages/CreateCheckin.aspx" %> 

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
            setFocus();
            $("#meetingName").html($("#MainContent_MeetingName").val());
            // subscribe to the keydown for the U-card input
            $("#cardtxt, #firstname, #lastname, #email").keydown(function (event) {
                if (event.keyCode == 13) {
                    entrySubmit()
                }
            });
        });
        function entrySubmit() {
            try {
                // perform the parse function if the student id card input is not empty
                if (checkStr($("#cardtxt").val())) {
                    var regEx = "^%(\\w+)\\^(\\d+)\\^{3}(\\d+)\\^(\\w+),\\s([\\w\\s]+)\\?;(\\d+)=(\\d+)\\?$";
                    var indicies = {
                        "firstName": "5",
                        "lastName": "4",
                        "middleInit": "-1",
                        "studentId": "2",
                        "email": "-1"
                    };
                    var result = parseUMN($("#cardtxt").val().trim(), regEx, indicies);

                    if (result === null || result === undefined) {
                        alert("Failed to parse card data. Try again or use manual entry.");
                        setFocus();
                        return false;
                    }

                    // check to make sure first name, last name, and student id are present in the results
                    var datas = ["firstName", "lastName", "studentId"];
                    var datasReadable = ["First Name", "Last Name", "Student ID"];
                    var index;
                    for (index = 0; index < datas.length; index++) {
                        if (!checkStr(result[datas[index]])) {
                            alert("Missing " + datasReadable[index] + ".");
                            setFocus();
                            return false;
                        }
                    }

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
                    addData(entry);

                    // clear the form, ready for adding the next entry
                    clearForm();
                    setFocus();

                    // navigate to the confirmation page
                    window.location.href = "Confirm.aspx";
                }
                // student id card slot empty so check for valid manual entry
                else if (checkStr($("#firstname").val()) && checkStr($("#lastname").val()) && checkStr($("#studentid").val())) {
                    var emailVal = "";
                    if (checkStr($("#email").val()))
                        emailVal = $("#email").val().trim();

                    // create new database entry
                    var entry = {
                        "firstname": $("#firstname").val().trim(),
                        "lastname": $("#lastname").val().trim(),
                        "studentid": $("#studentid").val().trim(),
                        "email": emailVal,
                        "meeting": $("#MainContent_MeetingName").val().trim(),
                        "date": $("#MainContent_MeetingDate").val().trim()
                    };

                    // add data to the database
                    addData(entry);

                    // clear the form, ready for adding the next entry
                    clearForm();
                    setFocus();

                    // navigate to the confirmation page
                    window.location.href = "Confirm.aspx";
                }
                // both are invalid
                else {
                    if (!checkStr($("#cardtxt").val())) {
                        if (checkStr($("#firstname").val()))
                            alert("Missing First Name.");
                        else if(checkStr($("#lastname").val()))
                            alert("Missing Last Name.");
                        else if(checkStr($("#studentid").val()))
                            alert("Missing Student ID.");
                    }
                    else
                        alert("Missing Card or Manual Input Data.")
                    setFocus();
                    return false;
                }
            }
            catch (err) {
                alert(err.message);
                setFocus();
            }

            return false;
        }
        function setFocus() {
            var us = $.cookie("use-swipe");
            if (us != null && us != undefined && us === "false") {
                $("#firstname").focus();
            }
            else {
                $("#cardtxt").focus();
            }
        }
        function clearForm() {
            $("#cardtxt").val("");
            $("#firstname").val("");
            $("#lastname").val("");
            $("#studentid").val("");
            $("#email").val("");
        }
    </script>
</asp:Content>
