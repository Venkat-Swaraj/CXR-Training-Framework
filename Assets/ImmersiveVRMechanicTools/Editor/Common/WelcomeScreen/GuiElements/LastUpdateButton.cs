using System;

namespace ImmersiveVRTools.Editor.Common.WelcomeScreen.GuiElements
{
    public class LastUpdateButton: ChangeMainViewButton, IMainScreenChanger
    {
        public LastUpdateButton(string text, Action<ProductWelcomeScreenBase> renderMainScrollViewFn) 
            : base(text, renderMainScrollViewFn)
        {
        }
    }
}