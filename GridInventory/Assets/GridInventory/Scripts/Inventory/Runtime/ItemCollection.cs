
using System.Collections.Generic;
using UnityEngine;

namespace GridInventorySystem
{
    public class ItemCollection : MonoBehaviour
    {
        [SerializeField]
        List<InventoryItemData> m_items = new();

        [SerializeField]
        protected List<int> m_Amounts = new();


        public List<InventoryItemData> Items { get => m_items; }


        public void Add(InventoryItemData item)
        {
            this.m_items.Add(item);
            int index = m_items.IndexOf(item);
            
            this.m_Amounts.Insert(index, item.Stack);            
            //if (onChange != null)
            //    onChange.Invoke();

        }

        public bool Remove(InventoryItemData item)
        {
            int index = m_items.IndexOf(item);
            bool result = m_items.Remove(item);
            if (result)
            {
                //this.m_Amounts.RemoveAt(index);                
                //if (onChange != null)
                //    onChange.Invoke();
            }
            return result;
        }
    }
}