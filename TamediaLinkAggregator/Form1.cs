using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HtmlAgilityPack;
using Newtonsoft.Json;
using OfflineMedia.Business.Helpers;
using OfflineMedia.Business.Models.Configuration;
using OfflineMedia.Business.Sources.Blick.Models;

namespace TamediaLinkAggregator
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            LoadPage();
        }

        private async void LoadPage()
        {
            var link = "http://mobile2.baz.ch";
            var html = await Download.DownloadStringAsync(new Uri(link));

            var doc1 = new HtmlAgilityPack.HtmlDocument();
            doc1.LoadHtml(html);

            var header = doc1.DocumentNode
                    .Descendants("head")
                    .FirstOrDefault();
            if (header != null)
            {
                //var newNode = HtmlNode.CreateNode("<meta http-equiv=\"X-UA-Compatible\" content=\"IE=edge\">");
                //header.PrependChild(newNode);
                var newNode = HtmlNode.CreateNode("<base href=\"" + link + "\">");
                header.PrependChild(newNode);

                html = doc1.DocumentNode.OuterHtml;


                webBrowser1.DocumentText = "0";
                webBrowser1.Document.OpenNew(true);
                webBrowser1.Document.Write(html);
                webBrowser1.Refresh();

            }
        }

        private bool _stopped = false;
        private async void button1_Click(object sender, EventArgs e)
        {
            var sourceModel = JsonConvert.DeserializeObject<List<SourceConfigurationModel>>(jsonInput.Text);

            foreach (var sourceConfigurationModel in sourceModel)
            {
                if (!sourceConfigurationModel.FeedConfigurationModels.Any())
                {
                    var html = await Download.DownloadStringAsync(new Uri(sourceConfigurationModel.LogicBaseUrl));

                    var doc1 = new HtmlAgilityPack.HtmlDocument();
                    doc1.LoadHtml(html);

                    var header = doc1.DocumentNode
                            .Descendants("head")
                            .FirstOrDefault();
                    if (header != null)
                    {
                        HtmlNode newNode = HtmlNode.CreateNode("<meta http-equiv=\"X-UA-Compatible\" content=\"IE=edge\">");
                        header.PrependChild(newNode);

                        html = doc1.DocumentNode.OuterHtml;


                        webBrowser1.DocumentText = "0";
                        webBrowser1.Document.OpenNew(true);
                        webBrowser1.Document.Write(html);
                        webBrowser1.Refresh();

                        _stopped = true;
                        

                        if (webBrowser1.Document != null)
                        {

                            var doc = new HtmlAgilityPack.HtmlDocument();
                            doc.LoadHtml(webBrowser1.Document.Body.OuterHtml);

                            List<HtmlNode> menu = doc.DocumentNode
                                .DescendantsAndSelf("aside")
                                .Where(
                                    o => o.GetAttributeValue("id", null) == "leftMenu"
                                )
                                .ToList();

                            if (menu.Count == 1)
                            {
                                var menuItems = menu.First().Descendants("li")
                                    .Where(
                                        o => o.GetAttributeValue("class", null) != "category"
                                    )
                                    .ToList();

                                foreach (var menuItem in menuItems)
                                {
                                    var a = menuItem.Descendants("a").FirstOrDefault();
                                    if (a != null)
                                    {
                                        var feedModel = new FeedConfigurationModel()
                                        {
                                            Url = a.GetAttributeValue("href", null),
                                            Name = a.InnerText,
                                            Guid = Guid.NewGuid()
                                        };
                                        sourceConfigurationModel.FeedConfigurationModels.Add(feedModel);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            jsonOutput.Text = JsonConvert.SerializeObject(sourceModel);
        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            _stopped = false;
        }
    }
}
