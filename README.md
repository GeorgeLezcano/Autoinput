# AutoInput

AutoInput is a lightweight Windows Forms application designed to automate and track simple timed inputs.  
It provides an interface to start and stop input intervals, monitor elapsed time, and count the number of automated inputs.

---

## Overview

**AutoInput** allows users to:

- Start and stop automated actions at a configurable interval (in milliseconds).  
- Track how long the automation has been active.  
- Count how many times an automated input has been triggered.  
- Assign custom hotkeys for starting/stopping and selecting a target input key.  

This tool is intended for simple repetitive automation scenarios such as key presses or clicks.

---

## Features (Current & Planned)

### ✅ Current
- Clean and simple WinForms UI
- Start/Stop timer control
- Adjustable interval input (100–10000 ms)
- Real-time tracking of elapsed time
- Input count tracking
- Basic reset functionality

### 🧭 Planned
- Customizable start/stop hotkey (default: **F8**)
- Customizable target input key (default: **Left Mouse Button**)
- Persistent settings (save preferences between sessions)
- Background/minimized operation
- Logging or export of activity sessions

---

---

## Technical Details

| Aspect | Description |
|--------|--------------|
| **Framework** | .NET 8 (WinForms) |
| **Language** | C# |
| **UI Type** | Windows Forms |
| **Default Interval** | 1000 ms |

---
