"use strict";

const trPassword = document.getElementById("password");
const trConfirmationPassword = document.getElementById("confirmation-password");
const ckbEditPassword = document.getElementById("EditPassword");

document.addEventListener("DOMContentLoaded", () => {
    hideOrDisplayPasswords();

    ckbEditPassword.addEventListener("click", () => hideOrDisplayPasswords());
});

const hideOrDisplayPasswords = () => {
    if (ckbEditPassword.checked) {
        trPassword.style.display = "";
        trConfirmationPassword.style.display = "";
    } else {
        trPassword.style.display = "none";
        trConfirmationPassword.style.display = "none";
    }
}