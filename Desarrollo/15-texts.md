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

## Workflow
- **Source of truth:** `Assets/_Project/Localization/Source/<TABLE>.tsv`, tab-separated `key, en, es[, note]` (use `
` for line breaks). Never edit the generated tables in `Localization/Tables/` by hand.
- Run **Heartbeat ▸ Import Localization** (`Heartbeat.EditorTools.LocalizationImporter.Import()`, callable from `execute_script`). It creates settings/locales if missing and upserts every row.
- Runtime: `Heartbeat.UI.Loc.Get("UI.TERM.SUBJECT", "R-H-09")` (table = first key segment, `{0}` args via string.Format), `LocalizedLabel` for static TMP labels, `Loc.SetLanguage("es")` (Bootstrap applies `GlobalData.language`). Missing keys render as the key itself.
- Localization uses Addressables: a player build needs **Addressables ▸ Build** first (E13-build).

## Rules
- Key format: `<table>.<area>.<id>`, e.g. `DOC.EMILIA.03`, `SUB.MOTHER.P2.01`.
- German lines: put `DE-NO-TRANSLATE` in the note column; the importer copies the English cell to Spanish.
- Corruption/glitch in documents is a render effect (TMP character jitter); it never removes information needed to progress.
- `DocumentDefinition` SOs reference table keys, owner, floor, kind (Emilia / Context / Confiscated) and the registry id.

## Tasks
`E15-documents` · `E15-bestiary` · `E15-posters` · `E15-es-table`.
