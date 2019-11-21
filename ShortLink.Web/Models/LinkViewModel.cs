using System;
using System.ComponentModel.DataAnnotations;

namespace ShortLink.Web.Models
{
    public class LinkViewModel
    {
        public int Id { get; set; }

        [Url(ErrorMessage = "Введите правильный URL")]
        [Required(ErrorMessage ="Ссылка не должна быть пустой")]
        public string LongUrl { get; set; }
        public string ShortUrl { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CountConversion { get; set; }
    }
}
