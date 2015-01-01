<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Output.aspx.cs" Inherits="IEEECheckin.ASPDocs.MemberPages.Output" %>
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
        #MainContent_output {
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

    <p class="section-header">Download <i class="fa fa-download"></i></p> 
    <div class="boxed-section margin-lg-after">
        <asp:HiddenField id="format" runat="server" />
        <asp:HiddenField id="submitData" runat="server" />
        <asp:HiddenField id="meeting" runat="server" />
        <asp:Button CssClass="form-control input-lg btn btn-info check-in" OnClick="SubmitCsv" Text="Get CSV" id="csvbutton" runat="server" />
        <br /><br />
        <asp:Button CssClass="form-control input-lg btn btn-info check-in" OnClick="SubmitJson" Text="Get JSON" id="jsonbutton" runat="server" />
        <br /><br />
        <asp:Button CssClass="form-control input-lg btn btn-info check-in" OnClick="SubmitGoogle" Text="Add to Google" id="googlebutton" runat="server" />
    </div>

    <div id="outputDiv">
        <asp:Table id="output" runat="server">
            <asp:TableHeaderRow>
                <asp:TableHeaderCell>First Name</asp:TableHeaderCell>
                <asp:TableHeaderCell>Last Name</asp:TableHeaderCell>
                <asp:TableHeaderCell>Student ID</asp:TableHeaderCell>
                <asp:TableHeaderCell>Email</asp:TableHeaderCell>
                <asp:TableHeaderCell>Meeting</asp:TableHeaderCell>
                <asp:TableHeaderCell>Date</asp:TableHeaderCell>
            </asp:TableHeaderRow>
        </asp:Table>
    </div>
</asp:Content>
<asp:Content ID="JavaScriptContent" ContentPlaceHolderID="JavaScripts" runat="server">
    <script type="text/javascript">
        function dbOpenCallback() {
            try {
                createOutput("#MainContent_output tr:last", decodeURI(GetQueryStringParams("meeting")), decodeURI(GetQueryStringParams("date")));
                getJson(decodeURIComponent(GetQueryStringParams("meeting")), decodeURI(GetQueryStringParams("date")));
                var meeting = decodeURIComponent(GetQueryStringParams("meeting")) + " " + decodeURIComponent(GetQueryStringParams("date"));
                $("#MainContent_meeting").val(meeting);
            }
            catch (err) {
                alert(err.message);
            }
        }
        function cursorCallback(outputVal) {
            if (outputVal != null && outputVal.trim() != "") {
                outputVal = outputVal.replace('/"', '/');
                outputVal = "First Name,Last Name,Student ID,Email,Meeting,Date\n" + outputVal;
            }
        }
        function cursorObjectCallback(outputVal) {
            var data = { "data": outputVal };
            var dataVal = JSON.stringify(data);
            $("#MainContent_submitData").val(dataVal);
        }
    </script>
</asp:Content>
