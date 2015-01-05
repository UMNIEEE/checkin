<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" validateRequest="false" CodeBehind="GoogleDocLoading.aspx.cs" Inherits="IEEECheckin.ASPDocs.MemberPages.GoogleDocLoading" %>

<%@ PreviousPageType VirtualPath="~/MemberPages/Output.aspx" %> 

<asp:Content ID="HeaderContent" ContentPlaceHolderID="HeadContent" runat="server">
    <style>
        .hide {
            display: none;
        }
    </style>
    <script type="text/javascript">
        function googleClick() {
            $("#MainContent_GoogleButton").click();
        }
    </script>
</asp:Content>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <h1 class="post-header">Loading Sheets</h1>
    <asp:HiddenField ID="SubmitData" runat="server" />
    <asp:HiddenField ID="MeetingName" runat="server" />
    <asp:HiddenField ID="MeetingDate" runat="server" />
    <asp:HiddenField ID="FolderTreeXml" runat="server" />
    <p>We are currently getting a list of your spreadsheets, please be patient as this may take some time.</p>
    <asp:Image ID="LoadingImage" ImageUrl="~/Images/ajax-loader.gif" runat="server" />
    <asp:Button CssClass="hide" PostBackUrl="~/MemberPages/GoogleDocSelect.aspx" ID="GoogleButton" runat="server" />

</asp:Content>
<asp:Content ID="JavaScriptContent" ContentPlaceHolderID="JavaScripts" runat="server">
    <script type="text/javascript">
        $(document).ready(function () {
            if (!isPostBack()) {
                var dataVal = JSON.stringify({ "accessToken": $.cookie("GoogleTokenCode"), "refreshToken": $.cookie("GoogleRefreshCode") });
                $.ajax({
                    type: "POST",
                    url: "GoogleDocLoading.aspx/InitTreeView",
                    data: dataVal,
                    contentType: 'application/json; charset=utf-8',
                    dataType: "json",
                    error: function (XMLHttpRequest, textStatus, errorThrown) {
                        alert("Request: " + XMLHttpRequest.toString() + "\n\nStatus: " + textStatus + "\n\nError: " + errorThrown);
                        returnValue = false;
                    },
                    success: function (data) {
                        if (data.d === "") {
                            alert("Could not retrieve data.")
                        }
                        else {
                            $("#MainContent_FolderTreeXml").val(data.d);
                            googleClick();
                        }
                    }
                });
            }
        });
        function isPostBack() { //function to check if page is a postback-ed one
            var isPostBackObject = document.getElementById('isPostBack');
            if (isPostBackObject != null)
                return true;
            else
                return false;
        }
    </script>
</asp:Content>
