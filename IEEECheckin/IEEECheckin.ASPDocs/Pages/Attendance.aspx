<%@ Page Title="Attendance" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Attendance.aspx.cs" Inherits="IEEECheckin.ASPDocs.Attendance" %>
<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
    <link rel="stylesheet" href="Content/attendance.css" />
    <script src="Scripts/Local/attendance.js"></script>
</asp:Content>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <h1 class="post-header">Meeting Attendance</h1>

    <p class="section-header">Filter Meeting Attendance <i class="fa fa-file-text"></i></p> 
    <div class="boxed-section margin-lg-after">
        <select class="selectpicker" id="ouputDropdown" onchange="selectionChanged()"></select>
    </div>

    <p class="section-header">Export <i class="fa fa-download"></i></p> 
    <div class="boxed-section margin-lg-after">
        <asp:HiddenField ID="SubmitData" runat="server" />
        <asp:HiddenField ID="MeetingName" runat="server" />
        <asp:HiddenField ID="MeetingDate" runat="server" />
        <asp:Button CssClass="form-control input-lg btn btn-info check-in" OnClick="SubmitCsv" Text="Get CSV" id="CsvButton" runat="server" />
        <br /><br />
        <asp:Button CssClass="form-control input-lg btn btn-info check-in" OnClick="SubmitJson" Text="Get JSON" id="JsonButton" runat="server" />
        <br /><br />
        <asp:Button CssClass="form-control input-lg btn btn-info check-in" PostBackUrl="~/MemberPages/GoogleDocLoading.aspx" Text="Add to Google" id="GoogleButton" runat="server" />
    </div>

    <p class="section-header">Clear Attendance <i class="fa fa-eraser"></i></p>
    <div class="boxed-section margin-lg-after">
        <asp:Button CssClass="form-control input-lg btn btn-info check-in" OnClientClick="return clearSubmit();" ID="ClearButton" Text="Clear" runat="server" />
    </div>

    <p class="section-header">Lottery <i class="fa fa-random"></i></p>
    <div class="boxed-section margin-lg-after">
        <asp:Button CssClass="form-control input-lg btn btn-info check-in" OnClientClick="return lotteryRoll();" ID="RandomButton" Text="Roll" runat="server" />
        <p class="text-center">And the winner is...</p>
        <div class="text-center" id="WinnerBox"></div>
    </div>

    </div><!--Not an orphaned /div in VS, just starts on the master page. Same for closing tag of div below.-->
    <div class="col-custom-offset col-custom col-lg-4 col-lg-offset-4 col-md-6 col-md-offset-3 col-sm-8 col-sm-offset-2 col-xs-12">
    <div id="outputDiv">
        <table id="output">
            <tbody>
                <tr id="headerRow">
                    <th>Index</th>
                    <th>First Name</th>
                    <th>Last Name</th>
                    <th>Student ID</th>
                    <th>Email</th>
                    <th>Meeting</th>
                    <th>Date</th>
                </tr>
            </tbody>
        </table>
</asp:Content>