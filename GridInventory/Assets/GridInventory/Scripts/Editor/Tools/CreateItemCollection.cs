using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class WindowInfo : ScriptableObject
{
   
    //public Object rootTransform;

    public int sizeX, sizeY;
    public Vector2Int cellSize = new Vector2Int(50, 50);
    public float second;

    public Color cellColor = Color.black;
    public Texture cellImage;

    public Color inventoryBackgroundColor = Color.white;
    public Sprite inventoryBackground;

    public bool enableBackgroundOutline;
    public Color backgroundOutlineColor = Color.black;
    public Sprite backgroundOutlineSprite;
}

public class CreateItemCollection : EditorWindow
{
    private static WindowInfo windowInfo;
    private SerializedObject windowInfoSO;

    private GameObject rootObj;

    //Inventory
    //private SerializedProperty rootTranform;

    private SerializedProperty sizeX, sizeY;

    private SerializedProperty cellSize;
    private SerializedProperty cellColor;
    private SerializedProperty cellImage;

    private SerializedProperty inventoryBackgroundColor;
    private SerializedProperty inventoryBackground;

    private SerializedProperty enableBackgroundOutline;
    private SerializedProperty backgroundOutlineColor;
    private SerializedProperty backgroundOutlineSprite;

    private bool enableHeader;
    private Vector2Int HeaderSize = new Vector2Int(0, 12);
    private Color HeaderColor = Color.black;
    private Sprite headerBackGroundSprite;
    private string headerTitle_Text = "";
    private bool draggableHeader = true;

    private Sprite viewportSprite;

    private bool enabledVerticalScrollbar;
    private Vector2Int sizeOfViewport;
    private Sprite scrollBackground;
    private Sprite handleSprite;

