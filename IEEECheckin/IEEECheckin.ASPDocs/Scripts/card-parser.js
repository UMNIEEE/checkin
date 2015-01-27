
function parseUMN(cardRaw, regEx, groupInds) {
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
            "middleInit": "",
            "studentId": "",
            "email": ""
        };

        // parse regex for data at indices as provided by user
        var datas = ["firstName", "lastName", "middleInit", "studentId", "email"];
        for (index = 0; index < datas.length; index++) {
            var indexName = datas[index];
            if (checkStr(groupInds[indexName])) {
                var ind = parseInt(groupInds[indexName]);
                if (!isNaN(ind) && ind >= 1 && ind < (m.length - 1)) {
                    if(checkStr(m[ind]))
                        entryInfo[indexName] = m[ind].trim().toLowerCase();
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