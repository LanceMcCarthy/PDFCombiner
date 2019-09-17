using System.Windows.Controls;
using PdfCombiner.Wpf.ViewModels;

namespace PdfCombiner.Wpf.Views
{
    public partial class CombinerView : UserControl
    {
        public CombinerView()
        {
            InitializeComponent();
            this.DataContext = new CombinerViewModel(this);
        }
    }
}
