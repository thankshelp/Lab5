using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Lab5
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const int port = 8888;
        static TcpListener listener;
        TcpClient client;
        NetworkStream stream = null;

        public MainWindow()
        {
            InitializeComponent();
        }

        public void Process(TcpClient tcpClient)
        {
            client = tcpClient;
            stream = null;
            try
            {
                stream = client.GetStream();
                byte[] data = new byte[64];
                while (true)
                {

                    StringBuilder builder = new StringBuilder();
                    int bytes = 0;
                    do
                    {

                        bytes = stream.Read(data, 0, data.Length);
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    }
                    while (stream.DataAvailable);

                    string message = builder.ToString();

                    if (message == "client disconnected")
                    {
                        //string message = builder.ToString();
                        Dispatcher.BeginInvoke(new Action(() =>  log.Items.Add(message)));
                    }                    
                    else
                    {
                        message = ReverseString(builder.ToString()) + " with love";
                        //Console.WriteLine(message);
                        data = Encoding.Unicode.GetBytes(message);
                        stream.Write(data, 0, data.Length);
                    }

                }
            }
            catch (Exception ex)
            {
                //Console.WriteLine(ex.Message);
            }
            finally
            {
                //освобождение ресурсов при завершении сеанса
                if (stream != null)
                    stream.Close();
                if (client != null)
                    client.Close();
            }
        }

        public void listen()
        {
            try
            {
                while (true)
                {
                    client = listener.AcceptTcpClient();
                    Dispatcher.BeginInvoke(new Action(() => log.Items.Add("client connected")));
                    Thread clientTread = new Thread(() => Process(client));
                    clientTread.Start();

                }
            }
            catch
            {
                if (stream != null)
                    stream.Close();
                if (client != null)
                    client.Close();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            listener = new TcpListener(IPAddress.Parse("127.0.0.1"), port);
            listener.Start();

            Thread clientTread = new Thread(() => listen());
            clientTread.Start();

            log.Items.Add("server start work");
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (listener != null)
                listener.Stop();
            log.Items.Add("server stop work");
        }
    


   
        public static string ReverseString(string s)
        {
            char[] arr = s.ToCharArray();
            Array.Reverse(arr);
            return new string(arr);
        }
    }

}   
