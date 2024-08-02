using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Text.Json;

namespace Kyrsach2._0
{
    /// <summary>
    /// Interaction logic for Register.xaml
    /// </summary>
    public partial class Register : Window
    {
        public string Ip = "127.0.0.1";
        public Register()
        {
            InitializeComponent();
        }

        int snum = 0;
        string nick;
        string password;
        string login;
        string email;
        private void Register_Click(object sender, RoutedEventArgs e)
        {
            
            nick = NickName.Text;
            password = Password.Password;
            login = Login.Text;
            email = Email.Text;
            Random random = new Random();
            snum = random.Next(1000, 10000);
            Task.Factory.StartNew(() => SendMessage(email, login, snum));
            RegisterGrid.Visibility = Visibility.Collapsed;
            CodeGrid.Visibility = Visibility.Visible;
            User user = new User(nick, login, password, email);            
        }
        public void Send(string str)
        {
            try
            {
                TcpClient tcpClient = new TcpClient();
                tcpClient = SignUp.tcpClient;
                var binnary = new BinaryFormatter();
                binnary.Serialize(tcpClient.GetStream(), str);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }       
        private static void SendMessage(string email, string login, int snum)
        {
            try
            {
                var fromAddress = new MailAddress("vladaxl9@gmail.com", "KAVO?");
                var toAddress = new MailAddress(email, "To User");
                const string fromPassword = "dutugi02r";
                const string subject = "KAVO?";
                string body = $"Dear:{login} your secret number is {snum}";
                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
                };
                using (var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body
                })
                    smtp.Send(message);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Check_Click(object sender, RoutedEventArgs e)
        {
            if (snum == Convert.ToInt32(Codes.Text))
            {
                User user = new User(nick, login, password, email);
                Send($"<Register>{JsonSerializer.Serialize<User>(user)}");
                this.Hide();
            }
            else
                MessageBox.Show("Wrong code");
        }
    }
}
