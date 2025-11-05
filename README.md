# <img src="App/Resources/app_default.ico" alt="AutoInput Icon" width="24" height="24" style="vertical-align: middle; margin-right: 6px;"> AutoInput

AutoInput is a small Windows app that runs simple, timed input for you — mouse clicks or a chosen keyboard key on a loop. Set the interval, pick what to press, optionally schedule a start/stop time, and press **Start**. It keeps count and time so you don’t have to. Built to save a little time on repetitive tasks.

---

## What it does

- Timed automation (0.1 second to 10 minutes).
- Run modes: **until stopped** or **for N inputs**.
- Global Start/Stop hotkey (default **F8**, configurable).
- Target options: **Mouse Left**, **Mouse Right**, or a specific **keyboard key**.
- Scheduling: start at an exact date/time, optional stop time.
- Sequences: define steps (key/click + delay) and repeat the sequence at your interval.
- Optional **Sequence Mode** toggle lets the Start/Stop hotkey run the selected sequence instead of a single key.
- Live status: elapsed time and input count.

---

## Quick start

1. Download the release `.exe` (or build it yourself from source).
2. Run AutoInput — no install required.
3. Set an interval (e.g., 2.5 seconds), choose your target input (mouse click or key), and press **Start**.
4. You can schedule automation to start/stop at a specific time if desired.
5. Save configurations for later and reload them as needed.
6. If a file named **`AutoInput_Config.json`** exists in the same folder as the executable, it will be automatically loaded at app startup.  
   If not found, AutoInput starts with default settings.

---

## Tabs overview

### General
- Choose your automation interval and run mode.
- Set whether to run indefinitely or stop after a set number of inputs.
- Configure your global hotkey and target key/mouse input.

### Schedule
- Set exact start and stop times.
- Enable or disable each independently.
- Works even when minimized — as long as AutoInput is running.

### Sequence
- Create a list of steps (key presses or mouse clicks) with delays between them.
- Use the **Sequence Mode** checkbox to let the global hotkey play the active sequence instead of a single input.
- Multiple sequences can be saved and selected from a dropdown.

### Config
- Save and load different configurations (`.json` files).
- Open the config folder quickly with **Open Folder**.
- Saved configurations remember interval, run mode, hotkeys, and sequences.

###  Info
- Shows app version and basic info.
- Links to the in-app User Guide (coming soon).

---

## Requirements
- Windows 10 or later
- .NET 8 Runtime (included in standalone build)

