﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="About.aspx.cs" Inherits="IEEECheckin.ASPDocs.MemberPages.About" %>
<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
    <style>
        .col-lg-offset-4 {
            margin-left: 25.0%;
        }
        .col-lg-4 {
            width: 50.0%;
        }
    </style>
</asp:Content>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <h1 class="post-header">About</h1>
    <h4 class="post-header">Meeting Check-in App v1.0</h4>
    <h3>Overview</h3>
    <p>
        The meeting check-in web app is a simple tool for signing in participants at meetings and events. A customizable interface
        allows for modifying and sharing custom styles to meet the desires of the user. Data is stored locally on the users machine
        and can be accessed and cleared from the web app. The users data is also available for easy downloading to several common file formats.
    </p>
    <h3>Instructions</h3>
    <p>
        <h5>Checking In</h5>
        <ul>
            <li>From the menu page create a new meeting (or select a pre-exisiting meeting from the drop-down)</li>
            <li>If using University of Minnesota U-Cards with a compatible card reader, focus on the card entry box and swipe the card.</li>
            <li>If not using U-Cards, have the participant manually enter their information in the "Manual Entry" section.</li>
            <li>Upon swiping the card or clicking the "Check In" button, a confirmation page will appear and shortly return to the check-in page.</li>
            <li>If the confirmation page does not appear or a message box appears, refresh the page and retry entering the user's info.
            If the problem persists, please contact us using the information below.</li>
            <li>When done with a check-in session, simply return to the menu page.</li>
            <li>You can re-enter a check-in session by selecting the meeting from the drop-down box.</li>
        </ul>
        <h5>Viewing/Managing Data</h5>
        <ul>
            <li>To view entered data, select the meeting from the drop-down box and click the "View" button on the menu.</li>
            <li>On the view page you can output the data to a Comma Seperated Value (CSV) file.
            This is a common format that can be opened by many applications include Microsoft Excel and Google Docs.</li>
            <li>To clear data, select the meeting from the drop-down box and click the "Clear" button and confirm.</li>
        </ul>
        <h5>Styling</h5>
        The web app's appearance can be customized to meet the users needs. To customize the app:
        <ul>
            <li>From the menu click on the "Format" button.</li>
            <li>On the format page you will find an example check-in page view. Below this is an extra box for formatting the apperance. This includes
                <ul>
                    <li>The top display image (must be a link to an image url found online)</li>
                    <li>The top header text</li>
                    <li>The use of a U-Card (limited to University of Minnesota only)</li>
                    <li>The background color</li>
                    <li>The button color</li>
                    <li>The text color</li>
                </ul>
            </li>
            <li>You can also reset the theme back to the base IEEE theme by clicking the "Reset Theme" button.</li>
            <li>To set a theme, simply change the values and navigate back to the menu page.</li>
            <li>Themes are saved as cookies. This means your theme will persist in your web browser as long as you have cookies enabled and you don't clear your browser cookies.</li>
            <li>The best logo images to use are either vector graphics (svg) or large images with transparent backgrounds.</li>
            <li>Optionally you can import or export a theme from another browser or computer by copying, sending,
            and pasting the full string found in the "Import/Export" textbox at the bottom of the page
                <ul>
                    <li>To try this out, copy and paste the following text into the "Import/Export" textbox:
                    <br /><br /> {"bodyBackgroundColor":"bb2cd1", "buttonBackgroundColor":"d63a54", "bodyColor":"00ff1e", "imageUrl":"http://goo.gl/bzHpYs", "headerText":"Cat Lovers of America", "useSwipe":"true"}
                    </li>
                </ul>
            </li>
        </ul>
        <h5>Other Notes</h5>
        <ul>
            <li>Data is stored locally on your machine, so only you have access to your data. As long as your computer is secure, the date entered is secure.</li>
            <li>Data entered will only exist on the computer it was entered on and inside the web browser used.</li>
            <li>Data will persist even after the web browser is closed and/or computer is turned off.</li>
            <li>Clearing the web browser's history/data/cookies may/will clear the user data entered and any styling modifications. Make sure to export any important data before doing so.</li>
            <li>This web app has been tested and confirmed to work with Google Chrome 39+, Mozilla Firefox 32+, IE 11+, Opera 26+.</li>
        </ul>
    </p>
    <h3>FAQ</h3>
    <p>
        Ask some questions and we'll post the answers here!
    </p>
    <h3>Disclaimer</h3>
    <p>
        The meeting check-in web app is provided as a free service by the University of Minnesota IEEE Tech Subcommittee.
        This service is provided as is and without any warranties. U of M IEEE Student Branch is not responsible for the use, 
        storage, or security of user data. U of M IEEE does not retain or access any data entered by users.
    </p>
    <h3>Contact</h3>
    <p>
        For any questions, comments, concerns, or suggestions contact us at <a href="mailto:ieee@umn.edu">ieee@umn.edu</a>
    </p>
</asp:Content>
<asp:Content ID="JavaScriptContent" ContentPlaceHolderID="JavaScripts" runat="server">
</asp:Content>
