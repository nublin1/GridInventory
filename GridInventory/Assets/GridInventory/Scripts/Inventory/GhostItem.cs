using GridInventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GhostItem : MonoBehaviour
{    
    ItemsCollection itemsCollection;
    [SerializeField] private Vector2Int stareCell;

    [SerializeField] private InventoryItem ghost_InventoryItem;

    public ItemsCollection Collection { get => itemsCollection; set => itemsCollection = value; }   
    public InventoryItem Ghost_InventoryItem { set => ghost_InventoryItem = value; }
    public Vector2Int StareCell {set => stareCell = value; }     


    void Update()
    {
        if (!stareCell.Equals(new Vector2Int(-1, -1)) && ghost_InventoryItem != null)
        {
            GetComponent<Image>().enabled = true;

            var positions = InventoryItem.CalculatePositionList(ghost_InventoryItem.Dir, ghost_InventoryItem.ItemData.Width, ghost_InventoryItem.ItemData.Height, stareCell);
            var placeble = itemsCollection.IsCanPlace(positions);
            if (placeble)
            {
                GetComponent<Image>().sprite = ghost_InventoryItem.GetComponent<Image>().sprite;

                Vector2Int rotationOffset = ghost_InventoryItem.GetRotationOffset();
                var worldPos = itemsCollection.GetWorldPosition(positions[0].x, positions[0].y);
                var pos = worldPos + new Vector3(rotationOffset.x * itemsCollection.GetScaledCell().x,
                    rotationOffset.y * itemsCollection.GetScaledCell().y, 0);
                transform.position = pos;
                transform.rotation = ghost_InventoryItem.transform.rotation;
            }
            //Debug.Log(pos_OnGrid);            
        }
        else
        {
            GetComponent<Image>().enabled = false;
            GetComponent<Image>().sprite = null;
        }
    }
}
