namespace PdfSearch.Services
{
    internal class AppConfig
    {
        public List<string> PdfFiles { get; set; } = new();
        public string CsvFile { get; set; } = string.Empty;
    }
}
