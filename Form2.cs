using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 开机发送邮件
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.stmpServer = textBox4.Text;
            Properties.Settings.Default.mailAccount = textBox1.Text;
            Properties.Settings.Default.pwd = textBox2.Text;
            Properties.Settings.Default.mailTo = textBox3.Text;
            Properties.Settings.Default.Save();
            Close();
        }

        private void Form2_Load(object sender, EventArgs e)// Settings不是数据库 是一个xml文件
        {
            Properties.Settings.Default.Upgrade();//更新xml文件的所有信息
            textBox1.Text = Properties.Settings.Default.mailAccount;
            textBox2.Text = Properties.Settings.Default.pwd;
            textBox3.Text = Properties.Settings.Default.mailTo;
            textBox4.Text = Properties.Settings.Default.stmpServer;

            if (Properties.Settings.Default.FirstStartup==true)//第一次运行判定这个变量是否更改过
            {
                //Properties.Settings.Default.Reset();
                Close();
            }
            /*else
            {
                Properties.Settings.Default.FirstStartup = true;
            }
            */
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.FirstStartup = true;
            label5.Visible = true;
            label6.Visible = true;
            button3.Visible = true;
            button2.Visible = false;
            Thread thd = new Thread(otherprocesskey);
            thd.Start();
            Properties.Settings.Default.Save();

        }

        private void otherprocesskey()//注册表编辑代码（开机自启
        {
            string where = Process.GetCurrentProcess().MainModule.FileName;
            RegistryKey keypass = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);//true表示可以修改
            string[] keynames = keypass.GetValueNames();//框框内是个集合
            bool result = false;
            foreach (string keyname in keynames)//遍历run项目下的所有子键,查看邮件子健是否存在
            {
                if (keyname == "邮件")
                {
                    //MessageBox.Show("有子健");
                    result = true;
                    break;//退出循环
                }
                /* else
                 {
                     MessageBox.Show("无子健");
                     result = false;                   
                 }*/
            }

            if (result == false)
            {
                //MessageBox.Show("无子键");
                keypass.SetValue("邮件", where);//2创建一个名字为邮件的键，值为where变量               
            }
            keypass.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Reset();
            label5.Visible = false;
            label6.Visible = false;
            button3.Visible = false;
            button2.Visible = true;

            RegistryKey keypass = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);//true表示可以修改
            string[] keynames = keypass.GetValueNames();//框框内是个集合
            bool result = false;
            foreach (string keyname in keynames)//遍历run项目下的所有子键,查看邮件子健是否存在
            {
                if (keyname == "邮件")
                {
                    //MessageBox.Show("有子健");
                    result = true;
                    break;//退出循环
                }
                /* else
                 {
                     MessageBox.Show("无子健");
                     result = false;                   
                 }*/
            }

            if (result == true)
            {               
                keypass.DeleteValue("邮件");
            }
            keypass.Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
