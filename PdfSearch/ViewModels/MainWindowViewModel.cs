using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CsvHelper;
using PdfSearch.Models;
using PdfSearch.Services;
using Syncfusion.Pdf;
using Syncfusion.Windows.Forms.PdfViewer;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace PdfSearch.ViewModels;

internal partial class MainWindowViewModel :ObservableObject
{
    private readonly AppConfigService _configService;

    public MainWindowViewModel(AppConfigService configService)
    {
        _configService = configService;
        LoadConfig();
    }

    private void LoadConfig()
    {
        PdfFiles = new(_configService.Config.PdfFiles.Select(f => new PdfFileViewModel(f)));
        CsvFilePath = _configService.Config.CsvFile;
    }

    [RelayCommand]
    public void AddPdfFile()
    {
        var fileDialog = new OpenFileDialog
        { 
            Filter = "PDF Files (*.pdf)|*.pdf", 
            Multiselect = true
        };
        if(fileDialog.ShowDialog() == false)
        {
            return;
        }
        foreach(var pdfFile in fileDialog.FileNames)
        {
            if(PdfFiles.Any(f => f.FullPath == pdfFile))
            {
                continue;
            }
            PdfFiles.Add(new PdfFileViewModel(pdfFile));
        }
        _configService.Update(c => c.PdfFiles = PdfFiles.Select(f => f.FullPath).ToList());
    }

    [RelayCommand]
    public void RemovePdfFile(PdfFileViewModel pdfFileViewModel)
    {
        PdfFiles.Remove(pdfFileViewModel);
        _configService.Update(c => c.PdfFiles = PdfFiles.Select(f => f.FullPath).ToList());
    }

    [ObservableProperty]
    public partial string CsvFilePath { get; set; } = string.Empty;

    partial void OnCsvFilePathChanged(string value)
    {
        CsvFileName = Path.GetFileName(value);
        CsvFileSelected = !string.IsNullOrEmpty(value);
    }

    [RelayCommand]
    public void ClearCsvFile()
    {
        CsvFilePath = string.Empty;
    }

    [ObservableProperty]
    public partial string CsvFileName { get; set;} = string.Empty;

    [ObservableProperty]
    public partial bool CsvFileSelected { get; set; }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        if(e.PropertyName == nameof(CsvFilePath))
        {
            base.OnPropertyChanged(new PropertyChangedEventArgs(nameof(CsvFileName)));
        }
        base.OnPropertyChanged(e);
    }

    [RelayCommand]
    public void AddCsvFile()
    {
        var fileDialog = new OpenFileDialog
        {
            Filter = "CSV Files (*.csv)|*.csv",
            Multiselect = true
        };
        if (fileDialog.ShowDialog() == false)
        {
            return;
        }
        CsvFilePath = fileDialog.FileName;
        _configService.Update(c => c.CsvFile = CsvFilePath);
    }

    [RelayCommand]
    public void Search()
    {
        var listOfSearchCriteria = GetSearchCriteriaFromCsvFile(CsvFilePath);
        var results = new List<PdfSearchResultViewModel>();
        SearchResults.Clear();

        foreach (var pdfFile in PdfFiles)
        {
            using (PdfDocumentView pdfDocumentView = new())
            {
                pdfDocumentView.Load(pdfFile.FullPath);

                var textFromPages = new List<string>();
                for(int i =0; i < pdfDocumentView.PageCount; i++)
                {
                    string extractedText = pdfDocumentView.ExtractText(i, out TextLines textLines);
                    textFromPages.Add(extractedText);
                }

                var pdfModel = new PdfFileModel(textFromPages);

                foreach (var searchCriteria in listOfSearchCriteria)
                {
                    if (IsPdfMatchingSearchCriteria(pdfModel, searchCriteria))
                    {
                        SearchResults.Add(new PdfSearchResultViewModel(pdfFile.FullPath, searchCriteria));
                    }
                }
            }
        }
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

    private List<List<string>> GetSearchCriteriaFromCsvFile(string csvFile)
    {
        var listOfSearchCriteria = new List<List<string>>();
        using (var reader = new StreamReader(csvFile))
        using (var csv = new CsvParser(reader, CultureInfo.InvariantCulture))
        {
            while (csv.Read())
            {
                var searchTerms = csv.Record?.ToList();
                if(searchTerms != null)
                {
                    listOfSearchCriteria.Add(searchTerms);
                }
            }
        }
        return listOfSearchCriteria;
    }

    [ObservableProperty]
    public partial ObservableCollection<PdfFileViewModel> PdfFiles { get; set; } = new();

    [ObservableProperty]
    public partial ObservableCollection<PdfSearchResultViewModel> SearchResults { get; set; } = new();
}
