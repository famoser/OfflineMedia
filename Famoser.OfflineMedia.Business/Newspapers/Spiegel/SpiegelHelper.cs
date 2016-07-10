﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Famoser.FrameworkEssentials.Logging;
using Famoser.OfflineMedia.Business.Helpers.Text;
using Famoser.OfflineMedia.Business.Models;
using Famoser.OfflineMedia.Business.Models.NewsModel;
using Famoser.OfflineMedia.Business.Models.NewsModel.ContentModels;
using Famoser.OfflineMedia.Business.Newspapers.Spiegel.Models;
using Famoser.OfflineMedia.Business.Repositories.Interfaces;
using HtmlAgilityPack;

namespace Famoser.OfflineMedia.Business.Newspapers.Spiegel
{
    public class SpiegelHelper : BaseMediaSourceHelper
    {
        public async Task<ArticleModel> FeedToArticleModel(Item nfa, FeedModel feedModel)
        {
            if (nfa == null || nfa.Link.Contains("/video/video"))
                return null;

            var title = nfa.Title.Substring(0, nfa.Title.IndexOf(":", StringComparison.Ordinal));
            var subTitle = nfa.Title.Substring(nfa.Title.IndexOf(":", StringComparison.Ordinal) + 2);
            var link = nfa.Link;
            if (link.Contains("#ref=rss"))
                link = link.Replace("#ref=rss", "");
            var a = ConstructArticleModel(feedModel);
            a.Title = title;
            a.SubTitle = subTitle;
            a.Teaser = nfa.Description;
            a.PublishDateTime = DateTime.Parse(nfa.PubDate);
            a.PublicUri = nfa.Link;
            a.LogicUri = link;


            if (nfa.Enclosure != null && nfa.Enclosure.Type != null && nfa.Enclosure.Type.Contains("image"))
            {
                var url = nfa.Enclosure.Url;
                if (url.Contains("thumbsmall"))
                {
                    var newurl = url.Replace("thumbsmall", "panoV9free");
                    HttpRequestMessage ms = new HttpRequestMessage(HttpMethod.Head, newurl);
                    using (var client = new HttpClient())
                    {
                        var res = await client.SendAsync(ms);
                        if (res.IsSuccessStatusCode)
                            url = newurl;
                        else
                            url = url.Replace("thumbsmall", "thumb");
                    }
                }
                a.LeadImage = new ImageContentModel { Url = url };
            }

            return a;
        }


