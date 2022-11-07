using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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

namespace ServerApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Server server;
        

        public MainWindow()
        {
            InitializeComponent();
            LBimages.SelectedIndex = 0; // Default to the list box of the images
            string cp = Directory.GetCurrentDirectory(); // The current directory 
            cp = cp.Substring(0, cp.IndexOf("ServerApp")) + @"\ServerApp\ServerApp\Images\"; // The directory of the images folder
            GetNames(cp);
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        // The button that give the server to listen to some clients
        {
            try
            {
                server = new Server(this);
                MessageBox.Show("Now wait for clients to connect");
            }
            catch
            {
                MessageBox.Show("Error");
            }
        }

        private void startMoving_Click(object sender, RoutedEventArgs e)
        // The button that starts the moving
        {
            try
            {
                server.StartMovingPic(LBimages.SelectedItem.ToString()); // Send to the moving the name of the image
            }
            catch
            {
                MessageBox.Show("You need first to click the start button"); // If the user click the start moving button before the start button
            }

        }

        private void GetNames(string file_path)
        // Show all the names of the images from the folder in the list box
        {
            LBimages.Items.Clear();
            string[] images_with_source = Directory.GetFileSystemEntries(file_path); // Array with all the source of the images
            string[] image_Dirs = new string[images_with_source.Length]; // Array that his length is the number of the images

            for (int i = 0; i < images_with_source.Length; i++)
            {
                image_Dirs[i] = images_with_source[i].Substring(images_with_source[i].LastIndexOf('\\') + 1); // Fill the array of the images names
                if ((image_Dirs[i] != "Icon.ico") && (image_Dirs[i] != "Thumbs.db")) // The first name is the icon and the second name is a hidden file which is not an image so it does not in the list box
                {
                    LBimages.Items.Add(image_Dirs[i]); // Add each image name to the list box
                }

            }
        }

        private void LBimages_SelectionChanged(object sender, SelectionChangedEventArgs e)
        // The selected item in the list box of the images
        {
            if (LBimages.SelectedItem != null)
            {
                string fileName = LBimages.SelectedItem.ToString();
                string cp = Directory.GetCurrentDirectory(); // The current directory 
                cp = cp.Substring(0, cp.IndexOf("ServerApp")) + @"\ServerApp\ServerApp\Images\"; // The directory of the images folder

                this.Img.Source = new BitmapImage(new Uri(cp + fileName)); // Show the selected item in the image
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        // Exit button
        {
            Application.Current.Shutdown(); // Exit the app
            Process [] proc = Process.GetProcessesByName("ServerApp");
	        proc[0].Kill(); // Kill the process
        }
    }
}
