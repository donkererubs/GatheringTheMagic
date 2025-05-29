// --- DTOs ---
interface CardDto {
    name: string;
    types: string[];
    statuses: string[];
}
interface GameState {
    activePlayer: string;
    currentPhase: string;
    playerDeck: number;
    opponentDeck: number;
    playerHand: CardDto[];
    opponentHand: CardDto[];
    playerBattlefield: CardDto[];
    opponentBattlefield: CardDto[];
}

// --- DOM References ---
// Use non-null assertions or checks so TypeScript knows they exist
const startBtn = document.getElementById("start") as HTMLButtonElement;
const nextPhaseBtn = document.getElementById("nextPhase") as HTMLButtonElement;
const statusDiv = document.getElementById("status") as HTMLDivElement;
const controlsDiv = document.getElementById("controls") as HTMLDivElement;
const handCount = document.getElementById("handCount") as HTMLSpanElement;
const handList = document.getElementById("handList") as HTMLUListElement;

// --- Helpers ---
function isYes(input: string | null): boolean {
    return input?.trim().toLowerCase() === "y";
}

// Render the top‐level status (active player, phase, deck & hand counts)
function renderState(s: GameState) {
    statusDiv.innerHTML = `
    Active: ${s.activePlayer} — Phase: ${s.currentPhase}<br/>
    Decks: You(${s.playerDeck}), Opp(${s.opponentDeck})<br/>
    Hands: You(${s.playerHand.length}), Opp(${s.opponentHand.length})
  `;
}

// Render your hand as “1. [Type1 | Type2]: Name” items
function renderHand(cards: CardDto[]) {
    handCount.textContent = `${cards.length}`;
    handList.innerHTML = cards
        .map((card, i) => {
            const types = card.types.join(" | ");
            return `<li>${i + 1}. [${types}]: ${card.name}</li>`;
        })
        .join("");
}

// Fetch the full game state from the API
async function fetchState(): Promise<GameState> {
    const res = await fetch("/api/game/state");
    if (!res.ok) throw new Error("Failed to fetch game state");
    return (await res.json()) as GameState;
}

// --- Event Handlers ---

// Start a new game
startBtn.addEventListener("click", async () => {
    await fetch("/api/game/start", { method: "POST" });
    const state = await fetchState();
    renderState(state);
    renderHand(state.playerHand);
    controlsDiv.style.display = "block";
});

// Go to the next phase
nextPhaseBtn.addEventListener("click", async () => {
    await fetch("/api/game/next-phase", { method: "POST" });
    const state = await fetchState();
    renderState(state);

    // Only render the hand during Main phases
    if (state.currentPhase.startsWith("Main")) {
        renderHand(state.playerHand);
    } else {
        handList.innerHTML = "";
        handCount.textContent = "0";
    }
});

// You can extend similarly for play/draw buttons if you add them...
