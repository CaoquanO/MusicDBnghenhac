const grid2 = document.getElementById("playlistGrid2");
const leftBtn2 = document.getElementById("leftBtn2");
const rightBtn2 = document.getElementById("rightBtn2");

rightBtn2.addEventListener("click", () => {
    grid2.scrollBy({ left: 200, behavior: "smooth" });
});

leftBtn2.addEventListener("click", () => {
    grid2.scrollBy({ left: -200, behavior: "smooth" });
});
