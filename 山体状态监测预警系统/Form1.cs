using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Media;

namespace 山体状态监测预警系统
{
    public partial class Form1 : Form
    {
        Socket socketClient;
        delegate void Receivecallback(string str, int n);
        Receivecallback delReceive;
        delegate void PictureCallback(string d, int n, Bitmap bmp, string firsttime);
        PictureCallback delPicture;

        string str;

        //1节点画图数据
        int isFirst_1z = 1;
        int i_1z = 0;
        int p1_y_1z;
        Bitmap[] bmp_1z = new Bitmap[4];
        string[] firstStr_1z = new string[5];
        string[] first_y_1z = new string[5];
        string[] firsttime_1z = new string[5];
        string firsttime1_1z;




        //2节点画图数据
        int isFirst_2z = 1;
        int i_2z = 0;
        int p1_y_2z;
        Bitmap[] bmp_2z = new Bitmap[4];
        string[] firstStr_2z = new string[5];
        string[] first_y_2z = new string[5];
        string[] firsttime_2z = new string[5]; 
        string firsttime1_2z;

        private void timer1_Tick(object sender, EventArgs e)
        {
            label1.Text = DateTime.Now.ToShortTimeString();
            label2.Text = DateTime.Now.ToShortTimeString();

        }
        public Form1()
        {

            InitializeComponent();
            delReceive = delreceive;
            delPicture = delpicture;
            try
            {
                socketClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint Point = new IPEndPoint(IPAddress.Parse("119.146.68.41"), 5000);
                socketClient.Connect(Point);
                Send();
                Thread thReceive = new Thread(Receive);
                thReceive.IsBackground = true;
                thReceive.Start();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }

        }


        //发送密码和请求
        void Send()
        {
            string strSend = "MVP666";
            byte[] buffer = Encoding.Default.GetBytes(strSend);
            socketClient.Send(buffer);
            string strSend1 = "请求";
            byte[] buffer2 = Encoding.Default.GetBytes(strSend1);
            socketClient.Send(buffer2);

        }


        //接收消息
        void Receive()
        {
            try
            {
                while (true)
                {
                    string str1;
                    byte[] buffer = new byte[1024];
                    int r = socketClient.Receive(buffer);
                    if (r == 0)
                        break;
                    else
                    {
                        str1 = Encoding.Default.GetString(buffer, 0, r);
                    }


                    //节点1数据处理
                    if (str1[0] == 'A')
                    {
                        str = (str1.Substring(str1.IndexOf('z'), str1.Length - str1.IndexOf('z')));
                        Paint("1z", str.Substring(1, 2), ref isFirst_1z, ref i_1z, ref p1_y_1z, ref bmp_1z, ref firstStr_1z, ref firsttime_1z, ref first_y_1z, ref firsttime1_1z);
                        if (str.Substring(1, 2) != "19" && str.Substring(1, 2) != "20")
                            text1Z.Invoke(delReceive, str.Substring(1, 2), 1);

                        text1x.Invoke(delReceive, str.Substring(str.IndexOf('x') + 1, str.IndexOf('y') - 1 - str.IndexOf('x')), 3);
                        text1y.Invoke(delReceive, str.Substring(str.IndexOf('y') + 1, str.Length - 1 - str.IndexOf('y')), 4);
                        if (str.Substring(1, 2) != "19" && str.Substring(1, 2) != "20")
                        {
                            SoundPlayer sp = new SoundPlayer();
                            sp.SoundLocation = System.Windows.Forms.Application.StartupPath + "\\" + "901130.wav";
                            sp.Load();
                            sp.Play();
                        }

                    }

                    //节点2数据处理
                    if (str1[0] == 'B')
                    {
                        str = (str1.Substring(str1.IndexOf('z'), str1.Length - str1.IndexOf('z')));
                        Paint("2z", str.Substring(1, 2), ref isFirst_2z, ref i_2z, ref p1_y_2z, ref bmp_2z, ref firstStr_2z, ref firsttime_2z, ref first_y_2z, ref firsttime1_2z);
                        if (str.Substring(1, 2) != "21" && str.Substring(1, 2) != "22")
                            text2Z.Invoke(delReceive, str.Substring(1, 2), 2);
                        text2x.Invoke(delReceive, str.Substring(str.IndexOf('x') + 1, str.IndexOf('y') - 1 - str.IndexOf('x')), 5);
                        text2y.Invoke(delReceive, str.Substring(str.IndexOf('y') + 1, str.Length - 1 - str.IndexOf('y')), 6);
                        if (str.Substring(1, 2) != "21" && str.Substring(1, 2) != "22")
                        {
                            SoundPlayer sp = new SoundPlayer();
                            sp.SoundLocation = System.Windows.Forms.Application.StartupPath + "\\" + "901130.wav";
                            sp.Load();
                            sp.Play();
                        }

                    }

                }
            }
            catch (Exception)
            {


            }

        }






