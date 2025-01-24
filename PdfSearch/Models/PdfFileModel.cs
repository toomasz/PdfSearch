namespace PdfSearch.Models
{
    class PdfFileModel
    {
        public PdfFileModel(List<string> textFromPages)
        {
            TextFromPages = textFromPages;
        }

        public List<string> TextFromPages { get; }
    }
}
