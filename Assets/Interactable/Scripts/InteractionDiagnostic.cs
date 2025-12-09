
namespace EJETAGame
{
    using UnityEngine;

    /// <summary>
    /// Diagnostic tool to check if interaction system is set up correctly.
    /// Add this to any GameObject and it will print diagnostic info to console.
    /// </summary>
    public class InteractionDiagnostic : MonoBehaviour
    {
        [ContextMenu("Run Diagnostics")]
        public void RunDiagnostics()
        {
            Debug.Log("=== INTERACTION SYSTEM DIAGNOSTICS ===");
            
            // Check Interactor
            CheckInteractor();
            
            // Check InteractionText
            CheckInteractionText();
            
            // Check Inventory
            CheckInventory();
            
            // Check for interactable objects
            CheckInteractableObjects();
            
            Debug.Log("=== END DIAGNOSTICS ===");
        }

        private void CheckInteractor()
        {
            Debug.Log("\n--- Checking Interactor ---");
            
            Interactor interactor = FindObjectOfType<Interactor>();
            if (interactor == null)
            {
                Debug.LogError("❌ Interactor component NOT FOUND in scene!");
                Debug.LogError("   Fix: Add Interactor component to PlayerFollowCamera");
            }
            else
            {
                Debug.Log($"✅ Interactor found on: {interactor.gameObject.name}");
                
                // Check interact range
                var rangeField = typeof(Interactor).GetField("interactRange", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (rangeField != null)
                {
                    float range = (float)rangeField.GetValue(interactor);
                    if (range <= 0)
                    {
                        Debug.LogError($"❌ Interact Range is {range} (should be > 0)");
                        Debug.LogError("   Fix: Set Interact Range to 5 in Inspector");
                    }
                    else
                    {
                        Debug.Log($"✅ Interact Range: {range}");
                    }
                }
            }
        }

        private void CheckInteractionText()
        {
            Debug.Log("\n--- Checking InteractionText ---");
            
            if (InteractionText.instance == null)
            {
                Debug.LogError("❌ InteractionText.instance is NULL!");
                Debug.LogError("   Fix: Create GameObject with InteractionText component in scene");
            }
            else
            {
                Debug.Log($"✅ InteractionText.instance exists");
                
                if (InteractionText.instance.textAppear == null)
                {
                    Debug.LogError("❌ InteractionText.textAppear is NULL!");
                    Debug.LogError("   Fix: Assign TextMeshProUGUI to textAppear field in Inspector");
                }
                else
                {
                    Debug.Log($"✅ InteractionText.textAppear is assigned");
                    Debug.Log($"   Text GameObject: {InteractionText.instance.textAppear.gameObject.name}");
                }
            }
        }

        private void CheckInventory()
        {
            Debug.Log("\n--- Checking Inventory ---");
            
            if (Inventory.instance == null)
            {
                Debug.LogError("❌ Inventory.instance is NULL!");
                Debug.LogError("   Fix: Create GameObject with Inventory component in scene");
            }
            else
            {
                Debug.Log($"✅ Inventory.instance exists");
                Debug.Log($"   Inventory GameObject: {Inventory.instance.gameObject.name}");
            }
        }

        private void CheckInteractableObjects()
        {
            Debug.Log("\n--- Checking Interactable Objects ---");
            
            IInteractable[] interactables = FindObjectsOfType<MonoBehaviour>() as IInteractable[];
            int count = 0;
            
            foreach (var obj in FindObjectsOfType<MonoBehaviour>())
            {
                if (obj is IInteractable)
                {
                    count++;
                    Debug.Log($"✅ Found interactable: {obj.gameObject.name} ({obj.GetType().Name})");
                    
                    // Check for collider
                    if (obj.GetComponent<Collider>() == null)
                    {
                        Debug.LogWarning($"   ⚠️ {obj.gameObject.name} has NO COLLIDER - raycast won't detect it!");
                    }
                }
            }
            
            if (count == 0)
            {
                Debug.LogWarning("⚠️ No interactable objects found in scene!");
                Debug.LogWarning("   Fix: Add KeyItem component to keys, or LockedDoor to doors");
            }
            else
            {
                Debug.Log($"✅ Found {count} interactable object(s)");
            }
        }

        private void Update()
        {
            // Auto-run diagnostics on play (optional - comment out if too spammy)
            // if (Time.time < 1f && Time.frameCount == 1)
            // {
            //     RunDiagnostics();
            // }
        }
    }
}

