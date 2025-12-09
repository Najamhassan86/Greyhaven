using UnityEngine;
using UnityEngine.UI;

namespace VariableInventorySystem
{
    public class StandardCell : VariableInventoryCell
    {
        [SerializeField] Vector2 defaultCellSize;
        [SerializeField] Vector2 margineSpace;

        [SerializeField] RectTransform sizeRoot;
        [SerializeField] RectTransform target;
        [SerializeField] Graphic background;
        [SerializeField] RawImage cellImage;
        [SerializeField] Graphic highlight;

        [SerializeField] StandardButton button;

        public override Vector2 DefaultCellSize => defaultCellSize;
        public override Vector2 MargineSpace => margineSpace;
        protected override IVariableInventoryCellActions ButtonActions => button;
        protected virtual StandardAssetLoader Loader { get; set; }

        protected bool isSelectable = true;
        protected IVariableInventoryAsset currentImageAsset;

        public Vector2 GetCellSize()
        {
            var width = ((CellData?.Width ?? 1) * (defaultCellSize.x + margineSpace.x)) - margineSpace.x;
            var height = ((CellData?.Height ?? 1) * (defaultCellSize.y + margineSpace.y)) - margineSpace.y;
            return new Vector2(width, height);
        }

        public Vector2 GetRotateCellSize()
        {
            var isRotate = CellData?.IsRotate ?? false;
            var cellSize = GetCellSize();
            if (isRotate)
            {
                var tmp = cellSize.x;
                cellSize.x = cellSize.y;
                cellSize.y = tmp;
            }

            return cellSize;
        }

        public override void SetSelectable(bool value)
        {
            ButtonActions.SetActive(value);
            isSelectable = value;
        }

        public virtual void SetHighLight(bool value)
        {
            highlight.gameObject.SetActive(value);
        }

        protected override void OnApply()
        {
            SetHighLight(false);
            target.gameObject.SetActive(CellData != null);
            ApplySize();

            if (CellData == null)
            {
                cellImage.gameObject.SetActive(false);
                background.gameObject.SetActive(false);
            }
            else
            {
                // update cell image
                if (currentImageAsset != CellData.ImageAsset)
                {
                    currentImageAsset = CellData.ImageAsset;

                    //Disable image while loading
                    cellImage.gameObject.SetActive(false);
                    cellImage.texture = null; //Clear previous texture
                    
                    if (Loader == null)
                    {
                        Loader = new StandardAssetLoader();
                    }

                    string imagePath = (CellData.ImageAsset as StandardAsset)?.Path ?? "NULL";
                    Debug.Log($"[StandardCell] Loading image for cell. Path: '{imagePath}'");

                    //Check if GameObject is active before starting coroutine
                    if (!gameObject.activeInHierarchy)
                    {
                        Debug.LogWarning($"[StandardCell] Cannot start coroutine - GameObject '{gameObject.name}' is inactive. Image will load when GameObject becomes active.");
                        //Don't start coroutine if GameObject is inactive
                        return;
                    }

                    StartCoroutine(Loader.LoadAsync(CellData.ImageAsset, tex =>
                    {
                        //Check if GameObject is still active when callback executes
                        if (!gameObject.activeInHierarchy)
                        {
                            return; //Don't apply texture if GameObject is inactive
                        }

                        if (tex != null)
                        {
                            cellImage.texture = tex;
                            cellImage.color = Color.white; //Ensure color is white (not tinted)
                            cellImage.gameObject.SetActive(true);
                            Debug.Log($"[StandardCell] ✓ Successfully loaded texture for cell. Texture size: {tex.width}x{tex.height}, Format: {tex.format}");
                        }
                        else
                        {
                            Debug.LogError($"[StandardCell] ✗ Failed to load texture for cell!");
                            Debug.LogError($"[StandardCell] ImageAsset path: '{imagePath}'");
                            Debug.LogError($"[StandardCell] Cell will show background color only. Check:");
                            Debug.LogError($"[StandardCell]   1. File exists at Assets/Resources/{imagePath}.png");
                            Debug.LogError($"[StandardCell]   2. Path is correct (no .png extension)");
                            Debug.LogError($"[StandardCell]   3. Texture import settings allow loading");
                            //Keep image disabled if texture failed to load - this prevents green/white color
                            cellImage.gameObject.SetActive(false);
                            cellImage.texture = null;
                        }
                    }));
                }
                else if (cellImage.texture == null && cellImage.gameObject.activeSelf)
                {
                    //Image asset hasn't changed but texture is null - this shouldn't happen, but handle it
                    Debug.LogWarning($"[StandardCell] Cell image is active but texture is null! Re-loading...");
                    cellImage.gameObject.SetActive(false);
                    //Trigger reload by resetting currentImageAsset
                    currentImageAsset = null;
                }

                background.gameObject.SetActive(true && isSelectable);
            }
        }

        protected virtual void ApplySize()
        {
            sizeRoot.sizeDelta = GetRotateCellSize();
            target.sizeDelta = GetCellSize();
            target.localEulerAngles = Vector3.forward * (CellData?.IsRotate ?? false ? 90 : 0);
        }

        private void OnEnable()
        {
            //If cell becomes active and has data but no texture, reload the image
            if (CellData != null && currentImageAsset == CellData.ImageAsset && 
                (cellImage == null || cellImage.texture == null) && 
                gameObject.activeInHierarchy)
            {
                //Reset currentImageAsset to trigger reload
                currentImageAsset = null;
                //Re-apply to load image
                if (CellData != null)
                {
                    OnApply();
                }
            }
        }
    }
}
