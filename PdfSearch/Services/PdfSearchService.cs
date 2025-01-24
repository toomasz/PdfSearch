using PdfSearch.Models;
using Syncfusion.Pdf;
using Syncfusion.Windows.Forms.PdfViewer;

namespace PdfSearch.Services;

class PdfSearchService
{
    public List<PdfSearchResultModel> FindPdfFilesMatchingSearchCriteria(List<string> pdfFiles, List<List<string>> listOfSearchCriteria)
    {
        var searchResults = new List<PdfSearchResultModel>();

        var pdfModels = LoadPdfFiles(pdfFiles);

        foreach (var pdfModel in pdfModels)
        {
            foreach (var searchCriteria in listOfSearchCriteria)
            {
                if (IsPdfMatchingSearchCriteria(pdfModel, searchCriteria))
                {
                    searchResults.Add(new PdfSearchResultModel(pdfModel, searchCriteria));
                }
            }
        }   

        return searchResults;
    }

    private List<PdfFileModel> LoadPdfFiles(List<string> pdfFiles)
    {
        var pdfModels = new List<PdfFileModel>();
        foreach (var pdfFilePath in pdfFiles)
        {
            using (PdfDocumentView pdfDocumentView = new())
            {
                pdfDocumentView.Load(pdfFilePath);
                var textFromPages = new List<string>();
                for (int i = 0; i < pdfDocumentView.PageCount; i++)
                {
                    string extractedText = pdfDocumentView.ExtractText(i, out TextLines textLines);
                    textFromPages.Add(extractedText);
                }
                pdfModels.Add(new PdfFileModel(pdfFilePath, textFromPages));
            }
        }
        return pdfModels;
    }

    private bool IsPdfMatchingSearchCriteria(PdfFileModel model, List<string> searchCriteria)
    {
        foreach (var keyword in searchCriteria)
        {
            if (!IsKeywordFound(keyword, model))
            {
                return false;
            }
        }
        return true;
    }

    private bool IsKeywordFound(string keyword, PdfFileModel pdfModel)
    {
        for (int i = 0; i < pdfModel.TextFromPages.Count; i++)
        {
            if (pdfModel.TextFromPages[i].IndexOf(keyword, StringComparison.InvariantCultureIgnoreCase) != -1)
            {
                return true;
            }
        }
        return false;
    }
}
