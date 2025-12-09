
namespace EJETAGame
{
    using UnityEngine;
    using TMPro;

    /// <summary>
    /// A key item that can be picked up and added to the player's inventory.
    /// Requires long-press E to pick up.
    /// Implements ILongPressInteractable to work with the interaction system.
    /// </summary>
    public class KeyItem : MonoBehaviour, ILongPressInteractable
    {
        [SerializeField] private string keyName = "Rust Key"; //The name/ID of this key (must match the door's required key)
        [SerializeField] private bool destroyOnPickup = true; //Whether to destroy the key object after picking it up
        [SerializeField] private float requiredHoldTime = 1.5f; //How long to hold E to pick up (in seconds)
        [SerializeField] private string imagePath = "Image/key"; //Path to the key image for visual inventory

        [Header("Debug Settings")]
        [SerializeField] private bool enableDebugLogs = true;

        private bool hasBeenPickedUp = false;
        private float currentHoldProgress = 0f;

        //ILongPressInteractable implementation
        public float RequiredHoldTime => requiredHoldTime;

        public void Interact()
        {
            //This is called when the long-press is completed
            //Check if inventory exists and key hasn't been picked up
            if (hasBeenPickedUp)
            {
                if (enableDebugLogs)
                {
                    Debug.LogWarning($"[KeyItem] {keyName} already picked up, ignoring Interact()");
                }
                return;
            }

            if (enableDebugLogs)
            {
                Debug.Log($"[KeyItem] Interact() called on {keyName}");
            }

            if (Inventory.instance == null)
            {
                Debug.LogError($"[KeyItem] Inventory instance not found! Make sure an Inventory component exists in the scene.");
                return;
            }

            if (enableDebugLogs)
            {
                Debug.Log($"[KeyItem] Adding {keyName} to inventory...");
            }

            //Add key to inventory
            if (Inventory.instance.AddItem(keyName, imagePath))
            {
                hasBeenPickedUp = true;

                if (enableDebugLogs)
                {
                    Debug.Log($"[KeyItem] {keyName} successfully added to inventory!");
                }

                //Show message to player that item was added
                if (InteractionText.instance != null)
                {
                    InteractionText.instance.SetText($"{keyName} added to inventory");
                }

                //Destroy or disable the key object
                if (destroyOnPickup)
                {
                    if (enableDebugLogs)
                    {
                        Debug.Log($"[KeyItem] Destroying {keyName} GameObject");
                    }
                    Destroy(gameObject);
                }
                else
                {
                    if (enableDebugLogs)
                    {
                        Debug.Log($"[KeyItem] Disabling {keyName} GameObject");
                    }
                    gameObject.SetActive(false);
                }
            }
            else
            {
                if (enableDebugLogs)
                {
                    Debug.LogWarning($"[KeyItem] Failed to add {keyName} to inventory (may already exist)");
                }
            }
        }

        public void OnInteractEnter()
        {
            if (enableDebugLogs)
            {
                Debug.Log($"[KeyItem] OnInteractEnter() called for {keyName}");
            }
            
            if (hasBeenPickedUp)
            {
                if (enableDebugLogs)
                {
                    Debug.Log($"[KeyItem] {keyName} already picked up, skipping OnInteractEnter");
                }
                return;
            }
            
            if (InteractionText.instance != null)
            {
                string text = $"Hold E to pick up {keyName}";
                InteractionText.instance.SetText(text);
                
                if (enableDebugLogs)
                {
                    Debug.Log($"[KeyItem] Set interaction text: \"{text}\"");
                }
            }
            else
            {
                if (enableDebugLogs)
                {
                    Debug.LogError($"[KeyItem] InteractionText.instance is NULL! Cannot show interaction text for {keyName}");
                }
            }
        }

        public void OnInteractExit()
        {
            if (enableDebugLogs)
            {
                Debug.Log($"[KeyItem] OnInteractExit() called for {keyName}");
            }
            
            currentHoldProgress = 0f;
            
            //Hide text when exiting interaction range
            if (InteractionText.instance != null && !hasBeenPickedUp)
            {
                InteractionText.instance.HideText();
            }
        }

        public void OnLongPressStart()
        {
            if (!hasBeenPickedUp)
            {
                currentHoldProgress = 0f;
            }
        }

        public void OnLongPressUpdate(float progress)
        {
            if (!hasBeenPickedUp)
            {
                currentHoldProgress = progress;
                UpdateProgressText(progress);
            }
        }

        public void OnLongPressComplete()
        {
            if (!hasBeenPickedUp && InteractionText.instance != null)
            {
                InteractionText.instance.SetText($"{keyName} picked up!");
            }
        }

        public void OnLongPressCancel()
        {
            currentHoldProgress = 0f;
            UpdateProgressText(0f);
        }

        private void UpdateProgressText(float progress)
        {
            if (InteractionText.instance != null && !hasBeenPickedUp)
            {
                int percentage = Mathf.RoundToInt(progress * 100f);
                InteractionText.instance.SetText($"Hold E to pick up {keyName} ({percentage}%)");
            }
        }
    }
}

