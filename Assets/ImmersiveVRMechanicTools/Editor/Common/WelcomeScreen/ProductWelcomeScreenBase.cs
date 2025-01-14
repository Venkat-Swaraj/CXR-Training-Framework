using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using ImmersiveVRTools.Editor.Common.WelcomeScreen.GuiElements;
using ImmersiveVRTools.Editor.Common.WelcomeScreen.Utilities;
using ImmersiveVrToolsCommon.Runtime.Logging;

namespace ImmersiveVRTools.Editor.Common.WelcomeScreen
{
    public abstract class ProductWelcomeScreenBase : EditorWindow
    {
        public abstract string WindowTitle { get; }
        public abstract Vector2 WindowSizePx { get; }
        public const int LeftSectionWidthPx = 145; //TODO: not 100% sure why it got locked at that
        private static int _ContentPanelWidthPx { get; set; }
        public int ContentPanelWidthPx => _ContentPanelWidthPx;

        private string _showAtStartupPreferenceKey;
        protected string ShowAtStartupPreferenceKey => _showAtStartupPreferenceKey ?? (_showAtStartupPreferenceKey = ProductPreferenceBase.CreateDefaultShowOptionPreferenceDefinition().PreferenceKey);

        public GUIStyle LabelStyle { get; private set; }
        public GUIStyle BoldTextStyle { get; private set; }
        public GUIStyle UpdateTextStyle { get; private set; }
        public GUIStyle ButtonStyle { get; private set; }
        public GUIStyle LinkStyle { get; private set; }
        public GUIStyle LabelSubTextStyle { get; private set; }
        public GUIStyle TextStyle { get; private set; }
        public GUIStyle SelectedSideButtonStyle { get; private set; }

        private ProductPreferenceBase.StartupShowMode _startupStartupShowMode;
        private GUIContent _productIcon;

        private Action<ProductWelcomeScreenBase> _renderMainScrollView;
        private Action<ProductWelcomeScreenBase> RenderMainScrollView
        {
            get => _renderMainScrollView;
            set
            {
                _renderMainScrollView = value;
            }
        }
        
        protected static readonly ScrollViewGuiSection LastUpdateUpdateScrollViewSection = new ScrollViewGuiSection(
            "New Update", (screen) =>
            {
                RenderAsRichTextWithClickableUrls(screen.LastUpdateText, GUILayout.ExpandHeight(true), screen);
            }
        );

        private Vector2 _scrollPosition = Vector2.zero;
        public string LastUpdateText { get; private set; }
        public bool IsUpdateTextViewAlreadyPreselected { get; private set; }

        private bool _isInitialized = false;
        private static string _LastSelectedScreenChangerNamePerfKey;
        private static string LastSelectedScreenChangerNamePerfKey
        {
            get
            {
                if (_LastSelectedScreenChangerNamePerfKey == null)
                {
                    _LastSelectedScreenChangerNamePerfKey = $"{Application.productName}_LastSelectedScreenChangerName";
                }

                return _LastSelectedScreenChangerNamePerfKey;
            }
        }

        public void OpenWindow()
        {
            OpenWindow(GetType(), WindowTitle, WindowSizePx);
        }

        protected static TWindowType OpenWindow<TWindowType>(string title, Vector2 windowSizePx) where TWindowType : ProductWelcomeScreenBase
        {
            return (TWindowType)OpenWindow(typeof(TWindowType), title, windowSizePx);
        }

        public static object OpenWindow(Type windowType, string title, Vector2 windowSizePx)
        {
            var window = GetWindow(windowType, true, title);
            window.minSize = windowSizePx;
            window.maxSize = windowSizePx;
            window.Show();

            _ContentPanelWidthPx = (int)windowSizePx.x - LeftSectionWidthPx;

            return window;
        }

        protected void OnEnableCommon(string productIconName)
        {
            if (!string.IsNullOrEmpty(productIconName) && _productIcon == null)
            {
                var iconGuid = AssetDatabase.FindAssets("ProductIcon64").FirstOrDefault();
                if (!string.IsNullOrEmpty(iconGuid))
                {
                    _productIcon = new GUIContent(AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GUIDToAssetPath(iconGuid)));
                }
            }
        }

