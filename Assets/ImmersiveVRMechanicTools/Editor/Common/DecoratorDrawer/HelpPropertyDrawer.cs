using ImmersiveVRTools.Runtime.Common.PropertyDrawer;
using UnityEditor;
using UnityEngine;

namespace ImmersiveVRTools.Editor.Common.DecoratorDrawer
{
    [CustomPropertyDrawer(typeof(HelpAttribute))]
    public class HelpPropertyDrawer : UnityEditor.DecoratorDrawer
    {
        private readonly int _heightPadding = 10;
        private readonly int _helpBoxIconWidth = 40;

        private GUIStyle _helpBoxStyle;
        private GUIContent _helpContent;
        
        public override void OnGUI(Rect position)
        {
            var attr = (HelpAttribute) attribute;
            EditorGUI.HelpBox(position, attr.Text, (MessageType)attr.Type);
        }
        
        public override float GetHeight()
        {
            Init();
            return _helpBoxStyle.CalcHeight(_helpContent, EditorGUIUtility.currentViewWidth - _helpBoxIconWidth) + _heightPadding;
        }

        private void Init()
        {
            if (_helpBoxStyle == null)
            {
                _helpBoxStyle = GUI.skin.GetStyle("HelpBox");
            }

            if (_helpContent == null)
            {
                var attr = (HelpAttribute) attribute;
                _helpContent = new GUIContent(attr.Text);
            }
        }
    }
}