# TODO — Hold Feature & Input Engine (Stopping Point)

Last updated: 2026-02-23

This file captures what is still **missing/wrong** and the **next implementation steps**.

---

## 1) Bugs / Fixes (do these first)

### 1.1 ScheduleTimer_Tick starts inputCountTimer twice
**Problem:** In `ScheduleTimer_Tick` you have `if (!holdTargetCheck.Checked) inputCountTimer.Start();` twice.

**Fix:** Remove the duplicate `Start()` call.

**Steps:**
1. Locate `ScheduleTimer_Tick`.
2. Keep only one `if (!holdTargetCheck.Checked) inputCountTimer.Start();`.

---

### 1.2 ScheduleTimer_Tick: hold mode should guard Keys.None
**Problem:** In scheduled start you do:

```csharp
if (holdTargetCheck.Checked && !_isTargetHeldDown)
{
    SendDown(targetKey);
    _isTargetHeldDown = true;
}
```

If `targetKey == Keys.None`, `SendDown` is a no-op but `_isTargetHeldDown` becomes `true` (incorrect state).

**Fix:**
```csharp
if (holdTargetCheck.Checked && !_isTargetHeldDown && targetKey != Keys.None)
{
    SendDown(targetKey);
    _isTargetHeldDown = true;
}
```

**Steps:**
1. Update the scheduled-start hold block with `targetKey != Keys.None`.

---

### 1.3 Sequence Hold edge case: Hold step with Keys.None can lock `_sequenceAwaitingRelease`
**Problem:**
- In `DoSequenceTick`, for a step where `step.Hold == true`, you set:
  - `_sequenceHeldKey = step.Key;`
  - `_sequenceAwaitingRelease = true;`
- But if `step.Key == Keys.None`, then:
  - release logic never runs (it requires `_sequenceHeldKey != Keys.None`)
  - `_sequenceAwaitingRelease` stays `true` forever.

**Fix options (pick one):**

**Option A (recommended): skip invalid hold steps**
```csharp
if (step.Hold && step.Key == Keys.None)
{
    _sequenceStepIndex++;
    inputCountTimer.Interval = Math.Max(step.DelayMS, 1);
    return;
}
```

**Option B:** Only set awaiting-release when `step.Key != Keys.None`.

**Steps:**
1. Add a guard at the top of the hold branch in `DoSequenceTick`.
2. Ensure `_sequenceAwaitingRelease` is only set when the key is not `Keys.None`.

---

## 2) Behavior Review Notes (current behavior is OK)

### 2.1 Sequence Hold implementation
Your current logic is coherent:

- Tick N: `SendDown(step.Key)` for Hold steps
- Tick N+1: release via `_sequenceAwaitingRelease` block
- Then continues sequence progression

That’s a valid “hold for DelayMS” interpretation.

---

## 3) Recommended Improvements (nice-to-have)

### 3.1 Detect SendInput failures
Right now `NativeInput` ignores the return value of `SendInput`.
If SendInput fails (focus restrictions, elevated window, etc.), you won’t know.

**Steps:**
1. Capture return value: `var sent = SendInput(...);`
2. If `sent == 0`, optionally log `Marshal.GetLastWin32Error()`.
3. Consider returning `bool` from NativeInput methods.

---

### 3.2 Reduce duplicate timer-start decision logic
You currently have “do we start inputCountTimer?” logic in multiple places:
- `ApplyRunningUiState`
- `ScheduleTimer_Tick`

**Steps:**
1. Create a helper like `StartOrStopInputTimerForMode()`.
2. Use it in both places.

---

### 3.3 Clarify “Hold” meaning in UI/Guide
Right now sequence step Hold means:
- “Hold down for DelayMS, then release next tick.”

Document that in your user guide.

---

## 4) Quick Testing Checklist

### General Hold
- [ ] Start normally (not scheduled), Hold enabled, targetKey set -> key stays down until Stop.
- [ ] Start with Hold enabled and `targetKey == Keys.None` -> should NOT set `_isTargetHeldDown`.
- [ ] Schedule start with Hold enabled -> key goes down at scheduled time; inputCountTimer does NOT start.
- [ ] Schedule stop while Hold enabled -> key is released.

### Sequence Hold
- [ ] Sequence with one Hold step -> key goes down, stays down ~DelayMS, then releases.
- [ ] Hold step with mouse (LButton) -> mouse down then up works.
- [ ] Mix Hold and non-hold steps -> ordering is correct.
- [ ] Stop mid-hold -> `ReleaseAllHeldInputs()` releases.
- [ ] Hold step with `Keys.None` -> does not soft-lock the run.

---

## 5) Minimal XML Comment Improvements (NativeInput)
(Only if you want to polish later)

- Add one-liners to `IsMouseKey`, `MouseDown`, `MouseUp`, `KeyDown`, `KeyUp` noting these are thin `SendInput` wrappers.
- Optional: document that input injection can be blocked by focus/elevation/anti-cheat.

