# Canvas Setup Check - Map_Hosp1.unity

## Current Status

### ✅ What's Already Set Up:

1. **Canvas exists** (fileID: 1948202822)
   - Name: "Canvas"
   - Render Mode: Screen Space - Overlay
   - Has CanvasScaler component
   - Has GraphicRaycaster component
   - Reference Resolution: 1280 x 720
   - Has 5 children

2. **StandardStashView Prefab Instance** (fileID: 1113348089)
   - Exists as a child of Canvas
   - This is the VariableInventorySystem stash view prefab
   - Size: 545 x 610
   - Positioned at center (anchor 0.5, 0.5)

3. **InventoryManager GameObject** (fileID: 2200000000)
   - Has `Inventory` component ✅
   - Has `InventoryUIManager` component ✅
   - `Inventory.useVisualInventory`: 1 (enabled) ✅
   - `Inventory.stashView`: Will be set automatically by InventoryUIManager ✅

4. **InventoryUIManager Component** (fileID: 2200000003)
   - Toggle Key: I (KeyCode 105) ✅
   - Inventory Width: 8 ✅
   - Inventory Height: 6 ✅

### ❌ What's Missing:

1. **InventoryUIManager References are NOT assigned:**
   - `inventoryPanel`: {fileID: 0} ❌ **NEEDS ASSIGNMENT**
   - `standardCore`: {fileID: 0} ❌ **NEEDS ASSIGNMENT**
   - `standardStashView`: {fileID: 0} ❌ **NEEDS ASSIGNMENT**

2. **InventoryPanel GameObject:**
   - No "InventoryPanel" GameObject found
   - Need to create a Panel to contain the inventory UI

3. **StandardCore Component:**
   - No StandardCore GameObject found
   - Need to create and configure StandardCore

## What You Need To Do:

### Step 1: Create InventoryPanel
1. In Unity, select the Canvas
2. Right-click → UI → Panel
3. Name it "InventoryPanel"
4. Set it to inactive initially (uncheck the checkbox in inspector)
5. Position and size it as needed (recommended: 600x700, centered)

### Step 2: Move StandardStashView into InventoryPanel
1. In Hierarchy, drag the "StandardStashView" GameObject
2. Drop it as a child of "InventoryPanel"
3. Adjust its RectTransform to fill the panel or position as desired

### Step 3: Create StandardCore
1. Right-click on "InventoryPanel" → Create Empty
2. Name it "StandardCore"
3. Add Component → Search for "StandardCore" (from VariableInventorySystem)
4. Configure StandardCore:
   - **Cell Prefab**: Drag `StandardCell.prefab` from `VariableInventorySystem/Sample/Resources/`
   - **Case Popup Prefab**: (Optional) Drag `StandardCaseViewPopup.prefab` or leave null
   - **Effect Cell Parent**: Create empty child GameObject, name it "EffectCellParent", add RectTransform
   - **Case Parent**: Create empty child GameObject, name it "CaseParent", add RectTransform

### Step 4: Assign References in InventoryUIManager

**Note:** The `Inventory` component's stashView and stashData will be set automatically by InventoryUIManager, so you don't need to assign them manually.

**In InventoryUIManager Component:**
1. Still on "InventoryManager" GameObject
2. In `InventoryUIManager` component, assign:
   - **Inventory Panel**: Drag the "InventoryPanel" GameObject
   - **Standard Core**: Drag the "StandardCore" GameObject  
   - **Standard Stash View**: Drag the "StandardStashView" GameObject

### Step 5: Verify StandardStashView Configuration
1. Select "StandardStashView" GameObject
2. Check that StandardStashView component has:
   - **Cell Prefab**: `StandardCell.prefab` assigned
   - **Scroll Rect**: Should reference a ScrollRect component
   - **Grid Layout Group**: Should reference a GridLayoutGroup component
   - All other fields properly configured

## Quick Fix Summary:

The main issues are **missing GameObjects and unassigned references**. You need to:

1. ✅ Canvas exists - Good!
2. ✅ StandardStashView exists - Good!
3. ✅ InventoryManager exists with both components - Good!
4. ❌ Create InventoryPanel - Missing
5. ❌ Create StandardCore - Missing  
6. ❌ Assign references in InventoryUIManager component - Not done

**Note:** The `Inventory` component's stashView and stashData are set automatically by InventoryUIManager when it initializes, so no manual assignment needed there.

Once you complete these steps, the inventory system should work!

