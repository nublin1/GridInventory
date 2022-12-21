using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GridInventorySystem;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(BaseItemPickerAttribute))]
public class ItemPickerDrawer : PickerDrawer<BaseItem>
{
    protected override List<BaseItem> GetItems(ItemDatabase database)
    {
        System.Type type = fieldInfo.FieldType;
        if (typeof(IList).IsAssignableFrom(fieldInfo.FieldType))
        {
            type = Utilities.GetElementType(fieldInfo.FieldType);
        }
        return database.allItems.Where(x => type.IsAssignableFrom(x.GetType())).ToList();
    }
}
