
// Function to toggle play once the song is loaded
//function togglePlay() {
//    const audio = document.getElementById('audio-player');
//    const playPauseButton = document.getElementById('play-pause-button');
//    const playPauseIcon = playPauseButton.getElementsByTagName('i')[0];

//    if (audio.paused) {
//        audio.play();
//        playPauseIcon.className = 'fas fa-pause';
//    } else {
//        audio.pause();
//        playPauseIcon.className = 'fas fa-play';
//    }
//}

function playSong(songId) {
    // Make an AJAX request to get the song data
    var xhr = new XMLHttpRequest();
    xhr.open('GET', '/Search/Index?handler=PlaySong&id=' + songId, true);
    xhr.responseType = 'blob';
    xhr.onload = function (e) {
        if (this.status == 200) {
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
            playPauseIcon.className = 'fas fa-pause';
        }
    };
    xhr.send();
}

document.addEventListener("DOMContentLoaded", function () {
    // Get UI elements
    const audioPlayer = document.getElementById("audio-player");
    const playPauseButton = document.getElementById("play-pause-button");
    const volumeSlider = document.getElementById("volume-slider");
    const currentTime = document.getElementById("current-time");
    const fullLength = document.getElementById("full-length");
    const musicProgress = document.getElementById("music-progress");

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

//const audio = document.getElementById('audio-player');
//audio.addEventListener('ended', () => {
//    const playPauseButton = document.getElementById('play-pause-button');
//    const playPauseIcon = playPauseButton.getElementsByTagName('i')[0];
//    playPauseIcon.className = 'fas fa-play';
//});
