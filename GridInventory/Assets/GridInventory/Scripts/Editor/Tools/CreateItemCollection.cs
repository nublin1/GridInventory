using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;


public class WindowInfo : ScriptableObject
{
    public int sizeX, sizeY;
    public Vector2Int cellSize = new Vector2Int(50, 50);
    public float second;

    public Color cellColor = Color.black;
    public Texture cellImage;
}

public class CreateItemCollection : EditorWindow
{
    private static WindowInfo info;
    private SerializedObject windowInfoSO;

    //Inventory
    private SerializedProperty sizeX, sizeY;

    private SerializedProperty cellSize;
    private SerializedProperty cellColor;
    private SerializedProperty cellImage;

    private Color inventoryBackgroundColor = Color.white;
    private Sprite inventoryBackground;

    private bool enableBackgroundOutline;
    private Color BackgroundOutlineColor = Color.black;
    private Sprite containerBackgroundOutline;

    private bool enableHeader;
    private Vector2Int HeaderSize = new Vector2Int(0, 12);
    private Color HeaderColor = Color.black;
    private Sprite headerBackGroundSprite;
    private string headerTitle_Text = "";
    private bool draggableHeader = true;

    private Sprite viewportSprite;

    //
    public int tabIndex = 0;    
    public string[] tabHeaders = new string[] { "Inventory" };

    [MenuItem("Tools/Create new inventory")]
    static void Init()
    {
        CreateItemCollection window = (CreateItemCollection)EditorWindow.GetWindow(typeof(CreateItemCollection));
        window.Show();
        window.titleContent = new GUIContent("Inventory Editor");


        // Limit size of the window
        window.minSize = new Vector2(450, 500);
        window.maxSize = new Vector2(1920, 720);
    }

    [InitializeOnLoadMethod]
    private static void OnLoad()
    {
        info = AssetDatabase.LoadAssetAtPath<WindowInfo>("Assets/WindowInfo.asset");
        if (!info)
        {
            info = CreateInstance<WindowInfo>();
            AssetDatabase.CreateAsset(info, "Assets/WindowInfo.asset");
            AssetDatabase.Refresh();
        }
    }

    private void CreateGUI()
    {
        windowInfoSO = new SerializedObject(info);
        sizeX = windowInfoSO.FindProperty("sizeX");
        sizeY = windowInfoSO.FindProperty("sizeY");
        cellSize = windowInfoSO.FindProperty("cellSize");

        cellColor = windowInfoSO.FindProperty("cellColor");
        cellImage = windowInfoSO.FindProperty("cellImage");

        //inventoryBackground = EditorGUIUtility.Load("Assets/GridInventory/GUI/Square.png") as Sprite;
        inventoryBackground = (Sprite)AssetDatabase.LoadAssetAtPath("Assets/GridInventory/GUI/Square.png", typeof(Sprite));
        containerBackgroundOutline = (Sprite)AssetDatabase.LoadAssetAtPath("Assets/GridInventory/GUI/Square Outline.png", typeof(Sprite));
        headerBackGroundSprite = (Sprite)AssetDatabase.LoadAssetAtPath("Assets/GridInventory/GUI/Header.png", typeof(Sprite));
        viewportSprite = (Sprite)AssetDatabase.LoadAssetAtPath("Resources/unity_builtin_extra/UIMask.png", typeof(Sprite));
    }

    void OnGUI()
    {
        windowInfoSO.Update();

        tabIndex = GUILayout.Toolbar(tabIndex, tabHeaders);

        if (tabIndex == 0)
        {      
            EditorGUILayout.PropertyField(sizeX, GUILayout.ExpandWidth(false));
            EditorGUILayout.PropertyField(sizeY, GUILayout.ExpandWidth(false));

            EditorGUILayout.PropertyField(cellSize, GUILayout.MaxWidth(200));
            EditorGUILayout.PropertyField(cellColor);
            EditorGUILayout.PropertyField(cellImage, false);

            EditorGUILayout.Space();

            inventoryBackgroundColor = EditorGUILayout.ColorField("Background color", inventoryBackgroundColor);
            inventoryBackground = (Sprite)EditorGUILayout.ObjectField("Container background", inventoryBackground, typeof(Sprite), false);

            enableBackgroundOutline = EditorGUILayout.Toggle("Container background outline", enableBackgroundOutline);
            if (enableBackgroundOutline)
            {
                BackgroundOutlineColor = EditorGUILayout.ColorField("Background outline color", BackgroundOutlineColor);
                containerBackgroundOutline = (Sprite)EditorGUILayout.ObjectField("Container background outline", containerBackgroundOutline, typeof(Sprite), false);
            }

            enableHeader = EditorGUILayout.Toggle("Container header", enableHeader);
            if (enableHeader)
            {
                HeaderColor = EditorGUILayout.ColorField("Header color", HeaderColor);
                headerBackGroundSprite = (Sprite)EditorGUILayout.ObjectField("Header background", headerBackGroundSprite, typeof(Sprite), false);
                headerTitle_Text = GUILayout.TextField(headerTitle_Text);

            }

            if (GUILayout.Button("Build inventory") && CheckCompleteness())
            {
                BuildInventory();
            }

            windowInfoSO.ApplyModifiedProperties();
        }
    }

