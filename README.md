# <img src="App/Resources/app_default.ico" alt="AutoInput Icon" width="24" height="24" style="vertical-align: middle; margin-right: 6px;"> AutoInput

AutoInput is a small Windows app that runs simple, timed input for you — mouse clicks or a chosen keyboard key — on a loop. Set the interval, pick what to press, optionally schedule a start/stop time, and press **Start**. It keeps count and time so you don’t have to. Built to save a little time on repetitive tasks.

---

## What it does

- Timed automation (0.1 second to 10 minutes).
- Run modes: **until stopped** or **for N inputs**.
- Global Start/Stop hotkey (default **F8**, configurable).
- Target options: **Mouse Left**, **Mouse Right**, or a specific **keyboard key**.
- Scheduling: start at an exact date/time, optional stop time.
- Sequences: define steps (key/click + delay) and repeat the sequence at your interval.
- Live status: elapsed time and input count.

---

## Quick start

1. Download the release `.exe` (or build it yourself; see below).
2. Open your target app or game and focus it.
3. In AutoInput:
   - Choose **Target** (mouse left/right or a specific key).
   - Set **Interval** (ms or seconds).
   - (Optional) Set **Run Mode** (until stopped / N inputs).
   - (Optional) Add a **Schedule** (start/stop).
   - (Optional) Create a **Sequence** (multi-step key/click + delay).
4. Press **Start** (or use the global hotkey). Press **Stop** to end.

Tips:
- ⚠️ Running AutoInput in certain apps may require administrator rights.
- Randomization can be approximated by varying delays inside the Sequence.

---

## Build & release

### Build (debug)
```bash
dotnet build
```

### Publish (manual)
Creates a single-file, self-contained Windows x64 executable under `./release/`:
```bash
dotnet publish App/App.csproj   -c Release   -r win-x64   --self-contained true   -p:PublishSingleFile=true   -p:IncludeNativeLibrariesForSelfExtract=true   -p:EnableCompressionInSingleFile=true   -p:DebugType=none   -p:DebugSymbols=false   -o release
```

### Publish (script)
Use the helper script to publish and keep only the `.exe`:
```bash
./create-release.sh
```
- Output: `./release/AutoInput.exe` (other files are removed by the script).

> Note: Windows Forms trimming is **not** enabled here to avoid runtime issues. The publish settings match what the app expects.

---

## Configuration & defaults

- **Hotkey**: default is **F8** (can be changed in the app).
- **Start/Stop button**: green when idle (ready to start), red when running.
- **Interval**: accepts milliseconds; input is validated by the UI.
- **Sequences**: each step consists of `Action (key/click)` + `Delay`.
- **Scheduling**: precise start timestamp; optional stop timestamp.

---

## Requirements

- Windows 10/11 x64
- .NET runtime is bundled in the self-contained release (no separate install needed).

---

## Notes
- Make sure the target window has focus when automation begins.
- Some apps block simulated input; try running AutoInput as Administrator if needed.

##  **⚠️ Use responsibly!**. Avoid sending automated input to systems that disallow it.

