using System.Collections.Generic;

namespace OfflineMediaV3.Business.Sources.Tamedia.Models
{
    public class Feed
    {
        public Category category { get; set; }
        public Category list { get; set; }
    }

    public class Category
    {
        public string id { get; set; }
        public string name { get; set; }
        public string title { get; set; }
        public List<PageElement> page_elements { get; set; }
    }

    public class PageElement
    {
        public List<Article> articles { get; set; }
    }

    public class Article
    {
        public string id { get; set; }
        public int legacy_id { get; set; }
        public string title { get; set; }
        public string lead { get; set; }
        public string text { get; set; }

        public string picture_medium_url { get; set; }
        public string picture_high_url { get; set; }

        public int first_published_at { get; set; }
        
        public string source { get; set; }
        public string source_annotation { get; set; }

        public List<Author> authors { get; set; }

        public TopElement top_element { get; set; }

        public List<ArticleElement> article_elements { get; set; }
    }

    public class TopElement
    {
        public string id { get; set; }
        public string boxtype { get; set; }
        public string title { get; set; }
        public string caption { get; set; }
        public string refresh { get; set; }
        public string cache { get; set; }
        public Video video { get; set; }
        public string picture_url { get; set; }
        public object picture_zoomable_url { get; set; }
        public string picture_caption { get; set; }
        public string picture_source_annotation { get; set; }
        public object lead { get; set; }
        public Slideshow slideshow { get; set; }
    }

    public class Slideshow
    {
        public int id { get; set; }
        public string title { get; set; }
        public string lead { get; set; }

        public string picture_medium_url { get; set; }

        public List<Picture> pictures { get; set; }
    }

    public class Picture
    {
        public object title { get; set; }
        public string caption { get; set; }

        public string photographer { get; set; }
        public string url { get; set; }
    }

    public class Video
    {
        public string title;
        public string poster_picture_url;
        public string url;
        public string lead;
    }

    public class Author
    {
        public string name { get; set; }
    }

    public class ArticleElement
    {
        public string id { get; set; }
        //possbile: video, articles, tags, slideshow
        public string boxtype { get; set; }
        public string title { get; set; }
        public string caption { get; set; }

        public List<ArticlePreview> article_previews { get; set; }
        public Video video { get; set; }
        public Slideshow slideshow { get; set; }

        public string picture_url { get; set; }
        public string picture_caption { get; set; }
        public string lead { get; set; }
    }

    public class ArticlePreview
    {
        public string id { get; set; }
        public string legacy_id { get; set; }
        public string title { get; set; }
        public string lead { get; set; }
        public string text { get; set; }
       
        public string picture_medium_url { get; set; }

        public string picture_high_url { get; set; }
        
        public int first_published_at { get; set; }

        public List<Author> authors { get; set; }
    }
}
