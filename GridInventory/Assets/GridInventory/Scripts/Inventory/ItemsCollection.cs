
using GridInventory;
using System;
using System.Collections.Generic;
using UnityEngine;


public class ItemsCollection : MonoBehaviour
{
    [SerializeField] private int gridWidth;
    [SerializeField] private int gridHeight;
    [SerializeField] private Vector2 cellSize = new Vector2(25, 25);
    [SerializeField] private Transform parentForItems;
    [SerializeField] List<InventoryItemData> items = new List<InventoryItemData>();

    [SerializeField] private Vector3 _originalPosition = Vector3.zero;
    private Vector3 scaleFactor;
    private GridCell[,] inventoryCells;

    #region getters
    public Vector3 OriginalPosition { get => _originalPosition; }
    public int GridWidth { get => gridWidth; set => gridWidth = value; }
    public int GridHeight { get => gridHeight; set => gridHeight = value; }
    public Vector2 CellSize { get => cellSize; }
    public Vector3 ScaleFactor { get => scaleFactor; }
    #endregion

    #region Events
    public delegate void AAddItem();
    public static event AAddItem OnAddItem;
    public delegate void AFailedAddItem();
    public static event AFailedAddItem OnFailedAddItem;
    public delegate void ARemoveItem();
    public static event ARemoveItem OnRemoveItem;
    #endregion

    private void Awake()
    {
        scaleFactor = GetComponentInParent<Canvas>().transform.lossyScale;
        Init();
    }

    private void Start()
    {
        InitStartItems();
    }

    private void Update()
    {
        _originalPosition = parentForItems.position + new Vector3(0f, 0f, 0);
    }

    public void Init()
    {
        inventoryCells = new GridCell[gridWidth, gridHeight];

        for (int i = 0; i < gridWidth; i++)
        {
            for (int j = 0; j < gridHeight; j++)
            {
                inventoryCells[i, j] = new GridCell(this, i, j);
            }
        }
    }

