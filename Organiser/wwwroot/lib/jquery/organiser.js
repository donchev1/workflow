
var basicUrl = "http://localhost:50871/"

$(document).ready(function () {
    var showMessages = $("#showMessages");
    var hideMessages = $("#hideMessages");
    var hasNewMessages = false;
    showMessages.click(showMessagesFunc);

    hideMessages.click(hideMessagesFunc);
});

function showMessagesFunc() {
    var newMessages = $("div[class$='newMessages'");
    var bumbam = $(newMessages[0]).css("display");
    var bumbam1 = $(newMessages[1]).css("display");

    hasNewMessages = (bumbam != "none" || bumbam1 != "none");
    var messageBox = $(".messageBox");

    if (hasNewMessages) {
        updateMonitor();
    }
    else {
        messageBox.slideToggle();
        showMessages.toggle();
        hideMessages.toggle();
    }
}

function hideMessagesFunc() {
    debugger;

    var newMessages = $("div[class$='newMessages'");
    var bumbam = $(newMessages[0]).css("display");
    var bumbam1 = $(newMessages[1]).css("display");

    hasNewMessages = (bumbam != "none" || bumbam1 != "none");
    var messageBox = $(".messageBox");

    if (hasNewMessages) {
        updateMonitor();
    }
    else {
        messageBox.slideToggle();
        showMessages.toggle();
        hideMessages.toggle();
    }
}



function DostuffWithButton() {
    $.ajax({
        url: "http://localhost:58962/order/Receive1?id=3",
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert("Request: " + XMLHttpRequest.statusText + "\n\nStatus: " + textStatus + "\n\nError: " + errorThrown);
        },
        success: function (result) {
            $('#dingdong').html(result);
        }
    });
}

    //$('td[class="location"]').click(function () {
    //    $((this).closest('tr')).next().slideToggle();
    //});

    $('a[name="disabledPickUp"]').click(function () {
        alert("The order is not ready for collection!");
    });










