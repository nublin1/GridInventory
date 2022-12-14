using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class GridInventoryEditorData : ScriptableObject
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

    public bool enableHeader;
    public Vector2Int HeaderSize = new Vector2Int(0, 12);
    public Color HeaderColor = Color.black;
    public Sprite headerBackGroundSprite;
    public string headerTitle_Text = "";
    public bool draggableHeader = true;

    public Sprite viewportSprite;

    public bool enabledVerticalScrollbar;
    public Vector2Int sizeOfViewport;
    public Sprite scrollBackground;
    public Sprite handleSprite;    
    public float slidebarWidth = 10;
}

public class InventoryEditorProperties
{
    public GameObject rootObj;

    public SerializedProperty sizeX, sizeY;

    public SerializedProperty cellSize;
    public SerializedProperty cellColor;
    public SerializedProperty cellImage;
    
    public SerializedProperty inventoryBackgroundColor;
    public SerializedProperty inventoryBackground;
    
    public SerializedProperty enableBackgroundOutline;
    public SerializedProperty backgroundOutlineColor;
    public SerializedProperty backgroundOutlineSprite;

    public SerializedProperty enableHeader;
    public SerializedProperty HeaderSize;
    public SerializedProperty HeaderColor;
    public SerializedProperty headerBackGroundSprite;
    public SerializedProperty headerTitle_Text;
    public SerializedProperty draggableHeader;

    public SerializedProperty viewportSprite;

    public SerializedProperty enabledVerticalScrollbar;
    public SerializedProperty sizeOfViewport;
    public SerializedProperty scrollBackground;
    public SerializedProperty handleSprite;
    public SerializedProperty slidebarWidth ;
}
