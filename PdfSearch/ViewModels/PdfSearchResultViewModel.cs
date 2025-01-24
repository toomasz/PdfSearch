using System.IO;

namespace PdfSearch.ViewModels;

class PdfSearchResultViewModel
{
    public PdfSearchResultViewModel(string pdfFilePath, List<string> searchCriteria)
    {
        PdfFilePath = pdfFilePath;
        SearchCriteria = searchCriteria;
    }
    public string PdfFilePath { get; }
    public string PdfFileName => Path.GetFileName(PdfFilePath);
    public List<string> SearchCriteria { get; }
}
