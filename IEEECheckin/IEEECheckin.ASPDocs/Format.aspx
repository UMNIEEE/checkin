<%@ Page Title="Format" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Format.aspx.cs" Inherits="IEEECheckin.ASPDocs.Format" %>
<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
    <script src="Scripts/Local/colorpicker.js"></script>
    <script src="Scripts/Local/colorpicker-eye.js"></script>
    <script src="Scripts/Local/colorpicker-utils.js"></script>
    <script src="Scripts/Local/predef-formats.js"></script>
    <script src="Scripts/Local/format.js"></script>
    <link rel="stylesheet" href="Content/colorpicker.css" />
    <link rel="stylesheet" href="Content/format.css" />
</asp:Content>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <h1 class="post-header">Format</h1>
    <h1 class="post-header">Meeting Check-In for:</h1>
    <h2 id="meetingName" class="post-header">Example Meeting</h2>
    <div id="swipe-section">
        <p class="section-label">ID Card Swipe Entry <i class="fa fa-credit-card"></i></p>
        <div id="form-ucard" class="boxed-section margin-lg-after" role="form">
            <div class="form-group no-margin-after">
                <input class="form-control input-lg margin-sm-after" autofocus="autofocus" type="password" name="cardtxt" id="cardtxt" placeholder="Click here, then swipe your card">
            </div>
        </div>
    </div>
    <p class="section-label">Manual Entry <i class="fa fa-pencil"></i></p>
    <div class="boxed-section margin-lg-after">
        <input class="form-control input-lg margin-sm-after" type="text" name="firstname" id="firstname" placeholder="First Name">
        <input class="form-control input-lg margin-sm-after" type="text" name="lastname" id="lastname" placeholder="Last Name">
        <input class="form-control input-lg margin-sm-after" type="text" name="email" id="email" placeholder="Email">
        <button class="form-control input-lg btn btn-info check-in" onclick="return false;" type="submit" id="checkinbutton" name="checkinbutton"><i class="fa fa-check"></i> Check In</button>
    </div>
    <p class="section-header">Edit Content <i class="fa fa-cogs"></i></p>
    <div class="boxed-section margin-lg-after">
        <p>Changes occur when a control loses focus (click somewhere else on page for changes to occur).</p>
        <input class="form-control input-lg margin-sm-after" type="text" name="imageUrl" id="imageUrl" placeholder="Image Url" onblur="updateImage()">
        <input class="form-control input-lg margin-sm-after" type="text" name="topText" id="topText" placeholder="Top Text" onblur="updateTopText()">
        <table><tbody>
            <tr><td><input class="margin-sm-after" type="checkbox" name="swipeCheck" id="swipeCheck" onblur="updateSwipe()"></td><td>Use ID Card Swipe</td></tr>
            <tr><td><input class="margin-sm-after" type="checkbox" name="themeShade" id="themeShade" onblur="updateThemeShade()"></td><td id="themeShadeText">Using Light Theme</td></tr>
        </tbody></table>
        <table>
            <tbody>
                <tr>
                    <td>Background Color</td>
                    <td>Button Color</td>
                    <td>Text Color</td>
                </tr>
                <tr>
                    <td><div id="colorSelector"></div></td>
                    <td><div id="colorSelector2"></div></td>
                    <td><div id="colorSelector3"></div></td>
                </tr>
            </tbody>
        </table>
        <br />
        <button class="form-control input-lg btn btn-info check-in" onclick="return resetTheme();" type="submit" id="resetbutton" name="resetbutton">Reset Theme</button>
        <br />
        <br />
        <p class="section-label">Select Existing Theme<i class="fa fa-pencil"></i></p>
        <asp:DropDownList class="selectpicker" ID="themeDropdown" AutoPostBack="false" AppendDataBoundItems="true" runat="server" onblur="updatePreDefTheme()">
            <asp:ListItem Text="Custom Theme" Value="" Selected="true"></asp:ListItem>
        </asp:DropDownList>
    </div>
    <p class="section-header">Select Card Format <i class="fa fa-credit-card"></i></p>
    <div class="boxed-section margin-lg-after">
        <p>Select your school/card format. See the about page for more information.</p>
        <asp:DropDownList class="selectpicker" ID="regexDropdown" AutoPostBack="false" AppendDataBoundItems="true" runat="server" onblur="updateRegex()">
            <asp:ListItem Text="No Card Format" Value="" Selected="true"></asp:ListItem>
        </asp:DropDownList>
    </div>
    <p class="section-header">Import/Export Format <i class="fa fa-copy"></i></p>
    <div class="boxed-section margin-lg-after">
        <p>Copy the output and paste it into another browser/computer to replicate theme.</p>
        <input class="form-control input-lg margin-sm-after" type="multiline" name="export" id="export" onblur="importTheme()">
    </div>
</asp:Content>
