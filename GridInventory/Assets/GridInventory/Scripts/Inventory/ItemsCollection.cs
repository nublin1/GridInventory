using GridInventory;
using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemsCollection : MonoBehaviour
{
    [SerializeField] private int gridWidth;
    [SerializeField] private int gridHeight;
    [SerializeField] private Vector2 cellSize = new Vector2(25, 25);

    [SerializeField] private Scrollbar scrollbar;
    [SerializeField] private Transform containerTransform;
    [SerializeField] List<InventoryItemData> items = new List<InventoryItemData>();

    //
    private Vector3 _originalPosition = Vector3.zero;
    private Vector3 scaleFactor;
    private GridCell[,] inventoryCells;

    #region getters
    public Vector3 OriginalPosition { get => _originalPosition; }
    public int GridWidth { get => gridWidth; set => gridWidth = value; }
    public int GridHeight { get => gridHeight; set => gridHeight = value; }
    public Vector2 CellSize { get => cellSize; set => cellSize = value; }
    public Vector3 ScaleFactor { get => scaleFactor; }
    public Scrollbar Scrollbar { get => scrollbar; }
    public Transform ContainerTransform { get => containerTransform; set => containerTransform = value; }
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
        if (scrollbar == null)
            scrollbar = GetComponentInChildren<Scrollbar>();
    }

    private void Start()
    {   
        scaleFactor = GetComponentInParent<Canvas>().transform.lossyScale;
        Init();
        InitStartItems();
    }

    private void Update()
    {
        _originalPosition = containerTransform.position + new Vector3(0f, 0f, 0);       
    }

    [Button]
    private void UpdateGridImageSize()
    {
        var gridImage = containerTransform.GetComponentInChildren<RawImage>();
        var gridImageRect = containerTransform.GetChild(0).GetComponent<RectTransform>();
        if (gridImage != null)
        {
            gridImageRect.sizeDelta = new Vector2(gridWidth * cellSize.x, gridHeight * cellSize.y);
            gridImage.uvRect = new Rect(0, 0, gridWidth, gridHeight);
        }
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
                        isPlaced = GenerateInventoryItem(new Vector2Int(x, y), item);
                }
            }
        }
    }

    public bool GenerateInventoryItem(Vector2Int pivotPosition, InventoryItemData itemData, Dir dir = Dir.Up)
    {
        var positions = InventoryItem.CalculatePositionList(dir, itemData.Width, itemData.Height, pivotPosition);
        bool isCanPlace = IsCanPlace(positions);
        if (isCanPlace)
        {
            Vector2Int rotationOffset = InventoryItem.GetRotationOffset(dir, itemData.Width, itemData.Height);
            var localPos = GetLocalPosition(positions[0].x, positions[0].y);
            var position = localPos + new Vector3(rotationOffset.x * GetScaledCell().x, rotationOffset.y * GetScaledCell().y, 0);

            var item = InventoryItem.CreateItem(containerTransform, itemData, dir, positions, position, GetScaledCell());
            PutItemToCells(item);

            OnAddItem?.Invoke();
            return true;
        }
        OnFailedAddItem?.Invoke();
        return false;
    }

    private void AddInventoryItem(InventoryItem _inventoryItem)
    {
        Vector2Int rotationOffset = _inventoryItem.GetRotationOffset();
        var worldPos = GetWorldPosition(_inventoryItem.GridPositionList[0].x, _inventoryItem.GridPositionList[0].y);
        var position = worldPos + new Vector3(rotationOffset.x * GetScaledCell().x, rotationOffset.y * GetScaledCell().y, 0);

        _inventoryItem.transform.position = position;
        _inventoryItem.transform.SetParent(containerTransform.transform, true);

        PutItemToCells(_inventoryItem);
        OnAddItem?.Invoke();
    }

    private void PutItemToCells(InventoryItem _inventoryItem)
    {
        foreach (var position in _inventoryItem.GridPositionList)
        {
            inventoryCells[position.x, position.y].SetCellData(_inventoryItem);
            //Debug.Log(position.x + "  " + position.y);
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

    public InventoryItem TryGetInventoryItem(Vector2Int activeCell)
    {
        if (inventoryCells[activeCell.x, activeCell.y].IsCellEmpty() == true)
            return null;

        var item = inventoryCells[activeCell.x, activeCell.y].InventoryItem;
        foreach (var cell in item.GridPositionList)
        {
            inventoryCells[cell.x, cell.y].ClearCellData();
        }

        return item;
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
        RectTransformUtility.ScreenPointToLocalPointInRectangle(containerTransform.GetComponent<RectTransform>(), mousePosition, null, out localPosition);

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

    public bool TryPlaceItem(Vector2Int cellPosXY, InventoryItem item)
    {
        var positions = InventoryItem.CalculatePositionList(item.Dir, item.ItemData.Width, item.ItemData.Height, cellPosXY);

        if (IsCanPlace(positions))
        {
            item.GridPositionList = positions;
            AddInventoryItem(item);
            return true;
        }

        OnFailedAddItem?.Invoke();
        return false;
    }

    public Vector2 GetScaledCell()
    {
        return cellSize * scaleFactor;
    }

    public bool IsIntersectWithTheItem(Bounds itemBounds)
    {
        var boundCollection = GetComponent<BoxCollider2D>().bounds;
        if (boundCollection.Intersects(itemBounds))
            return true;

        return false;
    }

    public void Scroll (Bounds itemBounds)
    {
        var boundCollection = GetComponent<BoxCollider2D>().bounds;
        var distanceNormal = (Input.mousePosition - boundCollection.center).normalized;

        if (distanceNormal.y > 0)
        {
            var centerCorner = boundCollection.center;
            centerCorner.y = centerCorner.y + boundCollection.size.y / 2;

            var dist = centerCorner.y - itemBounds.center.y;

            if (dist < itemBounds.size.y)
            {
                scrollbar.value += 1 * Time.deltaTime;
            }

        }
        else if (distanceNormal.y < 0)
        {
            var centerCorner = boundCollection.center;
            centerCorner.y = centerCorner.y - boundCollection.size.y / 2;

            var dist = itemBounds.center.y - centerCorner.y;

            if (dist < itemBounds.size.y)
            {
                scrollbar.value -= 1 * Time.deltaTime;
            }

        }
    }

    
}
