# Inventory Image Not Showing - Fix Guide

## Problem
Inventory cells show green/white color instead of the actual item images.

## Common Causes

### 1. **Texture Import Settings** (Most Common)

The images need to be imported with correct settings:

1. **Select your image** in `Assets/Resources/Image/` (e.g., `key.png`)
2. **In Inspector**, check **Texture Import Settings**:
   - **Texture Type**: Should be **"Default"** (NOT "Sprite (2D and UI)")
   - **sRGB (Color Texture)**: ✅ Checked
   - **Read/Write Enabled**: ✅ Checked (if needed)
   - **Max Size**: 512 or higher (for clarity)
   - Click **"Apply"**

**Why "Default" instead of "Sprite"?**
- The inventory system uses `RawImage` which needs `Texture2D`
- "Sprite" type is optimized for `Image` components, not `RawImage`
- The system can convert Sprites, but "Default" works better

### 2. **Image Path is Wrong**

Check that the path in your item components matches the file:

- ✅ **Correct**: `"Image/key"` (file: `Assets/Resources/Image/key.png`)
- ❌ **Wrong**: `"Image/key.png"` (includes extension)
- ❌ **Wrong**: `"/Image/key"` (leading slash)
- ❌ **Wrong**: `"key"` (missing folder)

### 3. **File Doesn't Exist**

Verify the file exists:
- Location: `Assets/Resources/Image/key.png`
- Folder must be named exactly **"Resources"** (capital R)
- Subfolder can be any name (e.g., "Image")

### 4. **Image Format Issues**

Make sure your image:
- Is a valid image format (PNG, JPG, etc.)
- Is not corrupted
- Has reasonable dimensions (64x64 to 512x512 recommended)

## Quick Fix Steps

### Step 1: Check Image Import Settings

1. Select `Assets/Resources/Image/key.png`
2. In Inspector → **Texture Type**: Change to **"Default"**
3. Click **"Apply"**
4. Repeat for `hammer.png` if you have it

### Step 2: Verify Paths

1. Check `KeyItem` component on your key prefab
2. **Image Path** should be: `"Image/key"` (no .png)
3. Check `HammerItem` component on your hammer prefab
4. **Image Path** should be: `"Image/hammer"` (no .png)

### Step 3: Test with Diagnostic Tool

1. Create empty GameObject in scene
2. Add Component → `InventoryImageDiagnostic`
3. Play the game
4. Check Console for diagnostic messages
5. Right-click component → **"Test All Images"** for detailed info

### Step 4: Check Console

When you add items to inventory, check Unity Console for:
- ✅ `[StandardAssetLoader] Successfully loaded...` = Good!
- ❌ `[StandardAssetLoader] Failed to load...` = Problem!

## Advanced: Manual Texture Check

If images still don't work, test manually:

```csharp
//In Unity Console or a test script:
Texture2D test = Resources.Load<Texture2D>("Image/key");
Debug.Log($"Texture: {test != null}, Size: {(test != null ? $"{test.width}x{test.height}" : "NULL")}");
```

## Still Not Working?

1. **Check Console** - Look for error messages from `[StandardAssetLoader]` or `[StandardCell]`
2. **Verify Resources folder** - Must be exactly `Assets/Resources/` (Unity recognizes this special folder)
3. **Reimport images** - Right-click image → **Reimport**
4. **Check RawImage component** - In the inventory cell prefab, make sure `RawImage` component exists and is configured
5. **Test with simple image** - Create a simple colored square sprite to test if the system works at all

## Expected Behavior

When working correctly:
- ✅ Console shows: `[StandardAssetLoader] Successfully loaded Texture2D...`
- ✅ Console shows: `[StandardCell] ✓ Successfully loaded texture for cell...`
- ✅ Inventory cell shows the actual image (not green/white)
- ✅ Image is clear and not pixelated

## Common Mistakes

1. ❌ Using "Sprite (2D and UI)" texture type → Use "Default"
2. ❌ Including `.png` in path → Use `"Image/key"` not `"Image/key.png"`
3. ❌ Wrong folder name → Must be `Resources` (capital R)
4. ❌ Image not in Resources folder → Must be in `Assets/Resources/`
