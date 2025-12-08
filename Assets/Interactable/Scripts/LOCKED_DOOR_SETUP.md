# Locked Door Setup Guide

## Overview

The `LockedDoor` component allows you to create doors that require specific keys to open. It integrates with your existing inventory and interaction systems.

## Component to Add: `LockedDoor`

Add the **`LockedDoor`** component to any door GameObject to make it require a key.

## Step-by-Step Instructions:

### 1. Prepare Your Door GameObject

You can use:
- An existing door prefab (like `door wall.prefab` or `P_Door_01_Base.prefab`)
- A custom door GameObject in your scene
- Any GameObject that represents a door

### 2. Add LockedDoor Component

1. Select the door GameObject (either in scene or prefab)
2. In Inspector, click **"Add Component"**
3. Search for **"LockedDoor"**
4. Add the component

### 3. Configure LockedDoor Settings

In the `LockedDoor` component, you'll see these fields:

#### **Required Key Name** (default: "Key")
- **This is the most important setting!**
- Must match the `keyName` from the `KeyItem` component exactly
- Example: If your key has `keyName = "Rust Key"`, set this to `"Rust Key"`
- Case-sensitive: "Rust Key" ≠ "rust key" ≠ "RUST KEY"

#### **Is Locked** (default: true)
- ✅ **true**: Door requires a key to open
- ❌ **false**: Door can be opened without a key (acts like a regular door)

#### **Consume Key On Use** (default: false)
- ✅ **true**: Key is removed from inventory after opening the door (one-time use)
- ❌ **false**: Key stays in inventory (can reuse the key)

#### **Open Rotation** (default: 90)
- How many degrees the door rotates when opening
- Positive values rotate clockwise, negative rotate counter-clockwise
- Common values: 90° (quarter turn), -90° (reverse quarter turn), 180° (half turn)

#### **Open Speed** (default: 2)
- How fast the door opens/closes (rotation speed multiplier)
- Higher = faster, Lower = slower
- Recommended: 1.0 - 5.0

#### **Door Transform** (optional)
- Leave empty to rotate the GameObject this component is on
- Or assign a child GameObject that should rotate (useful for complex door setups)
- Example: If door has a frame and a door panel, assign the door panel here

### 4. Add Collider (CRITICAL!)

The `Interactor` uses raycasting to detect objects, so your door **MUST have a Collider**:

