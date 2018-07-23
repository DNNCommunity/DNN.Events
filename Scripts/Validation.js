//
// DotNetNuke® - http://www.dnnsoftware.com
// Copyright (c) 2002-2008
// by DNNCorp
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
// the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
// to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions 
// of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.
//
// Events module 4.x
//

// shortcuts

/*global document: true, alert: true, setTimeout: true, dnn: true */

function byid(eID) {
    if (document.getElementById) {
        return document.getElementById(eID);
    } else if (document.all) {
        return document.all(eID);
    } else {
        return null;
    }
}

function selIndex(eID) {
    return byid(eID).options[byid(eID).selectedIndex].value;
}

function setselIndex(eID, v) {
    for (var i = 0; i < byid(eID).options.length; i++) {
        if (byid(eID).options[i].value == v) {
            byid(eID).options[i].selected = true;
            return;
        }
    }
}

// Validation functions
function timeoutFunc(fid) {
    byid(fid).focus();
    byid(fid).select();
}

function SetupRangeValidator(valfieldid, valfieldid2, valGroup, botno, topno, message) {
    var valField = byid(valfieldid);
    var valField2 = byid(valfieldid2);
    valField.maximumvalue = "" + topno + "";
    valField.minimumvalue = "" + botno + "";
    valField.errormessage = message;
    valField.innerHTML = message;
    valField2.errormessage = message;
    valField2.innerHTML = message;
    window.Page_ClientValidate(valGroup);
}

function valRemTime(valTime, valTime2, valGroup, remMeasurement, errorminutes, errorhours, errordays) {
    if (!errorhours) {
        errorhours = errorminutes;
    }
    if (!errordays) {
        errordays = errorminutes;
    }
    switch (selIndex(remMeasurement)) {
    case "m":
        SetupRangeValidator(valTime, valTime2, valGroup, 15, 60, errorminutes);
        break;
    case "h":
        SetupRangeValidator(valTime, valTime2, valGroup, 1, 24, errorhours);
        break;
    case "d":
        SetupRangeValidator(valTime, valTime2, valGroup, 1, 30, errordays);
        break;
    }
}


// functions used in EditEvents
function CopyField() {
    var sourceField;
    var targetField;
    sourceField = window.$find(arguments[0]);
    targetField = window.$find(arguments[1]);
    if (!isNaN(sourceField.get_selectedDate())) {
        if (sourceField.get_selectedDate() > targetField.get_selectedDate()) {
            targetField.set_selectedDate(sourceField.get_selectedDate());
        }
    } else {
        targetField.set_selectedDate(sourceField.get_selectedDate());
    }
}

function SetComboIndex() {
    var tpStartTime = window.$find(arguments[0]);
    var tpEndTime = window.$find(arguments[1]);
    var dpStartDate = window.$find(arguments[2]);
    var dpEndDate = window.$find(arguments[3]);
    var timeInterval = arguments[4];
    var tiStartTime = tpStartTime.get_timeView().getTime();
    var tiEndTime = tpEndTime.get_timeView().getTime();
    var dtStartDate = dpStartDate.get_selectedDate()._toFormattedString();
    var dtEndDate = dpEndDate.get_selectedDate()._toFormattedString();
    if (dtStartDate == dtEndDate && tiStartTime > tiEndTime) {
        var tvEndTime = tpEndTime.get_timeView();
        tvEndTime.setTime(tiStartTime.getHours(),
            tiStartTime.getMinutes() + parseInt(timeInterval),
            tiStartTime.getSeconds(),
            tiEndTime);
        dpEndDate.set_selectedDate(tiEndTime);
    }
}

function showTbl(chkField, tblDetail) {
    if (window.dnn.dom.getById(chkField).checked === true) {
        window.dnn.dom.getById(tblDetail).style.display = "block";
    } else {
        window.dnn.dom.getById(tblDetail).style.display = "none";
    }
}

function showTblSpecified(strDisplayType, chkField, tblDetail) {
    if (window.dnn.dom.getById(chkField).checked === true) {
        window.dnn.dom.getById(tblDetail).style.display = strDisplayType;
    } else {
        window.dnn.dom.getById(tblDetail).style.display = "none";
    }
}

