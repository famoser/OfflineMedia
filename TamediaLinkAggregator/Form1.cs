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
using TamediaLinkAggregator.Models;

namespace TamediaLinkAggregator
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private bool _stopped = false;
        private void button1_Click(object sender, EventArgs e)
        {
            var sourceModel = JsonConvert.DeserializeObject<List<ShortSourceConfigurationModel>>(jsonInput.Text);

            foreach (var sourceConfigurationModel in sourceModel)
            {
                var doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(htmlInput.Text);

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
                            o => o.GetAttributeValue("class", null) == "category"
                        )
                        .ToList();

                    foreach (var menuItem in menuItems)
                    {
                        var a = menuItem.Descendants("a").FirstOrDefault();
                        if (a != null)
                        {
                            var feedModel = new ShortFeedConfigurationModel()
                            {
                                Url = sourceConfigurationModel.LogicBaseUrl + "api" + a.GetAttributeValue("href", null),
                                Name = a.InnerText,
                                Guid = Guid.NewGuid()
                            };
                            sourceConfigurationModel.FeedConfigurationModels.Add(feedModel);
                        }
                    }
                }
            }
            jsonOutput.Text = JsonConvert.SerializeObject(sourceModel, Formatting.Indented);
        }
    }
}
