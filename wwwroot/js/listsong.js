document.addEventListener('DOMContentLoaded', function () {
    const leftBtn = document.getElementById('leftBtn');
    const rightBtn = document.getElementById('rightBtn');
    const playlistGrid = document.getElementById('playlistGrid');
    // Scroll trái (di chuyển về card trước)
    leftBtn.addEventListener('click', function () {
        playlistGrid.scrollBy({ left: -180, behavior: 'smooth' }); // Scroll 180px trái (1 card)
    });
    // Scroll phải (di chuyển đến card tiếp theo)
    rightBtn.addEventListener('click', function () {
        playlistGrid.scrollBy({ left: 180, behavior: 'smooth' }); // Scroll 180px phải (1 card)
    });
});