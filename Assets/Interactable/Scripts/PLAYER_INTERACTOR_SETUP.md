# Player Interactor Setup Guide

## Overview

The `Interactor` component casts a ray forward from the player/camera to detect interactable objects. Here's how to set it up.

## Where to Add Interactor

The `Interactor` should be on the GameObject that **rotates when you look around**. This is typically:

- **Option 1: Camera GameObject** ⭐ RECOMMENDED (for FPS games)
  - If your camera rotates independently (like in FPS games)
  - The ray will shoot forward from where you're looking

- **Option 2: Player GameObject** (for third-person games)
  - If the player body rotates with camera
  - The ray will shoot forward from player's facing direction

## Step-by-Step Setup:

### Step 1: Find Your Player/Camera

1. Open your scene (`Map_Hosp1.unity`)
2. In Hierarchy, find your player GameObject
   - Look for names like: "Player", "PlayerCapsule", "player and camera", "Main Camera", etc.
3. If you have a separate camera, find that too

### Step 2: Add Interactor Component

**For FPS Setup (Camera-based):**
1. Select your **Camera GameObject** (usually "Main Camera" or child of player)
2. In Inspector, click **"Add Component"**
3. Search for **"Interactor"**
4. Add the component

**For Third-Person Setup (Player-based):**
1. Select your **Player GameObject**
2. In Inspector, click **"Add Component"**
3. Search for **"Interactor"**
4. Add the component

### Step 3: Configure Interactor Settings

In the `Interactor` component, you'll see:

- **Interact Range** (default: needs to be set)
  - How far the interaction ray can reach
  - Recommended: **3-5 meters** for keys/doors
  - Set to `5` for a good default

- **Interact Key** (default: E)
  - The key to press for interactions
  - Default is `E` (KeyCode.E)
  - Change if you want a different key

- **Detected Object** (read-only)
  - Shows what object is currently being detected
  - Useful for debugging

### Step 4: Verify Setup

The `Interactor` uses:
- `transform.position` - Where the ray starts
- `transform.forward` - Direction the ray shoots

**Important**: Make sure the GameObject you added `Interactor` to:
- ✅ Rotates when you look around (mouse look)
- ✅ Is positioned where you want the ray to start (usually camera position)

## Common Player Setups:

### Setup A: First-Person Controller (FPS)
```
Player (root)
├── Camera (Main Camera)
│   └── Interactor ← ADD HERE
└── Other components...
```

### Setup B: Third-Person Controller
```
Player (root)
├── Interactor ← ADD HERE
├── Camera (child or separate)
└── Other components...
```

### Setup C: Simple Movement Script
If you're using `SimpleMovement.cs`:
- The camera rotates with the player
- Add `Interactor` to the **Player GameObject** (same one with SimpleMovement)

## Required Dependencies:

The `Interactor` needs these to work:

1. ✅ **InteractionText** - Must exist in scene
   - Shows "Hold E to pick up..." messages
   - Should be set up already

2. ✅ **Physics Raycast** - Works automatically
   - Objects need Colliders to be detected
   - Make sure keys/doors have colliders

## Testing:

After setup:
1. Play the game
2. Look at a key (with KeyItem component)
3. You should see: "Hold E to pick up Rust Key"
4. Hold E → Key should be picked up

## Troubleshooting:

**No interaction text appears:**
- Check that `InteractionText` component exists in scene
- Check that `InteractionText.instance` is not null
- Check console for errors

**Ray doesn't detect objects:**
- Check that objects have Colliders
- Check that `Interact Range` is set (not 0)
- Check that you're looking directly at the object
- Verify Interactor is on the GameObject that rotates

**Interactor on wrong GameObject:**
- If ray shoots in wrong direction, move Interactor to Camera
- If ray doesn't rotate, move Interactor to rotating GameObject

## Quick Setup Checklist:

- [ ] Find Player/Camera GameObject
- [ ] Add `Interactor` component
- [ ] Set `Interact Range` to 5 (or desired range)
- [ ] Verify `Interact Key` is E (or change if needed)
- [ ] Test by looking at a key
- [ ] Check that interaction text appears

## Example Configuration:

```
Interactor Component:
├── Interact Range: 5
├── Interact Key: E (KeyCode.E)
└── Detected Object: (shows current target)
```

## Notes:

- The `interactorSource` is automatically set to the GameObject's transform
- The ray shoots from the GameObject's position in its forward direction
- Works with both regular interactions (press E) and long-press (hold E)
- Automatically detects if an object needs long-press

