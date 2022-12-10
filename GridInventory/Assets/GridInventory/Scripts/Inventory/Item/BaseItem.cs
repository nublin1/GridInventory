using GridInventorySystem;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using NaughtyAttributes;

/*
* IileName: им€ по умолчанию при создании ассета.
* menuName: им€ ассета, отображаемое в Asset Menu.
* order: место размещени€ ассета в Asset Menu. Unity раздел€ет ассеты на подгруппы с множителем 50. “о есть значение 51 поместит новый ассет во вторую группу Asset Menu.
*/
[CreateAssetMenu(fileName = "New InventoryItem", menuName = "InventoryItem", order = 51)]
public class BaseItem : ScriptableObject
{
    #region GeneralSettings
    [ShowNonSerializedField]
    private string m_Id;

    [SerializeField]
    private string itemName;
    [SerializeField]
    private Sprite icon;
    [SerializeField]
    private int width;
    [SerializeField]
    private int height;
    #endregion
    
    private List<Vector2Int> gridPositionList;
    private Dir m_dir;
    private Transform m_itemTransform;

    private Image backgroundImage;
    private Image backgroundOutlineImage;
    private Image highlightImage;
    private Image itemIconImage;

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
    #endregion

    #region Properties
    public string Id { get => m_Id; }
    public string ItemName { get => itemName; }
    public Sprite Icon { get => icon; }
    public int Width { get => width; }
    public int Height { get => height; }
    public bool IsContainer { get => isContainer; }
    public GameObject Pf_ItemContainer { get => pf_ItemContainer; }
    public Transform ItemContainer { get => itemContainer; set => itemContainer = value; }
    public int Stack { get => m_Stack; set => m_Stack = value; }    
    public Dir Dir { get => m_dir; }
    public List<Vector2Int> GridPositionList { get => gridPositionList; set => gridPositionList = value; }
    public Transform ItemTransform { get => m_itemTransform; }
    public Image BackgroundImage { get => backgroundImage; }
    public Image BackgroundOutlineImage { get => backgroundOutlineImage; }
    public Image HighlightImage { get => highlightImage; }
    public Image ItemIconImage { get => itemIconImage; }

    #endregion

    public void Init(Dir dir = Dir.Up)
    {
        m_dir = dir;
        gridPositionList = CalculatePositionList(dir, width, height, Vector2Int.zero);    
    }

    protected virtual void OnEnable()
    {
        if (string.IsNullOrEmpty(this.m_Id))
        {
            this.m_Id = System.Guid.NewGuid().ToString();
        }
    }

    public void Update ()
    {
        var bounds = m_itemTransform.GetComponent<BoxCollider2D>().bounds;
        if (bounds != null && bounds.Contains(Input.mousePosition))        
            highlightImage.enabled = true;
        else 
            highlightImage.enabled = false;
        
    }

    public void SetRotation(Dir dir)
    {
        m_dir = dir;
        var newRot = Quaternion.Euler(0, 0, InventoryUtilities.GetRotationAngle(m_dir));
        m_itemTransform.rotation = newRot;

    }    

    public void CreateItemTransform(Vector2 cellSize)
    {
        GameObject itemObject = new();
        itemObject.name = itemName;
        var itemRect = itemObject.AddComponent<RectTransform>();
        itemRect.sizeDelta = new Vector2(cellSize.x * width, cellSize.y * height);
        itemRect.anchorMin = new Vector2(0f, 1f);
        itemRect.anchorMax = new Vector2(0f, 1f);
        itemRect.pivot = new Vector2(0f, 1f);
        itemRect.anchoredPosition = new Vector2(0f, 0f);

        itemObject.transform.rotation = Quaternion.Euler(0, 0, InventoryUtilities.GetRotationAngle(m_dir));

        // Collider
        var collider2d = itemObject.AddComponent<BoxCollider2D>();
        collider2d.offset = new Vector2(cellSize.x * width / 2, -cellSize.y * height / 2);
        collider2d.size = new Vector2(cellSize.x * width, cellSize.y * height);
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


    }

    public void ReculculatePositionList(Vector2Int pivotPosition)
    {
        gridPositionList = CalculatePositionList(m_dir, width, height, pivotPosition);
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
            Dir.Left => new Vector2Int(0, width),
            Dir.Down => new Vector2Int(width, height),
            Dir.Right => new Vector2Int(height, 0),
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
