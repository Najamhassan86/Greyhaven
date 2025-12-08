# Hammer System Setup Guide

## Overview

The hammer system allows players to destroy doors when they can't find the key. The hammer is a one-time use item that gets consumed when used.

## Components

1. **HammerItem** - Script for the hammer pickup
2. **DestructibleDoor** - Script for doors that can be destroyed

## Step 1: Set Up the Hammer Prefab

### 1.1 Add HammerItem Component

1. Open your hammer prefab (e.g., `Hammer.prefab` from Tom Wood asset)
2. Select the root GameObject
3. **Add Component** → Search for **"HammerItem"**
4. Add the component

### 1.2 Configure HammerItem Settings

- **Hammer Name** (default: "Hammer")
  - The unique identifier for this hammer
  - Must match the `requiredHammerName` in `DestructibleDoor`
  - Example: "Hammer", "Heavy Hammer", etc.

- **Destroy On Pickup** (default: true)
  - ✅ **true**: Hammer GameObject is destroyed after pickup (recommended)
  - ❌ **false**: Hammer GameObject is disabled but remains in scene

- **Required Hold Time** (default: 1.5)
  - How long the player must hold E to pick up the hammer (in seconds)

- **Image Path** (default: "Image/hammer")
  - Path to the hammer's image sprite for the visual inventory
  - Must be in `Assets/Resources/Image/` folder
  - Example: "Image/hammer", "Image/hammer_icon", etc.

### 1.3 Add Collider

The hammer **MUST have a Collider** for interaction detection:

1. Select the hammer GameObject
2. Check if it already has a Collider component
3. If not, add one:
   - **Add Component** → **Mesh Collider** (matches the hammer's mesh shape)
   - OR **Add Component** → **Box Collider** (simpler, faster - recommended)
   - OR **Add Component** → **Capsule Collider**

### 1.4 Create Hammer Inventory Image

1. Create or find a hammer icon/sprite
2. Place it in `Assets/Resources/Image/hammer.png`
3. Set Texture Type to "Sprite (2D and UI)" in import settings
4. Update `Image Path` in HammerItem if you use a different name

## Step 2: Set Up Destructible Doors

### 2.1 Add DestructibleDoor Component

1. Select the door GameObject you want to make destructible
2. **Add Component** → Search for **"DestructibleDoor"**
3. Add the component

**Note**: You can add this to the same door that has `LockedDoor` - they work together!

### 2.2 Configure DestructibleDoor Settings

- **Required Hammer Name** (default: "Hammer")
  - Must match the `hammerName` from the `HammerItem` component exactly
  - Case-sensitive: "Hammer" ≠ "hammer"

- **Destroy Key** (default: H)
  - Key to press to destroy the door
  - Change if needed (e.g., KeyCode.X)

- **Destroy Range** (default: 3)
  - How close the player needs to be to destroy the door (in meters)
  - Recommended: 2-5 meters

- **Destroy On Use** (default: true)
  - ✅ **true**: Door GameObject is destroyed after hammer use
  - ❌ **false**: Door GameObject is disabled but remains in scene

- **Destruction Effect** (optional)
  - Particle effect or debris GameObject to spawn when door is destroyed
  - Leave empty if you don't want effects

- **Destruction Sound** (optional)
  - AudioClip to play when door is destroyed
  - Leave empty if you don't want sound

## How It Works

1. **Player finds hammer** → Holds E to pick it up → Hammer added to inventory
2. **Player approaches locked door** → Sees "Press H to destroy door with Hammer"
3. **Player presses H** → Door is destroyed, hammer is consumed
4. **Player can now walk through** → Door is gone forever

## Example Setup

### Hammer Setup:
- GameObject: `Hammer.prefab`
- Component: `HammerItem`
- Hammer Name: `"Hammer"`
- Image Path: `"Image/hammer"`

### Door Setup:
- GameObject: `locked_door_01`
- Component: `LockedDoor` (for key-based locking)
- Component: `DestructibleDoor` (for hammer destruction)
- Required Hammer Name: `"Hammer"` ← Must match exactly!

## Multiple Hammers

You can have multiple hammers with different names:

- **Hammer 1**: `hammerName = "Hammer"` → Destroys doors with `requiredHammerName = "Hammer"`
- **Hammer 2**: `hammerName = "Heavy Hammer"` → Destroys doors with `requiredHammerName = "Heavy Hammer"`

## Testing

1. **Place hammer in scene** (or use prefab)
2. **Place destructible door in scene**
3. **Play the game**
4. **Pick up hammer** (hold E on hammer)
5. **Approach door** → Should show "Press H to destroy door with Hammer"
6. **Press H** → Door should be destroyed, hammer removed from inventory
7. **Walk through** → Door should be gone

## Troubleshooting

### "Press H" text doesn't appear:
- ✅ Check that `InteractionText` component exists in scene
- ✅ Check that player is within `Destroy Range`
- ✅ Check that player has the hammer in inventory (press I to check)

### Door doesn't destroy:
- ✅ Check that `Required Hammer Name` matches the hammer's `Hammer Name` exactly (case-sensitive!)
- ✅ Check that `Inventory` component exists in scene
- ✅ Check console for errors
- ✅ Verify hammer was actually added to inventory (press I to check)

### Hammer doesn't get picked up:
- ✅ Check that `Inventory` component exists in scene
- ✅ Check that hammer has a Collider
- ✅ Check console for errors

### Hammer doesn't appear in inventory:
- ✅ Check that visual inventory is set up
- ✅ Check that hammer image exists at the specified path
- ✅ Check console for errors

## Tips

- **Place hammers strategically**: Hide them in areas where players might get stuck
- **Use sparingly**: Since hammers are one-time use, make them rare
- **Combine with locked doors**: Doors can be both locked (need key) AND destructible (can use hammer)
- **Add effects**: Create particle effects for door destruction for better feedback
