using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GridInventoryInspector : EditorWindow
{
    private SerializedObject windowInfoSO;
    private static GridInventoryEditorData windowInfo;
    private GridInventoryEditor m_gridInventoryInspector;

    //Inventory
    [MenuItem("Tools/Create inventory", false, 0)]
    public static void ShowWindow()
    {
        GridInventoryInspector[] objArray = Resources.FindObjectsOfTypeAll<GridInventoryInspector>();
        GridInventoryInspector editor = (objArray.Length <= 0 ? ScriptableObject.CreateInstance<GridInventoryInspector>() : objArray[0]);

        editor.minSize = new Vector2(600, 300);
        editor.titleContent = new GUIContent("Inventory System");

        editor.Show();
    }

    [InitializeOnLoadMethod]
    private static void OnLoad()
    {
        windowInfo = (GridInventoryEditorData)AssetDatabase.LoadAssetAtPath("Assets/WindowInfo.asset", typeof(GridInventoryEditorData));
        if (!windowInfo)
        {
            windowInfo = CreateInstance<GridInventoryEditorData>();
            AssetDatabase.CreateAsset(windowInfo, "Assets/WindowInfo.asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
           
        }
    }

    private void OnEnable()
    {
        m_gridInventoryInspector = new GridInventoryEditor();
    }

    private void OnDisable()
    {
        this.m_gridInventoryInspector.OnDisable();
    }

    private void OnDestroy()
    {
        this.m_gridInventoryInspector.OnDestroy();
        AssetDatabase.SaveAssets();
    }

    private void Update()
    {
        if (EditorWindow.mouseOverWindow == this)
            Repaint();
    }

    private void CreateGUI()
    {     
        windowInfoSO = new SerializedObject(windowInfo);

        m_gridInventoryInspector.EditorProperties.sizeX = windowInfoSO.FindProperty("sizeX");
        m_gridInventoryInspector.EditorProperties.sizeY = windowInfoSO.FindProperty("sizeY");
        m_gridInventoryInspector.EditorProperties.cellSize = windowInfoSO.FindProperty("cellSize");

        m_gridInventoryInspector.EditorProperties.cellColor = windowInfoSO.FindProperty("cellColor");
        m_gridInventoryInspector.EditorProperties.cellImage = windowInfoSO.FindProperty("cellImage");

        m_gridInventoryInspector.EditorProperties.inventoryBackgroundColor = windowInfoSO.FindProperty("inventoryBackgroundColor");
        m_gridInventoryInspector.EditorProperties.inventoryBackground = windowInfoSO.FindProperty("inventoryBackground");

        m_gridInventoryInspector.EditorProperties.enableBackgroundOutline = windowInfoSO.FindProperty("enableBackgroundOutline");
        m_gridInventoryInspector.EditorProperties.backgroundOutlineColor = windowInfoSO.FindProperty("backgroundOutlineColor");
        m_gridInventoryInspector.EditorProperties.backgroundOutlineSprite = windowInfoSO.FindProperty("backgroundOutlineSprite");

        m_gridInventoryInspector.EditorProperties.enableHeader = windowInfoSO.FindProperty("enableHeader");
        m_gridInventoryInspector.EditorProperties.HeaderHeight = windowInfoSO.FindProperty("HeaderHeight");
        m_gridInventoryInspector.EditorProperties.HeaderColor = windowInfoSO.FindProperty("HeaderColor");
        m_gridInventoryInspector.EditorProperties.headerBackGroundSprite = windowInfoSO.FindProperty("headerBackGroundSprite");
        m_gridInventoryInspector.EditorProperties.headerTitle_Text = windowInfoSO.FindProperty("headerTitle_Text");
        m_gridInventoryInspector.EditorProperties.draggableHeader = windowInfoSO.FindProperty("draggableHeader");

        m_gridInventoryInspector.EditorProperties.viewportSprite = windowInfoSO.FindProperty("viewportSprite");

        m_gridInventoryInspector.EditorProperties.enabledVerticalScrollbar = windowInfoSO.FindProperty("enabledVerticalScrollbar");
        m_gridInventoryInspector.EditorProperties.sizeOfViewport = windowInfoSO.FindProperty("sizeOfViewport");
        m_gridInventoryInspector.EditorProperties.scrollBackground = windowInfoSO.FindProperty("scrollBackground");
        m_gridInventoryInspector.EditorProperties.handleSprite = windowInfoSO.FindProperty("handleSprite");
        m_gridInventoryInspector.EditorProperties.slidebarWidth = windowInfoSO.FindProperty("slidebarWidth");

        LoadDefaultResources();        
    }

    private void LoadDefaultResources()
    {
        windowInfo.inventoryBackground = (Sprite)AssetDatabase.LoadAssetAtPath("Assets/GridInventory/GUI/Square.png", typeof(Sprite));
        windowInfo.backgroundOutlineSprite = (Sprite)AssetDatabase.LoadAssetAtPath("Assets/GridInventory/GUI/Square Outline.png", typeof(Sprite));
        windowInfo.headerBackGroundSprite = (Sprite)AssetDatabase.LoadAssetAtPath("Assets/GridInventory/GUI/Header.png", typeof(Sprite));
        windowInfo.viewportSprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
        windowInfo.scrollBackground = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Background.psd");
        windowInfo.handleSprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
    }

    private void OnGUI()
    {
        windowInfoSO.Update();

        this.m_gridInventoryInspector.OnGUI(position);

        windowInfoSO.ApplyModifiedProperties();
    }
}
