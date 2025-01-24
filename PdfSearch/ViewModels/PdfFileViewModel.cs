using System.IO;

namespace PdfSearch.ViewModels
{
    internal class PdfFileViewModel
    {
        public PdfFileViewModel(string fullPath)
        {
            FullPath = fullPath;
        }
        public string FullPath { get; }
        public string Name => Path.GetFileName(FullPath);
    }
}
