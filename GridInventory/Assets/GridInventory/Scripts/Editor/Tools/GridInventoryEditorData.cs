using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}
