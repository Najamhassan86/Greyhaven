# Visual Inventory UI Setup Guide

This is a step-by-step guide to set up the VariableInventorySystem visual inventory UI.

## Prerequisites

Make sure you have:
- VariableInventorySystem assets in your project
- Prefabs available in `VariableInventorySystem/Sample/Resources/`:
  - `StandardCell.prefab`
  - `StandardStashView.prefab` (optional, we'll create our own)
  - `StandardCaseViewPopup.prefab` (optional, for containers)

## Step-by-Step Setup

### Step 1: Create Canvas and Panel

1. **Create Canvas:**
   - Right-click in Hierarchy → UI → Canvas
   - Name it "InventoryCanvas"
   - Set Canvas Scaler:
     - UI Scale Mode: Scale With Screen Size
     - Reference Resolution: 1920 x 1080
     - Screen Match Mode: Match Width Or Height (0.5)

2. **Create Inventory Panel:**
   - Right-click on Canvas → UI → Panel
   - Name it "InventoryPanel"
   - Set RectTransform:
     - Anchor: Center
     - Width: 600
     - Height: 700
   - Set Image component:
     - Color: Dark semi-transparent (e.g., RGBA: 0, 0, 0, 200)

### Step 2: Set Up StandardStashView

1. **Create StashView GameObject:**
   - Right-click on InventoryPanel → Create Empty
   - Name it "StandardStashView"
   - Add Component: `StandardStashView` (from VariableInventorySystem)

2. **Set Up ScrollRect:**
   - Right-click on StandardStashView → UI → Scroll View
   - Name it "ScrollView"
   - Set RectTransform:
     - Anchor: Stretch-Stretch
     - Left/Right/Top/Bottom: 10
   - Remove the default Content child (we'll create our own)

3. **Create Content Area:**
   - Right-click on ScrollView → Create Empty
   - Name it "Content"
   - Add Component: `RectTransform`
   - Add Component: `Grid Layout Group`
   - Add Component: `Content Size Fitter` (Vertical: Preferred Size)
   - Set Grid Layout Group:
     - Cell Size: 64 x 64 (or match your cell prefab size)
     - Spacing: 2 x 2
     - Constraint: Fixed Column Count (will be set by script)

4. **Create Background:**
   - Right-click on Content → UI → Image
   - Name it "Background"
   - Set Image:
     - Color: Very dark (e.g., RGBA: 20, 20, 20, 255)
   - Set RectTransform to fill Content

5. **Create Condition Indicator (for drag feedback):**
   - Right-click on StandardStashView → UI → Image
   - Name it "ConditionIndicator"
   - Set Image:
     - Color: White, Alpha: 0
   - Set RectTransform:
     - Size: 64 x 64
     - Initially inactive or transparent

6. **Configure StandardStashView Component:**
   - Cell Prefab: Drag `StandardCell.prefab` from `VariableInventorySystem/Sample/Resources/`
   - Scroll Rect: Drag the ScrollView GameObject
   - Grid Layout Group: Drag the Content GameObject
   - Condition: Drag the ConditionIndicator GameObject
   - Condition Transform: Drag the ConditionIndicator GameObject
   - Background: Drag the Background GameObject
   - Hold Scroll Padding: 50
   - Hold Scroll Rate: 10
   - Default Color: White (255, 255, 255, 100)
   - Positive Color: Green (0, 255, 0, 100)
   - Negative Color: Red (255, 0, 0, 100)

### Step 3: Set Up StandardCore

1. **Create Core GameObject:**
   - Right-click on InventoryPanel → Create Empty
   - Name it "StandardCore"
   - Add Component: `StandardCore` (from VariableInventorySystem)
   - Add Component: `Event System` (if not already in scene)
   - Add Component: `GraphicRaycaster` (for UI interaction)

2. **Create Effect Cell Parent:**
   - Right-click on StandardCore → Create Empty
   - Name it "EffectCellParent"
   - Add Component: `RectTransform`
   - This is where dragged items appear

3. **Create Case Parent (Optional):**
   - Right-click on StandardCore → Create Empty
   - Name it "CaseParent"
   - Add Component: `RectTransform`
   - This is for container popups (optional)

4. **Configure StandardCore Component:**
   - Cell Prefab: Drag `StandardCell.prefab` from `VariableInventorySystem/Sample/Resources/`
   - Case Popup Prefab: Drag `StandardCaseViewPopup.prefab` (optional, can be null)
   - Effect Cell Parent: Drag the EffectCellParent GameObject
   - Case Parent: Drag the CaseParent GameObject (or leave null)

### Step 4: Connect to InventoryUIManager

1. **Find or Create InventoryManager:**
   - Find the GameObject with `Inventory` and `InventoryUIManager` components
   - If it doesn't exist, create it:
     - Create Empty GameObject → Name it "InventoryManager"
     - Add Component: `Inventory`
     - Add Component: `InventoryUIManager`

2. **Configure InventoryUIManager:**
   - Toggle Key: I (default)
   - Inventory Panel: Drag the InventoryPanel GameObject
   - Standard Core: Drag the StandardCore GameObject
   - Standard Stash View: Drag the StandardStashView GameObject
   - Inventory Width: 8 (default)
   - Inventory Height: 6 (default)

### Step 5: Create Key Image Resources

1. **Create Resources Folder:**
   - In Assets, create: `Resources/Image/` (if it doesn't exist)

2. **Add Key Image:**
   - Place your key sprite/image in `Assets/Resources/Image/`
   - Name it `key.png` (or update `imagePath` in KeyItem components)

## Quick Setup Alternative

If you prefer, you can use the provided `StandardStashView.prefab` from the sample:

1. Drag `StandardStashView.prefab` into your InventoryPanel
2. Set up StandardCore as described above
3. Connect references in InventoryUIManager

## Testing

1. Play the scene
2. Look at a key → Hold E to pick it up
3. Press I → Inventory should open showing the key
4. Press I again → Inventory should close

## Troubleshooting

**Inventory doesn't open:**
- Check that InventoryPanel is assigned in InventoryUIManager
- Check that the Canvas is active

**Keys don't appear in inventory:**
- Check that StandardCore and StandardStashView are assigned
- Check that StandardCell prefab is assigned
- Check console for errors

**Can't interact with inventory:**
- Make sure EventSystem exists in scene
- Check that GraphicRaycaster is on the Canvas

**Images don't load:**
- Ensure key images are in `Assets/Resources/Image/`
- Check that image path in KeyItem matches your file name

