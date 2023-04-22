function goToNotificationsPage() {
    console.log("go to notif page ");
    window.location.href = '/Notification';

    displayNotifications(userId);
}

$(document).ready(function () {
    $('#notificationBell').on('click', function () {
        goToNotificationsPage();
    });
});

function displayNotifications(userId) {
    console.log("displayNotifications function called with userId = " + userId);
    // Make an AJAX call to get the notifications from the server
    var xhr = new XMLHttpRequest();
    xhr.open("GET", "/Notification/Index?handler=DisplayNotifications&userId=" + userId, true);
    xhr.onload = function () {
        if (this.status === 200) {
            var data = JSON.parse(this.responseText);

            data.forEach(function (notification) {
                var notificationMessage = new NotificationMessage(notification.Message);
                notificationMessage.display();
                console.log(notificationMessage);
            });
        } else if (this.status !== 200) {
            console.log("Error getting notifications.");
        }
    };
    xhr.send();
}

class NotificationMessage {
    constructor(message) {
        this.message = message;
    }

    display() {
        var messageElement = $("<div>").addClass("notification-message");
        var contentElement = $("<p>").text(this.message);

        messageElement.append(contentElement);
        $("#notifications-container").append(messageElement);
    }
}