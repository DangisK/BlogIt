namespace VlogAPI.Data
{
    public class LinkDTO
    {
        // Url
        public string Href { get; set; }

        // What it does
        public string Rel { get; set; }

        // GET/PUT/POST
        public string Method { get; set; }
    }
}
