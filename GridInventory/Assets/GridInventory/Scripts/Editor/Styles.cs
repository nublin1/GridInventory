using UnityEditor;
using UnityEngine;

public static class Styles
{
    public static GUIStyle selectButton;
    public static GUIStyle selectButtonText;
    public static GUIStyle elementButton;

    public static Color normalColor;
    public static Color hoverColor;
    public static Color activeColor;
    public static Color elementButtonColor;

    static Styles()
    {

        normalColor = EditorGUIUtility.isProSkin ? new Color(0.219f, 0.219f, 0.219f, 1f) : new Color(0.796f, 0.796f, 0.796f, 1f);
        hoverColor = EditorGUIUtility.isProSkin ? new Color(0.266f, 0.266f, 0.266f, 1f) : new Color(0.69f, 0.69f, 0.69f, 1f);
        activeColor = EditorGUIUtility.isProSkin ? new Color(0.172f, 0.364f, 0.529f, 1f) : new Color(0.243f, 0.459f, 0.761f, 1f);
        elementButtonColor = EditorGUIUtility.isProSkin ? new Color(0.788f, 0.788f, 0.788f, 1f) : new Color(0.047f, 0.047f, 0.047f, 1f);

        selectButton = new GUIStyle("TransitionSelectHead")
        {
            alignment = TextAnchor.MiddleLeft,
            padding = new RectOffset(5, 0, 0, 0),
            overflow = new RectOffset(0, -1, 0, 0),
        };
        selectButton.normal.background = ((GUIStyle)"ColorPickerExposureSwatch").normal.background;

        selectButtonText = new GUIStyle("TransitionTextSelectHead")
        {
            alignment = TextAnchor.MiddleLeft,
            padding = new RectOffset(5, 0, 0, 0),
            overflow = new RectOffset(0, -1, 0, 0),
            richText = true
        };
        selectButtonText.normal.background = null;
        selectButtonText.normal.textColor = EditorGUIUtility.isProSkin ? new Color(0.788f, 0.788f, 0.788f, 1f) : new Color(0.047f, 0.047f, 0.047f, 1f);

        elementButton = new GUIStyle("TransitionSelectHead")
        {
            alignment = TextAnchor.MiddleLeft,
            padding = new RectOffset(22, 0, 0, 0),
            overflow = new RectOffset(0, -1, 0, 0),
            margin = new RectOffset(1, 1, 0, 0),            
        };      

    }
}

