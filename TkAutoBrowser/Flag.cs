using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TkAutoBrowser
{
    public class Flag
    {
        public int browser_index { get; set; } //Vị trí của browser

        public bool loadpage { get; set; } //đã vào được trang chủ trước
        public string host { get; set; } 
        public int port { get; set; } 
        public string username { get; set; } 
        public string password { get; set; } 
        public string user_agent { get; set; } 
        

    }
}
