
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GridInventorySystem
{
    public class ItemCollection : MonoBehaviour, IEnumerable<InventoryItemData>
    {
        [SerializeField]
        List<BaseItem> m_items = new();

        [SerializeField]
        protected List<int> m_Amounts = new();


        public List<BaseItem> Items { get => m_items; }

        private void Awake()
        {
            Initialize();
        }

        public void Initialize()
        {
            if (this.m_Amounts.Count < this.m_items.Count)
            {
                for (int i = this.m_Amounts.Count; i < this.m_items.Count; i++)
                {
                    this.m_Amounts.Add(1);
                }
            }
        }


        public void Add(BaseItem item)
        {
            this.m_items.Add(item);
            int index = m_items.IndexOf(item);
            
            this.m_Amounts.Insert(index, item.Stack);            
            //if (onChange != null)
            //    onChange.Invoke();

        }

        public bool Remove(BaseItem item)
        {
            int index = m_items.IndexOf(item);
            bool result = m_items.Remove(item);
            if (result)
            {
                this.m_Amounts.RemoveAt(index);                
                //if (onChange != null)
                //    onChange.Invoke();
            }
            return result;
        }

        public IEnumerator<BaseItem> GetEnumerator()
        {
            return this.m_items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}