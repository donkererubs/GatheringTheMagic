# GatheringTheMagic

## Architecture
- **Domain**: CardDefinition, CardInstance, Deck, Game, Enums…
- **Infrastructure**: SampleCards seed data
- **CLI**: console app driving Game phases
- **Web**: ASP NET Core minimal API + SPA in wwwroot

## What Works
- Deck‐building, shuffling, opening hands, turn phases, draw/play, land rules, status flags…
- CLI UI, Web API endpoints with full state mapping

## Next Steps
- Implement combat step
- Trigger resolution (Upkeep, End Step)
- MTG stack and instantaneous interactions
- Persisting game state / multiple games
