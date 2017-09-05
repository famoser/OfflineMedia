using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Famoser.OfflineMedia.Business.Models;
using HtmlAgilityPack;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace Famoser.OfflineMedia.Utils.TamediaRefresher
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            var sourceModels = JsonConvert.DeserializeObject<List<SourceModel>>(inputTextBox.Text);
            EvaluateSources(new Stack<SourceModel>(sourceModels), new Queue<SourceModel>());
        }
        

        private void EvaluateSources(Stack<SourceModel> input, Queue<SourceModel> output)
        {
            if (!input.Any())
            {
                outputTextBox.Text = JsonConvert.SerializeObject(output, Formatting.Indented);
                return;
            }

            var first = input.Pop();
            output.Enqueue(first);

            //browser.Navigate(new Uri(first.LogicBaseUrl));
            browser.ScriptErrorsSuppressed = true;
            var text = browser.DocumentText;
            //the text does not contain the rendered html, only the retrieved one!
            browser.Navigate(new Uri(first.LogicBaseUrl));

            browser.DocumentTitleChanged += delegate
            {
                if (!browser.DocumentText.Contains("leftMenu"))
                {
                    return;
                }
                RefreshLinks(browser.DocumentText, first);
                EvaluateSources(input, output);
            };
        }

        private void RefreshLinks(string html, SourceModel source)
        {
            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(html);

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
                        var feedModel = new FeedModel()
                        {
                            Url = source.LogicBaseUrl + "api" + a.GetAttributeValue("href", null),
                            Name = a.InnerText,
                            Guid = Guid.NewGuid()
                        };
                        source.Feeds.Add(feedModel);
                    }
                }
            }
        }
    }
}
