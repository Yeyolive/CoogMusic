function goToNotificationsPage() {
    console.log("go to notif page ");
    window.location.href = '/Notification';
}

$(document).ready(function () {
    $('#notificationBell').on('click', function () {
        goToNotificationsPage();
    });
});

function displayNotifications(userId) {
    console.log("displayNotifications function called with userId = " + userId);
    // Make an AJAX call to get the notifications from the server
    $.ajax({
        type: "GET",
        url: "/Notification/" + userId,
        dataType: "json",
        success: function (data) {
            // Loop through the notifications and create a new HTML element for each one
            data.forEach(function (notification) {
                var messageElement = $("<div>").addClass("notification-message");
                var contentElement = $("<p>").text(notification.Message);

                messageElement.append(headingElement);
                messageElement.append(contentElement);
                $("#notifications-container").append(messageElement);
            });
        },
        error: function () {
            console.log("Error getting notifications.");
        }
    });
}/*

var mockData = [{ content: "Notification message 1" }, { content: "Notification message 2" }, { content: "Notification message 3" }];

function displayNotifications(userId) {
    console.log("displayNotifications function called with userId = " + userId);
    var mockData = [
        { Message: "fart" },
        { Message: "Notification message 2" },
        { Message: "Notification message 3" }
    ];

    // Loop through the notifications and create a new HTML element for each one
    mockData.forEach(function (notification) {
        var messageElement = $("<div>").addClass("notification-message");
        var contentElement = $("<p>").text(notification.Message);

        messageElement.append(contentElement);
        $("#notifications-container").append(messageElement);
    });
}
*/