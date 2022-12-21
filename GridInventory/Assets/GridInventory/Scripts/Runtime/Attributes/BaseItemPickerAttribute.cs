using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseItemPickerAttribute : PickerAttribute
{
	public BaseItemPickerAttribute() : this(false) { }

	public BaseItemPickerAttribute(bool utility) : base(utility) { }
}
