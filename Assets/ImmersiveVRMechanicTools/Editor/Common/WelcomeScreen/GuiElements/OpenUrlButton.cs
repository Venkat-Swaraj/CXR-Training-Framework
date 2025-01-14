using UnityEngine;

namespace ImmersiveVRTools.Editor.Common.WelcomeScreen.GuiElements
{
    public class OpenUrlButton : ClickableElement
    {
        public string Url { get; set; }

        public OpenUrlButton(string text, string url)
            : this(text, url, "BuildSettings.Web.Small")
        {
            Url = url;
        }
        
        public OpenUrlButton(string text, string url, string icon)
            : base(text, icon)
        {
            Url = url;
        }

        public override void OnClick(ProductWelcomeScreenBase welcomeScreen)
        {
            Application.OpenURL(Url);
        }
    }
}