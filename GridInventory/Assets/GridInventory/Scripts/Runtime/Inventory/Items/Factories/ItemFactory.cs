using GridInventorySystem;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEditor;
using UnityEngine;

public class ItemFactory : MonoBehaviour
{
    /*
    public static Transform CreateItemObject()
    {
        GameObject itemObject = new();
        itemObject.name = m_ItemName;


        var itemRect = itemObject.AddComponent<RectTransform>();
        itemRect.sizeDelta = new Vector2(cellSize.x * m_width, cellSize.y * m_height);
        itemRect.anchorMin = new Vector2(0f, 1f);
        itemRect.anchorMax = new Vector2(0f, 1f);
        itemRect.pivot = new Vector2(0f, 1f);
        itemRect.anchoredPosition = new Vector2(0f, 0f);

        itemObject.transform.rotation = Quaternion.Euler(0, 0, Utilities.GetRotationAngle(m_dir));

        // Collider
        var collider2d = itemObject.AddComponent<BoxCollider2D>();
        collider2d.offset = new Vector2(cellSize.x * m_width / 2, -cellSize.y * m_height / 2);
        collider2d.size = new Vector2(cellSize.x * m_width, cellSize.y * m_height);
        collider2d.isTrigger = true;


        // background
        GameObject background = new("background");
        background.transform.parent = itemObject.transform;
        var itemBackgroundRect = background.AddComponent<RectTransform>();
        itemBackgroundRect.sizeDelta = itemRect.sizeDelta;
        itemBackgroundRect.anchoredPosition = new Vector2(0f, 0f);

        backgroundImage = background.AddComponent<Image>();
        if (m_categoryBasedColor && m_category != null)
            backgroundImage.color = m_category.Color;
        else
            backgroundImage.color = m_backgroundColor;
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

        //highlight
        GameObject highlight = new("highlight");
        highlight.transform.parent = itemObject.transform;
        var highlightRect = highlight.AddComponent<RectTransform>();
        highlightRect.sizeDelta = itemRect.sizeDelta;
        highlightRect.anchoredPosition = new Vector2(0f, 0f);

        highlightImage = highlight.AddComponent<Image>();
        highlightImage.color = new Color(1, 1, 1, .09f);
        highlightImage.raycastTarget = false;
        highlightImage.enabled = false;

        //itemIcon
        GameObject itemIcon = new("itemIcon");
        itemIcon.transform.parent = itemObject.transform;
        var itemIconRect = itemIcon.AddComponent<RectTransform>();
        itemIconRect.sizeDelta = itemRect.sizeDelta;
        itemIconRect.anchoredPosition = new Vector2(0f, 0f);

        itemIconImage = itemIcon.AddComponent<Image>();
        itemIconImage.sprite = Icon;
        itemIconImage.raycastTarget = false;

        // ItemName
        GameObject itemNameGO = new("itemName");
        itemNameGO.transform.parent = itemObject.transform;
        m_itemNameRect = itemNameGO.AddComponent<RectTransform>();
        m_ItemNameText = itemNameGO.AddComponent<TextMeshProUGUI>();
        m_ItemNameText.fontSize = 10;
        m_ItemNameText.alignment = TextAlignmentOptions.TopRight;
        m_ItemNameText.text = ItemName;
        m_ItemNameText.raycastTarget = false;

        m_itemNameRect.sizeDelta = itemRect.sizeDelta;
        m_itemNameRect.anchoredPosition = new Vector2(0f, 0f);

        // ItemCount
        GameObject ItemCountGO = new("ItemCount");
        ItemCountGO.transform.parent = itemObject.transform;
        m_itemCountRect = ItemCountGO.AddComponent<RectTransform>();
        m_ItemCountText = ItemCountGO.AddComponent<TextMeshProUGUI>();
        m_ItemCountText.SetNativeSize();
        m_ItemCountText.fontSize = 10;
        m_ItemCountText.alignment = TextAlignmentOptions.BottomRight;
        m_ItemCountText.raycastTarget = false;

        m_itemCountRect.sizeDelta = itemRect.sizeDelta;
        m_itemCountRect.anchoredPosition = new Vector2(0f, 0f);

        if (m_maxStack <= 1)
            m_ItemCountText.enabled = false;

        UpdateDisplayItemCount();
        m_itemTransform = itemObject.transform;
    } */
}
