using System;

namespace ImmersiveVRTools.Editor.Common.WelcomeScreen.GuiElements
{
    public delegate bool ClickableElementShouldRenderFn();
    
    public abstract class ClickableElement
    {
        public string Text { get; }
        public string IconName { get; }
        private ClickableElementShouldRenderFn _shouldRenderFn;
        public ClickableElement(string text, string iconName)
        {
            Text = text;
            IconName = iconName;
        }

        public abstract void OnClick(ProductWelcomeScreenBase welcomeScreen);

        public bool ShouldRender()
        {
            return _shouldRenderFn == null || _shouldRenderFn();
        }

        public ClickableElement WithShouldRender(ClickableElementShouldRenderFn shouldRender)
        {
            _shouldRenderFn = shouldRender;
            return this;
        }
    }
}