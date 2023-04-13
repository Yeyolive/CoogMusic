let queue = [];
let currentSongIndex = 0;
var currentSongID = null;
var currentArtistName = null;
var currentArtistId = null;

function playSongFromSearch(songId, songTitle, artistName) {
    const song = {
        songId: songId,
        title: songTitle,
        artistName: artistName
    };
    queue = [song]; // Set queue to contain only the current song
    currentSongIndex = 0;
    playSong(song);
}

function playSongFromPlaylist(playlistSongs, index) {
    queue = playlistSongs; // Update the queue with the entire playlist
    currentSongIndex = index;
    playSong(queue[currentSongIndex]);
}

function playSong(song) {
    const starsContainer = document.getElementById("stars-container");
    if (!song) {
        starsContainer.style.display = "none";
        return;
    }
    starsContainer.style.display = "block";

    currentSongID = song.songId;
    //console.log(song);


    // Make an AJAX request to get the song data
    document.getElementById("song-title").innerText = capitalizeFirstLetter(song.title.toLowerCase());
    document.getElementById("artist-name").innerText = capitalizeFirstLetter(song.artistName.toLowerCase());
    var xhr = new XMLHttpRequest();
    xhr.open('GET', '/Search/Index?handler=PlaySong&id=' + song.songId, true);
    xhr.responseType = 'blob';
    xhr.onload = function (e) {
        if (this.status == 200) {
            // Get Artist ID from response header
            currentArtistId = parseInt(this.getResponseHeader("artist-id"));

            // Create a blob URL for the audio data
            var blob = new Blob([this.response], { type: 'audio/mpeg' });
            var url = URL.createObjectURL(blob);

            // Update the audio player to play the new audio file
            var audio = document.getElementById('audio-player');
            audio.src = url;
            audio.play();

            // Update the play-pause button icon to the pause icon
            const playPauseButton = document.getElementById('play-pause-button');
            const playPauseIcon = playPauseButton.getElementsByTagName('i')[0];
            playPauseButton.innerHTML = '<i class="fas fa-pause"></i>';
        }
    };
    xhr.send();
}

function playNext() {
    if (currentSongIndex < queue.length - 1) {
        currentSongIndex++;
        playSong(queue[currentSongIndex]);
    }
}

function playPrevious() {
    if (currentSongIndex > 0) {
        currentSongIndex--;
        playSong(queue[currentSongIndex]);
    }
}

function capitalizeFirstLetter(string) {
    return string.split(' ').map(function (word) {
        return word.charAt(0).toUpperCase() + word.slice(1);
    }).join(' ');
}

document.addEventListener("DOMContentLoaded", function () {
    // Get UI elements
    const audioPlayer = document.getElementById("audio-player");
    const playPauseButton = document.getElementById("play-pause-button");
    const volumeSlider = document.getElementById("volume-slider");
    const currentTime = document.getElementById("current-time");
    const fullLength = document.getElementById("full-length");
    const musicProgress = document.getElementById("music-progress");

    const skipButton = document.getElementById("skip-button");
    const prevButton = document.getElementById("prev-button");

    skipButton.addEventListener("click", function () {
        playNext();
    });

    prevButton.addEventListener("click", function () {
        playPrevious();
    });

    // Play/pause functionality
    playPauseButton.addEventListener("click", function () {
        if (audioPlayer.paused) {
            audioPlayer.play();
            playPauseButton.innerHTML = '<i class="fas fa-pause"></i>';
        } else {
            audioPlayer.pause();
            playPauseButton.innerHTML = '<i class="fas fa-play"></i>';
        }
    });

    // Update volume
    volumeSlider.addEventListener("input", function () {
        audioPlayer.volume = volumeSlider.value;
    });

    // Update current time and progress bar
    audioPlayer.addEventListener("timeupdate", function () {
        currentTime.textContent = formatTime(audioPlayer.currentTime);
        fullLength.textContent = formatTime(audioPlayer.duration);
        if (isFinite(audioPlayer.currentTime) && isFinite(audioPlayer.duration))
            musicProgress.value = (audioPlayer.currentTime / audioPlayer.duration) * 100;
    });

    // Seek functionality
    musicProgress.addEventListener("input", function () {
        audioPlayer.currentTime = (musicProgress.value / 100) * audioPlayer.duration;
    });

    // Format time in minutes and seconds (MM:SS)
    function formatTime(seconds) {
        const minutes = Math.floor(seconds / 60);
        const remainingSeconds = Math.floor(seconds % 60);
        return minutes.toString().padStart(2, "0") + ":" + remainingSeconds.toString().padStart(2, "0");
    }
});

function selectStar(star) {
    var stars = document.querySelectorAll('.star');
    var rating = star.dataset.rating;

    for (var i = 0; i < stars.length; i++) {
        if (i <= rating - 1) {
            stars[i].classList.remove('fa-regular');
            stars[i].classList.remove('fa-star');
            stars[i].classList.remove('fa-sm');
            stars[i].classList.remove('star');
            stars[i].classList.add('fa-solid');
            stars[i].classList.add('fa-star');
            stars[i].classList.add('fa-sm');
        } else {
            stars[i].classList.remove('fa-solid');
            stars[i].classList.remove('fa-star');
            stars[i].classList.remove('fa-sm');
            stars[i].classList.add('fa-regular');
            stars[i].classList.add('fa-star');
            stars[i].classList.add('fa-sm');
            stars[i].classList.add('star');
        }
    }
    updateRating(currentSongID, rating, currentArtistId);
}


function updateRating(songID, rating, artistId) {
    // Make an AJAX request to update the rating in database
    var xhr = new XMLHttpRequest();
    xhr.open('POST', '/Search/Index?handler=UpdateRating', true);
    xhr.setRequestHeader("Content-type", "application/x-www-form-urlencoded");
    xhr.setRequestHeader("RequestVerificationToken", document.getElementsByName('__RequestVerificationToken')[0].value);

    xhr.onload = function () {
        if (this.status == 200) { 
            var jsonResponse = JSON.parse(this.responseText);
            if (jsonResponse.success) {
                console.log(jsonResponse.message);
            } else {
                console.error(jsonResponse.message);
            }
        }
        else {
            console.error("Error updating rating");
        }
    };

    xhr.send(`songID=${songID}&rating=${rating}&ArtistId=${artistId}`);
}

document.getElementById("stars-container").style.display = "none";

const followButton = document.querySelector('.followButton');

followButton.addEventListener('click', () => {
    // Code to follow the artist goes here
    var xhr = new XMLHttpRequest();
    var artistId = currentArtistId;

    //console.log(artistId);
    xhr.open('POST', '/Search/Index?handler=FollowArtist', true);
    xhr.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");
    xhr.setRequestHeader("RequestVerificationToken", document.getElementsByName('__RequestVerificationToken')[0].value);

    xhr.onreadystatechange = () => {
        if (xhr.readyState === XMLHttpRequest.DONE) {
            if (xhr.status === 200) {
                console.log('Artist followed successfully!');
            } else {
                console.error('An error occurred while following the artist.');
            }
        }
    };
    //xhr.send(JSON.stringify({ artistName: currentArtistName }));
    xhr.send(`ArtistId=${artistId}`);
});
