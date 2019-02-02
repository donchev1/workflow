
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

    $('a[name="disabledPickUp"]').click(function () {
        alert("The order is not ready for collection!");
    });










