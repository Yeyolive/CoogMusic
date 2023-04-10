function addToPlaylist(songId) {
    var playlistId = ...; // Fetch the playlist ID for the current user from the server
    var url = "/AddToPlaylist?songId=" + songId + "&playlistId=" + playlistId;
    fetch(url, { method: "POST" })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                alert("Song added to the playlist!");
            } else {
                alert("Error adding song to the playlist.");
            }
        })
        .catch(error => {
            console.error("Error:", error);
        });
}
