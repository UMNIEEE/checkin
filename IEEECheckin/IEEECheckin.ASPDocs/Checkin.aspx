<%@ Page Title="Check-in" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Checkin.aspx.cs" Inherits="IEEECheckin.ASPDocs.Checkin" %>

<%@ PreviousPageType VirtualPath="~/Default.aspx" %> 

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
    <script src="Scripts/Local/checkin.js"></script>
    <script src="Scripts/card-parser.js"></script>
    <script src="Scripts/checkin.js"></script>
</asp:Content>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <h1 class="post-header">Meeting Check-In for:</h1>
    <h2 id="meetingName" class="post-header"></h2>

    <div class="overlay" id="umnOverlay" style="display: none;">
        <div class="modal">
            <asp:Button runat="server" ID="overlayClose" Text="X" OnClientClick="return hideOverlay('#umnOverlay')" style="float: right;" />
            <div id="links" style="margin-top: 45px;"></div>
        </div>
    </div>
    <div class="alert alert-success" style="display: none">
        <strong>Success!</strong> Your entry has been saved.
    </div>
    <div class="alert alert-warning" style="display: none">
        <strong>Warning!</strong> <div class="alert-warning-msg"></div>
    </div>
    <div id="swipe-section">
        <p class="section-label">ID Card Swipe Entry <i class="fa fa-credit-card"></i></p>
        <div class="boxed-section margin-lg-after">
            <div class="form-group no-margin-after">
                <asp:HiddenField ID="MeetingName" runat="server" />
                <asp:HiddenField ID="MeetingDate" runat="server" />
                <input class="form-control input-lg margin-sm-after clearable" autofocus="autofocus" type="password" id="cardtxt" placeholder="Click here, then swipe your card">
            </div>
        </div>
    </div>
    <p class="section-label">Manual Entry <i class="fa fa-pencil"></i></p>
    <div class="boxed-section margin-lg-after">
        <input class="form-control input-lg margin-sm-after clearable" type="text" id="firstname" placeholder="First Name">
        <input class="form-control input-lg margin-sm-after clearable" type="text" id="lastname" placeholder="Last Name">
        <input class="form-control input-lg margin-sm-after clearable" type="text" id="email" placeholder="Email">
        <input class="form-control input-lg margin-sm-after clearable" type="hidden" id="studentid">
        <button class="form-control input-lg btn btn-info check-in" onclick="return entrySubmit();" type="submit" id="checkinbutton" name="checkinbutton"><i class="fa fa-check"></i> Check In</button>
    </div>
</asp:Content>
