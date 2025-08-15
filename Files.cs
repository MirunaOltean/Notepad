
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notepad__
{
    public class Files : INotifyPropertyChanged
    {
        private string title, content;
        private string defaultText;
        bool exists;

        public Files(int v)
        {

        }

        public string Title
        {
            get => title;
            set
            {
                title = value;
                NotifyPropertyChanged("Title");
            }
        }
        public string Content
        {
            get => content;
            set
            {
                content = value;
                NotifyPropertyChanged("Content");
            }
        }

        public bool Exists
        {
            get => exists;
            set
            {
                exists = value;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                if (propertyName == "Content")
                {
                    if (exists && title[title.Length - 1] != '*')
                    {
                        File.Move(".\\Saved Files\\" + title + ".txt", ".\\Unsaved Files\\" + title + ".txt");
                        File.WriteAllText(".\\Unsaved Files\\" + title + ".txt", Content);
                    }
                    else
                    {
                        File.WriteAllText(".\\Unsaved Files\\" + title.Substring(0,title.Length-1) + ".txt", Content);
                    }

                    if (title[title.Length - 1] != '*')
                    {
                        Title += "*";
                    }
                }
                else if (propertyName == "Title")
                {

                }

                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
