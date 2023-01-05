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
using System.Windows.Shapes;

namespace Blackjack
{
    /// <summary>
    /// Interaction logic for Playlist_Window.xaml
    /// </summary>
    public partial class Playlist_Window : Window
    {
        /*  Default Colours:
         *  Active = #5e548e
         *  Not Active = #9f86c0
         */
        Color InActiveColor = (Color)ColorConverter.ConvertFromString("#9f86c0");
        Color ActiveColor = (Color)ColorConverter.ConvertFromString("#5e548e");
        public Playlist_Window()
        {
            InitializeComponent();
            //Convert colours to BRUSH
            SolidColorBrush ActiveBrush = new SolidColorBrush(ActiveColor);

            //Switch Active Colour
            switch (Properties.Settings.Default.Music)
            {
                case "The_Holy_Queen":
                    {
                        BorderHolyQueen.Background = ActiveBrush;
                        break;
                    }
                case "Goldenvengeance":
                    {
                        BorderGoldenvengeance.Background = ActiveBrush;
                        break;
                    }
                case "Wandering_Rose":
                    {
                        BorderWanderingRose.Background = ActiveBrush;
                        break;
                    }
                case "Jazz":
                    {
                        BorderJazz.Background = ActiveBrush;
                        break;
                    }
                case "Jazz2":
                    {
                        BorderJazz2.Background = ActiveBrush;
                        break;
                    }
            }
        }

        private void Image_MouseEnter(object sender, MouseEventArgs e)
        {
            //Wie is de sender?
            Image cover = (Image)sender;

            //Vergroot/Verklein Cover foto
            if (cover.Width == 90)
            {
                cover.Width = 400;

                //Scroll to Image
                switch (cover.Name)
                {
                    case "HolyQueenImage":
                        {
                            ScrollBar.ScrollToVerticalOffset(30);
                            break;
                        }
                    case "GoldenvengeanceImage":
                        {
                            ScrollBar.ScrollToVerticalOffset(130);
                            break;
                        }
                    case "WanderingRoseImage":
                        {
                            ScrollBar.ScrollToVerticalOffset(230);
                            break;
                        }
                    case "JazzImage":
                        {
                            ScrollBar.ScrollToVerticalOffset(340);
                            break;
                        }
                    case "JazzImage2":
                        {
                            ScrollBar.ScrollToVerticalOffset(450);
                            break;
                        }
                }
            }
            else
            {
                cover.Width = 90;

                //Scroll to Image
                switch (cover.Name)
                {
                    case "HolyQueenImage":
                        {
                            ScrollBar.ScrollToVerticalOffset(0);
                            break;
                        }
                    case "GoldenvengeanceImage":
                        {
                            ScrollBar.ScrollToVerticalOffset(50);
                            break;
                        }
                    case "WanderingRoseImage":
                        {
                            ScrollBar.ScrollToVerticalOffset(200);
                            break;
                        }
                    case "JazzImage":
                        {
                            ScrollBar.ScrollToVerticalOffset(200);
                            break;
                        }
                    case "JazzImage2":
                        {
                            ScrollBar.ScrollToVerticalOffset(200);
                            break;
                        }
                }
            }

            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //Convert colours to BRUSH
            SolidColorBrush ActiveBrush = new SolidColorBrush(ActiveColor);

            //Reset Panel Backgrounds
            ResetPanelColours();

            //Wie is de sender?
            Button btn = (Button)sender;

            switch (btn.Name)
            {
                case "BtnHolyQueen":
                    {
                        Properties.Settings.Default.Music = "The_Holy_Queen";
                        BorderHolyQueen.Background = ActiveBrush;
                        break;
                    }
                case "BtnGoldenvengeance":
                    {
                        Properties.Settings.Default.Music = "Goldenvengeance";
                        BorderGoldenvengeance.Background = ActiveBrush;
                        break;
                    }
                case "BtnWanderingRose":
                    {
                        Properties.Settings.Default.Music = "Wandering_Rose";
                        BorderWanderingRose.Background = ActiveBrush;
                        break;
                    }
                case "BtnJazz":
                    {
                        Properties.Settings.Default.Music = "Jazz";
                        BorderJazz.Background = ActiveBrush;
                        break;
                    }
                case "BtnJazz2":
                    {
                        Properties.Settings.Default.Music = "Jazz2";
                        BorderJazz2.Background = ActiveBrush;
                        break;
                    }
            }
        }

        private void ResetPanelColours()
        {
            //Convert colours to BRUSH
            SolidColorBrush InActiveBrush = new SolidColorBrush(InActiveColor);

            BorderHolyQueen.Background = InActiveBrush;
            BorderGoldenvengeance.Background = InActiveBrush;
            BorderWanderingRose.Background = InActiveBrush;
            BorderJazz.Background = InActiveBrush;
            BorderJazz2.Background = InActiveBrush;
        }
    }
}
