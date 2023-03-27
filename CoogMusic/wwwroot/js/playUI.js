var audioPlayer = document.getElementById("audio-player");
var playPauseButton = document.getElementById("play-pause-button");
var skipButton = document.getElementById("skip-button");
var backButton = document.getElementById("prev-button");
var songName = document.getElementById("song-name");
songName.innerHTML = "Play Something";

// Add event listener to the play/pause button
playPauseButton.addEventListener("click", function () {
    if (audioPlayer.paused) {
        audioPlayer.play();
        playPauseButton.innerHTML = '<i class="fas fa-pause"></i>'; // Change button icon to pause
        playPauseButton.classList.add("playing"); // Add "playing" class
    } else {
        audioPlayer.pause();
        playPauseButton.innerHTML = '<i class="fas fa-play"></i>'; // Change button icon to play
        playPauseButton.classList.remove("playing"); // Remove "playing" class
    }
});

// Update the song name when a new song is loaded
audioPlayer.addEventListener('loadedmetadata', function () {
    // Get the title of the song from the metadata, or use the filename if no title is available
    const filename = audioPlayer.currentSrc.split("/").pop();
    const songTitle = filename.split("_").join(" ").split(".")[0].replace(/-/g, " by ");;
    songName.innerHTML = songTitle; // Set the music title element to the title of the song
});

// Add event listener to the skip button
skipButton.addEventListener("click", function () {
    audioPlayer.currentTime += 10; // Skip 10 seconds
});
// Add event listener to the go back button
backButton.addEventListener("click", function () {
    audioPlayer.currentTime -= 10; // Go back 10 seconds
});

const musicProgress = document.getElementById('music-progress');

audioPlayer.addEventListener('timeupdate', () => {
    const currentTime = audioPlayer.currentTime;
    const duration = audioPlayer.duration;
    const progress = (currentTime / duration) * 100;
    musicProgress.value = progress;
});

// Get the current time element
var currentTimeElement = document.getElementById("current-time");

// Update the current time display
function updateCurrentTimeDisplay() {
    var currentTime = audioPlayer.currentTime;
    var minutes = Math.floor(currentTime / 60);
    var seconds = Math.floor(currentTime % 60);
    currentTimeElement.innerHTML = minutes + ":" + (seconds < 10 ? "0" : "") + seconds;
}
// Add event listener to the audio player to update the current time display
audioPlayer.addEventListener("timeupdate", function () {
    updateCurrentTimeDisplay();
});
// Call the updateCurrentTimeDisplay function on page load
updateCurrentTimeDisplay();

const audio = document.getElementById('audio-element');
const volumeSlider = document.getElementById('volume-slider');

volumeSlider.addEventListener('input', () => {
    audioPlayer.volume = volumeSlider.value;
});