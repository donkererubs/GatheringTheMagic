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
// Use non-null assertions or checks so TypeScript knows they exist
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
// Render the top‐level status (active player, phase, deck & hand counts)
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
    renderHand(state.playerHand);
    controlsDiv.style.display = "block";
}));
// Go to the next phase
nextPhaseBtn.addEventListener("click", () => __awaiter(void 0, void 0, void 0, function* () {
    yield fetch("/api/game/next-phase", { method: "POST" });
    const state = yield fetchState();
    renderState(state);
    // Only render the hand during Main phases
    if (state.currentPhase.startsWith("Main")) {
        renderHand(state.playerHand);
    }
    else {
        handList.innerHTML = "";
        handCount.textContent = "0";
    }
}));
// You can extend similarly for play/draw buttons if you add them...
