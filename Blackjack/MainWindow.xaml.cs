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
using System.Windows.Documents;
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
            Reset_Table();
            ResizeDeck();
            MoneyRelatedActions();

            for (int i= 0; i < HistoriekLijst.Rank; i++)
            {
                HistoriekLijst[i, 0] = $"NULL{i}";
            }
        }

        //Lijsten
        List<string> Deck = new List<string>() 
        {   "Clubs-Ace",     "Diamonds-Ace",     "Hearts-Ace",   "Spades-Ace",   //3
            "Clubs-Two",     "Diamonds-Two",     "Hearts-Two",   "Spades-Two",   //7
            "Clubs-Three",   "Diamonds-Three",   "Hearts-Three", "Spades-Three", //11
            "Clubs-Four",    "Diamonds-Four",    "Hearts-Four",  "Spades-Four",  //15
            "Clubs-Five",    "Diamonds-Five",    "Hearts-Five",  "Spades-Five",  //19
            "Clubs-Six",     "Diamonds-Six",     "Hearts-Six",   "Spades-Six",   //23
            "Clubs-Seven",   "Diamonds-Seven",   "Hearts-Seven", "Spades-Seven", //27
            "Clubs-Eight",   "Diamonds-Eight",   "Hearts-Eight", "Spades-Eight", //31
            "Clubs-Nine",    "Diamonds-Nine",    "Hearts-Nine",  "Spades-Nine",  //35
            "Clubs-Ten",     "Diamonds-Ten",     "Hearts-Ten",   "Spades-Ten",   //39
            "Clubs-Jack",    "Diamonds-Jack",    "Hearts-Jack",  "Spades-Jack",  //43
            "Clubs-Queen",   "Diamonds-Queen",   "Hearts-Queen", "Spades-Queen", //47
            "Clubs-King",    "Diamonds-King",    "Hearts-King",  "Spades-King"   //51
        };
        List<string> CardsInGame = new List<string>(); //ALL CARDS IN USE (player and cpu combined)
        List<string> PlayerDeck = new List<string>(); //All CARDS PLAYER
        List<string> PlayerDeck2 = new List<string>(); //Second deck for player (split)
        List<string> CpuDeck = new List<string>(); //ALL CARDS CPU
        List<int> PlayerHandValue = new List<int>(); //ALL CARD VALUES FROM PLAYER
        List<int> PlayerHandValue2 = new List<int>(); //Card values for second deck (player)
        List<int> CpuHandValue = new List<int>(); //ALL CARD VALUES FROM CPU
        string[,] HistoriekLijst = new string[9, 0];
        //BETTING
        int Money = 100;
        int Bet = 0;
        int ActiveDeck = 1; //Welke Deck word nu gebruikt? (speler)
        int Ronde = 0;
        //Andere Variabelen
        Random random = new Random();
        bool Soft17Clear = false;
        string CardName;


        public Image Card(bool Player, int Deck)
        {
            int LeftMargin = 0; //Used as the margin
            int Height = 150;
            int Width = 105;

            //Match margin so they stack on top of each other. [1st card margin 0, all others -85]
            if (Player)
            {
                if (PlayerDeckPanel.Children.Count != 0 && Deck == 1)
                {
                    LeftMargin = -85;
                }
                else if (PlayerDeckPanel2.Children.Count != 0 && Deck == 2)
                {
                    LeftMargin = -85;
                }
            }
            else
            {
                if (CpuDeckPanel.Children.Count != 0)
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

        private void Reset_Table()
        {
            //Reset Deck and values
            PlayerDeck.Clear();
            PlayerDeck2.Clear();
            CpuDeck.Clear();
            PlayerHandValue.Clear();
            PlayerHandValue2.Clear();
            CpuHandValue.Clear();

            //Cards on screen
            PlayerDeckPanel.Children.Clear();
            PlayerDeckPanel2.Children.Clear();
            CpuDeckPanel.Children.Clear();

            //ICONS
            IconResize(null);

            //other values
            Soft17Clear = false;
            TxtResults.Foreground = Brushes.White;
            TxtPlayerIcon.Text = "🤔";
            LblCpuScore.FontWeight = FontWeights.Regular;
            LblPlayerScore.FontWeight = FontWeights.Regular;

            //Undo Split settings
            ActiveDeck = 1;
            HighlightActiveDeck();
            RepositionDeckPanel(false);
        }

        private void IconResize(bool? Player)
        {
            if (!Player.HasValue)
            {
                TxtPlayerIcon.FontSize = 82;
                PlayerEllipse.Height = 120;
                PlayerEllipse.Width = 120;
                PlayerIconPanel.Height = 120;
                PlayerIconPanel.Width = 120;

                TxtCpuIcon.FontSize = 82;
                CpuEllipse.Height = 120;
                CpuEllipse.Width = 120;
                CpuIconPanel.Height = 120;
                CpuIconPanel.Width = 120;
                return;
            }

            if (Player.Value == true)
            {
                TxtPlayerIcon.FontSize = 112;
                PlayerEllipse.Height = 160;
                PlayerEllipse.Width = 160;
                PlayerIconPanel.Height = 160;
                PlayerIconPanel.Width = 160;

                TxtCpuIcon.FontSize = 82;
                CpuEllipse.Height = 120;
                CpuEllipse.Width = 120;
                CpuIconPanel.Height = 120;
                CpuIconPanel.Width = 120;
            }
            else
            {
                TxtCpuIcon.FontSize = 112;
                CpuEllipse.Height = 160;
                CpuEllipse.Width = 160;
                CpuIconPanel.Height = 160;
                CpuIconPanel.Width = 160;

                TxtPlayerIcon.FontSize = 82;
                PlayerEllipse.Height = 120;
                PlayerEllipse.Width = 120;
                PlayerIconPanel.Height = 120;
                PlayerIconPanel.Width = 120;
            }
        }

        private void Button_Enabling(string ButtonNames)
        {
            BtnDeel.Visibility = ButtonNames.Contains("Deel") ? Visibility.Visible : Visibility.Collapsed;
            BtnHit.Visibility = ButtonNames.Contains("Hit") ? Visibility.Visible : Visibility.Collapsed;
            BtnStand.Visibility = ButtonNames.Contains("Stand") ? Visibility.Visible : Visibility.Collapsed;
            BtnChangeBet.Visibility = ButtonNames.Contains("ChangeBet") ? Visibility.Visible : Visibility.Collapsed;
            BtnContinue.Visibility = ButtonNames.Contains("Continue") ? Visibility.Visible : Visibility.Collapsed;
            BtnNewGame.Visibility = ButtonNames.Contains("NewGame") ? Visibility.Visible : Visibility.Collapsed;
            BtnAllIn.Visibility = ButtonNames.Contains("AllIn") ? Visibility.Visible : Visibility.Collapsed;
            BtnSplit.Visibility = ButtonNames.Contains("Split") ? Visibility.Visible : Visibility.Collapsed;
            BtnDoubleDown.Visibility = ButtonNames.Contains("Double") ? Visibility.Visible : Visibility.Collapsed;
        }

        private void UpdateDisplayScore()
        {
            if (ActiveDeck == 1)
            {
                LblPlayerScore.Content = PlayerHandValue.Sum().ToString();
            }
            else
            {
                LblPlayerScore.Content = PlayerHandValue2.Sum().ToString();
            }

            LblCpuScore.Content = CpuHandValue.Sum().ToString();
        }

        private void ResizeDeck()
        {
            int PlayerSize = 0;
            int PlayerSize2 = 0;
            int CpuSize = 0;

            if (PlayerDeckPanel.Children.Count > 0)
            {
                PlayerSize = 110;
            }
            if (PlayerDeckPanel2.Children.Count > 0)
            {
                PlayerSize2 = 110;
            }
            if (CpuDeckPanel.Children.Count > 0)
            {
                CpuSize = 110;
            }

            foreach (var item in PlayerDeckPanel.Children)
            {
                PlayerSize += 20;
            }
            foreach (var item in PlayerDeckPanel2.Children)
            {
                PlayerSize2 += 20;
            }
            foreach (var item in CpuDeckPanel.Children)
            {
                CpuSize += 20;
            }

            PlayerDeckPanelBorder.Width = PlayerSize;
            PlayerDeckPanelBorder2.Width = PlayerSize2;
            CpuDeckPanelBorder.Width = CpuSize;
        }
        
        private void UpdateResultText(string Text, string TextColor)
        {
            //Display Text
            TxtResults.Text = Text;

            //Change TextColor
            switch (TextColor)
            {
                case "White":
                    {
                        TxtResults.Foreground = Brushes.White;
                        break;
                    }
                case "Red":
                    {
                        TxtResults.Foreground = Brushes.Red;
                        break;
                    }
                case "Green":
                    {
                        TxtResults.Foreground = Brushes.Green;
                        break;
                    }
                case "Orange":
                    {
                        TxtResults.Foreground = Brushes.Orange;
                        break;
                    }
                case "Gold":
                    {
                        TxtResults.Foreground = Brushes.Gold;
                        break;
                    }
            }
        }

        private async void DisplayDeck(bool ShowPlayerDeck)
        {
            //If PlayerDeck needs to be displayed, it means betpanel is on screen, so we shift betpanel away and pull playerdeck.
            //Betpanel on screen: Bottom-Margin = 10; Offscreen: Bottom-Margin = -200;
            //PlayerDeckPanelBorder and PlayerDeckPanel Only need visibility change When getting Displayed, ELSE same rules as The Table.
            //Only Playertable moves up. On screen margin = 0; Offscreen margin = 0, 200, 0, -200
            //CPU's Deck fades away. Panel = 1; Border = 0.4;
            int BetMargin;
            int DeckMargin;
            double DeckPanelOpacity = CpuDeckPanel.Opacity;
            double PanelBorderOpacity = CpuDeckPanelBorder.Opacity;

            //Prepare Table for next game
            Reset_Table();
            ResizeDeck();
            UpdateDisplayScore();
            MoneyRelatedActions();

            if (ShowPlayerDeck)
            {
                //Move BetPanel Down
                BetMargin = 10;
                while(BetMargin > -380)
                {
                    BetMargin -= 15;
                    BetPanel.Margin = new Thickness(0, 0, 0, BetMargin);
                    BetPanelBorder.Margin = new Thickness(0, 0, 0, BetMargin);
                    await Task.Delay(25);
                }

                //CPU DECK VISIBLE
                CpuDeckPanel.Opacity = 1;
                CpuDeckPanelBorder.Opacity = 0.4;

                //Move PlayerDeck Up
                DeckMargin = 200;
                while(DeckMargin > 0)
                {
                    DeckMargin -= 10;
                    PlayerTable.Margin = new Thickness(0, DeckMargin, 0, (DeckMargin - (DeckMargin * 2)));
                    await Task.Delay(25);
                }

                PlayerDeckPanel.Margin = new Thickness(0);
                PlayerDeckPanelBorder.Margin = new Thickness(0);
                PlayerDeckPanel2.Margin = new Thickness(0);
                PlayerDeckPanelBorder2.Margin = new Thickness(0);
                PlayerDeckPanel.Visibility = Visibility.Visible;
                PlayerDeckPanelBorder.Visibility = Visibility.Visible;

                //Show buttons
                Button_Enabling("Deel");

                //Edit Result Text
                await Task.Delay(2000);
                if (PlayerDeckPanel.Children.Count == 0)
                {
                    UpdateResultText("Make a Play.", "White");
                }
            }
            else
            {
                //Move PlayerDeck down AND CPU DECK FADE
                DeckMargin = 0;
                while (DeckMargin <= 200)
                {
                    DeckMargin += 10;
                    PlayerTable.Margin = new Thickness(0, DeckMargin, 0, (DeckMargin - (DeckMargin * 2)));
                    PlayerDeckPanelBorder.Margin = new Thickness(0, DeckMargin, 0, (DeckMargin - (DeckMargin * 2)));
                    PlayerDeckPanel.Margin = new Thickness(0, DeckMargin, 0, (DeckMargin - (DeckMargin * 2)));
                    PlayerDeckPanelBorder2.Margin = new Thickness(0, DeckMargin, 0, (DeckMargin - (DeckMargin * 2)));
                    PlayerDeckPanel2.Margin = new Thickness(0, DeckMargin, 0, (DeckMargin - (DeckMargin * 2)));

                    if (DeckPanelOpacity != 0)
                    {
                        DeckPanelOpacity -= 0.05;
                        CpuDeckPanel.Opacity = DeckPanelOpacity;
                        PanelBorderOpacity -= 0.02;
                        CpuDeckPanelBorder.Opacity = PanelBorderOpacity;
                    }
                    await Task.Delay(25);
                }

                //Edit Result text
                UpdateResultText("Place your bet!", "White");

                //Move BetPanel Up
                BetMargin = -390;
                while (BetMargin < 10)
                {
                    BetMargin += 15;
                    BetPanel.Margin = new Thickness(0, 0, 0, BetMargin);
                    BetPanelBorder.Margin = new Thickness(0, 0, 0, BetMargin);
                    await Task.Delay(25);
                }
            }
        }

        private async void BtnDeel_Click(object sender, RoutedEventArgs e)
        {
            //Table preparation
            Button_Enabling("");
            Reset_Table();
            LblPlayerScore.Content = "...";
            LblCpuScore.Content = "...";
            UpdateResultText("...", "White");
            Ronde++;

            //Verdeel 2 kaarten
            for (int i = 0; i < 2; i++)
            {
                //AddCard(PullCard(), true, 1);
                AddCard(PullCard(), true, 1);
                UpdateDisplayScore();
                await Task.Delay(500);
            }

            //Fix overflow to Avoid getting 22
            FixOverFlow(sender, e, true);

            //GiveCpuCard
            AddCard(PullCard(), false, 0);
            UpdateDisplayScore();
            await Task.Delay(500);
            //Secret Card
            CardName = "Card-Back";
            CpuDeckPanel.Children.Add(Card(false, 0));
            ResizeDeck();

            //Did we get a 21?
            if (PlayerHandValue.Sum() == 21)
            {
                Cpu_Turn(sender, e);
                return;
            }

            //Re-enable the needed buttons
            //2de Ace waarde word als 1 opgeslagen dus split word niet herkent in de 2e if statement
            if (PlayerDeck.ElementAt(0).Contains("Ace") && PlayerDeck.ElementAt(1).Contains("Ace") && Money >= Bet * 2) 
            {
                Button_Enabling("Hit Stand Split Double");
            }
            else if (PlayerHandValue.ElementAt(0) == PlayerHandValue.ElementAt(1) && Money >= Bet * 2)
            {
                Button_Enabling("Hit Stand Split Double");
            }
            else if (Money >= Bet * 2)
            {
                Button_Enabling("Hit Stand Double");
            }
            else
            {
                Button_Enabling("Hit Stand");
            }
        }

        private void BtnHit_Click(object sender, RoutedEventArgs e)
        {
            //Hide split or double button
            Button_Enabling("Hit Stand");

            AddCard(PullCard(), true, ActiveDeck);
            FixOverFlow(sender, e, true);

            //Switch to second deck
            if (PlayerHandValue.Sum() >= 21)
            {
                if (PlayerDeck2.Count() == 0) 
                { 
                    Cpu_Turn(sender, e); 
                }
                else
                {
                    ActiveDeck = 2;
                }
            }

            //End 2nd deck's turn if value exceeds 21
            if (PlayerHandValue2.Sum() >= 21)
            {
                ActiveDeck = 1;
                Cpu_Turn(sender, e);
            }

            //Display
            UpdateDisplayScore();
            HighlightActiveDeck();
        }

        private void BtnStand_Click(object sender, RoutedEventArgs e)
        {
            if (ActiveDeck == 2) //If on 2nd deck, start CPU TURN
            {
                ActiveDeck = 1;
                UpdateDisplayScore();
                HighlightActiveDeck();
                Cpu_Turn(sender, e);
                return;
            }

            //Check wether we have an active second deck
            if (PlayerHandValue2.Sum() == 0 && ActiveDeck == 1)
            {
                Cpu_Turn(sender, e);
            }
            else
            {
                ActiveDeck = 2;
                HighlightActiveDeck();
                UpdateDisplayScore();
            }
        }


        private void AddCard(int index, bool Player, int WhichDeck)
        {
            //Set CardName to pulled card
            CardName = Deck.ElementAt(index);
            //Add card to the game (Card List) IF IT'S A NEW CARD
            if (!CardsInGame.Contains(Deck.ElementAt(index)))
            {
                CardsInGame.Add(Deck.ElementAt(index));
            }
            
            //Who pulled the card?
            if (Player)
            {
                if (WhichDeck == 1)
                {
                    PlayerDeckPanel.Children.Add(Card(Player, 1));
                    PlayerDeck.Add(Deck.ElementAt(index));
                    PlayerHandValue.Add(CardValue());
                }
                else
                {
                    PlayerDeckPanel2.Children.Add(Card(Player, 2));
                    PlayerDeck2.Add(Deck.ElementAt(index));
                    PlayerHandValue2.Add(CardValue());
                }
            }
            else
            {
                CpuDeckPanel.Children.Add(Card(Player, 0));
                CpuDeck.Add(Deck.ElementAt(index));
                CpuHandValue.Add(CardValue());
            }

            ResizeDeck();

            //How many cards are left?
            ShuffleDeck();
        }

        private async void ShuffleDeck()
        {
            bool PlayAnimation = false;

            //Reshuffle AllCards if all cards have been played
            if (CardsInGame.Count == Deck.Count())
            {
                CardsInGame.Clear();
                PlayAnimation = true;
                TxtDeckCount.Text = (Deck.Count() - CardsInGame.Count()).ToString();
            }

            if (PlayAnimation)
            {
                int margin = 0;
                ShuffleNotifBorder.Visibility = Visibility.Visible;

                //Animation
                while(margin != -10)
                {
                    await Task.Delay(50);
                    margin--;
                    ShuffleNotifBorder.Margin = new Thickness(margin);
                }

                //Reset
                await Task.Delay(3000);
                ShuffleNotifBorder.Visibility = Visibility.Collapsed;
                ShuffleNotifBorder.Margin = new Thickness(0);
            }

            //Display
            TxtDeckCount.Text = (Deck.Count() - CardsInGame.Count()).ToString();
        }

        void FixOverFlow(object sender, RoutedEventArgs e, bool Player)
        {
            if (Player)
            {
                if (PlayerHandValue.Sum() > 21 && ActiveDeck == 1)
                {
                    TurnAceInto1(sender, e, Player);
                }
                else if (PlayerHandValue2.Sum() > 21 && ActiveDeck == 2)
                {
                    TurnAceInto1(sender, e, Player);
                }
            }
            else
            {
                if (CpuHandValue.Sum() > 21)
                {
                    TurnAceInto1(sender, e, Player);
                }
            }
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

        
        private void TurnAceInto1(object sender, RoutedEventArgs e, bool Player)
        {
            if (Player)
            {
                if (PlayerHandValue.Contains(11) && ActiveDeck == 1) //can we switch 11 with 1?
                {
                    PlayerHandValue.Remove(11);
                    PlayerHandValue.Add(1);
                }
                else if (PlayerHandValue2.Contains(11) && ActiveDeck == 2) //is the second deck active?
                {
                    PlayerHandValue2.Remove(11);
                    PlayerHandValue2.Add(1);
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
        }

        private async void Cpu_Turn(object sender, RoutedEventArgs e)
        {
            //Disabling da buttons
            Button_Enabling("");
            //Update text
            UpdateResultText("CPU Turn.", "White");

            //Wait
            await Task.Delay(1200);
            //Remove Secret Card
            CpuDeckPanel.Children.RemoveAt(CpuDeckPanel.Children.Count - 1);

            //Keep pulling cards until we reach 17+
            while (CpuHandValue.Sum() < 17)
            {
                AddCard(PullCard(), false, 0);
                if (CpuHandValue.Sum() < 22)
                {
                    UpdateDisplayScore();
                    await Task.Delay(500);
                }
            }

            //Does CPU have a blackjack?
            if (CpuHandValue.Sum() == 21 && CpuDeckPanel.Children.Count == 2)
            {
                Match_Results();
                return;
            }

            //Do we have a soft 17?
            if (!Soft17Clear) //only clear if we haven't already
            {
                CheckSoft17(sender, e);
                UpdateDisplayScore();
            }

            UpdateDisplayScore();
            //Keep pulling cards until we reach 17+
            while (CpuHandValue.Sum() < 17)
            {
                AddCard(PullCard(), false, 0);
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

        private async void Match_Results()
        {
            string Result;

            //Calculate Result
            if (CpuHandValue.Sum() == 21 && PlayerHandValue.Sum() != 21)
            {
                Result = "Cpu";
            }
            else if (CpuHandValue.Sum() == 21 && PlayerHandValue.Sum() == 21)
            {
                Result = "Draw";
            }
            else if (CpuHandValue.Sum() > 21 && PlayerHandValue.Sum() > 21)
            {
                Result = "Draw";
            }
            else if (CpuHandValue.Sum() == PlayerHandValue.Sum())
            {
                Result = "Draw";
            }
            else if (CpuHandValue.Sum() > 21)
            {
                Result = "Player";
            }
            else if (CpuHandValue.Sum() > PlayerHandValue.Sum())
            {
                Result = "Cpu";
            }
            else if (PlayerHandValue.Sum() > 21)
            {
                Result = "Cpu";
            }
            else
            {
                Result = "Player";
            }

            //Voeg toe aan historiek
            if (Result == "Player")
            {
                UpdateHistoriek(true);
            }
            else if (Result == "Cpu")
            {
                UpdateHistoriek(false);
            }
            else
            {
                UpdateHistoriek(null);
            }

            //Display Result
            if (Result == "Player")
            {
                UpdateResultText("Won", "Green");
                //PlayerIcon
                TxtPlayerIcon.Text = "😎";
                LblPlayerScore.FontWeight = FontWeights.Bold;
                IconResize(true);
            }
            else if (Result == "Cpu")
            {
                UpdateResultText("Lost", "Red");
                TxtPlayerIcon.Text = "😢";
                LblCpuScore.FontWeight = FontWeights.Bold;
                IconResize(false);
            }
            else
            {
                UpdateResultText("Push", "Orange");
                TxtPlayerIcon.Text = "😅";
                LblPlayerScore.FontWeight = FontWeights.Bold;
                LblCpuScore.FontWeight = FontWeights.Bold;
                IconResize(null);
            }

            //Hand out PAY
            BetHandling(Result);
            //Enabling da buttons
            Button_Enabling("ChangeBet Continue AllIn");

            //Check Second Deck ONLY IF NEEDED
            if (PlayerDeck2.Count == 0)
            {
                return;
            }
            //Disable buttons
            Button_Enabling("");
            await Task.Delay(1500); //Wait 
            LblPlayerScore.FontWeight = FontWeights.Normal;
            IconResize(null);

            ActiveDeck = 2;
            UpdateDisplayScore();
            HighlightActiveDeck();
            UpdateResultText("...", "White");

            await Task.Delay(800);
            //Execute same steps as above but this time for second deck
            //Calculate Result
            if (CpuHandValue.Sum() == 21 && PlayerHandValue2.Sum() != 21)
            {
                Result = "Cpu";
            }
            else if (CpuHandValue.Sum() == 21 && PlayerHandValue2.Sum() == 21)
            {
                Result = "Draw";
            }
            else if (CpuHandValue.Sum() > 21 && PlayerHandValue2.Sum() > 21)
            {
                Result = "Draw";
            }
            else if (CpuHandValue.Sum() == PlayerHandValue2.Sum())
            {
                Result = "Draw";
            }
            else if (CpuHandValue.Sum() > 21)
            {
                Result = "Player";
            }
            else if (CpuHandValue.Sum() > PlayerHandValue2.Sum())
            {
                Result = "Cpu";
            }
            else if (PlayerHandValue2.Sum() > 21)
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
                UpdateResultText("Won", "Green");
                TxtPlayerIcon.Text = "😎";
                LblPlayerScore.FontWeight = FontWeights.Bold;
                IconResize(true);
            }
            else if (Result == "Cpu")
            {
                UpdateResultText("Lost", "Red");
                TxtPlayerIcon.Text = "😢";
                LblCpuScore.FontWeight = FontWeights.Bold;
                IconResize(false);
            }
            else
            {
                UpdateResultText("Push", "Orange");
                TxtPlayerIcon.Text = "😅";
                LblPlayerScore.FontWeight = FontWeights.Bold;
                LblCpuScore.FontWeight = FontWeights.Bold;
                IconResize(null);
            }

            //Hand out PAY
            BetHandling(Result);
        }

        private void BetHandling(string Result)
        {
            if (Result == "Player")
            {
                //BLACKJACK? [NIET MOGELIJK BIJ EEN SPLIT!]
                if (PlayerDeck.Count == 2 && PlayerHandValue.Sum() == 21 && PlayerHandValue2.Count == 0)
                {
                    float i = Bet * 2.5f;
                    Math.Round(i);
                    Money += int.Parse(i.ToString());
                    UpdateResultText($"BLACKJACK! + €{i}", "Gold"); //Override default "Won x amount" text
                }
                else
                {
                    Money += Bet * 2;
                    TxtResults.Text += $" €{Bet * 2}";
                }
            }
            else if (Result == "Draw")
            {
                Money += Bet;
            }
            else
            {
                TxtResults.Text += $" €{Bet}";
            }

            //Update Header
            MoneyRelatedActions();
            //Enabling da buttons
            Button_Enabling("ChangeBet Continue AllIn");
            //Game Over?
            GameOver();
        }

        private async void MoneyRelatedActions()
        {
            //HEADER TEXT
            //LEFT SECTION
            TxtBet.Text = $"BET= €{Bet}";

            //RIGHT-SECTION
            TxtAmount.Text = $"€{Money}";
            //Betpanel
            TxtMoney.Text = $"MONEY= €{Money}";

            //Edit Slider
            SldAmount.Minimum = Money / 10;
            SldAmount.Maximum = Money;
            SldAmount.TickFrequency = Money / 10;
            SldAmount.SmallChange = Money / 10;
            SldAmount.LargeChange = Money / 2;

            await Task.Delay(1000);
            SldAmount.Value = SldAmount.Minimum;
        }

        private async void GameOver()
        {
            if (Money > 0) return;

            //Avoid a game over if player has a split and 2nd deck hasn't been checked yet
            if (PlayerHandValue2.Count > 0 && ActiveDeck == 1) { return; }

            Button_Enabling("");

            await Task.Delay(1500);
            while (TxtResults.Opacity > 0)
            {
                TxtResults.Opacity -= 0.05;
                await Task.Delay(50);
            }

            UpdateResultText("Game Over", "White");

            while (TxtResults.Opacity < 1)
            {
                TxtResults.Opacity += 0.02;
                await Task.Delay(25);
            }

            await Task.Delay(2000);
            Button_Enabling("NewGame");
        }

        private void BtnBet_Click(object sender, RoutedEventArgs e)
        {
            //Reset Bet Amount to 100% if given amount is higher than 100%
            bool BetParse = int.TryParse(TxtBetAmount.Text, out Bet);

            if (!BetParse)
            {
                UpdateResultText("Bet not accepted.", "White");
                return;
            }

            //Cannot BET €0
            if (Bet == 0)
            {
                UpdateResultText("We aint betting for free", "White");
                return;
            }

            //Place Bet
            Money -= Bet;

            //Hide BetPanel Show PlayerDeck
            DisplayDeck(true);

            //Display Bet acceptance
            UpdateResultText($"€{Bet} has been bet!", "White");

            MoneyRelatedActions();
        }

        private void BtnBetAllIn_Click(object sender, RoutedEventArgs e)
        {
            SldAmount.Value = SldAmount.Maximum;
            TxtBetAmount.Text = SldAmount.Maximum.ToString();
        }

        private void SldAmount_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            TxtBetAmount.Text = SldAmount.Value.ToString();
        }

        private void BtnChangeBet_Click(object sender, RoutedEventArgs e)
        {
            //Reset bet
            Bet = 0;

            DisplayDeck(false);
            Button_Enabling("");
        }

        private void BtnContinue_Click(object sender, RoutedEventArgs e)
        {
            //Can we Apply the same bet again?
            if (Money < Bet)
            {
                UpdateResultText("NO", "White");
                BtnContinue.Visibility = Visibility.Hidden;
                return;
            }

            Money -= Bet;
            MoneyRelatedActions();
            BtnDeel_Click(sender, e);
        }

        private async void BtnNewGame_Click(object sender, RoutedEventArgs e)
        {
            CardsInGame.Clear();
            TxtDeckCount.Text = (Deck.Count - CardsInGame.Count).ToString();
            Button_Enabling("");
            Money = 100;
            Bet = 0;
            Ronde = 0;
            DisplayDeck(false);
            await Task.Delay(500);
            Reset_Table();
        }

        private void BtnAllIn_Click(object sender, RoutedEventArgs e)
        {
            Bet = Money;
            Money = 0;
            MoneyRelatedActions();

            BtnDeel_Click(sender, e);
        }

        private async void BtnSplit_Click(object sender, RoutedEventArgs e)
        {
            //Does player have enough to split?
            if (Money < Bet)
            {
                UpdateResultText("Too broke to split >:D", "White");
                return;
            }

            //Take Bet
            Money -= Bet;

            //Table Preperations for split
            Button_Enabling("");
            RepositionDeckPanel(true);
            MoneyRelatedActions();

            //Plaats laatst getrokken kaart in de 2e deck lijst
            PlayerDeckPanel.Children.RemoveAt(PlayerDeckPanel.Children.Count - 1);
            PlayerHandValue.RemoveAt(1);

            //Voeg kaart toe aan de 2e Panel
            int index = Deck.FindIndex(a => a.Contains(PlayerDeck.ElementAt(1)));
            AddCard(index, true, 2);

            //Verwijder 2e kaart van de eerste lijst
            PlayerDeck.RemoveAt(1);

            //Geef aan elke deck 1 kaart
            UpdateDisplayScore();
            await Task.Delay(600);
            AddCard(PullCard(), true, 1);
            FixOverFlow(sender, e, true);
            UpdateDisplayScore();

            await Task.Delay(700);
            ActiveDeck = 2;
            HighlightActiveDeck();
            await Task.Delay(500);
            AddCard(PullCard(), true, 2);
            FixOverFlow(sender, e, true);
            UpdateDisplayScore();

            await Task.Delay(800);

            //Did Deck 1 get a blackjack?
            if (PlayerHandValue.Sum() != 21)
            {
                ActiveDeck = 1;
                HighlightActiveDeck();
            }
            else
            {
                ActiveDeck = 2;
            }

            //Start CPU_TURN if player split an ace
            if (PlayerDeck.ElementAt(0).Contains("Ace") && PlayerDeck2.ElementAt(0).Contains("Ace"))
            {
                Cpu_Turn(sender, e);
                return;
            }
            //Buttons
            Button_Enabling("Hit Stand");

            //Update Display Score
            UpdateDisplayScore();
        }

        private void HighlightActiveDeck()
        {
            if (ActiveDeck == 1)
            {
                PlayerDeckPanel.Opacity = 1;
                PlayerDeckPanelBorder.Opacity = 1;
                PlayerDeckPanel2.Opacity = 0.4;
                PlayerDeckPanelBorder2.Opacity = 0.3;
            }
            else
            {
                PlayerDeckPanel.Opacity = 0.4;
                PlayerDeckPanelBorder.Opacity = 0.3;
                PlayerDeckPanel2.Opacity = 1;
                PlayerDeckPanelBorder2.Opacity = 1;
            }
        }

        private void RepositionDeckPanel(bool Split)
        {
            if (Split)
            {
                //Verplaatste eerste deck naar -1 column
                PlayerDeckPanelBorder.SetValue(Grid.ColumnProperty, 1);
                PlayerDeckPanel.SetValue(Grid.ColumnProperty, 1);
                //Toon 2e deck
                PlayerDeckPanelBorder2.Visibility = Visibility.Visible;
                PlayerDeckPanel2.Visibility = Visibility.Visible;
                //Verplaats Icoon en Score
                PlayerIconPanel.SetValue(Grid.ColumnSpanProperty, 1);
                PlayerScorePanel.SetValue(Grid.ColumnSpanProperty, 1);
                PlayerScorePanel.SetValue(Grid.ColumnProperty, 5);
                //Vergroot Tafel
                PlayerTable.SetValue(Grid.ColumnSpanProperty, 4);
                PlayerTable.SetValue(Grid.ColumnProperty, 1);
                PlayerTable.Margin = new Thickness(15, 0, 10, 0);
                //Voeg marges toe dat ze niet te veel aan de uiteinde plakken
                PlayerDeckPanel.Margin = new Thickness(30, 0, 5, 0);
                PlayerDeckPanelBorder.Margin = new Thickness(30, 0, 5, 0);
            }
            else
            {
                //Verplaats de eerste deck naar +1 column
                PlayerDeckPanelBorder.SetValue(Grid.ColumnProperty, 2);
                PlayerDeckPanel.SetValue(Grid.ColumnProperty, 2);
                //Verberg 2e deck
                PlayerDeckPanelBorder2.Visibility = Visibility.Collapsed;
                PlayerDeckPanel2.Visibility = Visibility.Collapsed;
                //Verplaats Icoon en Score
                PlayerIconPanel.SetValue(Grid.ColumnSpanProperty, 2);
                PlayerScorePanel.SetValue(Grid.ColumnSpanProperty, 2);
                PlayerScorePanel.SetValue(Grid.ColumnProperty, 4);
                //Verklein Tafel
                PlayerTable.SetValue(Grid.ColumnSpanProperty, 2);
                PlayerTable.SetValue(Grid.ColumnProperty, 2);
                PlayerTable.Margin = new Thickness(0, PlayerTable.Margin.Top, 0, PlayerTable.Margin.Bottom);
                //Reset Marges
                PlayerDeckPanel.Margin = new Thickness(0);
                PlayerDeckPanelBorder.Margin = new Thickness(0, 5, 0, 5);
            }
        }

        private void PlayerDeckPanel_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            //Is Sender een Dockpanel?
            if (sender is DockPanel)
            {
                DockPanel Dpanel = (DockPanel)sender;

                if (Dpanel.Name == "PlayerDeckPanel")
                {
                    LblPlayerScore.Content = PlayerHandValue.Sum().ToString();
                }
                else
                {
                    LblPlayerScore.Content = PlayerHandValue2.Sum().ToString();
                }

                return;
            }
            
            //Is Sender een border?
            Border border = (Border)sender;

            if (border.Name == "PlayerDeckPanelBorder")
            {
                LblPlayerScore.Content = PlayerHandValue.Sum().ToString();
            }
            else
            {
                LblPlayerScore.Content = PlayerHandValue2.Sum().ToString();
            }
        }

        private void PlayerDeckPanel_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            UpdateDisplayScore();
        }

        private void BtnDoubleDown_Click(object sender, RoutedEventArgs e)
        {
            //Money related
            Money -= Bet;
            Bet *= 2;
            MoneyRelatedActions();

            //Add Card
            AddCard(PullCard(), true, ActiveDeck);
            FixOverFlow(sender, e, true);
            //Show Score
            UpdateDisplayScore();

            //End Turn
            Cpu_Turn(sender, e);
        }

        private void TxtHistoriek_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            string output = "";
            foreach (string row in HistoriekLijst)
            {
                output += row;
            }

            MessageBox.Show(output);
        }

        private async void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            bool active = true;
            while (active)
            {
                TxtTime.Text = DateTime.Now.ToString($"HH:mm:ss");
                await Task.Delay(1000);
            }
        }

        private void UpdateHistoriek(bool ? Win)
        {
            string Resultaat = "";

            if (Win.HasValue == true)
            {
                Resultaat = "gewonnen";
            }
            else if (Win.HasValue == false)
            {
                Resultaat = "verloren";
            }
            else
            {
                Resultaat = "niks";
            }

            string HistoriekLog = $"Ronde {Ronde}: {Bet} {Resultaat}-{Money}/Bank";

            //Shuif alle waardes op met 1
            for (int i = 0; i < 9; i++)
            {
                HistoriekLijst[i + 1, 0] = HistoriekLijst[i, 0];
            }

            //Voeg nieuwe waarde toe
            HistoriekLijst[0, 0] = HistoriekLog;
        }
    }
}
