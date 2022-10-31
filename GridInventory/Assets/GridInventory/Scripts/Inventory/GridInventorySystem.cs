using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    // Internal variables
    Vector2 halfCellSize;
    private GridXY_v2 grid;


    private void InventoryGeneration()
    {
        halfCellSize = new Vector2(cellSize / 2, cellSize / 2);

        if (viewportHeight > gridHeight)
            viewportHeight = gridHeight;

        Vector2 cellsAreaSize = new Vector2(cellSize * gridWidth, cellSize * gridHeight);
        Vector2 viewportSize = new Vector2(cellsAreaSize.x, cellSize * viewportHeight);

        transform.GetComponent<RectTransform>().sizeDelta = new Vector2(viewportSize.x + cells_Scroll_Spacing + scrollBar_Width, viewportSize.y);

        // Contains array of cells
        Transform cellsContainer = transform.Find("Scroll/Viewport/Slots");
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

    }
}