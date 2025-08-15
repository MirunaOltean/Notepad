using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Notepad__
{
    public partial class MainWindow : Window
    {
        private readonly File_Functions FileFunctions;
        private SearchWindow mySecondWindow;
        private ReplaceWindow myThirdWindow;
        private About AboutPage;
        private Tuple<int, int> Index;
        private bool RightTab;
        private string searchedText;

        public ObservableCollection<Files> files { get; set; } = new ObservableCollection<Files> { };
        public MainWindow()
        {
            InitializeComponent();
            FileFunctions = new File_Functions();
            DataContext = this;
            RightTab = false;

            SetFiles(".\\Unsaved Files", "*");
            SetFiles(".\\Saved Files", "");
            TreeView();
            if (files.Count == 0)
            {
                string name = "";
                FileFunctions.New(ref name);

                Files myfiles = new Files(0)
                {
                    Title = name + "*",
                    Content = "",
                    Exists = false
                };
                files.Add(myfiles);
            }
            TabMenu.SelectedItem = files[0];
        }

        private void TreeView()
        {
            TreeViewItem myFirstDriver = new TreeViewItem
            {
                Header = "C:",
                Tag = "C:\\"
            };
            TreeViewItem mySecondDriver = new TreeViewItem
            {
                Header = "D:",
                Tag = "D:\\"
            };
            myTreeView.Items.Add(myFirstDriver);
            myTreeView.Items.Add(mySecondDriver);
        }
        private void myTreeView_Selected(object sender, RoutedEventArgs e)
        {
            TreeViewItem CurrentDirectory = myTreeView.SelectedItem as TreeViewItem;
            if (!Path.HasExtension(CurrentDirectory.Tag.ToString()))
            {
                CurrentDirectory.Items.Clear();
                FileFunctions.NewViewTree(CurrentDirectory);
            }
            else
            {
                string contents = "";
                string name = FileFunctions.OpenFromTreeView(CurrentDirectory.Tag.ToString(), ref contents);

                Files myfiles = new Files(0)
                {
                    Title = name,
                    Content = contents,
                    Exists = true
                };
                files.Add(myfiles);
            }
        }
        private void SetFiles(string folderPath, string star)
        {
            foreach (string file in Directory.EnumerateFiles(folderPath, "*.txt"))
            {
                string contents = File.ReadAllText(file);

                Files myfiles = new Files(4)
                {
                    Title = file.Substring(folderPath.Length + 1, file.Length - folderPath.Length - 5) + star,
                    Content = contents,
                };

                if (FileFunctions.FileExists(myfiles.Title))
                    myfiles.Exists = true;
                files.Add(myfiles);
            }
        }
        private void New_Click(object sender, RoutedEventArgs e)
        {
            string name = "";
            FileFunctions.New(ref name);

            Files myfiles = new Files(0)
            {
                Title = name + "*",
                Content = "",
                Exists = false
            };
            files.Add(myfiles);
        }
        private void Open_Click(object sender, RoutedEventArgs e)
        {
            string name = "", contents = "";
            FileFunctions.Open(ref contents, ref name);

            Files myfiles = new Files(0)
            {
                Title = name,
                Content = contents,
                Exists = true
            };
            files.Add(myfiles);
        }
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            string name;
            if (TabMenu.SelectedIndex != -1)
            {
                name = files[TabMenu.SelectedIndex].Title;


                if (FileFunctions.Save(files[TabMenu.SelectedIndex].Content, ref name))
                {
                    if (name[name.Length - 1] == '*')
                    {
                        name = name.Remove(name.Length - 1, 1);
                    }
                }

                files[TabMenu.SelectedIndex].Title = name;
                files[TabMenu.SelectedIndex].Exists = true;
            }
        }
        private void SaveAs_Click(object sender, RoutedEventArgs e)
        {
            string name = files[TabMenu.SelectedIndex].Title;
            FileFunctions.SaveAs(ref name, files[TabMenu.SelectedIndex].Content);
            files[TabMenu.SelectedIndex].Title = name;
            files[TabMenu.SelectedIndex].Exists = true;
        }
        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            //Yes=close the file after saving it
            //No=close the file without saving it
            //Cancel=no changes
            string name = files[TabMenu.SelectedIndex].Title;
            if (name[name.Length - 1] == '*')
            {
                MessageBoxResult result = MessageBox.Show("Do you want to save this file?",
                          "Confirmation", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    Save_Click(sender, e);
                    FileFunctions.UpdateFilePaths(files[TabMenu.SelectedIndex].Title);
                    _ = files.Remove(files[TabMenu.SelectedIndex]);
                }
                else if (result == MessageBoxResult.No)
                {
                    string fileName = files[TabMenu.SelectedIndex].Title;
                    File.Delete(".\\Unsaved Files\\" + fileName.Substring(0, fileName.Length - 1) + ".txt");
                    FileFunctions.UpdateFilePaths(files[TabMenu.SelectedIndex].Title);
                    _ = files.Remove(files[TabMenu.SelectedIndex]);
                }
            }
            else
            {
                File.Delete(".\\Saved Files\\" + files[TabMenu.SelectedIndex].Title + ".txt");
                FileFunctions.UpdateFilePaths(files[TabMenu.SelectedIndex].Title);
                _ = files.Remove(files[TabMenu.SelectedIndex]);
            }
        }
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.Close();
        }
        private void Find_Click(object sender, RoutedEventArgs e)
        {
            mySecondWindow = new SearchWindow();
            mySecondWindow.Show();
        }
        private void Replace_Click(object sender, RoutedEventArgs e)
        {
            myThirdWindow = new ReplaceWindow();
            myThirdWindow.Show();
        }
        private void ReplaceAll_Click(object sender, RoutedEventArgs e)
        {

        }
        private void About_Click(object sender, RoutedEventArgs e)
        {
            AboutPage = new About();
            AboutPage.Show();
        }
        private void UpdateSelectedWord(object sender, bool next)
        {
            if (RightTab == false)
            {
                if (next)
                {
                    Index = FileFunctions.RightIndexes();
                }
                else
                {
                    Index = FileFunctions.LeftIndexes();
                }
                if (Index.Item1 != -1 && Index.Item1 != -2)
                {
                    RightTab = true;
                    TabMenu.SelectedItem = files[Index.Item1];
                }
            }
            else if (RightTab)
            {
                RightTab = false;
                if (next)
                {
                    mySecondWindow.IsButtonClicked = false;
                }
                else
                {
                    mySecondWindow.IsButton2Clicked = false;
                }
                TextBox textBox = sender as TextBox;
                int index = textBox.Text.IndexOf(searchedText, Index.Item2);
                textBox.SelectionBrush = Brushes.DarkSeaGreen;
                if (index >= 0)
                {
                    textBox.Select(index, searchedText.Length);
                }
            }
        }
        private void textBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (mySecondWindow != null)
            {
                if (FileFunctions.CheckSearchedString(ref searchedText, mySecondWindow.myTxt.Text))
                {
                    if ((bool)mySecondWindow.CheckBox.IsChecked)
                    {
                        mySecondWindow.IsFindAllClicked = 1;
                    }

                    //check if search will be done in all files
                    if (mySecondWindow.IsFindAllClicked == 1)
                    {
                        mySecondWindow.IsFindAllClicked = 2;

                        //create the indexes list
                        FileFunctions.FindAll(searchedText, files, -1);
                    }
                    else if (mySecondWindow.IsFindAllClicked == 0)
                    {
                        FileFunctions.FindAll(searchedText, files, TabMenu.SelectedIndex);
                    }
                }

                //search next word
                if (mySecondWindow.IsButtonClicked)
                {
                    UpdateSelectedWord(sender, true);
                }
                //search previous word
                else if (mySecondWindow.IsButton2Clicked)
                {
                    UpdateSelectedWord(sender, false);
                }
            }

            if (myThirdWindow != null)
            {
                if (FileFunctions.CheckSearchedString(ref searchedText, myThirdWindow.mySearchTxt.Text))
                {
                    //if the searched word is different and checkboxes are clicked we reset the values
                    if ((bool)myThirdWindow.CurrentDocumentBox.IsChecked)
                    {
                        myThirdWindow.IsCurrentDocumentClicked = 1;
                    }
                    else myThirdWindow.IsCurrentDocumentClicked = 0;
                    if ((bool)myThirdWindow.AllDocumentsBox.IsChecked)
                    {
                        myThirdWindow.IsAllDocumentsClicked = 1;
                    }
                    else myThirdWindow.IsAllDocumentsClicked = 0;

                    //check if search will be done in all files
                    if (myThirdWindow.IsAllDocumentsClicked == 1)
                    {
                        myThirdWindow.IsAllDocumentsClicked = 2;
                        RightTab = false;

                        //create the indexes list
                        FileFunctions.FindAll(searchedText, files, -1);
                    }
                    //otherwise we search just in the current document
                    else if (myThirdWindow.IsAllDocumentsClicked == 0)
                    {
                        RightTab = false;
                        FileFunctions.FindAll(searchedText, files, TabMenu.SelectedIndex);
                    }
                }
                if (myThirdWindow.IsButtonClicked)
                {
                    //replace all appearances of searched string with new string in all documents
                    if (myThirdWindow.IsCurrentDocumentClicked == 1 && myThirdWindow.IsAllDocumentsClicked == 2)
                    {
                        myThirdWindow.IsCurrentDocumentClicked = 2;
                        FileFunctions.ReplaceAllDocuments(searchedText, myThirdWindow.myReplaceTxt.Text, files);
                    }
                    //replace one appearence of searched string in current document
                    else if (myThirdWindow.IsCurrentDocumentClicked == 0 && myThirdWindow.IsAllDocumentsClicked == 0)
                    {
                        if (RightTab == false)
                        {
                            Index = FileFunctions.RightIndexes();
                            if (Index.Item1 != -1 && Index.Item1 != -2)
                            {
                                RightTab = true;
                                TabMenu.SelectedItem = files[Index.Item1];
                            }
                        }
                        else if (RightTab)
                        {
                            RightTab = false;
                            myThirdWindow.IsButtonClicked = false;
                            int index = files[Index.Item1].Content.IndexOf(searchedText);
                            FileFunctions.ReplaceOneWord(searchedText, myThirdWindow.myReplaceTxt.Text, files, index, Index.Item1);
                        }
                    }
                    //replace all appearences in current document
                    else if (myThirdWindow.IsCurrentDocumentClicked == 1 && myThirdWindow.IsAllDocumentsClicked == 0)
                    {
                        ObservableCollection<Files> copy = new ObservableCollection<Files>();
                        copy.Add(files[0]);
                        myThirdWindow.IsCurrentDocumentClicked = 2;
                        FileFunctions.ReplaceAllDocuments(searchedText, myThirdWindow.myReplaceTxt.Text, copy);
                    }
                    //replace the appeareances one by one in all documents
                    else if (myThirdWindow.IsCurrentDocumentClicked == 0 && myThirdWindow.IsAllDocumentsClicked == 2)
                    {
                        if (RightTab == false)
                        {
                            Index = FileFunctions.RightIndexes();
                            FileFunctions.RightIndexes();
                            if (Index.Item1 != -1 && Index.Item1 != -2)
                            {
                                RightTab = true;
                                TabMenu.SelectedItem = files[Index.Item1];
                            }
                        }
                        else if (RightTab)
                        {
                            RightTab = false;
                            myThirdWindow.IsButtonClicked = false;
                            int index = files[Index.Item1].Content.IndexOf(searchedText);
                            FileFunctions.ReplaceOneWord(searchedText, myThirdWindow.myReplaceTxt.Text, files, index, Index.Item1);
                        }
                    }
                }
            }
        }

        ~MainWindow()
        {

        }

        private void Cut_Click(object sender, RoutedEventArgs e)
        {
            FileFunctions.Functions("Cut", -1);
        }
        private void Copy_Click(object sender, RoutedEventArgs e)
        {
            FileFunctions.Functions("Copy", -1);
        }
        private void Paste_Click(object sender, RoutedEventArgs e)
        {
            FileFunctions.Functions("Paste", -1);
        }
        private void textBox2_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            FileFunctions.CopyCutPaste(sender as TextBox);
        }
        private void ConvertUpperCase_Click(object sender, RoutedEventArgs e)
        {
            FileFunctions.Functions("UpperCase", -1);
        }
        private void ConvertLowerCase_Click(object sender, RoutedEventArgs e)
        {
            FileFunctions.Functions("LowerCase", -1);
        }
        private void ReadOnly_Click(object sender, RoutedEventArgs e)
        {
            FileFunctions.Functions("ReadOnly", -1);
        }
        private void GoToLine_Click(object sender, RoutedEventArgs e)
        {
            FileFunctions.Functions("GoToLine", int.Parse(Line.Text));
        }
        private void RemoveEmptyLines_Click(object sender, RoutedEventArgs e)
        {
            FileFunctions.Functions("RemoveEmptyLines", -1);
        }
    }
}
