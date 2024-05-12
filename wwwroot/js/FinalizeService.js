"use strict"

const rating = document.getElementById("rating");
const hours = document.getElementById("hours");
const inputRating = document.getElementById("input-rating");
const inputHours = document.getElementById("input-hours");
const reviewContainer = document.getElementById("review");
const reportContainer = document.getElementById("report");
const toggleBtn = document.getElementById("switch");


document.addEventListener("DOMContentLoaded", () => {
    inputRating.value = 0;
    inputHours.value = 1;

    inputRating.addEventListener("input", (ev) => rating.textContent = ev.target.value);
    inputHours.addEventListener("input", (ev) => hours.textContent = ev.target.value);

    let side = "left";
    let isButtonDisabled = false;
   
    toggleBtn.addEventListener("click", () => {
        if (!isButtonDisabled) { 
            isButtonDisabled = true; 

            if (side === "left") {
                side = "right";
                reportContainer.style.display = "block";
                reviewContainer.style.display = "none";
            } else if (side === "right") {
                side = "left";
                reportContainer.style.display = "none";
                reviewContainer.style.display = "block";
            }

            setTimeout(() => isButtonDisabled = false, 100); 
        }
    });
});

