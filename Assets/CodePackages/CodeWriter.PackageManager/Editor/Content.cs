using UnityEditor;
using UnityEngine;

namespace CodeWriter.PackageManager
{
    class Content
    {
        public static GUIStyle greyMiniLabel;
        public static GUIStyle centeredMiniLabel;
        public static GUIStyle projectWindowGreyOverlay;
        public static GUIStyle greyBorder;
        public static GUIStyle groupBox;
        public static GUIStyle CN_Box;
        public static GUIStyle IN_BigTitie;
        public static GUIStyle warnIconSmall;
        public static GUIStyle infoIconSmall;

        static Content()
        {
            greyMiniLabel = new GUIStyle(EditorStyles.centeredGreyMiniLabel);
            greyMiniLabel.alignment = TextAnchor.UpperLeft;
            greyMiniLabel.margin = new RectOffset(10, 0, 0, 0);

            centeredMiniLabel = new GUIStyle(EditorStyles.miniLabel);
            centeredMiniLabel.alignment = TextAnchor.MiddleCenter;

            projectWindowGreyOverlay = new GUIStyle(GUI.skin.box);
            var bg = new Texture2D(1, 1);
            bg.hideFlags = HideFlags.DontSave;
            bg.SetPixel(0, 0, new Color(0.75f, 0.75f, 0.75f, 0.65f));
            bg.Apply();
            projectWindowGreyOverlay.normal.background = bg;

            var builtinSkin = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector);

            warnIconSmall = new GUIStyle(builtinSkin.GetStyle("CN EntryWarnIconSmall"));
            infoIconSmall = new GUIStyle(builtinSkin.GetStyle("CN EntryInfoIconSmall"));

            greyBorder = new GUIStyle(builtinSkin.GetStyle("grey_border"));
            greyBorder.stretchHeight = false;
            greyBorder.stretchWidth = false;

            groupBox = new GUIStyle(builtinSkin.GetStyle("GroupBox"));
            groupBox.margin = new RectOffset();
            groupBox.padding = new RectOffset();

            CN_Box = new GUIStyle(builtinSkin.GetStyle("CN Box"));
            CN_Box.stretchWidth = false;
            CN_Box.stretchHeight = false;

            IN_BigTitie = new GUIStyle(builtinSkin.GetStyle("IN BigTitle"));
            IN_BigTitie.margin = new RectOffset();
            IN_BigTitie.padding = new RectOffset();
        }
    }
}