function showhideTbls(strDisplayType,
    chkField1,
    tblDetail1,
    chkField2,
    tblDetail2,
    chkField3,
    tblDetail3,
    chkField4,
    tblDetail4,
    chkField5,
    tblDetail5) {
    showTblSpecified("block", chkField1, tblDetail1);
    showTblSpecified(strDisplayType, chkField2, tblDetail2);
    showTblSpecified(strDisplayType, chkField3, tblDetail3);
    showTblSpecified(strDisplayType, chkField4, tblDetail4);
    showTblSpecified(strDisplayType, chkField5, tblDetail5);
}

function showhideChk2(chkField1, tblDetail1, chkField2, tblDetail2) {
    showTbl(chkField1, tblDetail1);
    if (window.dnn.dom.getById(chkField1).checked === true) {
        window.dnn.dom.getById(tblDetail2).style.display = "block";
    } else if (window.dnn.dom.getById(chkField2).checked === true) {
        window.dnn.dom.getById(tblDetail2).style.display = "block";
    } else {
        window.dnn.dom.getById(tblDetail2).style.display = "none";
    }
}

function showTimes(chkField, cmbField1, cmbField2) {
    if (window.dnn.dom.getById(chkField).checked === true) {
        window.dnn.dom.getById(cmbField1).style.display = "none";
        window.dnn.dom.getById(cmbField2).style.display = "none";
    } else {
        window.dnn.dom.getById(cmbField1).style.display = "";
        window.dnn.dom.getById(cmbField2).style.display = "";
    }
}

// Functions used in Event Settings
function disableactivate(defaultview, ctlMonth, ctlWeek, ctlList) {

    byid(ctlMonth).disabled = false;
    byid(ctlWeek).disabled = false;
    byid(ctlList).disabled = false;

    switch (selIndex(defaultview)) {
    case "EventMonth.ascx":
        byid(ctlMonth).disabled = true;
        byid(ctlMonth).checked = true;
        break;
    case "EventWeek.ascx":
        byid(ctlWeek).disabled = true;
        byid(ctlWeek).checked = true;
        break;
    case "EventList.ascx":
        byid(ctlList).disabled = true;
        byid(ctlList).checked = true;
        break;
    }
}

function disableControl(sourceID, state, destID) {
    if (byid(sourceID).checked == state) {
        byid(destID).disabled = true;
    } else {
        byid(destID).disabled = false;
    }
}

function CheckBoxFalse(sourceID, state, destID) {
    if (byid(sourceID).checked == state) {
        byid(destID).checked = false;
    }
    disableControl(sourceID, state, destID);
}

function GetRadioButtonValue(id) {
    var rb = byid(id);
    var radio = rb.getElementsByTagName("input");
    for (var j = 0; j < radio.length; j++) {
        if (radio[j].checked) {
            return radio[j].value;
        }
    }
// ReSharper disable NotAllPathsReturnValue
}
// ReSharper restore NotAllPathsReturnValue

function disableRbl(sourceID, state, destID) {
    var rbValue = GetRadioButtonValue(sourceID);
    if (rbValue == state) {
        byid(destID).disabled = false;
    } else {
        byid(destID).disabled = true;
    }
}

function disableDDL(sourceID, state, destID) {
    var ddl = byid(sourceID);
    var ddlValue = ddl.options[ddl.selectedIndex].value;
    if (ddlValue == state) {
        byid(destID).disabled = false;
    } else {
        byid(destID).disabled = true;
    }
}

function disablelistsettings(sourceID, state, field1, field2, field3, field4) {
    if (byid(sourceID).checked == state) {
        byid(field1).disabled = false;
        byid(field2).disabled = false;
        byid(field3).disabled = true;
        byid(field4).disabled = true;
    } else {
        byid(field1).disabled = true;
        byid(field2).disabled = true;
        byid(field3).disabled = false;
        byid(field4).disabled = false;
    }
}

