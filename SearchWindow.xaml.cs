using System.Windows;
using System.Windows.Input;


namespace Notepad__
{
    public partial class SearchWindow : Window
    {
        public SearchWindow()
        {
            IsButtonClicked = false;
            IsButton2Clicked = false;
            IsFindAllClicked = 0;

            InitializeComponent();
        }

        public bool IsButtonClicked { get; set; }
        public bool IsButton2Clicked { get; set; }

        public int IsFindAllClicked { get; set; }


        private void TextBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            myTxt.Text = "";
        }
        private void IsFindAll_Checked(object sender, RoutedEventArgs e)
        {
            IsFindAllClicked = 1;
        }
        private void IsFindAll_Unchecked(object sender, RoutedEventArgs e)
        {
            IsFindAllClicked = 0;
        }
        ~SearchWindow()
        {

        }

        private void FindPrevious_Click(object sender, RoutedEventArgs e)
        {
            IsButton2Clicked = true;
        }

        private void FindNext_Click(object sender, RoutedEventArgs e)
        {
            IsButtonClicked = true;
        }
    }
}
