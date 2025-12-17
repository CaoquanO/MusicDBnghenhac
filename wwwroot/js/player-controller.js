
(function () {
    // Elements
    const audio = document.getElementById("audioPlayer");
    const playPauseBtn = document.getElementById("playPauseBtn");
    const prevBtn = document.getElementById("prevBtn");
    const nextBtn = document.getElementById("nextBtn");

    const volumeSlider = document.getElementById("volumeSlider");
    const progressBar = document.getElementById("progressBar");

    const currentTimeText = document.getElementById("currentTime");
    const totalTimeText = document.getElementById("totalTime");

    const playerImage = document.getElementById("player-image");
    const playerTitle = document.getElementById("player-title");
    const playerArtist = document.getElementById("player-artist");

    const SONG_KEY = "player_state";

    // Throttle lưu trạng thái (không lưu quá thường xuyên)
    let lastSaveAt = 0;
    function saveStateThrottled(force = false) {
        const now = Date.now();
        if (force || now - lastSaveAt > 1000) { // tối thiểu 1s
            saveState();
            lastSaveAt = now;
        }
    }

    function saveState() {
        try {
            const state = {
                songUrl: audio.src || "",
                currentTime: audio.currentTime || 0,
                volume: audio.volume ?? 1,
                isPlaying: !audio.paused,
                title: playerTitle?.textContent || "",
                artist: playerArtist?.textContent || "",
                image: playerImage?.src || ""
            };
            localStorage.setItem(SONG_KEY, JSON.stringify(state));
        } catch (e) {
            console.warn("Không lưu được trạng thái player:", e);
        }
    }

    function formatTime(t) {
        if (isNaN(t) || !isFinite(t)) return "0:00";
        const m = Math.floor(t / 60);
        const s = Math.floor(t % 60);
        return `${m}:${s < 10 ? "0" + s : s}`;
    }

    // ========== KHÔI PHỤC TRẠNG THÁI KHI LOAD ==========
    window.addEventListener("DOMContentLoaded", () => {
        try {
            const saved = localStorage.getItem(SONG_KEY);
            if (saved) {
                const state = JSON.parse(saved);

                if (state.songUrl) {
                    // gán src rồi chờ loadedmetadata để gán time & play
                    audio.src = state.songUrl;
                    audio.load();

                    audio.addEventListener("loadedmetadata", function onMeta() {
                        // Chỉ restore time nếu có giá trị hợp lệ
                        if (state.currentTime && state.currentTime > 0 && state.currentTime < audio.duration) {
                            audio.currentTime = state.currentTime;
                        } else {
                            audio.currentTime = 0; // mặc định về 0
                        }

                        audio.volume = state.volume ?? 1;

                        // restore UI (title / artist / image) nếu có
                        if (state.title) playerTitle.textContent = state.title;
                        if (state.artist) playerArtist.textContent = state.artist;
                        if (state.image) playerImage.src = state.image;

                        if (state.isPlaying) {
                            // chơi nếu trước đó đang chơi
                            audio.play().catch(() => { /* autoplay policy */ });
                        }
                        updatePlayPauseIcon();
                        updateTimeDisplays();
                        audio.removeEventListener("loadedmetadata", onMeta);
                    });
                }
            }
        } catch (e) {
            console.warn("Restore player state lỗi:", e);
        }
    });

    // ========== CLICK VÀO BÀI HÁT (SONG CARD) ==========
    // Các card của bạn có class .song-card và chứa data-song/data-title/data-artist/data-image
    document.addEventListener("click", function (ev) {
        const card = ev.target.closest(".song-card");
        if (!card) return;

        const songSrc = card.dataset.song;
        const title = card.dataset.title || "Không rõ";
        const artist = card.dataset.artist || "---";
        const image = card.dataset.image || (playerImage ? playerImage.src : "");

        if (!songSrc) return;

        // Thay bài: đặt src, reset currentTime = 0, update UI, play
        const isSameSong = audio.src && audio.src === songSrc;
        audio.src = songSrc;
        audio.load();

        // Khi đổi bài mới, chắc chắn bắt đầu từ 0
        audio.addEventListener("loadedmetadata", function onMetaChange() {
            audio.currentTime = 0;
            audio.play().catch(() => { }); // play, có thể bị autoplay policy
            updatePlayPauseIcon();
            updateTimeDisplays();
            audio.removeEventListener("loadedmetadata", onMetaChange);

            // Cập nhật UI
            if (playerTitle) playerTitle.textContent = title;
            if (playerArtist) playerArtist.textContent = artist;
            if (playerImage) playerImage.src = image;

            // Lưu trạng thái ngay khi đổi bài (với currentTime = 0)
            saveStateThrottled(true);
        });
    });

    // ========== PLAY/PAUSE ==========
    function updatePlayPauseIcon() {
        if (!playPauseBtn) return;
        if (audio.paused) {
            playPauseBtn.classList.remove("fa-pause-circle");
            playPauseBtn.classList.add("fa-play-circle");
        } else {
            playPauseBtn.classList.remove("fa-play-circle");
            playPauseBtn.classList.add("fa-pause-circle");
        }
    }

    if (playPauseBtn) {
        playPauseBtn.addEventListener("click", () => {
            if (audio.paused) {
                audio.play().catch(() => { });
            } else {
                audio.pause();
            }
            updatePlayPauseIcon();
            saveStateThrottled(true);
        });
    }

    // Prev / Next: bạn có thể gắn logic playlist ở đây (hiện tạm)
    if (prevBtn) prevBtn.addEventListener("click", () => {
        // TODO: implement playlist previous
        // khi đổi bài bằng prev, hãy đặt currentTime = 0 giống như khi click
        // ví dụ: set audio.src = prevSongUrl; audio.currentTime = 0; audio.play();
        console.log("prev clicked - implement playlist logic");
    });
    if (nextBtn) nextBtn.addEventListener("click", () => {
        // TODO: implement playlist next
        console.log("next clicked - implement playlist logic");
    });

    // ========== VOLUME ==========
    if (volumeSlider) {
        // nếu slider range 0..1 thì dùng trực tiếp, nếu 0..100 thì chia 100
        volumeSlider.addEventListener("input", () => {
            const v = parseFloat(volumeSlider.value);
            audio.volume = (v > 1 ? v / 100 : v);
            saveStateThrottled();
        });
    }

    // ========== PROGRESS & TIME UPDATE ==========
    audio.addEventListener("timeupdate", () => {
        if (!audio.duration || isNaN(audio.duration)) return;

        const percent = (audio.currentTime / audio.duration) * 100;
        if (progressBar) progressBar.value = percent;

        if (currentTimeText) currentTimeText.textContent = formatTime(audio.currentTime);
        if (totalTimeText) totalTimeText.textContent = formatTime(audio.duration);

        // lưu trạng thái có throttle
        saveStateThrottled();
    });

    // Tua bằng progressBar
    if (progressBar) {
        progressBar.addEventListener("input", () => {
            if (audio.duration) {
                audio.currentTime = (progressBar.value / 100) * audio.duration;
                saveStateThrottled(true);
            }
        });
    }

    // Khi kết thúc bài, tự chuyển tiếp (nếu có playlist logic bạn gắn vào đây)
    audio.addEventListener("ended", () => {
        // TODO: auto-play next if you have playlist
        updatePlayPauseIcon();
        saveStateThrottled(true);
    });

    // Cập nhật UI hiển thị thời gian (nếu load xong metadata)
    audio.addEventListener("loadedmetadata", updateTimeDisplays);

    function updateTimeDisplays() {
        if (audio.duration && !isNaN(audio.duration)) {
            if (totalTimeText) totalTimeText.textContent = formatTime(audio.duration);
            if (currentTimeText) currentTimeText.textContent = formatTime(audio.currentTime);
            if (progressBar) progressBar.max = 100;
        }
    }

    // ========== LƯU TRẠNG THÁI TRƯỚC KHI RỜI TRANG ==========
    window.addEventListener("beforeunload", () => {
        saveState();
    });

})();
