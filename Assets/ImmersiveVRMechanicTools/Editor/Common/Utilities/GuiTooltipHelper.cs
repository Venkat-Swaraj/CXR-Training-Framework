using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ImmersiveVRTools.Editor.Common.Utilities
{
    public class GuiTooltipHelper
    {
        private static readonly GUIStyle HelpTooltipStyle = new GUIStyle("IconButton");
        private static readonly Texture HelpTexture = EditorGUIUtility.TrIconContent("_Help").image;
        private static Dictionary<Vector4, GUIStyle> _rectOffsetToCachedHelpTooltipStyle = new Dictionary<Vector4, GUIStyle>(); 
        
        public static void AddHelperTooltip(string content) => AddHelperTooltip(content, Vector4.zero, 40);
        public static void AddHelperTooltip(string content, Vector4 margin, int helpTooltipWidth)
        {
            GUIStyle tooltipStyleWithPadding;
            if (margin == Vector4.zero) tooltipStyleWithPadding = HelpTooltipStyle;
            else
            {
                if (!_rectOffsetToCachedHelpTooltipStyle.ContainsKey(margin))
                    _rectOffsetToCachedHelpTooltipStyle[margin] = new GUIStyle(HelpTooltipStyle)
                    {
                        margin = new RectOffset((int) margin.x, (int) margin.z, (int) margin.y, (int) margin.w)
                    };

                tooltipStyleWithPadding = _rectOffsetToCachedHelpTooltipStyle[margin];
            }
            
            var documentationIcon = new GUIContent(HelpTexture, content);
            GUILayout.Label(documentationIcon, tooltipStyleWithPadding, GUILayout.Width(helpTooltipWidth));
        }
    }
}