using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TkAutoBrowser
{
    public class TkHelp
    {
        public static int loopComment = 0;
        public static TextBox textBox;
        public static void Comment(string comment)
        {

            if (textBox == null)
            {
                return;
            }

            textBox.BeginInvoke((Action)delegate ()
            {
                textBox.Text += comment + "  ------" + DateTime.Now.ToString("HH:ss") + "   \r\n";
                if (loopComment > 300)
                {
                    textBox.Clear();
                    loopComment = 0;
                }
            });
            loopComment++;


        }

        public static string RandomToken(int length)
        {
            Random random = new Random(Guid.NewGuid().GetHashCode());
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        public static string RandomCharToken(int length)
        {
            Random random = new Random(Guid.NewGuid().GetHashCode());
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        public static string RandomNumberString(int length)
        {
            Random random = new Random(Guid.NewGuid().GetHashCode());
            const string chars = "0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static void CheckNumberic(KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);

        }
        public static int GetNumberic(string number)
        {
            return int.Parse(number);
        }


        public static object locker = new object();
        public static void AddList(string path, string NoiDung)
        {
       

            lock (locker)
            {
                FileStream fs = new FileStream(path, FileMode.Append);
                StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);
                sw.WriteLine(NoiDung);
                sw.Flush();
                sw.Close();
            }

        }
        public static List<string> ReadList(string path)
        {
            return File.ReadAllLines(path).ToList();
        }
        public static List<string> OpenDialogAndSelectFile()
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            //openFileDialog1.InitialDirectory = "";
            openFileDialog1.Filter = "TXT files| *.txt";
            openFileDialog1.FilterIndex = 0;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string selectedFileName = openFileDialog1.FileName;
                var listFile = ReadList(selectedFileName);
                return listFile;
            }

            return new List<string>();


        }
      
    }
}
