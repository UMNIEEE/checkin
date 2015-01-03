<%@ Page Title="Google Doc Export" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="GoogleDocSelect.aspx.cs" Inherits="IEEECheckin.ASPDocs.MemberPages.GoogleDocSelect" %>

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
        <asp:TextBox class="form-control input-lg margin-sm-after" ID="NewDocument" placeholder="Meeting Name" runat="server" />
        <asp:Button CssClass="form-control input-lg btn btn-info check-in" ID="SubmitButton" OnClientClick="return submitGoogle()" Text="Select Doc" runat="server" />
    </div>
</asp:Content>
<asp:Content ID="JavaScriptContent" ContentPlaceHolderID="JavaScripts" runat="server">
    <script>
        function submitGoogle() {
            var data = {
                "submitData": $("#MainContent_SubmitData").val(),
                "meetingName": $("#MainContent_MeetingName").val(),
                "newDocumentName": $("#MainContent_NewDocument").val(),
                "selectedUri": $("#MainContent_SheetList option:selected").val(),
                "accessToken": $.cookie("GoogleTokenCode"),
                "refreshToken": $.cookie("GoogleRefreshCode")
            };
            var dataVal = JSON.stringify(data);
            $.ajax({
                type: "POST",
                url: "Output.aspx/SubmitGoogle",
                data: dataVal,
                contentType: 'application/json; charset=utf-8',
                dataType: "json",
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    alert("Request: " + XMLHttpRequest.toString() + "\n\nStatus: " + textStatus + "\n\nError: " + errorThrown);
                },
                success: function (data) {
                    alert(data)
                }
            });

            return false;
        }
    </script>
</asp:Content>
