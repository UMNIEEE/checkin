f<!doctype html>
<html lang="en">
<head>
<meta charset="utf-8">
<!--<meta http-equiv="refresh" content="1; url=checkin.php" />-->
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
            <h1 class="post-header">Meeting Check-In</h1>
        </div>
        
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
        
        //Pull raw data
        $firstName = $_POST['firstname'];
        $lastName = $_POST['lastname'];
        $studentid = $_POST['studentid'];
        $meeting = $_GET['meeting'];
        $current_date = date("Y-m-d H:i:s");
        
        // Create new event
        if($meeting != null && strpos($meeting, 'new') !== FALSE)
        {
            $meeting = trim(str_replace('new', ' ', $meeting, 1));
            echo $meeting;
            $query_max_id = "SELECT Max(EventId) FROM ent_event;";
            $event_part_id = 0;
            if($results = $con->query($query_max_id))
            {
                if($results->num_rows == 0)
                    $event_part_id = 0;
                else
                {
                    $results->data_seek(0);
                    $row = $results->fetch_assoc();
                    $meeting = $row["EventId"] + 1;
                }
                    
                $results->close();
            }
            else if ($meeting == null)
                die('Could not connect: ' . mysqli_connect_error());
            
            $query_create_event =   "INSERT INTO ent_event (EventId, Name, Type, StartDate, EndDate, DateCreated, DateLastModified) VALUES (". $event_id .", '" . $meeting . "', 1, " . $current_date . ", " . $current_date .  ", " . $current_date . ", " . $current_date . ");";
            echo $query_create_event;
            $con->query($query_create_event) or die('Could not connect: ' . mysqli_connect_error());
            $meeting = $event_id;
        }
        else if($meeting == null)
            die('No meeting present: ' . $meeting . ", " . $_GET['meetingName'] );

        //Check if user exists
        $safe_to_update = false;
        $query_user_exists = "SELECT UStudentId FROM ent_person WHERE UStudentId = " . $studentid . ";";
        echo $query_user_exists;
        $person_id = -1;
        if($results = $con->query($query_user_exists) && $results->num_rows == 0)
        {
            $safe_to_update = true;
            $results->close();
        }
        else //User doesn't exist
        {
            $query_max_id = "SELECT Max(PersonId) FROM ent_person;";
            $person_id = 0;
            if($results = $con->query($query_max_id))
            {
                if($results->num_rows == 0)
                    $person_id = 0;
                else
                {
                    $results->data_seek(0);
                    $row = $results->fetch_assoc();
                    $person_id = $row["PersonId"] + 1;
                }
                    
                $results->close();
            }
            
            $query_create_user =   "INSERT INTO ent_person (PersonId, FirstName, LastName, UStudentId, DateCreated, DateLastModified) VALUES (". $person_id .", '" . $firstName . "', '" . $lastName . "', " . $studentid . ", " . $current_date . ", " . $current_date . ");";
            echo $query_create_user;
            $con->query($query_create_user) or die('Could not connect: ' . mysqli_connect_error());
        }
        
        //Update a Users Attendance for the chosen Meeting
        if($person_id >= 0)
        {
            $query_max_id = "SELECT Max(EventParticipantId) FROM eventparticipants;";
            $event_part_id = 0;
            if($results = $con->query($query_max_id))
            {
                if($results->num_rows == 0)
                    $event_part_id = 0;
                else
                {
                    $results->data_seek(0);
                    $row = $results->fetch_assoc();
                    $event_part_id = $row["EventParticipantId"] + 1;
                }
                    
                $results->close();
            }
            else
                die('Could not connect: ' . mysqli_connect_error());
            
            $query_update = "INSERT INTO eventparticipants (EventParticipantId, EventId, PersonId, DateCreated, DateLastModified) VALUES (" . $event_part_id . ", " . $meeting . ", " . $person_id . ", " . $current_date . ", " . $current_date . ";";
            echo $query_update;
            $con->query($query_update) or die('Could not connect: ' . mysqli_connect_error());
            $con->close(); 
        }
        else
            die('Improper personId.');
    ?>
    </div>
</body>
</html>
