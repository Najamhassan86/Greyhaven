# Interaction System Troubleshooting Guide

## Quick Diagnostic Checklist

Go through each item and check if it's set up correctly:

### ✅ 1. Interactor Component

- [x] **Interactor exists on PlayerFollowCamera**
  
  - Check: Hierarchy → "player and camera" → "PlayerFollowCamera"
  - Inspector should show `Interactor` component
  - **Fix**: Add Component → "Interactor"

- [x] **Interact Range is set**
  
  - In Interactor component, `Interact Range` should be > 0
  - Recommended: **5** (meters)
  - **Fix**: Set to 5 if it's 0 or empty

- [x] **Interact Key is set**
  
  - Should be `E` (KeyCode.E)
  - **Fix**: Change if needed

### ✅ 2. InteractionText Component

- [x] **InteractionText exists in scene**
  
  - Found: "InteractionText" GameObject exists ✅
  - Check: Hierarchy should have a GameObject with `InteractionText` component

- [ ] **InteractionText has textAppear assigned**
  
  - In `InteractionText` component, `Text Appear` field should have a TextMeshProUGUI assigned
  - **Fix**: 
    1. Create a Canvas if you don't have one
    2. Create a TextMeshProUGUI (UI → Text - TextMeshPro)
    3. Assign it to `InteractionText.textAppear`

- [ ] **InteractionText.instance is not null**
  
  - The singleton should initialize in Awake()
  - **Check**: In Play mode, check console for null reference errors

### ✅ 3. Interactable Objects (Keys)

- [ ] **Key has KeyItem component**
  
  - Check: Select your key prefab/instance
  - Inspector should show `KeyItem` component
  - **Fix**: Add Component → "KeyItem"

- [ ] **Key has Collider**
  
  - Required for raycast detection
  - Check: Key should have MeshCollider, BoxCollider, or SphereCollider
  - **Fix**: Add Component → Collider (any type)

- [ ] **Key is active in scene**
  
  - GameObject should be active (checkbox checked)
  - **Fix**: Enable the GameObject

### ✅ 4. Raycast Detection

- [ ] **Looking directly at object**
  
  - Ray shoots from camera forward
  - Must look directly at the key
  - **Test**: Move closer and look directly at key

- [ ] **Object is within range**
  
  - Check `Interact Range` in Interactor
  - Key should be within that distance
  - **Fix**: Increase Interact Range or move closer

- [ ] **No obstacles blocking ray**
  
  - Ray must hit the key's collider
  - Check for walls/objects in between
  - **Fix**: Remove obstacles or adjust position

### ✅ 5. Inventory System

- [ ] **Inventory component exists**
  
  - Should be on "InventoryManager" GameObject
  - **Fix**: Create GameObject → Add Component → "Inventory"

- [ ] **Inventory.instance is not null**
  
  - Check console for errors
  - **Fix**: Make sure Inventory GameObject is active

## Common Issues & Solutions

### Issue 1: "No interaction text appears"

**Symptoms:**

- Looking at key but no text shows
- No "Hold E to pick up..." message

**Possible Causes:**

1. **InteractionText.textAppear is null**
   
   - **Check**: Select InteractionText GameObject → Inspector → `Text Appear` field
   - **Fix**: Create TextMeshProUGUI and assign it

2. **InteractionText.instance is null**
   
   - **Check**: Console for "NullReferenceException"
   - **Fix**: Make sure InteractionText GameObject is active and component is enabled

3. **Text UI is hidden/transparent**
   
   - **Check**: TextMeshProUGUI color/alpha
   - **Fix**: Set color alpha to 255 (fully visible)

4. **Interactor not detecting object**
   
   - **Check**: Interactor `Detected Object` field (in Play mode)
   - **Fix**: See Issue 2 below

### Issue 2: "Ray doesn't detect objects"

**Symptoms:**

- No object detected even when looking at key
- `Detected Object` field stays empty

**Possible Causes:**

1. **No Collider on key**
   
   - **Check**: Select key → Inspector → Should have Collider component
   - **Fix**: Add Component → Mesh Collider or Box Collider

2. **Interact Range too small**
   
   - **Check**: Interactor → `Interact Range` value
   - **Fix**: Increase to 5 or higher

3. **Wrong GameObject has Interactor**
   
   - **Check**: Interactor should be on camera/player that rotates
   - **Fix**: Move Interactor to "PlayerFollowCamera"

4. **Ray hitting wrong object first**
   
   - **Check**: Another object might be blocking
   - **Fix**: Move closer or adjust key position

5. **Key GameObject inactive**
   
   - **Check**: Key GameObject checkbox in Hierarchy
   - **Fix**: Enable the GameObject

### Issue 3: "Interaction text appears but E doesn't work"

