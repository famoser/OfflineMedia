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
            //fix to use IE 11
            TryConfigureWebControl();
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            var sourceModels = JsonConvert.DeserializeObject<List<SourceModel>>(inputTextBox.Text);


            EvaluateSources(new Stack<SourceModel>(sourceModels), new Queue<SourceModel>());

        }


        private static void TryConfigureWebControl()
        {
            var appName = Process.GetCurrentProcess().ProcessName + ".exe";
            var ieval = 11000;

            RegistryKey regKey = null;
            try
            {
                regKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION", true);

                //If the path is not correct or 
                //If user't have priviledges to access registry 
                if (regKey == null)
                {
                    throw new Exception("failed setting WebControl to IE11");
                }

                string findAppKey = Convert.ToString(regKey.GetValue(appName));

                //Check if key is already present 
                if (findAppKey == "" + ieval)
                {
                    //already set
                    return;
                }

                //If key is not present or different from desired, add/modify the key , key value 
                regKey.SetValue(appName, ieval, RegistryValueKind.DWord);

                //check for the key after adding 
                findAppKey = Convert.ToString(regKey.GetValue(appName));

                if (findAppKey == "" + ieval)
                {
                    throw new Exception("please restart the application; all is fine :) (set the WebControl to IE11)");
                }
                throw new Exception("failed setting WebControl to IE11; current value is  " + ieval);
            }
            catch (Exception ex)
            {
                //throw new Exception("failed setting WebControl to IE11: " + ex.ToString());
            }
            finally
            {
                //Close the Registry 
                regKey?.Close();
            }
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
            browser.Navigate(new Uri("http://webdbg.com/ua.aspx"));
            browser.DocumentCompleted += delegate
            {
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
