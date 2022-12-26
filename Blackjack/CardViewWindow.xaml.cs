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
    /// Interaction logic for CardViewWindow.xaml
    /// </summary>
    public partial class CardViewWindow : Window
    {
        public CardViewWindow()
        {
            InitializeComponent();
            //Change Window name to card name.
            string WindowName = Properties.Settings.Default.CardName;
            WindowName = string.Concat(WindowName.Replace("-", " "));
            Title = WindowName;
            //Display the image
            CardImage.Source = new BitmapImage(new Uri($"/Cards/{Properties.Settings.Default.CardName}.png", UriKind.Relative));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
