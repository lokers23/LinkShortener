using LinkShortener.Domain.Enums;

namespace LinkShortener.Domain.Response
{
    public class Response<T>
    {
        public T? Data { get; set; }
        public string? Message { get; set; }
        public StatusCode StatusCode{ get; set; }
    }
}
