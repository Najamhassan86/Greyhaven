
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
        }
    }
}

