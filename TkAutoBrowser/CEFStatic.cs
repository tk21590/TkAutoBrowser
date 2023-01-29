using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TkAutoBrowser
{
    public class CEFStatic
    {
        public static List<CEFBrowser> list_cefBrowser { get; set; } = new List<CEFBrowser>();

        public static List<string> list_proxy { get; set; } = new List<string>();
        public static List<string> list_agent { get; set; } = new List<string>();
        public static bool isProxy { get; set; }
        public static int start_Proxy { get; set; }
        public static int start_Agent { get; set; }
        public static bool isAgent { get; set; }
        public static bool isImage { get; set; }
        public static bool isWebRTC { get; set; }
        public static bool isMulti { get; set; } //Đồng bộ thao tác
        public static int MouseX { get; set; }
        public static int MouseY { get; set; }
    }
}
