<?php
// debug/console library
include 'ChromePhp.php';

header('Content-Type: application/json');

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
    $firstName = $_POST['firstname'];
    $lastName = $_POST['lastname'];
    $studentid = $_POST['studentid'];
    $email = $_POST['email'];
    $meeting = $_GET['meeting'];
    $current_date = date("Y-m-d H:i:s");
    //ChromePhp::log("Data: FN-" . $firstName . ", LN-" . $lastName . ", SID-" . $studentid . ", E-" . $email . ", M-" . $meeting . ", D-" . $current_date);
    
    // Check if person exists
    $person_exists = false;
    $query_user_exists = "SELECT PersonId FROM ent_person WHERE UStudentId = '" . $studentid . "';";
    $person_id = -1;
    if($results = $con->query($query_user_exists)) {
        if($results->num_rows > 0) {
            $row = $results->fetch_assoc();
            $person_id = $row["PersonId"];
            //ChromePhp::log($row["PersonId"]);
            $person_exists = true;
        }
        else {
        
        }
        $results->close();
    }
    else {
        ChromePhp::log("Error: " . 'Could not connect: ' . mysqli_connect_error());
        die('Could not connect: ' . mysqli_connect_error());
    }
    
    // Person doesn't exist
    if(!$person_exists) 
    {
        
        // get max person id
        $query_max_id = "SELECT Max(PersonId) as PersonId FROM ent_person;";
        
        if($results = $con->query($query_max_id)) {
            if($results->num_rows <= 0)
                $person_id = 0;
            else {
                $row = $results->fetch_assoc();
                //ChromePhp::log($row["PersonId"]);
                $person_id = $row["PersonId"] + 1;
            }
                    
            $results->close();
        }
        else {
            ChromePhp::log("Error: " . 'Could not connect: ' . mysqli_connect_error());
            die('Could not connect: ' . mysqli_connect_error());
        }
        
        if($studentid != null)
            $query_create_user = "INSERT INTO ent_person (PersonId, FirstName, LastName, UStudentId, EmailOne, DateCreated, DateLastModified) VALUES (". $person_id .", '" . $firstName . "', '" . $lastName . "', " . $studentid . ", '" . $email . "', '" . $current_date . "', '" . $current_date . "');";
        else
            $query_create_user = "INSERT INTO ent_person (PersonId, FirstName, LastName, EmailOne, DateCreated, DateLastModified) VALUES (". $person_id .", '" . $firstName . "', '" . $lastName . "', '" . $email . "', '" . $current_date . "', '" . $current_date . "');";
        //ChromePhp::log($query_create_user);
        
        if(!$con->query($query_create_user)) {
            ChromePhp::log("Error: " . 'Could not connect: ' . mysqli_connect_error());
            die('Could not connect: ' . mysqli_connect_error());
        }
    }
    
    //Update a Users Attendance for the chosen Meeting
    if($person_id >= 0) {
    
        // get max event participant id
        $query_max_id = "SELECT Max(EventParticipantId) as EventParticipantId FROM eventparticipants;";
        $event_part_id = 0;
        if($results = $con->query($query_max_id)) {
            if($results->num_rows <= 0)
                $event_part_id = 0;
            else {
                $results->data_seek(0);
                $row = $results->fetch_assoc();
                $event_part_id = $row["EventParticipantId"] + 1;
            }
                    
            $results->close();
        }
        else {
            ChromePhp::log("Error: " . 'Could not connect: ' . mysqli_connect_error());
            die('Could not connect: ' . mysqli_connect_error());
        }
            
        $query_update = "INSERT INTO eventparticipants (EventParticipantId, EventId, PersonId, DateCreated, DateLastModified) VALUES (" . $event_part_id . ", " . $meeting . ", " . $person_id . ", '" . $current_date . "', '" . $current_date . "');";
        //ChromePhp::log($query_update);
        
        if(!$con->query($query_update)){
            ChromePhp::log("Error: " . 'Could not connect: ' . mysqli_connect_error());
            die('Could not connect: ' . mysqli_connect_error());
        }
    }
    else {
        ChromePhp::log("Error: " . 'Improper personId: ' . $person_id);
        die('Improper personId: ' . $person_id);
    }
    
    if($con != null)
        $con->close();
}
catch(Exception $e) {
    ChromePhp::log('Caught exception: ' . $e->getMessage());
    die('Caught exception: ' . $e->getMessage());
}

echo json_encode(true);

?>