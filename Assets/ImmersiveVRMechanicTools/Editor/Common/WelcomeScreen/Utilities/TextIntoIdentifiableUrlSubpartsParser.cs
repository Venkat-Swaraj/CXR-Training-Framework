using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using ImmersiveVrToolsCommon.Runtime.Logging;

namespace ImmersiveVRTools.Editor.Common.WelcomeScreen.Utilities
{
    public static class TextIntoIdentifiableUrlSubpartsParser
    {
        private static readonly string _breakDownATagsRegex = ".*(<a.*href=\"(.+)\".*>.+</a>).*";

        public static List<TextWithLinksPart> Parse(string text)
        {
            try
            {
                var subparts = new List<TextWithLinksPart>();
                var matches = Regex.Matches(text, _breakDownATagsRegex).Cast<Match>().ToList();

                if (matches.Count == 0)
                {
                    return GenerateNoUrlsResult(text);
                }

                var i = 0;
                var currentStrIndex = 0;
                do {
                    var currentMatch = matches[i];
                    var currentGroup = currentMatch.Groups[1];

                    subparts.Add(ParseTextWithLinksPart(currentMatch, text, currentStrIndex, false, currentGroup.Index - currentStrIndex));
                    currentStrIndex += currentGroup.Index - currentStrIndex;

                    subparts.Add(ParseTextWithLinksPart(currentMatch, text, currentStrIndex, true, currentGroup.Length));
                    currentStrIndex += currentGroup.Length;
	    
                    if(i == matches.Count - 1) {
                        subparts.Add(ParseTextWithLinksPart(currentMatch, text, currentStrIndex, false, null));
                    }
                } while(++i < matches.Count);
                return subparts.Where(s => !string.IsNullOrEmpty(s.Text)).ToList();
            }
            catch (Exception e)
            {
                LoggerScoped.LogDebug("Unable to break down text into url identifiable parts, returning original, ex: {e}");
                return GenerateNoUrlsResult(text);
            }
        }

        private static List<TextWithLinksPart> GenerateNoUrlsResult(string text)
        {
            return new List<TextWithLinksPart>()
            {
                new TextWithLinksPart(text, string.Empty)
            };
        }

        private static TextWithLinksPart ParseTextWithLinksPart(Match currentMatch, string text, int currentStrIndex, bool isUrl, int? lenght)
        {
            return new TextWithLinksPart(
                lenght.HasValue ? text.Substring(currentStrIndex, lenght.Value) : text.Substring(currentStrIndex),
                isUrl ? currentMatch.Groups[2].Value : string.Empty
            );
        }

        public class TextWithLinksPart {
            public string Text { get; }
            public string Url {get; }
            public bool IsLink => !string.IsNullOrEmpty(Url);

            public TextWithLinksPart(string text, string url) {
                Text = text;
                Url = url;
            }
        }
    }
}