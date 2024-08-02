using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Text.Json;
using Microsoft.Win32;
using System.Windows.Media.Imaging;
using System.IO;
using System.Windows.Input;
using System.Windows.Controls;
using System.Security.Cryptography;

namespace Kyrsach2._0
{
    [Serializable()]
    public class ApplicationViewModels : INotifyPropertyChanged
    {
        private UserPanel selectedUserPanel;       
        private User selectedFriend;
        private User selectedWantFriendUser;
        private RelayCommand sendCommand;
        private RelayCommand openSettingCommand;
        private RelayCommand showWindowCommand;
        private RelayCommand removeCommand;
        private RelayCommand removeUserCommand;
        private string searchFriend;
        private bool isSelected;
        private RelayCommand sendAddCommand;
        private bool isWantFriend;
        private int countWantFriend;
        private bool inputString;
        private RelayCommand openCommand;
        private RelayCommand setImageCommand;
        private RelayCommand addGroupCommand;
        
        public void SendFile(FileMessage message)
        {
            message.IsLoaded = false;
            formatter.Serialize(tcpClient.GetStream(), $"<GetFile><{MainUser.Id}> {message.FileName}");
        }
        
        public RelayCommand SetImageCommand
        {
            get
            {
                return setImageCommand ??
                  (setImageCommand = new RelayCommand(obj =>
                  {
                      OpenFileDialog op = new OpenFileDialog();
                      if (op.ShowDialog() == true)
                      {
                          MainUser.Image = op.FileName;
                          FileStream fs = File.Open(op.FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                          byte[] buffer = new byte[fs.Length];
                          int len = (int)fs.Length;
                          fs.Read(buffer, 0, len);
                          formatter.Serialize(tcpClient.GetStream(), $"<Avatar>{MainUser.Id}{Path.GetExtension(op.FileName)}");
                          sendData(buffer, tcpClient.GetStream());
                          
                      }
                  }));
            }
        }

        public RelayCommand AddGroupCommand
        {
            get
            {
                return addGroupCommand ??
                  (addGroupCommand = new RelayCommand(obj =>
                  {
                      Group group = new Group() { NickName = "291" };
                      group.Users.Add(MainUser);
                      if (SelectedUser is User user)
                      {
                          group.Users.Add(user);
                          group.Messages.Add(new Message() { SenderMessage = "Hello", SendByMe = false, SenderName = MainUser.NickName, ImageUser=MainUser.Image });
                          FriendUsers.Add(group);
                      }
                      
                     
                   
                  }));
            }
        }

        public RelayCommand RemoveUserCommand
        {
            get
            {
                return removeUserCommand ??
                  (removeUserCommand = new RelayCommand(obj =>
                  {
                      FriendUsers.Remove(SelectedUser);
                  }));
            }
        }



        public RelayCommand OpenCommand
        {
            get
            {
                return openCommand ??
                  (openCommand = new RelayCommand(obj =>
                  {
                      OpenFileDialog op = new OpenFileDialog();                                                           
                      if (op.ShowDialog() == true)
                      {                          
                          FileStream fs = File.Open(op.FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);                         
                          byte[] buffer = new byte[fs.Length];
                          int len = (int)fs.Length;
                          fs.Read(buffer, 0, len);
                          BinaryFormatter br = new BinaryFormatter();
                          FileInfo TheFile = new FileInfo(op.FileName);
                          if (Path.GetExtension(op.FileName).Equals(".png") || Path.GetExtension(op.FileName).Equals(".jpg"))
                          {                                                           
                              br.Serialize(tcpClient.GetStream(), $"<Photo><{MainUser.Id}.{SelectedUser.Id}> {TheFile.Name}");
                              sendData(buffer, tcpClient.GetStream());
                              SelectedUser.Messages.Add(new Message { Image = new ImageMessage { FileName=TheFile.Name, MThumbnaiURL = "http://anywhere", LocalFilePath = Convert.ToString(new BitmapImage(new Uri(op.FileName))) }, SendByMe = true, Initial = SelectedUser.Initial });
                              SelectedUser.LastMessage = "Image";                             
                          }
                          else
                          {
                              br.Serialize(tcpClient.GetStream(), $"<File><{MainUser.Id}.{SelectedUser.Id}> {TheFile.Name}");
                              sendData(buffer, tcpClient.GetStream());
                              SelectedUser.Messages.Add(new Message { SendByMe = true , File = new FileMessage{ FileName=TheFile.Name, LocalFilePath= op.FileName, MThumbnaiURL= "http://anywhere"}, Initial=SelectedUser.Initial });
                              SelectedUser.LastMessage = TheFile.Name;
                          }
                      }
                  }));
            }
        } 

        public RelayCommand OpenSettingCommand
        {
            get
            {

                return openSettingCommand ??
                    (openSettingCommand = new RelayCommand(obj =>
                    {
                        Setting setting = new Setting();
                        setting.ShowDialog();
                    }));
            }
        }
        public bool IsWantFriend
        {
            get => isWantFriend;
            set
            {
                isWantFriend = value;
                OnPropertyChanged("IsWantFriend");
            }
        }

        public bool InputString
        {
            get => inputString;
            set
            {
                inputString = value;
                OnPropertyChanged("InputString");
            }
        }


        public int CountWantFriend
        {
            get
            {
                if (countWantFriend == 0)
                    IsWantFriend = false;
                return countWantFriend;
            }
            set
            {

                countWantFriend = value;
                OnPropertyChanged("CountWantFriend");
            }
        }

        public RelayCommand SendAddCommand
        {
            get
            {

                return sendAddCommand ??
                    (sendAddCommand = new RelayCommand(obj =>
                    {
                        if (SelectedFriend != null)
                        {
                            formatter.Serialize(tcpClient.GetStream(), $"<Okey><{MainUser.Id}.{SelectedFriend.Id}> {JsonSerializer.Serialize<User>(MainUser)}");
                            FriendUsers.Add(SelectedFriend);
                            CountWantFriend--;
                            WantFriends.Remove(SelectedFriend);
                        }
                    }));
            }
        }
        public void sendData(byte[] data, NetworkStream stream)
        {
            int bufferSize = 1024;

            byte[] dataLength = BitConverter.GetBytes(data.Length);

            stream.Write(dataLength, 0, 4);

            int bytesSent = 0;
            int bytesLeft = data.Length;

            while (bytesLeft > 0)
            {
                int curDataSize = Math.Min(bufferSize, bytesLeft);

                stream.Write(data, bytesSent, curDataSize);

                bytesSent += curDataSize;
                bytesLeft -= curDataSize;
            }
        }

        public RelayCommand RemoveCommand
        {
            get
            {

                return removeCommand ??
                    (removeCommand = new RelayCommand(obj =>
                    {
                        if (SelectedFriend != null)
                        {
                            WantFriends.Remove(SelectedFriend);
                            CountWantFriend--;
                        }
                    }));
            }
        }

        public bool IsSelected
        {
            get => isSelected;
            set
            {
                isSelected = value;
                OnPropertyChanged("IsSelected");
            }
        }
        public User SelectedFriend
        {
            get { return selectedFriend; }
            set
            {
                selectedFriend = value;
                OnPropertyChanged("SelectedFriend");
            }
        }

        public User SelectedWantFriendUser
        {
            get => selectedWantFriendUser; 
            set
            {
                selectedWantFriendUser = value;
                OnPropertyChanged("SelectedWantFriendUser");
            }
        }


        public ObservableCollection<User> WantFriends { get; set; }
        public ObservableCollection<User> WatchFriends { get; set; }


        public RelayCommand ShowWindowCommand
        {
            get
            {

                return showWindowCommand ??
                    (showWindowCommand = new RelayCommand(obj =>
                    {
                        searchFriend = "";
                        ObservableCollection<User> users = new ObservableCollection<User>();
                        WatchFriends = users;
                        AddFriend friend = new AddFriend();
                        friend.Show();                      
                    }));
            }
        }

        private RelayCommand searchCommand;
        private RelayCommand addCommand;
        public RelayCommand AddCommand
        {
            get
            {

                return addCommand ??
                    (addCommand = new RelayCommand(obj =>
                    {
                        if (SelectedWantFriendUser != null)
                        {
                            if (!CheckUser(SelectedWantFriendUser))
                            {
                                var binnary = new BinaryFormatter();
                                binnary.Serialize(tcpClient.GetStream(), $"<AddFriend><{MainUser.Id}.{SelectedWantFriendUser.Id}> {JsonSerializer.Serialize<User>(MainUser)}");
                            }
                            else
                            {
                                MessageBox.Show("This user is your friend");
                            }
                        }

                    }));
            }
        }

        public RelayCommand SearchCommand
        {
            get
            {

                return searchCommand ??
                    (searchCommand = new RelayCommand(obj =>
                    {
                        for (int i = 0; i < WatchFriends.Count; i++)
                        {
                            WatchFriends.Remove(WatchFriends[i]);
                        }
                        if (!searchFriend.Equals(""))
                        {
                            var binnary = new BinaryFormatter();
                            var user = JsonSerializer.Serialize<User>(new User(searchFriend));
                            binnary.Serialize(tcpClient.GetStream(), $"<Find>{user}");
                        }
                    }));
            }
        }       
        public RelayCommand SendCommand
        {
            get
            {

                return sendCommand ??
                    (sendCommand = new RelayCommand(obj =>
                    {
                        if (!Chat.Equals(""))
                        {
                            BinaryFormatter binary = new BinaryFormatter();                                                     
                            binary.Serialize(tcpClient.GetStream(), $"<{MainUser.Id}.{SelectedUser.Id}> {Chat}");
                            SelectedUser.Messages.Add(new Message() { SenderMessage = $"{Chat}", SenderName = SelectedUser.Initial, SendByMe = true });
                            SelectedUser.LastMessage = Chat;
                            Chat = "";

                        }
                    }));
            }
        }
        static public byte[] RSAEncrypt(byte[] DataToEncrypt, RSAParameters RSAKeyInfo, bool DoOAEPPadding)
        {
            RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
            RSA.ImportParameters(RSAKeyInfo);


            return RSA.Encrypt(DataToEncrypt, DoOAEPPadding);
        }
        private string chat;
        public string Chat
        {
            get => chat ?? "";          
            set
            {
                chat = value;
                OnPropertyChanged("Chat");
            }
        }

        public string SearchFriend
        {
            get => searchFriend;
            set
            {
                searchFriend = value;
                OnPropertyChanged("SearchFriend");
            }
        }

        public ObservableCollection<UserPanel> FriendUsers { get; set; }

        public UserPanel SelectedUser
        {
            get
            {
                if (selectedUserPanel != null)
                {
                    IsSelected = true;
                    InputString = false;                    
                    return selectedUserPanel;
                }
                return selectedUserPanel;
            }
            set
            {
                selectedUserPanel = value;
                OnPropertyChanged("SelectedUser");
            }
        }
        TcpClient tcpClient = SignUp.tcpClient;    
        public User MainUser
        {
            get => SignUp.MyAccount;
            set
            {                
                OnPropertyChanged("MainUser");
            }
        }
        BinaryFormatter formatter;

        string message;
        public ApplicationViewModels()
        {
            Directory.CreateDirectory(@"C:\KAVOMessangeR");
            Directory.CreateDirectory(@"C:\KAVOMessangeR\Photo");
            Directory.CreateDirectory(@"C:\KAVOMessangeR\File");
            Directory.CreateDirectory(@"C:\KAVOMessangeR\MainUserInfo");
            IsSelected = false;
            InputString = true;
            WatchFriends = new ObservableCollection<User>();
            formatter = new BinaryFormatter();           
            WantFriends = new ObservableCollection<User>();
            FriendUsers = new ObservableCollection<UserPanel>();
            do
            {
                User obj = JsonSerializer.Deserialize<User>((string)formatter.Deserialize(tcpClient.GetStream()));               
                Application.Current.Dispatcher.Invoke(() =>
                {
                    FriendUsers.Add(obj);                    
                });
            }
            while (tcpClient.GetStream().DataAvailable);
            Task.Factory.StartNew(async () =>
            {
                while (true)
                {
                    if(!tcpClient.Connected)
                    {
                        MessageBox.Show("Disconnect");
                    }
                    message = (string)formatter.Deserialize(tcpClient.GetStream());                  
                    await Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                       
                        if (message.StartsWith("<YesHave>"))
                        {
                            message = message.Replace("<YesHave>", "");
                            User user = JsonSerializer.Deserialize<User>(message);
                            if (!user.Equals(MainUser))
                            {
                                WatchFriends.Add(user);
                            }
                        }
                        
                        else if (message.StartsWith("<Add>"))
                        {

                            message = message.Replace("<Add>", "");
                            int i = FirstEntry(message, ' ');
                            int id = Convert.ToInt32(message.Substring(0, i));
                            message = message.Substring(i);
                            User user = JsonSerializer.Deserialize<User>(message);
                            WantFriends.Add(user);
                            CountWantFriend++;
                            IsWantFriend = true;
                        }
                        else if (message.StartsWith("<Okey>"))
                        {
                            message = message.Replace("<Okey>", "");
                            User user = JsonSerializer.Deserialize<User>(message);
                            if (!CheckUser(user))
                            {
                                FriendUsers.Add(user);
                            }
                        }
                        else if (message.StartsWith("<TakeFile>"))
                        {
                            byte[] buffer = getData(tcpClient.GetStream());
                            Console.WriteLine(buffer.Length);
                            message = message.Replace("<TakeFile>", "");
                            string path = Path.Combine(@"D:\KAVOMessangeR\File\", message);
                            if (!File.Exists(path))
                            {
                                using (var fss = new FileStream(path, FileMode.CreateNew, FileAccess.Write))
                                    fss.Write(buffer, 0, buffer.Length);
                            }
                        }
                        else if (message.StartsWith("<Photo>"))
                        {
                            byte[] buffer = getData(tcpClient.GetStream());
                            message = message.Replace("<Photo>", "");
                            int i = FirstEntry(message, ' ');
                            string id = message.Substring(0, i).Trim('<', '>');
                            message = message.Substring(i).Trim();
                            string path = Path.Combine(@"D:\KAVOMessangeR\Photo", message);
                            if (!File.Exists(path))
                            {
                                using (var fss = new FileStream(path, FileMode.CreateNew, FileAccess.Write))
                                    fss.Write(buffer, 0, buffer.Length);
                            }
                            foreach (var v in FriendUsers)
                            {
                                if (v.Id == Convert.ToInt32(id))
                                {
                                    v.Messages.Add(new Message { Image = new ImageMessage { MThumbnaiURL = "http://anywhere", LocalFilePath = path }, ImageUser = v.Image, SendByMe = false, Initial = v.Initial });
                                    v.LastMessage = "Image";
                                }
                            }
                        }
                        else if (message.StartsWith("<File>"))
                        {
                            message = message.Replace("<File>", "");
                            int i = FirstEntry(message, ' ');
                            string id = message.Substring(0, i).Trim('<', '>');
                            message = message.Substring(i).Trim();
                            foreach (var v in FriendUsers)
                            {
                                if (v.Id == Convert.ToInt32(id))
                                {
                                    v.Messages.Add(new Message { File = new FileMessage { FileName = message }, ImageUser = v.Image, SendByMe = false, Initial = v.Initial });
                                    v.LastMessage = message;
                                }
                            }
                        }
                        else
                        {
                            int i = FirstEntry(message, ' ');
                            string id = message.Substring(0, i).Trim('<', '>');
                            message = message.Substring(i).Trim();
                            foreach (var v in FriendUsers)
                            {
                                if (v.Id == Convert.ToInt32(id))
                                {
                                    v.Messages.Add(new Message() { SenderMessage = message, ImageUser = v.Image, SenderName = v.Initial, SendByMe = false });
                                    v.LastMessage = message;
                                }
                            }

                        }


                    });

                }
            });
        }



        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        private bool CheckUser(User user)
        {
            foreach (var v in FriendUsers)
            {
                if (v.Equals(user))
                {
                    return true;
                }
            }
            return false;
        }

        public int FirstEntry(string str, char c)
        {
            int i = 0;
            foreach (var v in message)
            {
                if (v.Equals(' '))
                {
                    break;
                }
                i++;
            }
            return i;
        }
        public byte[] getData(NetworkStream Stream)
        {

            byte[] fileSizeBytes = new byte[4];
            int bytes = Stream.Read(fileSizeBytes, 0, 4);
            int dataLength = BitConverter.ToInt32(fileSizeBytes, 0);

            int bytesLeft = dataLength;
            byte[] data = new byte[dataLength];

            int bufferSize = 1024;
            int bytesRead = 0;

            while (bytesLeft > 0)
            {
                int curDataSize = Math.Min(bufferSize, bytesLeft);
                if (tcpClient.Available < curDataSize)
                    curDataSize = tcpClient.Available; //This saved me

                bytes = Stream.Read(data, bytesRead, curDataSize);

                bytesRead += curDataSize;
                bytesLeft -= curDataSize;
            }

            return data;
        }
    }

}
