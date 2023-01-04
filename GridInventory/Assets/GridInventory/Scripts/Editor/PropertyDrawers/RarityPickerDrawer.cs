using GridInventorySystem;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;


namespace Assets.GridInventory.Scripts.Editor.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(RarityPickerAttribute))]
    public class RarityPickerDrawer : PickerDrawer<Rarity>
    {
        protected override List<Rarity> GetItems(ItemDatabase database)
        {
            System.Type type = fieldInfo.FieldType;
            if (typeof(IList).IsAssignableFrom(fieldInfo.FieldType))
            {
                type = Utilities.GetElementType(fieldInfo.FieldType);
            }
            return database.allRarities.Where(x => type.IsAssignableFrom(x.GetType())).ToList();
        }
    }
}