using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;
using TMPro;
using UnityEditor.VersionControl;
using UnityEngine.EventSystems;
using System.Linq;

namespace GridInventory
{
    public class GridInventorySystem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler
    {
        [SerializeField] private List<ItemsCollection> availableCollections;

        // Prefabs
        [Header("Inventory Prefabs")]
        [SerializeField] private bool setupPrefabs;     
        

        // Internal variables        
        [SerializeField] private ItemsCollection activeItemCollection;
        private GhostItem ghostItem;
        [SerializeField] private Vector2Int stareCell;
        private Vector2Int oldCell;

        // prev Item Data
        [SerializeField] ItemsCollection lastItemCollection;
        InventoryItem iteract_InventoryItem;
        Dir oldDir;
       

        #region Events
        public delegate void AChangeCollection();
        public static event AChangeCollection OnChangeCollection;
        #endregion


        private void Awake()
        {
            var itemsCollection = GetComponentsInChildren<ItemsCollection>();
            foreach (var collection in itemsCollection)
            {
                var isExist = availableCollections.Any(x => collection == x);
                if (!isExist)
                    availableCollections.Add(collection);
            }

            activeItemCollection = availableCollections[0];
        }

        private void Start()
        {
            ghostItem = GetComponentInChildren<GhostItem>();
            ghostItem.Collection = activeItemCollection;
            //ghostItem.RectSlots = rectSlots;
            ghostItem.gameObject.SetActive(false);
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

                iteract_InventoryItem.transform.SetParent(transform, false);
                oldCell = stareCell;
                oldDir = iteract_InventoryItem.Dir;
            }

            if (Input.GetMouseButton(0))
            {
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
                    {
                        clearIteract_InventoryItem();
                    }
                }
                else
                {
                    lastItemCollection.GenerateInventoryItem(oldCell, iteract_InventoryItem.ItemData, oldDir);
                    GameObject.Destroy(iteract_InventoryItem.gameObject);

                    clearIteract_InventoryItem();
                }
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (iteract_InventoryItem == null)
                    return;

                iteract_InventoryItem.Dir = InventoryUtilities.GetNextDir(iteract_InventoryItem.Dir);
            }
        }       
        

        private void DefineTargetCollection()
        {
            foreach (var collection in availableCollections)
            {
                var potencialCell = collection.GetCellXY(Input.mousePosition);
                bool isValidPosition = !collection.OutOfBoundsCheck(potencialCell);
                if (isValidPosition)
                {
                    activeItemCollection = collection;
                    ghostItem.Collection = collection;
                    stareCell = potencialCell;
                    return;
                }
            }

            activeItemCollection = null;
        }

        private void clearIteract_InventoryItem()
        {
            iteract_InventoryItem = null;
            lastItemCollection = null;
            ghostItem.Ghost_InventoryItem = null;
            ghostItem.gameObject.SetActive(false);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {

        }

        public void OnPointerExit(PointerEventData eventData)
        {

        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            Debug.Log("Test");
        }
    }
}