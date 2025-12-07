# Your Player Setup - Map_Hosp1.unity

## Found GameObjects:

### 1. **"player and camera"** (Parent GameObject)
   - **Location**: Root of scene hierarchy
   - **Position**: (-11.26, 1.51, -0.51)
   - **Children**:
     - PlayerCapsule
     - PlayerFollowCamera ⭐ **THIS IS YOUR CAMERA**
     - (one more child)

### 2. **"PlayerFollowCamera"** (Camera GameObject)
   - **Location**: Child of "player and camera"
   - **Type**: Third-person follow camera
   - **This is what rotates when you look around**

## Where to Add Interactor:

Since you have a **"PlayerFollowCamera"**, this is a third-person setup. The camera rotates independently to follow the player.

### ⭐ RECOMMENDED: Add to "PlayerFollowCamera"

**Why?**
- The camera rotates when you look around
- The interaction ray should shoot from where you're looking
- This gives the most accurate interaction detection

## Step-by-Step Instructions:

### Step 1: Open Your Scene
1. Open `Map_Hosp1.unity` in Unity
2. In Hierarchy, expand **"player and camera"**
3. Find **"PlayerFollowCamera"** (it's a child)

### Step 2: Add Interactor to Camera
1. **Select "PlayerFollowCamera"** in Hierarchy
2. In Inspector, click **"Add Component"**
3. Search for **"Interactor"**
4. Click to add it

### Step 3: Configure Interactor
In the `Interactor` component that just appeared:

1. **Set Interact Range:**
   - Find the **"Interact Range"** field
   - Set it to **5** (or your preferred distance in meters)
   - This is how far you can interact with objects

2. **Verify Interact Key:**
   - **"Interact Key"** should be **E** (KeyCode.E)
   - This is correct, no need to change unless you want a different key

### Step 4: Verify Setup
The `Interactor` component should now show:
- ✅ **Interact Range**: 5 (or your value)
- ✅ **Interact Key**: E
- **Detected Object**: (will show what you're looking at during play)

## Alternative: Add to "player and camera" (If Camera Doesn't Rotate)

If your camera doesn't rotate independently (unlikely with "PlayerFollowCamera"), you can add it to the parent:

1. Select **"player and camera"** GameObject
2. Add **Interactor** component
3. Configure the same way

**But try the camera first!** It's almost certainly the right choice.

## Testing:

After adding Interactor:

1. **Play the game** (Press Play)
2. **Look at a key** (one with KeyItem component)
3. **You should see**: "Hold E to pick up Rust Key"
4. **Hold E** for 1.5 seconds
5. **Key should be picked up** and added to inventory

## Quick Checklist:

- [ ] Open Map_Hosp1.unity
- [ ] Expand "player and camera" in Hierarchy
- [ ] Select "PlayerFollowCamera"
- [ ] Add Component → "Interactor"
- [ ] Set Interact Range to 5
- [ ] Verify Interact Key is E
- [ ] Test in Play mode

## Troubleshooting:

**Interaction text doesn't appear:**
- Make sure `InteractionText` component exists in scene
- Check console for errors
- Verify you're looking directly at the key

**Ray doesn't detect objects:**
- Check that objects have Colliders
- Increase Interact Range if objects are too far
- Make sure you're looking at the object (camera forward direction)

**Wrong interaction direction:**
- If ray shoots in wrong direction, try adding Interactor to "player and camera" instead
- Or check camera rotation settings

## Your Scene Structure:

```
Scene Root
└── player and camera
    ├── PlayerCapsule (player body)
    ├── PlayerFollowCamera ⭐ ADD INTERACTOR HERE
    └── (other children)
```

## Summary:

**Add `Interactor` component to: "PlayerFollowCamera"**

This will make the interaction ray shoot forward from where the camera is looking, which is perfect for your third-person setup!

