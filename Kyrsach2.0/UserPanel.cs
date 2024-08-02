using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Kyrsach2._0
{
    public class UserPanel : INotifyPropertyChanged
    {
        public  int Id { get; set; }
        private string nickName;
        private string image;
        public  bool IsImage
        {
            get => Image == null;
            set => OnPropertyChanged("IsImage");
        }
        public  string Image
        {
            get => image;
            set
            {
                image = value;
                OnPropertyChanged("Image");
            }
        }
        public  ObservableCollection<Message> Messages { get; set; }
        private string lastMessage;
        public  string LastMessage
        {
            get => lastMessage;
            set
            {
                lastMessage = value;
                OnPropertyChanged("LastMessage");
            }
        }
        public  string NickName
        {

            get => nickName;
            set
            {
                nickName = value;
                OnPropertyChanged("NickName");
            }
        }
        public  string Initial
        {
            get
            {
                if (nickName != null)
                {
                    return nickName.Substring(0, 2).ToUpper();
                }
                return "";
            }
            set
            {
                OnPropertyChanged("Initial");
            }
        }
        
        public string Login { get; set; }
        public string Password { get; set; }
        public string email;

        public event PropertyChangedEventHandler PropertyChanged;

        public  string Email
        {

            get => email;
            set
            {
                email = value;
                OnPropertyChanged("Email");
            }
        }

        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

    }
}
