namespace SeekProject.Models
{
    public class AnalyzeRequest
    {
        public string ImageBase64 { get; set; } = string.Empty;
        public string Prompt { get; set; } = string.Empty;
    }
}
