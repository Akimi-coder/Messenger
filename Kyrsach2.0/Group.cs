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
    public class Group : UserPanel
    {
        public ObservableCollection<User> Users { get; set; }           
        public Group()
        {
            Messages = new ObservableCollection<Message>();
            Users = new ObservableCollection<User>();
        }

    }
}
