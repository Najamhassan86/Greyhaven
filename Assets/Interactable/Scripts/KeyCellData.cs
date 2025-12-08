
namespace EJETAGame
{
    using UnityEngine;
    using VariableInventorySystem;

    /// <summary>
    /// Cell data for keys in the VariableInventorySystem.
    /// Represents a key item that can be displayed in the visual inventory UI.
    /// </summary>
    public class KeyCellData : IVariableInventoryCellData
    {
        public int Id => 0;
        public int Width { get; private set; }
        public int Height { get; private set; }
        public bool IsRotate { get; set; }
        public IVariableInventoryAsset ImageAsset { get; }

        public string KeyName { get; private set; } //Store the key name for reference

        public KeyCellData(string keyName, string imagePath = "Image/key", int width = 1, int height = 1)
        {
            KeyName = keyName;
            Width = width;
            Height = height;
            IsRotate = false;
            ImageAsset = new StandardAsset(imagePath);
            
            //Debug: Verify image path (works in both editor and runtime)
            VerifyImagePath(imagePath, keyName);
        }

        /// <summary>
        /// Verifies that the image exists at the specified path
        /// </summary>
        private void VerifyImagePath(string imagePath, string itemName)
        {
            //Try loading as Texture2D
            var testTexture = Resources.Load<Texture2D>(imagePath);
            if (testTexture != null)
            {
                Debug.Log($"[KeyCellData] ✓ Image found at '{imagePath}' as Texture2D ({testTexture.width}x{testTexture.height}) for '{itemName}'");
                return;
            }

            //Try loading as Sprite
            var testSprite = Resources.Load<Sprite>(imagePath);
            if (testSprite != null)
            {
                Debug.LogWarning($"[KeyCellData] ⚠ Image at '{imagePath}' is a Sprite. System will convert it, but consider changing import settings to 'Default' for better performance.");
                Debug.LogWarning($"[KeyCellData] For item: '{itemName}'");
                return;
            }

            //Try loading as generic Texture
            var testGeneric = Resources.Load<Texture>(imagePath);
            if (testGeneric != null)
            {
                Debug.LogWarning($"[KeyCellData] ⚠ Image at '{imagePath}' loaded as generic Texture. May need conversion.");
                return;
            }

            //Image not found
            Debug.LogError($"[KeyCellData] ✗ Image NOT FOUND at path: '{imagePath}'");
            Debug.LogError($"[KeyCellData] For item: '{itemName}'");
            Debug.LogError($"[KeyCellData] Expected location: Assets/Resources/{imagePath}.png");
            Debug.LogError($"[KeyCellData] Make sure:");
            Debug.LogError($"[KeyCellData]   1. File exists at Assets/Resources/{imagePath}.png");
            Debug.LogError($"[KeyCellData]   2. Path doesn't include .png extension (use 'Image/key' not 'Image/key.png')");
            Debug.LogError($"[KeyCellData]   3. Texture import settings allow loading (Texture Type: Default or Sprite)");
        }
    }
}

