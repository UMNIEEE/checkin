<?php
// debug/console library
include 'ChromePhp.php';

// parameters
if(empty($_POST['submitData'])) die("No data submitted.");
if(empty($_POST['format'])) die("No format specified.");
    
$data = json_decode($_POST['submitData'], true);
$format = $_POST['format'];
    
switch($format) {
    case "csv":
        $fileName = "meeting-sign-ins-" . date("Y-m-d-H-i-s") . ".csv";
        // output headers so that the file is downloaded rather than displayed
        header('Content-Type: text/csv; charset=utf-8');
        header('Content-Disposition: attachment; filename=' . $fileName);
        formatCsv($data);
        break;
    case "json":
        $fileName = "meeting-sign-ins-" . date("Y-m-d-H-i-s") . ".js";
        // output headers so that the file is downloaded rather than displayed
        header('Content-Type: text/plain; charset=utf-8');
        header('Content-Disposition: attachment; filename=' . $fileName);
        formatJson($data);
        break;
    case "xml":
        die("XML not currently supported");
        break;
    case "sql":
        die("SQL not currently supported");
        break;
}  

function formatCsv($data) {
    try {
        // create a file pointer connected to the output stream
        $output = fopen('php://output', 'w');
    
        // output the column headings
        //fputcsv($output, array('First Name', 'Last Name', 'Student Id', 'Email', 'Meeting', 'Date'));

        $firstLineKeys = false;
        foreach($data["data"] as $line) {
            try {
                if (empty($firstLineKeys))
	            {
		            $firstLineKeys = array_keys($line);
		            fputcsv($output, $firstLineKeys);
		            $firstLineKeys = array_flip($firstLineKeys);
	            }
	            // Using array_merge is important to maintain the order of keys according to the first element
	            fputcsv($output, array_merge($firstLineKeys, $line));
            }
            catch(Exception $e) {
                ChromePhp::log('Caught exception: ' . $e->getMessage());
                die('Caught exception: ' . $e->getMessage());
            }
        }
    }
    catch(Exception $e) {
        ChromePhp::log('Caught exception: ' . $e->getMessage());
        die('Caught exception: ' . $e->getMessage());
    }
}

function formatJson($data) {
    $output = fopen('php://output', 'w');
    
    fputs($output, json_encode($data["data"]));
}

?>