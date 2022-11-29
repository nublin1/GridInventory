using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;

namespace GridInventorySystem
{
    public class GridInventoryManager : MonoBehaviour
    {
        [SerializeField] private List<GridInventory> availableCollections;
        [SerializeField] private PointerEventData m_PointerEventData;


        // Internal variables        
        [SerializeField] private GridInventory activeItemCollection;
        private GhostItem ghostItem;
        [SerializeField] private Vector2Int stareCell;


        // prev Item Data
        [SerializeField] GridInventory lastItemCollection;
        private Vector2Int oldCell;
        [SerializeField] InventoryItem iteract_InventoryItem;
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
            ghostItem = GetComponentInChildren<GhostItem>();
            ghostItem.Collection = activeItemCollection;
            //ghostItem.gameObject.SetActive(false);
        }

        private void Update()
        {
            DefineTargetCollection();

            if (Input.GetMouseButtonDown(0))
            {
                if (activeItemCollection == null)
                    return;

                iteract_InventoryItem = activeItemCollection.TryGetInventoryItem(stareCell);

                if (iteract_InventoryItem == null)
                    return;

                lastItemCollection = activeItemCollection;
                iteract_InventoryItem.transform.SetParent(transform, false);
                oldCell = iteract_InventoryItem.GridPositionList[0];
                oldDir = iteract_InventoryItem.Dir;
            }

            if (Input.GetMouseButton(0))
            {
                ScrollActiveItemCollection();

                if (iteract_InventoryItem == null)
                    return;

                iteract_InventoryItem.transform.position = Input.mousePosition;
            }

            if (Input.GetMouseButtonUp(0))
            {
                if (iteract_InventoryItem == null)
                    return;

                if (activeItemCollection != null)
                {
                    bool isPlaced = activeItemCollection.TryPlaceItem(stareCell, iteract_InventoryItem);

                    if (isPlaced)
                        ClearIteract_InventoryItem();

                    else
                        ReturnItemToInitialPosition();
                }
                else
                {
                    ReturnItemToInitialPosition();
                }
            }

            if (Input.GetMouseButtonDown(1))
            {
                if (stareCell.Equals(new Vector2Int(-1, -1)))
                    return;

                activeItemCollection.TryUsingItem(stareCell);
            }



            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (iteract_InventoryItem == null)
                    return;
                RotateIteractItem();
            }
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

        private void ReturnItemToInitialPosition()
        {
            iteract_InventoryItem.Dir = oldDir; 
            lastItemCollection.TryPlaceItem(oldCell, iteract_InventoryItem);           

            ClearIteract_InventoryItem();
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
                        stareCell = collection.GetCellXY(Input.mousePosition);
                        ghostItem.Collection = collection;
                        ghostItem.StareCell = stareCell;
                        ghostItem.Ghost_InventoryItem = iteract_InventoryItem;
                        return;
                    }
                }                
            }

            activeItemCollection = null;
            ghostItem.Collection = null;
            stareCell = new Vector2Int(-1, -1);
            ghostItem.StareCell = stareCell;
        }

        void ScrollActiveItemCollection()
        {
            if (iteract_InventoryItem == null || activeItemCollection == null || activeItemCollection.Scrollbar == null)
                return;

            var itemBounds = iteract_InventoryItem.transform.GetComponent<BoxCollider2D>().bounds;
            var isIntersect = activeItemCollection.IsIntersectWithTheItem(itemBounds);

            if (isIntersect && activeItemCollection.Scrollbar != null)
                activeItemCollection.Scroll(itemBounds);
        }

        private void ClearIteract_InventoryItem()
        {
            iteract_InventoryItem = null;
            lastItemCollection = null;
            ghostItem.Ghost_InventoryItem = null;
            ghostItem.StareCell = new Vector2Int(-1, -1);
            //ghostItem.gameObject.SetActive(false);
        }

        private void RotateIteractItem()
        {
            iteract_InventoryItem.Dir = InventoryUtilities.GetNextDir(iteract_InventoryItem.Dir);
        }

    }
}