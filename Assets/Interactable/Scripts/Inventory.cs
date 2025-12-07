
namespace EJETAGame
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using VariableInventorySystem;

    /// <summary>
    /// Simple inventory system to store items (like keys) that the player collects.
    /// Uses singleton pattern for easy access from anywhere.
    /// Can sync with VariableInventorySystem for visual display.
    /// </summary>
    public class Inventory : MonoBehaviour
    {
        public static Inventory instance { get; private set; }

        private HashSet<string> items = new HashSet<string>(); //Store unique item IDs/names
        
        //VariableInventorySystem integration
        [SerializeField] private bool useVisualInventory = true;
        [SerializeField] private StandardStashViewData stashData;
        [SerializeField] private StandardStashView stashView;
        private Dictionary<string, KeyCellData> keyDataMap = new Dictionary<string, KeyCellData>(); //Map key names to their cell data

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(this);
            }
            else
            {
                instance = this;
            }
        }

        /// <summary>
        /// Initialize the visual inventory system. Call this after setting up stashView and stashData.
        /// </summary>
        public void InitializeVisualInventory(StandardStashView view, StandardStashViewData data)
        {
            stashView = view;
            stashData = data;
            useVisualInventory = (view != null && data != null);
            
            if (useVisualInventory)
            {
                Debug.Log("Visual inventory initialized");
            }
        }

        /// <summary>
        /// Add an item to the inventory
        /// </summary>
        /// <param name="itemName">The name/ID of the item to add</param>
        /// <param name="imagePath">Path to the image asset for visual inventory (default: "Image/key")</param>
        /// <returns>True if item was added, false if it already exists</returns>
        public bool AddItem(string itemName, string imagePath = "Image/key")
        {
            if (items.Contains(itemName))
            {
                Debug.Log($"Item '{itemName}' already in inventory");
                return false;
            }

            items.Add(itemName);
            Debug.Log($"Added '{itemName}' to inventory");

            //Add to visual inventory if enabled
            if (useVisualInventory && stashData != null && stashView != null)
            {
                var keyCellData = new KeyCellData(itemName, imagePath);
                keyDataMap[itemName] = keyCellData;

                //Find an empty slot and insert the key
                var insertId = stashData.GetInsertableId(keyCellData);
                if (insertId.HasValue)
                {
                    stashData.InsertInventoryItem(insertId.Value, keyCellData);
                    
                    //Ensure the entire parent hierarchy is active before applying (cells need to be active to start coroutines)
                    //Activate Canvas -> InventoryPanel -> StandardStashView -> cells
                    Canvas canvas = stashView.GetComponentInParent<Canvas>(true); //includeInactive
                    GameObject inventoryPanel = stashView.transform.parent?.gameObject;
                    bool wasCanvasActive = canvas != null ? canvas.gameObject.activeSelf : true;
                    bool wasPanelActive = inventoryPanel != null ? inventoryPanel.activeSelf : true;
                    bool wasStashViewActive = stashView.gameObject.activeSelf;
                    
                    //Activate Canvas if it exists and is inactive
                    if (canvas != null && !wasCanvasActive)
                    {
                        canvas.gameObject.SetActive(true);
                    }
                    
                    //Activate parent panel if it exists and is inactive
                    if (inventoryPanel != null && !wasPanelActive)
                    {
                        inventoryPanel.SetActive(true);
                    }
                    
                    //Activate stash view if inactive
                    if (!wasStashViewActive)
                    {
                        stashView.gameObject.SetActive(true);
                    }
                    
                    //Wait a frame to ensure everything is active before applying
                    //This ensures cells are created as active
                    //Use this MonoBehaviour (Inventory) to start coroutine since it's always active
                    StartCoroutine(ApplyStashViewDelayed(stashView, stashData, wasCanvasActive, wasPanelActive, wasStashViewActive, canvas, inventoryPanel));
                    
                    Debug.Log($"Added '{itemName}' to visual inventory");
                }
                else
                {
                    Debug.LogWarning($"No space in inventory for '{itemName}'");
                }
            }

            return true;
        }

        /// <summary>
        /// Check if the player has a specific item
        /// </summary>
        /// <param name="itemName">The name/ID of the item to check</param>
        /// <returns>True if the item exists in inventory</returns>
        public bool HasItem(string itemName)
        {
            return items.Contains(itemName);
        }

        /// <summary>
        /// Remove an item from the inventory
        /// </summary>
        /// <param name="itemName">The name/ID of the item to remove</param>
        /// <returns>True if item was removed, false if it didn't exist</returns>
        public bool RemoveItem(string itemName)
        {
            if (items.Remove(itemName))
            {
                Debug.Log($"Removed '{itemName}' from inventory");

                //Remove from visual inventory if enabled
                if (useVisualInventory && stashData != null && stashView != null && keyDataMap.ContainsKey(itemName))
                {
                    var keyCellData = keyDataMap[itemName];
                    var itemId = stashData.GetId(keyCellData);
                    
                    if (itemId.HasValue)
                    {
                        stashData.InsertInventoryItem(itemId.Value, null); //Set to null to remove
                        stashView.Apply(stashData);
                    }
                    
                    keyDataMap.Remove(itemName);
                }

                return true;
            }
            return false;
        }

        /// <summary>
        /// Get all items in the inventory
        /// </summary>
        /// <returns>Array of all item names</returns>
        public string[] GetAllItems()
        {
            string[] itemArray = new string[items.Count];
            items.CopyTo(itemArray);
            return itemArray;
        }

        /// <summary>
        /// Coroutine to apply stash view data after ensuring everything is active
        /// </summary>
        private IEnumerator ApplyStashViewDelayed(StandardStashView view, StandardStashViewData data, 
            bool wasCanvasActive, bool wasPanelActive, bool wasStashViewActive, 
            Canvas canvas, GameObject inventoryPanel)
        {
            //Wait one frame to ensure all GameObjects are fully active
            yield return null;
            
            //Now apply - cells will be created as active
            view.Apply(data);
            
            //Restore original states if we changed them (only if inventory is closed)
            //Check if inventory is open by checking if panel is still active
            if (inventoryPanel != null && !inventoryPanel.activeSelf)
            {
                if (!wasStashViewActive)
                {
                    view.gameObject.SetActive(false);
                }
                if (!wasPanelActive)
                {
                    inventoryPanel.SetActive(false);
                }
                if (canvas != null && !wasCanvasActive)
                {
                    canvas.gameObject.SetActive(false);
                }
            }
        }
    }
}

