var globData = [];
var globRawData = [];
var globUCard = {};
var globUPerson = {};

// Change selector to the id of the input where the U-card data will go
var cardInputBoxSelector = "#MainContent_cardTxt";

// Performs a U of M person search due to a button click.
function manualSearch(firstName, lastName) {
    if (globUCard === null || globUCard === undefined) {
        globUCard = {
            "firstName": firstName.trim(),
            "lastName": lastName.trim(),
            "middleInit": "",
            "studentId": -1,
            "libraryId": "",
            "codabar": "",
            "someNumOne": -1,
            "someNumTwo": -1
        };
    }
    else {
        globUCard["firstName"] = firstName;
        globUCard["lastName"] = lastName;
    }
    var urlPath = "http://www.umn.edu/lookup?SET_INSTITUTION=UMNTC&type=name&CN=" +
                    firstName + "+" + lastName + "&campus=a&role=any";
    crawl(urlPath);
    return false;
}
// Parses the individual person result returned by the crawl
function parseData() {
    if (globData === null || globData === undefined || globData.length === 0)
        return false;

    // TODO - parse U of M data here
    var regFirst = new RegExp(globUCard["firstName"], "i");
    var regLast = new RegExp(globUCard["lastName"], "i");

    var firstNameInd = globData[0].search(regFirst);
    if (firstNameInd >= 0 && globUCard !== null && globUCard !== undefined && globUCard["firstName"].trim() !== "")
        globUCard["firstName"] = globData[0].substr(firstNameInd, globUCard["firstName"].length).trim();
    else
        firstNameInd = -1;
    
    var lastNameInd = globData[0].search(regLast);
    if (lastNameInd >= 0 && globUCard !== null && globUCard !== undefined && globUCard["lastName"].trim() !== "")
        globUCard["lastName"] = globData[0].substr(lastNameInd, globUCard["lastName"].length).trim();
    else
        lastNameInd = -1;

    if (firstNameInd < 0) {
        if (lastNameInd >= 0) {
            globUCard["firstName"] = globData[0].substr(0, lastNameInd - 3).trim();
        }
    }
    if (lastNameInd < 0) {
        if (firstNameInd >= 0) {
            globUCard["lastName"] = globData[0].substr(firstNameInd + 3).trim();
        }
    }

    var email = "";
    var college = 0;
    var collegeName = "";
    var campus = 0;
    var campusName = "";
    var sem = 0;
    var semName = "";
    var year = "";
    var x500 = "";
    var street = "";
    var city = "";
    var state = "";
    var zip = "";
    var phone = "";

    var i = 0;
    for (i = 0; i < globData.length; i++) {
        var value = globData[i];
        if (value.search("Email Address:") >= 0) {
            email = value.split(":")[1].trim();
        }
        else if (value.search("Enrollment:") >= 0) {
            var enroll = globRawData[i].split(":")[1].trim();
            enroll = enroll.split(/(\r\n|\n|\r)/gm);
            // remove empty spaces
            var j = 0;
            var newEnroll = [];
            for (j = 0; j < enroll.length; j++) {
                if (enroll[j].trim() !== "")
                    newEnroll[newEnroll.length] = enroll[j].trim();
            }
            enroll = newEnroll;
            if (enroll !== undefined && enroll !== null && enroll.length >= 3) {
                collegeName = enroll[0].split("-")[0].trim();
                switch (enroll[0].split("-")[0].trim()) {
                    case "Science/Eng":
                        college = 1;
                        break;
                    case "Lib Arts":
                        college = 2;
                        break;
                    case "Bio Science":
                        college = 3;
                        break;
                    case "Management":
                        college = 4;
                        break;
                    case "Design":
                        college = 5;
                        break;
                    case "Food/Agr/Nat Res Sci":
                        college = 6;
                        break;
                    case "Educ/Hum Dev":
                        college = 7;
                        break;
                }
                campusName = enroll[1];
                switch (enroll[1]) {
                    case "Crookston":
                        campus = 1;
                        break;
                    case "Duluth":
                        campus = 2;
                        break;
                    case "Morris":
                        campus = 3;
                        break;
                    case "Rochester":
                        campus = 4;
                        break;
                    case "Twin Cities":
                        campus = 5;
                        break;
                    case "Other":
                        campus = 6;
                        break;
                }
                var enrollDate = enroll[2].split(" ");
                if (enrollDate !== undefined && enrollDate !== null && enrollDate.length == 2) {
                    semName = enrollDate[0].trim();
                    switch (enrollDate[0].trim()) {
                        case "Fall":
                            sem = 1;
                            break;
                        case "Spring":
                            sem = 2;
                            break;
                        case "May Term": // probably not correct
                            sem = 3;
                            break;
                        case "Summer":
                            sem = 4;
                    }
                    year = enrollDate[1];
                }
            }
        }
        else if (value.search("Internet ID:") >= 0) {
            x500 = value.split(":")[1].trim();
        }
        else if (value.search("Mobile/Cell Phone:") >= 0) {
            phone = value.split(":")[1].trim();
        }
        else if (value.search("Phone:") >= 0 && phone === "") {
            phone = value.split(":")[1].trim();
        }
        else if (value.search("Address:") >= 0) {
            var addr = globRawData[i].split(":")[1].trim();
            addr = addr.split(/(\r\n|\n|\r)/gm);
            var k = 0;
            var newAddr = [];
            for (k = 0; k < addr.length; k++) {
                if (addr[k].trim() !== "")
                    newAddr[newAddr.length] = addr[k].trim();
            }
            addr = newAddr;
            if (addr !== undefined && addr !== null && addr.length >= 2) {
                street = addr[0].trim();
                city = addr[1].split(",")[0].trim();
                var stateZip = addr[1].split(",")[1].trim();
                state = stateToIndex(stateZip.split(" ")[0].trim()); // TODO - get state from abbreviation to index
                zip = stateZip.split(" ")[1].trim();
            }
        }
    }

    var uPerson = {};
    try {
        uPerson = {
            "firstName": globUCard["firstName"].trim(),
            "lastName": globUCard["lastName"].trim(),
            "email": email.trim(),
            "phone": phone.trim(),
            "x500": x500.trim(),
            "college": collegeName.trim(),
            "campus": campusName.trim(),
            "enrollSem": semName.trim(),
            "enrollYear": year.trim(),
            "street": street.trim(),
            "city": city.trim(),
            "zipcode": zip.trim(),
            "state": state.trim()
        };
        globUPerson = uPerson;
    }
    catch (err) {

    }

    try{
        parseCrawlComplete(uPerson);
    }
    catch (err) {

    }

    return false;
}
// Parses the individual selected from a search result for multiple individuals and formats for displaying.
function searchResultSelect(row) {
    if (row === null || row === undefined)
        return;

    var urlPath = "http://www.umn.edu" + $("#" + row).attr("title");
    var resultsHtml = "";
    $.ajax({
        url: urlPath, type: 'GET', success: function (res) {
            var data = [];
            var head = $(res.responseText).find('tr').each(function (idx, item) {
                var value = $(this).text().trim();
                data[data.length] = value;
            }); // find each

            // get first name data
            var h2Result = $(res.responseText).find('h2:first');
            var value = h2Result.text().trim().replace(/(\r\n|\n|\r)/gm, " ");
            data.unshift(value);
            data.unshift(h2Result.text().trim())

            if(data.length <= 0) {
                resultsHtml = "No results found. <br/>"
                return;
            }

            var i = 0;
            for (i = 0; i < data.length; i++) {
                resultsHtml += data[i];
                resultsHtml += "<br/>";
            }

            globData = data;
            globRawData = data;

            resultsHtml += "<br/>";
            resultsHtml += '<input type="submit" name="crawlConfirm" value="Confirm" onclick="return parseData();" id="crawlConfirm">';
            $("#crawlConfirm").focus();
            $("#crawlConfirm").keyup(function () {
                parseData();
                return false;
            });
        } // ajax function
    }); // ajax

    try {
        searchResultSelected(resultsHtml);
    }
    catch (err) {

    }
}
// crawls the U of M person search results page, be it an individual or multiple people.
function crawl(urlPath) {
    if (urlPath === null || urlPath === undefined)
        return;

    var resultsHtml = "";
    $.ajax({
        url: urlPath, type: 'GET', success: function (res) {
            var multiResult = false;
            var data = [];
            var rawData = [];

            var head = $(res.responseText).find('tr').each(function (idx, item) {
                if (idx != 0 && multiResult) { // create list to choose from
                    var link = $(this).find('a:first').attr("href");
                    var value = $(this).attr("id", "row" + idx);
                    value = value.attr("title", link);
                    $('#results').append(value);
                    // remove hyperlinks
                    $('#row' + idx).find("a:first").replaceWith("<div class='clickable' onclick='searchResultSelect(\"row" + idx + "\"," + div + ")'>" +
                        $(this).find("a:first").text() + "</div>");
                    $('#row' + idx).find("a:last").replaceWith($(this).find("a:last").text());
                }
                else if (idx != 0 && !multiResult) { // found exact
                    var value = $(this).text().trim().replace(/(\r\n|\n|\r)/gm, " ");
                    rawData[rawData.length] = $(this).text().trim();
                    data[data.length] = value;
                }
                else { // first row
                    var value = $(this).text().trim().replace(/(\r\n|\n|\r)/gm, " ");
                    if (value.search("Name") >= 0 && value.search("Email") >= 0) {
                        multiResult = true;
                        resultsHtml += "<table id='results' style='{list-style: none;}'></table>";
                    }
                    else {
                        data[data.length] = value;
                        rawData[rawData.length] = $(this).text().trim();
                    }
                }
            }); // find each

            if (!multiResult) {
                // get first name data
                var h2Result = $(res.responseText).find('h2:first');
                var value = h2Result.text().trim().replace(/(\r\n|\n|\r)/gm, " ");
                data.unshift(value);
                rawData.unshift(h2Result.text().trim())

                var i = 0;
                for (i = 0; i < data.length; i++) {
                    resultsHtml += data[i];
                    resultsHtml += "<br/>";
                }

                globData = data;
                globRawData = rawData;

                resultsHtml += "<br/>";
                resultsHtml += '<input type="submit" name="crawlConfirm" value="Confirm" onclick="return parseData();" id="crawlConfirm">';
                $("#crawlConfirm").focus();
                $("#crawlConfirm").keyup(function () {
                    parseData();
                    return false;
                });
            }
            else {
                if ($("#results tr").length <= 0) {
                    resultsHtml += "No results found.";
                    resultsHtml += "<br/>";
                }
                else {
                    resultsHtml = $("#results tr").length + " results found." + "<br/>";
                }
            }                
        } // ajax function
    }); // ajax

    return resultsHtml;
}
// document ready, where the U-card parsing happens
$(document).ready(function () {
    // Handle enter press in the first name box for U-card swipe completion.
    $(cardInputBoxSelector).keydown(function (event) {
        if (event.keyCode == 13) {
            //decode u-card
            var ucardRaw = $(cardInputBoxSelector).val();

            var re2 = new RegExp("[?^]");
            var firstSplit = ucardRaw.split(re2);
            if (firstSplit.length < 4) // check if it is actually a u-card
                return;
            
            var firstName = "";
            var middleInit = "";
            var lastName = "";
            var studentId = -1;
            var libraryId = "";
            var codabar = "";
            var someNumOne = -1;
            var someNumTwo = -1;

            // Parse the U-card data for first and last name
            if (firstSplit !== null && firstSplit !== undefined) {

                // Non-essential/less-essential parsing
                try {
                    libraryId = firstSplit[0];
                    libraryId = libraryId.replace("%").trim();
                    if(!isNaN(parseInt(firstSplit[1]))) {
                        studentId = parseInt(firstSplit[1]);
                    }
                }
                catch (err) {

                }

                // First name, last name, middle initial parsing
                var re3 = new RegExp("[,]");
                var i = 0;
                var nameInd = -1;
                for (i = 0; i < firstSplit.length; i++) {

                    firstName = "";
                    middleInit = "";
                    lastName = "";
                    // check if a comma in split == person name
                    var ind = firstSplit[i].indexOf(",");
                    if (ind !== null && ind !== undefined && ind >= 0) {

                        nameInd = i;
                        // split last name, first name middle initial
                        var secondSplit = firstSplit[i].split(re3);
                        if (secondSplit !== null && secondSplit !== undefined && secondSplit.length == 2) {

                            lastName = secondSplit[0].trim().toLowerCase();
                            // split first name middle initial
                            var thirdSplit = secondSplit[1].trim().split(" ");

                            if (thirdSplit !== null && thirdSplit !== undefined) {

                                // middle initial == string length of 1
                                if (thirdSplit[thirdSplit.length - 1].length == 1) {
                                    middleInit = thirdSplit[thirdSplit.length - 1].trim().toLowerCase();
                                }
                                else
                                    middleInit = null;

                                // everything minus the middle initial is the first name
                                var j = 0;
                                var thirdLen = thirdSplit.length;
                                if (middleInit !== null)
                                    thirdLen = thirdLen - 1;
                                for (j = 0; j < thirdLen; j++) {
                                    firstName = firstName + thirdSplit[j] + " ";
                                }
                                firstName = firstName.trim().toLowerCase();
                            }
                        }

                        break; // from for loop
                    } // if ind
                } // for firstSplit

                // Non-essential/less-essential parsing
                try {
                    codabar = firstSplit[nameInd - 1];
                    var re4 = new RegExp("[;=]");
                    var someNumArray = firstSplit[nameInd + 1].split(re4);
                    var k = 0;
                    for (k = 0; k < someNumArray.length; k++) {
                        if ((isNaN(someNumOne) || someNumOne <= -1) && someNumArray[k] !== null && someNumArray[k] !== undefined && someNumArray[k] !== "") {
                            someNumOne = parseInt(someNumArray[k], 10);
                        }
                        else if (someNumArray[k] !== null && someNumArray[k] !== undefined && someNumArray[k] !== "") {
                            someNumTwo = parseInt(someNumArray[k], 10);
                        }
                    }
                }
                catch (err) {

                }

                var uCard = {};
                try {
                    uCard = {
                        "firstName": firstName,
                        "lastName": lastName,
                        "middleInit": middleInit,
                        "studentId": studentId,
                        "libraryId": libraryId,
                        "codabar": codabar,
                        "someNumOne": someNumOne,
                        "someNumTwo": someNumTwo
                    };
                    globUCard = uCard;
                }
                catch (err) {
                    alert(err.message);
                }

                try {
                    return parseUCardComplete(uCard);
                }
                catch (err) {

                }

            } // if firstSplit
        } // if event.keyCode == 13
    }); // keydown
});
// Convert a state name or abbreviation into its corresponding index, according to the DB.
function stateToIndex(state) {
    state = state.replace(" ", "").trim().toLowerCase();
    if (state === "in")
        return 14;
    var states = {
        alabama: 1, al: 1,
        alaska: 2, ak: 2,
        arizona: 3, az: 3,
        arkansas: 4, ar: 4,
        california: 5, ca: 5,
        colorado: 6, co: 6,
        connecticut: 7, ct: 7,
        deleware: 8, de: 8,
        florida: 9, fl: 9,
        georgia: 10, ga: 10,
        hawaii: 11, hi: 11,
        idaho: 12, id: 12,
        illinois: 13, il: 13,
        indiana: 14,
        iowa: 15, iw: 15,
        kansas: 16, ks: 16,
        kentucky: 17, ky: 17,
        louisiana: 18, la: 18,
        maine: 19, me: 19,
        maryland: 20, ma: 20,
        massachusetts: 21, ma: 21,
        michigan: 22, mi: 22,
        minnesota: 23, mn: 23,
        mississippi: 24, ms: 24,
        missouri: 25, mo: 25,
        montana: 26, mt: 26,
        nebraska: 27, ne: 27,
        nevada: 28, nv: 28,
        newhampshire: 29, nh: 29,
        newjersey: 30, nj: 30,
        newmexico: 31, nm: 31,
        newyork: 32, ny: 32,
        northcarolina: 33, nc: 33,
        northdakota: 34, nd: 34,
        ohio: 35, oh: 35,
        oklahoma: 36, ok: 36,
        oregon: 37, or: 37,
        pennsylvania: 38, pa: 38,
        rhodeisland: 39, ri: 39,
        southcarolina: 40, sc: 40,
        southdakota: 41, sd: 41,
        tennessee: 42, tn: 42,
        texas: 43, tx: 43,
        utah: 44, ut: 44,
        vermont: 45, vt: 45,
        virginia: 46, va: 46,
        washington: 47, wa: 47,
        westvirginia: 48, wv: 48,
        wisconsin: 49, wi: 49,
        wyoming: 50, wy: 50,
        districtofcolumbia: 51, dc: 51,
        alberta: 52,
        britishcolombia: 53,
        manitoba: 54,
        newbrunswick: 55,
        newfoundland: 56,
        northwestterritories: 57,
        noviascotia: 58,
        nunavut: 59,
        ontario: 60,
        princeedwardisland: 61,
        quebec: 62,
        saskatchewan: 63,
        yukon: 64,
        other: 65,
    };
    var value = states[state];
    if(value === undefined || value === null)
        return 0;
    else
        return value;
}