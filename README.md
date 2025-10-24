# AutoInput

AutoInput is a small Windows app that runs simple, timed input for you. Set an interval, choose a target (mouse click or key), and press **Start**. It keeps count and time so you don’t have to. 
Built to save a little time on repetitive tasks.

---

## What it does

- Timed automation (0.1 second to 10 minutes).
- Run modes: **until stopped** or **for N inputs**
- Global Start/Stop hotkey (default **F8**, configurable)
- Target options: **Mouse Left**, **Mouse Right**, or a specific **keyboard key**
- Scheduling: start at an exact date/time, optional stop time
- Sequences: define steps (key/click + delay) and repeat the sequence at your interval
- Live status: elapsed time and input count
- Works minimized
- Save/Load configuration and optional auto-load on startup
- Export session activity to CSV

> Use responsibly. Some applications and games block or forbid synthetic input.

---

## Install

**Requirements**
- Windows 10/11
- .NET 8 (Runtime or SDK)

**Option A — Run a build**
1. Download a build (e.g., from your Releases or your own publish output).
2. Run `Autoinput.exe`.

**Option B — Build it yourself**
```bash
# from the repo root
dotnet build -c Release

# or publish to a folder
dotnet publish App/App.csproj -c Release -r win-x64 -o ./release
# executable is in ./release
```

> Note: WinForms and “single-file with trimming” don’t play nicely together. A single-folder publish is the safer default.

---

## Quick start

1. Open **General** → set **Interval (ms)**.
2. Pick a **Run mode** (until stopped or for N inputs).
3. Open **Key Settings** → choose the **Start/Stop Keybind** and the **Target**.
4. Optional: set **Schedule** (start time and optional stop time).
5. Optional: define a **Sequence** (rows of key/click + delay).  
6. Click **Start** (or hit the hotkey). Watch **Active Time** and **Input Count**.

Stop with the same button or the hotkey.

---

## Hotkeys

- Global **Start/Stop**: default **F8** (configurable in **Key Settings**)

If another tool uses the same key, pick a different one.

---

## Notes

- The interval is the outer loop; a sequence is the inner script that runs on each tick.
- If an app ignores input, try running AutoInput **as Administrator**.
- Randomization can be simulated by varying delays inside the Sequence.

---

## Development

- C# / .NET 8, Windows Forms
- Entry: `Program.cs` → `MainForm`
- Utility: `App.Utils.Formatter`

Build normally with `dotnet build` or publish with `dotnet publish` as shown above.

---
