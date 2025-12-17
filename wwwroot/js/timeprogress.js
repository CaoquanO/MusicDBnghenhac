document.addEventListener("DOMContentLoaded", () => {

    const audio = document.getElementById("audioPlayer");

    // Nút điều khiển
    const playBtn = document.getElementById("playPauseBtn");
    const nextBtn = document.getElementById("nextBtn");
    const prevBtn = document.getElementById("prevBtn");

    // Time + progress
    const currentTimeText = document.getElementById("currentTime");
    const totalTimeText = document.getElementById("totalTime");
    const progressBar = document.getElementById("progressBar");

    // Volume
    const volumeSlider = document.getElementById("volumeSlider");

    // ============================
    // PLAY / PAUSE
    // ============================
    playBtn.addEventListener("click", () => {
        if (audio.paused) {
            audio.play();
            playBtn.classList.remove("fa-play-circle");
            playBtn.classList.add("fa-pause-circle");
        } else {
            audio.pause();
            playBtn.classList.remove("fa-pause-circle");
            playBtn.classList.add("fa-play-circle");
        }
    });

    // ============================
    // LOAD BÀI → LẤY THỜI LƯỢNG
    // ============================
    audio.addEventListener("loadedmetadata", () => {
        totalTimeText.textContent = formatTime(audio.duration);
    });

    // ============================
    // CẬP NHẬT PROGRESS + TIME
    // ============================
    audio.addEventListener("timeupdate", () => {
        progressBar.value = (audio.currentTime / audio.duration) * 100;
        currentTimeText.textContent = formatTime(audio.currentTime);
    });

    // ============================
    // TUA NHẠC
    // ============================
    progressBar.addEventListener("input", () => {
        audio.currentTime = (progressBar.value / 100) * audio.duration;
    });

    // ============================
    // VOLUME
    // ============================
    volumeSlider.addEventListener("input", () => {
        audio.volume = volumeSlider.value;
    });

    // ============================
    // FORMAT TIME
    // ============================
    function formatTime(t) {
        if (isNaN(t)) return "0:00";
        let m = Math.floor(t / 60);
        let s = Math.floor(t % 60);
        return `${m}:${s < 10 ? "0" + s : s}`;
    }

});
