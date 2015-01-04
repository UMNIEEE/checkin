<%@ Page Title="Google Doc Export" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="GoogleDocSelect.aspx.cs" Inherits="IEEECheckin.ASPDocs.MemberPages.GoogleDocSelect" %>

<%@ PreviousPageType VirtualPath="~/MemberPages/Output.aspx" %> 

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
    <style>
    </style>
</asp:Content>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <h1 class="post-header">Google Doc Export</h1>
    
    <p class="section-header">Export <i class="fa fa-download"></i></p> 
    <div class="boxed-section margin-lg-after">
        <p class="section-label">Existing Worksheet<i class="fa fa-filter"></i></p>
        <ul>
            <li>Data will be added to a worksheet in the selected spreadsheet.</li>
            <li>A new worksheet will be created for each meeting.</li>
            <li>Best practice would be to save all meetings in a given year to the same spreadsheet.</li>
            <li>Spreadsheets can be shared and multiple users can write data to the spreadsheets.</li>
            <li>You can add rows and columns to the worksheets, but you must keep the names and position of the first row.</li>
            <li>Changing the first row may lead to unexpected results or data not being added.</li>
        </ul>
        <br />
        <asp:HiddenField ID="SubmitData" runat="server" />
        <asp:HiddenField ID="MeetingName" runat="server" />
        <asp:HiddenField ID="MeetingDate" runat="server" />
        <asp:XmlDataSource ID="SheetTreeDataSource" runat="server" />
        <asp:TreeView ID="SheetTree" DataSourceID="SheetTreeDataSource" runat="server">
            <DataBindings>
                <asp:TreeNodeBinding DataMember="GoogleFolder" TextField="Title" />
                <asp:TreeNodeBinding DataMember="GoogleSheet" TextField="Title" ValueField="FeedUri"/>
            </DataBindings>
        </asp:TreeView>
        <br /><br />
        <asp:TextBox class="form-control input-lg margin-sm-after" ID="NewDocument" placeholder="Meeting Name" runat="server" />
        <asp:Button CssClass="form-control input-lg btn btn-info check-in" ID="SubmitButton" OnClick="SubmitGoogle" Text="Select Doc" runat="server" />
    </div>
</asp:Content>
<asp:Content ID="JavaScriptContent" ContentPlaceHolderID="JavaScripts" runat="server">
</asp:Content>
