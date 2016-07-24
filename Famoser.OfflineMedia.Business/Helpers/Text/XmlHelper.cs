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
                    LogHelper.Instance.Log(LogLevel.Error, 
                        "ZeitHelper.EvaluateFeed failed: Feed is null after deserialisation", "XmlDeserializer");
                else
                    return zeitFeed;
            }
            catch (Exception ex)
            {
                //todo wtf is this?
                if (ex.Message.StartsWith("There is an error in XML document (1, "))
                {
                    //example: "There is an error in XML document (1, 46013)."
                    var ms = ex.Message.Substring("There is an error in XML document (1, ".Length);
                    var index = Convert.ToInt32(ms.Substring(0, ms.Length - 2));
                    str = str.Insert(index, "\n\n\n");
                    if (index > 1000)
                    {
                        str = str.Substring(index - 1000);
                    }
                }
                else if (ex.Message.StartsWith("There is an error in XML document ("))
                {
                    //example: "There is an error in XML document (1, 46013)."
                    var ms = ex.Message.Substring("There is an error in XML document (".Length);
                    var line = ms.Split(new[] { "," }, StringSplitOptions.None)[0];
                    var index = Convert.ToInt32(line.Trim());
                    var lines = str.Split(new string[] { "\n" }, StringSplitOptions.None).ToList();
                    if (lines.Count > index)
                    {
                        lines.Insert(index - 1, "[FAILLURE]");
                        lines.Insert(index + 2, "[/FAILLURE]");
                    }
                    var start = index - 4 > 0 ? index - 4 : 0;
                    str = "";
                    for (int i = start; i < lines.Count; i++)
                    {
                        str += lines[i];
                    }
                }
                else
                {
                    LogHelper.Instance.Log(LogLevel.Error, "XmlDeserializer failed: Exception occured for obj " + typeof(T).Name + " message: " + ex.Message, "XmlDeserializer");
                }
            }
            return default(T);
        }
    }
}
