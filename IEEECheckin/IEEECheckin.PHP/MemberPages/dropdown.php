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
    <?php
        // Create connection
        $host="localhost";
        $port=4468;
        $socket="";
        $user="ieeegofirst";
        $password="0ed2df89a5!";
        $dbname="ieee";

        $con = new mysqli($host, $user, $password, $dbname, $port, $socket);

        if ($con->connect_errno)
        {
           die('Could not connect: ' . mysqli_connect_error());
        }
        
        $query_uno = "select * from ent_event order by startdate desc;"; // TODO - limit event dates to past year
        $result_uno = $con->query($query_uno);
        $result_array = array();
        $result_uno->data_seek(0);
        while($row = $result_uno->fetch_assoc())
        {
            $date_array = explode(" ", $row['StartDate']);
            if(count($date_array))
                $result_array[] = $row['Name'] . " " . $date_array[0];
            else
                $result_array[] = $row['Name'];
        }
        $full_name = $firstName. ' ' .$lastName; 
        $con->close();  
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
            <img class="logo" src="../Images/ieee.svg">
            <h1 class="post-header">Meeting Check-In</h1>
            <p class="section-header">Meeting Options<i class="fa fa-list"></i></p>  
            <!-- Insert action to redirect to checkin.php -->
            <form class="boxed-section margin-lg-after" action="checkin.php" method="POST" enctype="multipart/form-data" role="form"> 
                <select class="selectpicker" id="meeting" name="meeting">
                <?php
                    foreach ($result_array as $entry){
                    echo  '<option>' . htmlspecialchars($entry) . '</option>';
                    }
                ?> 
                </select>
                <button class="form-control input-lg btn btn-info check-in" type="submit" value="checkin.php"><i class="fa fa-check"></i>New Meeting</button>
            </form>
            <!-- Button to Post selection -->

            <script src="//netdna.bootstrapcdn.com/bootstrap/3.1.1/js/bootstrap.min.js"></script>
            <p class="footer">Powered by the IEEE Tech Subcommittee</p>
        </div>
    </div>
</body>
</html>
