using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class GridInventoryEditor
{
    private InventoryEditorProperties m_EditorProperties = new InventoryEditorProperties();
    public InventoryEditorProperties EditorProperties { get => m_EditorProperties; }

    private const float LIST_RESIZE_WIDTH = 10f;

    //private bool m_StartDrag;
    private Rect m_DragRect = Rect.zero;

    protected Rect m_SidebarRect = new Rect(0, 10, 200, 500);
    protected Vector2 m_SidebarScrollPosition;

    private List<string> m_Items = new List<string>() { "General", "Background", "Header", "Scrollbar" };

    List<Rect> fields_Rects;

    int m_SelectedItemIndex;
    string selectedItem
    {
        get
        {
            if (m_SelectedItemIndex > -1 && m_SelectedItemIndex < m_Items.Count)
            {
                return m_Items[m_SelectedItemIndex];
            }
            return default;
        }
    }


    public void OnEnable()
    {

    }

    public void OnDisable()
    {

    }

    public void OnDestroy()
    {

    }

    public void OnGUI(Rect position)
    {
        DrawSidebar(new Rect(0, 0, m_SidebarRect.width, position.height));
        DrawContent(new Rect(m_SidebarRect.width, 0, 450, position.height));
        //ResizeSidebar();
    }   
    

    private void DrawSidebar(Rect position)
    {
        m_SidebarRect = position;

        GUILayout.BeginArea(m_SidebarRect, "", EditorStyles.textArea);
        GUILayout.BeginHorizontal();
        GUILayout.Space(1f);
        GUILayout.EndHorizontal();
        EditorGUILayout.Space();


        fields_Rects = new List<Rect>();
        for (int i = 0; i < m_Items.Count; i++)
        {
            var currentItem = m_Items[i];

            using (var h = new EditorGUILayout.HorizontalScope(Styles.selectButton, GUILayout.Height(25f)))
            {
                Color backgroundColor = GUI.backgroundColor;
                Color textColor = Styles.selectButtonText.normal.textColor;
                GUI.backgroundColor = Styles.normalColor;

                if (selectedItem != null && selectedItem.Equals(currentItem))
                {
                    GUI.backgroundColor = Styles.activeColor;
                    Styles.selectButtonText.normal.textColor = Color.white;
                    Styles.selectButtonText.fontStyle = FontStyle.Bold;
                }
                else if (h.rect.Contains(Event.current.mousePosition))
                {
                    GUI.backgroundColor = Styles.hoverColor;
                    Styles.selectButtonText.normal.textColor = textColor;
                    Styles.selectButtonText.fontStyle = FontStyle.Normal;
                }

                GUI.Label(h.rect, GUIContent.none, Styles.selectButton);
                Rect rect = h.rect;
                rect.width -= LIST_RESIZE_WIDTH * 0.5f;
                if (rect.Contains(Event.current.mousePosition) && Event.current.type == EventType.MouseDown && Event.current.button == 0)
                {
                    Select(currentItem);
                }

                DrawItemLabel(currentItem);
                //string error = HasConfigurationErrors(currentItem);
                //if (!string.IsNullOrEmpty(error))
                //{
                //    GUI.backgroundColor = Styles.warningColor;
                //    Rect errorRect = new Rect(h.rect.width - 20f, h.rect.y + 4.5f, 16f, 16f);
                //    GUI.Label(errorRect, new GUIContent("", error), (GUIStyle)"CN EntryWarnIconSmall");
                //}

                GUI.backgroundColor = backgroundColor;
                Styles.selectButtonText.normal.textColor = textColor;
                Styles.selectButtonText.fontStyle = FontStyle.Normal;
                fields_Rects.Add(rect);
            }
        }

        for (int j = 0; j < fields_Rects.Count; j++)
        {

            Rect rect = fields_Rects[j];
            Rect rect1 = new Rect(rect.x, rect.y, rect.width, rect.height * 0.5f);
            Rect rect2 = new Rect(rect.x, rect.y + rect.height * 0.5f, rect.width, rect.height * 0.5f);

            if (rect1.Contains(Event.current.mousePosition))
            {
                m_DragRect = rect;
                m_DragRect.y = rect.y + 10f - 25f;
                m_DragRect.x = rect.x + 5f;
                break;
            }
            else if (rect2.Contains(Event.current.mousePosition))
            {
                m_DragRect = rect;
                m_DragRect.y = rect.y + 10f;
                m_DragRect.x = rect.x + 5f;

                break;
            }
            else
            {
                m_DragRect = Rect.zero;
            }
        }

        //GUILayout.EndScrollView();
        if (GUILayout.Button("Build inventory") && CheckReadyToBuildInventory())
        {
            BuildInventory();
        }
        GUILayout.EndArea();
    }

    protected virtual void DrawContent(Rect position)
    {
        GUILayout.BeginArea(position, "", EditorStyles.helpBox);

        if (m_SelectedItemIndex == 0)
        {
            var rootTextInfo = "The object to which the inventory will be attached after creation. If empty, a new object will be created";
            //EditorGUILayout.PropertyField(rootTranform, new GUIContent("RootTranform (Optional)", rootTextInfo), true);
            EditorProperties.rootObj = (GameObject)EditorGUILayout.ObjectField(new GUIContent("RootTranform (Optional)", rootTextInfo), EditorProperties.rootObj, typeof(GameObject), true);

            EditorGUILayout.PropertyField(EditorProperties.sizeX, GUILayout.ExpandWidth(false));
            EditorGUILayout.PropertyField(EditorProperties.sizeY, GUILayout.ExpandWidth(false));
            EditorGUILayout.PropertyField(EditorProperties.cellSize, GUILayout.MaxWidth(200));
            EditorGUILayout.PropertyField(EditorProperties.cellColor);
            EditorGUILayout.PropertyField(EditorProperties.cellImage, false);
        }
        else if (m_SelectedItemIndex == 1)
        {
            EditorGUILayout.PropertyField(EditorProperties.inventoryBackgroundColor);
            EditorGUILayout.PropertyField(EditorProperties.inventoryBackground, false);

            EditorGUILayout.PropertyField(EditorProperties.enableBackgroundOutline);
            if (EditorProperties.enableBackgroundOutline.boolValue)
            {
                EditorGUILayout.PropertyField(EditorProperties.backgroundOutlineColor, false);
                EditorGUILayout.PropertyField(EditorProperties.backgroundOutlineSprite, false);
            }
        }
        else if (m_SelectedItemIndex == 2)
        {
            EditorGUILayout.PropertyField(EditorProperties.enableHeader);
            if (EditorProperties.enableHeader.boolValue)
            {
                EditorGUILayout.PropertyField(EditorProperties.HeaderHeight);
                EditorGUILayout.PropertyField(EditorProperties.HeaderColor);
                EditorGUILayout.PropertyField(EditorProperties.headerBackGroundSprite);
                EditorGUILayout.PropertyField(EditorProperties.headerTitle_Text);
                EditorGUILayout.PropertyField(EditorProperties.draggableHeader);
            }
        }
        else if (m_SelectedItemIndex == 3)
        {
            EditorGUILayout.PropertyField(EditorProperties.enabledVerticalScrollbar);
            if (EditorProperties.enabledVerticalScrollbar.boolValue)
            {
                EditorGUILayout.PropertyField(EditorProperties.sizeOfViewport);
                EditorGUILayout.PropertyField(EditorProperties.scrollBackground);
                EditorGUILayout.PropertyField(EditorProperties.handleSprite);
                EditorGUILayout.PropertyField(EditorProperties.slidebarWidth);
            }
        }

        GUILayout.EndArea();
    }

    private void ResizeSidebar()
    {
        Rect rect = new Rect(m_SidebarRect.width - LIST_RESIZE_WIDTH * 0.5f, m_SidebarRect.y, LIST_RESIZE_WIDTH, m_SidebarRect.height);
        EditorGUIUtility.AddCursorRect(rect, MouseCursor.ResizeHorizontal);
        int controlID = GUIUtility.GetControlID(FocusType.Passive);
    }

    void DrawItemLabel(string currentItem)
    {
        GUILayout.BeginHorizontal();
        //Color color = Color.green;
        //GUI.backgroundColor = color;
        GUILayout.Label(currentItem, Styles.selectButtonText);
        GUILayout.EndHorizontal();
    }

    void Select(string item)
    {
        int index = m_Items.IndexOf(item);
        if (this.m_SelectedItemIndex != index)
        {
            this.m_SelectedItemIndex = index;
            //this.m_ScrollPosition.y = 0f;
        }
    }

    private bool CheckReadyToBuildInventory()
    {
        if (EditorProperties.sizeX.intValue < 1 || EditorProperties.sizeY.intValue < 1)
        {
            EditorUtility.DisplayDialog("Setup uncompleted", " Please check container size, value can't be less 1", "Continue");
            return false;
        }

        if (EditorProperties.cellSize.vector2IntValue.x < 1 || EditorProperties.cellSize.vector2IntValue.y < 1)
        {
            EditorUtility.DisplayDialog("Setup uncompleted", " Please check cell size, value can't be less 1", "Continue");
            return false;
        }

        if (EditorProperties.cellImage.objectReferenceValue as Texture == null)
        {
            EditorUtility.DisplayDialog("Setup uncompleted", " Please attach sprite to cell image field. Otherwise, inventory cells will be invisible", "Continue");
            return false;
        }

        return true;
    }

    private void BuildInventory()
    {
        GameObject obj;

        if (EditorProperties.rootObj == null)
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
            obj = EditorProperties.rootObj;
        }

        //
        var container = new GameObject("container");
        container.transform.parent = obj.transform;
        container.transform.localPosition = Vector3.zero;


        var containerRect = container.AddComponent<RectTransform>();
        var _cellSize = EditorProperties.cellSize.vector2IntValue;
        var _sizeOfViewport = EditorProperties.sizeOfViewport.vector2IntValue;
        containerRect.sizeDelta = new Vector2(_cellSize.x * EditorProperties.sizeX.intValue, _cellSize.y * EditorProperties.sizeY.intValue);
        if (EditorProperties.enabledVerticalScrollbar.boolValue == true)
            containerRect.sizeDelta = new Vector2(_cellSize.x * _sizeOfViewport.x + EditorProperties.slidebarWidth.floatValue, _cellSize.y * _sizeOfViewport.y);

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
        backgroundImage.sprite = (Sprite)EditorProperties.inventoryBackground.objectReferenceValue;
        backgroundImage.color = EditorProperties.inventoryBackgroundColor.colorValue;
        backgroundImage.type = Image.Type.Sliced;
        backgroundImage.raycastTarget = false;

        // Background Outline
        if (EditorProperties.enableBackgroundOutline.boolValue)
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
            backgroundOutlineImage.sprite = EditorProperties.backgroundOutlineSprite.objectReferenceValue as Sprite;
            backgroundOutlineImage.color = EditorProperties.backgroundOutlineColor.colorValue;
            backgroundOutlineImage.type = Image.Type.Sliced;
            backgroundOutlineImage.raycastTarget = false;
        }

        // Header
        if (EditorProperties.enableHeader.boolValue)
        {
            var header = new GameObject("Header");
            header.transform.parent = visual.transform;
            header.transform.localPosition = Vector3.zero;

            RectTransform rectHeader = header.AddComponent<RectTransform>();
            rectHeader.anchorMin = new Vector2(0, 1);
            rectHeader.anchorMax = new Vector2(1, 1);
            rectHeader.pivot = new Vector2(0.5f, 0f);
            rectHeader.sizeDelta = new Vector2(0, EditorProperties.HeaderHeight.floatValue);

            // HeaderBackground
            var headerBackground = new GameObject("headerBackground");
            headerBackground.transform.parent = header.transform;
            headerBackground.transform.localPosition = Vector3.zero;

            RectTransform headerBackgroundRect = headerBackground.AddComponent<RectTransform>();
            headerBackgroundRect.anchorMin = new Vector2(0, 1);
            headerBackgroundRect.anchorMax = new Vector2(1, 1);
            headerBackgroundRect.pivot = new Vector2(0f, 1f);
            headerBackgroundRect.sizeDelta = new Vector2(0, EditorProperties.HeaderHeight.floatValue);

            Image headerBackgroundImage = headerBackground.AddComponent<Image>();
            headerBackgroundImage.sprite = EditorProperties.headerBackGroundSprite.objectReferenceValue as Sprite;
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
            textMeshTitle.text = EditorProperties.headerTitle_Text.stringValue;
            textMeshTitle.fontStyle = (FontStyles)FontStyle.Bold;
            textMeshTitle.fontSizeMin = 5;
            textMeshTitle.fontSizeMax = 72;
            //textMeshTitle.alignment = (TextAlignmentOptions)TextAnchor.MiddleLeft;
            textMeshTitle.enableAutoSizing = true;

            if (EditorProperties.draggableHeader.boolValue)
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
        rectViewport.sizeDelta = new Vector2(EditorProperties.sizeX.intValue * _cellSize.x, EditorProperties.sizeY.intValue * _cellSize.y);
        if (EditorProperties.enabledVerticalScrollbar.boolValue)
            rectViewport.sizeDelta = new Vector2(_sizeOfViewport.x * _cellSize.x, _sizeOfViewport.y * _cellSize.y);
        rectViewport.anchoredPosition = new Vector2(0, 0);

        viewport.AddComponent<Mask>();

        Image ViewportImage = viewport.AddComponent<Image>();
        ViewportImage.sprite = EditorProperties.viewportSprite.objectReferenceValue as Sprite;
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
        gridImageRect.sizeDelta = new Vector2(EditorProperties.sizeX.intValue * _cellSize.x, EditorProperties.sizeY.intValue * _cellSize.y);


        var gridTexture = gridImage.AddComponent<RawImage>();
        gridTexture.color = EditorProperties.cellColor.colorValue;
        gridTexture.texture = EditorProperties.cellImage.objectReferenceValue as Texture;
        gridTexture.uvRect = new Rect(0, 0, EditorProperties.sizeX.intValue, EditorProperties.sizeY.intValue);
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
        ItemsContainerRect.sizeDelta = new Vector2(EditorProperties.sizeX.intValue * _cellSize.x, EditorProperties.sizeY.intValue * _cellSize.y);

        // Scrollbar if enabled
        if (EditorProperties.enabledVerticalScrollbar.boolValue)
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
            if (EditorProperties.enabledVerticalScrollbar.boolValue) scrollbar.vertical = true;
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
            scrollbarVerticalRect.sizeDelta = new Vector2(EditorProperties.slidebarWidth.floatValue, 0);
            scrollbarVerticalRect.anchoredPosition = new Vector2(-EditorProperties.slidebarWidth.floatValue / 2, 0);

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
            scrollbarVerticalBackgroundImage.sprite = (Sprite)EditorProperties.scrollBackground.objectReferenceValue;
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
            handleImage.sprite = EditorProperties.handleSprite.objectReferenceValue as Sprite;
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
        itemCollection.GridWidth = EditorProperties.sizeX.intValue;
        itemCollection.GridHeight = EditorProperties.sizeY.intValue;
        itemCollection.CellSize = _cellSize;
        itemCollection.ContainerTransform = ItemsContainer.transform;

        // BoxCollider
        var boxCollider = visual.AddComponent<BoxCollider2D>();
        boxCollider.isTrigger = true;
        boxCollider.size = rectViewport.sizeDelta;

    }
}