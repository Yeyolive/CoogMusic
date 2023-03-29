//const uploadTypeSelect = document.getElementById('uploadType');
//const albumDetailsDiv = document.getElementById('album-details');
//const songDetailsDiv = document.getElementById('song-details');
//const songFileInput = document.getElementById('file');

//uploadTypeSelect.addEventListener('change', () => {
//    if (uploadTypeSelect.value === 'album') {
//        albumDetailsDiv.style.display = 'block';
//        songDetailsDiv.style.display = 'none';
//        songFileInput.setAttribute('multiple', '');
//    } else {
//        albumDetailsDiv.style.display = 'none';
//        songDetailsDiv.style.display = 'block';
//        songFileInput.removeAttribute('multiple');
//    }
//});

//function handleFileSelect(event) {
//    const fileInput = event.target;
//    const preview = document.getElementById('preview');
//    const file = fileInput.files[0];
//    const reader = new FileReader();

//    reader.onloadend = function () {
//        preview.src = reader.result;
//    }

//    if (file) {
//        reader.readAsDataURL(file);
//    } else {
//        preview.src = "";
//    }
//}

//// function handleSubmit(event) {
////   event.preventDefault();
////   const form = event.target;
////   const formData = new FormData(form);

////   if (uploadTypeSelect.value === 'album') {
////     // Handle multiple files for album upload
////     const fileInputs = document.querySelectorAll('#file[type="file"]');
////     formData.delete('file');
////     for (const fileInput of fileInputs) {
////       for (const file of fileInput.files) {
////         formData.append('file', file);
////       }
////     }
////   }

////   fetch(form.action, {
////     method: 'POST',
////     body: formData
////   })
////     .then(response => response.json())
////     .then(data => {
////       alert(`Song title: ${data.title}\nArtist name: ${data.artist}\nGenre: ${data.genre}\nFile name: ${data.fileName}`);
////       form.reset();
////       document.getElementById('preview').src = "";
////     })
////     .catch(error => {
////       console.error(error);
////     });
//// }


//songFileInput.addEventListener('change', handleFileSelect);
//document.getElementById('upload-form').addEventListener('submit', handleSubmit);
