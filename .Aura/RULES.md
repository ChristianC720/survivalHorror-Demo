# Rules to paste into Aura → Advanced Settings ("rules Aura should always follow")

Aura does not read CLAUDE.md automatically. Paste the block below into Aura's Advanced Settings rules for this project (once per machine).

```
Project: Identity Dissolution (Heartbeat), Unity 6000.6.6f1 URP. Before any task run the /heartbeat-context skill.
- Read CLAUDE.md and the matching Desarrollo/ guide before changing anything. GDD/ (Spanish) is the design source of truth.
- Code goes in Assets/_Project/Scripts/<System>, namespace Heartbeat.<System>. No singletons outside Bootstrap; use Services.Get<T>() and GameEvents.
- All balance numbers in ScriptableObjects. Never read Keyboard.current/Mouse.current in gameplay; use InputRouter.
- Only PauseService may set Time.timeScale.
- Kit pieces and rooms: built by hand on the 2 m grid with exact pivots per Desarrollo/KIT_SPEC.md (skills /build-kit-piece, /build-room-prefab). Never generate them with scripts.
- Blender models are PH_ primitive placeholders (/place-placeholder-prop). Audio = AudioCue assets with empty clips.
- Save the scene before batch edits. After C# edits check compile errors and logs. Finish gameplay tasks with /playtest-task.
- Do not write one-off editor scripts into Assets/. Do not commit; Claude Code handles git.
```
