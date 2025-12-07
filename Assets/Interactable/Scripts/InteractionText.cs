
namespace EJETAGame
{
    using TMPro;
    using Unity.VisualScripting;
    using UnityEngine;

    public class InteractionText : MonoBehaviour
    {
        public static InteractionText instance { get; private set; }

        public TextMeshProUGUI textAppear;
        
        [Header("Debug Settings")]
        [SerializeField] private bool enableDebugLogs = true;
        
        private static bool duplicateWarningShown = false; //Only show warning once per session
        
        private void Awake()
        {
            if(instance!= null && instance != this)
            {
                if (enableDebugLogs && !duplicateWarningShown)
                {
                    Debug.LogWarning($"[InteractionText] Multiple instances found! Destroying duplicate on {gameObject.name}");
                    Debug.LogWarning($"[InteractionText] Keeping instance on '{instance.gameObject.name}', removing duplicate on '{gameObject.name}'");
                    Debug.LogWarning($"[InteractionText] To fix permanently: Remove InteractionText component from '{gameObject.name}' GameObject in Inspector");
                    duplicateWarningShown = true; //Only show once
                }
                Destroy(this);
                return; //Exit early to prevent further initialization
            }
            else
            {
                instance = this;
                
                //Try to auto-find textAppear if not assigned
                if (textAppear == null)
                {
                    //Look for TextMeshProUGUI in children
                    textAppear = GetComponentInChildren<TextMeshProUGUI>();
                    
                    if (textAppear == null)
                    {
                        //Look in this GameObject
                        textAppear = GetComponent<TextMeshProUGUI>();
                    }
                    
                    if (textAppear != null && enableDebugLogs)
                    {
                        Debug.Log($"[InteractionText] Auto-found textAppear: {textAppear.gameObject.name}");
                    }
                }
                
                if (enableDebugLogs)
                {
                    Debug.Log($"[InteractionText] Instance initialized on {gameObject.name}");
                    if (textAppear == null)
                    {
                        Debug.LogError("[InteractionText] textAppear is NULL! Assign TextMeshProUGUI in Inspector.");
                        Debug.LogError("[InteractionText] Looking for TextMeshProUGUI in children...");
                        
                        //List all TextMeshProUGUI components in scene for debugging
                        TextMeshProUGUI[] allTexts = FindObjectsOfType<TextMeshProUGUI>();
                        Debug.LogError($"[InteractionText] Found {allTexts.Length} TextMeshProUGUI component(s) in scene:");
                        foreach (var txt in allTexts)
                        {
                            Debug.LogError($"  - {txt.gameObject.name} (on {txt.transform.parent?.name ?? "root"})");
                        }
                    }
                    else
                    {
                        Debug.Log($"[InteractionText] textAppear assigned to: {textAppear.gameObject.name}");
                    }
                }
            }
        }

        public void SetText(string text)
        {
            if (textAppear == null)
            {
                if (enableDebugLogs)
                {
                    Debug.LogError("[InteractionText] SetText called but textAppear is NULL!");
                    Debug.LogError("[InteractionText] Fix: Assign TextMeshProUGUI to 'textAppear' field in Inspector.");
                }
                return;
            }
            
            //Make sure text GameObject is active
            if (!textAppear.gameObject.activeSelf)
            {
                if (enableDebugLogs)
                {
                    Debug.LogWarning($"[InteractionText] Text GameObject '{textAppear.gameObject.name}' was inactive, activating it...");
                }
                textAppear.gameObject.SetActive(true);
            }
            
            //Make sure Canvas is active
            Canvas canvas = textAppear.GetComponentInParent<Canvas>();
            if (canvas != null && !canvas.gameObject.activeInHierarchy)
            {
                if (enableDebugLogs)
                {
                    Debug.LogWarning($"[InteractionText] Canvas '{canvas.gameObject.name}' was inactive, activating it...");
                }
                canvas.gameObject.SetActive(true);
            }
            
            textAppear.SetText(text);
            
            //Check text visibility
            if (enableDebugLogs)
            {
                Debug.Log($"[InteractionText] Text set to: \"{text}\"");
                Debug.Log($"[InteractionText] Text GameObject: {textAppear.gameObject.name}, Active: {textAppear.gameObject.activeInHierarchy}");
                Debug.Log($"[InteractionText] Text Color Alpha: {textAppear.color.a:F2} (should be > 0.01)");
                Debug.Log($"[InteractionText] Text Enabled: {textAppear.enabled}");
                
                if (textAppear.color.a < 0.01f)
                {
                    Debug.LogWarning("[InteractionText] Text alpha is very low! Text may be invisible. Set color alpha to 1.0");
                }
            }
        }

        /// <summary>
        /// Hide the interaction text (deactivate the text GameObject)
        /// </summary>
        public void HideText()
        {
            if (textAppear != null)
            {
                textAppear.gameObject.SetActive(false);
                
                if (enableDebugLogs)
                {
                    Debug.Log("[InteractionText] Text hidden");
                }
            }
        }

        /// <summary>
        /// Clear the text and hide it
        /// </summary>
        public void ClearText()
        {
            if (textAppear != null)
            {
                textAppear.SetText("");
                textAppear.gameObject.SetActive(false);
                
                if (enableDebugLogs)
                {
                    Debug.Log("[InteractionText] Text cleared and hidden");
                }
            }
        }

    }

}