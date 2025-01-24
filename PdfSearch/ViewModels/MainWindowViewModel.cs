using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CsvHelper;
using PdfSearch.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Windows;
using MessageBox = System.Windows.MessageBox;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace PdfSearch.ViewModels;

internal partial class MainWindowViewModel :ObservableObject
{
    private readonly AppConfigService _configService;
    private readonly PdfSearchService _pdfSearchService;

    public MainWindowViewModel(AppConfigService configService, PdfSearchService pdfSearchService)
    {
        _configService = configService;
        _pdfSearchService = pdfSearchService;
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
        try
        {
            if(string.IsNullOrEmpty(CsvFilePath))
            {
                MessageBox.Show("Please select a CSV file", "Search Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if(PdfFiles.Count == 0)
            {
                MessageBox.Show("Please add PDF files", "Search Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var listOfSearchCriteria = GetSearchCriteriaFromCsvFile(CsvFilePath);
            SearchResults.Clear();
            var results = _pdfSearchService.FindPdfFilesMatchingSearchCriteria(PdfFiles.Select(f => f.FullPath).ToList(), listOfSearchCriteria);
            foreach (var result in results)
            {
                SearchResults.Add(new PdfSearchResultViewModel(result.PdfFile, result.SearchCriteria));
            }
            if(SearchResults.Count == 0)
            {
                MessageBox.Show("No results found", "Search Results", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
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
