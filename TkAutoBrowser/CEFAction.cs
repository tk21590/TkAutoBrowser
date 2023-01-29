using CefSharp;
using CefSharp.WinForms;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TkAutoBrowser
{
    public class CEFAction
    {
        public ChromiumWebBrowser browser { get; set; }
        public CEFAction(ChromiumWebBrowser browser)
        {
           
            this.browser = browser;
        }
        /// <summary>
        /// Gửi thông tin keyboard đến web theo dạng chữ
        /// </summary>
        /// <param name="sData"></param>
        /// <returns></returns>
        public async Task SendStringToBrowser(string sData)
        {

            var charArray = sData.ToCharArray();

            foreach (char c in charArray)
            {
                CefSharp.KeyEvent keyEvent = new KeyEvent();

                keyEvent.WindowsKeyCode = (int)c;
                keyEvent.FocusOnEditableField = true;
                keyEvent.IsSystemKey = false;
                keyEvent.Type = KeyEventType.Char;
                browser.GetBrowser().GetHost().SendKeyEvent(keyEvent);

                await Task.Delay(10);
            }
        }

        /// <summary>
        /// Chạy hàm javascript trả về 1 object hoặc null
        /// </summary>
        /// <param name="script"></param>
        /// <returns></returns>
        public async Task<object> Script_Object(string script)
        {
            string err_msg;

            try
            {
                var main = browser.GetMainFrame();
                if (main == null)
                {

                    return null;
                }
                JavascriptResponse task = await browser.GetMainFrame().EvaluateScriptAsync(script, null);
                if (task.Success)
                {
                    var respone = task.Result;
                    return respone;
                }
                else
                {
                    err_msg =  script + "\r\n" + task.Message;
                    TkHelp.Comment("script_obj:"+err_msg);
                    return null;

                }
            }
            catch (Exception errorr)
            {
                err_msg = "ERR_C:" + script + "\r\n" + errorr.Message;
                TkHelp.Comment("script_obj:" + err_msg);

                return null;
            }
        }

        /// <summary>
        /// Mô phỏng click chuột trái với vị trí chỉ định X và Y 
        /// </summary>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        public void Mouse_LeftClick(int X,int Y)
        {

            browser.GetBrowser().GetHost().SendMouseMoveEvent(X, Y, false, CefEventFlags.None);
            Thread.Sleep(50);

            browser.GetBrowser().GetHost().SendMouseClickEvent(X, Y, MouseButtonType.Left, false, 1, CefEventFlags.None);
            Thread.Sleep(60);
            browser.GetBrowser().GetHost().SendMouseClickEvent(X, Y, MouseButtonType.Left, true, 1, CefEventFlags.None);

        }


        public async Task<string> GetCookiesAsync(string url)
        {

            var cookieManager = browser.GetCookieManager();
            var visitor = new CookieCollector();

            cookieManager.VisitUrlCookies(url, true, visitor);

            var cookies = await visitor.Task; // AWAIT !!!!!!!!!

            var cookieHeader = CookieCollector.GetCookieHeader(cookies);
            return cookieHeader;
        }

        public bool AddCookiesJson(string url,string cookies)
        {
            browser.GetCookieManager().DeleteCookies();
            dynamic cookiesObject = JsonConvert.DeserializeObject<List<Cookie>>(cookies);
            for (int i = 0; i < cookiesObject.Count; i++)
            {
                Cookie cookie = cookiesObject[i];
                browser.GetCookieManager().SetCookie(url, cookie);

            }



            return true;
        }

    }

}
