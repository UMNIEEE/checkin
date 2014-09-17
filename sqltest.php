f<!doctype html>
<html lang="en">
<head>
<meta charset="utf-8">
<meta http-equiv="refresh" content="1; url=<?php echo 'checkin.php?meeting=' . $_GET['meeting']; ?>" />
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
	$meeting = $_GET['meeting'];
	$studentid = $_POST['studentID'];

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

	//Check if user exists
	$safe_to_update = false;
	
	$query_user_exists = "SELECT Studentid FROM Users WHERE Studentid = " . $studentid . ";";
	echo $query_user_exists;
	$result = mysql_query($query_user_exists);
	if (!mysql_num_rows($result)==0)
	{
		$safe_to_update = true;
	}
	else //User doesn't exist
	{
		$query_create_user =   "INSERT INTO Users (Name, Active, Studentid) VALUES ('" . $firstName . " " . $lastName . "', 'No'," . $studentid . ");";
		echo $query_create_user;
		$result_two = mysql_query($query_create_user);
			//if (!mysql_num_rows($result_two)==0)
			//{
				$safe_to_update = true;
			//}
	}
	//Update a Users Attendance for the chosen Meeting
	if ($safe_to_update)
	{
    	$query_update = "UPDATE Users SET `" . $meeting . "` = `" . $meeting . "` + 1 WHERE Studentid = ". $studentid . ";";
    	echo $query_update;
    	mysql_query($query_update) or die (mysql_error());
	}

	

	mysql_close($con);	
	?>
    </div>
</body>
</html>
