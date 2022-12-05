using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Blackjack
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //clubs (♣), diamonds (♦), hearts (♥) and spades (♠)
        public MainWindow()
        {
            InitializeComponent();
        }


        //Lijsten
        List<string> Deck = new List<string>(); //ALL 52 CARDS IN THE GAME
        List<string> CardsInGame = new List<string>(); //ALL CARDS IN USE (player and cpu combined)
        List<string> PlayerDeck = new List<string>(); //All CARDS PLAYER
        List<string> CpuDeck = new List<string>(); //ALL CARDS CPU
        List<int> PlayerHandValue = new List<int>(); //ALL CARD VALUES FROM PLAYER
        List<int> CpuHandValue = new List<int>(); //ALL CARD VALUES FROM CPU
        //Andere Variabelen
        Random random = new Random();
        bool Soft17Clear = false;
        int PlayerValue;
        int CpuValue;
        string CardName;


        public Image Card(bool Player)
        {
            int LeftMargin = 0; //Used as the margin
            int Height = 150;
            int Width = 105;

            //Match margin so they stack on top of each other. [1st card margin 0, all others -85]
            if (Player)
            {
                if (SpnlPlayerDeck.Children.Count != 0)
                {
                    LeftMargin = -85;
                }
            }
            else
            {
                if (SpnlCpuDeck.Children.Count != 0)
                {
                    LeftMargin = -85;
                }
            }

            //Make secret card bigger so it matches other card sizes
            if (CardName == "Card-Back")
            {
                Height = 155;
                Width = 110;
            }
            
            //Create Card
            Image Card = new Image
            {
                Height = Height,
                Width = Width,
                Stretch = Stretch.UniformToFill,
                Source = new BitmapImage(new Uri($"/Cards/{CardName}.png", UriKind.Relative)),
                Margin = new Thickness(LeftMargin, 0, 0, 0)
            };

            return Card;
        }

        public int PullCard()
        {
            //Pull a random card that does not exist in the game yet
            int index = random.Next(0, 52);

            while (CardsInGame.Contains(Deck.ElementAt(index)))
            {
                index = random.Next(0, 52);
            }

            return index;
        }

        private void CreerDeck(object sender, RoutedEventArgs e)
        {
            //Voeg alle 52 kaarten in Deck
            Deck.Add("Clubs-Ace");
            Deck.Add("Diamonds-Ace");
            Deck.Add("Hearts-Ace");
            Deck.Add("Spades-Ace"); //3
            Deck.Add("Clubs-Two");
            Deck.Add("Diamonds-Two");
            Deck.Add("Hearts-Two");
            Deck.Add("Spades-Two"); //7
            Deck.Add("Clubs-Three");
            Deck.Add("Diamonds-Three");
            Deck.Add("Hearts-Three");
            Deck.Add("Spades-Three"); //11
            Deck.Add("Clubs-Four");
            Deck.Add("Diamonds-Four");
            Deck.Add("Hearts-Four");
            Deck.Add("Spades-Four"); //15
            Deck.Add("Clubs-Five");
            Deck.Add("Diamonds-Five");
            Deck.Add("Hearts-Five");
            Deck.Add("Spades-Five"); //19
            Deck.Add("Clubs-Six");
            Deck.Add("Diamonds-Six");
            Deck.Add("Hearts-Six");
            Deck.Add("Spades-Six"); //23
            Deck.Add("Clubs-Seven");
            Deck.Add("Diamonds-Seven");
            Deck.Add("Hearts-Seven");
            Deck.Add("Spades-Seven"); //27
            Deck.Add("Clubs-Eight");
            Deck.Add("Diamonds-Eight");
            Deck.Add("Hearts-Eight");
            Deck.Add("Spades-Eight"); //31
            Deck.Add("Clubs-Nine");
            Deck.Add("Diamonds-Nine");
            Deck.Add("Hearts-Nine");
            Deck.Add("Spades-Nine"); //35
            Deck.Add("Clubs-Ten");
            Deck.Add("Diamonds-Ten");
            Deck.Add("Hearts-Ten");
            Deck.Add("Spades-Ten"); //39
            Deck.Add("Clubs-Jack");
            Deck.Add("Diamonds-Jack");
            Deck.Add("Hearts-Jack");
            Deck.Add("Spades-Jack"); //43
            Deck.Add("Clubs-Queen");
            Deck.Add("Diamonds-Queen");
            Deck.Add("Hearts-Queen");
            Deck.Add("Spades-Queen"); //47
            Deck.Add("Clubs-King");
            Deck.Add("Diamonds-King");
            Deck.Add("Hearts-King");
            Deck.Add("Spades-King"); //51
        }

        private void Reset_Table()
        {
            //Maak lijst leeg
            //Cards
            CardsInGame.Clear();
            PlayerDeck.Clear();
            CpuDeck.Clear();

            //Cards Value
            PlayerHandValue.Clear();
            CpuHandValue.Clear();

            //CardDisplay
            SpnlPlayerDeck.Children.Clear();
            SpnlCpuDeck.Children.Clear();

            //Reseting values
            Soft17Clear = false;
            TxtResults.Text = "...";
            TxtResults.Foreground = Brushes.White;
            TxtPlayerIcon.Text = "🤔";
            LblCpuScore.FontWeight = FontWeights.Regular;
            LblPlayerScore.FontWeight = FontWeights.Regular;
        }

        private void Button_Enabling(bool Deel, bool Hit, bool Stand)
        {
            if (Deel)
            {
                BtnDeel.IsEnabled = true;
            }
            else
            {
                BtnDeel.IsEnabled = false;
            }

            if (Hit)
            {
                BtnHit.IsEnabled = true;
            }
            else
            {
                BtnHit.IsEnabled = false;
            }

            if (Stand)
            {
                BtnStand.IsEnabled = true;
            }
            else
            {
                BtnStand.IsEnabled = false;
            }
        }

        private void UpdateDisplayScore()
        {
            LblPlayerScore.Content = PlayerValue.ToString();
            LblCpuScore.Content = CpuValue.ToString();
        }

        private async void BtnDeel_Click(object sender, RoutedEventArgs e)
        {
            //Table preparation
            Button_Enabling(false, false, false);
            Reset_Table();
            LblPlayerScore.Content = "...";
            LblCpuScore.Content = "...";

            //Verdeel 2 kaarten
            for (int i = 0; i < 2; i++)
            {
                AddCard(PullCard(), true);
                UpdateDisplayScore();
                await Task.Delay(500);
            }

            //Fix overflow to Avoid getting 22
            FixOverFlow(sender, e, true);

            //GiveCpuCard
            AddCard(PullCard(), false);
            UpdateDisplayScore();
            await Task.Delay(500);
            //Secret Card
            CardName = "Card-Back";
            SpnlCpuDeck.Children.Add(Card(false));

            //Did we get a 21?
            if (PlayerValue == 21)
            {
                Cpu_Turn(sender, e);
            }

            //Re-enable the needed buttons
            Button_Enabling(false, true, true);
        }

        private void BtnHit_Click(object sender, RoutedEventArgs e)
        {
            AddCard(PullCard(), true);
            FixOverFlow(sender, e, true);

            if (PlayerValue >= 21)
            {
                Cpu_Turn(sender, e);
            }

            //Display
            UpdateDisplayScore();
        }

        private void BtnStand_Click(object sender, RoutedEventArgs e)
        {
            //Start CPU's turn
            Cpu_Turn(sender, e);
        }


        private void AddCard(int index, bool Player)
        {
            //Set CardName to pulled card
            CardName = Deck.ElementAt(index);
            //Add card to the game (Card List)
            CardsInGame.Add(Deck.ElementAt(index));

            //Who pulled the card?
            if (Player)
            {
                SpnlPlayerDeck.Children.Add(Card(Player));
                PlayerDeck.Add(Deck.ElementAt(index));
                PlayerHandValue.Add(CardValue());
            }
            else
            {
                SpnlCpuDeck.Children.Add(Card(Player));
                CpuDeck.Add(Deck.ElementAt(index));
                CpuHandValue.Add(CardValue());
            }

            DeckValue();
        }

        void FixOverFlow(object sender, RoutedEventArgs e, bool Player)
        {
            if (Player)
            {
                if (PlayerValue > 21)
                {
                    TurnAceInto1(sender, e, Player);
                }
            }
            else
            {
                if (CpuValue > 21)
                {
                    TurnAceInto1(sender, e, Player);
                }
            }

            DeckValue();
        }

        public int CardValue()
        {
            if (CardName.Contains("Ace"))
            {
                return 11;
            }
            else if (CardName.Contains("Two"))
            {
                return 2;
            }
            else if (CardName.Contains("Three"))
            {
                return 3;
            }
            else if (CardName.Contains("Four"))
            {
                return 4;
            }
            else if (CardName.Contains("Five"))
            {
                return 5;
            }
            else if (CardName.Contains("Six"))
            {
                return 6;
            }
            else if (CardName.Contains("Seven"))
            {
                return 7;
            }
            else if (CardName.Contains("Eight"))
            {
                return 8;
            }
            else if (CardName.Contains("Nine"))
            {
                return 9;
            }
            else if (CardName.Contains("Ten") || CardName.Contains("Jack") || CardName.Contains("Queen") || CardName.Contains("King"))
            {
                return 10;
            }
            else
            {
                return -1;
            }
        }

        private void DeckValue()
        {
            //For the Player
            PlayerValue = 0;
            foreach (var value in PlayerHandValue)
            {
                PlayerValue += value;
            }

            //For Cpu
            CpuValue = 0;
            foreach (var value in CpuHandValue)
            {
                CpuValue += value;
            }
        }

        private void TurnAceInto1(object sender, RoutedEventArgs e, bool Player)
        {
            if (Player)
            {
                if (PlayerHandValue.Contains(11)) //can we switch 11 with 1?
                {
                    PlayerHandValue.Remove(11);
                    PlayerHandValue.Add(1);
                }
            }
            else //For CPU
            {
                if (CpuHandValue.Contains(11)) //can we switch 11 with 1?
                {
                    CpuHandValue.Remove(11);
                    CpuHandValue.Add(1);
                }
            }
            DeckValue();
        }

        private async void Cpu_Turn(object sender, RoutedEventArgs e)
        {
            //Enabling da buttons
            Button_Enabling(false, false, false);

            //Wait
            await Task.Delay(1200);
            //Remove Secret Card
            SpnlCpuDeck.Children.RemoveAt(SpnlCpuDeck.Children.Count - 1);

            //Keep pulling cards until we reach 17+
            DeckValue();
            while (CpuValue < 17)
            {
                AddCard(PullCard(), false);
                DeckValue();
                UpdateDisplayScore();
                await Task.Delay(500);
            }

            //Does CPU have a blackjack?
            if (CpuValue == 21)
            {
                Match_Results();
                return;
            }

            //Do we have a soft 17?
            if (!Soft17Clear) //only clear if we haven't already
            {
                CheckSoft17(sender, e);
                DeckValue();
                UpdateDisplayScore();
            }

            //Keep pulling cards until we reach 17+
            while (CpuValue < 17)
            {
                AddCard(PullCard(), false);
                DeckValue();
                UpdateDisplayScore();
                await Task.Delay(500);
            }

            //Wait
            await Task.Delay(250);

            //Who won the game now?
            Match_Results();
        }

        private void CheckSoft17(object sender, RoutedEventArgs e)
        {
            //Is there an ace?
            if (CpuHandValue.Contains(11))
            {
                CpuHandValue.Remove(11);
                CpuHandValue.Add(1);
                Soft17Clear = true;
            }
        }

        private void Match_Results()
        {
            string Result;

            //Calculate Result
            if (CpuValue == 21 && PlayerValue != 21)
            {
                Result = "Cpu";
            }
            else if (CpuValue == 21 && PlayerValue == 21)
            {
                Result = "Draw";
            }
            else if (CpuValue > 21 && PlayerValue > 21)
            {
                Result = "Draw";
            }
            else if (CpuValue == PlayerValue)
            {
                Result = "Draw";
            }
            else if (CpuValue > 21)
            {
                Result = "Player";
            }
            else if (CpuValue > PlayerValue)
            {
                Result = "Cpu";
            }
            else if (PlayerValue > 21)
            {
                Result = "Cpu";
            }
            else
            {
                Result = "Player";
            }

            //Display Result
            if (Result == "Player")
            {
                TxtResults.Text = "Win";
                TxtPlayerIcon.Text = "😎";
                LblPlayerScore.FontWeight = FontWeights.Bold;
                TxtResults.Foreground = Brushes.Green;
            }
            else if (Result == "Cpu")
            {
                TxtResults.Text = "Lose";
                TxtPlayerIcon.Text = "😢";
                LblCpuScore.FontWeight = FontWeights.Bold;
                TxtResults.Foreground = Brushes.Red;
            }
            else
            {
                TxtResults.Text = "Draw";
                TxtPlayerIcon.Text = "😅";
                LblPlayerScore.FontWeight = FontWeights.Bold;
                LblCpuScore.FontWeight = FontWeights.Bold;
                TxtResults.Foreground = Brushes.DarkOrange;
            }

            //Enabling da buttons
            Button_Enabling(true, false, false);
        }
    }
}
