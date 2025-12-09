
namespace EJETAGame
{
    using UnityEngine;
    using VariableInventorySystem;

    /// <summary>
    /// Diagnostic tool to check if inventory images are loading correctly.
    /// Add this to a GameObject in your scene and it will test image loading.
    /// </summary>
    public class InventoryImageDiagnostic : MonoBehaviour
    {
        [Header("Test Settings")]
        [SerializeField] private string[] testPaths = { "Image/key", "Image/hammer" };
        [SerializeField] private bool runOnStart = true;

        private void Start()
        {
            if (runOnStart)
            {
                TestAllImages();
            }
        }

        [ContextMenu("Test All Images")]
        public void TestAllImages()
        {
            Debug.Log("=== INVENTORY IMAGE DIAGNOSTIC ===");
            Debug.Log($"Testing {testPaths.Length} image path(s)...");

            foreach (string path in testPaths)
            {
                TestImagePath(path);
            }

            Debug.Log("=== END DIAGNOSTIC ===");
        }

        private void TestImagePath(string path)
        {
            Debug.Log($"\n--- Testing path: '{path}' ---");

            //Test 1: Texture2D
            Texture2D tex2D = Resources.Load<Texture2D>(path);
            if (tex2D != null)
            {
                Debug.Log($"✓ Texture2D found: {tex2D.width}x{tex2D.height}, Format: {tex2D.format}");
            }
            else
            {
                Debug.LogWarning($"✗ Texture2D NOT found");
            }

            //Test 2: Sprite
            Sprite sprite = Resources.Load<Sprite>(path);
            if (sprite != null)
            {
                Debug.Log($"✓ Sprite found: {sprite.texture.width}x{sprite.texture.height}, Rect: {sprite.textureRect}");
            }
            else
            {
                Debug.LogWarning($"✗ Sprite NOT found");
            }

            //Test 3: Generic Texture
            Texture generic = Resources.Load<Texture>(path);
            if (generic != null)
            {
                Debug.Log($"✓ Generic Texture found: {generic.width}x{generic.height}, Type: {generic.GetType().Name}");
            }
            else
            {
                Debug.LogWarning($"✗ Generic Texture NOT found");
            }

            //Test 4: Check file exists
            string fullPath = $"Assets/Resources/{path}.png";
            Debug.Log($"Expected file location: {fullPath}");
            
            if (tex2D == null && sprite == null && generic == null)
            {
                Debug.LogError($"✗✗✗ NO IMAGE FOUND AT PATH: '{path}' ✗✗✗");
                Debug.LogError($"Make sure file exists at: {fullPath}");
                Debug.LogError($"And that it's imported correctly in Unity.");
            }
        }

        [ContextMenu("List All Resources")]
        public void ListAllResources()
        {
            Debug.Log("=== ALL RESOURCES IN Resources FOLDER ===");
            Object[] allResources = Resources.LoadAll("");
            Debug.Log($"Found {allResources.Length} resource(s):");
            foreach (var resource in allResources)
            {
                Debug.Log($"  - {resource.name} ({resource.GetType().Name})");
            }
            Debug.Log("=== END RESOURCES LIST ===");
        }
    }
}
