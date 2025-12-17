(function () {

    /* ===============================
       ELEMENTS
    =============================== */
    const audio = document.getElementById("audioPlayer");
    if (!audio) return;

    const playPauseBtn = document.getElementById("playPauseBtn");
    const prevBtn = document.getElementById("prevBtn");
    const nextBtn = document.getElementById("nextBtn");
    const progressBar = document.getElementById("progressBar");
    const volumeSlider = document.getElementById("volumeSlider");

    const currentTimeText = document.getElementById("currentTime");
    const totalTimeText = document.getElementById("totalTime");
    const img = document.getElementById("player-image");
    const title = document.getElementById("player-title");
    const artist = document.getElementById("player-artist");

    const STORAGE_KEY = "music_player_state_final";

    /* ===============================
       PLAYLIST
    =============================== */
    const songCards = document.querySelectorAll(".song-card");
    const playlist = [];
    let currentIndex = -1;

    songCards.forEach((card, index) => {
        playlist.push({
            src: card.dataset.song,
            title: card.dataset.title,
            artist: card.dataset.artist,
            image: card.dataset.image,
            el: card
        });

        card.addEventListener("click", () => playSong(index));
    });

    /* ===============================
       CORE
    =============================== */
    function playSong(index) {
        const song = playlist[index];
        if (!song) return;

        currentIndex = index;

        audio.pause();
        audio.src = song.src;
        audio.currentTime = 0;
        audio.play().catch(() => { });

        updateUI(song);
        highlight(index);
        updatePlayPauseIcon();
        saveState(true);
    }

    function updateUI(song) {
        img.src = song.image;
        title.textContent = song.title;
        artist.textContent = song.artist;
    }

    function highlight(index) {
        songCards.forEach(c => c.classList.remove("playing"));
        songCards[index]?.classList.add("playing");
    }

    /* ===============================
       PLAY / PAUSE
    =============================== */
    playPauseBtn.addEventListener("click", () => {
        audio.paused ? audio.play() : audio.pause();
        updatePlayPauseIcon();
        saveState(true);
    });

    function updatePlayPauseIcon() {
        playPauseBtn.classList.toggle("fa-play-circle", audio.paused);
        playPauseBtn.classList.toggle("fa-pause-circle", !audio.paused);
    }

    /* ===============================
       NEXT / PREV
    =============================== */
    nextBtn.onclick = () =>
        playSong((currentIndex + 1) % playlist.length);

    prevBtn.onclick = () =>
        playSong((currentIndex - 1 + playlist.length) % playlist.length);

    audio.addEventListener("ended", nextBtn.onclick);

    /* ===============================
       PROGRESS
    =============================== */
    audio.addEventListener("timeupdate", () => {
        if (!audio.duration) return;

        progressBar.value = audio.currentTime / audio.duration * 100;
        currentTimeText.textContent = format(audio.currentTime);
        totalTimeText.textContent = format(audio.duration);
        saveState();
    });

    progressBar.oninput = () => {
        audio.currentTime = audio.duration * (progressBar.value / 100);
        saveState(true);
    };

    function format(t) {
        if (isNaN(t)) return "0:00";
        const m = Math.floor(t / 60);
        const s = Math.floor(t % 60);
        return `${m}:${s < 10 ? "0" : ""}${s}`;
    }

    /* ===============================
       VOLUME
    =============================== */
    volumeSlider.oninput = () => {
        audio.volume = volumeSlider.value;
        saveState(true);
    };

    /* ===============================
       LOCAL STORAGE (SAFE)
    =============================== */
    let lastSave = 0;
    function saveState(force = false) {
        if (!force && Date.now() - lastSave < 1000) return;
        lastSave = Date.now();

        localStorage.setItem(STORAGE_KEY, JSON.stringify({
            index: currentIndex,
            time: audio.currentTime,
            volume: audio.volume,
            playing: !audio.paused
        }));
    }

    /* ===============================
       RESTORE
    =============================== */
    window.addEventListener("DOMContentLoaded", () => {
        const saved = localStorage.getItem(STORAGE_KEY);
        if (!saved) return;

        const s = JSON.parse(saved);
        if (s.index == null || !playlist[s.index]) return;

        currentIndex = s.index;
        const song = playlist[s.index];

        audio.src = song.src;
        audio.volume = s.volume ?? 1;

        updateUI(song);
        highlight(s.index);

        audio.addEventListener("loadedmetadata", function restore() {
            if (s.time < audio.duration) audio.currentTime = s.time;
            if (s.playing) audio.play().catch(() => { });
            updatePlayPauseIcon();
            audio.removeEventListener("loadedmetadata", restore);
        });
    });

})();