    private void BuildInventory()
    {
        // Drawing Canvas
        var obj = new GameObject();
        obj.AddComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
        obj.AddComponent<CanvasScaler>();
        obj.AddComponent<GraphicRaycaster>();
        obj.name = "Container Canvas";

        //
        var container = new GameObject("container");
        container.transform.parent = obj.transform;
        container.transform.localPosition = Vector3.zero;

        var containerRect = container.AddComponent<RectTransform>();
        containerRect.sizeDelta = new Vector2(info.cellSize.x * info.sizeX, info.cellSize.y * info.sizeY);

        // Background
        var background = new GameObject("background");
        background.transform.parent = container.transform;
        background.transform.localPosition = Vector3.zero;

        var backgroundRect = background.AddComponent<RectTransform>();
        backgroundRect.anchorMin = new Vector2(0, 0);
        backgroundRect.anchorMax = new Vector2(1, 1);
        backgroundRect.pivot = new Vector2(0.5f, .5f);
        backgroundRect.sizeDelta = Vector2.zero;

        Image backgroundImage = background.AddComponent<Image>();
        backgroundImage.sprite = inventoryBackground;
        backgroundImage.color = inventoryBackgroundColor;
        backgroundImage.type = Image.Type.Sliced;

        // Background Outline
        if (enableBackgroundOutline)
        {
            var backgroundOutline = new GameObject("backgroundOutline");
            backgroundOutline.transform.parent = container.transform;
            backgroundOutline.transform.localPosition = Vector3.zero;

            var backgroundOutlineRect = backgroundOutline.AddComponent<RectTransform>();
            backgroundOutlineRect.anchorMin = new Vector2(0, 0);
            backgroundOutlineRect.anchorMax = new Vector2(1, 1);
            backgroundOutlineRect.pivot = new Vector2(0.5f, .5f);
            backgroundOutlineRect.sizeDelta = Vector2.zero;

            Image backgroundOutlineImage = backgroundOutline.AddComponent<Image>();
            backgroundOutlineImage.sprite = containerBackgroundOutline;
            backgroundOutlineImage.color = inventoryBackgroundColor;
            backgroundOutlineImage.type = Image.Type.Sliced;
        }

        // Header
        if (enableHeader)
        {
            var header = new GameObject("Header");
            header.transform.parent = container.transform;
            header.transform.localPosition = Vector3.zero;

            RectTransform rectHeader = header.AddComponent<RectTransform>();
            rectHeader.anchorMin = new Vector2(0, 1);
            rectHeader.anchorMax = new Vector2(1, 1);
            rectHeader.pivot = new Vector2(0.5f, 0f);
            rectHeader.sizeDelta = new Vector2(0, HeaderSize.y);

            // HeaderBackground
            var headerBackground = new GameObject("headerBackground");
            headerBackground.transform.parent = header.transform;
            headerBackground.transform.localPosition = Vector3.zero;

            RectTransform headerBackgroundRect = headerBackground.AddComponent<RectTransform>();
            headerBackgroundRect.anchorMin = new Vector2(0, 1);
            headerBackgroundRect.anchorMax = new Vector2(1, 1);
            headerBackgroundRect.pivot = new Vector2(0f, 1f);
            headerBackgroundRect.sizeDelta = new Vector2(0, HeaderSize.y);

            Image headerBackgroundImage = headerBackground.AddComponent<Image>();
            headerBackgroundImage.sprite = headerBackGroundSprite;
            headerBackgroundImage.color = new Color(0, 0, 0, 1);
            headerBackgroundImage.type = Image.Type.Sliced;

            // Header Title
            GameObject headerTitle = new GameObject("Title");
            headerTitle.transform.parent = header.transform;

            RectTransform rectTitle = headerTitle.AddComponent<RectTransform>();
            rectTitle.anchorMin = new Vector2(0, 0);
            rectTitle.anchorMax = new Vector2(1, 1);
            rectTitle.pivot = new Vector2(0.5f, 1f);
            rectTitle.offsetMin = new Vector2(5f, 0f);
            rectTitle.offsetMax = new Vector2(-5f, 0f);

            TextMeshProUGUI textMeshTitle = headerTitle.AddComponent<TextMeshProUGUI>();
            textMeshTitle.text = headerTitle_Text;
            textMeshTitle.fontStyle = (FontStyles)FontStyle.Bold;
            textMeshTitle.fontSizeMin = 5;
            textMeshTitle.fontSizeMax = 72;
            //textMeshTitle.alignment = (TextAlignmentOptions)TextAnchor.MiddleLeft;
            textMeshTitle.enableAutoSizing = true;

            if (draggableHeader)
            {
                header.AddComponent<DragPanel>();
            }
        }

        // Viewport
        var viewport = new GameObject("Viewport");
        viewport.transform.parent = container.transform;
        viewport.transform.localPosition = Vector3.zero;

        RectTransform rectViewport = viewport.AddComponent<RectTransform>();
        rectViewport.anchorMin = new Vector2(0, 0);
        rectViewport.anchorMax = new Vector2(1, 1);
        rectViewport.pivot = new Vector2(0, 1);
        rectViewport.sizeDelta = new Vector2(0, 0);

        viewport.AddComponent<Mask>();

        Image ViewportImage = viewport.AddComponent<Image>();
        ViewportImage.sprite = viewportSprite;
        ViewportImage.color = Color.white;
        ViewportImage.type = Image.Type.Sliced;

        // GridImage
        var gridImage = new GameObject("GridImage");
        gridImage.transform.parent = viewport.transform;

        var gridImageRect = gridImage.AddComponent<RectTransform>();
        gridImageRect.anchorMin = new Vector2(0, 1);
        gridImageRect.anchorMax = new Vector2(0, 1);
        gridImageRect.pivot = new Vector2(0, 1);
        gridImageRect.transform.localPosition = Vector3.zero;
        gridImageRect.sizeDelta = new Vector2(info.sizeX * info.cellSize.x, info.sizeY * info.cellSize.y);

        var gridTexture = gridImage.AddComponent<RawImage>();
        gridTexture.color = info.cellColor;
        gridTexture.texture = info.cellImage;
        gridTexture.uvRect = new Rect(0, 0, info.sizeX, info.sizeY);

        // ItemsContainer
        var ItemsContainer = new GameObject("ItemsContainer");
        ItemsContainer.transform.parent = viewport.transform;

        var ItemsContainerRect = ItemsContainer.AddComponent<RectTransform>();
        ItemsContainerRect.anchorMin = new Vector2(0, 1);
        ItemsContainerRect.anchorMax = new Vector2(0, 1);
        ItemsContainerRect.pivot = new Vector2(0, 1);
        ItemsContainerRect.transform.localPosition = Vector3.zero;
        ItemsContainerRect.sizeDelta = Vector2.zero;


        // ItemCollection
        var itemCollection = container.AddComponent<ItemsCollection>();
        itemCollection.GridWidth = info.sizeX;
        itemCollection.GridHeight = info.sizeY;
        itemCollection.CellSize = info.cellSize;
        itemCollection.ContainerTransform = ItemsContainer.transform;

    }

    private bool CheckCompleteness()
    {
        if (info.sizeX < 1 || info.sizeY < 1)
        {
            EditorUtility.DisplayDialog("Setup uncompleted", " Please check container size, value can't be less 1", "Continue");
            return false;
        }

        if (info.cellSize.x < 1 || info.cellSize.y < 1)
        {
            EditorUtility.DisplayDialog("Setup uncompleted", " Please check cell size, value can't be less 1", "Continue");
            return false;
        }

        if (info.cellImage == null)
        {
            EditorUtility.DisplayDialog("Setup uncompleted", " Please attach sprite to cell image field. Otherwise, inventory cells will be invisible", "Continue");
            return false;
        }

        return true;
    }
}