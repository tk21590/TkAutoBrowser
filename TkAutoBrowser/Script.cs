using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TkAutoBrowser
{
    public class Script
    {
        public static string GetId(string id)
        {
            return $"document.getElementById('{id}');";
        }  
        public static string GetClassName(string classname)
        {
            return $"document.getElementsByClassName('{classname}');";
        }
        public static string ClickId(string id)
        {
            return $"document.getElementById('{id}').click();";
        }
        public static string ClickClassName(string classname)
        {
            return $"document.getElementsByClassName('{classname}').click();";
        }
    }
}
