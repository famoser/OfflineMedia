using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace OfflineMedia.Data.Helpers
{
    public class TextHelper
    {
        private const string ReplaceRegex = "\\bDer\\b|\\bDie\\b|\\bDas\\b";

        public static List<string> GetImportantWords(string text)
        {
            if (text != null)
            {
                Regex rgx = new Regex("[^a-zA-Z0-9äüößéèêëçàâæîïôœùûüÿ -]");
                text = rgx.Replace(text, "");

                text = Regex.Replace(text, ReplaceRegex, "");

                if (text.Contains(". "))
                {
                    var phrases = text.Split(new[] {". "}, StringSplitOptions.None);
                    var res = new List<string>();
                    foreach (var phrase in phrases)
                    {
                        res.AddRange(
                            phrase.Split(' ')
                                .Skip(1)
                                .Where(
                                    s =>
                                        s.Length > 2 &&
                                        string.Equals(s.Substring(0, 1), s.Substring(0, 1).ToUpper(),
                                            StringComparison.Ordinal) &&
                                        !string.Equals(s.Substring(0, 1), s.Substring(0, 1).ToLower(),
                                            StringComparison.Ordinal))
                                .ToList());
                    }
                    return res;
                }

                return
                    text.Split(' ')
                        .Skip(1)
                        .Where(
                            s =>
                                s.Length > 2 &&
                                string.Equals(s.Substring(0, 1), s.Substring(0, 1).ToUpper(), StringComparison.Ordinal) &&
                                !string.Equals(s.Substring(0, 1), s.Substring(0, 1).ToLower(), StringComparison.Ordinal))
                        .ToList();
            }
            return new List<string>();
        }

        public static List<string> FusionLists(params List<string>[] lists)
        {
            var dic = new Dictionary<string, int>();
            foreach (var list in lists)
            {
                foreach (var item in list)
                {
                    if (dic.ContainsKey(item))
                        dic[item]++;
                    else
                        dic.Add(item, 1);
                }
            }
            return dic.OrderByDescending(d => d.Value).Select(d => d.Key).ToList();
        }
    }
}