        private string CleanHtml(string html)
        {
            /* clean this html:
            <p>Deutschland ist Handball-Europameister! Das Team von Trainer Dagur Sigurdsson gewann im Endspiel gegen Spanien 24:17 (10:6), überzeugte dabei wieder mal vor allem in der Abwehr. </p><p>
<div class="adition" id="content_ad_1" data-position="content_ad_1">
	<script type="text/javascript">
		if (typeof ADI != 'undefined') ADI.ad('content_ad_1');
	</script>
</div>
Damit hat sich Deutschland auch die Teilnahme an den Olympischen Spielen im Sommer in Rio de Janeiro gesichert. In der Vorrunde hatte die deutsche Mannschaft zum Auftakt noch <a href="/sport/sonst/handball-em-deutschland-verliert-gegen-spanien-a-1072409.html" title="29:32 gegen Spanien verloren" class="text-link-int lp-text-link-int">29:32 gegen Spanien verloren</a>. </p><p>Die Anfangsphase war geprägt von guter Defensivarbeit. So fiel innerhalb der ersten fünf Minuten nur ein einziger Treffer: Rune Dahmke erzielte die Führung für Deutschland (2. Minute). In der Folge agierte vor allem die deutsche Abwehr extrem sicher und souverän. Und wenn die spanische Offensive mal durchkam, reagierte Torwart Andreas Wolff immer wieder überragend. </p><p>Der nachnominierte Kai Häfner sorgte durch drei Tore für eine erste komfortable Führung (4:1). Spanien traf zunächst nur durch einen Siebenmeter und erzielte erst nach knapp zwölf Minuten sein zweites Tor - in Überzahl (5:2).</p><p>
<div class="adition" id="content_ad_2" data-position="content_ad_2">
	<script type="text/javascript">
		if (typeof ADI != 'undefined') ADI.ad('content_ad_2');
	</script>
</div>
Deutschland zog anschließend sogar auf 7:2 davon, musste danach aber immer wieder Unterzahlsituationen überstehen. Allein in der ersten Halbzeit kassierte das DHB-Team insgesamt fünf Zeitstrafen. Auch deshalb blieben die Deutschen zeitweise knapp neun Minuten ohne eigenen Treffer. Aber die Abschlüsse der Spanier blieben einfach zu unpräzise, und so kam der zweifache Weltmeister bis zur Pause nur sechsmal zum Torerfolg. So wenige Treffer hatte noch nie ein Team in der ersten Halbzeit eines EM-Finals erzielt. Kurz vor der Schlusssirene sorgte Julius Kühn für den 10:6-Pausenstand.</p><p>
<b>Wolff pariert knapp die Hälfte der spanischen Würfe</b>
</p><p>Auch in der zweiten Halbzeit fand Deutschland den besseren Start, erhöhte gut fünf Minuten nach Wiederbeginn auf 13:7. Spanien zeigte sich extrem verunsichert. Die Iberer verteidigten jetzt offensiver, blieben im Angriff aber weiter zu ungefährlich. Sowohl Joan Cañellas als auch Valero Rivera verwarfen dazu jeweils einen Siebenmeter. Und Wolff bildete weiter einen ganz sicheren Rückhalt für Deutschland - er wehrte insgesamt 48 Prozent der gegnerischen Würfe ab. Häfner, <a href="/sport/sonst/handball-em-kai-haefner-wirft-deutschland-ins-finale-a-1074808.html" title="Matchwinner aus dem Halbfinale gegen Norwegen" class="text-link-int lp-text-link-int">Matchwinner aus dem Halbfinale gegen Norwegen</a>, sorgte mit seinem Tor zum 16:9 erstmals für einen Sieben-Tore-Vorsprung. </p><p>Spanien, eigentlich <a href="/sport/sonst/handball-em-deutschland-will-spanien-im-finale-bezwingen-a-1074888.html" title="als Favorit ins Finale gegangen" class="text-link-int lp-text-link-int">als Favorit ins Finale gegangen</a>, wirkte zunehmend verzweifelt. Der Rückstand schien uneinholbar - vor allem, weil sich die Leistung der Spanier nicht steigerte. Unnötige Fehler und ungenaue Würfe brachten Deutschland immer wieder einfache Ballgewinne. Spätestens das 21:13 durch Dahmke bedeutete bereits in der 53. Minute die Vorentscheidung. Jetzt wurde es für das schwache Spanien, das noch nie einen Europameistertitel gewonnen hat, eine Blamage. Das Team von Trainer Manolo Cadenes gab sich in der Schlussphase auf. Deutschland zog noch auf 24:17 davon. Auch das ist Gegentorrekord bei einem EM-Finale in der 22-jährigen Geschichte des Wettbewerbs.</p><p><i>aev</i></p><p class="einestages-forum-info"><strong>Bitte beachten: Auf <i>einestages</i> können Hinweise nur unter Ihrem Klarnamen veröffentlicht werden. </strong></p><p>
	<strong>© SPIEGEL ONLINE 2016</strong><br>
		
			Alle Rechte vorbehalten<br>
			<a href="/extra/a-853891.html">Vervielfältigung nur mit Genehmigung der SPIEGELnet GmbH</a></p>
            */
            if (html.Contains("<strong>© SPIEGEL ONLINE 2016</strong>"))
            {
                html = html.Substring(0,
                    html.IndexOf("<strong>© SPIEGEL ONLINE 2016</strong>", StringComparison.Ordinal));
            }
            while (html.Contains("<div class=\"adition\""))
            {
                var part1 = html.Substring(0, html.IndexOf("<div class=\"adition\"", StringComparison.Ordinal));
                var part2 = html.Substring(html.IndexOf("<div class=\"adition\"", StringComparison.Ordinal));
                part2 = part2.Substring(part2.IndexOf("</div>", StringComparison.Ordinal) + "</div>".Length);
                html = part1 + part2;
            }
            return html;
        }

        public SpiegelHelper(IThemeRepository themeRepository) : base(themeRepository)
        {
        }

        public override Task<List<ArticleModel>> EvaluateFeed(FeedModel feedModel)
        {
            return ExecuteSafe(async () =>
            {

                var articlelist = new List<ArticleModel>();
                var feed = await DownloadAsync(feedModel);
                if (feed == null) return articlelist;

                feed = feed.Substring(feed.IndexOf(">", StringComparison.Ordinal));
                feed = feed.Substring(feed.IndexOf("<", StringComparison.Ordinal));
                feed = HtmlHelper.RemoveXmlLvl(feed);
                feed = feed.Replace("content:encoded", "content");

                var channel = XmlHelper.Deserialize<Channel>(feed);
                if (channel == null)
                    LogHelper.Instance.Log(LogLevel.Error,
                        "SpiegelHelper.EvaluateFeed failed: channel is null after deserialisation", this);
                else
                {
                    foreach (var item in channel.Item)
                    {
                        var article = await FeedToArticleModel(item, feedModel);
                        if (article != null)
                            articlelist.Add(article);
                    }
                }

                return articlelist;
            });
        }

        public override Task<bool> EvaluateArticle(ArticleModel articleModel)
        {
            return ExecuteSafe(async () =>
            {
                //await AddThemesAsync(nfa.Category);

                var article = await DownloadAsync(articleModel);
                var doc = new HtmlDocument();
                doc.LoadHtml(article);

                var articleColumn = doc.DocumentNode
                    .Descendants("div")
                    .FirstOrDefault(o => o.GetAttributeValue("id", null) != null &&
                                         o.GetAttributeValue("id", null).Contains("js-article-column"));

                var content = articleColumn
                    .Descendants("p");

                if (content != null)
                {
                    var html = content.Aggregate("", (current, htmlNode) => current + htmlNode.OuterHtml);
                    articleModel.Content.Add(new TextContentModel()
                    {
                        Content = HtmlConverter.CreateOnce().HtmlToParagraph(CleanHtml(html))
                    });
                    return true;
                }
                return false;
            });
        }
    }
}