// Copy the content of the startdate control to the enddate control
function CopyStartDateToEnddate(startdate, enddate, starttime, endtime, chkField) {
    window.$find(enddate).set_selectedDate(window.$find(startdate).get_selectedDate());
    if (window.dnn.dom.getById(chkField).checked !== true) {
        var tpStartTime = window.$find(starttime);
        var tpEndTime = window.$find(endtime);
        var tvStartTime = tpStartTime.get_timeView();
        var tvEndTime = tpEndTime.get_timeView();
        var tiStartTime = tvStartTime.getTime();
        var tiEndTime = tvEndTime.getTime();
        tvEndTime.setTime(tiStartTime.getHours(), tiStartTime.getMinutes(), tiStartTime.getSeconds(), tiEndTime);
    }
}

// Limit characters to be entered in a field with a message
function limitText(limitField, limitNum, message) {
    if (limitField.value.length > limitNum) {
        limitField.value = limitField.value.substring(0, limitNum);
        alert(message + " " + limitNum);
    }
}

// functions used in Edit Categories
function GetColor(valColor) {
    if (valColor.length === 0 || valColor.substring(0, 1) === "#") {
        return valColor;
    } else {
        return "#" + valColor;
    }
}

// ReSharper disable UnusedParameter
// ReSharper disable UseOfImplicitGlobalInFunctionScope
function ValidateColor(source, arguments) {
    var txtColor = GetColor(arguments.Value);
    if (txtColor.length === 0) {
        arguments.IsValid = true;
    }

    var regColorcode = /^(#)?([0-9a-fA-F]{3})([0-9a-fA-F]{3})?$/;
    arguments.IsValid = regColorcode.test(txtColor);
}
// ReSharper restore UnusedParameter
// ReSharper restore UseOfImplicitGlobalInFunctionScope

function CategoryPreviewPane(colPickerBack,
    colPickerFore,
    previewpane,
    lblPreviewCat,
    catForeColor,
    catBackColor,
    catName,
    catError) {

    var newForeColor = GetColor(byid(catForeColor).value);
    var newBackColor = GetColor(byid(catBackColor).value);

    if (window.Page_IsValid) {
        if (newForeColor != "") {
            byid(catForeColor).value = newForeColor;
            byid(lblPreviewCat).style.color = newForeColor;
            window.$find(colPickerFore).set_selectedColor(newForeColor);
        }
        if (newBackColor != "") {
            byid(catBackColor).value = newBackColor;
            byid(previewpane).style.backgroundColor = newBackColor;
            window.$find(colPickerBack).set_selectedColor(newBackColor);
        }
        byid(lblPreviewCat).innerHTML = byid(catName).value;
    } else {
        byid(lblPreviewCat).innerHTML = catError;
        byid(lblPreviewCat).style.color = "#ffffff";
        byid(previewpane).style.backgroundColor = "#ed1c24";
    }
}

// ReSharper disable UnusedParameter
// ReSharper disable UseOfImplicitGlobalInFunctionScope
function ValidateTime(source, arguments) {
    var timeInterval = parseInt(source.TimeInterval);
    var errorMessage = source.ErrorMessage;
    var tiEndTime = window.$find(source.ClientID).get_timeView().getTime();
    var totalMins = (tiEndTime.getHours() * 60) + tiEndTime.getMinutes();
    var remainder = totalMins % timeInterval;
    if (remainder != 0) {
        source.innerHTML = errorMessage;
        source.errormessage = errorMessage;
        arguments.IsValid = false;
    }
}
// ReSharper restore UseOfImplicitGlobalInFunctionScope
// ReSharper restore UnusedParameter

function btnUpdateClick(btnUpdateUniqueID, ddlCategoriesClientID) {
    var ddlCategories = window.$find(ddlCategoriesClientID);
    if (ddlCatText != ddlCategories.get_text()) {
        window.__doPostBack(btnUpdateUniqueID, "");
    }
}

var ddlCatText = "";

function storeText(ddlCategoriesClientID) {
    var ddlCategories = window.$find(ddlCategoriesClientID);
    ddlCatText = ddlCategories.get_text();
}