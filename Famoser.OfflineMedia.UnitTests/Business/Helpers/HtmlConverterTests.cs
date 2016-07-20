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
            Assert.IsTrue(paragraphs[0].ParagraphType == ParagraphType.SecondaryTitle);
            Assert.IsTrue(paragraphs[1].ParagraphType == ParagraphType.Title);
            Assert.IsTrue(paragraphs[2].ParagraphType == ParagraphType.SecondaryTitle);
        }

        [TestMethod]
        public void HtmlSzenarioTitleCursivBoldUnderline()
        {
            //arrange
            var html = "<p>" +
                       "normal" +
                       "<b>bold</b>" +
                       "<strong>bold 2</strong>" +
                       "<em>bold 3</em>" +
                       "<i>cursives</i>" +
                       "<u>underline</u>" +
                       "end" +
                       "</p>";
            var converter = new HtmlConverter();

            //act
            var paragraphs = converter.HtmlToParagraph(html);

            //assert
            Assert.IsTrue(paragraphs.Count == 1);
            Assert.IsTrue(paragraphs[0].Children.Count == 7);
            Assert.IsTrue(paragraphs[0].Children[0].Text == "normal");
            Assert.IsTrue(paragraphs[0].Children[0].TextType == TextType.Normal);

            Assert.IsTrue(paragraphs[0].Children[1].Text == "bold");
            Assert.IsTrue(paragraphs[0].Children[1].TextType == TextType.Bold);

            Assert.IsTrue(paragraphs[0].Children[2].Text == "bold 2");
            Assert.IsTrue(paragraphs[0].Children[2].TextType == TextType.Bold);

            Assert.IsTrue(paragraphs[0].Children[3].Text == "bold 3");
            Assert.IsTrue(paragraphs[0].Children[3].TextType == TextType.Bold);

            Assert.IsTrue(paragraphs[0].Children[4].Text == "cursives");
            Assert.IsTrue(paragraphs[0].Children[4].TextType == TextType.Cursive);

            Assert.IsTrue(paragraphs[0].Children[5].Text == "underline");
            Assert.IsTrue(paragraphs[0].Children[5].TextType == TextType.Underline);

            Assert.IsTrue(paragraphs[0].Children[6].Text == "end");
            Assert.IsTrue(paragraphs[0].Children[6].TextType == TextType.Normal);
        }

        [TestMethod]
        public void HtmlSzenarioComplexCursivBoldUnderline()
        {
            //arrange
            var html = "<p>" +
                       "normal" +
                       "<b><i>bold & cursiv</i></b>" +
                       "<b><strong><i>bold & cursiv 2</i></strong></b>" +
                       "<b><strong><strong><strong><i>bold & cursiv 3</i></strong></strong></strong></b>" +
                       "end" +
                       "</p>";
            var converter = new HtmlConverter();

            //act
            var paragraphs = converter.HtmlToParagraph(html);

            //assert
            Assert.IsTrue(paragraphs.Count == 1);
            Assert.IsTrue(paragraphs[0].Children.Count == 5);
            Assert.IsTrue(paragraphs[0].Children[0].Text == "normal");
            Assert.IsTrue(paragraphs[0].Children[0].TextType == TextType.Normal);

            Assert.IsTrue(paragraphs[0].Children[1].Text == null);
            Assert.IsTrue(paragraphs[0].Children[1].TextType == TextType.Bold);
            Assert.IsTrue(paragraphs[0].Children[1].Children.Count == 1);

            Assert.IsTrue(paragraphs[0].Children[1].Children[0].Text == "bold & cursiv");
            Assert.IsTrue(paragraphs[0].Children[1].Children[0].TextType == TextType.Cursive);
            Assert.IsTrue(paragraphs[0].Children[1].Children[0].Children.Count == 0);

            Assert.IsTrue(paragraphs[0].Children[2].Text == null);
            Assert.IsTrue(paragraphs[0].Children[2].TextType == TextType.Bold);
            Assert.IsTrue(paragraphs[0].Children[2].Children.Count == 1);

            Assert.IsTrue(paragraphs[0].Children[2].Children[0].Text == "bold & cursiv 2");
            Assert.IsTrue(paragraphs[0].Children[2].Children[0].TextType == TextType.Cursive);
            Assert.IsTrue(paragraphs[0].Children[2].Children[0].Children.Count == 0);

            Assert.IsTrue(paragraphs[0].Children[3].Text == null);
            Assert.IsTrue(paragraphs[0].Children[3].TextType == TextType.Bold);
            Assert.IsTrue(paragraphs[0].Children[3].Children.Count == 1);

            Assert.IsTrue(paragraphs[0].Children[3].Children[0].Text == "bold & cursiv 3");
            Assert.IsTrue(paragraphs[0].Children[3].Children[0].TextType == TextType.Cursive);
            Assert.IsTrue(paragraphs[0].Children[3].Children[0].Children.Count == 0);

            Assert.IsTrue(paragraphs[0].Children[4].Text == "end");
            Assert.IsTrue(paragraphs[0].Children[4].TextType == TextType.Normal);
        }

        [TestMethod]
        public void HtmlSzenarioHyperlinks()
        {
            //arrange
            var html =
                "<p>Text before link <a href=\"http://link.ch\">link text</a> more text after link</p><p>Normal Content</p>";
            var converter = new HtmlConverter();

            //act
            var paragraphs = converter.HtmlToParagraph(html);

            //assert
            Assert.IsTrue(paragraphs.Count == 2);
            Assert.IsTrue(paragraphs[0].ParagraphType == ParagraphType.Text);
            Assert.IsTrue(paragraphs[0].Children.Count == 3);
            Assert.IsTrue(paragraphs[0].Children[0].Text == "Text before link ");
            Assert.IsTrue(paragraphs[0].Children[1].Text == "http://link.ch");
            Assert.IsTrue(paragraphs[0].Children[2].Text == " more text after link");

            Assert.IsTrue(paragraphs[0].Children[0].TextType == TextType.Normal);
            Assert.IsTrue(paragraphs[0].Children[0].Children.Count == 0);

            Assert.IsTrue(paragraphs[0].Children[1].TextType == TextType.Hyperlink);
            Assert.IsTrue(paragraphs[0].Children[1].Children.Count == 1);
            Assert.IsTrue(paragraphs[0].Children[1].Children[0].Text == "link text");
            Assert.IsTrue(paragraphs[0].Children[1].Children[0].TextType == TextType.Normal);
            Assert.IsTrue(paragraphs[0].Children[1].Children[0].Children.Count == 0);

            Assert.IsTrue(paragraphs[0].Children[2].TextType == TextType.Normal);
            Assert.IsTrue(paragraphs[0].Children[2].Children.Count == 0);
        }
        
        [TestMethod]
        public void HtmlSzenarioComplexHyperlinks()
        {
            //arrange
            var html ="<p>Text before link <a href=\"http://link.ch\">link text<b>Bold link text</b></a> more text after link</p>";
            var converter = new HtmlConverter();

            //act
            var paragraphs = converter.HtmlToParagraph(html);

            //assert
            Assert.IsTrue(paragraphs.Count == 1);
            Assert.IsTrue(paragraphs[0].Children[1].Text == "http://link.ch");
            Assert.IsTrue(paragraphs[0].Children[1].Children.Count == 2);
            Assert.IsTrue(paragraphs[0].Children[1].Children[0].Text == "link text");
            Assert.IsTrue(paragraphs[0].Children[1].Children[0].TextType == TextType.Normal);

            Assert.IsTrue(paragraphs[0].Children[1].Children[1].Text == "Bold link text");
            Assert.IsTrue(paragraphs[0].Children[1].Children[1].TextType == TextType.Bold);
        }
    }
}
