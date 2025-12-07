
namespace EJETAGame
{
    using UnityEngine;
    using VariableInventorySystem;

    /// <summary>
    /// Manages the visual inventory UI. Opens/closes with I key.
    /// Handles the VariableInventorySystem integration.
    /// </summary>
    public class InventoryUIManager : MonoBehaviour
    {
        [SerializeField] private KeyCode toggleKey = KeyCode.I;
        [SerializeField] private GameObject inventoryPanel; //The UI panel containing the inventory
        [SerializeField] private StandardCore standardCore; //Reference to the VariableInventorySystem core
        [SerializeField] private StandardStashView standardStashView; //Reference to the stash view
        [SerializeField] private int inventoryWidth = 8; //Width of inventory grid
        [SerializeField] private int inventoryHeight = 6; //Height of inventory grid

        [Header("Debug Settings")]
        [SerializeField] private bool enableDebugLogs = true;

        private StandardStashViewData stashData;
        private bool isInventoryOpen = false;
        private bool canvasWasActivatedByUs = false; //Track if we activated the Canvas
        private UnityEngine.EventSystems.EventSystem[] disabledEventSystems; //Track EventSystems we disabled

        private void Awake()
        {
            if (enableDebugLogs)
            {
                Debug.Log("[InventoryUIManager] Awake() called");
                Debug.Log($"[InventoryUIManager] Toggle Key: {toggleKey}");
                Debug.Log($"[InventoryUIManager] Inventory Panel: {(inventoryPanel != null ? inventoryPanel.name : "NULL")}");
                Debug.Log($"[InventoryUIManager] Standard Core: {(standardCore != null ? standardCore.gameObject.name : "NULL")}");
                Debug.Log($"[InventoryUIManager] Standard Stash View: {(standardStashView != null ? standardStashView.gameObject.name : "NULL")}");
            }

            //Initialize the inventory system
            if (standardCore != null && standardStashView != null)
            {
                if (enableDebugLogs)
                {
                    Debug.Log("[InventoryUIManager] Initializing VariableInventorySystem...");
                }

                standardCore.Initialize();
                standardCore.AddInventoryView(standardStashView);

                //Create the stash data
                stashData = new StandardStashViewData(inventoryWidth, inventoryHeight);
                standardStashView.Apply(stashData);

                if (enableDebugLogs)
                {
                    Debug.Log($"[InventoryUIManager] Inventory initialized: {inventoryWidth}x{inventoryHeight} grid");
                }

                //Connect to Inventory singleton
                if (Inventory.instance != null)
                {
                    Inventory.instance.InitializeVisualInventory(standardStashView, stashData);
                    if (enableDebugLogs)
                    {
                        Debug.Log("[InventoryUIManager] Connected to Inventory singleton");
                    }
                }
                else
                {
                    if (enableDebugLogs)
                    {
                        Debug.LogWarning("[InventoryUIManager] Inventory.instance is NULL! Inventory component may be missing.");
                    }
                }
            }
            else
            {
                if (enableDebugLogs)
                {
                    if (standardCore == null)
                    {
                        Debug.LogError("[InventoryUIManager] StandardCore is NULL! Assign it in Inspector.");
                    }
                    if (standardStashView == null)
                    {
                        Debug.LogError("[InventoryUIManager] StandardStashView is NULL! Assign it in Inspector.");
                    }
                }
            }

            //Start with inventory closed
            if (inventoryPanel != null)
            {
                inventoryPanel.SetActive(false);
                if (enableDebugLogs)
                {
                    Debug.Log($"[InventoryUIManager] Inventory panel '{inventoryPanel.name}' set to inactive (closed)");
                }
            }
            else
            {
                if (enableDebugLogs)
                {
                    Debug.LogError("[InventoryUIManager] Inventory Panel is NULL! Cannot show/hide inventory. Assign it in Inspector.");
                }
            }
        }

