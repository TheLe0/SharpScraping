namespace Scrapping.Models
{
    public class SeleniumConfig
    {
        public string? UrlPage { get; set; }
        public int Timeout { get; set; }
        public int Headless { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? Captcha { get; set; }
    }
}
