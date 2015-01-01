// create an instance of a db object for us to store the IDB data in
var db;
var dbName = "attendees";

// create a blank instance of the object that is used to transfer data into the IDB. This is mainly for reference
var newItem = [
      { index: "", studentid: "", firstname: "", lastname: "", email: "", day: 0, month: 0, year: 0, meeting: "" }
];

$(document).ready(function () {

    // In the following line, you should include the prefixes of implementations you want to test.
    window.indexedDB = window.indexedDB || window.mozIndexedDB || window.webkitIndexedDB || window.msIndexedDB;
    // DON'T use "var indexedDB = ..." if you're not in a function.
    // Moreover, you may need references to some window.IDB* objects:
    window.IDBTransaction = window.IDBTransaction || window.webkitIDBTransaction || window.msIDBTransaction;
    window.IDBKeyRange = window.IDBKeyRange || window.webkitIDBKeyRange || window.msIDBKeyRange;
    // (Mozilla has never prefixed these objects, so we don't need window.mozIDB*)

    // Let us open our database
    var DBOpenRequest = window.indexedDB.open(dbName, 4);

    // Gecko-only IndexedDB temp storage option:
    // var request = window.indexedDB.open(dbName, {version: 4, storage: "temporary"});

    // these two event handlers act on the database being opened successfully, or not
    DBOpenRequest.onerror = function (event) {
        alert("Error loading database.");
    };

    DBOpenRequest.onsuccess = function (event) {
        //alert("Database initialized.");

        // store the result of opening the database in the db variable. This is used a lot below
        db = DBOpenRequest.result;
        try {
            if(dbOpenCallback != undefined)
                dbOpenCallback();
        }
        catch (err) {

        }
    };

    // This event handles the event whereby a new version of the database needs to be created
    // Either one has not been created before, or a new version number has been submitted via the
    // window.indexedDB.open line above
    //it is only implemented in recent browsers
    DBOpenRequest.onupgradeneeded = function (event) {
        var db = event.target.result;

        db.onerror = function (event) {
            alert("Error loading database.");
        };

        // Create an objectStore for this database
        var objectStore = db.createObjectStore(dbName, { keyPath: "index" });

        // define what data items the objectStore will contain
        objectStore.createIndex("firstname", "firstname", { unique: false });
        objectStore.createIndex("lastname", "lastname", { unique: false });
        objectStore.createIndex("email", "email", { unique: false });
        objectStore.createIndex("day", "day", { unique: false });
        objectStore.createIndex("month", "month", { unique: false });
        objectStore.createIndex("year", "year", { unique: false });
        objectStore.createIndex("meeting", "meeting", { unique: false });

        //alert("Object store created.");
    };

});

