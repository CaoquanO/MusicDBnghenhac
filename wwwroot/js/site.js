const audio = document.getElementById("audioPlayer");
const volumeSlider = document.getElementById("volumeSlider");

volumeSlider.addEventListener("input", () => {
    audio.volume = volumeSlider.value;
});

