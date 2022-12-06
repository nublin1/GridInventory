using GridInventorySystem;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEditor;

public class BaseItem
{
    private InventoryItemData m_itemData;
    private List<Vector2Int> gridPositionList;
    private Dir m_dir;
    private Transform m_itemTransform;

    private Image backgroundImage;
    private Image backgroundOutlineImage;
    private Image itemIconImage;

    #region getters
    public InventoryItemData ItemData { get => m_itemData; }
    public Dir Dir { get => m_dir; }
    public List<Vector2Int> GridPositionList { get => gridPositionList; set => gridPositionList = value; }
    public Transform ItemTransform { get => m_itemTransform; }
    public Image BackgroundImage { get => backgroundImage; }
    public Image BackgroundOutlineImage { get => backgroundOutlineImage; }
    public Image ItemIconImage { get => itemIconImage; }

    #endregion

    public void SetRotation(Dir dir)
    {
        m_dir = dir;
        var newRot = Quaternion.Euler(0, 0, InventoryUtilities.GetRotationAngle(m_dir));
        m_itemTransform.rotation = newRot;

    }

    public static BaseItem CreateItem(Dir dir, InventoryItemData data)
    {
        BaseItem item = new BaseItem();
        item.m_itemData = data;
        item.gridPositionList = CalculatePositionList(dir, data.Width, data.Height, Vector2Int.zero);
        item.m_dir = dir;

        return item;
    }

    public void CreateItemTransform(Vector2 cellSize)
    {
        GameObject itemObject = new();
        itemObject.name = ItemData.name;
        var itemRect = itemObject.AddComponent<RectTransform>();
        itemRect.sizeDelta = new Vector2(cellSize.x * m_itemData.Width, cellSize.y * m_itemData.Height);
        itemRect.anchorMin = new Vector2(0f, 1f);
        itemRect.anchorMax = new Vector2(0f, 1f);
        itemRect.pivot = new Vector2(0f, 1f);
        itemRect.anchoredPosition = new Vector2(0f, 0f);

        itemObject.transform.rotation = Quaternion.Euler(0, 0, InventoryUtilities.GetRotationAngle(m_dir));

        // Collider
        var collider2d = itemObject.AddComponent<BoxCollider2D>();
        collider2d.offset = new Vector2(cellSize.x * m_itemData.Width / 2, -cellSize.y * m_itemData.Height / 2);
        collider2d.size = new Vector2(cellSize.x * m_itemData.Width, cellSize.y * m_itemData.Height);
        collider2d.isTrigger = true;

        m_itemTransform = itemObject.transform;

        // background
        GameObject background = new("background");
        background.transform.parent = itemObject.transform;
        var itemBackgroundRect = background.AddComponent<RectTransform>();
        itemBackgroundRect.sizeDelta = itemRect.sizeDelta;
        itemBackgroundRect.anchoredPosition = new Vector2(0f, 0f);

        backgroundImage = background.AddComponent<Image>();
        backgroundImage.color = new Color(0, 0, 0, 0.15f);
        backgroundImage.raycastTarget = false;

        // background Outline
        GameObject backgroundOutline = new("backgroundOutline");
        backgroundOutline.transform.parent = itemObject.transform;
        var itemBackgroundOutlineRect = backgroundOutline.AddComponent<RectTransform>();
        itemBackgroundOutlineRect.sizeDelta = itemRect.sizeDelta;
        itemBackgroundOutlineRect.anchoredPosition = new Vector2(0f, 0f);

        backgroundOutlineImage = backgroundOutline.AddComponent<Image>();
        backgroundOutlineImage.sprite = (Sprite)AssetDatabase.LoadAssetAtPath("Assets/GridInventory/GUI/Square Outline.png", typeof(Sprite));
        backgroundOutlineImage.color = new Color(0, 0, 0, 0.6f);
        backgroundOutlineImage.raycastTarget = false;

        //itemIcon
        GameObject itemIcon = new("itemIcon");
        itemIcon.transform.parent = itemObject.transform;
        var itemIconRect = itemIcon.AddComponent<RectTransform>();
        itemIconRect.sizeDelta = itemRect.sizeDelta;
        itemIconRect.anchoredPosition = new Vector2(0f, 0f);

        itemIconImage = itemIcon.AddComponent<Image>();
        itemIconImage.sprite = m_itemData.Icon;
        itemIconImage.raycastTarget = false;


    }

    public void ReculculatePositionList(Vector2Int pivotPosition)
    {
        gridPositionList = CalculatePositionList(m_dir, m_itemData.Width, m_itemData.Height, pivotPosition);
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
        return m_dir switch
        {
            Dir.Left => new Vector2Int(0, m_itemData.Width),
            Dir.Down => new Vector2Int(m_itemData.Width, m_itemData.Height),
            Dir.Right => new Vector2Int(m_itemData.Height, 0),
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
