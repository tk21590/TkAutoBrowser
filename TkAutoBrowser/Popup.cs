using CefSharp;
using CefSharp.WinForms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TkAutoBrowser.Properties;

namespace TkAutoBrowser
{
    public class Popup : ILifeSpanHandler
    {

        public bool DoClose(IWebBrowser chromiumWebBrowser, IBrowser browser)
        {
            return false;
        }

        public void OnAfterCreated(IWebBrowser chromiumWebBrowser, IBrowser browser)
        {
         
           
        }

        public void OnBeforeClose(IWebBrowser chromiumWebBrowser, IBrowser browser)
        {
        }

        public bool OnBeforePopup(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, string targetUrl, string targetFrameName, WindowOpenDisposition targetDisposition, bool userGesture, IPopupFeatures popupFeatures, IWindowInfo windowInfo, IBrowserSettings browserSettings, ref bool noJavascriptAccess, out IWebBrowser newBrowser)
        {
            
            //windowInfo.X = 300;
            //windowInfo.Y = 800;
            //windowInfo.Width = 400;
            //windowInfo.Height = 600;

            //var popup_browser = (ChromiumWebBrowser)chromiumWebBrowser;
            //Cef.UIThreadTaskFactory.StartNew(delegate
            //{


            //});


            newBrowser = null;

            return false;

        }
    }
}
