// Write your JavaScript code.

//Source: https://stackoverflow.com/questions/1484506/random-color-generator
function rainbow(numOfSteps, step) {
    // This function generates vibrant, "evenly spaced" colours (i.e. no clustering). This is ideal for creating easily distinguishable vibrant markers in Google Maps and other apps.
    // Adam Cole, 2011-Sept-14
    // HSV to RBG adapted from: http://mjijackson.com/2008/02/rgb-to-hsl-and-rgb-to-hsv-color-model-conversion-algorithms-in-javascript
    var r, g, b;
    var h = step / numOfSteps;
    var i = ~~(h * 6);
    var f = h * 6 - i;
    var q = 1 - f;
    switch (i % 6) {
        case 0: r = 1; g = f; b = 0; break;
        case 1: r = q; g = 1; b = 0; break;
        case 2: r = 0; g = 1; b = f; break;
        case 3: r = 0; g = q; b = 1; break;
        case 4: r = f; g = 0; b = 1; break;
        case 5: r = 1; g = 0; b = q; break;
    }
    var c = "#" + ("00" + (~ ~(r * 255)).toString(16)).slice(-2) + ("00" + (~ ~(g * 255)).toString(16)).slice(-2) + ("00" + (~ ~(b * 255)).toString(16)).slice(-2);
    return (c);
}

if (!Array.prototype.lastItem) {
    Array.prototype.lastItem = function () {
        return this[this.length - 1];
    };
};

if (!Array.prototype.sum) {
    Array.prototype.sum = function () {
        return this.reduce((a, b) => a + b, 0);
    };
};

function round(number, numOfDigits) {
    var coef = Math.pow(10, numOfDigits);
    return (Math.round(number * coef) / coef).toFixed(numOfDigits);
}

function getDateFromTimestamp(unix_timestamp) {
    // Create a new JavaScript Date object based on the timestamp
    // multiplied by 1000 so that the argument is in milliseconds, not seconds.
    var date = new Date(unix_timestamp * 1000);
    var day = "0" + date.getDay();
    var month = "0" + (date.getMonth() + 1);
    var year = "0" + date.getFullYear();

    // Will display time in 10:30:23 format
    return day.substr(-2) + '.' + month.substr(-2) + '.' + year.substr(-4);
}

function makePost(route, dataObject, isAsync, onSuccess, onError) {
    $.ajax({
        type: "POST",
        url: route,
        data: dataObject,
        dataType: "text",
        async: isAsync,
        success: function (data, textStatus, jqXHR) {
            if (onSuccess) {
                onSuccess(data, jqXHR);
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            if (onError) {
                onError();
            }
        }
    });
}

function makeRequest(route, requestType, onLoaded, onError) {
    var xhr = new XMLHttpRequest();
    xhr.onreadystatechange = function () {
        if (this.readyState == 4 && this.status == 200) {
            onLoaded(this.responseText);
        }
        else if (this.readyState == 4) {
            if (onError) {
                onError();
            }
        }
    };
    xhr.open(requestType, route);
    xhr.send();
}

function removeDisplayNone(elementsClass, doRemove) {
    if (doRemove === true) {
        $("." + elementsClass).removeClass("d-none");
    }
}

function setDisplayNone(elementsClass, doSet) {
    if (doSet === true) {
        $("." + elementsClass).addClass("d-none");
    }
}
