"use strict";
var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
// --- DOM References ---
const startBtn = document.getElementById("start");
const nextPhaseBtn = document.getElementById("nextPhase");
const statusDiv = document.getElementById("status");
const controlsDiv = document.getElementById("controls");
const handCount = document.getElementById("handCount");
const handList = document.getElementById("handList");
// --- Helpers ---
function isYes(input) {
    return (input === null || input === void 0 ? void 0 : input.trim().toLowerCase()) === "y";
}
// Render the top-level status (active player, phase, deck & hand counts)
function renderState(s) {
    statusDiv.innerHTML = `
    Active: ${s.activePlayer} — Phase: ${s.currentPhase}<br/>
    Decks: You(${s.playerDeck}), Opp(${s.opponentDeck})<br/>
    Hands: You(${s.playerHand.length}), Opp(${s.opponentHand.length})
  `;
}
// Render your hand as “1. [Type1 | Type2]: Name” items
function renderHand(cards) {
    handCount.textContent = `${cards.length}`;
    handList.innerHTML = cards
        .map((card, i) => {
        const types = card.types.join(" | ");
        return `<li>${i + 1}. [${types}]: ${card.name}</li>`;
    })
        .join("");
}
// Get a nice tooltip string for a card
function getCardTooltip(card) {
    return `Name: ${card.name}
            Types: ${card.types.join(", ")}`;
    //Statuses: ${card.statuses.join(", ")}
}
// Highlight a card div, removing highlight from siblings
function highlightCard(div) {
    var _a, _b;
    Array.from((_b = (_a = div.parentElement) === null || _a === void 0 ? void 0 : _a.children) !== null && _b !== void 0 ? _b : []).forEach(c => c.classList.remove("highlight"));
    div.classList.add("highlight");
}
// Render a battlefield zone (your or opponent's) with clickable and hoverable cards
function renderBattlefield(zoneId, cards, onCardClick) {
    const zoneElement = document.getElementById(zoneId);
    if (!zoneElement)
        return;
    zoneElement.innerHTML = "";
    if (cards.length === 0) {
        const emptyMsg = document.createElement("div");
        emptyMsg.className = "empty-zone-message";
        emptyMsg.textContent = "This battlefield is empty.";
        zoneElement.appendChild(emptyMsg);
        return;
    }
    cards.forEach((card, i) => {
        const cardDiv = document.createElement("div");
        cardDiv.className = "card";
        cardDiv.textContent = `[${card.types.join(" | ")}] ${card.name}`;
        if (card.statuses && card.statuses["tapped"]) {
            cardDiv.classList.add("tapped");
        }
        cardDiv.title = getCardTooltip(card);
        if (onCardClick) {
            cardDiv.addEventListener("click", () => onCardClick(card, i));
        }
        zoneElement.appendChild(cardDiv);
    });
}
// Render all card zones (hand, your battlefield, opponent battlefield)
function renderAllZones(state) {
    renderHand(state.playerHand);
    renderBattlefield("your-battlefield-cards", state.playerBattlefield, (card, i) => {
        const zone = document.getElementById("your-battlefield-cards");
        const div = zone.children[i];
        highlightCard(div);
        console.log("You clicked your battlefield card:", card);
        // Add your game logic here!
    });
    renderBattlefield("opponent-battlefield-cards", state.opponentBattlefield, (card, i) => {
        const zone = document.getElementById("opponent-battlefield-cards");
        const div = zone.children[i];
        highlightCard(div);
        console.log("You clicked opponent's battlefield card:", card);
        // Add logic here if you want interaction with opponent's cards
    });
}
// Fetch the full game state from the API
function fetchState() {
    return __awaiter(this, void 0, void 0, function* () {
        const res = yield fetch("/api/game/state");
        if (!res.ok)
            throw new Error("Failed to fetch game state");
        return (yield res.json());
    });
}
// --- Event Handlers ---
// Start a new game
startBtn.addEventListener("click", () => __awaiter(void 0, void 0, void 0, function* () {
    yield fetch("/api/game/start", { method: "POST" });
    const state = yield fetchState();
    renderState(state);
    renderAllZones(state);
    controlsDiv.style.display = "block";
}));
// Go to the next phase
nextPhaseBtn.addEventListener("click", () => __awaiter(void 0, void 0, void 0, function* () {
    yield fetch("/api/game/next-phase", { method: "POST" });
    const state = yield fetchState();
    renderState(state);
    // Render everything, or restrict as you like
    renderAllZones(state);
}));
// app.js
document.addEventListener("DOMContentLoaded", () => {
    // Grab all relevant elements
    const startBtn = document.getElementById("start");
    const statusDiv = document.getElementById("status");
    const controls = document.getElementById("controls");
    const nextPhaseBtn = document.getElementById("nextPhase");
    // Check that startBtn, statusDiv, and controls exist before attaching listener
    if (startBtn) {
        startBtn.addEventListener("click", () => {
            if (controls) {
                controls.style.display = "block";
            }
            else {
                console.warn("Warning: #controls element not found in DOM.");
            }
            if (statusDiv) {
                statusDiv.textContent = "Game started. It’s your turn.";
            }
            else {
                console.warn("Warning: #status element not found in DOM.");
            }
        });
    }
    else {
        console.warn("Warning: #start button not found in DOM.");
    }
    // Check nextPhaseBtn before attaching listener
    if (nextPhaseBtn) {
        nextPhaseBtn.addEventListener("click", () => {
            console.log("Advancing to next phase…");
            // your phase-advancement logic here
        });
    }
    else {
        console.warn("Warning: #nextPhase button not found in DOM.");
    }
});
// === Example app.js snippet ===
// Simulate your “hand” structure. In your actual game, you probably fill this when drawing cards.
let playerHand = [
    { name: "Forest", imageUrl: "images/forest.jpg" },
    { name: "Llanowar Elves", imageUrl: "images/llanowar-elves.jpg" },
    { name: "Serra Angel", imageUrl: "images/serra-angel.jpg" }
];
// Utility function: given your hand array, render each card into the "#your-hand" div
function renderPlayerHand() {
    const yourHandDiv = document.getElementById("your-hand");
    if (!yourHandDiv) {
        console.warn('Cannot render hand → element "#your-hand" not found in DOM.');
        return;
    }
    // 1) Clear out old cards
    yourHandDiv.innerHTML = "";
    // 2) For each card in playerHand, create a .card element
    playerHand.forEach(cardObj => {
        // Create a wrapper DIV
        const cardEl = document.createElement("div");
        cardEl.classList.add("card");
        // If your card object has an imageURL, show the image:
        if (cardObj.imageUrl) {
            const img = document.createElement("img");
            img.src = cardObj.imageUrl;
            img.alt = cardObj.name;
            cardEl.appendChild(img);
        }
        // Add the card’s name below or above the image
        const nameEl = document.createElement("div");
        nameEl.textContent = cardObj.name;
        nameEl.style.position = "absolute";
        nameEl.style.bottom = "6px";
        nameEl.style.width = "100%";
        nameEl.style.fontWeight = "600";
        nameEl.style.fontSize = "0.8em";
        cardEl.appendChild(nameEl);
        // 3) Append this card into #your-hand
        yourHandDiv.appendChild(cardEl);
    });
}
// When the DOM is ready (and after you populate playerHand), call renderPlayerHand():
document.addEventListener("DOMContentLoaded", () => {
    // ... your existing setup code (start button, etc.) ...
    // Example: simulate drawing cards once the “Start” button is clicked
    const startBtn = document.getElementById("start");
    if (startBtn) {
        startBtn.addEventListener("click", () => {
            // Suppose your game logic deals cards into playerHand here...
            // For demonstration, we already have 3 cards in playerHand above.
            // Now render them:
            renderPlayerHand();
        });
    }
    // If you add or remove cards from playerHand later (e.g. drawing a card),
    // just call renderPlayerHand() again to refresh the zone.
});
