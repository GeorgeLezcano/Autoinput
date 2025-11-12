
## Goals for v1.5.0

- [x] **User Guide form** – Add a modeless help window with embedded HTML or markdown.
- [x] **Confirm on exit** – Prompt to save configuration before closing.
- [x] **Schedule fixes** – If start/stop times are in the past, uncheck or start now.
- [x] **Auto-uncheck schedule checkboxes after firing**.
- [ ] **Readable punctuation in sequence combo** – Use `KeyMapper` for `/ . , ; ' [ ] - = \ ``.
- [x] **Single-instance check** – Prevent multiple AutoInput instances with a `Mutex`.
- [x] **High-DPI + double-buffering** – Call `Application.SetHighDpiMode(SystemAware)`.
- [ ] **Count mode toggle** – Switch between counting inputs or full sequences.
- [ ] **Remember window size & position** – Save and restore from config.


## Goals for v1.6.0

- [ ] **Export session log / CSV (timestamp, key, delay)**.
- [ ] **Stopwatch-based elapsed timer** – Replace per-tick counter for accuracy.
- [ ] **Unit tests (pure helpers)** – For `TimeUtils`, `LabelFormatter`, `KeyMapper`.