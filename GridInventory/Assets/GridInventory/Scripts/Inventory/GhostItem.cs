using GridInventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GhostItem : MonoBehaviour
{
    RectTransform rectSlots;
    ItemsCollection itemsCollection;

    InventoryItem ghost_InventoryItem;

    public ItemsCollection Collection { get => itemsCollection; set => itemsCollection = value; }
    public RectTransform RectSlots { set => rectSlots = value; }
    public InventoryItem Ghost_InventoryItem { get => ghost_InventoryItem; set => ghost_InventoryItem = value; }

    CanvasScaler canvasScaler;

    private void Awake()
    {
        canvasScaler = GetComponentInParent<CanvasScaler>();
    }


    void Update()
    {
        var isValidPosition = itemsCollection.IsValidPosition(Input.mousePosition, ghost_InventoryItem.ItemData.Width, ghost_InventoryItem.ItemData.Height, ghost_InventoryItem.Dir);
        if (isValidPosition)
        {
            GetComponent<Image>().enabled = true;

            Vector2Int pos_OnGrid = itemsCollection.GetCellXY(Input.mousePosition);
            //Debug.Log(pos_OnGrid);

            Vector2Int rotationOffset = ghost_InventoryItem.GetRotationOffset();
            var worldPos = itemsCollection.GetWorldPosition(pos_OnGrid.x, pos_OnGrid.y);
            var pos = worldPos + new Vector3(rotationOffset.x * itemsCollection.GetScaledCell().x,
                rotationOffset.y * itemsCollection.GetScaledCell().y, 0);
            transform.position = pos;
            transform.rotation = ghost_InventoryItem.transform.rotation;
        }
        else
        {
            GetComponent<Image>().enabled = false;
        }
    }
}
