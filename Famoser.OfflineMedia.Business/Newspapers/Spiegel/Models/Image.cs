﻿using System.Xml.Serialization;

namespace Famoser.OfflineMedia.Business.Newspapers.Spiegel.Models
{
    [XmlRoot(ElementName = "image")]
    public class Image
    {
        [XmlElement(ElementName = "title")]
        public string Title { get; set; }
        [XmlElement(ElementName = "link")]
        public string Link { get; set; }
        [XmlElement(ElementName = "url")]
        public string Url { get; set; }
    }
}