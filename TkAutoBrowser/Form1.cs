using CefSharp;
using CefSharp.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TkAutoBrowser.Properties;

namespace TkAutoBrowser
{
    public partial class MainBrowser : Form
    {
        public MainBrowser()
        {
            InitializeComponent();
        }
        CEFHelp CEFHelp = new CEFHelp();
        TkHelp TkHelp = new TkHelp();
        private void MainBrowser_Load(object sender, EventArgs e)
        {
            TkHelp.textBox = textBox;
            CEFHelp.start_index = 1;
            CEFHelp.row_number = 4;

            toolTip1.SetToolTip(iconButton5, "Chọn proxy qua file txt theo cấu trúc:\r\nhost:port:username:password");
            toolTip1.SetToolTip(iconButton6, "Chọn danh sách useragent của bạn \r\nNếu không có toàn bộ browser sẽ sử dụng user agent mặc định");
            toolTip1.SetToolTip(cb_image, "Khi bấm chọn sẽ tắt hiển thị toàn bộ hình ảnh trên browser, \r\n" +
                "Tùy chọn này chỉ được thực hiện trong lần chạy đầu tiên \r\nNếu muốn thay đổi vui lòng restart tool");
            toolTip1.SetToolTip(cb_isWebrtc, "Bấm chọn để tắt webrtc của browser , tránh rò rĩ địa chỉ IP\r\n" +
                "Tùy chọn này chỉ được thực hiện trong lần chạy đầu tiên \r\nNếu muốn thay đổi vui lòng restart tool");


            toolTip1.SetToolTip(iconButton7, "Khi thực hiện save cookie , phần mềm sẽ tự động save đến thư mục File/Cookie/Browser_X.txt\r\n" +
                "Mỗi browser sẽ được lưu trên 1 file txt chứa cookie riêng");

            toolTip1.SetToolTip(iconButton8, "Load cookie cần thêm địa chỉ web cần load với đầy đủ https đầu, ví dụ https://shopee.vn/ \r\n" +
              "Mỗi browser sẽ xóa toàn bộ cookie cũ và dựa theo thư mục File/Cookie/Browser_X.txt mà bạn đã lưu sau đó tiến hành add cookie\r\n" +
              "Lưu ý : cookie của browser nào sẽ theo browser đó mà bạn đã đặt trước");

            toolTip1.SetToolTip(iconButton9, "Phiên bản này là phiên bản đầu tiên và trong giai đoạn thử nghiệm cho nên sẽ không tránh khỏi sai sót \r\n" +
                      "Mỗi phiên bản sẽ miễn phí hoàn toàn cho nên mong các bạn không mang đi bán với bất kì hình thức nào\r\n" +
                      "Liên hệ hỗ trợ (Kiệt):\r\n" +
                      "Sđt,zalo : 0888-055-888\r\n" +
                      "Facebook : facebook.com/dontcarenothing");

        }
        bool isFirstStart = false;
        private void button1_Click(object sender, EventArgs e)
        {
            if (!isFirstStart)
            {
                CEFHelp.DefaultSetting(CEFStatic.isImage, CEFStatic.isWebRTC, true, true, "Mozilla/5.0 (Windows NT 10.0; Win64; x64) CefBrowser 1.0");
                isFirstStart = true;
                cb_image.Enabled = false;
                cb_isWebrtc.Enabled = false;
            }

            CEFHelp.Control_Create(MainGroup);

        }
        private void button2_Click(object sender, EventArgs e)
        {
            foreach (var groupBrowser in CEFStatic.list_cefBrowser)
            {
                Cef.UIThreadTaskFactory.StartNew(delegate
                {
                    groupBrowser.CloseBrowser();
                });
            }
            CEFStatic.list_cefBrowser.Clear();
            TkHelp.Comment($"Đã xóa toàn bộ browser");

        }
        /// <summary>
        /// Tự động thêm browser khi có groupbox đc add vào
        /// </summary>
        private void MainGroup_ControlAdded(object sender, ControlEventArgs e)
        {
            if (e.Control is GroupBox)
            {
                try
                {
                    Cef.UIThreadTaskFactory.StartNew(delegate
                    {
                        string err = "";
                        var group = e.Control as GroupBox;
                        string proxy = "";
                        var flag = new Flag();
                        CEFBrowser cefBrowser = new CEFBrowser(flag, group);
                        CEFStatic.list_cefBrowser.Add(cefBrowser);

                        //cefBrowser.url = "https://rewards.bing.com/";
                        //cefBrowser.url = "https://www.etsy.com/";
                        cefBrowser.url = txt_url.Text;
                        Popup popup = new Popup();
                        cefBrowser.CreateBrowser(true, e.Control, out err, popup);
                        //TkHelp.Comment($"{err}");

                    });

                }
                catch (Exception err)
                {
                    TkHelp.Comment(err.Message);
                }

            }
        }

