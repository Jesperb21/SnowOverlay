using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace TestOverlay
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region initialize variables
        Random rand = new Random();
        int counter = 0;
        int spawn = 2;
        int clusterSize = 5;
        int scanForImages = 1200;//30 sec        
        int scannerCounter = 1200;//start by scanning
        List<Uri> Images = new List<Uri>();

        List<ContentControl> flakes = new List<ContentControl>();
        List<Image> oldFlakeRevamp = new List<Image>();
        List<SnowFlake> snowflakes = new List<SnowFlake>();
        #region animations
        DoubleAnimation animation = new DoubleAnimation
        {
            From = -250,
            To = SystemParameters.VirtualScreenWidth,
            Duration = new Duration(TimeSpan.FromSeconds(20)),
            RepeatBehavior = RepeatBehavior.Forever,
        };
        DoubleAnimation heightAnimation = new DoubleAnimation
        {
            From = 50,
            To = 150,
            Duration = new Duration(TimeSpan.FromSeconds(2)),
            RepeatBehavior = RepeatBehavior.Forever,
            AutoReverse = true,
        };
        #endregion
        #endregion

        public MainWindow()
        {
            InitializeComponent();
            Application.Current.MainWindow.Height = SystemParameters.VirtualScreenHeight;
            Application.Current.MainWindow.Width = SystemParameters.VirtualScreenWidth;
            Application.Current.MainWindow.Top = 0;
            Application.Current.MainWindow.Left = 0;
            godo();

            #region storyboard animations
            santaImg.BeginAnimation(Window.LeftProperty, animation);
            santaImg.BeginAnimation(Window.TopProperty, heightAnimation);
            #endregion

            
        }

        #region exit
        /***
         * exit the application
         */
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        #endregion

        private void godo()
        {
            
            DispatcherTimer dodo = new DispatcherTimer();
            dodo.Interval = TimeSpan.FromMilliseconds(25);
            dodo.Tick += doStuff;
            dodo.Start();
        }

        #region main loop
        void doStuff(object sender, EventArgs e)
        {
            if (counter >= spawn)
            {
                //old flakes
                /*
                ContentControl temp = new ContentControl();
                temp.Width = 50;
                temp.Template = Resources["snowflake"] as ControlTemplate;
                Canvas.SetTop(temp, -100);
                Canvas.SetLeft(temp, rand.Next(0, (int)mainGrid.ActualWidth));
                temp.Opacity = 0.5;
                //add to screen
                mainGrid.Children.Add(temp);
                flakes.Add(temp);
                */
                //new flakes
                for (int i = 0; i < clusterSize; i++)
                {
                    int size = rand.Next(5, 25);
                    SnowFlake tempFlake = new SnowFlake(size, rand.Next(0, ((int)mainGrid.ActualWidth - size)), (rand.NextDouble()*5)+1);

                    //add stuff to screen
                    snowflakes.Add(tempFlake);
                    Ellipse flaky = tempFlake.flake;
                    Canvas.SetTop(flaky, tempFlake.y);
                    Canvas.SetLeft(flaky, tempFlake.x);
                    flaky.Opacity = 0.6;
                    mainGrid.Children.Add(flaky);
                }

                //old flakes revamp
                Image tempy = new Image();
                tempy.Width = rand.Next(5, 50);
                tempy.Source = new BitmapImage(Images[rand.Next(0, (Images.Count()-1))]);
                Canvas.SetTop(tempy, -100);
                Canvas.SetLeft(tempy, rand.Next(0, (int)mainGrid.ActualWidth));
                mainGrid.Children.Add(tempy);
                oldFlakeRevamp.Add(tempy);
                counter = 0;


            }
            else
            {
                counter++;
            }

            //move stuff
            //old flakes
            /*for (int i = 0; i < flakes.Count(); i++ )
            {
                ContentControl flake = flakes[i];
                Canvas.SetTop(flake, Canvas.GetTop(flake) + 3);
                if (Canvas.GetTop(flake) >= mainGrid.ActualHeight)
                {
                    removeFlake(flake);
                };
            }*/
            //elipse flakes
            for (int i = 0; i < snowflakes.Count(); i++)
            {
                SnowFlake flake = snowflakes[i];
                Ellipse flakeElipse = flake.flake;
                Canvas.SetTop(flakeElipse,
                Canvas.GetTop(flakeElipse) + flake.fallspeed);
                if (Canvas.GetTop(flakeElipse) >= mainGrid.ActualHeight)
                {
                    removeSnowFlake(flake);
                }
            }
            //revamp flakes
            for (int i = 0; i < oldFlakeRevamp.Count(); i++)
            {
                Canvas.SetTop(oldFlakeRevamp[i], Canvas.GetTop(oldFlakeRevamp[i]) + 3);
                if (Canvas.GetTop(oldFlakeRevamp[i]) >= mainGrid.ActualHeight)
                {
                    removeImageFlake(oldFlakeRevamp[i]);
                }
            }

                //scan for new images
                if (scannerCounter >= scanForImages)
                {
                    string directory = "/images/flakes/";
                    directory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + directory;
                    Images.Clear();
                    foreach (string file in Directory.GetFiles(directory).Where(f => f.EndsWith(".png")))
                    {
                        Images.Add(new Uri(file));
                    }
                    scannerCounter = 0;
                }
                else
                {
                    scannerCounter++;
                }
        }
        #endregion

        #region remove methods

        private void removeImageFlake(Image image)
        {
            mainGrid.Children.Remove(image);
            oldFlakeRevamp.Remove(image);
        }
        private void removeFlake(ContentControl flake)
        {
            mainGrid.Children.Remove(flake);
            flakes.Remove(flake);
        }
        private void removeSnowFlake(SnowFlake flake){
            mainGrid.Children.Remove(flake.flake);
            snowflakes.Remove(flake);
        }

        #endregion

        #region clickthrough-able window
        protected override void OnSourceInitialized(EventArgs e)
        {
            // Get this window's handle         
            IntPtr hwnd = new WindowInteropHelper(this).Handle;
            // Change the extended window style to include WS_EX_TRANSPARENT         
            int extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
            SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle | WS_EX_TRANSPARENT);
            base.OnSourceInitialized(e);
        }
        public const int WS_EX_TRANSPARENT = 0x00000020; public const int GWL_EXSTYLE = (-20);

        [DllImport("user32.dll")]
        public static extern int GetWindowLong(IntPtr hwnd, int index);

        [DllImport("user32.dll")]
        public static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);
        #endregion

        #region context menu
        #region snowflake context menu
        private void snowFlakeAmountChange(object sender, RoutedEventArgs e)
        {
            RadioButton button = (sender as RadioButton);
            switch (button.Content.ToString())
            {
                case "almost none":
                    spawn = 100;
                    clusterSize = 3;
                    break;
                case "Few":
                    spawn = 50;
                    clusterSize = 3;
                    break;
                case "Medium":
                    spawn = 25;
                    clusterSize = 4;
                    break;
                case "Many":
                    spawn = 2;
                    clusterSize = 5;
                    break;
                case "Extreme":
                    spawn = 1;
                    clusterSize = 50;
                    break;
            }

        }
        #endregion

        #region santa context menu
        private void SantaVisibility(object sender, RoutedEventArgs e)
        {
            CheckBox checker = (sender as CheckBox);
            if ((bool)checker.IsChecked)
            {
                santaImg.Visibility = Visibility.Visible;
            }
            else
            {
                santaImg.Visibility = Visibility.Hidden;
            }
        }

        private void santaStuff(object sender, RoutedEventArgs e)
        {
            /*animation.AccelerationRatio +=2;
            animation.SpeedRatio += 2;
            animation.Duration = animation.Duration.Subtract(new Duration(TimeSpan.FromSeconds(1)));
            santaImg.BeginAnimation(Window.LeftProperty,animation);*/
            santaImg.BeginAnimation(Window.LeftProperty, null);
        }

        #endregion

        private void AlwaysOnTopClick(object sender, RoutedEventArgs e)
        {
            CheckBox checker = (sender as CheckBox);
            if ((bool)checker.IsChecked)
            {
                Application.Current.MainWindow.Topmost = true;
            }
            else
            {
                Application.Current.MainWindow.Topmost = false;
            }
        }

        #endregion
    }
}
