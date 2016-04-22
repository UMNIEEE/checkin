﻿<%@ Page Title="Create Check-in" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="IEEECheckin.ASPDocs.Default" %>
<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
    <script src="Scripts/Local/default.js"></script>
</asp:Content>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <h1 class="post-header">Welcome</h1>
    <div class="alert alert-info" style="display: none">
        <strong>Info!</strong> Must select a meeting or create a new one.
    </div>
    <p class="text-center">Create a new meeting or select one of your previous meetings to begin.</p>

    <p class="section-header">Meeting Options <i class="fa fa-list"></i></p>  
    <div class="boxed-section margin-lg-after">
        <p class="section-label">Select Existing Meeting<i class="fa fa-calendar-o"></i></p>
        <select class="selectpicker" id="meetingDropdown" name="meeting"></select>
        <p class="section-label">Create New Meeting<i class="fa fa-pencil"></i></p>
        <asp:TextBox CssClass="form-control input-lg margin-sm-after" ID="MeetingName" placeholder="Meeting Name" runat="server" />
        <asp:HiddenField ID="DropdownValue" runat="server" />
        <asp:Button CssClass="form-control input-lg btn btn-info check-in" ID="MeetingButton" OnClientClick="return meetingSubmit();" Text="Start Meeting" runat="server" />
        <asp:Button CssClass="hidden" ID="MeetingButtonHidden" PostBackUrl="~/Checkin.aspx" Text="Start Meeting" runat="server" />

    </div>

</asp:Content>