        internal void EnsureInitialized()
        {
            if (_isInitialized) return;

            EnsureStylesInitialized(); 
            _startupStartupShowMode = (ProductPreferenceBase.StartupShowMode)EditorPrefs.GetInt(ShowAtStartupPreferenceKey, (int)ProductPreferenceBase.StartupShowMode.Always);
            LastUpdateText = ProductPreferenceBase.CreateDefaultLastUpdateText().GetEditorPersistedValueOrDefault() as string;
            
            _isInitialized = true;
        }

        protected void RenderGUI(List<GuiSection> leftSections, GuiSection topSection, GuiSection bottomSection, ScrollViewGuiSection mainScrollViewGuiSection)
        {
            EnsureInitialized();

            if (RenderMainScrollView == null)
            {
                SetInitiallyVisibleMainScreen(leftSections, mainScrollViewGuiSection);
            }

            EditorGUILayout.BeginHorizontal(GUIStyle.none, GUILayout.ExpandWidth(true));
            {
                if (leftSections != null && leftSections.Any())
                {
                    EditorGUILayout.BeginVertical(GUILayout.Width(leftSections.First().WidthPx));
                    foreach (var leftSection in leftSections)
                    {
                        {
                            RenderSectionStart(leftSection);

                            foreach (var element in leftSection.ClickableElements)
                            {
                                var lastUpdateButton = element as LastUpdateButton;
                                if (lastUpdateButton != null)
                                {
                                    if (!string.IsNullOrWhiteSpace(LastUpdateText))
                                    {
                                        if (!IsUpdateTextViewAlreadyPreselected)
                                        {
                                            ChangeMainScrollViewRenderFn(lastUpdateButton.RenderMainScrollViewFn, lastUpdateButton.Text);
                                            IsUpdateTextViewAlreadyPreselected = true; 
                                        }
                                        RenderClickableElementIfVisible(element);
                                    }
                                }
                                else
                                {
                                    RenderClickableElementIfVisible(element);
                                }
                            }

                            GUILayout.Space(10);
                        }
                    }

                    EditorGUILayout.EndVertical();
                }
                
                EditorGUILayout.BeginVertical(GUILayout.Width(topSection.WidthPx), GUILayout.ExpandHeight(true));
                {
                    if (topSection != null)
                    {
                        RenderSectionStart(topSection);

                        if (topSection.ClickableElements.Count > 0)
                        {
                            EditorGUILayout.BeginHorizontal(GUILayout.Width(topSection.WidthPx));
                            {
                                foreach (var element in topSection.ClickableElements)
                                {
                                    RenderClickableElementIfVisible(element);
                                }
                            }
                            EditorGUILayout.EndHorizontal();
                        }
                    }

                    
                    if (mainScrollViewGuiSection != null)
                    {
                        RenderSectionStart(mainScrollViewGuiSection);
                        _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, "ProgressBarBack", GUILayout.ExpandHeight(true), GUILayout.Width(mainScrollViewGuiSection.WidthPx));

                        RenderMainScrollView(this);

                        GUILayout.EndScrollView();
                    }


                    if (bottomSection != null)
                    {
                        EditorGUILayout.BeginHorizontal(GUILayout.Width(bottomSection.WidthPx));
                        {
                            EditorGUILayout.BeginVertical();
                            {
                                RenderSectionStart(bottomSection);

                                EditorGUILayout.BeginHorizontal();
                                {
                                    for (var i = 0; i < bottomSection.ClickableElements.Count; i++)
                                    {
                                        var bottomLink = bottomSection.ClickableElements[i];
                                        if (GUILayout.Button(bottomLink.Text, LinkStyle))
                                            bottomLink.OnClick(this);

                                        if (i < bottomSection.ClickableElements.Count - 1)
                                        {
                                            GUILayout.Label("-");
                                        }
                                    }
                                }
                                EditorGUILayout.EndHorizontal();
                                GUILayout.Space(7);
                            }
                            EditorGUILayout.EndVertical();

                            GUILayout.FlexibleSpace();
                            EditorGUILayout.BeginVertical();
                            GUILayout.Space(7);
                            if (_productIcon != null) GUILayout.Label(_productIcon);
                            EditorGUILayout.EndVertical();
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndHorizontal();


            EditorGUILayout.BeginHorizontal("ProjectBrowserBottomBarBg", GUILayout.ExpandWidth(true), GUILayout.Height(22));
            {
                GUILayout.FlexibleSpace();
                EditorGUI.BeginChangeCheck();
                var cache = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = 100;
                _startupStartupShowMode = (ProductPreferenceBase.StartupShowMode)EditorGUILayout.EnumPopup("Show At Startup", _startupStartupShowMode, GUILayout.Width(220));
                EditorGUIUtility.labelWidth = cache;
                if (EditorGUI.EndChangeCheck())
                {
                    EditorPrefs.SetInt(ShowAtStartupPreferenceKey, (int)_startupStartupShowMode);
                }
            }
            EditorGUILayout.EndHorizontal();
        }
        
        protected static void RenderAsRichTextWithClickableUrls(string text, GUILayoutOption guiLayoutOption, ProductWelcomeScreenBase screen)
        {
            foreach (var subpart in TextIntoIdentifiableUrlSubpartsParser.Parse(text)) //todo: cache
            {
                if (GUILayout.Button(subpart.Text, screen.UpdateTextStyle, guiLayoutOption))
                {
                    if (!string.IsNullOrEmpty(subpart.Url))
                    {
                        Application.OpenURL(subpart.Url);
                    }
                }
            }
        }

        private void SetInitiallyVisibleMainScreen(List<GuiSection> leftSections, ScrollViewGuiSection mainScrollViewGuiSection)
        {
            // if updates not visible show last selected screen or welcome
            if (string.IsNullOrWhiteSpace(LastUpdateText))
            {
                var lastSelectedScreenChangerName = GetLastSelectedScreenChangerName();
                if (!string.IsNullOrEmpty(lastSelectedScreenChangerName))
                {
                    var lastSelectedScreenChangerBeforeReload = leftSections
                        .SelectMany(s => s.ClickableElements)
                        .Where(e => e.Text == lastSelectedScreenChangerName && e is IMainScreenChanger)
                        .Cast<IMainScreenChanger>()
                        .FirstOrDefault();

                    if (lastSelectedScreenChangerBeforeReload != null)
                    {
                        ChangeMainScrollViewRenderFn(lastSelectedScreenChangerBeforeReload.RenderMainScrollViewFn,
                            lastSelectedScreenChangerBeforeReload.Text);
                    }
                    else
                    {
                        LoggerScoped.LogWarning($"Last remembered welcome screen section: '{lastSelectedScreenChangerName}' does not exist, changing to default screen");
                        RenderMainScrollView = mainScrollViewGuiSection.RenderMainScrollViewSection;
                    }
                }
                else
                {
                    RenderMainScrollView = mainScrollViewGuiSection.RenderMainScrollViewSection;
                }
            }
            else
            {
                RenderMainScrollView = mainScrollViewGuiSection.RenderMainScrollViewSection;
            }
        }

        private static string GetLastSelectedScreenChangerName()
        {
            return EditorPrefs.GetString(LastSelectedScreenChangerNamePerfKey, string.Empty);
        }

        private void RenderSectionStart(GuiSectionBase bottomSection)
        {
            if (!string.IsNullOrEmpty(bottomSection.LabelText)) GUILayout.Label(bottomSection.LabelText, LabelStyle);
            if (!string.IsNullOrEmpty(bottomSection.LabelSubText))
            {
                GUILayout.Label(bottomSection.LabelSubText, LabelSubTextStyle);
            }
        }

        private void RenderClickableElementIfVisible(ClickableElement clickableElement)
        {
            if(!clickableElement.ShouldRender())
                return;
            
            GUIContent buttonGuiContent;
            if (!string.IsNullOrEmpty(clickableElement.IconName))
            {
                buttonGuiContent = new GUIContent($" {clickableElement.Text}", EditorGUIUtility.IconContent(clickableElement.IconName).image);
            }
            else
            {
                buttonGuiContent = new GUIContent(clickableElement.Text);
            }

            var isSelected = GetLastSelectedScreenChangerName() == clickableElement.Text;
            if (GUILayout.Button(buttonGuiContent, isSelected ? SelectedSideButtonStyle : GUI.skin.button, GUILayout.ExpandWidth(true)))
            {
                clickableElement.OnClick(this);
            }
        }

        public void ChangeMainScrollViewRenderFn(Action<ProductWelcomeScreenBase> renderFn, string name)
        {
            RenderMainScrollView = renderFn;
            EditorPrefs.SetString(LastSelectedScreenChangerNamePerfKey, name);
        }

        private void EnsureStylesInitialized()
        {
            if (LabelStyle == null)
            {
                LabelStyle = new GUIStyle("BoldLabel")
                {
                    margin = new RectOffset(4, 4, 4, 4),
                    padding = new RectOffset(2, 2, 2, 2),
                    fontSize = 13
                };
            }

            if (BoldTextStyle == null)
            {
                BoldTextStyle = new GUIStyle("BoldLabel")
                {
                    margin = new RectOffset(0, 0, 0, 0),
                    padding = new RectOffset(2, 2, 2, 2),
                    fontSize = 13
                };
            }
            
            if (UpdateTextStyle == null)
            {
                UpdateTextStyle = new GUIStyle("BoldLabel")
                {
                    margin = new RectOffset(0, 0, 0, 0),
                    padding = new RectOffset(2, 2, 2, 2),
                    fontSize = 13,
                    richText = true
                };
            }

            if (ButtonStyle == null)
            {
                ButtonStyle = new GUIStyle(GUI.skin.button)
                {
                    alignment = TextAnchor.MiddleLeft
                };
            }

            if (LinkStyle == null)
            {
                LinkStyle = new GUIStyle
                {
                    normal = { textColor = new Color(0.2980392f, 0.4901961f, 1f) },
                    hover = { textColor = Color.white },
                    active = { textColor = Color.grey },
                    margin = { top = 3, bottom = 2 }
                };
            }

            if (LabelSubTextStyle == null)
            {
                LabelSubTextStyle = new GUIStyle("WordWrappedMiniLabel") { padding = { top = -5 } };
            }

            if (TextStyle == null)
            {
                TextStyle = new GUIStyle("WordWrappedMiniLabel")
                {
                    padding = { bottom = 5 },
                    fontSize = 12,
                    richText = true
                };
            }

            if (SelectedSideButtonStyle == null)
            {
                SelectedSideButtonStyle = new GUIStyle(GUI.skin.button);
                SelectedSideButtonStyle.normal.background = Texture2D.linearGrayTexture;
                SelectedSideButtonStyle.fontStyle = FontStyle.Bold;
            }
        }

        protected static void GenerateCommonWelcomeText(string productName, ProductWelcomeScreenBase screen, string customText = null)
        {
            GUILayout.Label(
                (@"Thanks for using the asset!

If at any stage you got some questions or need help please let me know via Unity Forum page.

Or if you prefer you can also reach out directly via contact page.

There are some options that you can customise, those are visible in sections on the left. 

You can always get back to this screen via: 
1) Window -> <ProductName> -> Start Screen 
2) Edit -> Preferences... -> <ProductName>" 
+ (string.IsNullOrWhiteSpace(customText) ? "" : $"\r\n\r\n{customText}") + @"

Hope the asset will meet your expectations.")
                    .Replace("<ProductName>", productName),
                screen.TextStyle, GUILayout.ExpandHeight(true));
        }

        protected static string GetScenePath(string sceneName)
        {
            var demoScene = AssetDatabase.FindAssets($"t:Scene {sceneName}").FirstOrDefault();
            return demoScene != null ? AssetDatabase.GUIDToAssetPath(demoScene) : null;
        }
    }
}