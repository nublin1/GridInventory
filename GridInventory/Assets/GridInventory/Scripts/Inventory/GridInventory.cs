using GridInventorySystem;
using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(ItemCollection))]
public class GridInventory : MonoBehaviour
{
    [SerializeField] private int gridWidth;
    [SerializeField] private int gridHeight;
    [SerializeField] private Vector2 cellSize = new Vector2(25, 25);

    [SerializeField] private GridInventoryManager inventorySystem;
    [SerializeField] private Scrollbar scrollbar;
    [SerializeField] private Transform containerTransform;

    [SerializeField] private Button closeButton;

    //
    private bool isContainer = false;
    private Vector3 _originalPosition = Vector3.zero;
    private Vector3 scaleFactor;
    private GridCell[,] inventoryCells;

    private ItemCollection m_Collection;   

    #region Properties
    public bool IsContainer { get => isContainer; set => isContainer = value; }
    public Vector3 OriginalPosition { get => _originalPosition; }
    public int GridWidth { get => gridWidth; set => gridWidth = value; }
    public int GridHeight { get => gridHeight; set => gridHeight = value; }
    public Vector2 CellSize { get => cellSize; set => cellSize = value; }
    public Vector3 ScaleFactor { get => scaleFactor; }
    public Scrollbar Scrollbar { get => scrollbar; }
    public Transform ContainerTransform { get => containerTransform; set => containerTransform = value; }
    public GridInventoryManager InventorySystem { get => inventorySystem; set => inventorySystem = value; }

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

        if (TryGetComponent(out ItemCollection _itemCollection))
            m_Collection = _itemCollection;
    }

    private void Start()
    {
        scaleFactor = GetComponentInParent<Canvas>().transform.lossyScale;

        Init();

        if (closeButton != null)
            closeButton.onClick.AddListener(CloseButtonAction);
    }

    private void Update()
    {

        _originalPosition = containerTransform.position;
    }

    private void CloseButtonAction()
    {
        transform.Find("Visual").gameObject.SetActive(false);
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

        foreach (var itemData in m_Collection.Items)
        {
            var item = BaseItem.CreateItem(Dir.Up, itemData);
            if (IsCanBePlacedInInventory(item))
            {
                GenerateItem(item);
            }
        }
    }

    public bool IsCanBePlacedInInventory(BaseItem _item)
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                if (IsPositionsEmpty(_item.GridPositionList))
                    return true;

                else
                    _item.ReculculatePositionList(new Vector2Int(x, y));
            }
        }
        return false;
    }

    private void GenerateItem(BaseItem baseItem)
    {
        baseItem.CreateItemTransform(CellSize);
        PlaceItemToCells(baseItem);

        if (baseItem.ItemData.Pf_ItemContainer != null)
        {
            var itemInventory = Instantiate(baseItem.ItemData.Pf_ItemContainer, inventorySystem.transform);
            itemInventory.transform.Find("Visual").gameObject.SetActive(false);

            baseItem.ItemData.ItemContainer = itemInventory.transform;
            var itemCollection = itemInventory.GetComponent<GridInventory>();
            itemCollection.isContainer = true;
            inventorySystem.AddItemContainer(itemInventory.GetComponent<GridInventory>());
        }
    }


    public bool PlaceItem(BaseItem placeableItem)
    {
        var cellXY = GetCellXY(Input.mousePosition);
        if (OutOfBoundsCheck(cellXY))
            return false;

        if (this.isContainer == true && placeableItem.ItemData.IsContainer == true)
            return false;

        placeableItem.ReculculatePositionList(cellXY);
        if (IsPositionsEmpty(placeableItem.GridPositionList))
        {            
            PlaceItemToCells(placeableItem);
            m_Collection.Add(placeableItem.ItemData);
            OnAddItem?.Invoke();
            return true;
        }

        BaseItem observedItemInCell = inventoryCells[cellXY.x, cellXY.y].InventoryItem; 
        if (observedItemInCell != null)
        {
            var stack = CanStack(placeableItem, observedItemInCell);
            if (stack)
            {
                PlaceItemToCells(placeableItem);
            }

            if (observedItemInCell.ItemData.IsContainer)
            {
                var invContainer = inventoryCells[cellXY.x, cellXY.y].InventoryItem.ItemData.ItemContainer.GetComponent<GridInventory>();                
                if (invContainer.IsCanBePlacedInInventory(placeableItem))
                {                    
                    invContainer.PlaceItemToCells(placeableItem);                    
                    OnAddItem?.Invoke();
                    return true;
                }
            }
        }

        OnFailedAddItem?.Invoke();
        return false;
    }

    private bool CanStack(BaseItem item, BaseItem comparedItem)
    {
        //Check if max stack is reached
        if ((item.ItemData.Stack + comparedItem.ItemData.Stack) <= comparedItem.ItemData.Stack)
        {
            return true;
        }

        return false;
    }

    public void PlaceItemToCells(BaseItem _inventoryItem)
    {
        Vector2Int rotationOffset = _inventoryItem.GetRotationOffset();
        var localPos = GetLocalPosition(_inventoryItem.GridPositionList[0].x, _inventoryItem.GridPositionList[0].y);
        var position = localPos + new Vector3(rotationOffset.x * cellSize.x, rotationOffset.y * cellSize.y, 0);

        _inventoryItem.ItemTransform.GetComponent<RectTransform>().anchoredPosition = position;
        _inventoryItem.ItemTransform.transform.SetParent(containerTransform.transform, false);

        foreach (var pos in _inventoryItem.GridPositionList)
        {
            inventoryCells[pos.x, pos.y].SetCellData(_inventoryItem);
        }
    }

    private bool IsPositionsEmpty(List<Vector2Int> gridPositionList)
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

    public BaseItem GetInventoryItem()
    {
        var cellXY = GetCellXY(Input.mousePosition);

        if (OutOfBoundsCheck(cellXY) == false && inventoryCells[cellXY.x, cellXY.y].IsCellEmpty() == true)
            return null;

        var item = inventoryCells[cellXY.x, cellXY.y].InventoryItem;
        foreach (var cell in item.GridPositionList)        
            inventoryCells[cell.x, cell.y].ClearCellData();

        m_Collection.Remove(item.ItemData);
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
        var pos = new Vector3(cellSize.x * x, cellSize.y * y * -1, 0);
        return pos;
    }

    private Vector2Int GetCellXY(Vector2 mousePosition)
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
