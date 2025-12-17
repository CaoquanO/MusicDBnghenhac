// ===============================
// 1. PHỤC HỒI PLAYER SAU KHI ĐỔI TRANG
// ===============================
window.addEventListener("DOMContentLoaded", () => {
    const audio = document.getElementById("audioPlayer");

    let saved = localStorage.getItem("player_state");

    if (saved) {
        let state = JSON.parse(saved);

        if (state.FilePath) {
            audio.src = state.FilePath;
            audio.load();

            audio.addEventListener("loadedmetadata", () => {
                audio.currentTime = state.currentTime || 0;
                audio.volume = state.volume ?? 1;

                if (state.isPlaying) audio.play();
                updatePlayPauseIcon();
            });
        }

        // *** KHÔI PHỤC HÌNH ẢNH + TÊN + NGHỆ SĨ ***
        if (state.image) document.getElementById("player-image").src = state.image;
        if (state.title) document.getElementById("player-title").textContent = state.title;
        if (state.artist) document.getElementById("player-artist").textContent = state.artist;
    }
});

const audio = document.getElementById("audioPlayer");

// ===============================
// 2. LẤY PHẦN TỬ THEO ĐÚNG ID HTML
// ===============================
const playPauseBtn = document.getElementById("playPauseBtn");
const prevBtn = document.getElementById("prevBtn");
const nextBtn = document.getElementById("nextBtn");

const volumeSlider = document.getElementById("volumeSlider");
const progressBar = document.getElementById("progressBar");

const currentTimeText = document.getElementById("currentTime");
const totalTimeText = document.getElementById("totalTime");

const img = document.getElementById("player-image");
const title = document.getElementById("player-title");
const artist = document.getElementById("player-artist");

// ===============================
// 3. ĐỔI ICON PLAY/PAUSE
// ===============================
function updatePlayPauseIcon() {
    if (audio.paused)
        playPauseBtn.classList.replace("fa-pause-circle", "fa-play-circle");
    else
        playPauseBtn.classList.replace("fa-play-circle", "fa-pause-circle");
}

// ===============================
// 4. PLAY / PAUSE
// ===============================
playPauseBtn.addEventListener("click", () => {
    if (audio.paused) audio.play();
    else audio.pause();

    updatePlayPauseIcon();
});

// ===============================
// 5. VOLUME
// ===============================
volumeSlider.addEventListener("input", () => {
    audio.volume = volumeSlider.value;
});

// ===============================
// 6. CẬP NHẬT PROGRESS
// ===============================
audio.addEventListener("timeupdate", () => {
    if (!audio.duration) return;

    progressBar.value = (audio.currentTime / audio.duration) * 100;

    currentTimeText.textContent = formatTime(audio.currentTime);
    totalTimeText.textContent = formatTime(audio.duration);
});

// tua
progressBar.addEventListener("input", () => {
    if (audio.duration) {
        audio.currentTime = (progressBar.value / 100) * audio.duration;
    }
});

// format thời gian
function formatTime(t) {
    let m = Math.floor(t / 60);
    let s = Math.floor(t % 60);
    return `${m}:${s < 10 ? "0" + s : s}`;
}

// ===============================
// 7. LƯU TOÀN BỘ TRẠNG THÁI PLAYER
// ===============================
window.addEventListener("beforeunload", () => {
    localStorage.setItem("player_state", JSON.stringify({
        FilePath: audio.src,
        currentTime: audio.currentTime,
        volume: audio.volume,
        isPlaying: !audio.paused,

        // *** LƯU THÊM HÌNH/INFO ***
        image: img.src,
        title: title.textContent,
        artist: artist.textContent
    }));
});
// ===============================
// 8. HÀM PHÁT BÀI MỚI (CHUẨN NHẤT)
// ===============================
function playNewSong(filePath, imageUrl, songTitle, songArtist) {
    audio.pause();          // dừng bài cũ
    audio.src = filePath;   // đổi bài hát
    audio.load();           // tải lại bắt buộc
    audio.currentTime = 0;  // RESET về 0:00
    audio.play();           // chạy bài mới

    // cập nhật UI
    img.src = imageUrl;
    title.textContent = songTitle;
    artist.textContent = songArtist;

    updatePlayPauseIcon();

    // lưu ngay để khi đổi trang còn đúng bài
    localStorage.setItem("player_state", JSON.stringify({
        FilePath: filePath,
        currentTime: 0,
        volume: audio.volume,
        isPlaying: true,
        image: imageUrl,
        title: songTitle,
        artist: songArtist
    }));
}
