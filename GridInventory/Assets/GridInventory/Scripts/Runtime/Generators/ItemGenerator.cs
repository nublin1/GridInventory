using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GridInventorySystem;
using UnityEngine;

[RequireComponent(typeof(ItemCollection))]
public class ItemGenerator : MonoBehaviour
{
    [BaseItemPicker(true)]
    [SerializeField]
    List<BaseItem> m_Pools;

    void Start()
    {
        GenerateItems();

        if(TryGetComponent(out ItemCollection collection))
        {
            var _inst = ItemCollection.CreateInstances(m_Pools.ToArray());
            collection.AddItems(_inst.ToList());
        }
    }

  
    void Update()
    {
        
    }

    void GenerateItems()
    {

    }
}
