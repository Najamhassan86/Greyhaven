
namespace EJETAGame
{
    using UnityEngine;

    /// <summary>
    /// A door that requires a specific key to open.
    /// Implements IInteractable to work with the interaction system.
    /// </summary>
    public class LockedDoor : MonoBehaviour, IInteractable
    {
        [SerializeField] private string requiredKeyName = "Rust Key"; //The name/ID of the key needed to open this door (must match KeyItem's keyName exactly)
        [SerializeField] private bool isLocked = true; //Whether the door is currently locked
        [SerializeField] private bool consumeKeyOnUse = false; //Whether to remove the key from inventory after opening
        [SerializeField] private float openRotation = 90f; //Rotation angle when door is open (in degrees)
        [SerializeField] private float openSpeed = 2f; //Speed of door opening animation
        [SerializeField] private Transform doorTransform; //The transform to rotate (if null, uses this object's transform)
        [SerializeField] private bool enableDoor = true; //Enable/disable this door (if false, door won't show interaction text or respond to interactions)
        
        [Header("Rotation Settings")]
        [SerializeField] private Vector3 pivotPoint = Vector3.zero; //Pivot point offset in local space (where the hinge is). If zero, rotates around object center.
        [SerializeField] private bool usePivotPoint = false; //Enable pivot point rotation (door rotates around hinge instead of center)

        [Header("Debug Settings")]
        [SerializeField] private bool enableDebugLogs = true; //Enable/disable debug logging

        private bool isOpen = false;
        private Quaternion closedRotation;
        private Quaternion targetRotation;
        private Vector3 worldPivotPoint; //World space pivot point for rotation
        private Vector3 closedPosition; //Initial position when door is closed (for pivot rotation)
        private Vector3 closedPivotOffset; //Offset from pivot to door center when closed (for pivot rotation)

        private void Start()
        {
            if (enableDebugLogs)
            {
                Debug.Log($"[LockedDoor] Initializing door: {gameObject.name}");
                Debug.Log($"[LockedDoor] Required Key Name: '{requiredKeyName}'");
                Debug.Log($"[LockedDoor] Is Locked: {isLocked}");
                Debug.Log($"[LockedDoor] Consume Key On Use: {consumeKeyOnUse}");
                Debug.Log($"[LockedDoor] Open Rotation: {openRotation}°");
                Debug.Log($"[LockedDoor] Open Speed: {openSpeed}");
            }

            //If no door transform specified, use this object's transform
            if (doorTransform == null)
            {
                doorTransform = transform;
                if (enableDebugLogs)
                {
                    Debug.Log($"[LockedDoor] No door transform assigned, using this GameObject's transform: {gameObject.name}");
                }
            }
            else
            {
                if (enableDebugLogs)
                {
                    Debug.Log($"[LockedDoor] Using assigned door transform: {doorTransform.gameObject.name}");
                }
            }

            //Ensure doorTransform is initialized before accessing rotation
            EnsureDoorTransformInitialized();
            
            if (doorTransform != null)
            {
                closedRotation = doorTransform.rotation;
                targetRotation = closedRotation;
                closedPosition = doorTransform.position; //Store initial position

                //Calculate world space pivot point if using pivot rotation
                if (usePivotPoint)
                {
                    worldPivotPoint = doorTransform.TransformPoint(pivotPoint);
                    //Calculate the offset from pivot to door center in world space when closed
                    closedPivotOffset = doorTransform.position - worldPivotPoint;
                    if (enableDebugLogs)
                    {
                        Debug.Log($"[LockedDoor] Using pivot point rotation. Pivot offset (local): {pivotPoint}, Pivot (world): {worldPivotPoint}");
                        Debug.Log($"[LockedDoor] Closed pivot offset: {closedPivotOffset}");
                    }
                }

                if (enableDebugLogs)
                {
                    Debug.Log($"[LockedDoor] Closed rotation set to: {closedRotation.eulerAngles}");
                }
            }
            else
            {
                Debug.LogError($"[LockedDoor] Failed to initialize doorTransform on '{gameObject.name}'!");
            }

            //Validate setup
            if (Inventory.instance == null)
            {
                Debug.LogWarning($"[LockedDoor] WARNING: Inventory.instance is NULL! Door '{gameObject.name}' will not work properly.");
                Debug.LogWarning($"[LockedDoor] Fix: Add an Inventory component to a GameObject in the scene.");
            }
        }

