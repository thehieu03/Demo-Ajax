namespace Entity.ModelRequest
{
    public class CreateNewsArticleRequest
    {
        public string? NewsTitle { get; set; }
        public string Headline { get; set; } = string.Empty;
        public string? NewsContent { get; set; }
        public string? NewsSource { get; set; }
        public short? CategoryId { get; set; }
        public bool NewsStatus { get; set; } = false;
        public List<int> TagIds { get; set; } = new List<int>();
    }
}
