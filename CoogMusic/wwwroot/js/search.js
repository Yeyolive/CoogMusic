function addToPlaylist(songId, songTitle, artistName) {
    // Show the modal
    var modal = document.getElementById("playlist-modal");
    modal.style.display = "block";

    // Load user's playlists and display them
    $.get("/Search?handler=UserPlaylists", function (data) {
            var container = document.getElementById("playlists-container");
            container.innerHTML = '';
            data.forEach(playlist => {
                console.log('Playlist:', playlist);

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
                            'Content-Type': 'application/json',
                            'RequestVerificationToken': document.getElementsByName('__RequestVerificationToken')[0].value
                        },
                        body: JSON.stringify({ playlistId: playlist.id, songId: songId, songTitle: songTitle, artistName: artistName })
                    })
                        .then(response => {
                            if (response.ok) {
                                alert('Song added to the playlist!');
                            } else {
                                alert('Failed to add the song to the playlist.');
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