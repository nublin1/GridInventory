using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New ItemDatabase", menuName = "Items/ItemDatabase", order = 51)]
[System.Serializable]
public class ItemDatabase : ScriptableObject
{    
    public List<BaseItem> items = new List<BaseItem>();


	public List<BaseItem> allItems
	{
		get
		{
			List<BaseItem> all = new List<BaseItem>(items);			
			return all;
		}
	}
}
