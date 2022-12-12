using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GridInventoryEditor : EditorWindow
{
    private SerializedObject windowInfoSO;
    private static GridInventoryEditorData windowInfo;
    private GridInventoryInspector m_gridInventoryInspector;
    

    [MenuItem("Tools/TestEditor", false, 0)]
    public static void ShowWindow()
    {
        GridInventoryEditor[] objArray = Resources.FindObjectsOfTypeAll<GridInventoryEditor>();
        GridInventoryEditor editor = (objArray.Length <= 0 ? ScriptableObject.CreateInstance<GridInventoryEditor>() : objArray[0]);

        editor.minSize = new Vector2(690, 300);
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
            AssetDatabase.Refresh();
        }
    }

    private void OnEnable()
    {
        m_gridInventoryInspector = new GridInventoryInspector();
    }

    private void OnDisable()
    {
        this.m_gridInventoryInspector.OnDisable();
    }

    private void OnDestroy()
    {
        this.m_gridInventoryInspector.OnDestroy();
    }

    private void Update()
    {
        if (EditorWindow.mouseOverWindow == this)
            Repaint();
    }

    private void CreateGUI()
    {
        windowInfoSO = new SerializedObject(windowInfo);



        LoadDefaultResources();
    }

    private void LoadDefaultResources()
    {

    }

    private void OnGUI()
    {
        this.m_gridInventoryInspector.OnGUI(position);
    }
}
