<%@ Page Title="Check-In" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Checkin.aspx.cs" Inherits="IEEECheckin.ASP.Checkin" %>

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <h1 class="post-header">Meeting Check-In for:</h1>
    <h2 id="meetingName" class="post-header"></h2>
            
    <p class="section-label">Swipe Card Entry <i class="fa fa-credit-card"></i></p>
    <div class="boxed-section margin-lg-after">
        <div class="form-group no-margin-after">
            <asp:TextBox class="form-control input-lg margin-sm-after" autofocus="autofocus" type="password" ID="cardTxt" placeholder="Click here, then swipe your card" runat="server" />
        </div>
    </div>
    <p class="section-label">Manual Entry <i class="fa fa-pencil"></i></p>
    <div class="boxed-section margin-lg-after">
        <asp:TextBox class="form-control input-lg margin-sm-after" type="text" ID="firstName" placeholder="First Name" runat="server" />
        <asp:TextBox class="form-control input-lg margin-sm-after" type="text" ID="lastName" placeholder="Last Name" runat="server" />
        <asp:TextBox class="form-control input-lg margin-sm-after" type="text" ID="email" placeholder="Email" runat="server" />
        <asp:HiddenField ID="studentId" runat="server" />
        <asp:Button class="form-control input-lg btn btn-info check-in" Text="Check In" OnClick="Submit" ID="checkinButton" runat="server" />
    </div>
</asp:Content>
<asp:Content runat="server" ID="JavaScriptContent" ContentPlaceHolderID="JavaScripts">
    <script type="text/javascript">
        // document ready
        $(document).ready(function () {
            $("#headerSection").css("display", "none");
            $("#MainContent_firstName").val("");
            $("#MainContent_lastName").val("");
            $("#MainContent_studentId").val("");
            $("#MainContent_email").val("");
            $("#MainContent_cardTxt").val("");

            $("#MainContent_cardTxt").focus();
            var meetingName = GetQueryStringParams("name");
            if(meetingName != null)
                $("#meetingName").html(decodeURI(meetingName));
            $("#MainContent_email").keydown(function (event) {
                if (event.keyCode == 13) {
                    event.preventDefault();
                    $("#MainContent_checkinButton").click();
                }
            });
        });
        // called when U-card parsing has completed
        function parseUCardComplete(uCard) {
            if(uCard === null || uCard === undefined)
                return false;
                    
            $("#MainContent_firstName").val(uCard["firstName"]);
            $("#MainContent_lastName").val(uCard["lastName"]);
            $("#MainContent_studentId").val(uCard["studentId"]);
                    
            // Change this to true to crawl the U of M directory for people
            var crawlUMN = false;
            if (crawlUMN) {
                // create U of M people search URL query.
                if (middleInit !== null) {
                    var urlPath = "http://www.umn.edu/lookup?SET_INSTITUTION=UMNTC&type=name&CN=" +
                        firstName + "+" + middleInit + "+" + lastName + "&campus=a&role=any";
                }
                else {
                    var urlPath = "http://www.umn.edu/lookup?SET_INSTITUTION=UMNTC&type=name&CN=" +
                        firstName + "+" + lastName + "&campus=a&role=any";
                }

                crawl(urlPath); // returns raw HTML of the search results that can be displayed in an overlay
            }
            else {
                $("#MainContent_checkinButton").click();
                return true;
            }
                     
            return false;
        }
        // called when crawl parsing of a single person has completed
        function parseCrawlComplete(uPerson) {
            if(uPerson === null || uPerson === undefined)
                return;
            // add post crawl code here
        }
        // called when crawl parsing of multiple persons returned has completed
        // html - raw HTML of the search results that can be displayed in an overlay
        function searchResultSelected(html) {
                    
        }
    </script>
</asp:Content>
