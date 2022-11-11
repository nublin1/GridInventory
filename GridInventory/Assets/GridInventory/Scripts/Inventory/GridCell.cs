using UnityEngine;
using UnityEngine.UI;

namespace GridInventory
{
    public class GridCell
    {
        private ItemsCollection gridXY;
        private int x;
        private int y;

        private InventoryItem inventoryItem;        

        public GridCell(ItemsCollection grid, int x, int y)
        {
            this.gridXY = grid;
            this.x = x;
            this.y = y;
        }

        public InventoryItem InventoryItem { get => inventoryItem; }

        public bool IsCellEmpty()
        {
            if (inventoryItem == null)
                return true;
            else
                return false;
        }

        public void ClearCellData()
        {
            inventoryItem = null;
        }

        public void SetCellData(InventoryItem _inventoryItem)
        {
            inventoryItem = _inventoryItem;
        }
    }
}