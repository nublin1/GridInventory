using GridInventorySystem;
using NaughtyAttributes;
using System.Collections.Generic;
using System.Linq;
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
    private bool wasInited = false;
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
        Init();
    }

    private void Start()
    {    
             
    }

    private void Update()
    {
        _originalPosition = containerTransform.position;

        foreach (BaseItem item in m_Collection.Items)
        {
            item.Update();
        }
    }

    private void CloseButtonAction()
    {
        transform.Find("Visual").gameObject.SetActive(false);
    }

    private void Init()
    {
        // Init Components
        if (scrollbar == null)
            if (TryGetComponent(out Scrollbar _scrollbar))
                scrollbar = _scrollbar;

        if (scrollbar == null)
            if (TryGetComponent(out ItemCollection _itemCollection))
                m_Collection = _itemCollection;

        if (closeButton != null)
            closeButton.onClick.AddListener(CloseButtonAction);

        scaleFactor = GetComponentInParent<Canvas>().transform.lossyScale;

        // init inventory
        if (inventoryCells == null)
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

        if (wasInited)
            return;

        m_Collection.Initialize();

        for (int i = 0; i < m_Collection.Items.Count; i++)
        {
            m_Collection.Items[i].Init(Dir.Up);
            var pos = CanAddItem(m_Collection.Items[i]);
            if (pos != null)
            {
                m_Collection.Items[i].ReculculatePositionList(pos[0]);
                GenerateItem(m_Collection.Items[i]);
                PlaceItemToCells(m_Collection.Items[i]);
            }
        }

        wasInited = true;
    }

    public List<BaseItem> InitItems(List<BaseItem> items)
    {
        for (int i = 0; i < items.Count; i++)
        {
            items[i].Init(Dir.Up);
            GenerateItem(items[i]);
        }

        return items;
    }

    private void GenerateItem(BaseItem baseItem)
    {
        baseItem.CreateItemTransform(CellSize);


        if (baseItem.Pf_ItemContainer != null)
        {
            var itemInventory = Instantiate(baseItem.Pf_ItemContainer, inventorySystem.transform);
            itemInventory.transform.Find("Visual").gameObject.SetActive(false);

            baseItem.ItemContainer = itemInventory.transform;
            var itemCollection = itemInventory.GetComponent<GridInventory>();
            itemCollection.isContainer = true;
            inventorySystem.AddItemContainer(itemInventory.GetComponent<GridInventory>());
        }
    }


    public void RemoveAllItems()
    {
        for (int i = m_Collection.Items.Count - 1; i >= 0; i--)
        {
            if (m_Collection.Items[i].ItemTransform != null)
            {
                RemoveItemCompletely(m_Collection.Items[i]);
            }
        }

        m_Collection.Items.Clear();
    }

    public void AddItems(List<BaseItem> baseItems)
    {
        Init();
        

        List<BaseItem> itemsToAdd = new();        
        for (int i = baseItems.Count - 1; i >= 0; i--)
        {
            if (IsContainedItem(baseItems[i]))
            {
                var item = GetInventoryItem(baseItems[i].Id);
                if (item.GridPositionList != null)
                    continue;
                else
                {
                    itemsToAdd.Add(baseItems[i]);
                    continue;
                }
            }
            
            itemsToAdd.Add(baseItems[i]);              
        }

        itemsToAdd = InitItems(itemsToAdd);

        foreach (var item in itemsToAdd)
        {
            AddItem(item, item.GridPositionList[0]);
        }
    }

    public bool AddItem(BaseItem item, Vector2Int _cellXY)
    {
        var observedItemInCell = inventoryCells[_cellXY.x, _cellXY.y].InventoryItem;
        if (observedItemInCell == null)
        {
            item.ReculculatePositionList(_cellXY);
            PlaceItemToCells(item);
            m_Collection.Add(item);
            OnAddItem?.Invoke();
            return true;
        }
        else
        {
            if (CanStack(item, observedItemInCell))
            {
                return StackItems(item, observedItemInCell);
            }

            if (observedItemInCell.IsContainer)
            {
                var invContainer = inventoryCells[_cellXY.x, _cellXY.y].InventoryItem.ItemContainer.GetComponent<GridInventory>();
                invContainer.AddItem(item, item.GridPositionList[0]);
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Checks if the item can be added to this container at any position. Free cells is required.
    /// </summary>
    /// <param name="_item">Item to check.</param>
    /// <returns>Returns true if the item can be added.</returns>
    public List<Vector2Int> CanAddItem(BaseItem _item)
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                _item.ReculculatePositionList(new Vector2Int(x, y));

                if (IsPositionsEmpty(_item.GridPositionList))
                    return _item.GridPositionList;
            }
        }
        return null;
    }

    /// <summary>
    /// Checks if the item can be added to this container at cell position.
    /// </summary>
    /// <param name="placeableItem">Item to check.</param>
    /// <param name="_cellXY">Cell to check.</param>
    /// <returns>Returns true if the item can be added.</returns>
    public bool CanAddItem(BaseItem placeableItem, Vector2Int _cellXY)
    {
        if (OutOfBoundsCheck(_cellXY))
            return false;

        if (this.isContainer == true && placeableItem.IsContainer == true)
            return false;

        placeableItem.ReculculatePositionList(_cellXY);
        if (IsPositionsEmpty(placeableItem.GridPositionList))
            return true;


        BaseItem observedItemInCell = inventoryCells[_cellXY.x, _cellXY.y].InventoryItem;
        if (observedItemInCell != null)
        {
            if (CanStack(placeableItem, observedItemInCell))
                return true;

            if (observedItemInCell.IsContainer)
            {
                var invContainer = inventoryCells[_cellXY.x, _cellXY.y].InventoryItem.ItemContainer.GetComponent<GridInventory>();
                var pos = invContainer.CanAddItem(placeableItem);
                if (pos != null)
                    return true;
            }
        }

        return false;
    }

    private bool CanStack(BaseItem item, BaseItem comparedItem)
    {
        if (item.name.Equals(comparedItem.name) && comparedItem.Stack < comparedItem.MaxStack)
            return true;

        return false;
    }

    private bool StackItems(BaseItem item, BaseItem comparedItem)
    {

        // Check if comparedItem max stack is reached or if MaxStack is invalid
        if (comparedItem.Stack >= comparedItem.MaxStack || comparedItem.MaxStack <= 0)
            return false;

        // Check if it fits in one stack
        if ((item.Stack + comparedItem.Stack) <= comparedItem.MaxStack)
        {
            comparedItem.Stack += item.Stack;
            comparedItem.UpdateDisplayItemCount();
            item.Stack = 0;
            RemoveItemCompletely(item);
            return true;
        }
        else
        {
            // Check if item's stack is already at the maximum capacity
            if (item.Stack >= item.MaxStack)
            {
                var t_buffer = comparedItem.Stack;
                item.Stack = t_buffer;
                comparedItem.Stack = item.MaxStack;
                comparedItem.UpdateDisplayItemCount();
                item.UpdateDisplayItemCount();
                m_Collection.UpdateAmounts();

                return false;
            }

            var remainder = comparedItem.MaxStack - (item.Stack + comparedItem.Stack);
            comparedItem.Stack = comparedItem.MaxStack;
            item.Stack += remainder;
            comparedItem.UpdateDisplayItemCount();
            item.UpdateDisplayItemCount();
            m_Collection.UpdateAmounts();
            return false;
        }
    }

    private void PlaceItemToCells(BaseItem _inventoryItem)
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

    public bool IsContainedItem(BaseItem _inventoryItem)
    {
        foreach(var item in m_Collection.Items)        
            if (_inventoryItem.Id.Equals(item.Id)) return true;        
        
        return false;
    }

    public BaseItem GetInventoryItem(Vector2Int _cell)
    {
        if (OutOfBoundsCheck(_cell) == true || inventoryCells[_cell.x, _cell.y].IsCellEmpty() == true)
            return null;

        var item = inventoryCells[_cell.x, _cell.y].InventoryItem;
        return item;
    }

    public BaseItem GetInventoryItem(string ID)
    {
        for(int i =0; i< m_Collection.Items.Count(); i++)
        {
            if (m_Collection.Items[i].Id.Equals(ID)) return m_Collection.Items[i];
        }

        return null;
    }

    public virtual bool RemoveItem(BaseItem item)
    {
        if (item == null || m_Collection.Contains(item) == false) { return false; }

        foreach (var cell in item.GridPositionList)
            inventoryCells[cell.x, cell.y].ClearCellData();

        //Remove item from the collection
        this.m_Collection.Remove(item);

        return true;

    }

    /// <summary>
    /// Removes the item from collection and destroy all references
    /// </summary>
    /// <param name="item"></param>
    public void RemoveItemCompletely(BaseItem item)
    {
        Destroy(item.ItemTransform.gameObject);
        RemoveItem(item);
    }

    public void TryUsingItem(Vector2Int cell)
    {
        var ObservedItem = (inventoryCells[cell.x, cell.y].InventoryItem);
        if (ObservedItem == null)
            return;

        if (ObservedItem.IsContainer)
        {
            ObservedItem.ItemContainer.transform.Find("Visual").gameObject.SetActive(true);
        }
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
                if (scrollbar.value <= 1f)
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
                if (scrollbar.value >= 0)
                    scrollbar.value -= 1 * Time.deltaTime;
            }

        }
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
}
