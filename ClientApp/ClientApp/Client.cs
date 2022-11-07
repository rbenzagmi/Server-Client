using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Media.Imaging;
using System.Windows;

namespace ClientApp
{
    class Client
    {
        public static Socket client; // Create new socket

        private static string server_ip;

        private static byte[] bufferRec;

        private static byte[] bufferSend = new byte[1024];

        private static int port;

        private static IPEndPoint addr;

        static MainWindow mainWin;

        public Client(MainWindow win, string serverIp, int serverPort)
        // Open the client socket 
        {
            try
            {
                mainWin = win;
                client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); // Open new client socket
                server_ip = serverIp;
                port = serverPort;
                addr = new IPEndPoint(IPAddress.Parse(server_ip), port);
            }
            catch
            {
                MessageBox.Show("Error with socket of the client");
            }

        }

        public void Open()
        // Open thread of the requests from the server
        {
            Thread tr1 = new Thread(new ThreadStart(RequestsFromTheServer));
            tr1.Start();

        }

        private void RequestsFromTheServer()
        // The method works in own thread and listening to requests from the server
        {
            try
            {
                client.Connect(addr); // The connection of the client
            }
            catch
            {
                MessageBox.Show("Error with the connection of the client");
            }

            try
            {
                bufferRec = new byte[1024];
                string receivedBytes = "";
                receivedBytes = client.Receive(bufferRec).ToString(); // Recieve request from the server
                string rec = "";
                rec = System.Text.Encoding.ASCII.GetString(bufferRec); // The request
                string newRec = string.Empty;
                if (rec.IndexOf("\0") != -1)
                {
                    newRec = rec.Substring(0, rec.IndexOf("\0")); // Substring the two words
                }
                else
                    newRec = rec;

                string nameOfPic = newRec.Split('@')[0]; // The name of the image from the request

                mainWin.Dispatcher.Invoke((Action)(() =>
                {
                    mainWin.image.Source = new BitmapImage(new Uri(@"/Images/" + nameOfPic, UriKind.Relative)); // Put the image in the client window
                }));

                if (newRec.Split('@')[1] == "move") // If the second word is this word so the client need to move
                {
                    mainWin.StartMovingThePic();
                }
            }
            catch
            {
                MessageBox.Show("Error with recieve of the client");
            }



        }

        internal void Finished()
        // Send to the server that the image finish to move
        {
            try
            {
                bufferSend = System.Text.Encoding.ASCII.GetBytes("Finished");
                client.Send(bufferSend, SocketFlags.None); // Send to the server that the client finish
            }
            catch
            {
                MessageBox.Show("Error with send of the client");
            }

        }
    }
}
