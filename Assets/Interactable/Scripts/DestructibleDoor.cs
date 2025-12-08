
namespace EJETAGame
{
    using UnityEngine;

    /// <summary>
    /// A door that can be destroyed with a hammer.
    /// Works with LockedDoor or can be standalone.
    /// </summary>
    public class DestructibleDoor : MonoBehaviour
    {
        [Header("Destruction Settings")]
        [SerializeField] private string requiredHammerName = "Hammer"; //The name/ID of the hammer needed to destroy this door
        [SerializeField] private KeyCode destroyKey = KeyCode.H; //Key to press to destroy the door
        [SerializeField] private float destroyRange = 3f; //How close player needs to be to destroy the door
        [SerializeField] private bool destroyOnUse = true; //Whether to destroy the door GameObject after destruction
        [SerializeField] private GameObject destructionEffect; //Optional particle effect or debris when door is destroyed
        [SerializeField] private AudioClip destructionSound; //Optional sound effect when door is destroyed

        [Header("Debug Settings")]
        [SerializeField] private bool enableDebugLogs = true;

        //Static flag to track if player has ever consumed a hammer (one-time use)
        private static bool hammerWasConsumed = false;

        private bool isDestroyed = false;
        private Transform playerTransform;
        private AudioSource audioSource;

        private void Start()
        {
            //Try to find player by tag or name
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player == null)
            {
                //Try finding by name
                player = GameObject.Find("Player");
            }
            if (player != null)
            {
                playerTransform = player.transform;
            }

            //Get or add AudioSource for destruction sound
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null && destructionSound != null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }

            if (enableDebugLogs)
            {
                Debug.Log($"[DestructibleDoor] Initialized on door: {gameObject.name}");
                Debug.Log($"[DestructibleDoor] Required Hammer: '{requiredHammerName}'");
                Debug.Log($"[DestructibleDoor] Destroy Key: {destroyKey}");
                Debug.Log($"[DestructibleDoor] Destroy Range: {destroyRange}m");
            }
        }

        private void Update()
        {
            if (isDestroyed) return;

            //Check if player is in range
            if (playerTransform == null) return;

            float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

            if (distanceToPlayer <= destroyRange)
            {
                //Player is in range - show interaction text only if they have hammer or haven't used one yet
                if (InteractionText.instance != null)
                {
                    bool hasHammer = Inventory.instance != null && Inventory.instance.HasItem(requiredHammerName);
                    
                    if (hasHammer)
                    {
                        InteractionText.instance.SetText($"Press {destroyKey} to destroy door with {requiredHammerName}");
                    }
                    else if (!hammerWasConsumed)
                    {
                        //Only show "need hammer" message if player hasn't consumed a hammer yet
                        InteractionText.instance.SetText($"Need {requiredHammerName} to destroy this door");
                    }
                    //If hammer was consumed, don't show any message (player already used their hammer)
                }

                //Check if player presses destroy key and has hammer
                if (Input.GetKeyDown(destroyKey))
                {
                    if (Inventory.instance == null)
                    {
                        Debug.LogError("[DestructibleDoor] Inventory instance not found!");
                        return;
                    }

                    if (Inventory.instance.HasItem(requiredHammerName))
                    {
                        DestroyDoor();
                    }
                    else
                    {
                        if (enableDebugLogs)
                        {
                            Debug.Log($"[DestructibleDoor] Player doesn't have {requiredHammerName}!");
                        }
                        
                        if (InteractionText.instance != null)
                        {
                            InteractionText.instance.SetText($"You need {requiredHammerName} to destroy this door!");
                        }
                    }
                }
            }
        }

        private void DestroyDoor()
        {
            if (isDestroyed) return;

            isDestroyed = true;

            if (enableDebugLogs)
            {
                Debug.Log($"[DestructibleDoor] Destroying door: {gameObject.name}");
            }

            //Play destruction sound
            if (audioSource != null && destructionSound != null)
            {
                audioSource.PlayOneShot(destructionSound);
            }

            //Spawn destruction effect
            if (destructionEffect != null)
            {
                GameObject effect = Instantiate(destructionEffect, transform.position, transform.rotation);
                //Auto-destroy effect after 5 seconds if it doesn't destroy itself
                Destroy(effect, 5f);
            }

            //Remove hammer from inventory (one-time use)
            if (Inventory.instance != null)
            {
                Inventory.instance.RemoveItem(requiredHammerName);
                hammerWasConsumed = true; //Mark that hammer was consumed
                if (enableDebugLogs)
                {
                    Debug.Log($"[DestructibleDoor] Removed {requiredHammerName} from inventory (one-time use)");
                    Debug.Log($"[DestructibleDoor] Hammer was consumed - other doors will no longer show hammer messages");
                }
            }

            //Show message to player
            if (InteractionText.instance != null)
            {
                InteractionText.instance.SetText($"Door destroyed! {requiredHammerName} was consumed.");
            }

            //Disable or destroy the door
            if (destroyOnUse)
            {
                //Disable colliders first so player can walk through
                Collider[] colliders = GetComponentsInChildren<Collider>();
                foreach (var collider in colliders)
                {
                    collider.enabled = false;
                }

                //Disable renderers to make door invisible
                Renderer[] renderers = GetComponentsInChildren<Renderer>();
                foreach (var renderer in renderers)
                {
                    renderer.enabled = false;
                }

                //Disable LockedDoor component if it exists
                LockedDoor lockedDoor = GetComponent<LockedDoor>();
                if (lockedDoor != null)
                {
                    lockedDoor.enabled = false;
                }

                //Destroy the GameObject after a short delay (to allow sound/effects to play)
                Destroy(gameObject, 2f);

                if (enableDebugLogs)
                {
                    Debug.Log($"[DestructibleDoor] Door GameObject will be destroyed in 2 seconds");
                }
            }
            else
            {
                //Just disable the door
                gameObject.SetActive(false);
            }
        }

        private void OnDrawGizmosSelected()
        {
            //Draw destroy range in Scene view
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, destroyRange);
        }
    }
}
