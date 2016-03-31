using System;
using System.Collections.Generic;
using System.Text;
using HtmlAgilityPack;

namespace OfflineMedia.WinUniversal.DisplayHelper.DependencyObjects
{
    internal class TagDefinition
    {
        public TagDefinition()
        {
            this.Attributes = new Dictionary<string, string>();
        }

        public TagDefinition(string xamlTag)
            : this()
        {
            this.BeginXamlTag = xamlTag;
            this.EndXamlTag = string.Format(xamlTag, string.Empty).Replace("<", "</");
        }

        public TagDefinition(string xamlBeginTag, string xamlEndTag, bool mustBeTop = false, bool ignore = false)
            : this()
        {
            this.BeginXamlTag = xamlBeginTag;
            this.EndXamlTag = xamlEndTag;
            this.MustBeTop = mustBeTop;
            this.Ignore = ignore;
        }

        public TagDefinition(Action<StringBuilder, HtmlNode> customAction)
            : this()
        {
            this.CustomAction = customAction;
            this.IsCustom = true;
        }

        public string BeginXamlTag { get; set; }
        public string EndXamlTag { get; set; }
        public Dictionary<string, string> Attributes { get; set; }
        public bool MustBeTop { get; set; }
        public bool IsCustom { get; set; }
        public bool Ignore { get; set; }
        public Action<StringBuilder, HtmlNode> CustomAction { get; set; }
    }
}
