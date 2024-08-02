using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Interaction logic for User.xaml
    /// </summary>
    [Serializable()]
    public partial class Users : Window
    {
        public static ApplicationViewModels Application = new ApplicationViewModels();
        public static string fileName;
        User user = new User();
        public Users()
        {
            InitializeComponent();
            DataContext = Application;            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            Message message = (Message)btn.DataContext;
            Application.SendFile(message.File);
        }

       
    }
}
