using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using System.Drawing.Printing;
using System.Drawing;

namespace ServerApp
{
    class Server
    {
        private static Socket server; // The server socket

        private static Socket client; // The client who connect

        private static int port = 40000;

        private static IPAddress ipAddress = Dns.Resolve(Dns.GetHostName()).AddressList[0]; // Server IP

        private static IPEndPoint Addr;

        private static List<Socket> clients = new List<Socket>(); // List of clients who connected

        private static byte[] sendBuffer = new byte[1024];

        private static byte[] recBuffer;

        private static bool Once = true;

        static MainWindow mw; // The main window

        int count = 0;

        private string nameOfPic = string.Empty; // The name of the image that chosen

        public Server(MainWindow mainWin)
        // Open the server socket and wait for the connections of the clients
        {
            try
            {
                Addr = new IPEndPoint(ipAddress, port);
                server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); // Create new socket of the server
                server.Bind(Addr);
                server.Listen(15); // Listen to maximum of 15 clients 
                mw = mainWin;
                Thread threadOfConnect = new Thread(new ThreadStart(connect)); // Thread of the connection
                threadOfConnect.Start();
            }
            catch
            {
                MessageBox.Show("Error with the connections");
            }
        }

        private void recieve()
        // The server recieve
        {
            while (true)
            {
                int newCount = count;
                if (count != 0)
                {
                    newCount = count - 1;
                }

                try
                {
                    recBuffer = new byte[1024];
                    string receivedBytes = "";
                    receivedBytes = clients[newCount].Receive(recBuffer).ToString(); // Recieve request from specific client
                    string rec = "";
                    rec = System.Text.Encoding.ASCII.GetString(recBuffer); // The request
                    string newRec = string.Empty;

                    if (rec.IndexOf("\0") != -1)
                    {
                        newRec = rec.Substring(0, rec.IndexOf("\0")); // Substring the first word
                    }
                    else
                        newRec = rec;

                    if (newRec == "Finished") // If the word is this word so the client finish
                    {
                        mw.Dispatcher.BeginInvoke(new ThreadStart(() => mw.Status.Text += "Client " + (newCount) + Environment.NewLine)); // Write in the text box that this client finish
                        StartMovingPic(nameOfPic);
                    }
                }
                catch
                { }
            }
        }

        private void connect()
        // Connect to clients
        {
            while (true)
            {
                try
                {
                    client = server.Accept();
                }
                catch (Exception)
                {
                    MessageBox.Show("Error with the accept");
                }

                if (Once)
                {
                    Once = false;
                    Thread threadOfRecieve = new Thread(new ThreadStart(recieve)); // Thread of the recieve
                    threadOfRecieve.Start();
                }
                AddClientToList(clients);
            }
        }

        private void AddClientToList(List<Socket> clients)
        // Add each connect client to the list
        {
            try
            {
                clients.Add(client);
            }
            catch
            {
                MessageBox.Show("Can not add this client");
            }

            mw.Dispatcher.BeginInvoke(new ThreadStart(() => mw.IPlist.Items.Add(IPAddress.Parse(((IPEndPoint)client.RemoteEndPoint).Address.ToString()).ToString()))); // Add to the list box of the ip the ip that right now connected
        }



        internal void StartMovingPic(string picName)
        // The image start to move in the client
        {
            nameOfPic = picName; // Each client will recieve the image

            if (count < clients.Count) // If this is not the end of the clients
            {
                try
                {
                    sendBuffer = System.Text.Encoding.ASCII.GetBytes(picName + "@move");
                    clients[count].Send(sendBuffer); // Send to the client the name of the image and the word move
                    count++; // Go to the next client
                }
                catch
                {
                    MessageBox.Show("Error with the send of the server");
                }
            }
            else
            {
                mw.Dispatcher.BeginInvoke(new ThreadStart(() => mw.Status.Text += "The end")); // Show in the text box that the moving its over

                try
                {
                    PrintDocument pd = new PrintDocument();
                    pd.PrintPage += PrintPage;
                    pd.Print(); // print the image
                }
                catch
                {
                    MessageBox.Show("Error with the print");
                } 
            }
        }

        private void PrintPage(object obj, PrintPageEventArgs eventargs)
        // Print the chosen image
        {
            string cp = Directory.GetCurrentDirectory(); // The current directory 
            cp = cp.Substring(0, cp.IndexOf("ServerApp")) + @"\ServerApp\ServerApp\Images\"; // The directory of the images folder

            try
            {
                if (File.Exists(cp + nameOfPic)) // If the file exists
                {
                    Image img = Image.FromFile(cp + nameOfPic); // Load the image from the file
                    Rectangle rect = eventargs.MarginBounds; // Adjust the size of the image to the page to print the full image without loosing any part of it

                    if ((double)img.Width / (double)img.Height > (double)rect.Width / (double)rect.Height) // If the image is wider
                    {
                        rect.Height = (int)((double)img.Height / (double)img.Width * (double)rect.Width);
                    }
                    else
                    {
                        rect.Width = (int)((double)img.Width / (double)img.Height * (double)rect.Height);
                    }
                    eventargs.Graphics.DrawImage(img, rect);
                }
            }
            catch
            {
                MessageBox.Show("Error with the printing");
            }
        }
    }
}
