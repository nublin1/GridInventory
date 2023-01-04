using UnityEditor;
using UnityEngine;


public class CategoryPickerAttribute : PickerAttribute
{
    public CategoryPickerAttribute() : this(false) { }
    
    public CategoryPickerAttribute(bool utility) : base(utility) { }
}
