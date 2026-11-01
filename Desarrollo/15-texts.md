# E-15 Texts and localization

**Goal:** every string in Localization tables (English primary, Spanish), German never translated.

**GDD sources:** `03 - Historia y Mundo/05 - Textos del Juego.md` (all drafts), `04 - Diálogos y Narrativa.md`, `03 - Personajes.md` (bestiary), D-52, D-58, D-62, D-79.

## Tables (`Assets/_Project/Localization/`)
| Table | Content |
|---|---|
| `UI` | menus, options, prompts, terminal labels (`HEALTH: FINE` …), confirmations |
| `SUB` | subtitles: The Mother's protocol lines, Lukas's murmurs, Emilia's voice note |
| `DOC` | 6 Emilia documents, 12 context documents (Vitalis memos, Kessler guard reports, The Mother's clinical reports, incident logs) |
| `BEST` | bestiary entries (clinical reports) for the 4 enemy types + recycled |
| `CONF` | 12 confiscated objects (label: code, date, owner + 1–2 cold lines) |
| `POST` | tutorial and ambience posters (key names come from the binding, `{key}` smart string) |
| `END` | ending texts and credits |

## Rules
- Key format: `<table>.<area>.<id>`, e.g. `DOC.EMILIA.03`, `SUB.MOTHER.P2.01`.
- German lines: same text in both locales; mark the entry with the metadata comment `DE-NO-TRANSLATE`.
- Corruption/glitch in documents is a render effect (TMP character jitter); it never removes information needed to progress.
- `DocumentDefinition` SOs reference table keys, owner, floor, kind (Emilia / Context / Confiscated) and the registry id.

## Tasks
`E15-documents` · `E15-bestiary` · `E15-posters` · `E15-es-table`.
