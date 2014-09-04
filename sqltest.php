f<!doctype html>
<html lang="en">
<head>
<meta charset="utf-8">
<!--<meta http-equiv="refresh" content="1; url=checkin.php" />-->
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
            <h1 class="post-header">Meeting Check-In</h1>
        </div>
	<?php
	
	//Pull raw data
	$firstName = $_POST['firstname'];
	$lastName = $_POST['lastname'];
	$meeting = $_POST['meeting'];
	echo $firstName;
	echo $lastName;
	echo $meeting;

	// Create connection
	$con = mysql_connect(localhost, "root", "4Eatiaeaor");
	if (!$con)
	{
	   die('Could not connect: ' . mysql_error());
	}
	else
	{
		echo 'Connected Successfully';
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
	foreach ($result_array as $entry){
		echo $entry;
	}

	$full_name = $firstName. ' ' .$lastName;
	
	mysql_close($con);	
	?>
    </div>
</body>
</html>
