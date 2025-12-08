
namespace EJETAGame
{
    using UnityEngine;
    using TMPro;

    /// <summary>
    /// A hammer item that can be picked up and used to destroy doors.
    /// Requires long-press E to pick up.
    /// Implements ILongPressInteractable to work with the interaction system.
    /// </summary>
    public class HammerItem : MonoBehaviour, ILongPressInteractable
    {
        [SerializeField] private string hammerName = "Hammer"; //The name/ID of this hammer
        [SerializeField] private bool destroyOnPickup = true; //Whether to destroy the hammer object after picking it up
        [SerializeField] private float requiredHoldTime = 1.5f; //How long to hold E to pick up (in seconds)
        [SerializeField] private string imagePath = "Image/hammer"; //Path to the hammer image for visual inventory

        [Header("Debug Settings")]
        [SerializeField] private bool enableDebugLogs = true;

        private bool hasBeenPickedUp = false;
        private float currentHoldProgress = 0f;

        //ILongPressInteractable implementation
        public float RequiredHoldTime => requiredHoldTime;

        public void Interact()
        {
            //This is called when the long-press is completed
            //Check if inventory exists and hammer hasn't been picked up
            if (hasBeenPickedUp)
            {
                if (enableDebugLogs)
                {
                    Debug.LogWarning($"[HammerItem] {hammerName} already picked up, ignoring Interact()");
                }
                return;
            }

            if (enableDebugLogs)
            {
                Debug.Log($"[HammerItem] Interact() called on {hammerName}");
            }

            if (Inventory.instance == null)
            {
                Debug.LogError($"[HammerItem] Inventory instance not found! Make sure an Inventory component exists in the scene.");
                return;
            }

            if (enableDebugLogs)
            {
                Debug.Log($"[HammerItem] Adding {hammerName} to inventory...");
            }

            //Add hammer to inventory
            if (Inventory.instance.AddItem(hammerName, imagePath))
            {
                hasBeenPickedUp = true;

                if (enableDebugLogs)
                {
                    Debug.Log($"[HammerItem] {hammerName} successfully added to inventory!");
                }

                //Show message to player
                if (InteractionText.instance != null)
                {
                    InteractionText.instance.SetText($"{hammerName} added to inventory. Press H near a door to destroy it.");
                }

                //Destroy or disable the hammer object
                if (destroyOnPickup)
                {
                    if (enableDebugLogs)
                    {
                        Debug.Log($"[HammerItem] Destroying {hammerName} GameObject");
                    }
                    Destroy(gameObject);
                }
                else
                {
                    if (enableDebugLogs)
                    {
                        Debug.Log($"[HammerItem] Disabling {hammerName} GameObject");
                    }
                    gameObject.SetActive(false);
                }
            }
            else
            {
                if (enableDebugLogs)
                {
                    Debug.LogWarning($"[HammerItem] Failed to add {hammerName} to inventory (may already exist)");
                }
            }
        }

        public void OnInteractEnter()
        {
            if (enableDebugLogs)
            {
                Debug.Log($"[HammerItem] OnInteractEnter() called for {hammerName}");
            }
            
            if (hasBeenPickedUp)
            {
                if (enableDebugLogs)
                {
                    Debug.Log($"[HammerItem] {hammerName} already picked up, skipping OnInteractEnter");
                }
                return;
            }
            
            if (InteractionText.instance != null)
            {
                string text = $"Hold E to pick up {hammerName}";
                InteractionText.instance.SetText(text);
                
                if (enableDebugLogs)
                {
                    Debug.Log($"[HammerItem] Set interaction text: \"{text}\"");
                }
            }
            else
            {
                if (enableDebugLogs)
                {
                    Debug.LogError($"[HammerItem] InteractionText.instance is NULL! Cannot show interaction text for {hammerName}");
                }
            }
        }

        public void OnInteractExit()
        {
            if (enableDebugLogs)
            {
                Debug.Log($"[HammerItem] OnInteractExit() called for {hammerName}");
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
                InteractionText.instance.SetText($"{hammerName} picked up!");
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
                InteractionText.instance.SetText($"Hold E to pick up {hammerName} ({percentage}%)");
            }
        }
    }
}
