using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

namespace GridInventorySystem
{
    public class ItemCollection : MonoBehaviour, IEnumerable<BaseItem>
    {
        [BaseItemPicker (true)]
        [SerializeField]
        List<BaseItem> m_Items = new();

        [SerializeField]
        protected List<int> m_Amounts = new();
        

        public List<BaseItem> Items { get => m_Items; }

        private void Awake()
        {
            Initialize();
        }

        public void Initialize()
        {
            m_Amounts.Clear();  
            m_Items = CreateInstances(m_Items.ToArray()).ToList();

            for (int i = 0; i < this.m_Items.Count; i++)
            {
                this.m_Amounts.Add(m_Items[i].Stack);
            }
        }               


        public void Add(BaseItem item)
        {
            this.m_Items.Add(item);
            int index = m_Items.IndexOf(item);
            
            this.m_Amounts.Insert(index, item.Stack);            
            //if (onChange != null)
            //    onChange.Invoke();

        }

        public bool Remove(BaseItem item)
        {
            int index = m_Items.IndexOf(item);
            bool result = m_Items.Remove(item);
            if (result)
            {
                this.m_Amounts.RemoveAt(index);                
                //if (onChange != null)
                //    onChange.Invoke();
            }
            return result;
        }

        public void UpdateAmounts()
        {
            for (int i = 0; i < m_Amounts.Count; i++)
            {
                m_Amounts[i] = m_Items[i].Stack;
            }
        }

        public static BaseItem[] CreateInstances(BaseItem[] items)
        {
            BaseItem[] instances = new BaseItem[items.Length];

            for (int i = 0; i < items.Length; i++)
            {
                BaseItem item = items[i];
                item = Instantiate(item) as BaseItem;

                instances[i] = item;
            }

            return instances;
        }

        public IEnumerator<BaseItem> GetEnumerator()
        {
            return this.m_Items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}