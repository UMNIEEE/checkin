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
            <p class="section-label"><?php echo $_POST['meeting']; ?></p>
            <br />
            <form class="boxed-section margin-lg-after" action="sqltest.php" method="POST" enctype="multipart/form-data" role="form">
                <div class="form-group no-margin-after">
                    <p class="section-label">Swipe Card Entry <i class="fa fa-credit-card"></i></p>
                    <input class="form-control input-lg" autofocus="autofocus" type="password" name="cardtxt" id="cardtxt" placeholder="Click here, then swipe your card">
                    <input class="form-control input-lg margin-sm-after" type="hidden" name"meeting" value="<?php echo $meeting; ?>">
                    <br />
                    <p class="section-label">Manual Entry <i class="fa fa-pencil"></i></p>
                    <input class="form-control input-lg margin-sm-after" type="text" name="firstname" id="firstname" placeholder="First Name">
                    <input class="form-control input-lg margin-sm-after" type="text" name="lastname" id="lastname" placeholder="Last Name">
                    <input class="form-control input-lg margin-sm-after" type="hidden" name="meeting" value="<?php echo $meeting; ?>">
                    <input class="form-control input-lg margin-sm-after" type="hidden" name="studentid" id="studentid">
                    <button class="form-control input-lg btn btn-info check-in" type="submit" value="sqltest.php" id="checkinbutton" name="checkinbutton"><i class="fa fa-check"></i> Check In</button>
                </div>
            </form>
            <script src="//ajax.googleapis.com/ajax/libs/jquery/2.1.1/jquery.min.js"></script>
            <script src="//netdna.bootstrapcdn.com/bootstrap/3.1.1/js/bootstrap.min.js"></script>
            <script src="../Scripts/check-in.js"></script>
            <script src="../Scripts/umn-crawl.js"></script>
            <script type="text/javascript">
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
            </script>
            <p class="footer">Powered by the IEEE Tech Subcommittee</p>
        </div>
    </div>
</body>
</html>
