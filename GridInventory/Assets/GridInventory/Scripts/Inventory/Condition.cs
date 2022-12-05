using UnityEngine;
using UnityEngine.UI;

public class Condition : MonoBehaviour
{
    GridInventory itemsCollection;    
    [SerializeField] private BaseItem ghost_InventoryItem;
    Image image;
    RectTransform rectTransform;

    public GridInventory Collection { set => itemsCollection = value; }
    public BaseItem Ghost_InventoryItem { set => ghost_InventoryItem = value; }

    private void Awake()
    {
        image = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        if (itemsCollection == null || ghost_InventoryItem == null)
        {
            image.enabled = false;
            return;
        }

        var cell = itemsCollection.GetCellXY(Input.mousePosition);

        Vector2Int rotationOffset = ghost_InventoryItem.GetRotationOffset();
        var worldPos = itemsCollection.GetWorldPosition(cell.x, cell.y);
        var pos = worldPos + new Vector3(rotationOffset.x * itemsCollection.GetScaledCell().x,
            rotationOffset.y * itemsCollection.GetScaledCell().y, 0);
        transform.position = pos;
        transform.rotation = ghost_InventoryItem.ItemTransform.rotation;

        transform.SetSiblingIndex(transform.childCount - 1);
        rectTransform.sizeDelta = ghost_InventoryItem.ItemTransform.GetComponent<RectTransform>().sizeDelta;
        image.enabled = true;

        var canPlace = itemsCollection.CanAddItem(ghost_InventoryItem, cell);
        if (canPlace)
        {
            image.color = Color.green;           
        }
        else
        {
            image.color = Color.red;
        }


        /*
        if (!stareCell.Equals(new Vector2Int(-1, -1)) && ghost_InventoryItem != null)
        {
            transform.SetSiblingIndex(transform.childCount - 1);

            var newSize = new Vector2(ghost_InventoryItem.ItemData.Width * itemsCollection.CellSize.x, ghost_InventoryItem.ItemData.Height * itemsCollection.CellSize.y);
            GetComponent<RectTransform>().sizeDelta = newSize;

            var positions = BaseItem.CalculatePositionList(ghost_InventoryItem.Dir, ghost_InventoryItem.ItemData.Width, ghost_InventoryItem.ItemData.Height, stareCell);

            Vector2Int rotationOffset = ghost_InventoryItem.GetRotationOffset();
            var worldPos = itemsCollection.GetWorldPosition(positions[0].x, positions[0].y);
            var pos = worldPos + new Vector3(rotationOffset.x * itemsCollection.GetScaledCell().x,
                rotationOffset.y * itemsCollection.GetScaledCell().y, 0);
            transform.position = pos;
            transform.rotation = ghost_InventoryItem.transform.rotation;

            var placeble = itemsCollection.IsPositionsEmpty(positions);
            if (placeble)
            {
                GetComponent<Image>().color = Color.green;
            }
            else
                GetComponent<Image>().color = Color.red;

            var observedItem = itemsCollection.GetInventoryItemData(stareCell);
            if (observedItem != null && observedItem.IsContainer)
            {
                var canPutToContainer = observedItem.ItemContainer.GetComponent<GridInventory>().IsCanBePlaced(ghost_InventoryItem.ItemData, out positions, ghost_InventoryItem.Dir);
                if (canPutToContainer)
                    GetComponent<Image>().color = Color.green;
                else
                    GetComponent<Image>().color = Color.red;
            }

            if (itemsCollection.IsContainer == true && ghost_InventoryItem.ItemData.IsContainer == true)
                GetComponent<Image>().color = Color.red;



            GetComponent<Image>().enabled = true;

        }
        else
        {
            GetComponent<Image>().enabled = false;
            //GetComponent<Image>().sprite = null;
        }
        */
    }
}
