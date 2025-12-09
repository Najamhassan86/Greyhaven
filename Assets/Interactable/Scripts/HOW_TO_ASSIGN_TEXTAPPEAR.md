# How to Assign "Text Appear" Field - Visual Guide

## Important: It's a FIELD, Not a Component!

**"Text Appear" is NOT a component** - it's a **field** (property) inside the `InteractionText` component.

## Exact Steps:

### Step 1: Select InteractionText GameObject
1. In **Hierarchy** (left panel)
2. Find and click **"InteractionText"** GameObject
3. It should highlight/select

### Step 2: Look in Inspector Panel
1. Look at **Inspector** panel (right side of screen)
2. You'll see several components listed:
   - Rect Transform
   - **InteractionText (Script)** ← THIS ONE!
   - TextMeshPro - Text (UI) (if it's on the same GameObject)

### Step 3: Find the Field Inside InteractionText Component
1. Scroll to **"InteractionText (Script)"** component in Inspector
2. Click to expand it if collapsed
3. Inside this component, you'll see:

```
┌─────────────────────────────────────┐
│ InteractionText (Script)            │
├─────────────────────────────────────┤
│ [Debug Settings]                     │
│   Enable Debug Logs: ☑              │
│                                     │
│ Text Appear: [None] ← THIS FIELD!  │
│   (TextMeshProUGUI)                 │
└─────────────────────────────────────┘
```

**The field shows:**
- `[None]` or `[None (TextMeshProUGUI)]` if empty
- `Text (TMP) (TextMeshProUGUI)` if assigned

### Step 4: Assign the TextMeshProUGUI

**Method 1: Drag and Drop**
1. In **Hierarchy**, expand "InteractionText"
2. You should see **"Text (TMP)"** as a child
3. **Drag "Text (TMP)"** from Hierarchy
4. **Drop it** into the "Text Appear" field in Inspector

**Method 2: Use the Circle Icon**
1. Click the **small circle icon** (target) next to "Text Appear" field
2. A picker window opens
3. Select **"Text (TMP)"** from the list
4. Click to assign

## Auto-Fix Added!

I've updated the code to **automatically find** the TextMeshProUGUI if it's not assigned!

**The script will now:**
1. Look for TextMeshProUGUI in children
2. Auto-assign it if found
3. Log a message if it auto-assigned

**So you might not need to assign it manually!** Just play the game and check the Console - it will tell you if it auto-found the text.

## If You Still Don't See the Field

**Possible reasons:**

1. **Wrong GameObject selected**
   - Make sure you selected "InteractionText" GameObject
   - Not "Text (TMP)" or "InteractionCanvas"

2. **Component not visible**
   - Scroll down in Inspector
   - The field is inside the "InteractionText (Script)" component

3. **Script not compiled**
   - Check Console for compilation errors
   - Fix any errors first

4. **Different script version**
   - The field name is `textAppear` (lowercase 't')
   - It's a public field, so it should always show

## Quick Test

1. **Play the game**
2. **Look at a key**
3. **Check Console** for:
   - `[InteractionText] Auto-found textAppear: Text (TMP)` ✅
   - OR `[InteractionText] textAppear is NULL!` ❌

If you see the auto-found message, it's working! If you see NULL, then manually assign it using the steps above.

## Visual Reference

```
Unity Editor Layout:
┌──────────┬──────────────┬──────────┐
│          │              │          │
│Hierarchy │   Scene      │Inspector │
│          │              │          │
│InteractionText ←───────→ InteractionText│
│├─Text(TMP)     │       │  Component:   │
│                │       │  InteractionText│
│                │       │  ├─Text Appear:│
│                │       │  │ [None] ←───│
│                │       │  └─Debug...   │
└──────────┴──────────────┴──────────┘
```

Drag "Text (TMP)" from left → Drop into "Text Appear" field on right!

