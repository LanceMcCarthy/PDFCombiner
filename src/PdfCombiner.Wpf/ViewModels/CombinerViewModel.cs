using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using PdfCombiner.Wpf.Models;
using Telerik.Windows.Controls;
using Telerik.Windows.Documents.Fixed.FormatProviders.Pdf.Streaming;

namespace PdfCombiner.Wpf.ViewModels
{
    public class CombinerViewModel : ViewModelBase
    {
        private readonly RadOpenFileDialog openFileDialog;
        private readonly RadSaveFileDialog saveFileDialog;
        private bool isSaveButtonEnabled;

        public CombinerViewModel()
        {
            openFileDialog = new RadOpenFileDialog { Owner = Application.Current.MainWindow, ExpandToCurrentDirectory = false };
            openFileDialog.Filter = "PDF Documents|*.pdf";
            openFileDialog.Multiselect = true;

            saveFileDialog = new RadSaveFileDialog { Owner = Application.Current.MainWindow, ExpandToCurrentDirectory = false };
            saveFileDialog.Filter = "PDF Documents|*.pdf";

            AddItemCommand = new DelegateCommand(AddItem);
            ClearCommand = new DelegateCommand(ClearItems);
            CombineItemsCommand = new DelegateCommand(CombineItems);

            ItemsToMerge.CollectionChanged += ItemsToMerge_CollectionChanged;
        }

        private void ItemsToMerge_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            IsSaveButtonEnabled = ItemsToMerge.Count > 1;
        }

        public ObservableCollection<CombineItem> ItemsToMerge { get; } = new ObservableCollection<CombineItem>();

        public bool IsSaveButtonEnabled
        {
            get => isSaveButtonEnabled;
            set => SetProperty(ref isSaveButtonEnabled, value);
        }

        public DelegateCommand AddItemCommand { get; set; }

        public DelegateCommand ClearCommand { get; set; }

        public DelegateCommand CombineItemsCommand { get; set; }

        private void AddItem(object obj)
        {
            openFileDialog.ShowDialog();
            
            if (openFileDialog.DialogResult == true)
            {
                foreach (var fileName in openFileDialog.FileNames)
                {
                    ItemsToMerge.Add(new CombineItem
                    {
                        DisplayName = Path.GetFileNameWithoutExtension(fileName),
                        FilePath = fileName
                    });
                }
            }
        }

        private void ClearItems(object obj)
        {
            ItemsToMerge.Clear();
        }

        private void CombineItems(object obj)
        {
            saveFileDialog.ShowDialog();

            if(saveFileDialog.DialogResult == true)
            {
                if (!Path.GetExtension(saveFileDialog.FileName).ToLower().Contains("pdf"))
                {
                    MessageBox.Show("You need to save the file as a PDF file.", "Invalid Filename");
                    return;
                }

                var result = CombineDocuments(saveFileDialog.FileName);

                if (result)
                {
                    MessageBox.Show($"You have merged {ItemsToMerge.Count} documents together.", "Success");

                    ItemsToMerge.Clear();
                }
            }
        }

        private bool CombineDocuments(string saveFilePath)
        {
            try
            {
                using (PdfStreamWriter fileWriter = new PdfStreamWriter(File.OpenWrite(saveFilePath)))
                {
                    foreach (var combineItem in ItemsToMerge)
                    {
                        var documentName = combineItem.FilePath;

                        using (PdfFileSource fileToMerge = new PdfFileSource(File.OpenRead(documentName)))
                        {
                            // Iterate through the pages of the current document 
                            foreach (PdfPageSource pageToMerge in fileToMerge.Pages)
                            {
                                // Append the current page to the fileWriter, which holds the stream of the result file 
                                fileWriter.WritePage(pageToMerge);
                            }
                        }
                    }
                }

                return true;
            }
            catch(Exception ex)
            {
                MessageBox.Show($"There was a problem combining the documents. Send Lance this error: {ex.Message}", "Error");
                return false;
            }
        }
    }
}
