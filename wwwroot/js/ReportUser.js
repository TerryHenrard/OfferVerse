﻿document.addEventListener("DOMContentLoaded", () => document.getElementById("Description").addEventListener("input", (ev) => document.getElementById("length").textContent = -100 + ev.target.value.length));