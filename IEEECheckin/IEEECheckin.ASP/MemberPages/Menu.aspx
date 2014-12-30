<%@ Page Title="Menu" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Menu.aspx.cs" Inherits="IEEECheckin.ASP.Menu" %>

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <h1 class="post-header">Meeting Menu</h1>
    <p class="section-header">Meeting Options <i class="fa fa-list"></i></p>  
    <!-- Insert action to redirect to checkin.php -->
    <div class="boxed-section margin-lg-after">
        <p class="section-label">Select Existing Meeting<i class="fa fa-calendar-o"></i></p>
        <asp:DropDownList class="selectpicker" ID="meetingDropdown" AutoPostBack="false" runat="server"></asp:DropDownList>
        <p class="section-label">Create New Meeting<i class="fa fa-pencil"></i></p>
        <asp:TextBox class="form-control input-lg margin-sm-after" type="text" ID="newMeeting" placeholder="Meeting Name" runat="server" />
        <asp:Button class="form-control input-lg btn btn-info check-in" ID="meetingButton" OnClick="Submit" Text="Start Meeting" runat="server" />
    </div>
</asp:Content>
<asp:Content runat="server" ID="JavaScriptContent" ContentPlaceHolderID="JavaScripts">
    <script type="text/javascript">
        // document ready
        $(document).ready(function () {
            // prevent form submit due to an enter key press.
            $("#MainContent_newMeeting").keydown(function (event) {
                if (event.keyCode == 13) {
                    event.preventDefault();
                    $("#MainContent_meetingButton").click();
                }
            });
        });
    </script>
</asp:Content>