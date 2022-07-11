using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using System.Windows.Forms;
using Microsoft.Win32;
using System.Diagnostics;
using System.Web;
using System.Net.Mail;
using System.Net;


namespace 开机发送邮件
{
    public partial class Form1 : Form
    {
        [DllImport("winInet.dll ")]//调用winInetdll
        public extern static bool InternetGetConnectedState(out int connetiondescription, int reservedvalue);//dll中的一个函数
        int a = 0;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)//程序加载时运行的代码
        {           
            var frm2 =new Form2();
            frm2.ShowDialog();//执行窗口2

            Properties.Settings.Default.Upgrade();//更新xml数据

            //Thread thd = new Thread(otherprocesskey);
            //thd.Start();
            Thread thd2 = new Thread(otherprocessinternet);
            thd2.Start();
        }

        /*private void otherprocesskey()//注册表编辑代码（开机自启
        {
            string where = Process.GetCurrentProcess().MainModule.FileName;
            RegistryKey keypass = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run",true);//true表示可以修改
            string[] keynames = keypass.GetValueNames();//框框内是个集合
            bool result = false;
            foreach (string keyname in keynames)//遍历run项目下的所有子键,查看邮件子健是否存在
            {
                if(keyname == "邮件")
                {
                    //MessageBox.Show("有子健");
                    result = true;
                    break;//退出循环
                }
               else
                {
                    MessageBox.Show("无子健");
                    result = false;                   
                }
            }

            if (result == false)
            {
                //MessageBox.Show("无子键");
                keypass.SetValue("邮件", where);//2创建一个名字为邮件的键，值为where变量
                keypass.Close();

            }
        }*/

        public void otherprocessinternet()//网络自检
        {
            int i = 0;
        loop: Thread.Sleep(1000);
            if (InternetGetConnectedState(out i, 0))//检查是否有网络
            {
                //MessageBox.Show("有网络");
                Thread r = new Thread(otherprocessemail);
                r.Start();
            }
            else
            {
                //MessageBox.Show("无网络");
                Thread.Sleep (2000);
                goto loop;//跳到loop哪里去，现代编程正式禁止使用，是个狗屎代码
            }
        }

        private void otherprocessemail()//当网络接通时执行以下代码
        {
            //MessageBox.Show("YOU DIT IT");
            //Process.GetCurrentProcess().MainModule.FileName 是显示当前程序完全路径

            //Environment.Exit(0);
            string stmpServer = @Properties.Settings.Default.stmpServer;//smtp服务器地址
            string mailAccount = @Properties.Settings.Default.mailAccount;//邮箱账号
            string pwd = @Properties.Settings.Default.pwd;//邮箱密码（qq邮箱此处使用授权码，其他邮箱见邮箱规定使用的是邮箱密码还是授权码）

            string mailTo = @Properties.Settings.Default.mailTo;
            string mailTitle = @"开机报告";//设置发送邮件得标题
            System.DateTime currenttime = System.DateTime.Now;
            string mailContent = @"您的电脑于" + currenttime.Year + @"年" + currenttime.Month + @"月" + currenttime.Day + @"日" + currenttime.Hour + @"时" + currenttime.Minute + @"分" + currenttime.Second + @"秒开机，是时候动身了";//设置发送邮件内容
            //MessageBox.Show(mailContent);
            SmtpClient smtpClient = new SmtpClient();
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;//指定电子邮件发送方式
            smtpClient.Host = Properties.Settings.Default.stmpServer;//指定发送方SMTP服务器
            smtpClient.EnableSsl = true;//使用安全加密连接
            smtpClient.UseDefaultCredentials = true;//不和请求一起发送
            smtpClient.Credentials = new NetworkCredential(mailAccount, pwd);//设置发送账号密码

            MailMessage mailMessage = new MailMessage(mailAccount,mailTo);//实例化邮件信息实体并设置发送方和接收方
            mailMessage.Subject = mailTitle;
            mailMessage.Body = mailContent;
            mailMessage.BodyEncoding = Encoding.UTF8;//设置发送邮件得编码
            mailMessage.IsBodyHtml = false;//设置标题是否为HTML格式
            mailMessage.Priority = MailPriority.Normal;//设置邮件发送优先级



            try
            {
                smtpClient.Send(mailMessage);//发送邮件
                //MessageBox.Show("邮件已发送");
            }
            catch
            {
                throw;
            }
            Environment.Exit(0);//结束自己
        }
    }
}