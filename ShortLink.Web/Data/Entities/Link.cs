using System;

namespace ShortLink.Web.Data.Entities
{
    public class Link
    {
        public int Id { get; set; }
        public string LongUrl { get; set; }
        public string ShortUrl { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CountConversion { get; set; }
    }
}
