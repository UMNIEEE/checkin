(function () {

    String.prototype.hashCode = function () {
        var hash = 0, i, chr, len;
        if (this.length === 0) return hash;
        for (i = 0, len = this.length; i < len; i++) {
            chr = this.charCodeAt(i);
            hash = ((hash << 5) - hash) + chr;
            hash |= 0; // Convert to 32bit integer
        }
        return hash;
    };

    // The AngularJS module.
    var app = angular.module('checkin', ['ngRoute']);

    app.config(function ($indexedDBProvider) {
        $indexedDBProvider.connection('myIndexedDB')
        .upgradeDatabase(1, function (event, db, tx) {
            var objStore = db.createObjectStore('people', { keyPath: 'id' });
              objStore.createIndex('meetind_idx', 'meeting', { unique: false });
              objStore.createIndex('firstName_idx', 'firstName', { unique: false });
              objStore.createIndex('lastName_idx', 'lastName', { unique: false });
              objStore.createIndex('email_idx', 'email', { unique: false });
              objStore.createIndex('date_idx', 'date', { unique: false });
          });
    });

    app.config(function ($routeProvider) {
        $routeProvider

        // route for the home page
        .when('/Default.aspx', {
            templateUrl: 'pages/Default.html',
            controller: 'MainController'
        })

        // route for the about page
        .when('/about', {
            templateUrl: 'pages/About.html',
            controller: 'AboutController'
        })

        // route for the contact page
        .when('/checkin', {
            templateUrl: 'pages/Checkin.html',
            controller: 'CheckinController'
        });
    });

    /**
    *  Controls the meeting creation/select
    */
    app.controller('MainController', ['$scope', '$timeout', function ($scope, $timeout) {
        var controller = this;

        controller.init = function(){
            $("#MainContent_MeetingName").keydown(function (e) {
                var charCode = e.charCode || e.keyCode || e.which;
                if (charCode == 13) {
                    controller.meetingSubmit();
                }
            });
        }

        controller.dbOpenCallback = function() {
            try {
                controller.getMeetings();
            }
            catch (err) {
            }
        }

        controller.meetingCallback = function(meetings) {
            var htmlVal = "";
            for (var i = 0; i < meetings.length; i++) {
                htmlVal = htmlVal + "<option value='" + "meeting=" + meetings[i].meeting + "&" + "date=" + meetings[i].date + "'>" + meetings[i].meeting + " " + meetings[i].date + "</option>";
            }

            $("#meetingDropdown").html("<option value=''>New Meetings</option>" + htmlVal);
        }

        controller.meetingSubmit = function() {
            var dropdown = $("#meetingDropdown option:selected").val();
            var meeting = $("#MainContent_MeetingName").val();
            $("#MainContent_DropdownValue").val(dropdown);
            if ((dropdown == null || dropdown == undefined || dropdown.trim() === "") && (meeting == null || meeting == undefined || meeting.trim() === "")) {
                $('.alert-info').show();
                $timeout(function () { $('.alert-info').hide(); }, 3000);
                return false;
            }

            $("#MainContent_MeetingButtonHidden").click();
            return false;
        }

        controller.decodeMeeting = function(meeting) {
            var meet = "", date = "";

            if (meeting !== null && meeting !== undefined && meeting.trim() !== "") {
                var sURLVariables = meeting.split('&');
                for (var i = 0; i < sURLVariables.length; i++) {
                    var sParameterName = sURLVariables[i].split('=');
                    if (sParameterName[0] == "meeting") {
                        meet = sParameterName[1];
                    }
                }
                for (var i = 0; i < sURLVariables.length; i++) {
                    var sParameterName = sURLVariables[i].split('=');
                    if (sParameterName[0] == "date") {
                        date = sParameterName[1];
                    }
                }
            }

            return { "name": meet, "date": date };
        }

    }]);

    /**
    * Controls the checkin.
    */
    app.controller('CheckinController', ['$scope', '$timeout', '$indexedDB', function ($scope, $timeout, $indexedDB) {
        var controller = this;
        controller.successPromise;
        controller.indices = {
            "firstName": "5,7",
            "lastName": "4",
            "middleName": "6",
            "studentId": "2",
            "email": "-1"
        };

        controller.init = function () {
            controller.setFocus();
            $("#meetingName").html($("#MainContent_MeetingName").val());
            // subscribe to the keydown events
            // move from firstname to lastname
            $("#firstname").keydown(function (e) {
                var charCode = e.charCode || e.keyCode || e.which;
                if (charCode == 13) {
                    e.preventDefault();
                    $("#lastname").focus();
                    return false;
                }
            });
            // move from lastname to email
            $("#lastname").keydown(function (e) {
                var charCode = e.charCode || e.keyCode || e.which;
                if (charCode == 13) {
                    e.preventDefault();
                    $("#email").focus();
                    return false;
                }
            });
            // submit the form
            $("#email, #cardtxt").keydown(function (e) {
                var charCode = e.charCode || e.keyCode || e.which;
                if (charCode == 13) {
                    e.preventDefault();
                    entrySubmit();
                    return false;
                }
            });
        }

        controller.entrySubmit = function() {
            try {
                cancel(controller.successPromise);
                $('.alert-success').hide();
                var entry = null;
                // perform the parse function if the student id card input is not empty
                if (checkStr($("#cardtxt").val())) {
                    // create/retrieve regular expression
                    var regex = "^%(\\w+)\\^(\\d+)\\^{3}(\\d+)\\^(\\w+),\\s(?:([\\w\\s]+)\\s(\\w{1})\\?;|([\\w\\s]+)\\?;)(\\d+)=(\\d+)\\?$";
                    
                    var re = $.cookie("card-regex");
                    if (re != null && re != undefined) {
                        var rejson = JSON.parse(re);
                        if (checkStr(rejson["regex"]))
                            regex = rejson["regex"];
                        if (rejson["indices"] != null && rejson["indices"] != undefined)
                            controller.indices = rejson["indices"];
                    }

                    // parse the card
                    var result = controller.parseCard($("#cardtxt").val().trim(), regex, controller.indices);
                    if (result === null || result === undefined) {
                        $('.alert-warning-msg').html("Failed to parse card data. Try again or use manual entry.")
                        $('.alert-warning').show();
                        setTimeout(function () { $('.alert-warning').hide(); }, 3000);
                        controller.clearForm();
                        controller.setFocus();
                        return false;
                    }

                    // check to make sure first name and last name id are present in the results
                    var datas = ["firstName", "lastName"];
                    var datasReadable = ["First Name", "Last Name"];
                    var index;
                    for (index = 0; index < datas.length; index++) {
                        if (!checkStr(result[datas[index]])) {
                            $('.alert-warning-msg').html("Missing " + datasReadable[index] + ".")
                            $('.alert-warning').show();
                            setTimeout(function () { $('.alert-warning').hide(); }, 3000);
                            controller.clearForm();
                            controller.setFocus();
                            return false;
                        }
                    }

                    // check if email is present
                    var emailVal = "";
                    if (checkStr($("#email").val()))
                        emailVal = $("#email").val().trim();

                    // create new database entry
                    entry = {
                        "firstName": result["firstName"],
                        "lastName": result["lastName"],
                        "email": emailVal,
                        "meeting": $("#MainContent_MeetingName").val().trim(),
                        "date": $("#MainContent_MeetingDate").val().trim()
                    };
                }
                // student id card slot empty so check for valid manual entry
                else if (checkStr($("#firstname").val()) && checkStr($("#lastname").val())) {
                    // check if email is present
                    var emailVal = "";
                    if (checkStr($("#email").val()))
                        emailVal = $("#email").val().trim();
                    // check if student id present (for whatever reason)
                    var studentId = "";
                    if (checkStr($("#studentid").val()))
                        studentId = $("#studentid").val().trim();

                    // create new database entry
                    entry = {
                        "firstName": $("#firstname").val().trim(),
                        "lastName": $("#lastname").val().trim(),
                        "email": emailVal,
                        "meeting": $("#MainContent_MeetingName").val().trim(),
                        "date": $("#MainContent_MeetingDate").val().trim()
                    };
                }
                // both are invalid
                else {
                    // card input is empty, but manual is not empty, but incomplete
                    if (!checkStr($("#cardtxt").val()) && (checkStr($("#firstname").val()) || checkStr($("#lastname").val()))) {
                        // firstname missing
                        if (!checkStr($("#firstname").val()) && checkStr($("#lastname").val())) {
                            $('.alert-warning-msg').html("Missing First Name.")
                            $('.alert-warning').show();
                            $timeout(function () { $('.alert-warning').hide(); }, 3000);
                            $("#firstname").focus();
                            return false;
                        }
                            // lastname missing
                        else if (!checkStr($("#lastname").val()) && checkStr($("#firstname").val())) {
                            $('.alert-warning-msg').html("Missing Last Name.")
                            $('.alert-warning').show();
                            $timeout(function () { $('.alert-warning').hide(); }, 3000);
                            $("#lastname").focus();
                            return false;
                        }
                            // something else missing
                        else {
                            $('.alert-warning-msg').html("Missing Card or Manual Input Data.")
                            $('.alert-warning').show();
                            $timeout(function () { $('.alert-warning').hide(); }, 3000);
                        }
                    }
                        // card input and manual input are empty
                    else {
                        $('.alert-warning-msg').html("Missing Card or Manual Input Data.")
                        $('.alert-warning').show();
                        $timeout(function () { $('.alert-warning').hide(); }, 3000);
                    }

                    controller.setFocus();

                    return false;
                }

                if (entry !== null) {
                    // add entry to database
                    $indexedDB.openStore('people', function (store) {

                        store.insert(entry).then(function (e) {
                            controller.clearForm();
                            controller.setFocus();

                            $('.alert-success').show();
                            controller.successPromise = $timeout(function () { $('.alert-success').hide(); }, 3000);
                        });
                    });
                }
            }
            catch (err) {
                controller.setFocus();
            }

            return false;
        }

        // Execute the regular expression to parse the card information
        controller.parseCard = function (cardRaw, regEx, groupInds) {
            //decode u-card
            try {

                if (!checkStr(cardRaw) && !checkStr(regEx) && groupInds !== null && groupInds !== undefined)
                    return null;
                cardRaw = cardRaw.trim();

                // create and execute regex
                var regEx = new RegExp(regEx);
                var m = regEx.exec(cardRaw);

                if (m === null || m === undefined || m.length <= 0)
                    return null;

                var entryInfo = {
                    "firstName": "",
                    "lastName": "",
                    "middleName": "",
                    "email": ""
                };

                // parse regex for data at indices as provided by user
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
                alert(err.message);
                return null;
            }
        }

        // set focus based on saved cookies
        controller.setFocus = function() {
            var us = $.cookie("use-swipe");
            if (us != null && us != undefined && us === "true") {
                $("#cardtxt").focus();
            }
            else {
                $("#firstname").focus();
            }
        }

        // clear the entries in the form
        controller.clearForm = function() {
            $("input.clearable").each(function (index, element) {
                try {
                    $(this).val("");
                }
                catch (err) {

                }
            });
        }

    }]);

})();