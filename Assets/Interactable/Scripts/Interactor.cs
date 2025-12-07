
namespace EJETAGame
{
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using TMPro;
    using Unity.VisualScripting;
    using UnityEngine;

    public class Interactor : MonoBehaviour
    {
        Transform interactorSource; //Point of origin for our Interaction source
        [SerializeField] float interactRange; //How far our Interaction can detect;
        
        [Header("Detection Settings")]
        [SerializeField] private bool useSphereCast = true; //Use SphereCast instead of Raycast (more forgiving)
        [SerializeField] private float sphereCastRadius = 0.2f; //Radius of sphere for SphereCast (in meters)

        private IInteractable currentInteractable; //Track the currently detected interactable object;

        public GameObject detectedObject;

        [Header("Debug Settings")]
        [SerializeField] private bool enableDebugLogs = true; //Enable/disable debug logging
        [SerializeField] private bool drawRayInScene = true; //Draw ray in Scene view

        private void Awake()
        {
            interactorSource = transform;
            
            if (enableDebugLogs)
            {
                Debug.Log($"[Interactor] Initialized on {gameObject.name}");
                Debug.Log($"[Interactor] Interact Range: {interactRange}");
                Debug.Log($"[Interactor] Interact Key: {interactKey}");
            }
        }
        [SerializeField] KeyCode interactKey = KeyCode.E; //Key to press for interaction

        //Long-press support
        private float holdTime = 0f;
        private bool isHolding = false;
        private IInteractable holdTarget = null;

        private void Update()
        {
            //We send a ray to detect all objects;
            Ray r = new Ray(interactorSource.position, interactorSource.forward);
            
            //Draw ray/sphere in Scene view for debugging
            if (drawRayInScene)
            {
                if (useSphereCast)
                {
                    //Draw sphere cast as a capsule
                    Vector3 start = interactorSource.position;
                    Vector3 end = interactorSource.position + interactorSource.forward * interactRange;
                    Debug.DrawLine(start, end, Color.red);
                    //Note: Unity doesn't have built-in sphere visualization, but the line shows direction
                }
                else
                {
                    Debug.DrawRay(interactorSource.position, interactorSource.forward * interactRange, Color.red);
                }
            }
            
            bool hitDetected = false;
            RaycastHit hitInfo;
            
            //Use SphereCast or Raycast based on setting
            if (useSphereCast)
            {
                hitDetected = Physics.SphereCast(r, sphereCastRadius, out hitInfo, interactRange);
                
                if (enableDebugLogs && Time.frameCount % 60 == 0 && hitDetected)
                {
                    Debug.Log($"[Interactor] SphereCast hit: {hitInfo.collider.gameObject.name} at distance {hitInfo.distance:F2}m (radius: {sphereCastRadius}m)");
                }
            }
            else
            {
                hitDetected = Physics.Raycast(r, out hitInfo, interactRange);
                
                if (enableDebugLogs && Time.frameCount % 60 == 0 && hitDetected)
                {
                    Debug.Log($"[Interactor] Raycast hit: {hitInfo.collider.gameObject.name} at distance {hitInfo.distance:F2}m");
                }
            }
            
            if (hitDetected)
            {
                detectedObject = hitInfo.collider.gameObject;
                
                //We check if the object we collided with has the IInteractable interface;
                if (detectedObject.TryGetComponent(out IInteractable interactObj))
                {
                    if (enableDebugLogs && currentInteractable != interactObj)
                    {
                        Debug.Log($"[Interactor] Found IInteractable: {detectedObject.name} ({interactObj.GetType().Name})");
                    }
                    if (currentInteractable != interactObj)
                    {
                        //Cancel long-press if switching to a different interactable
                        if (isHolding && holdTarget != null && holdTarget != interactObj)
                        {
                            if (holdTarget is ILongPressInteractable longPressTarget)
                            {
                                longPressTarget.OnLongPressCancel();
                            }
                            isHolding = false;
                            holdTime = 0f;
                            holdTarget = null;
                        }

                        //Exit the previous interactable object if exists;
                        if (currentInteractable != null)
                            currentInteractable.OnInteractExit();

                        //Enter the new interactable object;
                        interactObj.OnInteractEnter();
                        currentInteractable = interactObj;
                    }

                    //We activate our text component;
                    if (InteractionText.instance != null && InteractionText.instance.textAppear != null)
                    {
                        //Make sure the text GameObject and all parents are active
                        InteractionText.instance.textAppear.gameObject.SetActive(true);
                        
                        //Make sure the Canvas is active
                        Canvas canvas = InteractionText.instance.textAppear.GetComponentInParent<Canvas>();
                        if (canvas != null && !canvas.gameObject.activeInHierarchy)
                        {
                            if (enableDebugLogs)
                            {
                                Debug.LogWarning($"[Interactor] Canvas '{canvas.gameObject.name}' is inactive! Activating it...");
                            }
                            canvas.gameObject.SetActive(true);
                        }
                        
                        //Check if text is actually visible
                        if (enableDebugLogs && Time.frameCount % 60 == 0)
                        {
                            bool isActive = InteractionText.instance.textAppear.gameObject.activeInHierarchy;
                            bool isActiveSelf = InteractionText.instance.textAppear.gameObject.activeSelf;
                            Color textColor = InteractionText.instance.textAppear.color;
                            
                            Debug.Log($"[Interactor] Activated interaction text for: {detectedObject.name}");
                            Debug.Log($"[Interactor] Text GameObject activeInHierarchy: {isActive}, activeSelf: {isActiveSelf}");
                            Debug.Log($"[Interactor] Text color: R={textColor.r:F2}, G={textColor.g:F2}, B={textColor.b:F2}, A={textColor.a:F2}");
                            
                            if (!isActive)
                            {
                                Debug.LogWarning("[Interactor] Text GameObject is not active in hierarchy! Check parent GameObjects.");
                            }
                            if (textColor.a < 0.01f)
                            {
                                Debug.LogWarning("[Interactor] Text alpha is too low! Text is transparent. Set alpha to 1.0");
                            }
                        }
                    }
                    else
                    {
                        if (enableDebugLogs && Time.frameCount % 60 == 0)
                        {
                            if (InteractionText.instance == null)
                            {
                                Debug.LogWarning("[Interactor] InteractionText.instance is NULL! Cannot show interaction text.");
                                Debug.LogWarning("[Interactor] Fix: Create GameObject with InteractionText component in scene.");
                            }
                            else if (InteractionText.instance.textAppear == null)
                            {
                                Debug.LogWarning("[Interactor] InteractionText.instance.textAppear is NULL! Assign TextMeshProUGUI in Inspector.");
                                Debug.LogWarning($"[Interactor] InteractionText GameObject: {InteractionText.instance.gameObject.name}");
                            }
                        }
                    }

                    //Check if this interactable requires long-press
                    bool requiresLongPress = interactObj is ILongPressInteractable;

                    if (requiresLongPress)
                    {
                        HandleLongPress(interactObj);
                    }
                    else
                    {
                        //Regular interaction - call Interact on key press
                        if (Input.GetKeyDown(interactKey))
                        {
                            if (enableDebugLogs)
                            {
                                Debug.Log($"[Interactor] E key pressed! Interacting with {detectedObject.name}");
                            }
                            interactObj.Interact();
                        }
                    }
                }
                else
                {
                    // Object hit but doesn't have IInteractable component
                    if (enableDebugLogs && Time.frameCount % 60 == 0)
                    {
                        Debug.Log($"[Interactor] Raycast hit {detectedObject.name} but it doesn't have IInteractable component");
                    }
                    
                    // No object detected, exit the previous interactable object if exists
                    if (currentInteractable != null)
                    {
                        if (enableDebugLogs)
                        {
                            Debug.Log($"[Interactor] Exiting interactable: {currentInteractable.GetType().Name}");
                        }
                        
                        //Hide interaction text
                        if (InteractionText.instance != null)
                        {
                            InteractionText.instance.HideText();
                        }

                        currentInteractable.OnInteractExit();
                        currentInteractable = null;
                    }

                }
            }
            else
            {
                // Raycast hit nothing
                if (enableDebugLogs && Time.frameCount % 120 == 0) //Log every 120 frames when not hitting anything
                {
                    Debug.Log($"[Interactor] Raycast hit nothing. Range: {interactRange}m, Position: {interactorSource.position}, Forward: {interactorSource.forward}");
                }
                
                // No object detected, exit the previous interactable object if exists
                if (currentInteractable != null)
                {
                    if (enableDebugLogs)
                    {
                        Debug.Log($"[Interactor] No object in range, exiting interactable");
                    }
                    
                    //Hide interaction text
                    if (InteractionText.instance != null)
                    {
                        InteractionText.instance.HideText();
                    }

                    currentInteractable.OnInteractExit();
                    currentInteractable = null;
                }

                //Reset long-press when not looking at anything
                if (isHolding)
                {
                    if (enableDebugLogs)
                    {
                        Debug.Log("[Interactor] Resetting long-press (no target)");
                    }
                    isHolding = false;
                    holdTime = 0f;
                    holdTarget = null;
                }
            }

        }

