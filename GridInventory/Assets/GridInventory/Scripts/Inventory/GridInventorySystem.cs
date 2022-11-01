using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;
using System;

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

    Transform cellsContainer;

    private void Awake()
    {
        cellsContainer = transform.Find("Scroll/Viewport/Slots");
    }

    private void Start()
    {
        grid = new GridXY_v2();
    }

    [Button("Generate inventory")]
    private void InventoryGeneration()
    {
        ClearExitedCells();

        halfCellSize = new Vector2(cellSize / 2, cellSize / 2);

        if (viewportHeight > gridHeight)
            viewportHeight = gridHeight;

        Vector2 cellsAreaSize = new Vector2(cellSize * gridWidth, cellSize * gridHeight);
        Vector2 viewportSize = new Vector2(cellsAreaSize.x, cellSize * viewportHeight);

        transform.GetComponent<RectTransform>().sizeDelta = new Vector2(viewportSize.x + cells_Scroll_Spacing + scrollBar_Width, viewportSize.y);

        // Contains array of cells        
        cellsContainer.localScale = new Vector3(1f, 1f, 1f);
        cellsContainer.GetComponent<GridLayoutGroup>().cellSize = new Vector2(cellSize, cellSize);
        cellsContainer.GetComponent<GridLayoutGroup>().spacing = Vector2.zero;
        cellsContainer.GetComponent<GridLayoutGroup>().startCorner = GridLayoutGroup.Corner.UpperLeft;
        cellsContainer.GetComponent<GridLayoutGroup>().startAxis = GridLayoutGroup.Axis.Horizontal;
        cellsContainer.GetComponent<GridLayoutGroup>().childAlignment = TextAnchor.MiddleCenter;

        Transform header = transform.Find("Scroll/Scrollbar Vertical");
        header.GetComponent<RectTransform>().sizeDelta = new Vector2(0, header_Heigth);

        Transform scrollBar = transform.Find("Scroll/Scrollbar Vertical");
        scrollBar.GetComponent<RectTransform>().sizeDelta = new Vector2(scrollBar_Width, 0);

        GenerateGrid();

    }

    private void ClearExitedCells()
    {
        if(cellsContainer == null)
            cellsContainer = transform.Find("Scroll/Viewport/Slots");

        if (cellsContainer.childCount > 0)
        {
            for (int i = 0; i < cellsContainer.childCount; i++)
            {
                DestroyImmediate(cellsContainer.GetChild(i).gameObject);
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
        }

        if (transform.Find("Background Outline") == null)
        {
            GameObject backgroundOutline = new GameObject("Background Outline");
            backgroundOutline.transform.parent = transform;

            RectTransform rectBackgroundOutline = backgroundOutline.GetComponent<RectTransform>();
            rectBackgroundOutline.anchorMin = new Vector2(0, 0);
            rectBackgroundOutline.anchorMax = new Vector2(1, 1);
            rectBackgroundOutline.pivot = new Vector2(0.5f, .5f);
        }
    }

    private void GenerateGrid()
    {
        if (grid == null)
        {
            grid = new GridXY_v2();
        }
        grid.GenerateCells(gridWidth, gridHeight, cellSize, transform.position, cellPrefab, cellsContainer);
    }
}