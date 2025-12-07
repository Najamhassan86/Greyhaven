# Inventory Not Showing - Troubleshooting Guide

## Quick Debug Steps

### Step 1: Check Console Logs
When you press **I**, check the Unity Console for these messages:

**✅ Good Signs:**
- `[InventoryUIManager] I key pressed! Toggling inventory...`
- `[InventoryUIManager] ToggleInventory() called. New state: OPEN`
- `[InventoryUIManager] Inventory panel 'InventoryPanel' set to: ACTIVE`
- `[InventoryUIManager] Panel activeInHierarchy: True`

**❌ Bad Signs:**
- `[InventoryUIManager] Inventory Panel is NULL!` → Panel not assigned
- `[InventoryUIManager] Cannot toggle inventory - inventoryPanel is NULL!` → Panel not assigned
- No messages at all → InventoryUIManager component might not be on a GameObject, or script has errors

### Step 2: Check InventoryUIManager Component

1. **Find InventoryManager GameObject** in Hierarchy
2. **Select it** and look at Inspector
3. **Check InventoryUIManager component:**
   - ✅ Toggle Key: **I** (KeyCode 105)
   - ✅ Inventory Panel: **InventoryPanel** (should show GameObject name, not "None")
   - ✅ Standard Core: Should show a GameObject name
   - ✅ Standard Stash View: Should show a GameObject name

**If any field shows "None":**
- Click the circle icon next to the field
- Select the correct GameObject from the picker

### Step 3: Check InventoryPanel GameObject

1. **Find "InventoryPanel"** in Hierarchy (under Canvas)
2. **Check if it's active:**
   - When game is NOT running: Should be **inactive** (checkbox unchecked)
   - When game IS running: Will toggle active/inactive when pressing I

3. **Check if it's visible:**
   - Make sure it's a child of **Canvas**
   - Check RectTransform position and size
   - Check if it's behind other UI elements (check sibling order)

### Step 4: Check Canvas

1. **Find "Canvas"** in Hierarchy
2. **Make sure it's active** (checkbox checked)
3. **Check Render Mode:**
   - Should be **"Screen Space - Overlay"** (recommended)
   - OR **"Screen Space - Camera"** (if using camera)
   - If "World Space", the panel might be positioned incorrectly

### Step 5: Check Input

1. **Make sure you're pressing the correct key:**
   - Default is **I** (capital i, not lowercase L)
   - Check Console to see if key press is detected

2. **Check if other scripts are interfering:**
   - Other scripts might be consuming the I key input
   - Check for Input.GetKeyDown(KeyCode.I) in other scripts

### Step 6: Check Cursor

When inventory opens:
- **Cursor should unlock** (you can move mouse freely)
- **Cursor should be visible** (you can see it)

If cursor doesn't unlock, the inventory might be opening but not visible.

## Common Issues and Fixes

### Issue 1: Panel is NULL
**Symptoms:** Console shows "Inventory Panel is NULL!"
**Fix:**
1. Select InventoryManager GameObject
2. In Inspector → InventoryUIManager component
3. Drag "InventoryPanel" from Hierarchy into "Inventory Panel" field

### Issue 2: Panel is Behind Other UI
**Symptoms:** Panel activates but you can't see it
**Fix:**
1. In Hierarchy, move InventoryPanel to be **last child** of Canvas (bottom of list)
2. Or adjust Canvas sorting order
3. Or make other UI panels inactive when inventory opens

### Issue 3: Panel is Off-Screen
**Symptoms:** Panel activates but not visible
**Fix:**
1. Select InventoryPanel
2. In Inspector → RectTransform
3. Check Anchors: Should be centered (0.5, 0.5)
4. Check Position: Should be (0, 0) if centered
5. Check Size: Should be visible (e.g., 600x700)

### Issue 4: Canvas is Inactive
**Symptoms:** Nothing happens, no UI visible
**Fix:**
1. Select Canvas GameObject
2. Make sure checkbox is **checked** (active)

### Issue 5: Key Not Detected
**Symptoms:** No Console messages when pressing I
**Fix:**
1. Check if InventoryUIManager component is **enabled** (checkbox checked)
2. Check if GameObject with InventoryUIManager is **active**
3. Check Console for script compilation errors
4. Try changing Toggle Key to something else (like P) to test

### Issue 6: Panel Starts Active
**Symptoms:** Panel is visible at game start
**Fix:**
1. Select InventoryPanel in Hierarchy
2. **Uncheck the checkbox** to make it inactive
3. The script will handle toggling it on/off

## Testing Checklist

- [ ] InventoryManager GameObject exists and is active
- [ ] InventoryUIManager component is attached and enabled
- [ ] All references are assigned (no "None" fields)
- [ ] InventoryPanel exists under Canvas
- [ ] InventoryPanel starts inactive (when game not running)
- [ ] Canvas is active
- [ ] Press I key and check Console for debug messages
- [ ] Cursor unlocks when inventory should open
- [ ] Panel becomes active in Hierarchy when I is pressed

## Still Not Working?

1. **Check Console for errors** - Red errors will tell you what's wrong
2. **Check if script compiled** - Look for compilation errors
3. **Try restarting Unity** - Sometimes fixes reference issues
4. **Check if InventoryUIManager GameObject is active** - Inactive GameObjects don't run Update()

## Debug Mode

The InventoryUIManager now has **extensive debug logging**. Make sure:
- **Enable Debug Logs** is checked in Inspector
- Console is open and visible
- Look for `[InventoryUIManager]` messages

These logs will tell you exactly what's happening at each step!

