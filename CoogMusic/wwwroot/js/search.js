function addToPlaylist(songId) {
    // Show the modal
    var modal = document.getElementById("playlist-modal");
    modal.style.display = "block";

    // Load user's playlists and display them
    $.get("/Search?handler=UserPlaylists", function (data) {
            var container = document.getElementById("playlists-container");
            container.innerHTML = '';
            data.forEach(playlist => {
                console.log('Playlist:', playlist);
                console.log(songId);

                //var button = document.createElement('button');
                //button.classList.add('btn', 'btn-primary', 'btn-action');
                //button.textContent = playlist.Title;
                //button.onclick = function () {

                var listItem = document.createElement('li');
                listItem.textContent = playlist.title;
                listItem.style.cursor = 'pointer';
                listItem.onclick = function () {
                    // Add the song to the selected playlist
                    fetch('/Search?handler=AddToPlaylist', {
                        method: 'POST',
                        headers: {
                            'Content-Type': 'application/x-www-form-urlencoded',
                            'RequestVerificationToken': document.getElementsByName('__RequestVerificationToken')[0].value
                        },
                        body: `PlaylistSongData.PlaylistId=${playlist.id}&PlaylistSongData.SongId=${songId}`
                    })
                        .then(response => response.json())
                        .then(data => {
                            var errorMessageElement = document.getElementById("error-message");
                            var successMessageElement = document.getElementById("success-message");

                            if (data.success) {
                                errorMessageElement.innerHTML = '';
                                errorMessageElement.classList.remove('alert', 'alert-warning', 'alert-dismissible', 'fade', 'show');

                                successMessageElement.innerHTML = '<strong>Song added to the playlist!</strong><button type="button" class="btn-close" data-bs-dismiss="alert"></button>';
                                successMessageElement.classList.add('alert', 'alert-success', 'alert-dismissible', 'fade', 'show');
                            } else {
                                errorMessageElement.innerHTML = '<strong>Failed to add the song to the playlist: ' + data.message + '</strong><button type="button" class="btn-close" data-bs-dismiss="alert"></button>';
                                errorMessageElement.classList.add('alert', 'alert-warning', 'alert-dismissible', 'fade', 'show');

                                successMessageElement.innerHTML = '';
                                successMessageElement.classList.remove('alert', 'alert-success', 'alert-dismissible', 'fade', 'show');
                            }
                        });


                    // Close the modal
                    modal.style.display = "none";
                };
                //container.appendChild(button);
                container.appendChild(listItem);
            });
        });
}

// Add the modal script
var modal = document.getElementById("playlist-modal");
var span = document.getElementsByClassName("close")[0];
span.onclick = function () {
    modal.style.display = "none";
};
window.onclick = function (event) {
    if (event.target == modal) {
        modal.style.display = "none";
    }
};