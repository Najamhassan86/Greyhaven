# Inventory System Setup Guide

This guide explains how to set up the key pickup and visual inventory system.

## Features
- **Long-press E** to pick up keys (1.5 seconds by default)
- **Press I** to open/close visual inventory
- Keys are displayed in the VariableInventorySystem UI
- Keys can be used to unlock doors

## Setup Steps

### 1. Set Up the Inventory System

1. Create an empty GameObject in your scene (name it "InventoryManager")
2. Add the `Inventory` component to it
3. Add the `InventoryUIManager` component to it

### 2. Set Up the Visual Inventory UI

1. In your scene, create a Canvas (if you don't have one)
2. Create a Panel as a child of the Canvas (this will be your inventory panel)
3. Set up the VariableInventorySystem UI inside this panel:
   - Add `StandardCore` component to a GameObject
   - Add `StandardStashView` component to another GameObject
   - Configure them according to VariableInventorySystem documentation
4. Assign references in `InventoryUIManager`:
   - `inventoryPanel`: The Canvas Panel GameObject
   - `standardCore`: The GameObject with StandardCore component
   - `standardStashView`: The GameObject with StandardStashView component
   - Adjust `inventoryWidth` and `inventoryHeight` as needed (default: 8x6)

### 3. Set Up Keys

1. Select your Rust Key prefab (or any key GameObject in the scene)
2. Add the `KeyItem` component
3. Configure:
   - `keyName`: The name of the key (e.g., "Rust Key", "Main Key")
   - `requiredHoldTime`: How long to hold E (default: 1.5 seconds)
   - `imagePath`: Path to the key image in Resources (default: "Image/key")
   - `destroyOnPickup`: Whether to destroy the key after picking up

### 4. Set Up Locked Doors

1. Select your door GameObject
2. Add the `LockedDoor` component
3. Configure:
   - `requiredKeyName`: Must match the key's `keyName`
   - `isLocked`: Whether the door starts locked
   - `consumeKeyOnUse`: Whether to remove key from inventory after use
   - `openRotation`: How many degrees to rotate when opening
   - `doorTransform`: The transform to rotate (leave null to use this object)

### 5. Create Key Images

1. Create a folder: `Assets/Resources/Image/` (if it doesn't exist)
2. Add your key sprite/image to this folder
3. Name it `key.png` (or update the `imagePath` in KeyItem)

## Usage

- **Look at a key** → Shows "Hold E to pick up [KeyName]"
- **Hold E** → Progress bar shows percentage (0-100%)
- **Release E early** → Cancels pickup
- **Complete hold** → Key is added to inventory and disappears
- **Press I** → Opens/closes visual inventory to view collected keys
- **Look at locked door** → Shows "Press E to unlock door (Requires [KeyName])"
- **Press E on door** → Unlocks and opens if you have the key

## Notes

- The simple `Inventory` system handles game logic (checking if player has keys)
- The `VariableInventorySystem` handles visual display
- Both systems stay in sync automatically
- You can have multiple keys with different names
- Doors require exact key name matches