1. Select the door GameObject
2. Check if it already has a Collider component
3. If not, add one:
   - **Add Component** → **Mesh Collider** (matches the door's mesh shape)
   - OR **Add Component** → **Box Collider** (simpler, faster - recommended)
   - OR **Add Component** → **Capsule Collider** (for rounded doors)

**Important Notes:**
- The collider should be on the same GameObject as `LockedDoor`, or on a child GameObject
- If using a child GameObject for the door panel, make sure that child has a collider
- The collider should NOT be a trigger (Is Trigger = false) for proper raycast detection

### 5. Set Up Door Rotation (Optional)

If your door has a complex hierarchy (frame + door panel):

1. Create an empty child GameObject under your door
2. Name it "Door Panel" or similar
3. Move the door mesh/model to be a child of this GameObject
4. In `LockedDoor` component, assign this child GameObject to **Door Transform**
5. The door panel will rotate while the frame stays stationary

### 6. Save (if working with prefab)

If you're editing a prefab:
1. Click **"Open Prefab"** button again (or press Ctrl+S / Cmd+S) to exit Prefab Mode
2. Click **"Save"** or Unity will auto-save

## Example Configuration:

```
LockedDoor Component:
├── Required Key Name: "Rust Key"
├── Is Locked: ✅ (checked)
├── Consume Key On Use: ❌ (unchecked - key stays in inventory)
├── Open Rotation: 90
├── Open Speed: 2
└── Door Transform: (empty - uses this GameObject)
```

## Matching Keys to Doors

### Example Setup:

**Key Setup:**
- GameObject: `rust_key.prefab`
- Component: `KeyItem`
- Key Name: `"Rust Key"`

**Door Setup:**
- GameObject: `locked_door_01`
- Component: `LockedDoor`
- Required Key Name: `"Rust Key"` ← Must match exactly!

### Multiple Keys and Doors:

You can have multiple keys and doors:

**Key 1:**
- Key Name: `"Rust Key"`
- Opens: Doors with Required Key Name = `"Rust Key"`

**Key 2:**
- Key Name: `"Main Key"`
- Opens: Doors with Required Key Name = `"Main Key"`

**Key 3:**
- Key Name: `"Basement Key"`
- Opens: Doors with Required Key Name = `"Basement Key"`

## How It Works:

1. **Player approaches door** → `Interactor` detects the door
2. **Interaction text appears** → Shows "Press E to unlock door (Requires Rust Key)"
3. **Player presses E**:
   - If player has the key → Door unlocks and opens
   - If player doesn't have the key → Shows "This door requires Rust Key"
4. **Door opens** → Rotates smoothly based on `Open Rotation` and `Open Speed`
5. **Player presses E again** → Door closes

## Additional Requirements:

### For the Locked Door System to Work:

1. ✅ **LockedDoor component** - Added to door GameObject
2. ✅ **Collider component** - Must exist on door (for raycast detection)
3. ✅ **InteractionText** - Must exist in scene (for showing interaction prompts)
4. ✅ **Interactor** - Must be on player (for detecting and interacting)
5. ✅ **Inventory** - Must exist in scene (for checking if player has key)
6. ✅ **KeyItem** - Key must be set up with matching `keyName`

## Testing:

After setting up:

1. **Place door in scene** (or use existing door)
2. **Place key in scene** (make sure key's `keyName` matches door's `Required Key Name`)
3. **Play the game**
4. **Test without key:**
   - Approach door → Should show "Press E to unlock door (Requires [Key Name])"
   - Press E → Should show "This door requires [Key Name]"
5. **Pick up the key** (hold E on key)
6. **Test with key:**
   - Approach door → Should show "Press E to unlock door (Requires [Key Name])"
   - Press E → Door should unlock and open smoothly
   - Press E again → Door should close

## Troubleshooting:

### "Press E" text doesn't appear:
- ✅ Check that `InteractionText` component exists in scene
- ✅ Check that `Interactor` component is on player
- ✅ Check that door has a Collider (not a trigger)
- ✅ Check that door is within `Interactor` range

### Door doesn't open even with key:
- ✅ Check that `Required Key Name` matches the key's `keyName` exactly (case-sensitive!)
- ✅ Check that `Inventory` component exists in scene
- ✅ Check console for errors
- ✅ Verify key was actually added to inventory (press I to check)

### Door opens but doesn't rotate:
- ✅ Check that `Door Transform` is assigned correctly
- ✅ Check that `Open Rotation` is not 0
- ✅ Check that `Open Speed` is not 0
- ✅ If using a child GameObject, make sure it's assigned to `Door Transform`

### Door rotates wrong direction:
- ✅ Change `Open Rotation` from positive to negative (or vice versa)
- ✅ Example: If door opens left when you want it right, change 90 to -90

### Door doesn't close:
- ✅ Press E again while looking at the door
- ✅ Check that door is fully open (wait for rotation to complete)

## Advanced: Multiple Doors with Same Key

If you want multiple doors to use the same key:

1. Set all doors' `Required Key Name` to the same value (e.g., "Rust Key")
2. Set `Consume Key On Use` to **false** (so key stays in inventory)
3. Player can now open all doors with that key

## Advanced: One-Time Use Keys

If you want keys to be consumed after use:

1. Set `Consume Key On Use` to **true**
2. After opening the door, the key is removed from inventory
3. Useful for keys that should only work once

## Tips:

- **Name your keys descriptively**: "Rust Key", "Main Key", "Basement Key" (not just "Key")
- **Test door rotation**: Make sure the door opens in the right direction before finalizing
- **Use child GameObjects**: For complex doors, use a child GameObject for the rotating part
- **Check console**: If something doesn't work, check Unity Console for error messages
