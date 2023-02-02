using GridInventorySystem;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using NaughtyAttributes;
using TMPro;


/*
* IileName: им€ по умолчанию при создании ассета.
* menuName: им€ ассета, отображаемое в Asset Menu.
* order: место размещени€ ассета в Asset Menu. Unity раздел€ет ассеты на подгруппы с множителем 50. “о есть значение 51 поместит новый ассет во вторую группу Asset Menu.
*/
[CreateAssetMenu(fileName = "New InventoryItem", menuName = "Items/InventoryItem", order = 51)]
[System.Serializable]
public class BaseItem : ScriptableObject, IDataPersistence
{
    #region GeneralSettings
    [SerializeField]
    private string m_Id;

    [SerializeField]
    private string m_ItemName = "New item";
    [BoxGroup("Images")]
    [SerializeField]
    private Sprite icon;
    [BoxGroup("Images")]
    [SerializeField]
    private bool m_categoryBasedColor = false;
    [BoxGroup("Images")]
    [HideIf("m_categoryBasedColor")]
    [SerializeField]
    private Color m_backgroundColor = new Color(0, 0, 0, 0.15f);
    [SerializeField]
    private int m_width = 1;
    [SerializeField]
    private int m_height = 1;
    [SerializeField]
    private GameObject m_prefab;
    [SerializeField]
    [Multiline(4)]
    private string m_Description = string.Empty;
    #endregion

    #region containerOptions
    [SerializeField]
    private bool isContainer;

    [ShowIf("isContainer")]
    [SerializeField]
    private GameObject pf_ItemContainer;
    private Transform itemContainer;
    #endregion

    #region StackOptions
    [SerializeField]
    [Range(1, 100)]
    private int m_Stack = 1;

    [SerializeField]
    [Range(1, 999f)]
    private int m_maxStack;

    [SerializeField]
    private bool m_showMaxStack;
    #endregion

    #region Other
    [RarityPicker(true)]
    [SerializeField]
    private Rarity rarity;
    [CategoryPicker(true)]
    [SerializeField]
    private Category m_category;
    #endregion
    
    private List<Vector2Int> gridPositionList;
    private Dir m_dir;
    private Transform m_itemTransform;

    private Image backgroundImage;
    private Image backgroundOutlineImage;
    private Image highlightImage;
    private Image itemIconImage;

    private RectTransform m_itemNameRect;
    private TextMeshProUGUI m_ItemNameText;
    private RectTransform m_itemCountRect;
    private TextMeshProUGUI m_ItemCountText;

    #region Properties
    public string Id { get => m_Id; set => m_Id = value; }   
    public string ItemName { get => m_ItemName; set => m_ItemName = value; }
    public Sprite Icon { get => icon; }
    public bool IsCategoryBasedColor { get => m_categoryBasedColor; set => m_categoryBasedColor = value; }
    public Color BackgroundColor { get => m_backgroundColor; }
    public int Width { get => m_width; }
    public int Height { get => m_height; }
    public GameObject Prefab { get => m_prefab; }
    public string Description { get => m_Description; }
    public bool IsContainer { get => isContainer; }
    public GameObject Pf_ItemContainer { get => pf_ItemContainer; }
    public Transform ItemContainer { get => itemContainer; set => itemContainer = value; }
    public int Stack { get => m_Stack; set => m_Stack = value; }
    public int MaxStack { get => m_maxStack; }
    public bool ShowMaxStack { get => m_showMaxStack; }
    public Dir Dir { get => m_dir; }
    public Category Category { get => m_category; }
    public List<Vector2Int> GridPositionList { get => gridPositionList; set => gridPositionList = value; }
    public Transform ItemTransform       { get => m_itemTransform; set => m_itemTransform = value; }
    public Image BackgroundImage         { get => backgroundImage; set => backgroundImage = value; }
    public Image BackgroundOutlineImage  { get => backgroundOutlineImage; set => backgroundOutlineImage = value; }
    public Image HighlightImage          { get => highlightImage; set => highlightImage = value; }
    public Image ItemIconImage           { get => itemIconImage; set => itemIconImage = value; }
    public TextMeshProUGUI ItemNameText  { get => m_ItemNameText; set => m_ItemNameText = value; }
    public TextMeshProUGUI ItemCountText { get => m_ItemCountText; set => m_ItemCountText = value; }
    #endregion

    public void Init(Dir dir = Dir.Up)
    {
        m_dir = dir;
        gridPositionList = CalculatePositionList(dir, m_width, m_height, Vector2Int.zero);
    }    

    private void Awake()
    {
        name = ItemName;        
    }   

    public void Update()
    {
        if (m_itemTransform == null)
            return;

        var bounds = m_itemTransform.GetComponent<BoxCollider2D>().bounds;
        if (bounds != null && bounds.Contains(Input.mousePosition))
            highlightImage.enabled = true;
        else
            highlightImage.enabled = false;        
    }

    public void SetRotation(Dir dir)
    {
        if (dir == m_dir)
            return;

        m_dir = dir;
        var newRot = Quaternion.Euler(0, 0, Utilities.GetRotationAngle(m_dir));
        m_itemTransform.rotation = newRot;

        var newSize = new Vector2(m_itemNameRect.sizeDelta.y, m_itemNameRect.sizeDelta.x);
        m_itemNameRect.localRotation = Quaternion.Inverse(newRot);
        m_itemNameRect.sizeDelta = newSize;

        m_itemCountRect.localRotation = Quaternion.Inverse(newRot);
        m_itemCountRect.sizeDelta = newSize;

    }

    public void UpdateDisplayItemCount()
    {
        if (m_maxStack <= 1)
            return;

        if (m_showMaxStack)
            m_ItemCountText.text = m_Stack.ToString() + "/" + m_maxStack.ToString();
        else
            m_ItemCountText.text = m_Stack.ToString();
    }

    public void CreateItemTransform(Vector2 cellSize)
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
    }

    public void ReculculatePositionList(Vector2Int pivotPosition)
    {
        gridPositionList = CalculatePositionList(m_dir, m_width, m_height, pivotPosition);
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
            Dir.Left => new Vector2Int(0, m_width),
            Dir.Down => new Vector2Int(m_width, m_height),
            Dir.Right => new Vector2Int(m_height, 0),
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

    public void LoadData(Dictionary<string, object> data)
    {
        
    }

    public void SaveData(ref Dictionary<string, object> data)
    {
        data.Add("ID", Id);
        data.Add("Name", m_ItemName);
        data.Add("Stack", m_Stack);
        data.Add("Dir", Dir);
        if (gridPositionList != null)
            data.Add("Position", gridPositionList[0]);
        else
            data.Add("Position", new Vector2Int(-1, -1));
    }
}
