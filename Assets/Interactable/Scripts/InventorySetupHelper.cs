
namespace EJETAGame
{
    using UnityEngine;
    using UnityEngine.UI;
    using VariableInventorySystem;

    /// <summary>
    /// Helper script to automatically set up the visual inventory UI.
    /// Attach this to a GameObject and click "Setup Inventory UI" in the inspector,
    /// or call SetupInventoryUI() from code.
    /// </summary>
    [System.Serializable]
    public class InventorySetupHelper : MonoBehaviour
    {
        [Header("Setup Options")]
        [SerializeField] private bool createCanvas = true;
        [SerializeField] private bool useExistingCanvas = false;
        [SerializeField] private Canvas existingCanvas;
        
        [Header("Inventory Settings")]
        [SerializeField] private int inventoryWidth = 8;
        [SerializeField] private Vector2 cellSize = new Vector2(64, 64);
        [SerializeField] private Vector2 cellSpacing = new Vector2(2, 2);

        [Header("Prefab References")]
        [SerializeField] private GameObject standardCellPrefab;
        [SerializeField] private GameObject standardCasePopupPrefab;

        [ContextMenu("Setup Inventory UI")]
        public void SetupInventoryUI()
        {
            Canvas canvas = GetOrCreateCanvas();
            if (canvas == null)
            {
                Debug.LogError("Failed to create or find Canvas!");
                return;
            }

            GameObject panel = CreateInventoryPanel(canvas);
            GameObject stashView = CreateStandardStashView(panel);
            GameObject core = CreateStandardCore(panel);

            Debug.Log("Inventory UI setup complete! Now assign references in InventoryUIManager:");
            Debug.Log($"- Inventory Panel: {panel.name}");
            Debug.Log($"- Standard Core: {core.name}");
            Debug.Log($"- Standard Stash View: {stashView.name}");
        }

        private Canvas GetOrCreateCanvas()
        {
            if (useExistingCanvas && existingCanvas != null)
            {
                return existingCanvas;
            }

            Canvas canvas = FindObjectOfType<Canvas>();
            if (canvas != null && !createCanvas)
            {
                return canvas;
            }

            if (createCanvas)
            {
                GameObject canvasObj = new GameObject("InventoryCanvas");
                canvas = canvasObj.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvasObj.AddComponent<CanvasScaler>();
                canvasObj.AddComponent<GraphicRaycaster>();

                CanvasScaler scaler = canvas.GetComponent<CanvasScaler>();
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1920, 1080);
                scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
                scaler.matchWidthOrHeight = 0.5f;

                // Add EventSystem if it doesn't exist
                if (FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
                {
                    GameObject eventSystem = new GameObject("EventSystem");
                    eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
                    eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
                }

                return canvas;
            }

            return null;
        }

        private GameObject CreateInventoryPanel(Canvas canvas)
        {
            GameObject panel = new GameObject("InventoryPanel");
            panel.transform.SetParent(canvas.transform, false);

            RectTransform rectTransform = panel.AddComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            rectTransform.sizeDelta = new Vector2(600, 700);
            rectTransform.anchoredPosition = Vector2.zero;

            Image image = panel.AddComponent<Image>();
            image.color = new Color(0, 0, 0, 200f / 255f);

            return panel;
        }

