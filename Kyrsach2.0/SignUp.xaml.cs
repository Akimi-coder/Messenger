using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Kyrsach2._0
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class SignUp : Window
    {
        public string Ip = "127.0.0.1";
        public SignUp()
        {
            InitializeComponent();
        }
        private void Label_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Register register = new Register();
            register.Owner = this;
            register.ShowDialog();
        }
        public static TcpClient tcpClient;
        public static User MyAccount;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {               
                string login = Login.Text;
                string password = Password.Password;
                User user = new User(login, password);
                SendMessage(tcpClient.GetStream(), $"<Login>{JsonSerializer.Serialize<User>(user)}");          
                var binnary = new BinaryFormatter();
                do
                {
                    Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        var mess = ReceiveMessage(tcpClient.GetStream());
                        MessageBox.Show(mess);
                        do

                        {
                            if (mess.Equals("No"))
                            {
                                Notification.Visibility = Visibility.Visible;
                            }
                            else if (mess.StartsWith("Yes<GetAvatar>"))
                            {
                                byte[] buffer = getData(tcpClient.GetStream());
                                mess = mess.Replace("Yes", "");
                                mess = mess.Replace("<GetAvatar>", "");
                                int i = FirstEntry(mess, ' ');
                                string filename = mess.Substring(0, i);
                                mess = mess.Substring(i).Trim();
                                string path = System.IO.Path.Combine(@"D:\KAVOMessangeR\MainUserInfo\", filename);
                                if (!File.Exists(path))
                                {
                                    using (var fss = new FileStream(path, FileMode.CreateNew, FileAccess.Write))
                                        fss.Write(buffer, 0, buffer.Length);
                                }
                                MyAccount = JsonSerializer.Deserialize<User>(mess);
                                MyAccount.Image = path;
                                Users code = new Users();
                                this.Close();
                                code.Show();
                            }
                            else if (mess.StartsWith("Yes<NoAvatar>"))
                            {
                                mess = mess.Replace("Yes<NoAvatar>", "");
                                MyAccount = JsonSerializer.Deserialize<User>(mess);
                                MessageBox.Show(MyAccount.ToString());
                                Users code = new Users();
                                code.Owner = this;
                                this.Close();
                                code.Show();
                            }
                        }
                        while (tcpClient.GetStream().DataAvailable);
                    });

                }
                while (tcpClient.GetStream().DataAvailable);

            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);             
            }
        }
        public int FirstEntry(string str, char c)
        {
            int i = 0;
            foreach (var v in str)
            {
                if (v.Equals(c))
                {
                    break;
                }
                i++;
            }
            return i;
        }
        public void Send(string str)
        {           
            var binnary = new BinaryFormatter();
            binnary.Serialize(tcpClient.GetStream(), str);
        }
        private void SendMessage(NetworkStream stream, string message)
        {
            try
            {
                string jsonString = JsonSerializer.Serialize(message);
                byte[] jsonBytes = Encoding.UTF8.GetBytes(jsonString);
                stream.Write(jsonBytes, 0, jsonBytes.Length);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка при надсиланні повідомлення: {ex.Message}");
            }
        }
        private string ReceiveMessage(NetworkStream stream)
        {
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                string jsonString = reader.ReadToEnd();
                return JsonSerializer.Deserialize<string>(jsonString);
            }

        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {            
            tcpClient = new TcpClient();
            while (!tcpClient.Connected)
            {
                try
                {

                    tcpClient.Connect(Ip, 9988);
                }
                catch {  }
            }

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
