using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GridInventorySystem
{
    public static class Utilities
    {        
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

        public static IEnumerable<SerializedProperty> EnumerateChildProperties(SerializedProperty property)
        {
            var iterator = property.Copy();
            var end = iterator.GetEndProperty();
            if (iterator.NextVisible(enterChildren: true))
            {
                do
                {
                    if (SerializedProperty.EqualContents(iterator, end))
                        yield break;

                    yield return iterator;
                }
                while (iterator.NextVisible(enterChildren: false));
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