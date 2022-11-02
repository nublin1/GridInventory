using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;
using System;
using TMPro;
using Inventory;

public class GridInventorySystem : MonoBehaviour
{
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
    private GridXY_v2 grid;

    GameObject slots;
    RectTransform rectSlots;

    private void Awake()
    {
        slots = transform.Find("Scroll/Viewport/Slots").gameObject;
    }

    private void Start()
    {
        grid = new GridXY_v2();
    }

    private void Update()
    {
        Vector2 rrr = slots.transform.GetChild(0).GetComponent<RectTransform>().rect.center;
        grid.OriginalPosition = slots.transform.GetChild(0).GetComponent<RectTransform>().TransformPoint(rrr);       

        Vector2 posit = Vector2.zero;
        posit = InventoryUtilities.CalculateInventorySlotCoordinate(Input.mousePosition, grid);
        //Debug.Log(posit);
    }

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
            gridLayoutGroup.childAlignment = TextAnchor.UpperCenter;
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
            ViewportImage.color = new Color(.105f, .121f, .133f , 1);
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
        if (grid == null)
        {
            grid = new GridXY_v2();
        }
        grid.GenerateCells(gridWidth, gridHeight, cellSize, cellPrefab, slots.transform);
    }
}