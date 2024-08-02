using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Kyrsach2._0
{
    public class Message : INotifyPropertyChanged
    {
        public string senderName;
        public string senderMessage;
        private ImageMessage image;
        private FileMessage file;
        public string Initial
        {
            get => SenderName.Substring(0, 2).ToUpper();
            set
            {
                senderName = value;
                OnPropertyChanged("Initial");
            }
        }
        private string imageUser;
        public string ImageUser
        {
            get => imageUser;
            set
            {
                imageUser = value;
                OnPropertyChanged("ImageUser");
            }
        }
     

        public bool IsUserImage
        {
            get => ImageUser == null;
            set
            {               
                OnPropertyChanged("IsUserImage");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }


        public string SenderName
        {

            get => senderName;
            set
            {
                senderName = value;
                OnPropertyChanged("SenderName");
            }
        }

        public ImageMessage Image
        {

            get => image;
            set
            {
                image = value;
                OnPropertyChanged("Image");
            }
        }

        public FileMessage File
        {

            get => file;
            set
            {
                file = value;
                OnPropertyChanged("File");
            }
        }

        public string SenderMessage
        {

            get => senderMessage;
            set
            {
                senderMessage = value;
                OnPropertyChanged("SenderMessage");
            }
        }

        

        private bool sendByMe;

        public bool SendByMe
        {
            get => sendByMe;
            set
            {
                sendByMe = value;
                OnPropertyChanged("SendByMe");
            }
        }       

        public bool HasMessage
        {
            get => SenderMessage != null;
            set
            {                
                OnPropertyChanged("HasMessage");
            }
        }

        public bool HasImageMessage
        {
            get => Image != null;
            set => OnPropertyChanged("HasImageMessage");
        }

        public bool HasFileMessage
        {
            get => File != null;
            set => OnPropertyChanged("HasFileMessage");
        }




    }
}
