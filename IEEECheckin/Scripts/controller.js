(function () {

    // Adds a hash function to strings (used for indexing) 
    String.prototype.hashCode = function () {
        var hash = 0, i, chr, len;
        if (this.length === 0) return hash;
        for (i = 0, len = this.length; i < len; i++) {
            chr = this.charCodeAt(i);
            hash = ((hash << 5) - hash) + chr;
            hash |= 0; // Convert to 32bit integer
        }
        return hash.toString();
    };

    // Adds the number of days to the date object
    Date.prototype.addDays = function (days) {
        var dat = new Date(this.valueOf());
        dat.setDate(dat.getDate() + days);
        return dat;
    }

    // Checks if a string is not null, undefined, and empty
    function checkStr(str) {
        return (str !== null && str !== undefined && str.trim() !== "")
    }

    // The AngularJS module.
    var app = angular.module('checkin', ['ngRoute', 'ngCookies', 'indexedDB']);
    var popMsgTimeout = 6000;

    // Stores and communicates the current id between controllers
    app.service('$currentMeetingService', function ($indexedDB) {
        var _id = "";

        var setMeeting = function (id) {
            _id = id;
        };

        var getMeeting = function (callback) {
            // Get existing meeting
            $indexedDB.openStore('meetings', function (meetings) {
                meetings.find(_id).then(function (e) {
                    callback(e);
                });
            });
        };

        return {
            "setMeeting": setMeeting,
            "getMeeting": getMeeting
        };
    });

    /**
    * Updates the format on the page from the cookies
    */
    app.service('$formatService', function ($cookies) {

        var updateFormat = function () {

            var iu = $cookies.get("image-url");
            if (iu != null && iu != undefined && iu !== "") {
                $cookies.put("image-url", iu, { expires: new Date().addDays(365), path: "/" });
                $("#logoImage").attr("src", iu);
                $("#logoImage").removeAttr("visibility");
            }
            else {
                $("#logoImage").attr("visibility", "hidden");
            }

            var bbc = $cookies.get("body-background-color");
            if (bbc != null && bbc != undefined) {
                $cookies.put("body-background-color", bbc, { expires: new Date().addDays(365), path: "/" });
                $("body").css("background-color", "#" + bbc);
            }

            var bubc = $cookies.get("button-background-color");
            if (bubc != null && bubc != undefined) {
                $cookies.put("button-background-color", bubc, { expires: new Date().addDays(365), path: "/" });
                $("button").css("background-color", "#" + bubc);
                $("input[type='submit'][class*='form-control']").css("background-color", "#" + bubc);
            }

            var bc = $cookies.get("body-color"); // text color
            if (bc != null && bc != undefined) {
                $cookies.put("body-color", bc, { expires: new Date().addDays(365), path: "/" });
                $("body").css("color", "#" + bc);
                $("button").css("color", "#" + bc);
                $("input[type='submit'][class*='form-control']").css("color", "#" + bc);
                $("[class*='boxed-section']").css("border", "1px dashed #" + bc);
            }

            var ts = $cookies.get("theme-shade");
            if (ts != null && ts != undefined && ts === "light") {
                $cookies.put("theme-shade", ts, { expires: new Date().addDays(365), path: "/" });
                $("img[class*='shaded']").each(function () {
                    $(this).attr("src", $(this).attr("src").replace("dark", "light"));
                });
            }
            else if (ts != null && ts != undefined && ts === "dark") {
                $cookies.put("theme-shade", ts, { expires: new Date().addDays(365), path: "/" });
                $("img[class*='shaded']").each(function () {
                    $(this).attr("src", $(this).attr("src").replace("light", "dark"));
                });
            }

            var ht = $cookies.get("header-text");
            if (ht != null && ht != undefined) {
                $cookies.put("header-text", ht, { expires: new Date().addDays(365), path: "/" });
                $("#topHeader").html(ht);
            }

            var us = $cookies.get("use-swipe");
            if (us != null && us != undefined && us === "true") {
                $cookies.put("use-swipe", us, { expires: new Date().addDays(365), path: "/" });
                $("#swipe-section").attr("style", "display: inherit;");
            }
            else {
                $("#swipe-section").attr("style", "display: none;");
            }

            var sel = $cookies.get("card-regex");
            if (sel != null && sel != undefined)
                $cookies.put("card-regex", sel, { expires: new Date().addDays(365), path: "/" });
        }

        return {
            updateFormat
        };
    });

    // IndexedDB service provider
    app.config(['$indexedDBProvider', function ($indexedDBProvider) {
        var conn = $indexedDBProvider.connection('myIndexedDB');

        conn.upgradeDatabase(1, function (event, db, tx) {
            var attendeesStore = db.createObjectStore('attendees', { keyPath: 'id' });
            attendeesStore.createIndex('meeting_idx', 'meetingId', { unique: false });
            attendeesStore.createIndex('firstName_idx', 'firstName', { unique: false });
            attendeesStore.createIndex('lastName_idx', 'lastName', { unique: false });
            attendeesStore.createIndex('email_idx', 'email', { unique: false });

            var meetingStore = db.createObjectStore('meetings', { keyPath: 'id' });
            meetingStore.createIndex('name_idx', 'name', { unique: false });
            meetingStore.createIndex('date_idx', 'date', { unique: false });
        });
    }]);

    // AngularJS routing provider
    app.config(['$routeProvider', '$locationProvider', function ($routeProvider, $locationProvider) {
        $routeProvider

        // route for the home page
            .when('/', {
            templateUrl: 'Pages/Default.html',
            controller: 'MeetingController'
        })

        // alternate route for the home page
            .when('/Default.aspx', {
            templateUrl: 'Pages/Default.html',
            controller: 'MeetingController'
        })

        // route for the about page
            .when('/about', {
            templateUrl: 'Pages/About.html',
            controller: 'AboutController'
        })

        // route for the contact page
            .when('/checkin', {
            templateUrl: 'Pages/Checkin.html',
            controller: 'CheckinController'
        })

        // route for the attendance page
            .when('/attendance', {
            templateUrl: 'Pages/Attendance.html',
            controller: 'AttendanceController'
        })

        // route for the format page
            .when('/format', {
            templateUrl: 'Pages/Format.html',
            controller: 'FormatController'
        })

        // route to the home page
        .otherwise({
            templateUrl: 'Pages/Default.html',
            controller: 'MeetingController'
            });

        $locationProvider.html5Mode(true);
    }]);

    /**
    *  Controls the meeting creation/select
    */
    app.controller('MeetingController', function ($scope, $route, $routeParams, $location, $timeout, $indexedDB, $currentMeetingService, $formatService) {
        var controller = this;
        $scope.$route = $route;
        $scope.$location = $location;
        $scope.$routeParams = $routeParams;
        $scope.data = { model: null, meetings: [] };

        // Initialize the controller
        controller.init = function () {
            $formatService.updateFormat();
            // Enter key pressed
            $("#meetingName").keydown(function (e) {
                var charCode = e.charCode || e.keyCode || e.which;
                if (charCode == 13) {
                    $scope.meetingSubmit();
                }
            });

            controller.updateDropdown();
        }

        // Populate meeting dropdown from pre-existing meetings
        controller.updateDropdown = function () {
            try {
                $indexedDB.openStore('meetings', function (meetings) {
                    meetings.getAll().then(function (e) {
                        // TODO - organize meetings by date
                        $scope.data.meetings = e;
                        $("#meetingDropdown").selectedIndex = "0";
                    });
                });
            }
            catch (err) {
            }
        }

        // Start meeting button clicked
        $scope.meetingSubmit = function () {
            try {
                var existingMeetingId = $scope.data.model;
                var newMeetingName = $("#meetingName").val();
                if ((existingMeetingId == null || existingMeetingId == undefined || existingMeetingId.trim() === "") && (newMeetingName == null || newMeetingName == undefined || newMeetingName.trim() === "")) {
                    $('.alert-info').show();
                    $timeout(function () { $('.alert-info').hide(); }, popMsgTimeout);
                    return;
                }

                if (existingMeetingId == null || existingMeetingId == undefined || existingMeetingId.trim() === "") {
                    // Create new meeting
                    var name = newMeetingName;
                    var d = new Date();
                    var month = d.getMonth();
                    var day = d.getDay();
                    var date = d.getFullYear() + '-' + (month < 10 ? '0' : '') + month + '-' + (day < 10 ? '0' : '') + day;
                
                    // Add new meeting to database
                    $indexedDB.openStore('meetings', function (meetings) {
                        var entry = { "name": name, "date": date };
                        var id = JSON.stringify(entry).hashCode();
                        entry["id"] = id;
                        meetings.insert(entry).then(function (e) {
                            // Set current meeting
                            $currentMeetingService.setMeeting(entry.id);
                            controller.updateDropdown();
                            $location.path("/checkin");
                        }, function (err) {
                            if (err === "Key already exists in the object store.") {
                                err = "This meeting already exists.";
                            }
                            console.log("Error creating meeting: " + err);
                            $('.alert-warning-msg').html("Error creating meeting: " + err);
                            $('.alert-warning').show();
                            $timeout(function () { $('.alert-warning').hide(); }, popMsgTimeout);
                        });
                    });
                }
                else {
                    // Set current meeting
                    $currentMeetingService.setMeeting(existingMeetingId);
                    $location.path("/checkin");
                }
            }
            catch (err) {
                console.log("Error creating or selecting meeting: " + err.message);
                $('.alert-warning-msg').html("Error creating or selecting meeting: " + err.message);
                $('.alert-warning').show();
                $timeout(function () { $('.alert-warning').hide(); }, popMsgTimeout);
            }
        }

        controller.init();
    });

    /**
    * Controls the about page
    */
    app.controller('AboutController', function ($scope, $cookies, $formatService) {
        var controller = this;

        // Initialize the controller
        controller.init = function () {
            $formatService.updateFormat();
            var bc = $cookies.get("body-color"); // text color
            if (bc != null && bc != undefined) {
                $(".link-override").css("color", "#" + bc);
            }
            else {
                $(".link-override").css("color", "rgba(0, 0, 0, 0.8)");
            }
        }

        controller.init();
    });

    /**
    * Controls the checkin.
    */
    app.controller('CheckinController', function ($scope, $timeout, $cookies, $indexedDB, $currentMeetingService, $formatService) {
        var controller = this;
        // Current meeting object
        controller.meeting = {};
        // Content to display in the meeting title
        $scope.meetingDisplayName = "";
        // Mapping of the indicies of the groups in the regex to the data values
        controller.indices = {
            "firstName": "5,7",
            "lastName": "4",
            "middleName": "6",
            "studentId": "2",
            "email": "-1"
        };

        // Initialize the controller
        controller.init = function () {
            $formatService.updateFormat();
            controller.setFocus();
            // Subscribe to the keydown events
            // Move from firstname to lastname
            $("#firstname").keydown(function (e) {
                var charCode = e.charCode || e.keyCode || e.which;
                if (charCode == 13) {
                    e.preventDefault();
                    $("#lastname").focus();
                    return false;
                }
            });
            // Move from lastname to email
            $("#lastname").keydown(function (e) {
                var charCode = e.charCode || e.keyCode || e.which;
                if (charCode == 13) {
                    e.preventDefault();
                    $("#email").focus();
                    return false;
                }
            });
            // Submit the form
            $("#email, #cardtxt").keydown(function (e) {
                var charCode = e.charCode || e.keyCode || e.which;
                if (charCode == 13) {
                    e.preventDefault();
                    $scope.entrySubmit();
                    return false;
                }
            });

            // Get meeting and set name
            $currentMeetingService.getMeeting(function (e) {
                $scope.meetingDisplayName = e.name;
                controller.meeting = e;
            });
        }

        // Add entry to IndexedDB
        $scope.entrySubmit = function() {
            try {
                $('.alert-success').hide();
                var entry = null;
                // Perform the parse function if the ID card input is not empty
                if (checkStr($("#cardtxt").val())) {
                    // Create/retrieve regular expression (this is default for UMN card)
                    var regex = "^%(\\w+)\\^(\\d+)\\^{3}(\\d+)\\^(\\w+),\\s(?:([\\w\\s]+)\\s(\\w{1})\\?;|([\\w\\s]+)\\?;)(\\d+)=(\\d+)\\?$";
                    
                    // Try and get card parse regex from cookie's
                    var re = $cookies.get("card-regex");
                    if (re != null && re != undefined) {
                        var rejson = JSON.parse(re);
                        if (checkStr(rejson["regex"]))
                            regex = rejson["regex"];
                        if (rejson["indices"] != null && rejson["indices"] != undefined)
                            controller.indices = rejson["indices"];
                    }

                    // Parse the card
                    var result = controller.parseCard($("#cardtxt").val().trim(), regex, controller.indices);
                    if (result === null || result === undefined) {
                        $('.alert-warning-msg').html("Failed to parse card data. Try again or use manual entry.");
                        $('.alert-warning').show();
                        setTimeout(function () { $('.alert-warning').hide(); }, popMsgTimeout);
                        controller.clearForm();
                        controller.setFocus();
                        return false;
                    }

                    // Check to make sure first name and last name id are present in the results
                    var datas = ["firstName", "lastName"];
                    var datasReadable = ["First Name", "Last Name"];
                    var index;
                    for (index = 0; index < datas.length; index++) {
                        if (!checkStr(result[datas[index]])) {
                            $('.alert-warning-msg').html("Missing " + datasReadable[index] + ".");
                            $('.alert-warning').show();
                            setTimeout(function () { $('.alert-warning').hide(); }, popMsgTimeout);
                            controller.clearForm();
                            controller.setFocus();
                            return;
                        }
                    }

                    // Check if email is present
                    var emailVal = "";
                    if (checkStr($("#email").val()))
                        emailVal = $("#email").val().trim();

                    // Create new database entry
                    entry = {
                        "firstName": result["firstName"],
                        "lastName": result["lastName"],
                        "email": emailVal,
                        "meetingId": controller.meeting.id
                    };
                    var id = JSON.stringify(entry).hashCode();
                    entry["id"] = id;
                }
                // ID card slot empty so check for valid manual entry
                else if (checkStr($("#firstname").val()) && checkStr($("#lastname").val())) {
                    // Check if email is present
                    var emailVal = "";
                    if (checkStr($("#email").val()))
                        emailVal = $("#email").val().trim();

                    // Create new database entry
                    entry = {
                        "firstName": $("#firstname").val().trim(),
                        "lastName": $("#lastname").val().trim(),
                        "email": emailVal,
                        "meetingId": controller.meeting.id
                    };
                    var id = JSON.stringify(entry).hashCode();
                    entry["id"] = id;
                }
                // Both input methods are invalid
                else {
                    // Card input is empty, but manual is not empty, but incomplete so describe what is missing
                    if (!checkStr($("#cardtxt").val()) && (checkStr($("#firstname").val()) || checkStr($("#lastname").val()))) {
                        // Firstname missing
                        if (!checkStr($("#firstname").val()) && checkStr($("#lastname").val())) {
                            $('.alert-warning-msg').html("Missing First Name.")
                            $('.alert-warning').show();
                            $timeout(function () { $('.alert-warning').hide(); }, popMsgTimeout);
                            $("#firstname").focus();
                            return;
                        }
                        // Lastname missing
                        else if (!checkStr($("#lastname").val()) && checkStr($("#firstname").val())) {
                            $('.alert-warning-msg').html("Missing Last Name.");
                            $('.alert-warning').show();
                            $timeout(function () { $('.alert-warning').hide(); }, popMsgTimeout);
                            $("#lastname").focus();
                            return;
                        }
                        // Something else missing
                        else {
                            $('.alert-warning-msg').html("Missing Card or Manual Input Data.");
                            $('.alert-warning').show();
                            $timeout(function () { $('.alert-warning').hide(); }, popMsgTimeout);
                        }
                    }
                    // Card input and manual input are empty
                    else {
                        $('.alert-warning-msg').html("Missing Card or Manual Input Data.");
                        $('.alert-warning').show();
                        $timeout(function () { $('.alert-warning').hide(); }, popMsgTimeout);
                    }

                    controller.setFocus();

                    return;
                }

                // Add entry to database
                if (entry !== null) {
                    $indexedDB.openStore('attendees', function (attendees) {
                        attendees.insert(entry).then(function (e) {
                            controller.clearForm();
                            controller.setFocus();

                            $('.alert-success').show();
                            $timeout(function () { $('.alert-success').hide(); }, 3000);
                        }, function (err) {
                            if (err === "Key already exists in the object store.") {
                                err = "Person already entered for this meeting.";
                            }
                            $('.alert-warning-msg').html("Error adding person: " + err);
                            $('.alert-warning').show();
                            $timeout(function () { $('.alert-warning').hide(); }, popMsgTimeout);
                        });
                    });
                }
            }
            catch (err) {
                controller.setFocus();
            }

            return;
        }

        // Execute the regular expression to parse the card information
        controller.parseCard = function (cardRaw, regEx, groupInds) {
            try {
                if (!checkStr(cardRaw) && !checkStr(regEx) && groupInds !== null && groupInds !== undefined)
                    return null;
                cardRaw = cardRaw.trim();

                // Create and execute regex
                var regEx = new RegExp(regEx);
                var m = regEx.exec(cardRaw);

                if (m === null || m === undefined || m.length <= 0)
                    return null;

                // Info we are trying to parse
                var entryInfo = {
                    "firstName": "",
                    "lastName": "",
                    "middleName": "",
                    "email": ""
                };

                // Parse regex for data at indices as provided by user
                var datas = ["firstName", "lastName", "middleName", "email"];
                var index, i;
                for (index = 0; index < datas.length; index++) {
                    var indexName = datas[index];
                    if (checkStr(groupInds[indexName])) {
                        var indexValues = groupInds[indexName].split(",");
                        if (indexValues !== null && indexValues.length > 0) {
                            for (i = 0; i < indexValues.length; i++) {
                                var ind = parseInt(indexValues[i]);
                                if (!isNaN(ind) && ind >= 1 && ind < (m.length - 1)) {
                                    if (checkStr(m[ind])) {
                                        entryInfo[indexName] = m[ind].trim().toLowerCase();
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }

                return entryInfo;
            }
            catch (err) {
                console.log("Error parsing card: " + err.message);
                $('.alert-warning-msg').html("Error parsing card: " + err.message);
                $('.alert-warning').show();
                $timeout(function () { $('.alert-warning').hide(); }, popMsgTimeout);
                return null;
            }
        }

        // Set focus based on saved cookies (either card swipe or first name)
        controller.setFocus = function() {
            var us = $cookies.get("use-swipe");
            if (us != null && us != undefined && us === "true") {
                $("#cardtxt").focus();
            }
            else {
                $("#firstname").focus();
            }
        }

        // Clear the entries in the form
        controller.clearForm = function() {
            $("input.clearable").each(function (index, element) {
                try {
                    $(this).val("");
                }
                catch (err) {

                }
            });
        }

        controller.init();
    });

    /**
    * Controls the attendee management
    */
    app.controller('AttendanceController', function ($scope, $timeout, $indexedDB, $currentMeetingService, $formatService) {
        var controller = this;
        $scope.data = { "model": null, "meetings": [], "attendees": [], "winner": null };
        // Dictionary of meetings (key, meeting)
        controller.meetings = {};
        // Number of attendees in currently selected meeting
        controller.numRows = 0;

        // Initialize the controller
        controller.init = function () {
            $formatService.updateFormat();
            controller.updateDropdown();
            $scope.selectionChanged();
        }

        // Populate meeting dropdown from pre-existing meetings
        controller.updateDropdown = function () {
            $indexedDB.openStore('meetings', function (meetings) {
                meetings.getAll().then(function (e) {
                    // TODO - organize meetings by date
                    for (var i = 0; i < e.length; i = i + 1) {
                        controller.meetings[e[i].id] = e[i];
                    }
                    $scope.data.meetings = e;
                    $("#meetingDropdown").selectedIndex = "0";
                });
            });
        }

        // Meeting dropdown selection changed
        $scope.selectionChanged = function () {
            var meetingId = $scope.data.model;
            if (meetingId == null || meetingId == undefined || meetingId.trim() === "") {
                $indexedDB.openStore('attendees', function (attendees) {
                    attendees.getAll().then(function (e) {
                        controller.updateTable(e);
                    });
                });             
            }
            else {
                $indexedDB.openStore('attendees', function (attendees) {
                    var find = attendees.query();
                    find = find.$eq(meetingId);
                    find = find.$index("meeting_idx");
                    attendees.eachWhere(find).then(function (e) {
                        controller.updateTable(e);
                    });
                });
            }
        }

        // Update the table model with data
        controller.updateTable = function (e) {
            e = data = controller.sortDataByMeeting(e);

            for (var i = 0; i < e.length; i = i + 1) {
                e[i].index = i + 1;
                var meet = controller.meetings[e[i].meetingId];
                if (meet == null || meet == undefined) {
                    e[i].meeting = "";
                    e[i].date = "";
                }
                else {
                    e[i].meeting = meet.name;
                    e[i].date = meet.date;
                }
            }
            controller.numRows = e.length;
            $scope.data.winner = null;
            $scope.data.attendees = e;
        }

        // Clear the meeting(s) and attendees
        $scope.clearSubmit = function () {
            $scope.data.winner = null;
            var meetingId = $scope.data.model;
            if (meetingId == null || meetingId == undefined || meetingId.trim() === "") {
                // Clear all attendees
                $indexedDB.openStore('attendees', function (attendees) {
                    attendees.clear().then(function (e) {
                        $scope.selectionChanged();
                    });
                });
                // Clear all meetings
                $indexedDB.openStore('meetings', function (meetings) {
                    meetings.clear().then(function (e) {
                        controller.updateDropdown();
                    });
                });
            }
            else {
                // Clear attendees from the selected meeting
                $indexedDB.openStore('attendees', function (attendees) {
                    var find = attendees.query();
                    find = find.$eq(meetingId);
                    find = find.$index("meeting_idx");
                    attendees.eachWhere(find).then(function (e) {
                        for (var i = 0; i < e.length; i = i + 1) {
                            attendees.delete(e[i].id).then(function (g) {
                                $scope.selectionChanged();
                            });
                        }
                    });
                });
                // Clear the selected meeting
                $indexedDB.openStore('meetings', function (meetings) {
                    meetings.delete(meetingId).then(function (e) {
                        controller.updateDropdown();
                    });
                });
            }
        }

        // Randomly chooses an attendee of the currently selected meeting
        $scope.lotteryRoll = function () {
            var roll = Math.floor((Math.random() * (controller.numRows - 1)) + 1);
            var firstName = $("#row" + roll + " td:nth-child(2)").text();
            var lastName = $("#row" + roll + " td:nth-child(3)").text();
            $scope.data.winner = { "firstName": firstName, "lastName": lastName };
        }

        // Export attendees to CSV
        $scope.exportCsvSubmit = function () {
            controller.exportSubmit("csv");
        }

        // Export attendees to JSON
        $scope.exportJsonSubmit = function () {
            controller.exportSubmit("json");
        }

        // Formats and exports attendees
        controller.exportSubmit = function (type) {
            var meetingId = $scope.data.model;
            if (meetingId == null || meetingId == undefined || meetingId.trim() === "") {
                // Export all meetings
                $indexedDB.openStore('attendees', function (attendees) {
                    attendees.getAll().then(function (e) {
                        controller.export(type, null, e);
                    });
                });
            }
            else {
                // Export selected meeting
                $indexedDB.openStore('attendees', function (attendees) {
                    var find = attendees.query();
                    find = find.$eq(meetingId);
                    find = find.$index("meeting_idx");
                    attendees.eachWhere(find).then(function (e) {
                        controller.export(type, meetingId, e);
                    });
                });
            }
        }

        // Calls the export service to export the provided entries
        controller.export = function (type, meetingId, data) {
            if (data == null || data == undefined) {
                return;
            }

            data = controller.sortDataByMeeting(data);

            // Populate data with index, meeting name and date
            for (var i = 0; i < data.length; i = i + 1) {
                data[i].index = i + 1;
                var meet = controller.meetings[data[i].meetingId];
                if (meet == null || meet == undefined) {
                    data[i].meeting = "";
                    data[i].date = "";
                }
                else {
                    data[i].meeting = meet.name;
                    data[i].date = meet.date;
                }
                delete data[i].id;
                delete data[i].meetingId;
            }

            // Get meeting name and date for selected meeting (empty for all meetings)
            var name = "meeting_sign_ins";
            var date = "";
            if (meetingId != null && meetingId != undefined && meetingId.trim() != "") {
                var meet = controller.meetings[meetingId];
                if (meet != null && meet != undefined) {
                    name = meet.name;
                    date = meet.date;
                }
            }

            var fileName = name;
            if (date != "") {
                fileName = fileName + "_" + date;
            }

            var dataStr = "";

            // Call the export service
            if (type === "csv") {
                // Convert from JSON to CSV
                dataStr = "Index,First Name,Last Name,Email,Meeting,Date\r\n";
                for (var i = 0; i < data.length; i = i + 1) {
                    dataStr = dataStr + data[i].index + "," + data[i].firstName + "," + data[i].lastName + "," + data[i].email + "," + data[i].meeting + "," + data[i].date + "\r\n";
                }
            }
            else if (type === "json") {
                // Stringify to human readable JSON
                dataStr = JSON.stringify(data, null , 2);
            }
            else {
                return;
            }

            // Perform the client side "download"
            var link = document.createElement("a");
            link.href = "data:Application/octet-stream," + encodeURIComponent(dataStr);
            link.download = fileName + "." + type;

            var evt = document.createEvent("MouseEvent");
            evt.initMouseEvent("click", true, true, window, 0, 0, 0, 0, 0, false, false, false, false, 0, null);
            link.dispatchEvent(evt);
        }

        // Sort the attendees by date
        controller.sortDataByMeeting = function (data) {
            if (data == null || data == undefined) {
                return data;
            }

            for (var i = 0; i < data.length; i = i + 1) {
                // TODO - group attendees by meetings and organize by date
            }

            return data;
        }

        controller.init();
    });

    /**
    * Controls the format edit page
    */
    app.controller('FormatController', function ($scope, $timeout, $cookies, $http, $location, $formatService) {
        var controller = this;
        $scope.data = { thememodel: null, themes: [], regexmodel: null, regexes: [] };
        controller.themes = {};
        controller.regexes = {};

        // Initialize the controller
        controller.init = function () {
            $formatService.updateFormat();
            var dt = new Date();
            $(".footer").html("Powered by the U of M IEEE Tech Subcommittee. &copy; " + dt.getFullYear() + " - University of Minnesota IEEE Student Branch");
            controller.pageLoaded();

            $http.get('/IEEECheckin/Content/themes.json').then(function (res) {
                $scope.data.themes = res.data;
                for (var i = 0; i < $scope.data.themes.length; i = i + 1) {
                    controller.themes[$scope.data.themes[i].id] = $scope.data.themes[i];
                }
            });
            $http.get('/IEEECheckin/Content/regexes.json').then(function (res) {
                $scope.data.regexes = res.data;
                for (var i = 0; i < $scope.data.regexes.length; i = i + 1) {
                    controller.regexes[$scope.data.regexes[i].id] = $scope.data.regexes[i];
                }
            });
        }

        controller.pageLoaded = function() {
            var us = $cookies.get("use-swipe");
            if (us != null && us != undefined && us === "true") {
                $("#swipeCheck").prop("checked", true);
            }
            else {
                $("#swipeCheck").prop("checked", false);
            }

            try {
                var rn = $cookies.get("card-regex-id");
                if (checkStr(rn)) {
                    $scope.data.regexmodel = rn;
                }
                else {
                    $scope.data.regexmodel = null;
                }
            }
            catch (err) {

            }

            try {
                var tn = $cookies.get("theme-id");
                if (checkStr(tn)) {
                    $scope.data.thememodel = tn;
                }
                else {
                    $scope.data.thememodel = null;
                }
            }
            catch (err) {

            }

            var ts = $cookies.get("theme-shade");
            if (ts != null && ts != undefined && ts === "light") {
                $("#themeShade").prop("checked", false);
                $("#themeShadeText").html("Using Light Theme");
            }
            else {
                $("#themeShade").prop("checked", true);
                $("#themeShadeText").html("Using Dark Theme");
            }

            var bbc = $cookies.get("body-background-color");
            if (bbc == null || bbc == undefined)
                bbc = "ffffff";

            var bubc = $cookies.get("button-background-color");
            if (bubc == null || bubc == undefined)
                bubc = "ffffff";

            var bc = $cookies.get("body-color"); // text color
            if (bc == null || bc == undefined)
                bc = "000000";

            $('#colorSelector').ColorPicker({ // body background
                color: bbc,
                onShow: function (colpkr) {
                    $(colpkr).fadeIn(500);
                    return false;
                },
                onHide: function (colpkr) {
                    $(colpkr).fadeOut(500);
                    return false;
                },
                onChange: function (hsb, hex, rgb) {
                    $("body").css("background-color", "#" + hex);
                    $cookies.put("body-background-color", hex, { expires: new Date().addDays(365), path: "/" });
                    $("#colorSelector").css("background-color", "#" + hex);
                    controller.themeUpdated();
                }
            });
            $('#colorSelector2').ColorPicker({ // button color
                color: bubc,
                onShow: function (colpkr) {
                    $(colpkr).fadeIn(500);
                    return false;
                },
                onHide: function (colpkr) {
                    $(colpkr).fadeOut(500);
                    return false;
                },
                onChange: function (hsb, hex, rgb) {
                    $("button").css("background-color", "#" + hex);
                    $cookies.put("button-background-color", hex, { expires: new Date().addDays(365), path: "/" });
                    reduced = parseInt(hex, 16);
                    if (hex & 0x30 >= 0x0A0000)
                        reduced = reduced - 0x0A0000;
                    if (hex & 0x0C >= 0x000A00)
                        reduced = reduced - 0x000A00;
                    if (hex & 0x03 >= 0x00000A)
                        reduced = reduced - 0x00000A;
                    var temp = ("000000" + reduced.toString(16)).substr(-6)
                    $("button").css("border-color", "#" + temp);
                    $("#colorSelector2").css("background-color", "#" + hex);
                    controller.themeUpdated();
                }
            });
            $('#colorSelector3').ColorPicker({ // body (font) color
                color: bc,
                onShow: function (colpkr) {
                    $(colpkr).fadeIn(500);
                    return false;
                },
                onHide: function (colpkr) {
                    $(colpkr).fadeOut(500);
                    return false;
                },
                onChange: function (hsb, hex, rgb) {
                    $("body").css("color", "#" + hex);
                    $("button").css("color", "#" + hex);
                    $cookies.put("body-color", hex, { expires: new Date().addDays(365), path: "/" });
                    $("#colorSelector3").css("background-color", "#" + hex);
                    $("[class*='boxed-section']").css("border", "1px dashed #" + bc);
                    controller.themeUpdated();
                }
            });

            controller.themeUpdated();
        }

        $scope.updateImage = function() {
            var value = $("#imageUrl").val();
            if (value != null && value.toLowerCase().trim() === "ieee") {
                $("#logoImage").attr("src", "../IEEECheckin/Images/logo.svg");
                $("#logoImage").removeAttr("visibility");
                $cookies.put("image-url", "../IEEECheckin/Images/logo.svg", { expires: new Date().addDays(365), path: "/" });
            }
            else if (value != null && value.toLowerCase().trim() === "wie") {
                $("#logoImage").attr("src", "../IEEECheckin/Images/ieee_wie_purple.png");
                $("#logoImage").removeAttr("visibility");
                $cookies.put("image-url", "../IEEECheckin/Images/ieee_wie_purple.png", { expires: new Date().addDays(365), path: "/" });
            }
            else if (value != null && value.toLowerCase().trim() === "gofirst") {
                $("#logoImage").attr("src", "../IEEECheckin/Images/gofirst.png");
                $("#logoImage").removeAttr("visibility");
                $cookies.put("image-url", "../IEEECheckin/Images/gofirst.png", { expires: new Date().addDays(365), path: "/" });
            }
            else if (value != null && value !== "") {
                $("#logoImage").attr("src", value);
                $("#logoImage").removeAttr("visibility");
                $cookies.put("image-url", value, { expires: new Date().addDays(365), path: "/" });
            }
            else {
                $("#logoImage").attr("visibility", "hidden");
            }

            controller.themeUpdated();
            return false;
        }

        $scope.updateTopText = function() {
            var value = $("#topText").val();
            if (value != null) {
                $("#topHeader").html(value);
                $cookies.put("header-text", value, { expires: new Date().addDays(365), path: "/" });
            }
            controller.themeUpdated();
            return false;
        }

        $scope.updateSwipe = function() {
            if ($("#swipeCheck").prop("checked")) {
                $cookies.put("use-swipe", true, { expires: new Date().addDays(365), path: "/" });
                $("#swipe-section").attr("style", "display: inherit;");
            }
            else {
                $cookies.put("use-swipe", false, { expires: new Date().addDays(365), path: "/" });
                $("#swipe-section").attr("style", "display: none;");
            }
            controller.themeUpdated();
            return false;
        }

        $scope.updateThemeShade = function() {
            if ($("#themeShade").prop("checked")) {
                $cookies.put("theme-shade", "dark", { expires: new Date().addDays(365), path: "/" });
                $("#themeShadeText").html("Using Dark Theme");
            }
            else {
                $cookies.put("theme-shade", "light", { expires: new Date().addDays(365), path: "/" });
                $("#themeShadeText").html("Using Light Theme");
            }
            controller.themeUpdated();
            return false;
        }

        $scope.updateRegex = function() {
            var id = $scope.data.regexmodel;
            if (checkStr(id)) {
                var regex = controller.regexes[id].regex;
                $cookies.put("card-regex", JSON.stringify(regex), { expires: new Date().addDays(365), path: "/" });
                $cookies.put("card-regex-id", id, { expires: new Date().addDays(365), path: "/" });
            }
            else {
                $cookies.remove("card-regex", { path: '/' });
                $cookies.remove("card-regex-id", { path: '/' });
                $("#swipeCheck").prop("checked", false);
                $scope.updateSwipe();
            }
            return false;
        }

        $scope.updatePreDefTheme = function () {
            var id = $scope.data.thememodel;
            if (checkStr(id)) {
                $cookies.put("theme-id", id, { expires: new Date().addDays(365), path: "/" });
                var theme = controller.themes[id];
                $("#export").val(JSON.stringify(theme.theme));
                $scope.importTheme();
            }
            else {
                $cookies.put("theme-id", id, { expires: new Date().addDays(365), path: "/" });
                $cookies.remove("theme-id", { path: '/' });
            }
            return false;
        }

        $scope.resetTheme = function() {
            $cookies.remove("body-background-color", { path: '/' });
            $cookies.remove("button-background-color", { path: '/' });
            $cookies.remove("background-color", { path: '/' });
            $cookies.remove("body-color", { path: '/' });
            $cookies.remove("theme-shade", { path: '/' });
            $cookies.remove("image-url", { path: '/' });
            $cookies.remove("header-text", { path: '/' });
            $cookies.remove("use-swipe", { path: "/" });
            $cookies.remove("theme-id", { path: '/' });
            controller.pageLoaded();
            $formatService.updateFormat();
        }

        controller.themeUpdated = function() {
            var bbc = $cookies.get("body-background-color");
            if (bbc == null || bbc == undefined)
                bbc = "ffffff";
            $("#colorSelector").css("background-color", "#" + bbc);

            var bubc = $cookies.get("button-background-color");
            if (bubc == null || bubc == undefined)
                bubc = "ffffff";
            $("#colorSelector2").css("background-color", "#" + bubc);

            var bc = $cookies.get("body-color"); // text color
            if (bc == null || bc == undefined)
                bc = "000000";
            $("#colorSelector3").css("background-color", "#" + bc);

            var ts = $cookies.get("theme-shade");
            if (ts == null || ts == undefined)
                ts = "dark";
            if (ts === "dark") {
                $("#themeShade").prop("checked", true);
                $("#themeShadeText").html("Using Dark Theme");
            }
            else {
                $("#themeShade").prop("checked", false);
                $("#themeShadeText").html("Using Light Theme");
            }

            var iu = $cookies.get("image-url");
            if (iu == null || iu == undefined | iu === "") {
                iu = "";
                $("#logoImage").attr("visibility", "hidden");
            }
            else {
                $("#logoImage").attr("src", iu);
                $("#logoImage").removeAttr("visibility");
            }

            var ht = $cookies.get("header-text");
            if (ht == null || ht == undefined)
                ht = "Meeting Check-in Web App";
            $("#topText").val(ht);

            var us = $cookies.get("use-swipe");
            if (us == null || us == undefined)
                us = "false";
            if (us === "true") {
                $("#swipeCheck").prop("checked", true);
            }
            else {
                $("#swipeCheck").prop("checked", false);
            }

            var theme = {
                bodyBackgroundColor: bbc,
                buttonBackgroundColor: bubc,
                bodyColor: bc,
                themeShade: ts,
                imageUrl: iu,
                headerText: ht,
                useSwipe: us
            };
            $("#export").val(JSON.stringify(theme));

            $formatService.updateFormat();
        }

        $scope.importTheme = function() {
            try {
                var theme = jQuery.parseJSON($("#export").val());
                if (theme == null || theme == undefined)
                    return false;

                var bbc = theme.bodyBackgroundColor;
                if (bbc != null && bbc != undefined)
                    $cookies.put("body-background-color", bbc, { expires: new Date().addDays(365), path: "/" });

                var bubc = theme.buttonBackgroundColor;
                if (bubc != null && bubc != undefined)
                    $cookies.put("button-background-color", bubc, { expires: new Date().addDays(365), path: "/" });

                var bc = theme.bodyColor;
                if (bc != null && bc != undefined)
                    $cookies.put("body-color", bc, { expires: new Date().addDays(365), path: "/" });

                var ts = theme.themeShade;
                if (ts != null && ts != undefined)
                    $cookies.put("theme-shade", ts, { expires: new Date().addDays(365), path: "/" });

                var iu = theme.imageUrl;
                if (iu != null && iu != undefined)
                    $cookies.put("image-url", iu, { expires: new Date().addDays(365), path: "/" });

                var ht = theme.headerText;
                if (ht != null && ht != undefined)
                    $cookies.put("header-text", ht, { expires: new Date().addDays(365), path: "/" });

                var us = theme.useSwipe;
                if (us != null && us != undefined)
                    $cookies.put("use-swipe", us, { expires: new Date().addDays(365), path: "/" });

                controller.themeUpdated();
            }
            catch (err) {
            }
        }

        controller.init();
    });

})();