        private GameObject CreateStandardStashView(GameObject parent)
        {
            // Main StashView GameObject
            GameObject stashView = new GameObject("StandardStashView");
            stashView.transform.SetParent(parent.transform, false);
            RectTransform stashRect = stashView.AddComponent<RectTransform>();
            stashRect.anchorMin = Vector2.zero;
            stashRect.anchorMax = Vector2.one;
            stashRect.sizeDelta = Vector2.zero;
            stashRect.anchoredPosition = Vector2.zero;

            StandardStashView stashViewComponent = stashView.AddComponent<StandardStashView>();

            // ScrollView
            GameObject scrollView = new GameObject("ScrollView");
            scrollView.transform.SetParent(stashView.transform, false);
            RectTransform scrollRect = scrollView.AddComponent<RectTransform>();
            scrollRect.anchorMin = Vector2.zero;
            scrollRect.anchorMax = Vector2.one;
            scrollRect.sizeDelta = new Vector2(-20, -20);
            scrollRect.anchoredPosition = Vector2.zero;

            ScrollRect scrollRectComponent = scrollView.AddComponent<ScrollRect>();
            Image scrollImage = scrollView.AddComponent<Image>();
            scrollImage.color = new Color(0, 0, 0, 0);

            // Content
            GameObject content = new GameObject("Content");
            content.transform.SetParent(scrollView.transform, false);
            RectTransform contentRect = content.AddComponent<RectTransform>();
            contentRect.anchorMin = new Vector2(0, 1);
            contentRect.anchorMax = new Vector2(0, 1);
            contentRect.pivot = new Vector2(0, 1);
            contentRect.sizeDelta = new Vector2(0, 0);
            contentRect.anchoredPosition = Vector2.zero;

            GridLayoutGroup gridLayout = content.AddComponent<GridLayoutGroup>();
            gridLayout.cellSize = cellSize;
            gridLayout.spacing = cellSpacing;
            gridLayout.startCorner = GridLayoutGroup.Corner.UpperLeft;
            gridLayout.startAxis = GridLayoutGroup.Axis.Horizontal;
            gridLayout.childAlignment = TextAnchor.UpperLeft;
            gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            gridLayout.constraintCount = inventoryWidth;

            ContentSizeFitter sizeFitter = content.AddComponent<ContentSizeFitter>();
            sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            scrollRectComponent.content = contentRect;
            scrollRectComponent.horizontal = false;
            scrollRectComponent.vertical = true;

            // Background
            GameObject background = new GameObject("Background");
            background.transform.SetParent(content.transform, false);
            background.transform.SetAsFirstSibling();
            RectTransform bgRect = background.AddComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.sizeDelta = Vector2.zero;
            bgRect.anchoredPosition = Vector2.zero;

            Image bgImage = background.AddComponent<Image>();
            bgImage.color = new Color(0.08f, 0.08f, 0.08f, 1f);

            // Condition Indicator
            GameObject conditionIndicator = new GameObject("ConditionIndicator");
            conditionIndicator.transform.SetParent(stashView.transform, false);
            RectTransform conditionRect = conditionIndicator.AddComponent<RectTransform>();
            conditionRect.sizeDelta = cellSize;
            conditionRect.anchoredPosition = Vector2.zero;

            Image conditionImage = conditionIndicator.AddComponent<Image>();
            conditionImage.color = new Color(1, 1, 1, 0);

            // Configure StandardStashView component
            if (standardCellPrefab != null)
            {
                // Use reflection or set via SerializedObject to set private fields
                // For now, user will need to set these manually in inspector
                Debug.LogWarning("Please manually assign StandardCell prefab to StandardStashView component in inspector!");
            }

            // Set public fields via reflection (if needed)
            var stashViewType = typeof(StandardStashView);
            var scrollRectField = stashViewType.GetField("scrollRect", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var gridLayoutField = stashViewType.GetField("gridLayoutGroup", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var conditionField = stashViewType.GetField("condition", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var conditionTransformField = stashViewType.GetField("conditionTransform", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var backgroundField = stashViewType.GetField("background", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            if (scrollRectField != null) scrollRectField.SetValue(stashViewComponent, scrollRectComponent);
            if (gridLayoutField != null) gridLayoutField.SetValue(stashViewComponent, gridLayout);
            if (conditionField != null) conditionField.SetValue(stashViewComponent, conditionImage);
            if (conditionTransformField != null) conditionTransformField.SetValue(stashViewComponent, conditionRect);
            if (backgroundField != null) backgroundField.SetValue(stashViewComponent, bgRect);

            Debug.Log("StandardStashView created. Please assign StandardCell prefab in inspector!");

            return stashView;
        }

        private GameObject CreateStandardCore(GameObject parent)
        {
            GameObject core = new GameObject("StandardCore");
            core.transform.SetParent(parent.transform, false);
            core.AddComponent<RectTransform>();

            StandardCore coreComponent = core.AddComponent<StandardCore>();

            // Effect Cell Parent
            GameObject effectCellParent = new GameObject("EffectCellParent");
            effectCellParent.transform.SetParent(core.transform, false);
            effectCellParent.AddComponent<RectTransform>();

            // Case Parent (optional)
            GameObject caseParent = new GameObject("CaseParent");
            caseParent.transform.SetParent(core.transform, false);
            caseParent.AddComponent<RectTransform>();

            // Set fields via reflection
            var coreType = typeof(StandardCore);
            var cellPrefabField = coreType.GetField("cellPrefab", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var casePopupField = coreType.GetField("casePopupPrefab", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var effectCellParentField = coreType.GetField("effectCellParent", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var caseParentField = coreType.GetField("caseParent", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            if (standardCellPrefab != null && cellPrefabField != null)
                cellPrefabField.SetValue(coreComponent, standardCellPrefab);
            
            if (standardCasePopupPrefab != null && casePopupField != null)
                casePopupField.SetValue(coreComponent, standardCasePopupPrefab);

            if (effectCellParentField != null)
                effectCellParentField.SetValue(coreComponent, effectCellParent.GetComponent<RectTransform>());

            if (caseParentField != null)
                caseParentField.SetValue(coreComponent, caseParent.GetComponent<RectTransform>());

            Debug.Log("StandardCore created. Please verify prefab assignments in inspector!");

            return core;
        }
    }
}

