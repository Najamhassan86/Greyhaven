# Debug Logging Guide

## Overview

I've added comprehensive debug logging to help troubleshoot why interaction text is not showing. The logs will help you identify exactly where the problem is.

## What Was Added

### 1. **Interactor.cs** - Raycast & Detection Logging
- ✅ Logs when Interactor initializes
- ✅ Logs every raycast hit (every 60 frames to avoid spam)
- ✅ Logs when IInteractable is found
- ✅ Logs when InteractionText is activated/deactivated
- ✅ Logs warnings when InteractionText is null
- ✅ Logs long-press progress
- ✅ Draws ray in Scene view (red line)
- ✅ Logs when ray hits nothing

### 2. **InteractionText.cs** - Text System Logging
- ✅ Logs when instance initializes
- ✅ Logs when textAppear is assigned/missing
- ✅ Logs every time SetText() is called
- ✅ Logs errors when textAppear is null

### 3. **KeyItem.cs** - Key Interaction Logging
- ✅ Logs when OnInteractEnter() is called
- ✅ Logs when OnInteractExit() is called
- ✅ Logs when Interact() is called
- ✅ Logs inventory operations
- ✅ Logs errors when InteractionText is null

## How to Use

### Step 1: Enable Debug Logs

All debug logging is **enabled by default**. You can disable it in Inspector:

1. **For Interactor:**
   - Select "PlayerFollowCamera"
   - In `Interactor` component, find **"Enable Debug Logs"**
   - Uncheck to disable (leave checked for debugging)

2. **For InteractionText:**
   - Select "InteractionText" GameObject
   - In `InteractionText` component, find **"Enable Debug Logs"**
   - Uncheck to disable

3. **For KeyItem:**
   - Select your key prefab/instance
   - In `KeyItem` component, find **"Enable Debug Logs"**
   - Uncheck to disable

### Step 2: Open Console

1. In Unity: **Window → General → Console**
2. Make sure **"Collapse"** is OFF to see all messages
3. Clear console (right-click → Clear)

### Step 3: Play and Observe

1. **Press Play**
2. **Look at a key**
3. **Watch the Console** for debug messages

## What to Look For

### ✅ Good Signs (System Working):

```
[Interactor] Initialized on PlayerFollowCamera
[Interactor] Interact Range: 5
[InteractionText] Instance initialized on InteractionText
[InteractionText] textAppear assigned to: InteractionTextUI
[Interactor] Raycast hit: rust_key at distance 2.34m
[Interactor] Found IInteractable: rust_key (KeyItem)
[KeyItem] OnInteractEnter() called for Rust Key
[InteractionText] Text set to: "Hold E to pick up Rust Key"
[Interactor] Activated interaction text for: rust_key
```

### ❌ Bad Signs (Problems Found):

**Problem 1: No Raycast Hits**
```
[Interactor] Raycast hit nothing. Range: 5m, Position: (x, y, z), Forward: (x, y, z)
```
**Fix**: 
- Check Interact Range is set
- Check you're looking at the key
- Check key has Collider

**Problem 2: InteractionText is Null**
```
[Interactor] InteractionText.instance is NULL! Cannot show interaction text.
```
**Fix**: 
- Create InteractionText GameObject
- Add InteractionText component

**Problem 3: textAppear is Null**
```
[InteractionText] textAppear is NULL! Assign TextMeshProUGUI in Inspector.
[Interactor] InteractionText.instance.textAppear is NULL! Assign TextMeshProUGUI in Inspector.
```
**Fix**: 
- Create TextMeshProUGUI
- Assign to InteractionText.textAppear field

**Problem 4: No IInteractable Found**
```
[Interactor] Raycast hit rust_key but it doesn't have IInteractable component
```
**Fix**: 
- Add KeyItem component to key

**Problem 5: OnInteractEnter Not Called**
```
(No [KeyItem] OnInteractEnter() messages)
```
**Fix**: 
- Check key has KeyItem component
- Check Interactor is detecting the key
- Check key GameObject is active

## Visual Debugging

### Ray Visualization

The Interactor now **draws a red ray** in the Scene view:
1. Select "PlayerFollowCamera" in Hierarchy
2. In `Interactor` component, **"Draw Ray In Scene"** should be checked
3. In Play mode, switch to **Scene view**
4. You'll see a **red line** showing where the ray is shooting
5. The line length = Interact Range

**To see the ray:**
- Play the game
- Switch to Scene view (not Game view)
- Look for red line extending from camera

## Debug Checklist

When testing, check Console for:

- [ ] `[Interactor] Initialized` - Interactor is set up
- [ ] `[InteractionText] Instance initialized` - Text system ready
- [ ] `[Interactor] Raycast hit: [object name]` - Ray is detecting objects
- [ ] `[Interactor] Found IInteractable` - Object has interactable component
- [ ] `[KeyItem] OnInteractEnter()` - Key detected interaction
- [ ] `[InteractionText] Text set to: "..."` - Text is being set
- [ ] `[Interactor] Activated interaction text` - Text UI activated

## Common Debug Scenarios

### Scenario 1: No Logs at All
**Problem**: Scripts not running
**Check**: 
- Are GameObjects active?
- Are components enabled?
- Any compilation errors?

### Scenario 2: Raycast Hits But No IInteractable
```
[Interactor] Raycast hit: rust_key at distance 2.34m
[Interactor] Raycast hit rust_key but it doesn't have IInteractable component
```
**Problem**: Key missing KeyItem component
**Fix**: Add KeyItem component to key

### Scenario 3: IInteractable Found But No Text
```
[Interactor] Found IInteractable: rust_key (KeyItem)
[KeyItem] OnInteractEnter() called for Rust Key
[InteractionText] Text set to: "Hold E to pick up Rust Key"
[Interactor] InteractionText.instance.textAppear is NULL!
```
**Problem**: textAppear not assigned
**Fix**: Assign TextMeshProUGUI to InteractionText.textAppear

### Scenario 4: Text Set But Not Visible
```
[InteractionText] Text set to: "Hold E to pick up Rust Key"
[Interactor] Activated interaction text for: rust_key
```
**But text doesn't appear on screen**
**Problem**: Text UI might be:
- Hidden/transparent
- Behind other UI
- Wrong Canvas settings
- Text color same as background

## Performance Note

Debug logs are throttled to avoid spam:
- Raycast hits: Every 60 frames (~1 second at 60fps)
- No hits: Every 120 frames (~2 seconds)
- Long-press progress: Every 30 frames (~0.5 seconds)

This keeps console readable while still providing useful feedback.

## Disabling Debug Logs

Once everything works, you can disable debug logs:
1. Select each GameObject with debug components
2. Uncheck "Enable Debug Logs" in Inspector
3. This improves performance slightly

## Next Steps

1. **Play the game** with Console open
2. **Look at a key**
3. **Read the Console messages**
4. **Identify which step is failing**
5. **Fix the specific issue** based on the error message

The debug logs will tell you exactly where the problem is!

