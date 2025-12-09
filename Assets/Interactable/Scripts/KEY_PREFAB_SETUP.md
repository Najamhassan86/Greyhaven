# Key Prefab Setup Guide

## Component to Add: `KeyItem`

Add the **`KeyItem`** component to your `rust_key.prefab` to make it interactable.

## Step-by-Step Instructions:

### 1. Open Your Key Prefab
1. Navigate to `Assets/Rust Key/Prefabs/rust_key.prefab`
2. Double-click to open it in Prefab Mode (or select it and click "Open Prefab" in Inspector)

### 2. Add KeyItem Component
1. Select the root GameObject of the prefab
2. In Inspector, click **"Add Component"**
3. Search for **"KeyItem"**
4. Add the component

### 3. Configure KeyItem Settings

In the `KeyItem` component, you'll see these fields:

- **Key Name** (default: "Rust Key")
  - This is the unique identifier for this key
  - **Important**: This must match the `requiredKeyName` in any `LockedDoor` that uses this key
  - Example: "Rust Key", "Main Key", "Basement Key", etc.

- **Destroy On Pickup** (default: true)
  - ✅ **true**: Key GameObject is destroyed after pickup (recommended)
  - ❌ **false**: Key GameObject is disabled but remains in scene

- **Required Hold Time** (default: 1.5)
  - How long the player must hold E to pick up the key (in seconds)
  - Recommended: 1.0 - 2.0 seconds

- **Image Path** (default: "Image/key")
  - Path to the key's image sprite for the visual inventory
  - Must be in `Assets/Resources/Image/` folder
  - Example: "Image/key", "Image/rust_key", etc.

### 4. Add Collider (IMPORTANT!)

The `Interactor` uses raycasting to detect objects, so your key **MUST have a Collider**:

1. Select the key GameObject in the prefab
2. Check if it already has a Collider component
3. If not, add one:
   - **Add Component** → **Mesh Collider** (matches the key's mesh shape)
   - OR **Add Component** → **Box Collider** (simpler, faster)
   - OR **Add Component** → **Sphere Collider** (if key is roughly spherical)

**Note**: If your key prefab already has a collider (for physics), that's fine! The interaction system will use it.

### 5. Save the Prefab

1. Click **"Open Prefab"** button again (or press Ctrl+S / Cmd+S) to exit Prefab Mode
2. Click **"Save"** or Unity will auto-save

## Example Configuration:

```
KeyItem Component:
├── Key Name: "Rust Key"
├── Destroy On Pickup: ✅ (checked)
├── Required Hold Time: 1.5
└── Image Path: "Image/key"
```

## Additional Requirements:

### For the Interaction System to Work:

1. ✅ **KeyItem component** - Added to prefab
2. ✅ **Collider component** - Must exist (for raycast detection)
3. ✅ **InteractionText** - Must exist in scene (for showing "Hold E to pick up...")
4. ✅ **Interactor** - Must be on player (for detecting and interacting)
5. ✅ **Inventory** - Must exist in scene (for storing the key)

### For Visual Inventory:

1. ✅ **Key image sprite** - Place in `Assets/Resources/Image/key.png`
   - Or update `Image Path` to match your sprite location
   - Example: If sprite is `key_sprite.png`, set path to `"Image/key_sprite"`

## Testing:

After setting up:
1. Place the key prefab in your scene
2. Play the game
3. Look at the key → Should show "Hold E to pick up Rust Key"
4. Hold E for 1.5 seconds → Key should be added to inventory
5. Press I → Should see key in visual inventory

## Troubleshooting:

**"Hold E" text doesn't appear:**
- Check that `InteractionText` component exists in scene
- Check that `Interactor` component is on player
- Check that key has a Collider

**Key doesn't get picked up:**
- Check that `Inventory` component exists in scene
- Check console for errors
- Verify `Key Name` is set correctly

**Key doesn't appear in inventory:**
- Check that visual inventory is set up (InventoryPanel, StandardCore, etc.)
- Check that key image exists at the specified path
- Check console for errors

## Multiple Keys:

If you have multiple different keys:
- Each key prefab needs its own `KeyItem` component
- Each key should have a **unique** `Key Name`
- Example:
  - Key 1: "Rust Key"
  - Key 2: "Main Key"
  - Key 3: "Basement Key"

