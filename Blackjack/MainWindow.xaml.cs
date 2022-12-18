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
            Reset_Table();
            ResizeDeck();
            MoneyRelatedActions();
        }

        //Lijsten
        List<string> Deck = new List<string>(); //ALL 52 CARDS IN THE GAME
        List<string> CardsInGame = new List<string>(); //ALL CARDS IN USE (player and cpu combined)
        List<string> PlayerDeck = new List<string>(); //All CARDS PLAYER
        List<string> CpuDeck = new List<string>(); //ALL CARDS CPU
        List<int> PlayerHandValue = new List<int>(); //ALL CARD VALUES FROM PLAYER
        List<int> CpuHandValue = new List<int>(); //ALL CARD VALUES FROM CPU
        //BETTING
        int Money = 100;
        int Bet = 0;
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
                if (PlayerDeckPanel.Children.Count != 0)
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
            //CardsInGame.Clear();
            PlayerDeck.Clear();
            CpuDeck.Clear();

            //Cards Value
            PlayerHandValue.Clear();
            CpuHandValue.Clear();
            PlayerValue = 0;
            CpuValue = 0;

            //CardDisplay
            PlayerDeckPanel.Children.Clear();
            CpuDeckPanel.Children.Clear();

            //Reseting values
            Soft17Clear = false;
            TxtResults.Foreground = Brushes.White;
            TxtPlayerIcon.Text = "🤔";
            LblCpuScore.FontWeight = FontWeights.Regular;
            LblPlayerScore.FontWeight = FontWeights.Regular;
        }

        private void Button_Enabling(string ButtonNames)
        {
            if (ButtonNames.Contains("Deel"))
            {
                BtnDeel.Visibility = Visibility.Visible;
            }
            else
            {
                BtnDeel.Visibility = Visibility.Collapsed;
            }

            if (ButtonNames.Contains("Hit"))
            {
                BtnHit.Visibility = Visibility.Visible;
            }
            else
            {
                BtnHit.Visibility = Visibility.Collapsed;
            }

            if (ButtonNames.Contains("Stand"))
            {
                BtnStand.Visibility = Visibility.Visible;
            }
            else
            {
                BtnStand.Visibility = Visibility.Collapsed;
            }

            if (ButtonNames.Contains("ChangeBet"))
            {
                BtnChangeBet.Visibility = Visibility.Visible;
            }
            else
            {
                BtnChangeBet.Visibility = Visibility.Collapsed;
            }

            if (ButtonNames.Contains("Continue"))
            {
                BtnContinue.Visibility = Visibility.Visible;
            }
            else
            {
                BtnContinue.Visibility = Visibility.Collapsed;
            }

            if (ButtonNames.Contains("NewGame"))
            {
                BtnNewGame.Visibility = Visibility.Visible;
            }
            else
            {
                BtnNewGame.Visibility = Visibility.Collapsed;
            }

            if (ButtonNames.Contains("AllIn"))
            {
                BtnAllIn.Visibility = Visibility.Visible;
            }
            else
            {
                BtnAllIn.Visibility = Visibility.Collapsed;
            }

            if (ButtonNames.Contains("Split"))
            {
                BtnSplit.Visibility = Visibility.Visible;
            }
            else
            {
                BtnSplit.Visibility = Visibility.Collapsed;
            }
        }

        private void UpdateDisplayScore()
        {
            LblPlayerScore.Content = PlayerValue.ToString();
            LblCpuScore.Content = CpuValue.ToString();
        }

        private void ResizeDeck()
        {
            int PlayerSize = 0;
            int CpuSize = 0;

            if (PlayerDeckPanel.Children.Count > 0)
            {
                PlayerSize = 110;
            }
            if (CpuDeckPanel.Children.Count > 0)
            {
                CpuSize = 110;
            }

            foreach (var item in PlayerDeckPanel.Children)
            {
                PlayerSize += 20;
            }
            foreach (var item in CpuDeckPanel.Children)
            {
                CpuSize += 20;
            }

            PlayerDeckPanelBorder.Width = PlayerSize;
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

                //Prepare Table for next game
                Reset_Table();
                ResizeDeck();
                UpdateDisplayScore();
                MoneyRelatedActions();

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

                //Prepare Table for next game
                Reset_Table();
                ResizeDeck();
                UpdateDisplayScore();
                MoneyRelatedActions();

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
            CpuDeckPanel.Children.Add(Card(false));
            ResizeDeck();

            //Did we get a 21?
            if (PlayerValue == 21)
            {
                Cpu_Turn(sender, e);
                return;
            }

            //Re-enable the needed buttons
            Button_Enabling("Hit Stand");
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
                PlayerDeckPanel.Children.Add(Card(Player));
                PlayerDeck.Add(Deck.ElementAt(index));
                PlayerHandValue.Add(CardValue());
            }
            else
            {
                CpuDeckPanel.Children.Add(Card(Player));
                CpuDeck.Add(Deck.ElementAt(index));
                CpuHandValue.Add(CardValue());
            }

            DeckValue();
            ResizeDeck();

            //How many cards are left?
            ShuffleDeck();
        }

        private void ShuffleDeck()
        {
            int CardsLeft = Deck.Count - CardsInGame.Count;

            //Reshuffle AllCards if all cards have been played
            if (CardsInGame.Count == Deck.Count())
            {
                UpdateResultText("Deck Got Shuffled:)", "White");
                CardsInGame.Clear();
            }

            //Display
            TxtDeckCount.Text = CardsLeft.ToString();
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
            //Disabling da buttons
            Button_Enabling("");
            //Update text
            UpdateResultText("CPU Turn.", "White");

            //Wait
            await Task.Delay(1200);
            //Remove Secret Card
            CpuDeckPanel.Children.RemoveAt(CpuDeckPanel.Children.Count - 1);

            //Keep pulling cards until we reach 17+
            DeckValue();
            while (CpuValue < 17)
            {
                AddCard(PullCard(), false);
                DeckValue();
                if (CpuValue < 22)
                {
                    UpdateDisplayScore();
                    await Task.Delay(500);
                }
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

            UpdateDisplayScore();
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
                UpdateResultText("Won", "Green");
                TxtPlayerIcon.Text = "😎";
                LblPlayerScore.FontWeight = FontWeights.Bold;
            }
            else if (Result == "Cpu")
            {
                UpdateResultText("Lost", "Red");
                TxtPlayerIcon.Text = "😢";
                LblCpuScore.FontWeight = FontWeights.Bold;
            }
            else
            {
                UpdateResultText("Draw", "Orange");
                TxtPlayerIcon.Text = "😅";
                LblPlayerScore.FontWeight = FontWeights.Bold;
                LblCpuScore.FontWeight = FontWeights.Bold;
            }

            //Enabling da buttons
            Button_Enabling("ChangeBet Continue AllIn");
            //Hand out PAY
            BetHandling(Result);
        }

        private void BetHandling(string Result)
        {
            if (Result == "Player")
            {
                //BLACKJACK?
                if (PlayerDeck.Count == 2 && PlayerValue == 21)
                {
                    float i = Bet * 2.5f;
                    Math.Round(i);
                    Money += int.Parse(i.ToString());
                    UpdateResultText($"BLACKJACK! + €{i}", "Gold"); //Override default "Won x amount" text
                }
                else
                {
                    Money += Bet;
                    TxtResults.Text += $" €{Bet * 2}";
                }
            }
            else if (Result == "Draw")
            {
                //Money += Bet;
            }
            else
            {
                Money -= Bet;
                TxtResults.Text += $" €{Bet}";
            }

            //Update Header
            MoneyRelatedActions();
            //Game Over?
            GameOver();
        }

        private async void MoneyRelatedActions()
        {
            //HEADER TEXT
            //LEFT SECTION
            TxtBet.Text = $"BET= €{Bet}";

            //MIDDLE-SECTION
            TxtBetWin.Text = $"WIN= €{Money + Bet}";
            if (Money - Bet <= 0)
            {
                TxtBetLose.Text = "LOSE= GAME OVER!";
            }
            else
            {
                TxtBetLose.Text = $"LOSE= €{Money - Bet}";
            }

            //RIGHT-SECTION
            TxtAmount.Text = $"€{Money}";
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

            //APPLY BET
            //Money -= Bet;

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
            DisplayDeck(false);
            await Task.Delay(500);
            Reset_Table();
        }

        private void BtnAllIn_Click(object sender, RoutedEventArgs e)
        {
            Bet = Money;
            MoneyRelatedActions();

            BtnDeel_Click(sender, e);
        }

        private void BtnSplit_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
