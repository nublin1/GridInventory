using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using static UnityEditor.Progress;
using Newtonsoft.Json;
using Unity.VisualScripting;
using UnityEngine.UIElements;

namespace GridInventorySystem
{
    public class ItemCollection : MonoBehaviour, IEnumerable<BaseItem>, IDataPersistence
    {       
        [BaseItemPicker(true)]
        [SerializeField]
        List<BaseItem> m_Items = new();

        [SerializeField]
        protected List<int> m_Amounts = new();

        public List<BaseItem> Items { get => m_Items; }

        [SerializeField]
        private bool m_saveable = false;

        private void Awake()
        {           
           
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
                var item = items[i];
                item = Instantiate(item);
                item.Id = items[i].Id;

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

        public void LoadData(Dictionary<string, object> data)
        {            

            if (!data.ContainsKey(transform.name))
            {
                return;
            }

            List<object> loaded_Items = ((IEnumerable)data[transform.name]).Cast<object>()
                .Select(x => x == null ? x : x.ToString())
                .ToList();

            var databases = Utilities.GetAllDatabases();
            List<BaseItem> itemsToAdd= new List<BaseItem>();

            foreach (object item in loaded_Items)
            {
                Dictionary<string, object> itemData = JsonConvert.DeserializeObject<Dictionary<string, object>>(item.ToString());

                if (itemData == null)
                {
                    continue;
                }

                foreach (var database in databases)
                {
                    var dbItems = database.allItems;

                    foreach (var dbItem in dbItems)
                    {
                        if (itemData["ID"] != null && itemData["ID"].Equals(dbItem.Id))
                        {
                            BaseItem newitem = dbItem;
                            newitem = Instantiate(dbItem) as BaseItem;
                            newitem.Stack = itemData["Stack"].ConvertTo<int>();
                            newitem.Init((Dir)itemData["Dir"].ConvertTo<int>());
                            

                            itemsToAdd.Add(newitem);
                            break;
                        }
                    }
                }
            }

            itemsToAdd = transform.GetComponent<GridInventory>().InitItems(itemsToAdd);            
            transform.GetComponent<GridInventory>().AddItems(itemsToAdd);

            //m_Items = III; 
            // Initialize();

        }

        public void SaveData(ref Dictionary<string, object> data)
        {
            if (m_Items.Count == 0 || m_saveable== false)
            {
                return;
            }

            List<object> mItems = new List<object>();
            for (int i = 0; i < m_Items.Count; i++)
            {
                BaseItem item = m_Items[i];
                if (item != null)
                {
                    Dictionary<string, object> itemData = new Dictionary<string, object>();
                    item.SaveData(ref itemData);
                    mItems.Add(itemData);
                }
                else
                {
                    mItems.Add(null);
                }
            }

            data.Add(transform.name, mItems);

        }
    }
}