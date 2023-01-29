using CefSharp;
using CefSharp.Handler;
using CefSharp.WinForms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TkAutoBrowser.Properties;

namespace TkAutoBrowser
{
    public class CEFHelp
    {
        public int start_index { get; set; } =0;
        public int value { get; set; } = 1;
        public int row_number { get; set; } = 3;
        /// <summary>
        /// Cài đặt mặc định cho browser khi mở lên
        /// </summary>
        /// <param name="image">Mở tắt hình ảnh hiển thị (true = tắt)</param>
        /// <param name="webrtc">Mở tắt webrtc  (true = tắt)</param>
        /// <param name="gpu_acceleration">Mở tắt gpu acceleration  (true = tắt)</param>
        /// <param name="closed_all">Đóng tất cả qui trình con nếu qui trình mẹ bị tắt  (true = tắt)</param>
        /// <param name="defaul_useragent">useragent mặc định của browser</param>
        public void DefaultSetting(bool image, bool webrtc, bool gpu_acceleration, bool closed_all, string defaul_useragent)
        {
            var bitness = Environment.Is64BitProcess ? "x64" : "x86";
            var version = string.Format("Chromium: {0}, CEF: {1}, CefSharp: {2}",
             Cef.ChromiumVersion, Cef.CefVersion, Cef.CefSharpVersion);

            //Cài đặt chế độ hỗ trợ x86/64
            CefRuntime.SubscribeAnyCpuAssemblyResolver();
            //Kích hoạt dpi - thông tin thêm tại https://github.com/cefsharp/CefSharp/wiki/General-Usage#high-dpi-displayssupport
            Cef.EnableHighDPISupport();

            //Phần Setting Browser
            var cef_path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CEFDATA\\cef_data");
            var cefset = new CefSettings()
            {
                LogFile = Path.Combine(cef_path, "cef_log.txt"),
                CachePath = Path.Combine(cef_path, "cache"),
                UserDataPath = Path.Combine(cef_path, "userdata")
            };

            //Tắt  WebRTC
            if (webrtc)
                cefset.CefCommandLineArgs.Add("disable-media-stream");
            //Tắt tăng tốc phần cứng gpu acceleration
            if (gpu_acceleration)
                cefset.CefCommandLineArgs.Add("disable-gpu");

            //Tắt hình ảnh trên browser
            if (image)
                cefset.CefCommandLineArgs.Add("disable-image-loading", "1");


            cefset.UserAgent = defaul_useragent;

            Cef.Initialize(cefset, performDependencyCheck: true, browserProcessHandler: null);

            //Thoát quy trình con nếu quy trình mẹ bị tắt
            CefSharpSettings.SubprocessExitIfParentProcessClosed = closed_all;
        }

        /// <summary>
        /// Tạo control và thêm vào main control với số hàng ngang theo ý
        /// </summary>

        /// <param name="main_control">Control chính dùng để thêm các control phụ vào</param>
        public List<GroupBox> Control_Create(Control main_control)
        {
            List<GroupBox> groupBoxes = new List<GroupBox>();
            int SoDuRa = value % row_number;
            int SoHang = (value / row_number);
            if (value % row_number != 0) //Nếu chia ra dư , thì số hàng phải tăng thêm 1  VD : 13%5 == 3 dư 3 >> Số hàng 3
            {
                SoHang = SoHang + 1;
            }

            /// Vị trí được tính từ góc trái của khung X 
            int Width = main_control.Width / value; // Chiều Ngang Khung Nếu số Hàng là 1 thì chiều ngang = Chiều Ngang Tổng / Số AccThread , Nếu Số Hàng 2 thì chia cho 5
            if (value >= row_number)
            {
                Width = main_control.Width / row_number;
            }
            int Height = main_control.Height / SoHang; // Chiều Cao Khung Nếu Số Hàng là 1 thì chiều cao khung  = ycao , nếu hàng là 2 thì chia ra

            int FixHeight = SoHang - 1;

            for (int i = 0; i < value; i++)
            {
                //if (Static.cbb_Proxy_Index == 0 && i > 0)
                //{
                //    Thread.Sleep(2000);
                //}
                int val = i + 1;
                int n = i / row_number;

                if (i >= n * row_number && i < (n + 1) * row_number)
                {
                    FixHeight = n;
                    val = val - n * row_number;
                }



                int viTriX = (val - 1) * Width;
                int viTriY = FixHeight * Height;




                string name_control = "Browser_" + start_index;
                start_index++;



                GroupBox control = new GroupBox();
                control.Size = new Size(Width, Height);
                // group.Size = new Size(Width / 2, Height / 2); //Thử Nghiệm

                control.Location = new Point(viTriX, viTriY); //X =  Vị trí góc trái đầu tiên phải là 0 TRỤC X NGANG , Y = TRỤC Y ĐỨNG
                control.Name = name_control;
                control.Text = name_control;
                control.ForeColor = Color.White;
                main_control.BeginInvoke((Action)delegate ()
                {
                    main_control.Controls.Add(control);

                });
                groupBoxes.Add(control);
            }
            return groupBoxes;
        }
        /// <summary>
        /// Resize lại browser hiển thị khi phóng to thu nhỏ MainControl
        /// </summary>
          /// <param name="main_control">Control chính dùng để thêm các control phụ vào</param>
        public void Control_Resize(Control main_control)
        {
            int SoDuRa = value % row_number;
            int SoHang = (value / row_number);
            if (value % row_number != 0) //Nếu chia ra dư , thì số hàng phải tăng thêm 1  VD : 13%5 == 3 dư 3 >> Số hàng 3
            {
                SoHang = SoHang + 1;
            }

            /// Vị trí được tính từ góc trái của khung X 
            int Width = main_control.Width / value; // Chiều Ngang Khung Nếu số Hàng là 1 thì chiều ngang = Chiều Ngang Tổng / Số AccThread , Nếu Số Hàng 2 thì chia cho 5
            if (value >= row_number)
            {
                Width = main_control.Width / row_number;
            }
            int Height = main_control.Height / SoHang; // Chiều Cao Khung Nếu Số Hàng là 1 thì chiều cao khung  = ycao , nếu hàng là 2 thì chia ra

            int FixHeight = SoHang - 1;
            int i = 0;
            foreach (Control gr in main_control.Controls)
            {

                if (gr is GroupBox)
                {

                    int val = i + 1;
                    int n = i / row_number;

                    if (i >= n * row_number && i < (n + 1) * row_number)
                    {
                        FixHeight = n;
                        val = val - n * row_number;
                    }


                    int viTriX = (val - 1) * Width;
                    int viTriY = FixHeight * Height;
                    gr.Size = new Size(Width, Height);
                    gr.Location = new Point(viTriX, viTriY);
                    i++;
                }


            }
        }
    }
}
