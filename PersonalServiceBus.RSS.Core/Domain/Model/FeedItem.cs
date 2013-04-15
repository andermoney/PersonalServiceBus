using System;

namespace PersonalServiceBus.RSS.Core.Domain.Model
{
    public class FeedItem : EntityBase
    {
        public string RssId { get; set; }
        public string Title { get; set; }
        public string FeedId { get; set; }
        public string Category { get; set; }
        public string Url { get; set; }
        public string Author { get; set; }
        public string Content { get; set; }
        public DateTime DatePublished { get; set; }
    }
}