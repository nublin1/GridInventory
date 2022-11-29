using UnityEngine;
using UnityEngine.UI;

public class GhostItem : MonoBehaviour
{
    GridInventory itemsCollection;
    [SerializeField] private Vector2Int stareCell;
    [SerializeField] private InventoryItem ghost_InventoryItem;

    public GridInventory Collection { get => itemsCollection; set => itemsCollection = value; }
    public InventoryItem Ghost_InventoryItem { set => ghost_InventoryItem = value; }
    public Vector2Int StareCell { set => stareCell = value; }


    void Update()
    {
        if (!stareCell.Equals(new Vector2Int(-1, -1)) && ghost_InventoryItem != null)
        {
            transform.SetSiblingIndex(transform.childCount - 1);

            var newSize = new Vector2(ghost_InventoryItem.ItemData.Width * itemsCollection.CellSize.x, ghost_InventoryItem.ItemData.Height * itemsCollection.CellSize.y);
            GetComponent<RectTransform>().sizeDelta = newSize;

            var positions = InventoryItem.CalculatePositionList(ghost_InventoryItem.Dir, ghost_InventoryItem.ItemData.Width, ghost_InventoryItem.ItemData.Height, stareCell);

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
    }
}
