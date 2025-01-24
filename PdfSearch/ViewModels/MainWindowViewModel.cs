using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Syncfusion.Pdf;
using Syncfusion.Windows.Forms.PdfViewer;
using System.Collections.ObjectModel;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace PdfSearch.ViewModels;

internal partial class MainWindowViewModel :ObservableObject
{
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
        var pdfFiles = fileDialog.FileNames;
        foreach(var pdfFile in pdfFiles)
        {
            PdfFiles.Add(pdfFile);
        }
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
        var csvFile = fileDialog.FileName;
    }

    [RelayCommand]
    public void Search()
    {
        PdfDocumentView pdfDocumentView = new();
        //Load the PDF file.
        pdfDocumentView.Load(PdfFiles[0]);
        string extractedText = pdfDocumentView.ExtractText(0, out TextLines textLines);
    }

    [ObservableProperty]
    public ObservableCollection<string> PdfFiles { get; } = new();
}
