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

    [SerializeField] private GridInventorySystem inventorySystem;
    [SerializeField] private Scrollbar scrollbar;
    [SerializeField] private Transform containerTransform;
    [SerializeField] List<InventoryItemData> items = new List<InventoryItemData>();

    [SerializeField] private Button closeButton;

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
    public GridInventorySystem InventorySystem { get => inventorySystem; set => inventorySystem = value; }
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
        {
            if (TryGetComponent(out Scrollbar _scrollbar))
                scrollbar = _scrollbar;
        }
    }

    private void Start()
    {
        scaleFactor = GetComponentInParent<Canvas>().transform.lossyScale;

        UpdateGridImageSize();

        Init();

        if (closeButton != null)
            closeButton.onClick.AddListener(CloseButtonAction);
    }

    private void Update()
    {

        _originalPosition = containerTransform.position + transform.Find("Visual").localPosition;
    }

    private void CloseButtonAction()
    {
        transform.Find("Visual").gameObject.SetActive(false);
    }

    [Button]
    private void UpdateGridImageSize()
    {
        var gridImage = containerTransform.GetComponentInChildren<RawImage>();
        var gridImageRect = containerTransform.GetChild(0).GetComponent<RectTransform>();
        if (gridImage != null)
        {
            gridImageRect.sizeDelta = new Vector2(gridWidth * GetScaledCell().x, gridHeight * GetScaledCell().y);
            gridImage.uvRect = new Rect(0, 0, gridWidth, gridHeight);
        }
    }

    private void Init()
    {
        inventoryCells = new GridCell[gridWidth, gridHeight];

        for (int i = 0; i < gridWidth; i++)
        {
            for (int j = 0; j < gridHeight; j++)
            {
                inventoryCells[i, j] = new GridCell(this, i, j);
            }
        }

        foreach (var data in items)
        {
            List<Vector2Int> positions;
            if(IsCanBePlaced(data, out positions))
            {
                GenerateInventoryItem(positions, data);
            }
        }
    }

    public bool IsCanBePlaced(InventoryItemData _itemData, out List<Vector2Int> _positions, Dir dir = Dir.Up)
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                var positions = InventoryItem.CalculatePositionList(dir, _itemData.Width, _itemData.Height, new Vector2Int(x, y));
                if (IsPositionsEmpty(positions))
                {
                    _positions = positions;    
                    return true;
                }
            }
        }

        _positions = null;
        return false;
    }

    private void GenerateInventoryItem(List<Vector2Int> positions, InventoryItemData _itemData, Dir dir = Dir.Up)
    {
        var item = InventoryItem.CreateItem(_itemData, dir, GetScaledCell());
        item.GridPositionList = positions;
        PlaceItemToCells(item);

        if (item.ItemData.Pf_ItemContainer != null)
        {
            var itemInventory = Instantiate(item.ItemData.Pf_ItemContainer, inventorySystem.transform);
            itemInventory.transform.Find("Visual").gameObject.SetActive(false);
            

            item.ItemData.ItemContainer = itemInventory.transform;
            inventorySystem.AddItemContainer(itemInventory.GetComponent<ItemsCollection>());
        }

    }

    public bool TryPlaceItem(Vector2Int cellPosXY, InventoryItem item)
    {
        if (transform.Equals(item.ItemData.ItemContainer))
            return false;

        if (inventoryCells[cellPosXY.x, cellPosXY.y].InventoryItem != null && inventoryCells[cellPosXY.x, cellPosXY.y].InventoryItem.ItemData.IsContainer)
        {
            var invContainer = inventoryCells[cellPosXY.x, cellPosXY.y].InventoryItem.ItemData.ItemContainer.GetComponent<ItemsCollection>();
            List<Vector2Int> posit;
            if(invContainer.IsCanBePlaced(item.ItemData, out posit, item.Dir))
            {
                item.GridPositionList = posit;
                invContainer.PlaceItemToCells(item);

                OnAddItem?.Invoke();
                return true;
            }
        }

        var positions = InventoryItem.CalculatePositionList(item.Dir, item.ItemData.Width, item.ItemData.Height, cellPosXY);
        if (IsPositionsEmpty(positions))
        {
            item.GridPositionList = positions;
            PlaceItemToCells(item);
            OnAddItem?.Invoke();
            return true;
        }

        OnFailedAddItem?.Invoke();
        return false;
    }

    public void PlaceItemToCells(InventoryItem _inventoryItem)
    {
        Vector2Int rotationOffset = _inventoryItem.GetRotationOffset();
        var localPos = GetLocalPosition(_inventoryItem.GridPositionList[0].x, _inventoryItem.GridPositionList[0].y);
        var position = localPos + new Vector3(rotationOffset.x * GetScaledCell().x, rotationOffset.y * GetScaledCell().y, 0);

        _inventoryItem.GetComponent<RectTransform>().anchoredPosition = position;
        _inventoryItem.transform.SetParent(containerTransform.transform, false);

        foreach (var pos in _inventoryItem.GridPositionList)
        {
            inventoryCells[pos.x, pos.y].SetCellData(_inventoryItem);
        }
    }

    public bool IsPositionsEmpty(List<Vector2Int> gridPositionList)
    {
        foreach (var position in gridPositionList)
        {
            if (OutOfBoundsCheck(position) == true)
                return false;

            if (inventoryCells[position.x, position.y].IsCellEmpty() == false)
                return false;

        }
        return true;
    }

    public InventoryItem TryGetInventoryItem(Vector2Int activeCell)
    {
        if (OutOfBoundsCheck(activeCell) == false && inventoryCells[activeCell.x, activeCell.y].IsCellEmpty() == true)
            return null;

        var item = inventoryCells[activeCell.x, activeCell.y].InventoryItem;
        foreach (var cell in item.GridPositionList)
        {
            inventoryCells[cell.x, cell.y].ClearCellData();
        }

        return item;
    }

    public InventoryItemData GetInventoryItemData(Vector2Int activeCell)
    {
        if (OutOfBoundsCheck(activeCell) == false && inventoryCells[activeCell.x, activeCell.y].IsCellEmpty() == true)
            return null;

        return inventoryCells[activeCell.x, activeCell.y].InventoryItem.ItemData;
    }

    public Vector3 GetWorldPosition(int x, int y)
    {
        var scaledCell = GetScaledCell();
        var pos = new Vector3(scaledCell.x * x + _originalPosition.x, scaledCell.y * y * -1 + _originalPosition.y, 0);
        return pos;
    }

    public Vector3 GetLocalPosition(int x, int y)
    {
        var pos = new Vector3(GetScaledCell().x * x, GetScaledCell().y * y * -1, 0);
        return pos;
    }

    public Vector2Int GetCellXY(Vector2 mousePosition)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(containerTransform.GetComponent<RectTransform>(), mousePosition, null, out Vector2 localPosition);

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

    public Vector2 GetScaledCell()
    {
        return cellSize * scaleFactor;
    }

    public bool IsIntersectWithTheItem(Bounds itemBounds)
    {
        var boundCollection = GetComponentInChildren<BoxCollider2D>().bounds;
        if (boundCollection.Intersects(itemBounds))
            return true;

        return false;
    }

    public void Scroll(Bounds itemBounds)
    {
        var boundCollection = GetComponentInChildren<BoxCollider2D>().bounds;
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

            if (dist < itemBounds.size.y / 2)
            {
                scrollbar.value -= 1 * Time.deltaTime;
            }

        }
    }

    public void TryUsingItem(Vector2Int cell)
    {
        var ObservedItem = (inventoryCells[cell.x, cell.y].InventoryItem);
        if (ObservedItem == null)
            return;

        if (ObservedItem.ItemData.IsContainer)
        {
            ObservedItem.ItemData.ItemContainer.transform.Find("Visual").gameObject.SetActive(true);
        }
    }
}