        private void textBox_TextChanged(object sender, EventArgs e)
        {
            textBox.SelectionStart = textBox.TextLength;
            textBox.ScrollToCaret();

        }

        private void MainGroup_SizeChanged(object sender, EventArgs e)
        {
            CEFHelp.Control_Resize(MainGroup);
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            TkHelp.CheckNumberic(e);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            CEFHelp.value = TkHelp.GetNumberic(textBox1.Text);
            TkHelp.Comment($"Mở {CEFHelp.value} browser");

        }
        private void txt_row_KeyPress(object sender, KeyPressEventArgs e)
        {
            TkHelp.CheckNumberic(e);

        }
        private void txt_row_TextChanged(object sender, EventArgs e)
        {
            CEFHelp.row_number = TkHelp.GetNumberic(txt_row.Text);
            TkHelp.Comment($"Mỗi hàng chứa {CEFHelp.row_number} browser");
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            CEFHelp.start_index = TkHelp.GetNumberic(textBox3.Text);
            TkHelp.Comment($"Browser sẽ bắt đầu từ {CEFHelp.start_index}");
        }

        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            TkHelp.CheckNumberic(e);

        }
        private void iconButton1_Click(object sender, EventArgs e)
        {
            foreach (var groupBrowser in CEFStatic.list_cefBrowser)
            {
                Cef.UIThreadTaskFactory.StartNew(delegate
                {
                    groupBrowser.browser.LoadUrl(txt_url.Text);
                });
            }
        }

        private void iconButton2_Click(object sender, EventArgs e)
        {
            foreach (var groupBrowser in CEFStatic.list_cefBrowser)
            {
                Cef.UIThreadTaskFactory.StartNew(delegate
                {
                    groupBrowser.browser.Reload(false);
                });
            }
        }

        private void iconButton3_Click(object sender, EventArgs e)
        {
            foreach (var groupBrowser in CEFStatic.list_cefBrowser)
            {
                Cef.UIThreadTaskFactory.StartNew(delegate
                {
                    if (groupBrowser.browser.CanGoBack)
                    {
                        groupBrowser.browser.Back();

                    }
                });
            }
        }

        private void iconButton4_Click(object sender, EventArgs e)
        {
            foreach (var groupBrowser in CEFStatic.list_cefBrowser)
            {
                Cef.UIThreadTaskFactory.StartNew(delegate
                {
                    if (groupBrowser.browser.CanGoForward)
                    {
                        groupBrowser.browser.Forward();

                    }
                });
            }
        }