        private void Update()
        {
            //Toggle inventory with I key
            if (Input.GetKeyDown(toggleKey))
            {
                if (enableDebugLogs)
                {
                    Debug.Log($"[InventoryUIManager] {toggleKey} key pressed! Toggling inventory...");
                }
                ToggleInventory();
            }
        }

        /// <summary>
        /// Open or close the inventory UI
        /// </summary>
        public void ToggleInventory()
        {
            isInventoryOpen = !isInventoryOpen;

            if (enableDebugLogs)
            {
                Debug.Log($"[InventoryUIManager] ToggleInventory() called. New state: {(isInventoryOpen ? "OPEN" : "CLOSED")}");
            }

            if (inventoryPanel != null)
            {
                //Ensure Canvas and all parents are active before activating panel
                //Try GetComponentInParent first (includes inactive)
                Canvas canvas = inventoryPanel.GetComponentInParent<Canvas>(true); //includeInactive = true
                
                //If still not found, try finding any Canvas in the scene
                if (canvas == null)
                {
                    canvas = FindObjectOfType<Canvas>(true); //includeInactive = true
                    if (enableDebugLogs && canvas != null)
                    {
                        Debug.LogWarning($"[InventoryUIManager] Canvas not found as parent, but found Canvas '{canvas.gameObject.name}' in scene.");
                    }
                }
                
                if (enableDebugLogs)
                {
                    if (canvas == null)
                    {
                        Debug.LogError("[InventoryUIManager] Canvas not found! InventoryPanel must be a child of a Canvas.");
                        Debug.LogError($"[InventoryUIManager] InventoryPanel parent: {(inventoryPanel.transform.parent != null ? inventoryPanel.transform.parent.name : "NULL")}");
                    }
                    else
                    {
                        Debug.Log($"[InventoryUIManager] Found Canvas: '{canvas.gameObject.name}'");
                        Debug.Log($"[InventoryUIManager] Canvas activeSelf: {canvas.gameObject.activeSelf}, activeInHierarchy: {canvas.gameObject.activeInHierarchy}");
                    }
                }
                
                //Handle EventSystem duplicates BEFORE activating Canvas (to prevent warnings)
                if (isInventoryOpen)
                {
                    //Disable ALL EventSystems first, then we'll enable only one
                    UnityEngine.EventSystems.EventSystem[] allEventSystems = FindObjectsOfType<UnityEngine.EventSystems.EventSystem>(true);
                    if (allEventSystems.Length > 1)
                    {
                        //Find the first active EventSystem to keep (prefer one not in Canvas if possible)
                        UnityEngine.EventSystems.EventSystem eventSystemToKeep = null;
                        foreach (var es in allEventSystems)
                        {
                            if (es.gameObject.activeInHierarchy)
                            {
                                //Prefer EventSystem not in Canvas hierarchy
                                if (canvas == null || es.transform.root != canvas.transform.root)
                                {
                                    eventSystemToKeep = es;
                                    break;
                                }
                            }
                        }
                        //If all are in Canvas, just keep the first one
                        if (eventSystemToKeep == null && allEventSystems.Length > 0)
                        {
                            eventSystemToKeep = allEventSystems[0];
                        }
                        
                        //Disable all EventSystems temporarily
                        System.Collections.Generic.List<UnityEngine.EventSystems.EventSystem> toDisable = new System.Collections.Generic.List<UnityEngine.EventSystems.EventSystem>();
                        foreach (var es in allEventSystems)
                        {
                            if (es.gameObject.activeInHierarchy && es != eventSystemToKeep)
                            {
                                toDisable.Add(es);
                                es.gameObject.SetActive(false);
                            }
                        }
                        
                        //Re-enable the one we want to keep
                        if (eventSystemToKeep != null && !eventSystemToKeep.gameObject.activeSelf)
                        {
                            eventSystemToKeep.gameObject.SetActive(true);
                        }
                        
                        //Track which ones we disabled
                        disabledEventSystems = toDisable.ToArray();
                        
                        if (enableDebugLogs && disabledEventSystems.Length > 0)
                        {
                            Debug.LogWarning($"[InventoryUIManager] Found {allEventSystems.Length} EventSystems. Disabled {disabledEventSystems.Length} duplicate(s), keeping one active.");
                        }
                    }
                }
                
                //Activate Canvas if it exists and is inactive
                if (canvas != null)
                {
                    if (!canvas.gameObject.activeSelf)
                    {
                        if (enableDebugLogs)
                        {
                            Debug.LogWarning($"[InventoryUIManager] Canvas '{canvas.gameObject.name}' is inactive! Activating it...");
                        }
                        canvasWasActivatedByUs = true; //Remember we activated it
                        canvas.gameObject.SetActive(true);
                    }
                    else
                    {
                        canvasWasActivatedByUs = false; //Canvas was already active
                    }
                    
                    //Check and activate all parent GameObjects up to Canvas
                    Transform parent = inventoryPanel.transform.parent;
                    while (parent != null && parent != canvas.transform)
                    {
                        if (!parent.gameObject.activeSelf)
                        {
                            if (enableDebugLogs)
                            {
                                Debug.LogWarning($"[InventoryUIManager] Activating parent GameObject: '{parent.name}'");
                            }
                            parent.gameObject.SetActive(true);
                        }
                        parent = parent.parent;
                    }
                }
                
                //Handle EventSystems that might have been enabled when Canvas activated
                if (isInventoryOpen)
                {
                    //Check again after Canvas activation and disable any new duplicates
                    UnityEngine.EventSystems.EventSystem[] allEventSystems = FindObjectsOfType<UnityEngine.EventSystems.EventSystem>(true);
                    if (allEventSystems.Length > 1)
                    {
                        //Find which one to keep (prefer one not in Canvas)
                        UnityEngine.EventSystems.EventSystem eventSystemToKeep = null;
                        foreach (var es in allEventSystems)
                        {
                            if (es.gameObject.activeInHierarchy)
                            {
                                if (canvas == null || es.transform.root != canvas.transform.root)
                                {
                                    eventSystemToKeep = es;
                                    break;
                                }
                            }
                        }
                        if (eventSystemToKeep == null && allEventSystems.Length > 0)
                        {
                            eventSystemToKeep = allEventSystems[0];
                        }
                        
                        //Disable duplicates
                        System.Collections.Generic.List<UnityEngine.EventSystems.EventSystem> toDisable = new System.Collections.Generic.List<UnityEngine.EventSystems.EventSystem>();
                        foreach (var es in allEventSystems)
                        {
                            if (es.gameObject.activeInHierarchy && es != eventSystemToKeep)
                            {
                                if (!toDisable.Contains(es))
                                {
                                    toDisable.Add(es);
                                    es.gameObject.SetActive(false);
                                }
                            }
                        }
                        
                        //Update disabled list
                        if (disabledEventSystems == null || disabledEventSystems.Length == 0)
                        {
                            disabledEventSystems = toDisable.ToArray();
                        }
                        else
                        {
                            //Merge with existing disabled list
                            var merged = new System.Collections.Generic.List<UnityEngine.EventSystems.EventSystem>(disabledEventSystems);
                            foreach (var es in toDisable)
                            {
                                if (!merged.Contains(es))
                                {
                                    merged.Add(es);
                                }
                            }
                            disabledEventSystems = merged.ToArray();
                        }
                    }
                }
                
                //Re-enable EventSystems we disabled when closing inventory
                if (!isInventoryOpen)
                {
                    if (disabledEventSystems != null && disabledEventSystems.Length > 0)
                    {
                        foreach (var eventSystem in disabledEventSystems)
                        {
                            if (eventSystem != null)
                            {
                                eventSystem.gameObject.SetActive(true);
                            }
                        }
                        disabledEventSystems = null;
                        
                        if (enableDebugLogs)
                        {
                            Debug.Log("[InventoryUIManager] Re-enabled EventSystems that were disabled for inventory");
                        }
                    }
                }
                
                //Now activate/deactivate the panel
                inventoryPanel.SetActive(isInventoryOpen);
                
                //If closing inventory, always deactivate the Canvas (if it's the inventory Canvas)
                if (!isInventoryOpen)
                {
                    if (canvas != null)
                    {
                        //Always deactivate Canvas when closing inventory
                        if (enableDebugLogs)
                        {
                            Debug.Log($"[InventoryUIManager] Deactivating Canvas '{canvas.gameObject.name}' (closing inventory)");
                        }
                        canvas.gameObject.SetActive(false);
                        canvasWasActivatedByUs = false;
                    }
                }
                
                if (enableDebugLogs)
                {
                    Debug.Log($"[InventoryUIManager] Inventory panel '{inventoryPanel.name}' set to: {(isInventoryOpen ? "ACTIVE" : "INACTIVE")}");
                    Debug.Log($"[InventoryUIManager] Panel activeSelf: {inventoryPanel.activeSelf}, activeInHierarchy: {inventoryPanel.activeInHierarchy}");
                    
                    //Check Canvas status again
                    if (canvas != null)
                    {
                        Debug.Log($"[InventoryUIManager] Canvas '{canvas.gameObject.name}' - activeSelf: {canvas.gameObject.activeSelf}, activeInHierarchy: {canvas.gameObject.activeInHierarchy}");
                        
                        if (!inventoryPanel.activeInHierarchy && isInventoryOpen)
                        {
                            Debug.LogError($"[InventoryUIManager] Panel is still inactive in hierarchy after activation!");
                            Debug.LogError($"[InventoryUIManager] Panel activeSelf: {inventoryPanel.activeSelf}, activeInHierarchy: {inventoryPanel.activeInHierarchy}");
                            Debug.LogError($"[InventoryUIManager] Check if Canvas or any parent GameObject is inactive.");
                            
                            //Try to activate all parents again (including Canvas)
                            Transform checkParent = inventoryPanel.transform.parent;
                            while (checkParent != null)
                            {
                                if (!checkParent.gameObject.activeSelf)
                                {
                                    Debug.LogWarning($"[InventoryUIManager] Found inactive parent: '{checkParent.name}', activating...");
                                    checkParent.gameObject.SetActive(true);
                                }
                                checkParent = checkParent.parent;
                            }
                            
                            //Force Canvas active
                            if (canvas != null && !canvas.gameObject.activeSelf)
                            {
                                Debug.LogWarning($"[InventoryUIManager] Force activating Canvas: '{canvas.gameObject.name}'");
                                canvas.gameObject.SetActive(true);
                            }
                        }
                    }
                }
            }
            else
            {
                if (enableDebugLogs)
                {
                    Debug.LogError("[InventoryUIManager] Cannot toggle inventory - inventoryPanel is NULL!");
                    Debug.LogError("[InventoryUIManager] Fix: Assign InventoryPanel GameObject to 'inventoryPanel' field in Inspector.");
                }
                return;
            }

            //Lock/unlock cursor when opening/closing inventory
            if (isInventoryOpen)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                
                if (enableDebugLogs)
                {
                    Debug.Log("[InventoryUIManager] Cursor unlocked and made visible");
                }
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                
                if (enableDebugLogs)
                {
                    Debug.Log("[InventoryUIManager] Cursor locked and hidden");
                }
            }
        }

        /// <summary>
        /// Open the inventory
        /// </summary>
        public void OpenInventory()
        {
            if (!isInventoryOpen)
            {
                ToggleInventory();
            }
        }

        /// <summary>
        /// Close the inventory
        /// </summary>
        public void CloseInventory()
        {
            if (isInventoryOpen)
            {
                ToggleInventory();
            }
        }
    }
}

