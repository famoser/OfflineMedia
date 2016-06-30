using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using OfflineMedia.Business.Helpers;
using OfflineMedia.Business.Helpers.Text;

namespace OfflineMedia.Framework
{
    [TestClass]
    public class XmlHelperTests
    {
        [TestMethod]
        public void TestGetNodes()
        {
            var xml = "<xml><item><content>wdawda</content><content>wdawda</content><content>wdawda</content></item><item></item><item><content>wdawda</content></item><item><content>wdawda</content></item><item><content>wda  wda</content></item></xml>";
            var nodes = XmlHelper.GetNodes(xml, "item");
            Assert.IsTrue(nodes.Count == 5);
            Assert.IsTrue(nodes[0] == "<item><content>wdawda</content><content>wdawda</content><content>wdawda</content></item>");
            Assert.IsTrue(nodes[1] == "<item></item>");
            Assert.IsTrue(nodes[2] == "<item><content>wdawda</content></item>");
            Assert.IsTrue(nodes[3] == "<item><content>wdawda</content></item>");
            Assert.IsTrue(nodes[4] == "<item><content>wda  wda</content></item>");
        }
    }
}
