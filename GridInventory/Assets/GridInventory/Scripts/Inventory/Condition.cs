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
    }
}
