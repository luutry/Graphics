using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
//using MySql.Data.MySqlClient;
//using MySql.Data;
using MySQLDriverCS;


namespace Graphicsss
{
    public partial class Form1 : Form
    {
        FileSystemWatcher watcher = new FileSystemWatcher();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadPropertiesValue();
        }

        private void LoadPropertiesValue()
        {
            textBox1.Text = Properties.Settings.Default.URLLocation;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            if(openFile.ShowDialog()== DialogResult.OK && openFile.FileName != "")
            {
                textBox1.Text = openFile.FileName;

                Properties.Settings.Default.URLLocation = openFile.FileName;
                Properties.Settings.Default.Save();

                pictureBox1.ImageLocation = openFile.FileName;
            }
            openFile.Dispose();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string imgPath = "E:\\Pictures\\light.png";
            Bitmap bitmap = new Bitmap(imgPath);
           Image image = bitmap;
            Bitmap bitmap2 = new Bitmap(pictureBox1.Width,pictureBox1.Height);
            Graphics graphics = Graphics.FromImage(bitmap2);
            graphics.DrawImage(bitmap, new Rectangle(0, 0, pictureBox1.Width, pictureBox1.Height),
                new Rectangle(0, 0, image.Width, image.Height), GraphicsUnit.Display);
            graphics.Dispose();
            pictureBox1.Image = bitmap2;
            pictureBox1.SizeMode = PictureBoxSizeMode.Normal;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            bool result = connectShareDoc(@"\\127.0.0.1\Pictures", "Administrator","123");
            if (result)
            {
                string path = @"\\127.0.0.1\Pictures";
                string filePath = path + "\\" + "1-6.png";

                FileStream fileStream = new FileStream(filePath, FileMode.Open);
                byte[] bytes = new byte[fileStream.Length];
                fileStream.Read(bytes, 0, bytes.Length);
                fileStream.Close();

                Stream stream = new MemoryStream(bytes);
                Bitmap bitmap = new Bitmap(stream);
                pictureBox1.Image = bitmap;
            }
        }

        /// <summary>
        /// 建立局域网连接
        /// </summary>
        /// <param name="path"></param>
        /// <param name="userName"></param>
        /// <param name="passWord"></param>
        /// <returns></returns>
        private bool connectShareDoc(string path, string userName, string passWord)
        {
            bool Flag = false;
            Process proc = new Process();
            try
            {
                proc.StartInfo.FileName = "cmd.exe";
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.RedirectStandardInput = true;
                proc.StartInfo.RedirectStandardOutput = true;
                proc.StartInfo.RedirectStandardError = true;
                proc.StartInfo.CreateNoWindow = true;
                proc.Start();
                string dosLine = @"net use " + path + " /User:" + userName + " " + passWord + " /PERSISTENT:YES";
                proc.StandardInput.WriteLine(dosLine);
                proc.StandardInput.WriteLine("exit");
                while (!proc.HasExited)
                {
                    proc.WaitForExit(1000);
                }
                string errormsg = proc.StandardError.ReadToEnd();
                proc.StandardError.Close();
                if (string.IsNullOrEmpty(errormsg))
                {
                    Flag = true;
                }
                else
                {
                    throw new Exception(errormsg);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                proc.Close();
                proc.Dispose();
            }
            return Flag;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            MySQLConnection DBConn;
            DBConn = new MySQLConnection(new MySQLConnectionString("127.0.0.1", "test", "root", "root", 3306).AsString);
            try
            {
                DBConn.Open(); 
                string sql = "insert into testTable(id,name) values('54','542')";
                MySQLDataAdapter mda = new MySQLDataAdapter(sql, DBConn);
                DataSet ds = new DataSet();
                mda.Fill(ds, "testtable");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            DBConn.Close();
        }
    }
}