**Symptoms:**

- Text shows "Hold E to pick up..."
- But holding E does nothing

**Possible Causes:**

1. **KeyItem component missing**
   
   - **Check**: Key should have `KeyItem` component
   - **Fix**: Add Component → "KeyItem"

2. **Inventory.instance is null**
   
   - **Check**: Console for errors
   - **Fix**: Make sure Inventory GameObject exists and is active

3. **Key already picked up**
   
   - **Check**: Key might be disabled/destroyed
   - **Fix**: Reset scene or place new key

4. **Wrong key name**
   
   - **Check**: KeyItem → `Key Name` should match door requirements
   - **Fix**: Set correct key name

### Issue 4: "NullReferenceException errors"

**Symptoms:**

- Console shows errors like "NullReferenceException"
- Interaction doesn't work

**Common Errors:**

1. **"InteractionText.instance is null"**
   
   - **Fix**: Make sure InteractionText GameObject exists and is active

2. **"InteractionText.instance.textAppear is null"**
   
   - **Fix**: Assign TextMeshProUGUI to `textAppear` field

3. **"Inventory.instance is null"**
   
   - **Fix**: Create InventoryManager GameObject with Inventory component

## Step-by-Step Debug Process

### Step 1: Verify Interactor Setup

```
1. Select "PlayerFollowCamera" in Hierarchy
2. Check Inspector for Interactor component
3. Verify:
   - Interact Range: 5 (or higher)
   - Interact Key: E
4. If missing, add Interactor component
```

### Step 2: Verify InteractionText Setup

```
1. Find "InteractionText" GameObject in Hierarchy
2. Check Inspector for InteractionText component
3. Verify:
   - Text Appear field has TextMeshProUGUI assigned
   - GameObject is active
4. If textAppear is null:
   - Create Canvas (if needed)
   - Create TextMeshProUGUI
   - Assign to InteractionText.textAppear
```

### Step 3: Verify Key Setup

```
1. Select your key GameObject in scene
2. Check Inspector:
   - Has KeyItem component? ✅
   - Has Collider component? ✅
   - GameObject is active? ✅
3. If missing, add components
```

### Step 4: Test in Play Mode

```
1. Press Play
2. Look directly at key
3. Check Interactor component:
   - Detected Object should show the key
4. Check InteractionText:
   - Text should appear
5. Hold E
6. Check console for errors
```

## Debug Tools

### Enable Debug Logging

Add this to Interactor.cs temporarily to see what's happening:

```csharp
private void Update()
{
    Ray r = new Ray(interactorSource.position, interactorSource.forward);
    if (Physics.Raycast(r, out RaycastHit hitInfo, interactRange))
    {
        Debug.Log($"Ray hit: {hitInfo.collider.gameObject.name}");
        detectedObject = hitInfo.collider.gameObject;
        // ... rest of code
    }
    else
    {
        Debug.Log("Ray hit nothing");
    }
}
```

### Visualize Ray in Scene View

1. In Play mode, select "PlayerFollowCamera"
2. In Scene view, you can see the ray direction
3. Check if ray is pointing where you expect

## Quick Fixes

### Fix 1: Complete Setup from Scratch

If nothing works, set up from scratch:

1. **Create InteractionText UI:**
   
   - Canvas → UI → Text - TextMeshPro
   - Name it "InteractionTextUI"
   - Position it (e.g., bottom center)
   - Create GameObject "InteractionText" → Add InteractionText component
   - Assign TextMeshProUGUI to `textAppear` field

2. **Add Interactor:**
   
   - Select "PlayerFollowCamera"
   - Add Component → "Interactor"
   - Set Interact Range: 5

3. **Setup Key:**
   
   - Select key
   - Add Component → "KeyItem"
   - Add Component → Collider (if missing)

4. **Create Inventory:**
   
   - Create GameObject "InventoryManager"
   - Add Component → "Inventory"
   - Add Component → "InventoryUIManager"

### Fix 2: Check Console for Specific Errors

1. Open Console window (Window → General → Console)
2. Play the game
3. Look at key
4. Check for red error messages
5. Fix the specific error shown

## Still Not Working?

If you've checked everything and it still doesn't work:

1. **Check Unity Console** - Look for specific error messages
2. **Verify all scripts compiled** - Check for compilation errors
3. **Test with simple object** - Use InteractionTEST component on a simple cube
4. **Check namespace** - Make sure all scripts are in `EJETAGame` namespace

## Test Object Setup

To test if interaction system works at all:

1. Create a Cube in scene
2. Add Component → "InteractionTEST"
3. Set `Interaction Key` in component
4. Look at cube → Should show "Press [Key] to interact"
5. Press key → Cube should change color

If this works, the system is functional and the issue is with your key setup.