        private void iconButton5_Click(object sender, EventArgs e)
        {
            var list = TkHelp.OpenDialogAndSelectFile();
            foreach (var proxy in list)
            {
                if (proxy.Split(':').Length == 4)
                {
                    CEFStatic.list_proxy.Add(proxy);
                }
            }

            TkHelp.Comment($"Load {CEFStatic.list_proxy.Count} proxy success!");


        }
        private void iconButton7_Click(object sender, EventArgs e)
        {
            foreach (var groupBrowser in CEFStatic.list_cefBrowser)
            {
                Cef.UIThreadTaskFactory.StartNew(async delegate
                {
                    string url = groupBrowser.browser.Address;
                    string PATHname = "File\\Cookie\\" + textBox4.Text;
                    if (!Directory.Exists(PATHname))
                    {
                        Directory.CreateDirectory(PATHname);

                    }
                    string profileName = PATHname + "\\" + groupBrowser.thiscontrol.Text + ".txt";
                    var cookie = await groupBrowser.CEFAction.GetCookiesAsync(url);
                    TkHelp.AddList(profileName, cookie);
                    TkHelp.Comment($"{groupBrowser.thiscontrol.Text} xuất cookie đến {PATHname} thành công !");

                });
            }
        }

        private void iconButton8_Click(object sender, EventArgs e)
        {
            string url = textBox2.Text;
            if (string.IsNullOrEmpty(url))
            {
                TkHelp.Comment($"Chưa điền url cho cookie");

                return;
            }
            foreach (var groupBrowser in CEFStatic.list_cefBrowser)
            {
                Cef.UIThreadTaskFactory.StartNew(delegate
                {

                    string profileName = "File\\Cookie\\" + groupBrowser.thiscontrol.Text + ".txt";
                    if (File.Exists(profileName))
                    {
                        var cookie = TkHelp.ReadList(profileName);
                        try
                        {
                            groupBrowser.CEFAction.AddCookiesJson(url, cookie.First());
                            groupBrowser.browser.LoadUrlAsync(url);
                            TkHelp.Comment($"Load cookie xong!");
                        }
                        catch (Exception err)
                        {
                            TkHelp.Comment($"Cookie {groupBrowser.thiscontrol.Text} có lỗi\r\nMã lỗi :{err.Message}");
                        }

                    }
                    else
                    {
                        TkHelp.Comment($"{groupBrowser.thiscontrol.Text} không có cookie!");

                    }


                });
            }
        }
        private void iconButton6_Click(object sender, EventArgs e)
        {
            CEFStatic.list_agent = TkHelp.OpenDialogAndSelectFile();
            TkHelp.Comment($"Load {CEFStatic.list_agent.Count} user agent success!");

        }
        private void cb_isProxy_CheckedChanged(object sender, EventArgs e)
        {
            CEFStatic.isProxy = cb_isProxy.Checked;
            if (CEFStatic.isProxy)
                TkHelp.Comment($"Auto proxy");
            else
                TkHelp.Comment($"Không sử dụng proxy");

        }

        private void cb_userAgent_CheckedChanged(object sender, EventArgs e)
        {
            CEFStatic.isAgent = cb_userAgent.Checked;
            if (CEFStatic.isAgent)
                TkHelp.Comment($"Auto user agent");
            else
                TkHelp.Comment($"Không sử dụng user agent");



        }

        private void cb_image_CheckedChanged(object sender, EventArgs e)
        {
            CEFStatic.isImage = cb_image.Checked;
            if (CEFStatic.isImage)
                TkHelp.Comment($"Tắt hiển thị hình ảnh");
            else
                TkHelp.Comment($"Hiển thị hình ảnh website");

        }

        private void cb_isWebrtc_CheckedChanged(object sender, EventArgs e)
        {
            CEFStatic.isWebRTC = cb_isWebrtc.Checked;
            if (CEFStatic.isImage)
                TkHelp.Comment($"Tắt webrtc");
            else
                TkHelp.Comment($"Mở webrtc");
        }

        private void cb_multi_CheckedChanged(object sender, EventArgs e)
        {
            CEFStatic.isMulti = cb_multi.Checked;
            if (CEFStatic.isMulti)
                TkHelp.Comment($"Mở đồng bộ thao tác");
            else
                TkHelp.Comment($"Tắt đồng bộ thao tác");
        }


    }
}
