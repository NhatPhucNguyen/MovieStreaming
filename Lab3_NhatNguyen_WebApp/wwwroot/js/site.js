// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

const btnSearchRate = document.getElementById("btnRateSearch");
const rateSelected = document.getElementById("rateSelected");
btnSearchRate.addEventListener('click', (e) => {
    e.preventDefault();
    if (location.href.includes("?") && !location.href.includes("rate")) {
        window.location = location.href + "&rate=" + rateSelected.value;
    }
    else {
        window.location = location.pathname + "?rate=" + rateSelected.value;
    }
})
