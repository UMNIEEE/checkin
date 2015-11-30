<?php
// debug/console library
include 'ChromePhp.php';

header('Content-Type: application/json');

$event_id = -1;

try {
    // Create connection
    $host="localhost";
    $port=4468;
    $socket="";
    $user="ieeegofirst";
    $password="0ed2df89a5!";
    $dbname="ieee";
    
    $con = new mysqli($host, $user, $password, $dbname, $port, $socket);
    
    if ($con->connect_errno) {
        ChromePhp::log("Error" . 'Could not connect: ' . mysqli_connect_error());
        die('Could not connect: ' . mysqli_connect_error());
    }
        
    //Pull raw data
    $meeting = $_GET['meeting'];
    $current_date = date("Y-m-d H:i:s");
    //ChromePhp::log("M-" . $meeting . ", D-" . $current_date);
    
    // Create new event
    if($meeting != null) {
        
        // get meeting name
        $meetingName = trim($meeting);
        
        // get max meeting id
        $query_max_id = "SELECT Max(EventId) as EventId FROM ent_event;";
        if($results = $con->query($query_max_id)) {
            if($results->num_rows <= 0)
                $event_id = 0;
            else
            {
                $results->data_seek(0);
                $row = $results->fetch_assoc();
                $event_id = $row["EventId"] + 1;
                //ChromePhp::log($row["$event_id"]);
            }
                    
            $results->close();
        }
        else {
            ChromePhp::log("Error: " . 'Could not connect: ' . mysqli_connect_error());
            die('Could not connect: ' . mysqli_connect_error());
        }
        
        $query_create_event =   "INSERT INTO ent_event (EventId, Name, Type, StartDate, EndDate, DateCreated, DateLastModified) VALUES (". $event_id .", '" . $meetingName . "', 1, '" . $current_date . "', '" . $current_date .  "', '" . $current_date . "', '" . $current_date . "');";
        //ChromePhp::log($query_create_event);
        
        if(!$con->query($query_create_event)) {
            ChromePhp::log("Error: " . 'Could not connect: ' . mysqli_connect_error());
            die('Could not connect: ' . mysqli_connect_error());
        }
    }
    else if($meeting == null) {
        ChromePhp::log("Error: " . 'No meeting present: ' . $_GET['meeting']);
        die('No meeting present: ' . $_GET['meeting'] );
    }
    else {
    
    }
        
    if($con != null)
        $con->close();
}
catch(Exception $e) {
    ChromePhp::log('Caught exception: ' . $e->getMessage());
    die('Caught exception: ' . $e->getMessage());
}

echo json_encode($event_id);

?>