using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class Styles
{
	public static GUIStyle selectButton;
	public static GUIStyle selectButtonText;

	public static Color normalColor;	

	static Styles()
	{
		
		normalColor = EditorGUIUtility.isProSkin ? new Color(0.219f, 0.219f, 0.219f, 1f) : new Color(0.796f, 0.796f, 0.796f, 1f);
		

		selectButton = new GUIStyle("MeTransitionSelectHead")
		{
			alignment = TextAnchor.MiddleLeft,
			padding = new RectOffset(5, 0, 0, 0),
			overflow = new RectOffset(0, -1, 0, 0),
		};
		selectButton.normal.background = ((GUIStyle)"ColorPickerExposureSwatch").normal.background;

		selectButtonText = new GUIStyle("MeTransitionSelectHead")
		{
			alignment = TextAnchor.MiddleLeft,
			padding = new RectOffset(5, 0, 0, 0),
			overflow = new RectOffset(0, -1, 0, 0),
			richText = true
		};
		selectButtonText.normal.background = null;
		selectButtonText.normal.textColor = EditorGUIUtility.isProSkin ? new Color(0.788f, 0.788f, 0.788f, 1f) : new Color(0.047f, 0.047f, 0.047f, 1f);
	}
}

