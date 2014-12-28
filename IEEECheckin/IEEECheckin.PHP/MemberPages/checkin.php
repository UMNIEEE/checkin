<!doctype html>
<html lang="en">
<head>
<meta charset="utf-8">
    <title>IEEE Meeting Check-In</title>
    <link rel="stylesheet" href="//netdna.bootstrapcdn.com/bootstrap/3.1.1/css/bootstrap.min.css">
    <link href='http://fonts.googleapis.com/css?family=Lato:300,400,700|Oswald:300' rel='stylesheet' type='text/css'>
    <link rel="stylesheet" href="//netdna.bootstrapcdn.com/font-awesome/4.0.3/css/font-awesome.min.css">
    <link rel="stylesheet" href="../Content/Site.css">
    <!--[if lt IE 9]>
    <script src="http://html5shiv.googlecode.com/svn/trunk/html5.js"></script>
    <![endif]-->
</head>
<body>

    <div class="container">
        <div class="col-lg-4 col-lg-offset-4 col-md-6 col-md-offset-3 col-sm-8 col-sm-offset-2 col-xs-12">
            <h1 class="pre-header">University of Minnesota</h1>
            <img class="logo" src="../Images/ieee.svg">
            <h1 class="post-header">Meeting Check-In for:</h1>
            <h2 id="meetingName" class="post-header"></h2>
            
            <p class="section-label">Swipe Card Entry <i class="fa fa-credit-card"></i></p>
            <form class="boxed-section margin-lg-after" onsubmit="return false" role="form">
                <div class="form-group no-margin-after">
                    <input class="form-control input-lg margin-sm-after" autofocus="autofocus" type="password" name="cardtxt" id="cardtxt" placeholder="Click here, then swipe your card">
                </div>
            </form>
            <p class="section-label">Manual Entry <i class="fa fa-pencil"></i></p>
            <form class="boxed-section margin-lg-after" onsubmit="return formSubmit()" role="form" id="manualForm">
                <input class="form-control input-lg margin-sm-after" type="text" name="firstname" id="firstname" placeholder="First Name">
                <input class="form-control input-lg margin-sm-after" type="text" name="lastname" id="lastname" placeholder="Last Name">
                <input class="form-control input-lg margin-sm-after" type="text" name="email" id="email" placeholder="Email">
                <input class="form-control input-lg margin-sm-after" type="hidden" name="studentid" id="studentid" placeholder="Student ID">
                <button class="form-control input-lg btn btn-info check-in" type="submit" id="checkinbutton" name="checkinbutton"><i class="fa fa-check"></i> Check In</button>
            </form>

            <script src="//ajax.googleapis.com/ajax/libs/jquery/2.1.1/jquery.min.js"></script>
            <script src="//netdna.bootstrapcdn.com/bootstrap/3.1.1/js/bootstrap.min.js"></script>
            <script src="../Scripts/check-in.js"></script>
            <script src="../Scripts/umn-crawl.js"></script>
            <script type="text/javascript">
                // document ready
                $(document).ready(function () {
                    $("#cardtxt").focus();
                    var meetingName = GetQueryStringParams("name");
                    if(meetingName != null)
                    $("#meetingName").html(decodeURI(meetingName));
                });
                // called when U-card parsing has completed
                function parseUCardComplete(uCard) {
                    if(uCard === null || uCard === undefined)
                        return false;
                    
                    $("#firstname").val(uCard["firstName"]);
                    $("#lastname").val(uCard["lastName"]);
                    $("#studentid").val(uCard["studentId"]);
                    
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
                        $("#checkinbutton").click();
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
                function formSubmit() {
                    var urlVal = "submit.php?meeting=" + GetQueryStringParams("meeting");
                    $.ajax({type:'POST', 
                            url: urlVal, 
                            data:$('#manualForm').serialize(),
                            dataType: 'html', 
                            success: function(response) {
                                if(response === "true") {
                                    $("#firstname").val("");
                                    $("#lastname").val("");
                                    $("#studentid").val("");
                                    $("#email").val("");
                                    window.location.href = "confirm.html";
                                    }
                                else
                                    alert("Data not entered.");
                                }
                            });
                    return false;
                }
            </script>
            <p class="footer">Powered by the IEEE Tech Subcommittee</p>
        </div>
    </div>
</body>
</html>
