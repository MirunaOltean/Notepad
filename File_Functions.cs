using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace Notepad__
{
    public class File_Functions
    {
        private readonly List<KeyValuePair<string, string>> filesPaths;
        private readonly List<bool> newFiles;
        private readonly List<Tuple<int, int>> rightIndexes, leftIndexes;
        private string functionName;
        private int lineNumber;
        public File_Functions()
        {

            rightIndexes = new List<Tuple<int, int>> { };
            leftIndexes = new List<Tuple<int, int>> { };

            string file = ".\\FilesPaths.txt";
            filesPaths = new List<KeyValuePair<string, string>>();
            if (File.Exists(file))
            {
                StreamReader Textfile = new StreamReader(file);
                string line;
                while ((line = Textfile.ReadLine()) != null)
                {
                    string name = line.Split('\\').Last();
                    name = name.Split('.').First();
                    filesPaths.Add(new KeyValuePair<string, string>(name, line));
                }
            }

            newFiles = new List<bool>(20);
            newFiles.AddRange(Enumerable.Repeat(false, 20));

            file = ".\\NewFiles.txt";
            if (File.Exists(file))
            {
                StreamReader Textfile = new StreamReader(file);
                string line;

                while ((line = Textfile.ReadLine()) != null && line != "")
                {
                    newFiles[line[line.Length - 1] - '0'] = true; ;
                    Console.WriteLine(line);
                }
                Textfile.Close();
            }

        }

        public void UpdateFilePaths(string name)
        {
            foreach (KeyValuePair<string, string> pair in filesPaths)
            {
                if (pair.Key == name)
                {
                    filesPaths.Remove(pair);
                    break;
                }
            }
        }
        public bool FileExists(string title)
        {
            foreach (KeyValuePair<string, string> mypath in filesPaths)
            {
                if (mypath.Key == title)
                {
                    return true;
                }
            }
            return false;
        }

        internal void NewViewTree(TreeViewItem CurrentDirectory)
        {
            List<string> Extensions = new List<string> { ".doc", ".docx", ".txt", ".pdf", ".xlsx" };
            foreach (FileInfo file in new DirectoryInfo(CurrentDirectory.Tag.ToString()).GetFiles())
            {
                if (Extensions.Contains(file.Extension))
                {
                    TreeViewItem AnotherItem = new TreeViewItem
                    {
                        Header = file.Name,
                        Tag = file.FullName
                    };
                    _ = CurrentDirectory.Items.Add(AnotherItem);
                }
            }
            foreach (DirectoryInfo folder in new DirectoryInfo(CurrentDirectory.Tag.ToString()).GetDirectories())
            {

                TreeViewItem AnotherItem = new TreeViewItem
                {
                    Header = folder.Name,
                    Tag = folder.FullName
                };
                _ = CurrentDirectory.Items.Add(AnotherItem);
            }
        }
        public string OpenFromTreeView(string path, ref string contents)
        {
            contents = File.ReadAllText(path);
            string name = path.Split('\\').Last();
            name = name.Split('.').First();
            filesPaths.Add(new KeyValuePair<string, string>(name, path));
            File.AppendAllText(".\\FilesPaths.txt", path + Environment.NewLine);
            File.AppendAllText(".\\Saved Files\\" + name + ".txt", contents);
            return name;
        }
        public bool Save(string contents, ref string name)
        {
            bool exist = false;
            if (name[name.Length - 1] == '*')
            {
                string fileName = name.Split('*').First();
                foreach (KeyValuePair<string, string> mypath in filesPaths)
                {
                    if (mypath.Key == fileName)
                    {
                        StreamWriter sw = new StreamWriter(mypath.Value);
                        sw.WriteLine(contents);
                        sw.Close();
                        File.Move(".\\Unsaved Files\\" + fileName + ".txt", ".\\Saved Files\\" + fileName + ".txt");
                        exist = true;
                    }
                }

                if (exist == false)
                {
                    SaveAs(ref name, contents);
                    if (name[name.Length - 1] == '*')
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        public void New(ref string name)
        {
            for (int index = 1; index < newFiles.Count; index++)
            {
                if (newFiles[index] == false)
                {
                    name = "New" + index;
                    newFiles[index] = true;
                    _ = File.Create(".\\Unsaved Files\\" + name + ".txt");
                    File.AppendAllText(".\\NewFiles.txt", name + Environment.NewLine);
                    break;
                }
            }
        }
        public void Open(ref string contents, ref string name)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                contents = File.ReadAllText(openFileDialog.FileName);
                name = openFileDialog.FileName.Split('\\').Last();
                name = name.Split('.').First();
                string p = Path.GetFullPath(openFileDialog.FileName).ToString();
                filesPaths.Add(new KeyValuePair<string, string>(name, p));
                File.AppendAllText(".\\FilesPaths.txt", p + Environment.NewLine);
                File.Copy(p, ".\\Saved Files\\" + name + ".txt");
            }
        }
        public void SaveAs(ref string name, string contents)
        {
            string copy = name;
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Text file (*.txt)|*.txt|C# file (*.cs)|*.cs",
                FileName = name,
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
            };
            if (saveFileDialog.ShowDialog() == true)
            {
                File.WriteAllText(saveFileDialog.FileName, contents);

                //adding the saved file path to the FilesPaths.txt
                File.AppendAllText(".\\FilesPaths.txt", saveFileDialog.FileName + Environment.NewLine);

                //updating the name in notepad too
                name = saveFileDialog.FileName.Split('\\').Last();
                name = name.Split('.').First();

                filesPaths.Add(new KeyValuePair<string, string>(name, saveFileDialog.FileName));
                if (copy.Contains("*"))
                {
                    File.Move(".\\Unsaved Files\\" + copy.Split('*').First() + ".txt", ".\\Saved Files\\" + name + ".txt");
                }
                File.WriteAllText(".\\Saved Files\\" + name + ".txt", contents);

                if (copy.StartsWith("New"))
                {
                    int x = int.Parse(copy[3].ToString());
                    newFiles[x] = false;
                }
            }
        }
        public bool CheckSearchedString(ref string text, string readText)
        {
            if (readText == "" || readText == "  Type something...")
            {
                return false;
            }
            if (readText == text)
            {
                return false;
            }
            text = readText;
            return true;
        }

        //return for each file all the start indexes of the world searched
        internal void FindAll(string text, ObservableCollection<Files> files, int index)
        {
            if (index == -1)
            {
                for (int count = 0; count < files.Count; count++)
                {
                    MatchCollection matches = Regex.Matches(files[count].Content, @"\w+");
                    foreach (Match m in matches)
                    {
                       // Console.WriteLine($"\"{m.Value}\" at {m.Index}, length {m.Length}");
                        if (m.Value.Contains(text))
                        {
                            string word = m.Value;
                            while (word.IndexOf(text[0]) >= 0)
                            {
                                if (word.Substring(word.IndexOf(text[0]), text.Length) == text)
                                {
                                    rightIndexes.Add(new Tuple<int, int>(count, m.Index + word.IndexOf(text)));
                                  //  rightIndexes.Add(new Tuple<int, int>(-2, -2));
                                    break;
                                }
                                word = word.Substring(word.IndexOf(text[0]));
                            }
                        }
                    }
                }
            }
            else
            {
                MatchCollection matches = Regex.Matches(files[index].Content, @"\w+");
                foreach (Match m in matches)
                {
                    Console.WriteLine($"\"{m.Value}\" at {m.Index}, length {m.Length}");
                    if (m.Value.Contains(text))
                    {
                        string word = m.Value;
                        while (word.IndexOf(text[0]) >= 0)
                        {
                            if (word.Substring(word.IndexOf(text[0]), text.Length) == text)
                            {
                                rightIndexes.Add(new Tuple<int, int>(index, m.Index + word.IndexOf(text)));
                               // rightIndexes.Add(new Tuple<int, int>(-2, -2));
                                break;
                            }
                            word = word.Substring(word.IndexOf(text[0]));
                        }
                    }
                }
            }
        }
        internal Tuple<int, int> RightIndexes()
        {
            if (rightIndexes.Count > 0)
            {
                Tuple<int, int> myIndex = rightIndexes[0];
                Console.WriteLine("11");
                Console.WriteLine(myIndex.Item1);
                Console.WriteLine(myIndex.Item2);
                Console.WriteLine("\n");
                leftIndexes.Add(myIndex);
                rightIndexes.RemoveAt(0);
                return myIndex;
            }
            return new Tuple<int, int>(-1, -1);
        }
        internal Tuple<int, int> LeftIndexes()
        {
            if (leftIndexes.Count > 0)
            {
                Tuple<int, int> myIndex = leftIndexes.Last();
                Console.WriteLine("22");
                Console.WriteLine(myIndex.Item1);
                Console.WriteLine(myIndex.Item2);
                Console.WriteLine("\n");
                rightIndexes.Insert(0, myIndex);
                leftIndexes.RemoveAt(leftIndexes.IndexOf(leftIndexes.Last()));
                return myIndex;
            }
            return new Tuple<int, int>(-1, -1);
        }
        internal void ReplaceAllDocuments(string text, string newtext, ObservableCollection<Files> files)
        {
            for (int count = 0; count < files.Count; count++)
            {
                files[count].Content = files[count].Content.Replace(text, newtext);
            }
        }
        internal void ReplaceOneWord(string searchedText, string newText, ObservableCollection<Files> files, int index, int fileIndex)
        {
            if (index >= 0)
            {
                if (newText != "" || newText != "  Type something...")
                {
                    string content = files[fileIndex].Content.Substring(0, index);
                    content += newText;
                    content += files[fileIndex].Content.Substring(index + searchedText.Length);
                    files[fileIndex].Content = content;
                }
            }
        }
        internal void Functions(string functionName, int lineNumber)
        {
            this.functionName = functionName;
            this.lineNumber = lineNumber;
        }
        internal void CopyCutPaste(TextBox box)
        {
            switch (functionName)
            {
                case "Cut":
                    if (box.SelectedText != "")
                    {
                        box.Cut();
                    }
                    break;
                case "Copy":
                    if (box.SelectionLength > 0)
                    {
                        box.Copy();
                    }
                    break;
                case "Paste":
                    if (Clipboard.GetDataObject().GetDataPresent(DataFormats.Text))
                    {
                        if (box.SelectionLength > 0)
                        {
                            if (MessageBox.Show("Do you want to paste over current selection?", "Warning", MessageBoxButton.YesNo) == MessageBoxResult.No)
                            {
                                box.SelectionStart += box.SelectionLength;
                            }
                        }
                        box.Paste();
                    }
                    break;
                case "UpperCase":
                    box.SelectedText = box.SelectedText.ToUpper();
                    break;
                case "LowerCase":
                    box.SelectedText = box.SelectedText.ToLower();
                    break;
                case "ReadOnly":
                    box.IsReadOnly = !box.IsReadOnly;
                    break;
                case "GoToLine":
                    if (lineNumber - 1 < box.LineCount)
                    {
                        int length = box.GetLineText(lineNumber - 1).Length;
                        box.SelectionStart = box.GetCharacterIndexFromLineIndex(lineNumber - 1);
                        box.Focus();
                        box.CaretIndex = box.SelectionStart;
                        box.SelectionLength = length;
                        box.ScrollToLine(lineNumber - 1);
                        box.Select(box.SelectionStart, box.SelectionLength);
                    }
                    break;
                case "RemoveEmptyLines":
                    box.Text = Regex.Replace(box.Text, @"^\s+$[\r\n]*", string.Empty, RegexOptions.Multiline);
                    break;
                default:
                    break;
            }
            functionName = "";
        }
        ~File_Functions()
        {
            //updating the NewFiles file with the remaining new files
            File.WriteAllText(".\\NewFiles.txt", "");
            for (int i = 0; i < 20; i++)
            {
                if (newFiles[i])
                {
                    File.AppendAllText(".\\NewFiles.txt", "New" + i + Environment.NewLine);
                }
            }
            //updating the paths of the left files
            File.WriteAllText(".\\FilesPaths.txt", "");
            for (int i = 0; i < filesPaths.Count(); i++)
            {
                    File.AppendAllText(".\\FilesPaths.txt", filesPaths[i].Value+ Environment.NewLine);
            }
        }

    }
}