function addData() {
    var firstnameVal = $("#firstname").val().trim();
    var lastnameVal = $("#lastname").val().trim();
    var studentidVal = $("#studentid").val().trim();
    var emailVal = $("#email").val().trim();
    var meetingVal = decodeURIComponent(GetQueryStringParams("meeting")).trim();
    var dateVal = decodeURIComponent(GetQueryStringParams("date")).trim();
    var indexVal = lastnameVal + "-" + firstnameVal + "-" + studentidVal + "-" + Date.now();

    // Stop the form submitting if any values are left empty. This is just for browsers that don't support the HTML5 form
    // required attributes
    if (firstnameVal == "" || lastnameVal == "" || meetingVal == "" || dateVal == "") {
        var missingVal = "";
        if (firstnameVal == "")
            missingVal = "First Name";
        else if (lastnameVal == "")
            missingVal = "Last Name";
        else if (meetingVal == "")
            missingVal = "Meeting Name. Re-enter the meeting";
        else if (dateVal == "")
            missingVal = "Meeting Date. Re-enter the meeting";

        alert("Data not submitted — form incomplete: " + missingVal);
        return;
    } else {

        var dtParts = dateVal.split("-");
        // grab the values entered into the form fields and store them in an object ready for being inserted into the IDB
        var newItem = [
          { index: indexVal, studentid: studentidVal, firstname: firstnameVal, lastname: lastnameVal, email: emailVal, day: dtParts[2], month: dtParts[1], year: dtParts[0], meeting: meetingVal }
        ];

        // open a read/write db transaction, ready for adding the data
        var transaction = db.transaction([dbName], "readwrite");

        // report on the success of opening the transaction
        transaction.oncomplete = function () {
            //alert("Transaction completed: database modification finished.");
        }

        transaction.onerror = function () {
            alert("Transaction not opened due to error: " + transaction.error);
        }

        // call an object store that's already been added to the database
        var objectStore = transaction.objectStore(dbName);
        /*console.log(objectStore.indexNames);
        console.log(objectStore.keyPath);
        console.log(objectStore.name);
        console.log(objectStore.transaction);
        console.log(objectStore.autoIncrement);*/

        // add our newItem object to the object store
        var objectStoreRequest = objectStore.add(newItem[0]);
        objectStoreRequest.onsuccess = function (event) {

            // report the success of our new item going into the database
            //alert("New item added to database.");

            // clear the form, ready for adding the next entry
            $("#firstname").val("");
            $("#lastname").val("");
            $("#studentid").val("");
            $("#email").val("");

        }
    }
}

function GetQueryStringParams(sParam) {
    var sPageURL = window.location.search.substring(1);
    var sURLVariables = sPageURL.split('&');
    for (var i = 0; i < sURLVariables.length; i++) {
        var sParameterName = sURLVariables[i].split('=');
        if (sParameterName[0] == sParam) {
            return sParameterName[1];
        }
    }
    return null;
}

function deleteItems(meeting, date) {
    var transaction = db.transaction([dbName], "readwrite");
    var objectStore = transaction.objectStore(dbName);

    objectStore.openCursor().onsuccess = function (event) {
        try {
            var cursor = event.target.result;
            if (cursor) {
                var cursorDate = cursor.value.year + "-" + cursor.value.month + "-" + cursor.value.day;
                if (meeting == cursor.value.meeting && date == cursorDate) {
                    cursor.delete();
                }
                cursor.continue();
            } else {
                //alert("All elements displayed.");
                alert("Data for '" + meeting + date + "' cleared.");
                location.reload();
            }
        }
        catch (err) {
            alert(err.message);
        }
    }
}

function deleteItem(index) {

    // open a database transaction and delete the task, finding it by the name we retrieved above
    var transaction = db.transaction([dbName], "readwrite");
    var request = transaction.objectStore(dbName).delete(index);

    // report that the data item has been deleted
    transaction.oncomplete = function () {
        //alert('Task \"' + dataTask + '\" deleted.');
    }
}

function clearData() {
    // open a read/write db transaction, ready for clearing the data
    var transaction = db.transaction([dbName], "readwrite");

    // report on the success of opening the transaction
    transaction.oncomplete = function (event) {
        //alert("Transaction completed: database modification finished.");
    };


    transaction.onerror = function (event) {
        alert("Transaction not opened due to error: " + transaction.error);
    };

    // create an object store on the transaction
    var objectStore = transaction.objectStore(dbName);

    // clear all the data out of the object store
    var objectStoreRequest = objectStore.clear();

    objectStoreRequest.onsuccess = function (event) {
        // report the success of our clear operation
        alert("Data cleared.");
        location.reload();
    }
}

var outputText = "";

