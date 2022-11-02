using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Inventory
{
    public class InventoryUtilities
    {
        public static Vector2Int CalculateInventorySlotCoordinate(Vector2 mousePosition, GridXY_v2 grid)
        {
            grid.GetXY(mousePosition, out int x, out int y);
            return new Vector2Int(x, y);
        }
    }
}