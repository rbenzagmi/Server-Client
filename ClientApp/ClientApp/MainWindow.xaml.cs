using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ClientApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Client client;

        public MainWindow()
        {
            InitializeComponent();
            connectToServer();
            
        }

        private void connectToServer()
        // The connection to the server
        {
            try
            {
                client = new Client(this, "192.168.56.1", 40000);
                client.Open();
            }
            catch
            {
                MessageBox.Show("Error with the client");
            }

        }

        public void animat_Completed(object sender, EventArgs e)
        // When the image finish to move in the client window
        {
            client.Finished();

            try
            {
                this.Dispatcher.Invoke((Action)(() =>
                {

                    ThicknessAnimation animat = new ThicknessAnimation();
                    animat.From = new Thickness(canvas.Width, 0, 0, 0); // The start 
                    animat.To = new Thickness(canvas.Width+canvas.Width, 0, 0, 0); // The end
                    animat.Duration = new Duration(TimeSpan.FromSeconds(2)); // The time
                    image.BeginAnimation(Image.MarginProperty, animat); // Start to move
                }));
            }
            catch
            {
                MessageBox.Show("Error with the move");
            }
        }

        public void StartMovingThePic()
        // The move of the picture in the client window
        {
            try
            {
                this.Dispatcher.Invoke((Action)(() =>
                {

                    ThicknessAnimation animat = new ThicknessAnimation();
                    animat.From = new Thickness(0, 0, 0, 0); // The start 
                    animat.To = new Thickness(canvas.Width, 0, 0, 0); // The end
                    animat.Duration = new Duration(TimeSpan.FromSeconds(2)); // The time
                    animat.Completed += animat_Completed;
                    image.BeginAnimation(Image.MarginProperty, animat); // Start to move
                }));
            }
            catch
            {
                MessageBox.Show("Error with the move of the picture");
            }

        }
    }
}
