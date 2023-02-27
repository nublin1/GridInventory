using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;
using NaughtyAttributes;

namespace GridInventorySystem
{
    public class GridInventoryManager : MonoBehaviour
    {   
        [SerializeField] private List<GridInventory> availableCollections;
        private PointerEventData m_PointerEventData;


        // Internal variables        
        private GridInventory activeItemCollection;
        BaseItem iteract_InventoryItem;
        private Condition ghostItem;


        // prev Item Data
        GridInventory savedItemCollection;
        private BaseItem savedItem;
        private Vector2Int oldCell;
        Dir oldDir;


        #region Events
        public delegate void AAddCollection(GridInventory itemsCollection);
        public static event AAddCollection OnAddCollection;
        #endregion

        private void Awake()
        {
            var itemsCollection = GetComponentsInChildren<GridInventory>();
            foreach (var collection in itemsCollection)
            {
                var isExist = availableCollections.Any(x => collection == x);
                if (!isExist)
                    availableCollections.Add(collection);
                if (collection.InventorySystem == null)
                    availableCollections[^1].InventorySystem = this;               
            }

            activeItemCollection = availableCollections[0];
        }

        private void Start()
        {
            ghostItem = GetComponentInChildren<Condition>();
            ghostItem.Collection = activeItemCollection;
           
        }

        private void Update()
        {
            DefineTargetCollection();

            if (Input.GetMouseButtonDown(0))
            {
                if (activeItemCollection == null)
                    return;

                var activeCell = activeItemCollection.GetCellXY(Input.mousePosition);
                iteract_InventoryItem = activeItemCollection.GetInventoryItem(activeCell);
                if (iteract_InventoryItem == null)
                    return;

                activeItemCollection.ItemCollection.RemoveItem(iteract_InventoryItem);               

                savedItemCollection = activeItemCollection;
                savedItem = iteract_InventoryItem;
                iteract_InventoryItem.ItemTransform.SetParent(transform, false);
                oldCell = savedItem.GridPositionList[0];
                oldDir = iteract_InventoryItem.Dir;

                iteract_InventoryItem.BackgroundImage.enabled = false;
                iteract_InventoryItem.BackgroundOutlineImage.enabled = false;
                iteract_InventoryItem.HighlightImage.enabled = false;
                iteract_InventoryItem.ItemNameText.enabled = false;
                iteract_InventoryItem.ItemCountText.enabled = false;                
            }

            if (Input.GetMouseButton(0))
            {
                if (iteract_InventoryItem == null)
                    return;

                iteract_InventoryItem.ItemTransform.position = Input.mousePosition;
                ScrollActiveItemCollection();
            }

            if (Input.GetMouseButtonUp(0))
            {

                if (iteract_InventoryItem == null)
                    return;

                if (activeItemCollection == null)
                {
                    ReturnItemToInitialPosition();
                    return;
                }

                var cellXY = activeItemCollection.GetCellXY(Input.mousePosition);
                bool canPlace = activeItemCollection.CanAddItem(iteract_InventoryItem, cellXY);
                if (canPlace == false)
                {
                    ReturnItemToInitialPosition();
                    return;
                }

                iteract_InventoryItem.BackgroundImage.enabled = true;
                iteract_InventoryItem.BackgroundOutlineImage.enabled = true;
                iteract_InventoryItem.HighlightImage.enabled = true;
                iteract_InventoryItem.ItemNameText.enabled = true;
                iteract_InventoryItem.ItemCountText.enabled = true;


                activeItemCollection.ItemCollection.AddItem(iteract_InventoryItem, cellXY);
                ClearIteract_InventoryItem();                      
            }   

            if (Input.GetMouseButtonDown(1))
            {
                if (activeItemCollection == null)
                    return;

                activeItemCollection.TryUsingItem(activeItemCollection.GetCellXY(Input.mousePosition));
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (iteract_InventoryItem == null)
                    return;
                RotateIteractItem();
            }
        }

        private void ReturnItemToInitialPosition()
        {
            savedItem.SetRotation(oldDir);
            savedItem.BackgroundImage.enabled = true;
            savedItem.BackgroundOutlineImage.enabled = true;
            savedItem.HighlightImage.enabled = true;
            savedItem.ItemNameText.enabled = true;
            savedItem.ItemCountText.enabled = true;
            savedItemCollection.ItemCollection.AddItem(savedItem, oldCell);
            
            ClearIteract_InventoryItem();
        }

        public void AddItemContainer(GridInventory itemsCollection)
        {
            if (availableCollections.Contains(itemsCollection))
            {
                return;
            }

            itemsCollection.InventorySystem = this;
            availableCollections.Add(itemsCollection);
            OnAddCollection?.Invoke(itemsCollection);
        }

        public void RemoveItemContainer(GridInventory itemsCollection)
        {
            if (availableCollections.Contains(itemsCollection))
                availableCollections.Remove(itemsCollection);
        }

        private void ClearIteract_InventoryItem()
        {
            iteract_InventoryItem = null;
            savedItemCollection = null;
            savedItem = null;
            ghostItem.Ghost_InventoryItem = null;

        }

        private void DefineTargetCollection()
        {
            foreach (var collection in availableCollections)
            {
                if (transform.GetChild(0).gameObject.activeSelf == true)
                {
                    var boundsCollection = collection.transform.GetChild(0).transform.GetComponentInChildren<Collider2D>().bounds;
                    if (boundsCollection.Contains(Input.mousePosition))
                    {
                        activeItemCollection = collection;
                        ghostItem.Collection = collection;
                        ghostItem.Ghost_InventoryItem = iteract_InventoryItem;
                        return;
                    }
                }
            }

            activeItemCollection = null;
            ghostItem.Collection = null;
        }

        void ScrollActiveItemCollection()
        {
            if (iteract_InventoryItem == null || activeItemCollection == null || activeItemCollection.Scrollbar == null)
                return;

            var itemBounds = iteract_InventoryItem.ItemTransform.GetComponent<BoxCollider2D>().bounds;
            var isIntersect = activeItemCollection.IsIntersectWithTheItem(itemBounds);

            if (isIntersect && activeItemCollection.Scrollbar != null)
                activeItemCollection.Scroll(itemBounds);
        }

        private void RotateIteractItem()
        {
            iteract_InventoryItem.SetRotation(Utilities.GetNextDir(iteract_InventoryItem.Dir));
        }



    }
}