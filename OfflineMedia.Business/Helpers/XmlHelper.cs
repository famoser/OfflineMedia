using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfflineMedia.Business.Helpers
{
    public static class XmlHelper
    {
        public static List<string> GetNodes(string xml, string nodeName)
        {
            var res = new List<string>();

            while (xml.Contains("<" + nodeName + " ") || xml.Contains("<" + nodeName + ">"))
            {
                var index1 = xml.IndexOf("<" + nodeName + " ", StringComparison.Ordinal);
                var index2 = xml.IndexOf("<" + nodeName + ">", StringComparison.Ordinal);
                var index = (index1 < index2 && index1 != -1) || index2 == -1 ? index1 : index2;

                xml = xml.Substring(index);
                var endNodeIndex = xml.IndexOf("</" + nodeName, StringComparison.Ordinal);
                var endIndex = xml.Substring(endNodeIndex).IndexOf(">", StringComparison.Ordinal) + endNodeIndex + 1;
                res.Add(xml.Substring(0, endIndex));
                xml = xml.Substring(endIndex);
            }

            return res;
        }

        public static string GetSingleNode(string xml, string nodeName)
        {
            var list = GetNodes(xml, nodeName);
            return list.FirstOrDefault();
        }
    }
}
