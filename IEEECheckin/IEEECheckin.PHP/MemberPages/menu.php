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
// debug/console library
include 'ChromePhp.php';

try{
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
    
    $current_date = date("Y-m-d");
    
    // subtract a year from current date
    $past_date = new DateTime(date());
    $past_date->sub(new DateInterval("P1Y"));
    $past_date_str = $past_date->format("Y-m-d");
    
    $result_array = array(); // event names (display name)
    $value_array = array(); // event ids
    
    // get events from the past year
    $query_events = "SELECT * FROM Ent_Event WHERE StartDate >= '" . $past_date_str . "' ORDER BY StartDate DESC;";
    $results = $con->query($query_events);
    $results->data_seek(0);
    while($row = $results->fetch_assoc()) {
        $date_array = explode(" ", $row['StartDate']);
        if(count($date_array)) {
            $result_array[] = $row['Name'] . " " . $date_array[0]; // concat start date to end of name
            $value_array[] = $row['EventId'];
        }
        else {
            $result_array[] = $row['Name'];
            $value_array[] = $row['EventId'];
        }
    }
    if($con != null)
        $con->close();
}
catch(Exception $e) {
    ChromePhp::log('Caught exception: ' . $e->getMessage());
    die('Caught exception: ' . $e->getMessage());
}
?>

    <div class="container">
        <div class="col-lg-4 col-lg-offset-4 col-md-6 col-md-offset-3 col-sm-8 col-sm-offset-2 col-xs-12">
            <h1 class="pre-header">University of Minnesota</h1>
            <img class="logo" src="../Images/ieee.svg">
            <h1 class="post-header">Meeting Check-In</h1>
            <p class="section-header">Meeting Options<i class="fa fa-list"></i></p>  
            <!-- Insert action to redirect to checkin.php -->
            <form class="boxed-section margin-lg-after" id="meetingForm" method="post" role="form">
                <p class="section-label">Select Existing Meeting<i class="fa fa-calendar-o"></i></p>
                <select class="selectpicker" id="meetingDropdown" name="meeting">
                <?php
                    $index = 0;
                    foreach ($result_array as $entry)
                    {
                        echo  '<option value="' . $value_array[$index] . '">' . htmlspecialchars($entry) . '</option>';
                        $index = $index + 1;
                    }
                ?> 
                </select>
                <p class="section-label">Create New Meeting<i class="fa fa-pencil"></i></p>
                <input class="form-control input-lg margin-sm-after" type="text" id="newmeeting" placeholder="Meeting Name">
                <button class="form-control input-lg btn btn-info check-in" id="meetingButton" onclick="return meetingSubmit();" type="submit"><i class="fa fa-check"></i>Start Meeting</button>
            </form>

            <script src="//ajax.googleapis.com/ajax/libs/jquery/2.1.1/jquery.min.js"></script>
            <script src="//netdna.bootstrapcdn.com/bootstrap/3.1.1/js/bootstrap.min.js"></script>
            <script src="../Scripts/check-in.js"></script>
            <script src="../Scripts/umn-crawl.js"></script>
            <script>
            // document ready
            $(document).ready(function () {
                // prevent form submit due to an enter key press.
                $("#newmeeting").keydown(function (event) {
                    if (event.keyCode == 13) {
                        event.preventDefault();
                        meetingSubmit();
                        return false;
                    }
                });
            });
            function meetingSubmit() {
                if ($("#newmeeting").val() === null || $("#newmeeting").val() === undefined || $("#newmeeting").val().trim() === "") {
                    var meetingId = encodeURI($("#meetingDropdown option:selected").val());
                    var meetingName = $("#meetingDropdown option:selected").html();
                    var regEx = /((\d{4}-\d{2}-\d{2})$)/;
                    meetingName = meetingName.split(regEx)[0];
                    meetingName = encodeURI(meetingName.trim());
                    $("#newmeeting").val("");
                    window.location.href = "checkin.php?meeting=" + meetingId + "&name=" + meetingName;
                }
                else {
                    var urlVal = "event.php?meeting=" + encodeURI($("#newmeeting").val());
                    $.ajax({type:'GET', 
                            url: urlVal, 
                            dataType: 'json', 
                            success: function(response) {
                                if(response != null && response > 0) {
                                    var meetingName = $("#newmeeting").val();
                                    $("#newmeeting").val("");
                                    window.location.href = "checkin.php?meeting=" + response + "&name=" + meetingName;
                                }
                                else {
                                    alert("Could not create event.");
                                }
                            }
                            });
                }
                return false;
            }
            </script>
            <p class="footer">Powered by the IEEE Tech Subcommittee</p>
        </div>
    </div>
</body>
</html>
