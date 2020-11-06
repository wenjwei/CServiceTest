using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CServiceTest
{
    class Program
    {
        static void Main(string[] args)
        {
            
        }

        Dictionary<string, Socket> dic = new Dictionary<string, Socket>();  //记录通信用的Socket
        public void Start(string txtIP, string txtPort)
        {
            // ip地址
            // IPAddress ip = IPAddress.Any;
            IPAddress ip = IPAddress.Parse(txtIP);

            // 端口号
            IPEndPoint point = new IPEndPoint(ip, int.Parse(txtPort));

            //使用IPv4地址，流式socket方式，tcp协议传递数据
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                // 绑定 ip port
                socket.Bind(point);

                // 接收客户端连接数量
                socket.Listen(10);

                ShowMsg("服务器开始监听");

                //Thread thread = new Thread(AcceptInfo);
                //thread.IsBackground = true;
                //thread.Start(socket);

                while (true)
                {
                    try
                    {
                        //创建通信用的Socket
                        Socket tSocket = socket.Accept();

                        //IPEndPoint endPoint = (IPEndPoint)client.RemoteEndPoint;
                        //string me = Dns.GetHostName();//得到本机名称
                        //MessageBox.Show(me);

                        ShowMsg(tSocket.RemoteEndPoint.ToString() + "连接成功！");
                        dic.Add(tSocket.RemoteEndPoint.ToString(), tSocket);

                        //接收消息
                        Thread th = new Thread(ReceiveMsg);
                        th.IsBackground = true;
                        th.Start(tSocket);
                    }
                    catch (Exception ex)
                    {
                        ShowMsg(ex.Message);
                    }
                }

            }
            catch (Exception ex)
            {
                ShowMsg(ex.Message);
            }
        }

        //接收消息
        void ReceiveMsg(object o)
        {
            Socket client = o as Socket;
            while (true)
            {
                if (client.Connected == true)
                {
                    try
                    {
                        //定义byte数组存放从客户端接收过来的数据
                        byte[] buffer = new byte[1024 * 1024];

                        //将接收过来的数据放到buffer中，并返回实际接受数据的长度
                        int n = client.Receive(buffer);

                        //将字节转换成字符串
                        string words = Encoding.UTF8.GetString(buffer, 0, n);
                        if (string.IsNullOrEmpty(words) == false)
                        {
                            ShowMsg(client.RemoteEndPoint.ToString() + ":" + words);
                        }
                    }
                    catch (Exception ex)
                    {
                        ShowMsg(ex.Message);
                    }
                }
                else
                {
                    break;
                }
            }
            
        }

        void ShowMsg(string msg)
        {
            Console.WriteLine(msg);
        }


        //给客户端发送消息
        private void btnSend_Click(object sender, EventArgs e)
        {
            try
            {
                string ip = "cboIpPort.Text";
                byte[] buffer = Encoding.UTF8.GetBytes("txtMsg.Text");
                dic[ip].Send(buffer);
            }
            catch (Exception ex)
            {
                ShowMsg(ex.Message);
            }
        }
    }
}
