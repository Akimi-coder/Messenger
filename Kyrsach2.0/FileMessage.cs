using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Kyrsach2._0
{
    public class FileMessage : INotifyPropertyChanged
    {
        private string mThumbnaiURL;
        private string title;
        private string fileName;
        private long fileSize;
        private string localFilePath;
        private bool isLoaded;

        public bool IsLoaded
        {
            get => true;
            set
            {
                isLoaded = value;
                OnPropertyChanged("IsLoaded");
            }
        }

        public string LocalFilePath
        {
            get => localFilePath;
            set
            {
                localFilePath = value;
                OnPropertyChanged("LocalFilePath");
            }
        }

        public string FileName
        {
            get => fileName;
            set
            {
                fileName = value;
                OnPropertyChanged("FileName");
            }
        }

        public long FileSize
        {
            get => fileSize;
            set
            {
                fileSize = value;
                OnPropertyChanged("FileSize");
            }
        }

        public string Title
        {
            get => title;
            set
            {
                title = value;
                OnPropertyChanged("Title");
            }
        }

        public string MThumbnaiURL
        {
            get => mThumbnaiURL;
            set
            {
                if (value == mThumbnaiURL)
                    return;
                mThumbnaiURL = value;
                OnPropertyChanged("MThumbnaiURL");
            }
        }



        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
