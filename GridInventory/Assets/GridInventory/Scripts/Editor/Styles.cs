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

        normalColor         = new Color(0.219f, 0.219f, 0.219f, 1f);
        hoverColor          = new Color(0.266f, 0.266f, 0.266f, 1f);
        activeColor         = new Color(0.172f, 0.364f, 0.529f, 1f);
        elementButtonColor  = new Color(0.788f, 0.788f, 0.788f, 1f);

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
        selectButtonText.normal.textColor = new Color(0.788f, 0.788f, 0.788f, 1f);

        elementButton = new GUIStyle("TransitionSelectHead")
        {
            alignment = TextAnchor.MiddleLeft,
            padding = new RectOffset(22, 0, 0, 0),
            overflow = new RectOffset(0, -1, 0, 0),
            margin = new RectOffset(1, 1, 0, 0),            
        };      

    }
}

