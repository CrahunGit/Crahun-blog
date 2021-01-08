using System;

namespace WordDaze.Shared
{
    public class BlogPost
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public DateTime Posted { get; set; }
        public string Post { get; set; }
        public string ThumbnailImagePath { get; set; }
        public string PostSummary { get; set; }
    }
}
