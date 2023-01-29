using CefSharp.WinForms;
using CefSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CefSharp.Handler;
using CefSharp.DevTools;
using TkAutoBrowser.Properties;
using System.Runtime.Remoting.Contexts;
using CefSharp.Internals;
using Newtonsoft.Json;
using System.Xml.Linq;
using System.Drawing;

namespace TkAutoBrowser
{
    public class CEFBrowser
    {
        public ChromiumWebBrowser browser;

        /// <summary>
        /// Link web chạy
        /// </summary>
        public string url = "https://www.google.com/";
        /// <summary>
        /// Control (groupbox hoặc panel đang điều khiển nó)
        /// </summary>
        public GroupBox thiscontrol { get; set; }
        public CEFAction CEFAction { get; set; }

        public Flag flag { get; set; }

        /// <summary>
        /// Cài đặt mặc định cần truyền vào 1 phương thức Flag chứa các cài đặt đc thiết lập và control là groupbox chứa browser
        /// </summary>
        /// <param name="flag">Cài đặt được thiết lập trước</param>
        /// <param name="thiscontrol">Control chứa browser</param>
        public CEFBrowser(Flag flag, GroupBox thiscontrol)
        {
            this.flag = flag;
            this.thiscontrol = thiscontrol;
        }

        /// <summary>
        /// Tạo browser và gán nó vào control đưa vào
        /// </summary>
        /// <param name="webrtc">tắt webrtc khi mở browser</param>
        /// <param name="main_control">main control được browser gán vào</param>
        /// <param name="lifeSpanHandler">Event handler thao tác vòng đời của browser</param>
        public void CreateBrowser(bool webrtc, Control main_control, out string log_err, ILifeSpanHandler lifeSpanHandler = null)
        {
            log_err = "";

            try
            {

                browser = new ChromiumWebBrowser(url);
                CEFAction = new CEFAction(browser);

                RequestContextSettings contextsetting = new RequestContextSettings();
                // string name_profile = TkHelp.RandomToken(5);

                string profileName = "CEFDATA\\Browser\\A_CefBrowser1";
                string profilePath = Path.GetFullPath(profileName);
                contextsetting.CachePath = profilePath;



                RequestContext context = new RequestContext(contextsetting);
                browser.RequestContext = context;



                browser.LifeSpanHandler = lifeSpanHandler;



                browser.RequestHandler = new Request(flag);


                //Tắt webtc
                if (webrtc)
                {
                    string err = "";

                    //browser.RequestContext.SetPreference("webrtc.multiple_routes_enabled", true, out err);
                    //log_err += "err 1:" + err + "\r\n";
                    //browser.RequestContext.SetPreference("webrtc.nonproxied_udp_enabled", true, out err);
                    //log_err += "err 2:" + err + "\r\n";
                    browser.RequestContext.SetPreference("webrtc.ip_handling_policy", "disable_non_proxied_udp", out err);
                    if (!string.IsNullOrEmpty(err))
                    {
                        log_err += "err set webrtc:" + err + "\r\n";

                    }

                }

                main_control.BeginInvoke((Action)delegate ()
                {
                    main_control.Controls.Add(browser);
                });


            }
            catch (Exception err)
            {
                log_err += "cactch err:" + err.Message + "\r\n";

            }
            browser.IsBrowserInitializedChanged += Browser_IsBrowserInitializedChanged;
            browser.LoadingStateChanged += Browser_LoadingStateChanged;
            //Chỉ có tác dụng với browser đầu tiên trong danh sách
            if (this == CEFStatic.list_cefBrowser.First())
            {

                browser.JavascriptMessageReceived += Browser_JavascriptMessageReceived;

            }
            browser.FrameLoadEnd += Browser_FrameLoadEnd;
        }

        private void Browser_LoadingStateChanged(object sender, LoadingStateChangedEventArgs e)
        {
            if (e.IsLoading)
            {
                thiscontrol.ForeColor = Color.White;

            }
        }

