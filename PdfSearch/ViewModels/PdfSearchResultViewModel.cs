using PdfSearch.Models;
using System.IO;

namespace PdfSearch.ViewModels;

class PdfSearchResultViewModel
{
    public PdfSearchResultViewModel(PdfFileModel pdfFile, List<string> searchCriteria)
    {
        PdfFile = pdfFile;
        SearchCriteria = searchCriteria;
    }
    public PdfFileModel PdfFile { get; }
    public string PdfFileName => Path.GetFileName(PdfFile.FilePath);
    public List<string> SearchCriteria { get; }
}
