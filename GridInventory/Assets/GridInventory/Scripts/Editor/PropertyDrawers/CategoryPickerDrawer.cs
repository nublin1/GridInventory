using GridInventorySystem;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(CategoryPickerAttribute))]
public class CategoryPickerDrawer : PickerDrawer<Category>
{
    protected override List<Category> GetItems(ItemDatabase database)
    {
        System.Type type = fieldInfo.FieldType;
        if (typeof(IList).IsAssignableFrom(fieldInfo.FieldType))
        {
            type = Utilities.GetElementType(fieldInfo.FieldType);
        }
        return database.allCategories.Where(x => type.IsAssignableFrom(x.GetType())).ToList();
    }
}
