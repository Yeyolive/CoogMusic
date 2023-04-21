function goToNotificationsPage() {
    window.location.href = '/Notification';
}

$(document).ready(function () {
    $('#notificationBell').on('click', function () {
        goToNotificationsPage();
    });
});