function createOutput(tableSelector, meeting, date) {
    var transaction = db.transaction([dbName], "readonly");
    var objectStore = transaction.objectStore(dbName);
    outputText = "";

    objectStore.openCursor().onsuccess = function (event) {
        try {
            var cursor = event.target.result;
            if (cursor) {
                var cursorDate = cursor.value.year + "-" + cursor.value.month + "-" + cursor.value.day;
                if ((meeting === "" && date === "") || (meeting === "null" && date === "null") || (meeting === cursor.value.meeting && date === cursorDate)) {
                    var tableRow = "<tr>" +
                        "<td>" + cursor.value.firstname + "</td>" +
                        "<td>" + cursor.value.lastname + "</td>";

                    if (cursor.value.studentid != undefined)
                        tableRow = tableRow + "<td>" + cursor.value.studentid + "</td>";
                    else
                        tableRow = tableRow + "<td></td>";

                    tableRow = tableRow +
                        "<td>" + cursor.value.email + "</td>" +
                        "<td>" + cursor.value.meeting + "</td>" +
                        "<td>" + cursor.value.year + "-" + cursor.value.month + "-" + cursor.value.day + "</td>" +
                        "</tr>";
                    $(tableSelector).after(tableRow);

                    outputText = outputText + 
                        cursor.value.firstname + "," +
                        cursor.value.lastname + ",";

                    if (cursor.value.studentid != undefined)
                        outputText = outputText + cursor.value.studentid + ",";
                    else
                        outputText = outputText + ",";

                    outputText = outputText +
                        cursor.value.email + "," +
                        cursor.value.meeting + "," +
                        cursor.value.year + "-" + cursor.value.month + "-" + cursor.value.day +
                        "\n";
                }

                cursor.continue();
            } else {
                //alert("All elements displayed.");
                try {
                    if (cursorCallback != undefined)
                        cursorCallback(outputText);
                }
                catch (err) {

                }
            }
        }
        catch (err) {
            alert(err.message);
        }
    }
}

var outputObject = [];

function getJson(meeting, date) {
    var transaction = db.transaction([dbName], "readonly");
    var objectStore = transaction.objectStore(dbName);
    outputObject = [];

    objectStore.openCursor().onsuccess = function (event) {
        try {
            var cursor = event.target.result;
            if (cursor) {
                var cursorDate = cursor.value.year + "-" + cursor.value.month + "-" + cursor.value.day;
                if ((meeting === "" && date === "") || (meeting === "null" && date === "null") || (meeting === cursor.value.meeting && date === cursorDate)) {
                    var val = cursor.value;
                    delete val.index;
                    outputObject[outputObject.length] = val;
                }

                cursor.continue();
            } else {
                //alert("All elements displayed.");
                try {
                    if (cursorObjectCallback != undefined)
                        cursorObjectCallback(outputObject);
                }
                catch (err) {

                }
            }
        }
        catch (err) {
            alert(err.message);
        }
    }
}

var outputObjectsM = [];
var outputMeetings = [];

function getMeetings() {
    var transaction = db.transaction([dbName], "readonly");
    var objectStore = transaction.objectStore(dbName);
    outputObjectsM = [];
    outputMeetings = [];

    objectStore.openCursor().onsuccess = function (event) {
        try {
            var cursor = event.target.result;
            if (cursor) {
                var val = cursor.value;
                delete val.index;
                outputObjectsM[outputObjectsM.length] = val;
                cursor.continue();
            } else {
                //alert("All elements displayed.");

                for (var i = 0; i < outputObjectsM.length; i++) {
                    var meet = outputObjectsM[i].meeting;
                    var date = outputObjectsM[i].year + "-" + outputObjectsM[i].month + "-" + outputObjectsM[i].day;
                    var outputMeetLen = outputMeetings.length;
                    var newMeet = true;
                    if (outputMeetings.length > 0) {
                        for (var j = 0; j < outputMeetLen; j++) {
                            if (meet === outputMeetings[j].meeting) {
                                newMeet = false;
                                break;
                            }
                        }
                        if (newMeet) {
                            outputMeetings[outputMeetings.length] = { "meeting": meet, "date": date };
                        }
                    }
                    else {
                        outputMeetings[outputMeetings.length] = { "meeting": meet, "date": date };
                    }
                }

                try {
                    if (meetingCallback != undefined)
                        meetingCallback(outputMeetings);
                }
                catch (err) {

                }
            }
        }
        catch (err) {
            alert(err.message);
        }
    }
}