        private void Browser_FrameLoadEnd(object sender, FrameLoadEndEventArgs e)
        {
            if (e.Frame.IsMain)
            {
                if (this == CEFStatic.list_cefBrowser.First())
                {
                    browser.ExecuteScriptAsync(@"
	                                document.body.onmouseup = function(e)
	                                { 
                                     if(e.button==0){
                                      var objAsString = JSON.stringify({ x: e.pageX, y: e.pageY });
	                              	  CefSharp.PostMessage(objAsString);
                                      };
	                                }
	                              ");
                    thiscontrol.ForeColor = Color.Yellow;


                }
                else
                {
                    thiscontrol.ForeColor = Color.GreenYellow;

                }

            }
        }

        private void Browser_JavascriptMessageReceived(object sender, JavascriptMessageReceivedEventArgs e)
        {
            if (e.Message != null)
            {
                //Click chuột tại các vị trí chỉ định 
                if (CEFStatic.isMulti)
                {
                    TkHelp.Comment("Click chuột :" + e.Message.ToString());
                    string json = e.Message.ToString();
                    dynamic obj = JsonConvert.DeserializeObject(json);

                    int X = obj["x"];
                    int Y = obj["y"];

                    foreach (var groupBrowser in CEFStatic.list_cefBrowser)
                    {
                        if (groupBrowser != this)
                        {
                            Cef.UIThreadTaskFactory.StartNew(delegate
                            {
                                //TkHelp.Comment($"{groupBrowser.thiscontrol.Text} : click " + e.Message.ToString());
                                groupBrowser.CEFAction.Mouse_LeftClick(X, Y);

                            });
                        }

                    }

                }

            }
        }

        private void Browser_IsBrowserInitializedChanged(object sender, EventArgs e)
        {
            Cef.UIThreadTaskFactory.StartNew(async delegate
            {
                if (CEFStatic.isAgent)
                {
                    if (CEFStatic.list_agent.Count > CEFStatic.start_Agent)
                    {
                        flag.user_agent = CEFStatic.list_agent[CEFStatic.start_Agent];
                        using (DevToolsClient DTC = browser.GetDevToolsClient())
                        {
                            await DTC.Emulation.SetUserAgentOverrideAsync(flag.user_agent);
                        }
                    }
                    else
                    {
                        TkHelp.Comment($"Browser {flag.browser_index}: Useragent không có hoặc không đủ!");
                    }

                }


                if (CEFStatic.isProxy)
                {
                    try
                    {
                        if (CEFStatic.list_proxy.Count > CEFStatic.start_Proxy)
                        {
                            var proxy = CEFStatic.list_proxy[CEFStatic.start_Proxy];
                            CEFStatic.start_Proxy++;
                            var splitProxy = proxy.Split(':');
                            flag.host = splitProxy[0];
                            flag.port = int.Parse(splitProxy[1]);
                            flag.username = splitProxy[2];
                            flag.password = splitProxy[3];
                        }

                        else
                        {
                            TkHelp.Comment($"Browser {flag.browser_index}: Proxy không có hoặc không đủ");

                        }
                        var rc = browser.GetBrowser().GetHost().RequestContext;
                        var dict = new Dictionary<string, object>();
                        dict.Add("mode", "fixed_servers");
                        dict.Add("server", $"{flag.host}:{flag.port}");
                        string error = string.Empty;

                        var check = rc.SetPreference("proxy", dict, out error);

                    }
                    catch (Exception err)
                    {
                        TkHelp.Comment("Proxy err:" + err.Message);
                        return;
                    }
                }
                await browser.LoadUrlAsync(url);


                //browser.ExecuteScriptAsync(@"
                //                 document.body.onmousedown = function(e)
                //                 {
                //                      var objAsString = JSON.stringify({ x: e.pageX, y: e.pageY });
                //               	  CefSharp.PostMessage(objAsString);
                //                 }
                //               ");
                //await CEFAction.Script_Object(@"

                //                                function MouseLeftClick(p){
                //                                return {'X':p.pageX,'Y':p.pagepageY};

                //                                }
                //                                addEventListener('mousedown', MouseLeftClick, false);");



            });



        }


        #region HÀM HỖ TRỢ
        /// <summary>
        /// Xóa toàn bộ browser ra khỏi luồng điều khiển
        /// </summary>
        public void CloseBrowser()
        {
            browser.DestroyWindow();
            Control parentControl = thiscontrol.Parent;
            parentControl.BeginInvoke((Action)delegate ()
            {
                parentControl.Controls.Remove(thiscontrol);
            });
            thiscontrol.Dispose();
            browser.Dispose();
        }

        #endregion

    }
}