    private float slidebarWidth = 10;

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
        windowInfo = (WindowInfo)AssetDatabase.LoadAssetAtPath("Assets/WindowInfo.asset", typeof(WindowInfo));
        if (!windowInfo)
        {
            windowInfo = CreateInstance<WindowInfo>();
            AssetDatabase.CreateAsset(windowInfo, "Assets/WindowInfo.asset");
            AssetDatabase.Refresh();
            Debug.Log("Cr");
        }
    }

    private void OnDestroy()
    {
        AssetDatabase.SaveAssets();
    }

    private void CreateGUI()
    {
        windowInfoSO = new SerializedObject(windowInfo);        
        
        //rootTranform = windowInfoSO.FindProperty("rootTransform");

        sizeX = windowInfoSO.FindProperty("sizeX");
        sizeY = windowInfoSO.FindProperty("sizeY");
        cellSize = windowInfoSO.FindProperty("cellSize");

        cellColor = windowInfoSO.FindProperty("cellColor");
        cellImage = windowInfoSO.FindProperty("cellImage");

        inventoryBackgroundColor = windowInfoSO.FindProperty("inventoryBackgroundColor");
        inventoryBackground = windowInfoSO.FindProperty("inventoryBackground");

        enableBackgroundOutline = windowInfoSO.FindProperty("enableBackgroundOutline");
        backgroundOutlineColor = windowInfoSO.FindProperty("backgroundOutlineColor");
        backgroundOutlineSprite = windowInfoSO.FindProperty("backgroundOutlineSprite");

        LoadDefaultResources();
    }

    private void LoadDefaultResources()
    {
        windowInfo.inventoryBackground = (Sprite)AssetDatabase.LoadAssetAtPath("Assets/GridInventory/GUI/Square.png", typeof(Sprite));
        windowInfo.backgroundOutlineSprite = (Sprite)AssetDatabase.LoadAssetAtPath("Assets/GridInventory/GUI/Square Outline.png", typeof(Sprite));
        headerBackGroundSprite = (Sprite)AssetDatabase.LoadAssetAtPath("Assets/GridInventory/GUI/Header.png", typeof(Sprite));
        viewportSprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
        scrollBackground = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Background.psd");
        handleSprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
    }

    void OnGUI()
    {
        windowInfoSO.Update();

        tabIndex = GUILayout.Toolbar(tabIndex, tabHeaders);

        if (tabIndex == 0)
        {
            var rootTextInfo = "The object to which the inventory will be attached after creation. If empty, a new object will be created";
            //EditorGUILayout.PropertyField(rootTranform, new GUIContent("RootTranform (Optional)", rootTextInfo), true);
            rootObj = (GameObject)EditorGUILayout.ObjectField(new GUIContent("RootTranform (Optional)", rootTextInfo), rootObj, typeof(GameObject), true);

            EditorGUILayout.PropertyField(sizeX, GUILayout.ExpandWidth(false));
            EditorGUILayout.PropertyField(sizeY, GUILayout.ExpandWidth(false));

            EditorGUILayout.PropertyField(cellSize, GUILayout.MaxWidth(200));
            EditorGUILayout.PropertyField(cellColor);
            EditorGUILayout.PropertyField(cellImage, false);

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(inventoryBackgroundColor);
            EditorGUILayout.PropertyField(inventoryBackground, false);

            EditorGUILayout.PropertyField(enableBackgroundOutline);
            if (enableBackgroundOutline.boolValue)
            {
                EditorGUILayout.PropertyField(backgroundOutlineColor, false);
                EditorGUILayout.PropertyField(backgroundOutlineSprite, false);
            }

            
            enableHeader = EditorGUILayout.Toggle("Container header", enableHeader);
            if (enableHeader)
            {
                HeaderColor = EditorGUILayout.ColorField("Header color", HeaderColor);
                headerBackGroundSprite = (Sprite)EditorGUILayout.ObjectField("Header background", headerBackGroundSprite, typeof(Sprite), false);
                headerTitle_Text = GUILayout.TextField(headerTitle_Text);

            }

            enabledVerticalScrollbar = EditorGUILayout.Toggle("verticalScrollbar", enabledVerticalScrollbar);
            if (enabledVerticalScrollbar)
            {
                sizeOfViewport = EditorGUILayout.Vector2IntField("sizeOfViewport", sizeOfViewport, GUILayout.ExpandWidth(false));
            }
           
            if (GUILayout.Button("Build inventory") && CheckReadyToBuildInventory())
                BuildInventory();


            windowInfoSO.ApplyModifiedProperties();
        }
    }

    private void BuildInventory()
    {
        GameObject obj;

        if (rootObj == null)
        {
            // Drawing Canvas
            obj = new GameObject();
            obj.AddComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            obj.AddComponent<CanvasScaler>();
            obj.AddComponent<GraphicRaycaster>();
            obj.name = "Container Canvas";
        }
        else
        {
            obj = rootObj;
        }

        //
        var container = new GameObject("container");
        container.transform.parent = obj.transform;
        container.transform.localPosition = Vector3.zero;
       

        var containerRect = container.AddComponent<RectTransform>();
        containerRect.sizeDelta = new Vector2(windowInfo.cellSize.x * windowInfo.sizeX, windowInfo.cellSize.y * windowInfo.sizeY);
        if (enabledVerticalScrollbar == true)
            containerRect.sizeDelta = new Vector2(windowInfo.cellSize.x * sizeOfViewport.x + slidebarWidth, windowInfo.cellSize.y * sizeOfViewport.y);

        // Visual
        var visual = new GameObject("Visual");
        visual.transform.parent = container.transform;
        visual.transform.localPosition = Vector3.zero;

        var visualRect = visual.AddComponent<RectTransform>();
        visualRect.anchorMin = new Vector2(0, 0);
        visualRect.anchorMax = new Vector2(1, 1);
        visualRect.pivot = new Vector2(0.5f, .5f);
        visualRect.sizeDelta = Vector2.zero;

        // Background
        var background = new GameObject("background");
        background.transform.parent = visual.transform;
        background.transform.localPosition = Vector3.zero;

        var backgroundRect = background.AddComponent<RectTransform>();
        backgroundRect.anchorMin = new Vector2(0, 0);
        backgroundRect.anchorMax = new Vector2(1, 1);
        backgroundRect.pivot = new Vector2(0.5f, .5f);
        backgroundRect.sizeDelta = Vector2.zero;

        Image backgroundImage = background.AddComponent<Image>();
        backgroundImage.sprite = windowInfo.inventoryBackground;
        backgroundImage.color = windowInfo.inventoryBackgroundColor;
        backgroundImage.type = Image.Type.Sliced;
        backgroundImage.raycastTarget = false;

        // Background Outline
        if (windowInfo.enableBackgroundOutline)
        {
            var backgroundOutline = new GameObject("backgroundOutline");
            backgroundOutline.transform.parent = visual.transform;
            backgroundOutline.transform.localPosition = Vector3.zero;

            var backgroundOutlineRect = backgroundOutline.AddComponent<RectTransform>();
            backgroundOutlineRect.anchorMin = new Vector2(0, 0);
            backgroundOutlineRect.anchorMax = new Vector2(1, 1);
            backgroundOutlineRect.pivot = new Vector2(0.5f, .5f);
            backgroundOutlineRect.sizeDelta = Vector2.zero;

            Image backgroundOutlineImage = backgroundOutline.AddComponent<Image>();
            backgroundOutlineImage.sprite = windowInfo.backgroundOutlineSprite;
            backgroundOutlineImage.color = windowInfo.inventoryBackgroundColor;
            backgroundOutlineImage.type = Image.Type.Sliced;
            backgroundOutlineImage.raycastTarget = false;
        }

        // Header
        if (enableHeader)
        {
            var header = new GameObject("Header");
            header.transform.parent = visual.transform;
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
        viewport.transform.parent = visual.transform;

        RectTransform rectViewport = viewport.AddComponent<RectTransform>();
        rectViewport.anchorMin = new Vector2(0, 1);
        rectViewport.anchorMax = new Vector2(0, 1);
        rectViewport.pivot = new Vector2(0, 1);
        rectViewport.sizeDelta = new Vector2(windowInfo.sizeX * windowInfo.cellSize.x, windowInfo.sizeY * windowInfo.cellSize.y);
        if (enabledVerticalScrollbar)
            rectViewport.sizeDelta = new Vector2(sizeOfViewport.x * windowInfo.cellSize.x, sizeOfViewport.y * windowInfo.cellSize.y);
        rectViewport.anchoredPosition = new Vector2(0, 0);

        viewport.AddComponent<Mask>();

        Image ViewportImage = viewport.AddComponent<Image>();
        ViewportImage.sprite = viewportSprite;
        ViewportImage.color = new Color(0, 0, 0, 0.15f);
        ViewportImage.type = Image.Type.Sliced;
        ViewportImage.raycastTarget = false;

        // GridImage
        var gridImage = new GameObject("GridImage");

        var gridImageRect = gridImage.AddComponent<RectTransform>();
        gridImageRect.anchorMin = new Vector2(0, 1);
        gridImageRect.anchorMax = new Vector2(0, 1);
        gridImageRect.pivot = new Vector2(0, 1);
        gridImageRect.transform.localPosition = Vector3.zero;
        gridImageRect.sizeDelta = new Vector2(windowInfo.sizeX * windowInfo.cellSize.x, windowInfo.sizeY * windowInfo.cellSize.y);


        var gridTexture = gridImage.AddComponent<RawImage>();
        gridTexture.color = windowInfo.cellColor;
        gridTexture.texture = windowInfo.cellImage;
        gridTexture.uvRect = new Rect(0, 0, windowInfo.sizeX, windowInfo.sizeY);
        gridTexture.raycastTarget = false;

        // ItemsContainer
        var ItemsContainer = new GameObject("ItemsContainer");
        ItemsContainer.transform.parent = viewport.transform;

        gridImage.transform.SetParent(ItemsContainer.transform, false);

        var ItemsContainerRect = ItemsContainer.AddComponent<RectTransform>();
        ItemsContainerRect.anchorMin = new Vector2(0, 1);
        ItemsContainerRect.anchorMax = new Vector2(0, 1);
        ItemsContainerRect.pivot = new Vector2(0, 1);
        ItemsContainerRect.transform.localPosition = Vector3.zero;
        ItemsContainerRect.sizeDelta = new Vector2(windowInfo.sizeX * windowInfo.cellSize.x, windowInfo.sizeY * windowInfo.cellSize.y); ;

        // Scrollbar if enabled
        if (enabledVerticalScrollbar)
        {
            var scroll = new GameObject("Scroll");
            scroll.transform.parent = visual.transform;
            viewport.transform.SetParent(scroll.transform, false);

            var scrollRect = scroll.AddComponent<RectTransform>();
            scrollRect.anchorMin = new Vector2(0, 0);
            scrollRect.anchorMax = new Vector2(1, 1);
            scrollRect.pivot = new Vector2(0.5f, 0.5f);
            scrollRect.transform.localPosition = Vector3.zero;
            scrollRect.sizeDelta = Vector2.zero;

            var scrollbar = scroll.AddComponent<ScrollRect>();
            scrollbar.content = ItemsContainerRect;
            scrollbar.horizontal = false;
            if (enabledVerticalScrollbar) scrollbar.vertical = true;
            scrollbar.movementType = ScrollRect.MovementType.Clamped;
            scrollbar.scrollSensitivity = 20f;
            scrollbar.viewport = rectViewport;

            //verticalScrollbar
            var scrollbarVerticalObj = new GameObject("Scrollbar Vertical");
            scrollbarVerticalObj.transform.SetParent(scroll.transform, false);
            scrollbarVerticalObj.transform.localPosition = Vector3.zero;

            var scrollbarVerticalRect = scrollbarVerticalObj.AddComponent<RectTransform>();
            scrollbarVerticalRect.anchorMin = new Vector2(1, 0);
            scrollbarVerticalRect.anchorMax = new Vector2(1, 1);
            scrollbarVerticalRect.pivot = new Vector2(0.5f, 0.5f);
            scrollbarVerticalRect.sizeDelta = new Vector2(slidebarWidth, 0);
            scrollbarVerticalRect.anchoredPosition = new Vector2(-slidebarWidth / 2, 0);

            var scrollbarVertical = scrollbarVerticalObj.AddComponent<Scrollbar>();

            //
            var scrollbarVerticalBackgroundObj = new GameObject("Background");
            scrollbarVerticalBackgroundObj.transform.parent = scrollbarVerticalObj.transform;

            var scrollbarVerticalBackgroundRect = scrollbarVerticalBackgroundObj.AddComponent<RectTransform>();
            scrollbarVerticalBackgroundRect.anchorMin = new Vector2(0, 0);
            scrollbarVerticalBackgroundRect.anchorMax = new Vector2(1, 1);
            scrollbarVerticalBackgroundRect.pivot = new Vector2(0.5f, 0.5f);
            scrollbarVerticalBackgroundRect.sizeDelta = Vector2.zero;
            scrollbarVerticalBackgroundRect.localPosition = Vector3.zero;

            var scrollbarVerticalBackgroundImage = scrollbarVerticalBackgroundObj.AddComponent<Image>();
            scrollbarVerticalBackgroundImage.sprite = scrollBackground;
            scrollbarVerticalBackgroundImage.color = Color.black;
            scrollbarVerticalBackgroundImage.type = Image.Type.Sliced;

            //
            var slidingAreaObj = new GameObject("slidingArea");
            slidingAreaObj.transform.parent = scrollbarVerticalObj.transform;

            var slidingAreaRect = slidingAreaObj.AddComponent<RectTransform>();
            slidingAreaRect.anchorMin = new Vector2(0, 0);
            slidingAreaRect.anchorMax = new Vector2(1, 1);
            slidingAreaRect.pivot = new Vector2(0.5f, 0.5f);
            slidingAreaRect.sizeDelta = Vector2.zero;
            slidingAreaRect.localPosition = Vector3.zero;

            //
            var handleObj = new GameObject("Handle");
            handleObj.transform.parent = slidingAreaObj.transform;

            var handleRect = handleObj.AddComponent<RectTransform>();
            handleRect.pivot = new Vector2(0.5f, 0.5f);
            handleRect.sizeDelta = Vector2.zero;
            handleRect.localPosition = Vector3.zero;

            var handleImage = handleObj.AddComponent<Image>();
            handleImage.sprite = handleSprite as Sprite;
            handleImage.color = Color.blue;
            handleImage.type = Image.Type.Sliced;


            scrollbarVertical.targetGraphic = handleImage;
            scrollbarVertical.handleRect = handleRect;
            scrollbarVertical.direction = Scrollbar.Direction.BottomToTop;
            scrollbarVertical.value = 1;

            scrollbar.verticalScrollbar = scrollbarVertical;

        }

        container.transform.localScale = Vector3.one;

        //
        // ItemCollection
        var itemCollection = container.AddComponent<GridInventory>();
        itemCollection.GridWidth = windowInfo.sizeX;
        itemCollection.GridHeight = windowInfo.sizeY;
        itemCollection.CellSize = windowInfo.cellSize;
        itemCollection.ContainerTransform = ItemsContainer.transform;

        // BoxCollider
        var boxCollider = visual.AddComponent<BoxCollider2D>();
        boxCollider.isTrigger = true;
        boxCollider.size = rectViewport.sizeDelta;

    }

    private bool CheckReadyToBuildInventory()
    {
        if (windowInfo.sizeX < 1 || windowInfo.sizeY < 1)
        {
            EditorUtility.DisplayDialog("Setup uncompleted", " Please check container size, value can't be less 1", "Continue");
            return false;
        }

        if (windowInfo.cellSize.x < 1 || windowInfo.cellSize.y < 1)
        {
            EditorUtility.DisplayDialog("Setup uncompleted", " Please check cell size, value can't be less 1", "Continue");
            return false;
        }

        if (windowInfo.cellImage == null)
        {
            EditorUtility.DisplayDialog("Setup uncompleted", " Please attach sprite to cell image field. Otherwise, inventory cells will be invisible", "Continue");
            return false;
        }

        return true;
    }


}