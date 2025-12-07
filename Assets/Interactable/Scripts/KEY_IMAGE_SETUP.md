# Key Image Setup Guide

Since your Rust Key asset doesn't have a dedicated inventory sprite, here are **3 solutions**:

## Solution 1: Use Existing Texture (Easiest) ⭐ RECOMMENDED

Use the `rust_key_Albedo.png` texture that already exists in your asset.

### Steps:

1. **Create Resources Folder Structure:**
   - In Unity, right-click in `Assets` folder
   - Create → Folder → Name it **"Resources"**
   - Right-click `Resources` → Create → Folder → Name it **"Image"**

2. **Copy the Albedo Texture:**
   - Navigate to `Assets/Rust Key/Textures/rust_key_Albedo.png`
   - **Copy** the file (Ctrl+C / Cmd+C)
   - **Paste** it into `Assets/Resources/Image/` (Ctrl+V / Cmd+V)
   - Rename it to `key.png` (or keep as `rust_key_Albedo.png`)

3. **Configure Texture Import Settings:**
   - Select the copied texture in `Resources/Image/`
   - In Inspector, set:
     - **Texture Type**: Sprite (2D and UI)
     - **Sprite Mode**: Single
     - Click **"Apply"**

4. **Update KeyItem Component:**
   - In your `rust_key.prefab`, find the `KeyItem` component
   - Set **Image Path** to:
     - `"Image/key"` (if you renamed it to key.png)
     - OR `"Image/rust_key_Albedo"` (if you kept original name)
   - **Note**: Don't include the `.png` extension!

## Solution 2: Create a Simple Icon Sprite

If the Albedo texture is too detailed or doesn't look good in inventory:

1. **Create a Simple Sprite:**
   - In Unity: Right-click `Assets/Resources/Image/` → Create → 2D → Sprites → Square
   - Name it `key.png`
   - Or use any image editor to create a simple key icon (64x64 or 128x128 pixels)

2. **Use the Sprite:**
   - Set **Image Path** in KeyItem to `"Image/key"`

## Solution 3: Use a Placeholder (Quick Test)

For quick testing, you can use a placeholder:

1. **Create a Simple Colored Square:**
   - In Unity: Right-click `Assets/Resources/Image/` → Create → 2D → Sprites → Square
   - Name it `key.png`
   - In Inspector, you can change the color if needed

2. **Set Image Path:**
   - In KeyItem component: `"Image/key"`

## Important Notes:

### Path Format:
- ✅ **Correct**: `"Image/key"` (no extension, no leading slash)
- ❌ **Wrong**: `"Image/key.png"`
- ❌ **Wrong**: `"/Image/key"`
- ❌ **Wrong**: `"Assets/Resources/Image/key"`

### Folder Structure:
```
Assets/
└── Resources/          ← Must be named exactly "Resources"
    └── Image/          ← Your folder name
        └── key.png     ← Your image file
```

### Texture Import Settings:
For inventory sprites, recommended settings:
- **Texture Type**: Sprite (2D and UI)
- **Max Size**: 256 or 512 (depending on quality needed)
- **Compression**: None or Low (for clarity)
- **Filter Mode**: Bilinear or Point (Point for pixel art)

## Quick Setup Checklist:

- [ ] Create `Assets/Resources/Image/` folder
- [ ] Copy `rust_key_Albedo.png` to `Resources/Image/`
- [ ] Rename to `key.png` (optional)
- [ ] Set Texture Type to "Sprite (2D and UI)"
- [ ] Set Image Path in KeyItem to `"Image/key"`
- [ ] Test in game (press I to open inventory)

## Troubleshooting:

**Image doesn't appear in inventory:**
- Check that file is in `Assets/Resources/Image/` (exact path)
- Check that path in KeyItem doesn't include `.png` extension
- Check that texture is set to "Sprite (2D and UI)" type
- Check console for errors

**Image appears blurry:**
- Increase Max Size in texture import settings
- Set Compression to None
- Use higher resolution source image

**"Resources folder not found" error:**
- Make sure folder is named exactly "Resources" (capital R)
- Must be directly under Assets folder
- Unity will create a special icon on the folder when correct

## Alternative: Use Sprite Atlas (Advanced)

If you have many items, you can use a Sprite Atlas:
1. Create Sprite Atlas (2D → Sprite Atlas)
2. Add all item sprites to the atlas
3. Use path: `"AtlasName/spriteName"` (check VariableInventorySystem docs)

## Recommended Approach:

**For now, use Solution 1** (copy rust_key_Albedo.png):
- ✅ Uses existing asset
- ✅ Quick to set up
- ✅ Looks good in inventory
- ✅ Can refine later if needed