        private void Update()
        {
            //Ensure doorTransform is initialized
            EnsureDoorTransformInitialized();
            
            if (doorTransform == null) return;

            //Smoothly rotate door to target rotation
            float angleDifference = Quaternion.Angle(doorTransform.rotation, targetRotation);
            
            if (angleDifference > 0.1f)
            {
                if (enableDebugLogs && Time.frameCount % 60 == 0) //Log every 60 frames when rotating
                {
                    Debug.Log($"[LockedDoor] Rotating door '{gameObject.name}' - Current: {doorTransform.rotation.eulerAngles.y:F1}°, Target: {targetRotation.eulerAngles.y:F1}°, Difference: {angleDifference:F1}°");
                }

                if (usePivotPoint)
                {
                    //Smoothly interpolate rotation
                    Quaternion currentRotation = doorTransform.rotation;
                    Quaternion newRotation = Quaternion.Slerp(currentRotation, targetRotation, Time.deltaTime * openSpeed);
                    
                    //Calculate rotation delta
                    Quaternion rotationDelta = newRotation * Quaternion.Inverse(currentRotation);
                    
                    //Rotate around pivot point using the delta
                    RotateAroundPivot(doorTransform, worldPivotPoint, rotationDelta);
                }
                else
                {
                    //Rotate around object center (default behavior)
                    doorTransform.rotation = Quaternion.Slerp(doorTransform.rotation, targetRotation, Time.deltaTime * openSpeed);
                }
            }
            else
            {
                //Rotation complete - snap to exact target
                if (usePivotPoint)
                {
                    //Calculate final position based on closed offset rotated by target rotation
                    Vector3 rotatedOffset = targetRotation * (Quaternion.Inverse(closedRotation) * closedPivotOffset);
                    doorTransform.position = worldPivotPoint + rotatedOffset;
                    doorTransform.rotation = targetRotation;
                }
                else
                {
                    doorTransform.rotation = targetRotation;
                }
                
                if (enableDebugLogs && Time.frameCount % 60 == 0)
                {
                    Debug.Log($"[LockedDoor] Door '{gameObject.name}' rotation complete. Final rotation: {doorTransform.rotation.eulerAngles}");
                }
            }
        }

        /// <summary>
        /// Rotates a transform around a pivot point in world space using a rotation delta
        /// </summary>
        private void RotateAroundPivot(Transform target, Vector3 pivot, Quaternion rotationDelta)
        {
            //Get current offset from pivot
            Vector3 offset = target.position - pivot;
            
            //Rotate the offset by the rotation delta
            offset = rotationDelta * offset;
            
            //Update position
            target.position = pivot + offset;
            
            //Update rotation
            target.rotation = rotationDelta * target.rotation;
        }

        /// <summary>
        /// Ensures doorTransform is initialized. Can be called from any method.
        /// </summary>
        private void EnsureDoorTransformInitialized()
        {
            if (doorTransform == null)
            {
                doorTransform = transform;
                if (enableDebugLogs)
                {
                    Debug.Log($"[LockedDoor] doorTransform was null, initialized to: {gameObject.name}");
                }
            }
        }

        public void Interact()
        {
            //Check if door is enabled
            if (!enableDoor)
            {
                if (enableDebugLogs)
                {
                    Debug.Log($"[LockedDoor] Door '{gameObject.name}' is disabled, ignoring interaction");
                }
                return;
            }

            //Ensure doorTransform is initialized
            EnsureDoorTransformInitialized();
            
            if (doorTransform == null)
            {
                Debug.LogError($"[LockedDoor] doorTransform is NULL on '{gameObject.name}'! Cannot interact with door.");
                return;
            }

            if (enableDebugLogs)
            {
                Debug.Log($"[LockedDoor] Interact() called on door: {gameObject.name}");
                Debug.Log($"[LockedDoor] Current state - Is Open: {isOpen}, Is Locked: {isLocked}");
            }

            if (isOpen)
            {
                //Close the door
                if (enableDebugLogs)
                {
                    Debug.Log($"[LockedDoor] Door is open, closing door: {gameObject.name}");
                }
                CloseDoor();
                return;
            }

            if (isLocked)
            {
                //Check if player has the required key
                if (Inventory.instance == null)
                {
                    Debug.LogError($"[LockedDoor] Inventory instance not found! Make sure an Inventory component exists in the scene.");
                    Debug.LogError($"[LockedDoor] Door '{gameObject.name}' cannot check for key '{requiredKeyName}'");
                    return;
                }

                if (enableDebugLogs)
                {
                    Debug.Log($"[LockedDoor] Checking if player has key: '{requiredKeyName}'");
                }

                if (!Inventory.instance.HasItem(requiredKeyName))
                {
                    //Player doesn't have the key
                    if (enableDebugLogs)
                    {
                        Debug.Log($"[LockedDoor] Player does NOT have key '{requiredKeyName}'. Door remains locked.");
                        Debug.Log($"[LockedDoor] Available keys in inventory: {string.Join(", ", Inventory.instance.GetAllItems())}");
                    }

                    if (InteractionText.instance != null)
                    {
                        InteractionText.instance.SetText($"This door requires {requiredKeyName}");
                    }
                    else
                    {
                        if (enableDebugLogs)
                        {
                            Debug.LogWarning($"[LockedDoor] InteractionText.instance is NULL! Cannot show message to player.");
                        }
                    }
                    return;
                }

                //Player has the key - unlock and open
                if (enableDebugLogs)
                {
                    Debug.Log($"[LockedDoor] Player HAS key '{requiredKeyName}'! Unlocking door...");
                }

                isLocked = false;
                if (consumeKeyOnUse)
                {
                    if (enableDebugLogs)
                    {
                        Debug.Log($"[LockedDoor] Consuming key '{requiredKeyName}' from inventory (one-time use)");
                    }
                    Inventory.instance.RemoveItem(requiredKeyName);
                }
                else
                {
                    if (enableDebugLogs)
                    {
                        Debug.Log($"[LockedDoor] Key '{requiredKeyName}' kept in inventory (reusable)");
                    }
                }
            }
            else
            {
                if (enableDebugLogs)
                {
                    Debug.Log($"[LockedDoor] Door is unlocked, opening...");
                }
            }

            //Open the door
            OpenDoor();
        }

