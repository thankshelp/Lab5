﻿using System;
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
using System.Net.Sockets;
using System.Threading;

namespace Lab5client
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const int port = 8888;
        const string address = "127.0.0.1";

        TcpClient client = null;
        NetworkStream stream = null;
        name adt;

        public MainWindow()
        {
            InitializeComponent();
            adt = new name();
            if(adt.ShowDialog() == true)
            {
                string userName = adt.clientName.Text;
            }
            //Console.Write("Enter your name:");
            //string userName = Console.ReadLine();

            

        }

        public void listen()
        {
            try
            {

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
                    //   Console.WriteLine("Server: {0}", message);
                   // log.Items.Add(message);
                    Dispatcher.BeginInvoke(new Action( () => log.Items.Add(message)));

                }

            }
            catch (Exception ex)
            {
              //  Console.WriteLine(ex.Message);
            }
            finally
            {
                client.Close();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            client = new TcpClient(address, port);
            stream = client.GetStream();

            Thread clientTread = new Thread(() => listen());
            clientTread.Start();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
           // Console.Write(userName + ": ");
            string message = msg.Text;
          //  message = String.Format("{0}: {1}", userName, message);
            byte[] data = Encoding.Unicode.GetBytes(message);
            stream.Write(data, 0, data.Length);
            log.Items.Add(message);
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            
            string message = "client disconnected";
            byte[] data = Encoding.Unicode.GetBytes(message);
            stream.Write(data, 0, data.Length);
            client.Close();
        }
    }
}
