
using System.Collections.Generic;
using UnityEngine;

namespace GridInventorySystem
{
    public class ItemCollection : MonoBehaviour
    {
        [SerializeField] List<InventoryItemData> items = new List<InventoryItemData>();

        public List<InventoryItemData> Items { get => items; }
    }
}