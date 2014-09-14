<!doctype html>
<html lang="en">
<head>
<meta charset="utf-8">
    <title>IEEE Meeting Check-In</title>
    <link rel="stylesheet" href="//netdna.bootstrapcdn.com/bootstrap/3.1.1/css/bootstrap.min.css">
    <link href='http://fonts.googleapis.com/css?family=Lato:300,400,700|Oswald:300' rel='stylesheet' type='text/css'>
    <link rel="stylesheet" href="//netdna.bootstrapcdn.com/font-awesome/4.0.3/css/font-awesome.min.css">
    <link rel="stylesheet" href="css/checkin.css">
    <!--[if lt IE 9]>
    <script src="http://html5shiv.googlecode.com/svn/trunk/html5.js"></script>
    <![endif]-->
</head>
<body>
    <div class="container">
        <div class="col-lg-4 col-lg-offset-4 col-md-6 col-md-offset-3 col-sm-8 col-sm-offset-2 col-xs-12">
            <h1 class="pre-header">University of Minnesota</h1>
            <img class="logo" src="images/ieee.svg">
            <h1 class="post-header">Meeting Check-In for:</h1>
            <form class="boxed-section margin-lg-after" action="check.cgi" method="POST" enctype="multipart/form-data" role="form">
                <div class="form-group no-margin-after">
                    <?php
                    $meeting = test;
                    //Uncomment line below and delete the above line if dropdown page works
                    // $meeting = htmlspecialchars($_POST['meeting']); //This line takes input from POST
                    ?>
                    <input class="form-control input-lg" autofocus="autofocus" type="text" name="meeting" id="meeting" value="<?php echo $meeting; ?>" readonly>
                </div>
            </form>
            <p class="section-label">Swipe Card Entry <i class="fa fa-credit-card"></i></p>
            <form class="boxed-section margin-lg-after" action="check.cgi" method="POST" enctype="multipart/form-data" role="form">
                <div class="form-group no-margin-after">
                    <input class="form-control input-lg" autofocus="autofocus" type="password" name="cardtxt" id="cardtxt" placeholder="Click here, then swipe your card">
                    <input class="form-control input-lg margin-sm-after" type="hidden" name"meeting" value="<?php echo $meeting; ?>">
                </div>
            </form>
            <p class="section-label">Manual Entry <i class="fa fa-pencil"></i></p>
            <form class="boxed-section margin-lg-after" action="sqltest.php" method="POST" enctype="multipart/form-data" role="form">
                <input class="form-control input-lg margin-sm-after" type="text" name="firstname" id="firstname" placeholder="First Name">
                <input class="form-control input-lg margin-sm-after" type="text" name="lastname" id="lastname" placeholder="Last Name">
                <input class="form-control input-lg margin-sm-after" type="hidden" name"meeting" value="<?php echo $meeting; ?>">
                <button class="form-control input-lg btn btn-info check-in" type="submit" value="sqltest.php"><i class="fa fa-check"></i> Check In</button>
            </form>
            <form class="boxed-section margin-lg-after" action="email.cgi" method="POST" enctype="multipart/form-data" role="form">
                <input class="form-control input-lg margin-sm-after" type="email" name="recipient" id="recipient" placeholder="Recipient">
                <input class="form-control input-lg margin-sm-after" type="text" name="subject" id="subject" placeholder="Subject">
                <button class="form-control input-lg btn btn-info check-in" type="submit"><i class="fa fa-envelope"></i> Send Email</button>
            </form>
            <form class="boxed-section" action="sqltest.php" method="POST">
                <button class="form-control input-lg btn btn-info check-in" type="submit" value="sqltest.php"><i class="fa fa-eraser"></i> Create A New Check In</button>
            </form>            
            <script src="//netdna.bootstrapcdn.com/bootstrap/3.1.1/js/bootstrap.min.js"></script>
            <p class="footer">Powered by the IEEE Tech Subcommittee</p>
        </div>
    </div>
</body>
</html>