        //画图
        void Paint(string d, string str, ref int isFirst, ref int i, ref int p1_y, ref Bitmap[] bmp, ref string[] firstStr, ref string[] firsttime, ref string[] first_y, ref string firsttime1)
        {


            //Bitmap[] bmpTime = new Bitmap[3];

            SolidBrush mySolidBrush = new SolidBrush(Color.Black);
            Bitmap bmp1 = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            Pen pen = new Pen(Brushes.Black);
            Point pTime = new Point(pictureBox1.Width - 24, pictureBox1.Height - 102);

            Point p1 = new Point(0, p1_y);
            Point p2 = new Point(pictureBox1.Width, pictureBox1.Height - Convert.ToInt32(str) * 10);

            firstStr[i] = str;
            firsttime[i] = System.DateTime.Now.Second.ToString();
            first_y[i] = p1_y.ToString();
            //Point pData1 = new Point(p1.X, p1.Y - 10);
            Point pData = new Point(p2.X - 15, p2.Y - 20);

            if (isFirst == 1)   //第一次只画一个点
            {
                bmp[0] = bmp1;
                p1_y = pictureBox1.Height - Convert.ToInt32(str) * 10;


                //画点
                Graphics gFirstPoint = Graphics.FromImage(bmp[0]);
                //Rectangle retFirst = new Rectangle(0, p1_y, 3, 3);
                //gFirstPoint.DrawRectangle(pen, retFirst);

                gFirstPoint.FillRectangle(mySolidBrush, 0, p1_y, 3, 3);


                //画数据
                Point pFirstData = new Point(2, p1_y - 20);
                Graphics gFirstData = Graphics.FromImage(bmp[0]);
                gFirstData.DrawString(str, new Font("宋体", 8), Brushes.Blue, pFirstData);

                firsttime1 = firsttime[0];//将第一个时间存入firsttime1，一下画第一轮图的时候用到
                pictureBox1.Invoke(delPicture, d, 1, bmp[0], firsttime[0]);
                isFirst = 0;

            }
            else
            {



                if (i <= 3)
                {
                    if (i > 0)         //画在第二个box才新加一个bitmap
                    { bmp[i] = bmp1; }

                    //画线
                    Graphics g = Graphics.FromImage(bmp[i]);
                    g.DrawLine(pen, p1, p2);



                    firsttime[i] = System.DateTime.Now.Second.ToString();


                    //在线的终点下画时间
                    Graphics gTime = Graphics.FromImage(bmp[i]);
                    gTime.DrawString(firsttime[i] + "s", new Font("宋体", 8), Brushes.Blue, pTime);

                    //在线两端加点
                    Graphics gPoint1 = Graphics.FromImage(bmp[i]);
                    gPoint1.FillRectangle(mySolidBrush, p1.X, p1.Y, 3, 3);
                    Graphics gPoint2 = Graphics.FromImage(bmp[i]);
                    gPoint2.FillRectangle(mySolidBrush, p2.X - 3, p2.Y, 3, 3);


                    //在线的终端上画数据
                    Graphics gData2 = Graphics.FromImage(bmp[i]);
                    gData2.DrawString(str, new Font("宋体", 8), Brushes.Blue, pData);

                    if (i == 0)
                    {
                        pictureBox1.Invoke(delPicture, d, 1, bmp[i], firsttime1);//每画一个图都在第一个box画第一个时间

                    }
                    else if (i == 1)
                    {
                        pictureBox2.Invoke(delPicture, d, 2, bmp[i], firsttime1);


                    }
                    else if (i == 2)
                    {
                        pictureBox3.Invoke(delPicture, d, 3, bmp[i], firsttime1);

                    }
                    else if (i == 3)
                    {
                        pictureBox4.Invoke(delPicture, d, 4, bmp[i], firsttime1);

                    }

                    i++;

                }
                else
                {
                    if (d == "1z")
                    {
                        bmp[0] = (Bitmap)pictureBox2.Image;
                        bmp[1] = (Bitmap)pictureBox3.Image;
                        bmp[2] = (Bitmap)pictureBox4.Image;
                    }

                    else if (d == "2z")
                    {
                        bmp[0] = (Bitmap)pictureBox22.Image;
                        bmp[1] = (Bitmap)pictureBox23.Image;
                        bmp[2] = (Bitmap)pictureBox24.Image;
                    }

                    pictureBox1.Invoke(delPicture, d, 111, bmp[0], firsttime[0]);//第二个参数为11时，重新开始画第一图时在线的首端画上原来线末端的数据
                    pictureBox2.Invoke(delPicture, d, 2, bmp[1], firsttime[0]);
                    pictureBox3.Invoke(delPicture, d, 3, bmp[2], firsttime[0]);

                    bmp[3] = bmp1;

                    //画线
                    Graphics g = Graphics.FromImage(bmp[3]);
                    g.DrawLine(pen, p1, p2);
                    //画时间
                    Graphics gTime = Graphics.FromImage(bmp[3]);
                    string t = System.DateTime.Now.Second.ToString();
                    gTime.DrawString(t + "s", new Font("宋体", 8), Brushes.Blue, pTime);

                    //画两个点
                    Graphics gPoint1 = Graphics.FromImage(bmp[3]);
                    gPoint1.FillRectangle(mySolidBrush, p1.X - 5, p1.Y, 3, 3);
                    Graphics gPoint2 = Graphics.FromImage(bmp[3]);
                    gPoint2.FillRectangle(mySolidBrush, p2.X - 5, p2.Y, 3, 3);


                    //画数据
                    Graphics gData2 = Graphics.FromImage(bmp[3]);
                    gData2.DrawString(str, new Font("宋体", 8), Brushes.Blue, pData);

                    pictureBox4.Invoke(delPicture, d, 4, bmp[3], firsttime[0]);

                    moveFirst(firstStr, str);

                    moveFirst(firsttime, t);

                }
                p1_y = p2.Y;
                p1.Y = p1_y;
                moveFirst(first_y, (p1_y).ToString());
            }
        }

