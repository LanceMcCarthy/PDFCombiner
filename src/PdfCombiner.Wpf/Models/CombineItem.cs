namespace PdfCombiner.Wpf.Models
{
    public class CombineItem : BindableBase
    {
        private string _displayName;
        private string _filePath;

        public string DisplayName
        {
            get => _displayName;
            set => SetProperty(ref _displayName, value);
        }

        public string FilePath
        {
            get => _filePath;
            set => SetProperty(ref _filePath, value);
        }
    }
}
