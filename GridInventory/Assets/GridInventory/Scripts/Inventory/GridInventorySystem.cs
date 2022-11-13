using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;
using TMPro;
using UnityEditor.VersionControl;
using UnityEngine.EventSystems;

namespace GridInventory
{
    public class GridInventorySystem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private List<ItemsCollection> availableCollections;

        [SerializeField] private int gridWidth = 6;
        [SerializeField] private int gridHeight = 4;
        [SerializeField] private float cellSize = 100f;

        [Tooltip("Number of cells")]
        [SerializeField] private int viewportHeight;
        [SerializeField] private float cells_Scroll_Spacing;
        [SerializeField] private float scrollBar_Width = 5f;
        [SerializeField] private float header_Heigth = 12f;

        // Prefabs
        [Header("Inventory Prefabs")]
        [SerializeField] private bool setupPrefabs;
        [ShowIf("setupPrefabs")]
        [SerializeField] private GameObject cellPrefab;

        // Internal variables
        Vector2 halfCellSize;
        private ItemsCollection targetItemCollection;
        private GhostItem ghostItem;

        [SerializeField] ItemsCollection lastItemCollection;
        InventoryItem iteract_InventoryItem;
        Dir oldDir;
        Vector2Int oldPivot;

        GameObject slots;
        RectTransform rectSlots;

        #region Events
        public delegate void AChangeCollection();
        public static event AChangeCollection OnChangeCollection;
        #endregion


        private void Awake()
        {
            slots = transform.Find("Scroll/Viewport/Slots").gameObject;
            rectSlots = transform.Find("Scroll/Viewport/Slots").GetComponent<RectTransform>();

            targetItemCollection = GetComponent<ItemsCollection>();
        }

        private void Start()
        {
            ghostItem = GetComponentInChildren<GhostItem>();
            ghostItem.Collection = targetItemCollection;
            ghostItem.RectSlots = rectSlots;
            ghostItem.gameObject.SetActive(false);
        }

        private void Update()
        {
            DefineTargetCollection();

            Vector2Int pos_OnGrid = targetItemCollection.GetCellXY(Input.mousePosition);
            //Debug.Log(pos_OnGrid);

            if (Input.GetMouseButtonDown(0))
            {
                InventoryItem item = null;
                foreach (var collection in availableCollections)
                {
                    var _item = collection.GetInventoryItem(Input.mousePosition);
                    if (_item != null)
                    {
                        item = _item;
                        lastItemCollection = collection;
                    }
                }

                if (item != null)
                {
                    iteract_InventoryItem = item;
                    
                    oldDir = iteract_InventoryItem.Dir;
                    oldPivot = iteract_InventoryItem.GridPostionList[0];

                    iteract_InventoryItem.transform.SetParent(transform, false);
                    ghostItem.gameObject.SetActive(true);
                    ghostItem.Ghost_InventoryItem = iteract_InventoryItem;
                }
            }

            if (Input.GetMouseButton(0))
            {
                if (iteract_InventoryItem == null)
                    return;

                iteract_InventoryItem.transform.position = Input.mousePosition;
            }

            if (Input.GetMouseButtonUp(0))
            {
                if (iteract_InventoryItem == null)
                    return;

                bool isVaildPostion = targetItemCollection.IsValidPosition(Input.mousePosition, iteract_InventoryItem.ItemData.Width, iteract_InventoryItem.ItemData.Height,
                    iteract_InventoryItem.Dir);

                if (isVaildPostion)
                {
                    iteract_InventoryItem.GridPostionList = InventoryItem.CalculatePositionList(iteract_InventoryItem.Dir,
                        iteract_InventoryItem.ItemData.Width, iteract_InventoryItem.ItemData.Height, targetItemCollection.GetCellXY(Input.mousePosition));

                    targetItemCollection.AddInventoryItem(iteract_InventoryItem);
                    clearIteract_InventoryItem();
                }
                else
                {
                    targetItemCollection = lastItemCollection;
                    targetItemCollection.AddNewInventoryItem(oldPivot, iteract_InventoryItem.ItemData, oldDir);
                    GameObject.Destroy(iteract_InventoryItem.gameObject);

                    clearIteract_InventoryItem();
                }
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (iteract_InventoryItem == null)
                    return;

                iteract_InventoryItem.Dir = InventoryUtilities.GetNextDir(iteract_InventoryItem.Dir);
            }
        }

        private void DefineTargetCollection()
        {
            if (iteract_InventoryItem == null)
                return;

            bool isValidPosition = false;

            foreach (var collection in availableCollections)
            {
                isValidPosition = collection.IsValidPosition(Input.mousePosition, iteract_InventoryItem.ItemData.Width, iteract_InventoryItem.ItemData.Height, iteract_InventoryItem.Dir);
                if (isValidPosition)
                {
                    targetItemCollection = collection;
                    ghostItem.Collection = collection;
                    return;
                }
            }

            if (!isValidPosition)
                targetItemCollection = lastItemCollection;

        }

