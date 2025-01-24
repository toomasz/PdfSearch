using PdfSearch.ViewModels;
using System.Windows;

namespace PdfSearch;

public partial class App : System.Windows.Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        var mainWindow = new MainWindow
        {
            DataContext = new MainWindowViewModel()
        };
        mainWindow.Show();
        base.OnStartup(e);
    }
}
