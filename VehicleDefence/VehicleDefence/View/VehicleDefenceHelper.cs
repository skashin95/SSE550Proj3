using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;


namespace VehicleDefence.View
{
    using Model;
    using Windows.UI;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Media;
    using Windows.UI.Xaml.Media.Animation;
    using Windows.UI.Xaml.Media.Imaging;
    using Windows.UI.Xaml.Shapes;

    class VehicleDefenceHelper
    {
        private static readonly Random _random = new Random();

        public static IEnumerable<string> CreateImageList(AircraftType aircraftType)
        {
            string filename;
            switch (aircraftType)
            {
                //TODO find all the appropriate pictures for each aircraft type.
                //TODO name the pictures the appropriate names and make sure they are .png
                case AircraftType.Blimp:
                    filename = "blimp";
                    break;
                case AircraftType.CargoPlane:
                    filename = "cargoplane";
                    break;
                case AircraftType.FighterJet:
                    filename = "fighterjet";
                    break;
                case AircraftType.Helicopter:
                default:
                    filename = "helicopter";
                    break;
            }
            List<string> imageList = new List<string>();
            for (int i = 0; i < 4; i++)
            {
                imageList.Add(filename + i + ".png");
            }
            return imageList;
        }

        internal static FrameworkElement AircraftControlFactory(Aircraft aircraft, double scale)
        {
            IEnumerable<string> imageNames = CreateImageList(aircraft.AircraftType);
            AnimatedImage aircraftControl = new AnimatedImage(imageNames, TimeSpan.FromSeconds(.75));
            aircraftControl.Width = aircraft.Size.Width * scale;
            aircraftControl.Height = aircraft.Size.Height * scale;
            SetCanvasLocation(aircraftControl, aircraft.Location.X * scale, aircraft.Location.Y * scale);

            return aircraftControl;

        }
        
        internal static FrameworkElement ShotControlFactory(Shot shot, double scale)
        {
            Rectangle rectangle = new Rectangle();
            rectangle.Fill = new SolidColorBrush(Colors.Yellow);
            rectangle.Width = shot.ShotSize.Width * scale;
            rectangle.Height = shot.ShotSize.Height * scale;
            SetCanvasLocation(rectangle, shot.Location.X * scale, shot.Location.Y * scale);
            return rectangle;
        }

        public static FrameworkElement ScanLineFactory(int y, int width, double scale)
        {
            Rectangle rectangle = new Rectangle();
            rectangle.Width = width * scale;
            rectangle.Height = 2;
            rectangle.Opacity = .1;
            rectangle.Fill = new SolidColorBrush(Colors.White);
            SetCanvasLocation(rectangle, 0, y * scale);
            return rectangle;
        }

        internal static FrameworkElement PlayerControlFactory(Player player, double scale)
        {
            AnimatedImage playerControl = new AnimatedImage(new List<string>() {"player.png","player.png"}, TimeSpan.FromSeconds(1));
            playerControl.Width = player.Size.Width * scale;
            playerControl.Height = player.Size.Height * scale;
            SetCanvasLocation(playerControl, player.Location.X * scale, player.Location.Y * scale);
            return playerControl;
        }

        public static void SetCanvasLocation(FrameworkElement control, double x, double y)
        {
            Canvas.SetLeft(control, x);
            Canvas.SetTop(control,y);
        }

        public static void MoveElementOnCanvas(FrameworkElement FrameworkElement, double toX, double toY)
        {
            double fromX = Canvas.GetLeft(FrameworkElement);
            double fromY = Canvas.GetTop(FrameworkElement);

            Storyboard storyboard = new Storyboard();
            DoubleAnimation animationX = CreateDoubleAnimation(FrameworkElement, fromX, toX, "(Canvas.Left)");
            DoubleAnimation animationY = CreateDoubleAnimation(FrameworkElement, fromY, toY, "(Canvas.Top)");

            storyboard.Children.Add(animationX);
            storyboard.Children.Add(animationY);
            storyboard.Begin();
        }

        public static DoubleAnimation CreateDoubleAnimation(FrameworkElement frameworkElement, double from, double to, string propertyToAnimate)
        {
            return CreateDoubleAnimation(frameworkElement, from, to, propertyToAnimate, TimeSpan.FromMilliseconds(25));
        }

        public static DoubleAnimation CreateDoubleAnimation(FrameworkElement frameworkElement, double from, double to, string propertyToAnimate, TimeSpan timeSpan)
        {
            DoubleAnimation animation = new DoubleAnimation();
            Storyboard.SetTarget(animation, frameworkElement);
            Storyboard.SetTargetProperty(animation, propertyToAnimate);
            animation.From = from;
            animation.To = to;
            animation.Duration = timeSpan;
            return animation;
        }

        public static void ResizeElement(FrameworkElement control, double width, double height)
        {
            if (control.Width != width)
                control.Width = width;
            if (control.Height != height)
                control.Height = height;
        }

        public static BitmapImage CreateImageFromAssets(string imageFilename)
        {
            return new BitmapImage(new Uri("ms-appx:///Assets" + imageFilename));
        }
    }
}
