using GridInventory;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventoryItem : MonoBehaviour
{
    private InventoryItemData itemData;
    private Dir dir = Dir.Up;
    private List<Vector2Int> gridPositionList;  

    #region getters
    public InventoryItemData ItemData { get => itemData; }
    public Dir Dir { get => dir; set => dir = value; }
    public List<Vector2Int> GridPositionList { get => gridPositionList; set => gridPositionList = value; }
    #endregion

    private void Start()
    {
       
    }

    private void LateUpdate()
    {
        var newRot = Quaternion.Euler(0, 0, InventoryUtilities.GetRotationAngle(dir));
        transform.rotation = Quaternion.Lerp(transform.rotation, newRot, Time.deltaTime * 15.0f);

    }
    public static InventoryItem CreateItem(Transform parentForItems, InventoryItemData _itemData, Dir dir, List<Vector2Int> gridPositions, Vector3 worldPosition, Vector2 _cellSize)
    {
        GameObject gameObject = new GameObject(_itemData.ItemName);
        gameObject.transform.parent = parentForItems;
        var rectCell = gameObject.AddComponent<RectTransform>();
        rectCell.sizeDelta = new Vector2(_cellSize.x * _itemData.Width, _cellSize.y * _itemData.Height);
        rectCell.anchorMin = new Vector2(0f, 1f);
        rectCell.anchorMax = new Vector2(0f, 1f);
        rectCell.pivot = new Vector2(0f, 1f);

        gameObject.transform.localPosition = worldPosition;
        gameObject.transform.rotation = Quaternion.Euler(0, 0, InventoryUtilities.GetRotationAngle(dir));


        var image = gameObject.AddComponent<Image>();
        image.sprite = _itemData.Icon;
        image.raycastTarget = false;

        var collider2d = gameObject.AddComponent<BoxCollider2D>();
        collider2d.offset = new Vector2(_cellSize.x * _itemData.Width / 2, -_cellSize.y * _itemData.Height / 2);
        collider2d.size = new Vector2 (_cellSize.x * _itemData.Width, _cellSize.y * _itemData.Height);
        collider2d.isTrigger = true;

        InventoryItem item = gameObject.AddComponent<InventoryItem>();
        item.itemData = _itemData;
        item.dir = dir;
        item.gridPositionList = gridPositions;


        return item;
    }

    public static List<Vector2Int> CalculatePositionList(Dir dir, int width, int height, Vector2Int pivotPosition)
    {
        List<Vector2Int> tempGridPostionList = new List<Vector2Int>();

        switch (dir)
        {
            case Dir.Down:
            case Dir.Up:
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        tempGridPostionList.Add(pivotPosition + new Vector2Int(x, y));
                    }
                }
                break;
            case Dir.Left:
            case Dir.Right:
                for (int x = 0; x < height; x++)
                {
                    for (int y = 0; y < width; y++)
                    {
                        tempGridPostionList.Add(pivotPosition + new Vector2Int(x, y));
                    }
                }
                break;
        }

        return tempGridPostionList;
    }

    public Vector2Int GetRotationOffset()
    {
        return dir switch
        {
            Dir.Left => new Vector2Int(0, itemData.Width),
            Dir.Down => new Vector2Int(itemData.Width, itemData.Height),
            Dir.Right => new Vector2Int(itemData.Height, 0),
            _ => new Vector2Int(0, 0),
        };
    }

    public static Vector2Int GetRotationOffset(Dir dir, int width, int height)
    {
        switch (dir)
        {
            default:
            case Dir.Up: return new Vector2Int(0, 0);
            case Dir.Left: return new Vector2Int(0, width);
            case Dir.Down: return new Vector2Int(width, height);
            case Dir.Right: return new Vector2Int(height, 0);
        }
    }
}
