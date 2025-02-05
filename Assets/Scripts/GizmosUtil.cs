using System;
using Game;
using UnityEditor;
using UnityEngine;

public class GizmosUtil
{
    public const int LabelPadding = 10;
    public const bool AddNewLine = true;
    private static Texture2D _gizmoTextStyleBackground;
    private static GUIStyle _gizmoTextStyle;

    public static Texture2D GizmoTextStyleBackground => _gizmoTextStyleBackground ??=
        ((Func<Texture2D>)(() =>
        {
            var result = new Texture2D(1, 1, TextureFormat.ARGB32, false);
            result.SetPixel(0, 0, new Color(.05F, .05F, .05F, .5F));
            result.Apply();
            return result;
        }))();

    public static GUIStyle GizmoTextStyle => _gizmoTextStyle ??= new GUIStyle
    {
        font = Resources.Load<Font>("Fonts/monogram"),
        alignment = TextAnchor.MiddleLeft,
        padding = new RectOffset(5, 5, 5, 5),
        fixedWidth = 230,
        normal = { textColor = Color.white, background = GizmoTextStyleBackground },
        fontSize = 18,
        richText = true
    };

    public static string Field(string label, string value)
    {
        return $"{label,-LabelPadding}: {value}" + (AddNewLine ? "\n" : "");
    }

    public static string ColoredField(string label, string value, bool isGreen)
    {
        return Field(label, ColoredValue(value, isGreen));
    }

    public static string ColoredValue(string value, bool isGreen)
    {
        return (isGreen ? "<color=green>" : "<color=red>") + $"{value}</color>";
    }

    public static string ColoredValue(Vector2 value)
    {
        return
            $"({ColoredValue(value.x.ToString("F2"), value.x >= 0)}, {ColoredValue(value.y.ToString("F2"), value.y >= 0)})";
    }

    public static string ColoredField(string label, bool value)
    {
        return ColoredField(label, value.ToString(), value);
    }

    public static string ColoredField(string label, float value)
    {
        return ColoredField(label, value.ToString("F2"), value >= 0);
    }

    public static string ColoredField(string label, Vector2 value)
    {
        return Field(label, ColoredValue(value));
    }

    public static string ColoredField(string label, PlayerController.DirectionalState value)
    {
        return ColoredField(label, Enum.GetName(typeof(PlayerController.DirectionalState), value),
            value != PlayerController.DirectionalState.None);
    }


    public static void DrawString(string text, Vector3 worldPosition, Color textColor, Vector2 anchor,
        float textSize = 15f)
    {
#if UNITY_EDITOR
        var view = SceneView.currentDrawingSceneView;
        if (!view)
            return;
        var screenPosition = view.camera.WorldToScreenPoint(worldPosition);
        if (screenPosition.y < 0 || screenPosition.y > view.camera.pixelHeight || screenPosition.x < 0 ||
            screenPosition.x > view.camera.pixelWidth || screenPosition.z < 0)
            return;
        var pixelRatio = HandleUtility.GUIPointToScreenPixelCoordinate(Vector2.right).x -
                         HandleUtility.GUIPointToScreenPixelCoordinate(Vector2.zero).x;
        Handles.BeginGUI();
        var style = new GUIStyle(GUI.skin.label)
        {
            fontSize = (int)textSize,
            normal = new GUIStyleState { textColor = textColor }
        };
        var size = style.CalcSize(new GUIContent(text)) * pixelRatio;
        var alignedPosition =
            ((Vector2)screenPosition +
             size * ((anchor + Vector2.left + Vector2.up) / 2f)) * (Vector2.right + Vector2.down) +
            Vector2.up * view.camera.pixelHeight;
        GUI.Label(new Rect(alignedPosition / pixelRatio, size / pixelRatio), text, style);
        Handles.EndGUI();
#endif
    }
}