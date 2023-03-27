const songInput = document.getElementById('song-name');
const addSongBtn = document.getElementById('add-song-btn');
const artistInput = document.getElementById('artist-name');
const includeAllSongsCheckbox = document.querySelector('input[name="include-all-songs"]');
const addArtistBtn = document.getElementById('add-artist-btn');
const submitBtn = document.getElementById('submit-btn');
const generateReportBtn = document.getElementById('generate-report-btn');

addSongBtn.addEventListener('click', function () {
    // Add a new input field for the song
    const newSongInput = document.createElement('input');
    newSongInput.type = 'text';
    newSongInput.name = 'song-name';
    newSongInput.placeholder = 'Enter song name';
    songInput.parentNode.insertBefore(newSongInput, addSongBtn);
});

addArtistBtn.addEventListener('click', function () {
    // Add a new input field for the artist
    const newArtistInput = document.createElement('input');
    newArtistInput.type = 'text';
    newArtistInput.name = 'artist-name';
    newArtistInput.placeholder = 'Enter artist name';
    artistInput.parentNode.insertBefore(newArtistInput, addArtistBtn);
});

submitBtn.addEventListener('click', function (event) {
    event.preventDefault();
    // Retrieve the values from the input fields
    const songNames = Array.from(document.querySelectorAll('input[name="song-name"]')).map(input => input.value);
    const artistNames = Array.from(document.querySelectorAll('input[name="artist-name"]')).map(input => input.value);
    const includeAllSongs = includeAllSongsCheckbox.checked;
    // Do something with the data, like submit it to a server using AJAX

});

generateReportBtn.addEventListener('click', function () {
    // Generate a streaming report

});