        //第一个时间和数据坐标移动方法
        void moveFirst(string[] first, string str)
        {
            for (int i = 0; i <= first.Length - 3; i++)
            {
                first[i] = first[i + 1];
            }
            first[first.Length - 1] = str;
            first[first.Length - 2] = first[first.Length - 1];
        }


        //委托画图
        void delpicture(string d, int n, Bitmap bmp, string firsttime)
        {

            if (d == "1z")
            {
                if (n == 1)
                {
                    pictureBox1.Image = bmp;
                    Graphics g = Graphics.FromImage(bmp);
                    g.DrawString(firsttime + "s", new Font("宋体", 8), Brushes.Blue, 0, pictureBox1.Height - 102);
                }
                if (n == 111)
                {
                    pictureBox1.Image = bmp;
                    Graphics gstr = Graphics.FromImage(bmp);
                    gstr.DrawString(firstStr_1z[0], new Font("宋体", 8), Brushes.Blue, 0, Convert.ToInt32(first_y_1z[0]) - 20);
                    Graphics g = Graphics.FromImage(bmp);
                    g.DrawString(firsttime + "s", new Font("宋体", 8), Brushes.Blue, 0, pictureBox1.Height - 102);
                }
                if (n == 2)
                { pictureBox2.Image = bmp; }
                if (n == 3)
                { pictureBox3.Image = bmp; }

                if (n == 4)
                { pictureBox4.Image = bmp; }
            }


            if (d == "2z")
            {
                if (n == 1)
                {
                    pictureBox21.Image = bmp;
                    Graphics g = Graphics.FromImage(bmp);
                    g.DrawString(firsttime + "s", new Font("宋体", 8), Brushes.Blue, 0, pictureBox1.Height - 102);
                }
                if (n == 111)
                {
                    pictureBox21.Image = bmp;
                    Graphics gstr = Graphics.FromImage(bmp);
                    gstr.DrawString(firstStr_2z[0], new Font("宋体", 8), Brushes.Blue, 0, Convert.ToInt32(first_y_2z[0]) - 20);
                    Graphics g = Graphics.FromImage(bmp);
                    g.DrawString(firsttime + "s", new Font("宋体", 8), Brushes.Blue, 0, pictureBox1.Height - 102);
                }
                if (n == 2)
                { pictureBox22.Image = bmp; }
                if (n == 3)
                { pictureBox23.Image = bmp; }

                if (n == 4)
                { pictureBox24.Image = bmp; }

            }
        }





        //委托接收消息
        void delreceive(string str2, int n)
        {
            if (n == 1)
                text1Z.AppendText(label1.Text + ":" + System.DateTime.Now.Second.ToString() + "    " + str2 + " 度" + "\n");
            if (n == 2)
                text2Z.AppendText(label2.Text + ":" + System.DateTime.Now.Second.ToString() + "    " + str2 + " 度" + "\n");
            if (n == 3)
                text1x.AppendText(label1.Text + ":" + System.DateTime.Now.Second.ToString() + "    " + str2 + " g" + "\n");
            if (n == 4)
                text1y.AppendText(label1.Text + ":" + System.DateTime.Now.Second.ToString() + "    " + str2 + " g" + "\n");
            if (n == 5)
                text2x.AppendText(label2.Text + ":" + System.DateTime.Now.Second.ToString() + "    " + str2 + " g" + "\n");
            if (n == 6)
                text2y.AppendText(label2.Text + ":" + System.DateTime.Now.Second.ToString() + "    " + str2 + " g" + "\n");
        }






    }
}