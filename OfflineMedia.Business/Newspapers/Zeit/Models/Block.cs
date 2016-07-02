using System.Collections.Generic;
using System.Xml.Serialization;

namespace Famoser.OfflineMedia.Business.Newspapers.Zeit.Models
{
    [XmlRoot(ElementName = "block")]
    public class Block
    {
        [XmlElement(ElementName = "supertitle")]
        public string Supertitle { get; set; }
        [XmlElement(ElementName = "title")]
        public string Title { get; set; }
        [XmlElement(ElementName = "text")]
        public string Text { get; set; }
        [XmlElement(ElementName = "description")]
        public string Description { get; set; }
        [XmlElement(ElementName = "byline")]
        public string Byline { get; set; }
        [XmlElement(ElementName = "breadcrumb_title")]
        public string Breadcrumb_title { get; set; }
        [XmlElement(ElementName = "image")]
        public Image Image { get; set; }
        [XmlAttribute(AttributeName = "href")]
        public string Href { get; set; }
        [XmlAttribute(AttributeName = "uniqueId")]
        public string UniqueId { get; set; }
        [XmlAttribute(AttributeName = "year")]
        public string Year { get; set; }
        [XmlAttribute(AttributeName = "issue")]
        public string Issue { get; set; }
        [XmlAttribute(AttributeName = "ressort")]
        public string Ressort { get; set; }
        [XmlAttribute(AttributeName = "contenttype")]
        public string Contenttype { get; set; }
        [XmlAttribute(AttributeName = "publication-date")]
        public string Publicationdate { get; set; }
        [XmlAttribute(AttributeName = "expires")]
        public string Expires { get; set; }
        [XmlElement(ElementName = "author")]
        public List<Author> Author { get; set; }
        [XmlAttribute(AttributeName = "author")]
        public string _Author { get; set; }
        [XmlAttribute(AttributeName = "genre")]
        public string Genre { get; set; }
        [XmlAttribute(AttributeName = "ns0")]
        public string Ns0 { get; set; }
    }
}