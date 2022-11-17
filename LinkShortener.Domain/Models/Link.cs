#nullable disable

namespace LinkShortener.Domain.Models
{
    public partial class Link
    {
        public int Id { get; set; }
        public string LongUrl { get; set; }
        public string ShortUrl { get; set; }
        public DateTime DateCreate { get; set; }
        public int CountClick { get; set; }
    }
}
