using GridInventorySystem;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ItemFactory : MonoBehaviour
{
    /*
    public static Transform CreateItemObject(BaseItem item)
    {
        GameObject itemObject = new();
        itemObject.name = item.ItemName;


        var itemRect = itemObject.AddComponent<RectTransform>();
        itemRect.sizeDelta = new Vector2(cellSize.x * item.Width, cellSize.y * item.Height);
        itemRect.anchorMin = new Vector2(0f, 1f);
        itemRect.anchorMax = new Vector2(0f, 1f);
        itemRect.pivot = new Vector2(0f, 1f);
        itemRect.anchoredPosition = new Vector2(0f, 0f);

        itemObject.transform.rotation = Quaternion.Euler(0, 0, Utilities.GetRotationAngle(item.Dir));

        // Collider
        var collider2d = itemObject.AddComponent<BoxCollider2D>();
        collider2d.offset = new Vector2(cellSize.x * item.Width / 2, -cellSize.y * item.Height / 2);
        collider2d.size = new Vector2(cellSize.x * item.Width, cellSize.y * item.Height);
        collider2d.isTrigger = true;


        // background
        GameObject background = new("background");
        background.transform.parent = itemObject.transform;
        var itemBackgroundRect = background.AddComponent<RectTransform>();
        itemBackgroundRect.sizeDelta = itemRect.sizeDelta;
        itemBackgroundRect.anchoredPosition = new Vector2(0f, 0f);

        item.BackgroundImage = background.AddComponent<Image>();
        if (item.IsCategoryBasedColor && m_category != null)
            item.BackgroundImage.color = m_category.Color;
        else
            item.BackgroundImage.color = item.BackgroundColor;
        item.BackgroundImage.raycastTarget = false;

        // background Outline
        GameObject backgroundOutline = new("backgroundOutline");
        backgroundOutline.transform.parent = itemObject.transform;
        var itemBackgroundOutlineRect = backgroundOutline.AddComponent<RectTransform>();
        itemBackgroundOutlineRect.sizeDelta = itemRect.sizeDelta;
        itemBackgroundOutlineRect.anchoredPosition = new Vector2(0f, 0f);

        item.BackgroundOutlineImage = backgroundOutline.AddComponent<Image>();
        item.BackgroundOutlineImage.sprite = (Sprite)AssetDatabase.LoadAssetAtPath("Assets/GridInventory/GUI/Square Outline.png", typeof(Sprite));
        item.BackgroundOutlineImage.color = new Color(0, 0, 0, 0.6f);
        item.BackgroundOutlineImage.raycastTarget = false;

        //highlight
        GameObject highlight = new("highlight");
        highlight.transform.parent = itemObject.transform;
        var highlightRect = highlight.AddComponent<RectTransform>();
        highlightRect.sizeDelta = itemRect.sizeDelta;
        highlightRect.anchoredPosition = new Vector2(0f, 0f);

        item.HighlightImage = highlight.AddComponent<Image>();
        item.HighlightImage.color = new Color(1, 1, 1, .09f);
        item.HighlightImage.raycastTarget = false;
        item.HighlightImage.enabled = false;

        //itemIcon
        GameObject itemIcon = new("itemIcon");
        itemIcon.transform.parent = itemObject.transform;
        var itemIconRect = itemIcon.AddComponent<RectTransform>();
        itemIconRect.sizeDelta = itemRect.sizeDelta;
        itemIconRect.anchoredPosition = new Vector2(0f, 0f);

        item.ItemIconImage = itemIcon.AddComponent<Image>();
        item.ItemIconImage.sprite = item.Icon;
        item.ItemIconImage.raycastTarget = false;

        // ItemName
        GameObject itemNameGO = new("itemName");
        itemNameGO.transform.parent = itemObject.transform;
        m_itemNameRect = itemNameGO.AddComponent<RectTransform>();
        item.ItemNameText = itemNameGO.AddComponent<TextMeshProUGUI>();
        item.ItemNameText.fontSize = 10;
        item.ItemNameText.alignment = TextAlignmentOptions.TopRight;
        item.ItemNameText.text = item.ItemName;
        item.ItemNameText.raycastTarget = false;

        m_itemNameRect.sizeDelta = itemRect.sizeDelta;
        m_itemNameRect.anchoredPosition = new Vector2(0f, 0f);

        // ItemCount
        GameObject ItemCountGO = new("ItemCount");
        ItemCountGO.transform.parent = itemObject.transform;
        m_itemCountRect = ItemCountGO.AddComponent<RectTransform>();
        item.ItemCountText = ItemCountGO.AddComponent<TextMeshProUGUI>();
        item.ItemCountText.SetNativeSize();
        item.ItemCountText.fontSize = 10;
        item.ItemCountText.alignment = TextAlignmentOptions.BottomRight;
        item.ItemCountText.raycastTarget = false;

        m_itemCountRect.sizeDelta = itemRect.sizeDelta;
        m_itemCountRect.anchoredPosition = new Vector2(0f, 0f);

        if (m_maxStack <= 1)
            m_ItemCountText.enabled = false;

        item.UpdateDisplayItemCount();
        m_itemTransform = itemObject.transform;
    } */
}
