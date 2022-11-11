using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GridInventory
{
    public class InventoryUtilities
    {
        public static Vector2Int CalculateInventorySlotCoordinate(RectTransform rectTransform, Vector2 mousePosition, ItemsCollection grid)
        {
            grid.GetCellXY(rectTransform, mousePosition, out int x, out int y);
            return new Vector2Int(x, y);
        }

        public static Dir GetNextDir(Dir dir)
        {
            switch (dir)
            {
                case Dir.Up:        return Dir.Right;
                case Dir.Right:     return Dir.Up;

                //case Dir.Up:  return Dir.Right;
                //case Dir.Right:  return Dir.Down;
                //case Dir.Down:    return Dir.Left;
                //case Dir.Left: return Dir.Up;

                default: return Dir.Up;
            }
        }

        public static int GetRotationAngle(Dir dir)
        {
            switch (dir)
            {
                default:
                case Dir.Up: return 0;
                case Dir.Left: return 90;
                case Dir.Down: return 180;
                case Dir.Right: return 270;
            }
        }

        
    }

    public enum Dir
    {
        Up,
        Left,
        Right,
        Down,
    }
}