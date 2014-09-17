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
    <?php
        // Create connection
        $con = mysql_connect(localhost, "root", "4Eatiaeaor");
        if (!$con)
        {
           die('Could not connect: ' . mysql_error());
        }
        //Select database
        $db_selected = mysql_select_db('ieee', $con);
        if (!$db_selected) 
        {
           die ('Can\'t use ieee: ' . mysql_error());
        }
        //Amith's addition, Getting Column Names
        $query_uno = "select COLUMN_NAME
                            from information_schema.columns 
                            where table_schema = 'ieee' 
                            and table_name = 'users';";
        $result_uno = mysql_query($query_uno) or die(mysql_error());
        $result_array = array();
        while($row = mysql_fetch_assoc($result_uno))
        {
            $result_array[] = $row['COLUMN_NAME'];
        }
        $full_name = $firstName. ' ' .$lastName; 
        mysql_close($con);  
    ?>

     <!---
                <?php
                //foreach ($column_array as $current_column) {      
                  // <li role="presentation"><a role="menuitem" tabindex="-1" href="#"><?php echo $current_column;?></a></li>  
                }
                ?>
                -->

    <div class="container">
        <div class="col-lg-4 col-lg-offset-4 col-md-6 col-md-offset-3 col-sm-8 col-sm-offset-2 col-xs-12">
            <h1 class="pre-header">University of Minnesota</h1>
            <img class="logo" src="images/ieee.svg">
            <h1 class="post-header">Meeting Check-In</h1>
            <p class="section-header">Meeting Options<i class="fa fa-list"></i></p>  
            <!-- Insert action to redirect to checkin.php -->
            <form class="boxed-section margin-lg-after" action="checkin.php" method="POST" id="meetingForm" enctype="multipart/form-data" role="form"> 
                <select class="form-control" name="meeting" id="meetingDropdown">
                <?php
                    foreach ($result_array as $entry){
                    echo  '<option>' . htmlspecialchars($entry) . '</option>';
                    }
                ?>
                </select>
                <p class="section-label">Create New Meeting<i class="fa fa-pencil"></i></p>
            
                <input class="form-control input-lg margin-sm-after" type="text" name="newmeeting" id="newmeeting" placeholder="Meeting Name">
                
        
                <button class="form-control input-lg btn btn-info check-in" id="meetingButton" onclick="return meetingSubmit();" type="submit" value="checkin.php"><i class="fa fa-check"></i>New Meeting</button>

            </form>
            <!-- Button to Post selection -->
            <script src="//netdna.bootstrapcdn.com/bootstrap/3.1.1/js/bootstrap.min.js"></script>
            <script src="//ajax.googleapis.com/ajax/libs/jquery/2.1.1/jquery.min.js"></script>
            <script src="checkin.js"></script>   
            <p class="footer">Powered by the IEEE Tech Subcommittee</p>
        </div>
    </div>
</body>
</html>
