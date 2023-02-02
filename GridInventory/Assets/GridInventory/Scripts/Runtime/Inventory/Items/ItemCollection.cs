using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Unity.VisualScripting;
using UnityEngine;

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

        #region
        public delegate void AAddItem(BaseItem item, Vector2Int _cellXY);
        public event AAddItem OnAddItem;
        public delegate void ARemoveItem(BaseItem item);
        public event ARemoveItem OnRemoveItem;
        #endregion

        private void Awake()
        {

        }       


        public void AddItems(List<BaseItem> baseItems)
        {
            foreach (var _item in baseItems)
            {
                this.m_Items.Add(_item);
                int index = m_Items.IndexOf(_item);
                this.m_Amounts.Insert(index, _item.Stack);

                if (OnAddItem != null)
                    OnAddItem.Invoke(_item, new Vector2Int(-1, -1));
            }
        }

        public void AddItem(BaseItem item, Vector2Int _cellXY)
        {
            m_Items.Add(item);
            int index = m_Items.IndexOf(item);

            this.m_Amounts.Insert(index, item.Stack);
            if (OnAddItem != null)
                OnAddItem.Invoke(item, _cellXY);
        }

        public bool RemoveItem(BaseItem item)
        {
            int index = m_Items.IndexOf(item);
            bool result = m_Items.Remove(item);
            if (result)
            {
                this.m_Amounts.RemoveAt(index);
                if (OnRemoveItem != null)
                    OnRemoveItem.Invoke(item);
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
            List<BaseItem> itemsToAdd = new List<BaseItem>();
            GridInventory inventory = null;
            if (TryGetComponent(out GridInventory _inventory))
            {
                GetComponent<GridInventory>().InitInventory();
                inventory = _inventory;
            }

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
                            Dir dir = (Dir)itemData["Dir"].ConvertTo<int>();                           
                            var pos_str = itemData["Position"];
                            var posData = JsonConvert.DeserializeObject<Dictionary< string, object>> (pos_str.ToString());

                            Vector2Int pos = new Vector2Int(posData["x"].ConvertTo<int>(), posData["y"].ConvertTo<int>());
                            
                            if (inventory == null)
                            {
                                itemsToAdd.Add(newitem);
                                continue;
                            }

                            newitem = transform.GetComponent<GridInventory>().InitItem(newitem, dir);
                            if (pos == new Vector2Int(-1, -1))
                            {
                                itemsToAdd.Add(newitem);
                                continue;
                            }
                            
                            var canPlace = inventory.CanAddItem(newitem, pos);
                            if (canPlace)
                                newitem.ReculculatePositionList(pos);
                            
                            AddItem(newitem, pos);
                        }
                    }
                }
            }           

            AddItems(itemsToAdd);

        }

        public void SaveData(ref Dictionary<string, object> data)
        {
            if (m_Items.Count == 0 || m_saveable == false)
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