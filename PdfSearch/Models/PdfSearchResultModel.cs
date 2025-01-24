namespace PdfSearch.Models;

class PdfSearchResultModel
{
    public PdfSearchResultModel(PdfFileModel pdfFile, List<string> searchCriteria)
    {
        PdfFile = pdfFile;
        SearchCriteria = searchCriteria;
    }
    public PdfFileModel PdfFile { get; }
    public List<string> SearchCriteria { get; }
}
