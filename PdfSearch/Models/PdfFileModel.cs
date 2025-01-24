namespace PdfSearch.Models
{
    class PdfFileModel
    {
        public PdfFileModel(string filePath, List<string> textFromPages)
        {
            FilePath = filePath;
            TextFromPages = textFromPages;
        }
        public string FilePath { get; internal set; }
        public List<string> TextFromPages { get; }
    }
}