        private void clearIteract_InventoryItem()
        {
            iteract_InventoryItem = null;
            lastItemCollection = null;
            ghostItem.Ghost_InventoryItem = null;
            ghostItem.gameObject.SetActive(false);
        }

        /*
        [Button("Generate inventory")]
        private void InventoryGeneration()
        {
            ClearExitedCells();
            CreateBackground();
            CreateHeader();
            CreateScroll();

            halfCellSize = new Vector2(cellSize / 2, cellSize / 2);

            if (viewportHeight > gridHeight)
                viewportHeight = gridHeight;

            Vector2 cellsAreaSize = new Vector2(cellSize * gridWidth, cellSize * gridHeight);
            Vector2 viewportSize = new Vector2(cellsAreaSize.x, cellSize * viewportHeight);

            transform.GetComponent<RectTransform>().sizeDelta = new Vector2(viewportSize.x + cells_Scroll_Spacing + scrollBar_Width, viewportSize.y);

            GenerateGrid();
        }

        private void ClearExitedCells()
        {
            if (slots == null)
                slots = transform.Find("Scroll/Viewport/Slots").gameObject;

            if (slots.transform.childCount > 0)
            {
                for (int i = 0; i < slots.transform.childCount; i++)
                {
                    DestroyImmediate(slots.transform.GetChild(i).gameObject);
                }
            }
        }

        private void CreateBackground()
        {
            if (transform.Find("Background") == null)
            {
                GameObject background = new GameObject("Background");
                background.transform.parent = transform;

                RectTransform rectBackground = background.GetComponent<RectTransform>();
                rectBackground.anchorMin = new Vector2(0, 0);
                rectBackground.anchorMax = new Vector2(1, 1);
                rectBackground.pivot = new Vector2(0.5f, .5f);

                Image backgroundImage = background.AddComponent<Image>();
                backgroundImage.sprite = Resources.Load<Sprite>("Assets/GridInventory/GUI/Square.png");
                backgroundImage.color = new Color(0, 0, 0, 0.4f);
                backgroundImage.type = Image.Type.Sliced;
            }

            if (transform.Find("Background Outline") == null)
            {
                GameObject backgroundOutline = new GameObject("Background Outline");
                backgroundOutline.transform.parent = transform;

                RectTransform rectBackgroundOutline = backgroundOutline.GetComponent<RectTransform>();
                rectBackgroundOutline.anchorMin = new Vector2(0, 0);
                rectBackgroundOutline.anchorMax = new Vector2(1, 1);
                rectBackgroundOutline.pivot = new Vector2(0.5f, .5f);

                Image backgroundOutlineImage = backgroundOutline.AddComponent<Image>();
                backgroundOutlineImage.sprite = Resources.Load<Sprite>("Assets/GridInventory/GUI/Square Outline.png");
                backgroundOutlineImage.color = new Color(0, 0, 0, 0.4f);
                backgroundOutlineImage.type = Image.Type.Sliced;
            }
        }

        private void CreateHeader()
        {
            if (transform.Find("Header") == null)
            {
                GameObject header = new GameObject("Header");
                header.transform.parent = transform;

                RectTransform rectHeader = header.GetComponent<RectTransform>();
                rectHeader.anchorMin = new Vector2(0, 1);
                rectHeader.anchorMax = new Vector2(1, 1);
                rectHeader.pivot = new Vector2(0.5f, 0f);
                rectHeader.sizeDelta = new Vector2(0, header_Heigth);

                Image headerImage = header.AddComponent<Image>();
                headerImage.sprite = Resources.Load<Sprite>("Assets/GridInventory/GUI/Header.png");
                headerImage.color = new Color(0, 0, 0, 1);
                headerImage.type = Image.Type.Sliced;

                //
                GameObject title = new GameObject("Title");
                title.transform.parent = header.transform;

                RectTransform rectTitle = title.GetComponent<RectTransform>();
                rectTitle.anchorMin = new Vector2(0, 0);
                rectTitle.anchorMax = new Vector2(1, 1);
                rectTitle.pivot = new Vector2(0.5f, 1f);

                TextMeshPro textMeshTitle = title.AddComponent<TextMeshPro>();
                textMeshTitle.text = "Inventory";
                textMeshTitle.fontStyle = (FontStyles)FontStyle.Bold;
                textMeshTitle.enableAutoSizing = true;
                textMeshTitle.fontSizeMin = 5;
                textMeshTitle.fontSizeMax = 72;
                textMeshTitle.alignment = (TextAlignmentOptions)TextAnchor.MiddleLeft;

            }
        }

        private void CreateScroll()
        {
            if (transform.Find("Scroll") == null)
            {
                GameObject scroll = new GameObject("Scroll");
                scroll.transform.parent = transform;

                RectTransform rectScroll = scroll.GetComponent<RectTransform>();
                rectScroll.anchorMin = new Vector2(0, 1);
                rectScroll.anchorMax = new Vector2(1, 1);
                rectScroll.pivot = new Vector2(0.5f, 0.5f);

                //Viewport
                GameObject Viewport = new GameObject("Viewport");
                scroll.transform.parent = scroll.transform;

                RectTransform rectViewport = Viewport.GetComponent<RectTransform>();
                rectViewport.anchorMin = new Vector2(0, 0);
                rectViewport.anchorMax = new Vector2(1, 1);
                rectViewport.pivot = new Vector2(0.5f, 0.5f);

                Viewport.AddComponent<Mask>();

                Image ViewportImage = Viewport.AddComponent<Image>();
                ViewportImage.sprite = Resources.Load<Sprite>("Resources/unity_builtin_extra/UIMask.png");
                ViewportImage.color = Color.white;
                ViewportImage.type = Image.Type.Sliced;

                //Slots
                slots = new GameObject("Slots");
                slots.transform.parent = Viewport.transform;

                rectSlots = slots.GetComponent<RectTransform>();
                rectSlots.anchorMin = new Vector2(0, 1);
                rectSlots.anchorMax = new Vector2(1, 1);
                rectSlots.pivot = new Vector2(0f, 1f);

                GridLayoutGroup gridLayoutGroup = slots.AddComponent<GridLayoutGroup>();
                gridLayoutGroup.cellSize = new Vector2(cellSize, cellSize);
                gridLayoutGroup.startCorner = GridLayoutGroup.Corner.UpperLeft;
                gridLayoutGroup.startAxis = GridLayoutGroup.Axis.Horizontal;
                gridLayoutGroup.childAlignment = TextAnchor.UpperLeft;
                gridLayoutGroup.constraint = GridLayoutGroup.Constraint.Flexible;

                // Scrollbar Vertical
                GameObject scrollbar_Vert = new GameObject("Scrollbar Vertical");
                scrollbar_Vert.transform.parent = Viewport.transform;

                RectTransform rectScrollbar_Vert = scrollbar_Vert.GetComponent<RectTransform>();
                rectScrollbar_Vert.anchorMin = new Vector2(1, 0);
                rectScrollbar_Vert.anchorMax = new Vector2(1, 1);
                rectScrollbar_Vert.pivot = new Vector2(0.5f, 0.5f);
                rectScrollbar_Vert.sizeDelta = new Vector2(scrollBar_Width, 0);

                Image scrollbar_VertImage = scrollbar_Vert.AddComponent<Image>();
                ViewportImage.sprite = Resources.Load<Sprite>("Resources/unity_builtin_extra/BackGround.png");
                ViewportImage.color = new Color(.105f, .121f, .133f, 1);
                ViewportImage.type = Image.Type.Sliced;

                // Sliding Area
                GameObject slidingArea = new GameObject("Sliding Area");
                slidingArea.transform.parent = scrollbar_Vert.transform;

                RectTransform rectSlidingArea = slidingArea.GetComponent<RectTransform>();
                rectSlidingArea.anchorMin = new Vector2(0, 0);
                rectSlidingArea.anchorMax = new Vector2(1, 1);
                rectSlidingArea.pivot = new Vector2(0.5f, 0.5f);

                // Handle
                GameObject handle = new GameObject("Handle");
                handle.transform.parent = slidingArea.transform;

                RectTransform rectHandle = handle.GetComponent<RectTransform>();
                //rectHandle.anchorMin = new Vector2(0, 0);
                //rectHandle.anchorMax = new Vector2(1, 1);
                rectHandle.pivot = new Vector2(0.5f, 0.5f);

                Image HandleImage = handle.AddComponent<Image>();
                HandleImage.sprite = Resources.Load<Sprite>("Resources/unity_builtin_extra/UIMask.png");
                HandleImage.color = new Color(.205f, .213f, .247f, 1);
                HandleImage.type = Image.Type.Tiled;

                // Set Scrollbar Settings
                Scrollbar scrollbar_V = scrollbar_Vert.AddComponent<Scrollbar>();
                scrollbar_V.interactable = true;
                scrollbar_V.transition = Selectable.Transition.ColorTint;
                scrollbar_V.targetGraphic = HandleImage;
                scrollbar_V.handleRect = rectHandle;
                scrollbar_V.direction = Scrollbar.Direction.BottomToTop;
                scrollbar_V.value = 1;

                ScrollRect scrollRect = scroll.AddComponent<ScrollRect>();
                scrollRect.content = rectSlots;
                scrollRect.horizontal = false;
                scrollRect.vertical = true;
                scrollRect.scrollSensitivity = 20;
                scrollRect.viewport = rectViewport;
                scrollRect.verticalScrollbar = scrollbar_V;
            }
        }

        private void GenerateGrid()
        {
            if (itemCollection == null)
            {
                Vector3 scaleFactor = GetComponentInParent<Canvas>().transform.lossyScale;
                //grid = new ItemsCollection(gridWidth, gridHeight, cellSize, scaleFactor, slots.transform);
            }
            itemCollection.GenerateCells(gridWidth, gridHeight, cellSize, cellPrefab, slots.transform);
        }
        */

        public void OnPointerEnter(PointerEventData eventData)
        {

        }

        public void OnPointerExit(PointerEventData eventData)
        {

        }
    }
}