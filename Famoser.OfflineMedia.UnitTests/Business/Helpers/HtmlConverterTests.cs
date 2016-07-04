using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Famoser.OfflineMedia.Business.Enums.Models.TextModels;
using Famoser.OfflineMedia.Business.Helpers.Text;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace Famoser.OfflineMedia.UnitTests.Business.Helpers
{
    [TestClass]
    public class HtmlConverterTests
    {
        [TestMethod]
        public void HtmlSzenarioShortAndEasy()
        {
            //arrange
            var html = "<h1>Titel 1</h1>" +
                       "<h2>Secondary Titel 1</h2>" +
                       "<p>Content 1</p>";
            var converter = new HtmlConverter();

            //act
            var paragraphs = converter.HtmlToParagraph(html);

            //assert
            Assert.IsTrue(paragraphs.Count == 3);
            Assert.IsTrue(paragraphs[0].Children.Count == 1);
            Assert.IsTrue(paragraphs[0].ParagraphType == ParagraphType.Title);
            Assert.IsTrue(paragraphs[0].Children[0].Text == "Titel 1");
            Assert.IsTrue(paragraphs[0].Children[0].Children.Count == 0);
            Assert.IsTrue(paragraphs[0].Children[0].TextType == TextType.Normal);

            Assert.IsTrue(paragraphs[1].Children.Count == 1);
            Assert.IsTrue(paragraphs[1].ParagraphType == ParagraphType.SecondaryTitle);
            Assert.IsTrue(paragraphs[1].Children[0].Text == "Secondary Titel 1");
            Assert.IsTrue(paragraphs[1].Children[0].Children.Count == 0);
            Assert.IsTrue(paragraphs[1].Children[0].TextType == TextType.Normal);

            Assert.IsTrue(paragraphs[2].Children.Count == 1);
            Assert.IsTrue(paragraphs[2].ParagraphType == ParagraphType.Text);
            Assert.IsTrue(paragraphs[2].Children[0].Text == "Content 1");
            Assert.IsTrue(paragraphs[2].Children[0].Children.Count == 0);
            Assert.IsTrue(paragraphs[2].Children[0].TextType == TextType.Normal);
        }

        [TestMethod]
        public void HtmlSzenarioTitleCorrectlyResolved()
        {
            //arrange
            var html = "<h4>Secondary Titel 1</h4>" +
                       "<h3>Titel 1</h3>" +
                       "<h5>Secondary Titel 2</h5>";
            var converter = new HtmlConverter();

            //act
            var paragraphs = converter.HtmlToParagraph(html);

            //assert
            Assert.IsTrue(paragraphs.Count == 3);
            Assert.IsTrue(paragraphs[0].ParagraphType == ParagraphType.Title);
            Assert.IsTrue(paragraphs[1].ParagraphType == ParagraphType.SecondaryTitle);
            Assert.IsTrue(paragraphs[2].ParagraphType == ParagraphType.SecondaryTitle);
        }
    }
}
