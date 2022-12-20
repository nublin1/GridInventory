using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New ItemDatabase", menuName = "Items/ItemDatabase", order = 51)]
[System.Serializable]
public class ItemDatabase : ScriptableObject
{    
    public List<BaseItem> items = new List<BaseItem>();
}
