using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Famoser.FrameworkEssentials.Logging;

namespace Famoser.OfflineMedia.Business.Helpers.Text
{
    public class XmlHelper
    {
        public static string AddXmlHeaderNode(string feed, string name)
        {
            return "<?xml version=\"1.0\" encoding=\"utf-8\"?> <" + name + " xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">" + feed + " </" + name + ">";
        }

        public static string RemoveXmlLvl(string html)
        {
            html = html.Substring(html.IndexOf(">", StringComparison.Ordinal) + 1);
            html = html.Substring(0, html.LastIndexOf("<", StringComparison.Ordinal));
            html = html.Trim();
            return html;
        }

        public static List<string> GetNodes(string xml, string nodeName)
        {
            var res = new List<string>();
            var endnode = nodeName;
            if (endnode.Contains(" "))
                endnode = endnode.Substring(0, endnode.IndexOf(" ", StringComparison.Ordinal));

            while (xml.Contains("<" + nodeName + " ") || xml.Contains("<" + nodeName + ">"))
            {
                var index1 = xml.IndexOf("<" + nodeName + " ", StringComparison.Ordinal);
                var index2 = xml.IndexOf("<" + nodeName + ">", StringComparison.Ordinal);
                var index = (index1 < index2 && index1 != -1) || index2 == -1 ? index1 : index2;

                xml = xml.Substring(index);
                var endNodeIndex = xml.IndexOf("</" + endnode + ">", StringComparison.Ordinal);
                var lenght = +("</" + endnode + ">").Length;
                res.Add(xml.Substring(0, endNodeIndex + lenght));
                xml = xml.Substring(endNodeIndex + lenght);
            }

            return res;
        }

        public static string GetSingleNode(string xml, string nodeName)
        {
            var list = GetNodes(xml, nodeName);
            return list.FirstOrDefault();
        }

        internal static string RemoveNodes(string xml, params string[] nodes)
        {
            foreach (var node in nodes)
            {
                var removeNodes = GetNodes(xml, node);
                xml = removeNodes.Aggregate(xml, (current, removeNode) => current.Replace(removeNode, ""));
            }
            return xml;
        }


        public static T Deserialize<T>(string str)
        {
            var serializer = new XmlSerializer(typeof(T));
            TextReader reader = new StringReader(str.Trim());

            try
            {
                var zeitFeed = (T)serializer.Deserialize(reader);
                if (zeitFeed == null)
                    LogHelper.Instance.Log(LogLevel.Error, "ZeitHelper.EvaluateFeed failed: Feed is null after deserialisation", "XmlDeserializer");
                else
                    return zeitFeed;
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Log(LogLevel.Error, "XmlDeserializer failed: Exception occured for obj " + typeof(T).Name + " message: " + ex.Message, "XmlDeserializer");
            }
            return default(T);
        }
    }
}