        private void OpenDoor()
        {
            //Ensure doorTransform is initialized
            EnsureDoorTransformInitialized();
            
            if (doorTransform == null)
            {
                Debug.LogError($"[LockedDoor] doorTransform is NULL on '{gameObject.name}'! Cannot open door.");
                return;
            }

            if (enableDebugLogs)
            {
                Debug.Log($"[LockedDoor] Opening door: {gameObject.name}");
                Debug.Log($"[LockedDoor] Current rotation: {doorTransform.rotation.eulerAngles}");
                Debug.Log($"[LockedDoor] Target rotation: {closedRotation.eulerAngles} + {openRotation}° = {(closedRotation * Quaternion.Euler(0, openRotation, 0)).eulerAngles}");
            }

            isOpen = true;
            targetRotation = closedRotation * Quaternion.Euler(0, openRotation, 0);
            
            //Update pivot point if using pivot rotation
            if (usePivotPoint)
            {
                //Recalculate pivot point from current transform
                worldPivotPoint = doorTransform.TransformPoint(pivotPoint);
            }

            if (enableDebugLogs)
            {
                Debug.Log($"[LockedDoor] Door '{gameObject.name}' is now opening (target rotation set)");
            }
        }

        private void CloseDoor()
        {
            //Ensure doorTransform is initialized
            EnsureDoorTransformInitialized();
            
            if (doorTransform == null)
            {
                Debug.LogError($"[LockedDoor] doorTransform is NULL on '{gameObject.name}'! Cannot close door.");
                return;
            }

            if (enableDebugLogs)
            {
                Debug.Log($"[LockedDoor] Closing door: {gameObject.name}");
                Debug.Log($"[LockedDoor] Current rotation: {doorTransform.rotation.eulerAngles}");
                Debug.Log($"[LockedDoor] Target rotation: {closedRotation.eulerAngles}");
            }

            isOpen = false;
            targetRotation = closedRotation;
            
            //Update pivot point if using pivot rotation
            if (usePivotPoint)
            {
                //Recalculate pivot point from current transform
                worldPivotPoint = doorTransform.TransformPoint(pivotPoint);
            }

            if (enableDebugLogs)
            {
                Debug.Log($"[LockedDoor] Door '{gameObject.name}' is now closing (target rotation set)");
            }
        }

        public void OnInteractEnter()
        {
            //Don't show interaction text if door is disabled
            if (!enableDoor)
            {
                return;
            }

            if (enableDebugLogs)
            {
                Debug.Log($"[LockedDoor] OnInteractEnter() called on door: {gameObject.name}");
                Debug.Log($"[LockedDoor] State - Is Open: {isOpen}, Is Locked: {isLocked}");
            }

            if (InteractionText.instance != null)
            {
                string interactionText = "";
                
                if (isOpen)
                {
                    //Door is open - show close message
                    interactionText = "Press E to close door";
                }
                else if (isLocked)
                {
                    //Door is locked - check if player has the key
                    bool hasKey = Inventory.instance != null && Inventory.instance.HasItem(requiredKeyName);
                    
                    if (hasKey)
                    {
                        //Player has the key - show unlock message
                        interactionText = $"Press E to unlock door (Requires {requiredKeyName})";
                    }
                    else
                    {
                        //Player doesn't have the key - show requirement message
                        interactionText = $"This door requires {requiredKeyName}";
                    }
                }
                else
                {
                    //Door is unlocked but closed - show open message
                    interactionText = "Press E to open door";
                }

                InteractionText.instance.SetText(interactionText);

                if (enableDebugLogs)
                {
                    Debug.Log($"[LockedDoor] Set interaction text: \"{interactionText}\"");
                }
            }
            else
            {
                if (enableDebugLogs)
                {
                    Debug.LogWarning($"[LockedDoor] InteractionText.instance is NULL! Cannot show interaction text for door '{gameObject.name}'");
                    Debug.LogWarning($"[LockedDoor] Fix: Create GameObject with InteractionText component in scene.");
                }
            }
        }

        public void OnInteractExit()
        {
            if (enableDebugLogs)
            {
                Debug.Log($"[LockedDoor] OnInteractExit() called on door: {gameObject.name}");
            }
            //Nothing special needed when exiting
        }
    }
}

