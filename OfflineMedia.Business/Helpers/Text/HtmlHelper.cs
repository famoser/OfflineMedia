using System.Collections.Generic;

namespace Famoser.OfflineMedia.Business.Helpers.Text
{
    // ReSharper disable StringIndexOfIsCultureSpecific.1
    // ReSharper disable StringLastIndexOfIsCultureSpecific.1
    // ReSharper disable StringLastIndexOfIsCultureSpecific.3
    public class HtmlHelper
    {
        public static string ExtractId(string html, string id)
        {
            if (!html.Contains(id)) { return null; }
            var c = html.IndexOf("id=\"" + id + "\"");
            if (c == -1)
            {
                c = html.IndexOf("id='" + id + "'");
            }
            html = html.Substring(html.LastIndexOf("<", c, c - 1));
            string tag = html.Substring(1, html.IndexOf(" ") - 1);
            string gegentag = "</" + tag + ">";
            tag = "<" + tag;
            int counter = 0;
            int index = 0;
            while (true)
            {
                if (html.Substring(index).IndexOf(tag) < html.Substring(index).IndexOf(gegentag))
                {
                    if (html.Substring(index).IndexOf(tag) == -1)
                    {
                        counter--;
                        index = html.Substring(index).IndexOf(gegentag) + 1 + index;
                        if (counter == 0)
                        {
                            html = html.Substring(0, (index - 1) + gegentag.Length);
                            break;
                        }
                    }
                    else
                    {
                        counter++;
                        index = html.Substring(index).IndexOf(tag) + 1 + index;
                    }

                }
                else
                {
                    if (html.Substring(index).IndexOf(gegentag) == -1)
                    {
                        return null;
                    }
                    counter--;
                    index = html.Substring(index).IndexOf(gegentag) + 1 + index;
                    if (counter == 0)
                    {
                        html = html.Substring(0, (index - 1) + gegentag.Length);
                        break;
                    }
                }
            }
            return html;
        }

        //DOESNT WORK
        public static List<string> ExtractNode(string htmlcode, string node, string endnode)
        {
            int mainindex = 0;
            var list = new List<string>();
            while (htmlcode.Substring(mainindex).Contains(node))
            {
                string html = htmlcode.Substring(mainindex);
                html = html.Substring(html.IndexOf(node));
                mainindex = mainindex + html.IndexOf(node) + 1;
                int counter = 0;
                int index = 0;
                while (true)
                {
                    if (html.Substring(index).IndexOf(node) < html.Substring(index).IndexOf(endnode))
                    {
                        if (html.Substring(index).IndexOf(node) == -1)
                        {
                            counter--;
                            index = html.Substring(index).IndexOf(endnode) + 1 + index;
                            if (counter == 0)
                            {
                                html = html.Substring(0, (index - 1) + endnode.Length);
                                break;
                            }
                        }
                        else
                        {
                            counter++;
                            index = html.Substring(index).IndexOf(node) + 1 + index;
                        }

                    }
                    else
                    {
                        if (html.Substring(index).IndexOf(endnode) == -1)
                        {
                            return null;
                        }
                        counter--;
                        index = html.Substring(index).IndexOf(endnode) + 1 + index;
                        if (counter == 0)
                        {
                            html = html.Substring(0, (index - 1) + endnode.Length);
                            break;
                        }
                    }
                }
                list.Add(html);
            }
            return list;
        }

        //DOESNT WORK
        public static List<string> ExtractClasses(string htmlcode, string clas)
        {
            int mainindex = 0;
            var list = new List<string>();
            while (htmlcode.Substring(mainindex).Contains("class=\"" + clas + "\""))
            {
                string html = htmlcode.Substring(mainindex);
                int c = html.IndexOf("class=\"" + clas + "\"");
                html = html.Substring(html.LastIndexOf("<", c, c - 1));
                string tag = html.Substring(1, html.IndexOf(" ") - 1);
                string gegentag = "</" + tag + ">";
                tag = "<" + tag;
                int counter = 0;
                int index = 0;
                while (true)
                {
                    if (html.Substring(index).IndexOf(tag) < html.Substring(index).IndexOf(gegentag))
                    {
                        if (html.Substring(index).IndexOf(tag) == -1)
                        {
                            counter--;
                            index = html.Substring(index).IndexOf(gegentag) + 1 + index;
                            if (counter == 0)
                            {
                                html = html.Substring(0, (index - 1) + gegentag.Length);
                                break;
                            }
                        }
                        else
                        {
                            counter++;
                            index = html.Substring(index).IndexOf(tag) + 1 + index;
                        }

                    }
                    else
                    {
                        if (html.Substring(index).IndexOf(gegentag) == -1)
                        {
                            return null;
                        }
                        counter--;
                        index = html.Substring(index).IndexOf(gegentag) + 1 + index;
                        if (counter == 0)
                        {
                            html = html.Substring(0, (index - 1) + gegentag.Length);
                            break;
                        }
                    }
                }
                list.Add(html);
                mainindex = mainindex + index;
            }
            return list;
        }

        public static string[] GetFirstLink(string html)
        {
            if (!html.Contains("<a")) { return null; }
            var s = new string[2];
            html = html.Substring(html.IndexOf("<a "));
            html = html.Substring(html.IndexOf("href=\"") + ("href=\"").Length + 1);
            s[0] = html.Substring(0, html.IndexOf("\""));
            html = html.Substring(html.IndexOf(">") + 1);
            s[1] = html.Substring(0, html.IndexOf("</a>"));
            return s;
        }

        public static List<string> Split(string html)
        {
            var splitItems = new List<string>();
            string tag = html.Substring(1, html.IndexOf(" ") - 1);
            string gegentag = "</" + tag + ">";
            tag = "<" + tag;
            int oldindex = 0;
            int counter = 0;
            int index = 0;
            while (true)
            {
                if (html.Substring(index).IndexOf(tag) < html.Substring(index).IndexOf(gegentag))
                {
                    if (html.Substring(index).IndexOf(tag) == -1)
                    {
                        counter--;
                        index = html.Substring(index).IndexOf(gegentag) + 1 + index;
                        if (counter == 0)
                        {
                            splitItems.Add(html.Substring(oldindex));
                            break;
                        }
                    }
                    else
                    {
                        counter++;
                        index = html.Substring(index).IndexOf(tag) + 1 + index;
                    }

                }
                else
                {
                    if (html.Substring(index).IndexOf(gegentag) == -1)
                    {
                        return null;
                    }
                    counter--;
                    index = html.Substring(index).IndexOf(gegentag) + 1 + index;
                    if (counter == 0)
                    {
                        splitItems.Add(html.Substring(oldindex, (index - 1) + gegentag.Length - oldindex));
                        oldindex = (index - 1) + gegentag.Length;
                    }
                }
            }
            return splitItems;
        }

        #region XMLUtilities
        public static string AddXmlHeaderNode(string feed, string name)
        {
            return "<?xml version=\"1.0\" encoding=\"utf-8\"?> <" + name + " xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">" + feed + " </" + name + ">";
        }

        public static string RemoveXmlLvl(string html)
        {
            html = html.Substring(html.IndexOf(">") + 1);
            html = html.Substring(0, html.LastIndexOf("<"));
            html = html.Trim();
            return html;
        }
        #endregion
    }
    // ReSharper restore StringIndexOfIsCultureSpecific.1
    // ReSharper restore StringLastIndexOfIsCultureSpecific.1
    // ReSharper restore StringLastIndexOfIsCultureSpecific.3
}