    private void InitStartItems()
    {
        foreach (var item in items)
        {
            bool isPlaced = false;
            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    if (isPlaced == false)
                        isPlaced = AddNewInventoryItem(new Vector2Int(x, y), item);
                }
            }
        }
    }

    public bool AddNewInventoryItem(Vector2Int pivotPosition, InventoryItemData itemData, Dir dir = Dir.Up)
    {
        var positions = InventoryItem.CalculatePositionList(dir, itemData.Width, itemData.Height, pivotPosition);
        bool isCanPlace = IsCanPlace(positions);
        if (isCanPlace)
        {
            Vector2Int rotationOffset = InventoryItem.GetRotationOffset(dir, itemData.Width, itemData.Height);
            var localPos = GetLocalPosition(positions[0].x, positions[0].y);
            var position = localPos + new Vector3(rotationOffset.x * GetScaledCell().x, rotationOffset.y * GetScaledCell().y, 0);

            var item = InventoryItem.CreateItem(parentForItems, itemData, dir, positions, position, CellSize);
            PutItemToCells(item);

            OnAddItem?.Invoke();
            return true;
        }
        OnFailedAddItem?.Invoke();
        return false;
    }

    public void AddInventoryItem(InventoryItem _inventoryItem)
    {
        Vector2Int rotationOffset = _inventoryItem.GetRotationOffset();
        var worldPos = GetWorldPosition(_inventoryItem.GridPostionList[0].x, _inventoryItem.GridPostionList[0].y);
        var position = worldPos + new Vector3(rotationOffset.x * GetScaledCell().x, rotationOffset.y * GetScaledCell().y, 0);

        _inventoryItem.transform.position = position;
        _inventoryItem.transform.SetParent(parentForItems.transform, true);

        PutItemToCells(_inventoryItem);
        OnAddItem?.Invoke();
    }

    private void PutItemToCells(InventoryItem _inventoryItem)
    {
        foreach (var position in _inventoryItem.GridPostionList)
        {
            inventoryCells[position.x, position.y].SetCellData(_inventoryItem);
            Debug.Log(position.x + "  " + position.y);
        }
    }

    public bool IsCanPlace(List<Vector2Int> gridPostionList)
    {
        foreach (var position in gridPostionList)
        {
            if (OutOfBoundsCheck(position) == true || !inventoryCells[position.x, position.y].IsCellEmpty())
                return false;
        }
        return true;
    }

    public InventoryItem GetInventoryItem(Vector2 mousePosition)
    {
        var position = GetCellXY(mousePosition);

        if (OutOfBoundsCheck(position) == false && inventoryCells[position.x, position.y].IsCellEmpty() == false)
        {
            var item = inventoryCells[position.x, position.y].InventoryItem;
            foreach (var cell in item.GridPostionList)
            {
                inventoryCells[cell.x, cell.y].ClearCellData();
            }
            return item;
        }
        else
            return null;
    }

    public void GenerateCells(int width, int height, float cellSize,
           GameObject cellPrefab, Transform cellsContainer)
    {
        gridWidth = width;
        gridHeight = height;
        //this.cellSize = cellSize;

        // generate cells
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                GameObject cellVisual = GameObject.Instantiate(cellPrefab);
                cellVisual.transform.name = "Cell";
                cellVisual.transform.SetParent(cellsContainer.transform, true);
                cellVisual.transform.localPosition = new Vector3(0f, 0f, 0f);
                cellVisual.transform.localRotation = Quaternion.identity;
                cellVisual.transform.localScale = new Vector3(1f, 1f, 1f);

                Vector2 sizeOfCell = new Vector2(cellSize, cellSize);
                cellVisual.GetComponent<RectTransform>().sizeDelta = sizeOfCell;
                //cellVisual.transform.Find("SlotBackground").GetComponent<RectTransform>().sizeDelta = sizeOfCell;
                //cellVisual.transform.Find("SlotOutline").GetComponent<RectTransform>().sizeDelta = sizeOfCell;
            }
        }

        Vector2 rrr = cellsContainer.GetChild(0).GetComponent<RectTransform>().rect.center;
        _originalPosition = cellsContainer.GetChild(0).GetComponent<RectTransform>().TransformPoint(rrr);
    }

    public Vector3 GetWorldPosition(int x, int y)
    {
        var scaledCell = GetScaledCell();
        var pos = new Vector3(scaledCell.x * x + _originalPosition.x, scaledCell.y * y * -1 + _originalPosition.y, 0);
        return pos;
    }

    public Vector3 GetLocalPosition(int x, int y)
    {
        var scaledCell = GetScaledCell();
        var pos = new Vector3(cellSize.x * x, cellSize.y * y * -1, 0);
        return pos;
    }

    public void GetCellXY(RectTransform rectTransform, Vector2 mousePosition, out int x, out int y)
    {
        Vector2 localPosition;
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        localPosition = worldPosition - OriginalPosition;
        //RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, mousePosition, null, out localPosition);

        x = 0; y = 0;
        //x = Mathf.FloorToInt(localPosition.x / GetScaledCell().x);
        //y = Mathf.FloorToInt(localPosition.y / GetScaledCell().y * -1);
    }

    public Vector2Int GetCellXY(Vector2 mousePosition)
    {
        Vector2 localPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parentForItems.GetComponent<RectTransform>(), mousePosition, null, out localPosition);

        var x = Mathf.FloorToInt(localPosition.x / cellSize.x);
        var y = Mathf.FloorToInt(localPosition.y / cellSize.y * -1);

        return new Vector2Int(x, y);
    }

    // Return true if cell is OutOfBounds
    public bool OutOfBoundsCheck(Vector2Int cellPosXY)
    {
        if (cellPosXY.x >= gridWidth || cellPosXY.y >= gridHeight || cellPosXY.x < 0 || cellPosXY.y < 0)
            return true;

        return false;
    }

    public bool IsValidPosition(Vector2 mousePoistion, int width, int height, Dir dir = Dir.Up)
    {
        var pivot = GetCellXY(mousePoistion);
        var positions = InventoryItem.CalculatePositionList(dir, width, height, pivot);

        return IsCanPlace(positions);
    }

    public Vector2 GetScaledCell()
    {
        return cellSize * scaleFactor;
    }
}
