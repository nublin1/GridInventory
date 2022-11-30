using UnityEngine;
using UnityEngine.UI;

namespace GridInventorySystem
{
    public class GridCell
    {
        private GridInventory gridXY;
        private int x;
        private int y;

        private BaseItem inventoryItem;        

        public GridCell(GridInventory grid, int x, int y)
        {
            this.gridXY = grid;
            this.x = x;
            this.y = y;
        }

        public BaseItem InventoryItem { get => inventoryItem; }

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

        public void SetCellData(BaseItem _inventoryItem)
        {
            inventoryItem = _inventoryItem;
        }
    }
}