using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Kyrsach2._0
{


    [Serializable()]
    public class User : UserPanel
    {
        
        public User() { Messages = new ObservableCollection<Message>(); }

        public User(string nickName)
        {
            NickName = nickName;
            LastMessage = "";
            Messages = new ObservableCollection<Message>();
        }

        public User(string login, string password)
        {
            Login = login;
            Password = password;
            Email = "";          
        }

        public User(string nick, string login, string password, string email)
        {
            NickName = nick;
            Login = login;
            Password = password;
            Email = email;
            Messages = new ObservableCollection<Message>();
        }
       

        public override string ToString()
        {
            return $"{NickName} {Login} {Password} {Email}";
        }       

        public override bool Equals(object obj)
        {
            if (this == obj)
                return true;
            if(obj is User)            
                return obj.ToString().Equals(this.ToString());
            return false;
            
        }

      

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }
    }
}
