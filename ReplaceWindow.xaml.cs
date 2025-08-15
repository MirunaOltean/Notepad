using System.Windows;
using System.Windows.Input;

namespace Notepad__
{
    public partial class ReplaceWindow : Window
    {
        public ReplaceWindow()
        {
            InitializeComponent();
            IsButtonClicked = false;
            IsCurrentDocumentClicked = 0;
            IsAllDocumentsClicked = 0;
        }

        public bool IsButtonClicked { get; set; }
        public int IsCurrentDocumentClicked { get; set; }
        public int IsAllDocumentsClicked { get; set; }

        private void myReplaceTxt_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            myReplaceTxt.Text = "";
        }
        private void mySearchTxt_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            mySearchTxt.Text = "";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            IsButtonClicked = true;
        }

        private void CurrentDocument_Checked(object sender, RoutedEventArgs e)
        {
            IsCurrentDocumentClicked = 1;
        }
        private void AllDocuments_Checked(object sender, RoutedEventArgs e)
        {
            IsAllDocumentsClicked = 1;
        }
    }
}