        private void HandleLongPress(IInteractable interactObj)
        {
            ILongPressInteractable longPressObj = interactObj as ILongPressInteractable;

            if (Input.GetKey(interactKey))
            {
                if (!isHolding || holdTarget != interactObj)
                {
                    //Start holding
                    isHolding = true;
                    holdTarget = interactObj;
                    holdTime = 0f;
                    
                    if (enableDebugLogs)
                    {
                        Debug.Log($"[Interactor] Started long-press on {detectedObject.name}. Required time: {longPressObj.RequiredHoldTime}s");
                    }
                    
                    longPressObj?.OnLongPressStart();
                }

                //Increment hold time
                holdTime += Time.deltaTime;

                //Update progress
                float progress = Mathf.Clamp01(holdTime / longPressObj.RequiredHoldTime);
                longPressObj?.OnLongPressUpdate(progress);
                
                if (enableDebugLogs && Time.frameCount % 30 == 0) //Log progress every 30 frames
                {
                    Debug.Log($"[Interactor] Long-press progress: {(progress * 100f):F0}% ({holdTime:F2}s / {longPressObj.RequiredHoldTime}s)");
                }

                //Check if held long enough
                if (holdTime >= longPressObj.RequiredHoldTime)
                {
                    //Complete the long press
                    if (enableDebugLogs)
                    {
                        Debug.Log($"[Interactor] Long-press completed! Interacting with {detectedObject.name}");
                    }
                    
                    longPressObj?.OnLongPressComplete();
                    interactObj.Interact();
                    
                    //Reset
                    isHolding = false;
                    holdTime = 0f;
                    holdTarget = null;
                }
            }
            else if (Input.GetKeyUp(interactKey))
            {
                //Released before completing
                if (isHolding && holdTarget == interactObj)
                {
                    if (enableDebugLogs)
                    {
                        Debug.Log($"[Interactor] Long-press cancelled (released early). Progress was: {(holdTime / longPressObj.RequiredHoldTime * 100f):F0}%");
                    }
                    
                    longPressObj?.OnLongPressCancel();
                    isHolding = false;
                    holdTime = 0f;
                    holdTarget = null;
                }
            }
        }

    }
}

