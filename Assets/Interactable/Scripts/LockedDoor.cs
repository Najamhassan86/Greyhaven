
namespace EJETAGame
{
    using UnityEngine;

    /// <summary>
    /// A door that requires a specific key to open.
    /// Implements IInteractable to work with the interaction system.
    /// </summary>
    public class LockedDoor : MonoBehaviour, IInteractable
    {
        [SerializeField] private string requiredKeyName = "Key"; //The name/ID of the key needed to open this door
        [SerializeField] private bool isLocked = true; //Whether the door is currently locked
        [SerializeField] private bool consumeKeyOnUse = false; //Whether to remove the key from inventory after opening
        [SerializeField] private float openRotation = 90f; //Rotation angle when door is open (in degrees)
        [SerializeField] private float openSpeed = 2f; //Speed of door opening animation
        [SerializeField] private Transform doorTransform; //The transform to rotate (if null, uses this object's transform)

        private bool isOpen = false;
        private Quaternion closedRotation;
        private Quaternion targetRotation;

        private void Start()
        {
            //If no door transform specified, use this object's transform
            if (doorTransform == null)
            {
                doorTransform = transform;
            }

            closedRotation = doorTransform.rotation;
            targetRotation = closedRotation;
        }

        private void Update()
        {
            //Smoothly rotate door to target rotation
            if (doorTransform.rotation != targetRotation)
            {
                doorTransform.rotation = Quaternion.Lerp(doorTransform.rotation, targetRotation, Time.deltaTime * openSpeed);
            }
        }

        public void Interact()
        {
            if (isOpen)
            {
                //Close the door
                CloseDoor();
                return;
            }

            if (isLocked)
            {
                //Check if player has the required key
                if (Inventory.instance == null)
                {
                    Debug.LogError("Inventory instance not found! Make sure an Inventory component exists in the scene.");
                    return;
                }

                if (!Inventory.instance.HasItem(requiredKeyName))
                {
                    //Player doesn't have the key
                    if (InteractionText.instance != null)
                    {
                        InteractionText.instance.SetText($"This door requires {requiredKeyName}");
                    }
                    Debug.Log($"Door is locked! You need {requiredKeyName} to open it.");
                    return;
                }

                //Player has the key - unlock and open
                isLocked = false;
                if (consumeKeyOnUse)
                {
                    Inventory.instance.RemoveItem(requiredKeyName);
                    Debug.Log($"Used {requiredKeyName} to unlock door");
                }
            }

            //Open the door
            OpenDoor();
        }

        private void OpenDoor()
        {
            isOpen = true;
            targetRotation = closedRotation * Quaternion.Euler(0, openRotation, 0);
            Debug.Log("Door opened!");
        }

        private void CloseDoor()
        {
            isOpen = false;
            targetRotation = closedRotation;
            Debug.Log("Door closed!");
        }

        public void OnInteractEnter()
        {
            if (InteractionText.instance != null)
            {
                if (isOpen)
                {
                    InteractionText.instance.SetText("Press E to close door");
                }
                else if (isLocked)
                {
                    InteractionText.instance.SetText($"Press E to unlock door (Requires {requiredKeyName})");
                }
                else
                {
                    InteractionText.instance.SetText("Press E to open door");
                }
            }
        }

        public void OnInteractExit()
        {
            //Nothing special needed when exiting
        }
    }
}

