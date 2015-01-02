<%@ Page Title="Google Doc Export" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="GoogleDocSelect.aspx.cs" EnableSessionState="True" Inherits="IEEECheckin.ASPDocs.MemberPages.GoogleDocSelect" %>

<%@ PreviousPageType VirtualPath="~/MemberPages/Output.aspx" %> 

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
    <style>
        #MainContent_SheetList {
            color: #000000;
            height: 300px;
        }
    </style>
</asp:Content>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <h1 class="post-header">Google Doc Export</h1>
    
    <p class="section-header">Export <i class="fa fa-download"></i></p> 
    <div class="boxed-section margin-lg-after">
        <p class="section-label">Existing Worksheet<i class="fa fa-filter"></i></p>
        <asp:TextBox CssClass="form-control input-lg margin-sm-after" ID="SubmitData" runat="server" />
        <asp:TextBox CssClass="form-control input-lg margin-sm-after" ID="MeetingName" runat="server" />
        <asp:ListBox ID="SheetList" runat="server"></asp:ListBox>
        <asp:TextBox class="form-control input-lg margin-sm-after" ID="newDocument" placeholder="Meeting Name" runat="server" />
        <asp:Button CssClass="form-control input-lg btn btn-info check-in" ID="submitButton" OnClick="SubmitGoogle" Text="Select Doc" runat="server" />
    </div>
</asp:Content>
<asp:Content ID="JavaScriptContent" ContentPlaceHolderID="JavaScripts" runat="server">
</asp:Content>
