using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Blackjack
{
    public partial class MainWindow : Window
    {
        //clubs (♣), diamonds (♦), hearts (♥) and spades (♠)
        public MainWindow()
        {
            InitializeComponent();
            Reset_Table();
            ResizeDeck();
            MoneyRelatedActions();
            PlaySound();
        }

        #region Lijsten
        /// <summary>
        /// This is the main deck that holds all the cards.
        /// </summary>
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
        /// <summary>
        /// This list keeps track of all the played cards.
        /// </summary>
        List<string> CardsInGame = new List<string>(); //ALL CARDS IN USE (player and cpu combined)
        /// <summary>
        /// Holds all the cards that Player has in his first deck.
        /// </summary>
        List<string> PlayerDeck = new List<string>(); //All CARDS PLAYER
        /// <summary>
        /// Holds all the cards that Player has in his second deck.
        /// </summary>
        List<string> PlayerDeck2 = new List<string>(); //Second deck for player (split)
        /// <summary>
        /// Holds all the cards that CPU has in its deck.
        /// </summary>
        List<string> CpuDeck = new List<string>(); //ALL CARDS CPU
        /// <summary>
        /// Holds all the values of the cards in Player's first deck.
        /// </summary>
        List<int> PlayerHandValue = new List<int>(); //ALL CARD VALUES FROM PLAYER
        /// <summary>
        /// Holds all the values of the cards in Player's second deck.
        /// </summary>
        List<int> PlayerHandValue2 = new List<int>(); //Card values for second deck (player)
        /// <summary>
        /// Holds all the values of the cards in CPU's deck.
        /// </summary>
        List<int> CpuHandValue = new List<int>(); //ALL CARD VALUES FROM CPU
        #endregion

        #region Variabelen
        //Integers
        int Money = 100;
        int Bet = 0;
        /// <summary>
        /// Indicates which deck is being active.
        /// <para>Default value = 1. This can change in case of a split. Values can be either 1 or 2.</para>
        /// <para>All actions that affect the deck uses this integer to determine which deck is getting the requested actions.</para>
        /// </summary>
        int ActiveDeck = 1;
        int Ronde = 0;

        //Strings
        /// <summary>
        /// Holds a list of strings that shows the history of the last 10 rounds.
        /// <para>This can be viewed when pressed on "Historiek" in the HEADER.</para>
        /// </summary>
        string[] Historiek = new string[10];
        /// <summary>
        /// Holds the name of the pulled card.
        /// This value gets passed along multiple methods.
        /// </summary>
        string CardName;
        /// <summary>
        /// This string only exists to check wether background music got modified or not.
        /// <para>
        /// If this string doesn't match the string at (Properties.Settings.Default.Music)
        /// then that means that the background music has been changed. 
        /// </para>
        /// <para>
        /// PlaySound() gets called.
        /// This string gets updated to the current background music.
        /// </para>
        /// <para>Defauly value = "The_Holy_Queen"</para>
        /// </summary>
        string Music = "The_Holy_Queen";

        //Booleans
        /// <summary>
        /// To keep track wether a soft 17 got already cleared or not.
        /// </summary>
        bool Soft17Clear = false;

        //Andere
        Random Random = new Random();
        SoundPlayer MusicPlayer = null;
        #endregion

        #region Table Preperations
        /// <summary>
        /// Prepares the table for a new game.
        /// </summary>
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
        /// <summary>
        /// Shows or collapses the necessary main buttons.
        /// </summary>
        /// <param name="buttonNames">All the names of the buttons that needs to be visible.<para>Else, it will be collapsed.</para></param>
        private void Button_Enabling(string buttonNames)
        {
            BtnDeel.Visibility = buttonNames.Contains("Deel") ? Visibility.Visible : Visibility.Collapsed;
            BtnHit.Visibility = buttonNames.Contains("Hit") ? Visibility.Visible : Visibility.Collapsed;
            BtnStand.Visibility = buttonNames.Contains("Stand") ? Visibility.Visible : Visibility.Collapsed;
            BtnChangeBet.Visibility = buttonNames.Contains("ChangeBet") ? Visibility.Visible : Visibility.Collapsed;
            BtnContinue.Visibility = buttonNames.Contains("Continue") ? Visibility.Visible : Visibility.Collapsed;
            BtnNewGame.Visibility = buttonNames.Contains("NewGame") ? Visibility.Visible : Visibility.Collapsed;
            BtnAllIn.Visibility = buttonNames.Contains("AllIn") ? Visibility.Visible : Visibility.Collapsed;
            BtnSplit.Visibility = buttonNames.Contains("Split") ? Visibility.Visible : Visibility.Collapsed;
            BtnDoubleDown.Visibility = buttonNames.Contains("Double") ? Visibility.Visible : Visibility.Collapsed;
        }
        /// <summary>
        /// Resizes the border around the cards.
        /// </summary>
        private void ResizeDeck()
        {
            //Local Variables
            int playerSize = 0;
            int playerSize2 = 0;
            int cpuSize = 0;

            //Add 140px for the first card.
            if (PlayerDeckPanel.Children.Count > 0)
            {
                playerSize = 140;
            }
            if (PlayerDeckPanel2.Children.Count > 0)
            {
                playerSize2 = 140;
            }
            if (CpuDeckPanel.Children.Count > 0)
            {
                cpuSize = 140;
            }

            //Any other card increases the size with +25.
            foreach (var item in PlayerDeckPanel.Children)
            {
                playerSize += 25;
            }
            foreach (var item in PlayerDeckPanel2.Children)
            {
                playerSize2 += 25;
            }
            foreach (var item in CpuDeckPanel.Children)
            {
                cpuSize += 25;
            }

            //Apply Sizes.
            PlayerDeckPanelBorder.Width = playerSize;
            PlayerDeckPanelBorder2.Width = playerSize2;
            CpuDeckPanelBorder.Width = cpuSize;
        }
        /// <summary>
        /// Used to switch between PlayerDeck and BetPanel.
        /// </summary>
        /// <param name="showPlayerDeck">Do we need to show the PlayerDeck?<para>Else, BetPanel will be shown.</para></param>
        private async void DisplayDeck(bool showPlayerDeck)
        {
            ///<summary>
            ///If PlayerDeck needs to be displayed, it means betpanel is on screen, so we shift betpanel away and pull playerdeck.
            ///Betpanel on screen: Bottom-Margin = 10; Offscreen: Bottom-Margin = -210;
            ///PlayerDeckPanelBorder and PlayerDeckPanel Only need visibility change When getting Displayed, ELSE same rules as The Table.
            ///Only Playertable moves up. On screen margin = 0; Offscreen margin = 0, 210, 0, -210
            ///CPU's Deck fades away. [When Visible: Panel = 1; Border = 0.4;]
            /// </summary>

            //Local Variables
            int betMargin;
            int deckMargin;

            //Prepare Table for next game
            ResizeDeck();
            UpdateDisplayScore();
            MoneyRelatedActions();

            if (showPlayerDeck)
            {
                //Prepare Table
                Reset_Table();

                //Move BetPanel Down
                betMargin = 10;
                while (betMargin > -390)
                {
                    betMargin -= 15;
                    BetPanel.Margin = new Thickness(0, 0, 0, betMargin);
                    BetPanelBorder.Margin = new Thickness(0, 0, 0, betMargin);
                    await Task.Delay(25);
                }


                //CPU DECK VISIBLE
                CpuDeckPanel.Opacity = 1;
                CpuDeckPanelBorder.Opacity = 0.4;


                //Move PlayerDeck Up
                deckMargin = 210;
                while (deckMargin > 0)
                {
                    deckMargin -= 10;
                    PlayerTable.Margin = new Thickness(0, deckMargin, 0, (deckMargin - (deckMargin * 2)));
                    await Task.Delay(25);
                }

                //Display other components and reset default values
                PlayerDeckPanel.Margin = new Thickness(0);
                PlayerDeckPanelBorder.Margin = new Thickness(0);
                PlayerDeckPanel2.Margin = new Thickness(0);
                PlayerDeckPanelBorder2.Margin = new Thickness(0);
                PlayerDeckPanel.Visibility = Visibility.Visible;
                PlayerDeckPanelBorder.Visibility = Visibility.Visible;

                //Show buttons
                Button_Enabling("Deel");

                //Show a message to "make a play" if player hasn't done so in 2 seconds.
                await Task.Delay(2000);
                if (PlayerDeckPanel.Children.Count == 0)
                {
                    UpdateDisplayText("Make a Play.", "White");
                }
            }
            else
            {
                //Move PlayerDeck down AND CPU DECK FADE
                deckMargin = 0;
                while (deckMargin <= 210)
                {
                    deckMargin += 10;
                    PlayerTable.Margin = new Thickness(0, deckMargin, 0, (deckMargin - (deckMargin * 2)));
                    PlayerDeckPanelBorder.Margin = new Thickness(0, deckMargin, 0, (deckMargin - (deckMargin * 2)));
                    PlayerDeckPanel.Margin = new Thickness(0, deckMargin, 0, (deckMargin - (deckMargin * 2)));
                    PlayerDeckPanelBorder2.Margin = new Thickness(0, deckMargin, 0, (deckMargin - (deckMargin * 2)));
                    PlayerDeckPanel2.Margin = new Thickness(0, deckMargin, 0, (deckMargin - (deckMargin * 2)));

                    if (CpuDeckPanel.Opacity != 0)
                    {
                        CpuDeckPanel.Opacity -= 0.05;
                    }
                    if (CpuDeckPanelBorder.Opacity != 0)
                    {
                        CpuDeckPanelBorder.Opacity -= 0.02;
                    }

                    await Task.Delay(25);
                }

                //Hide playerpanels
                PlayerDeckPanel.Visibility = Visibility.Collapsed;
                PlayerDeckPanelBorder.Visibility = Visibility.Collapsed;

                //Reset Table
                Reset_Table();

                //Edit Result text
                UpdateDisplayText("Place your bet!", "White");

                //Move BetPanel Up
                betMargin = -390;
                while (betMargin < 10)
                {
                    betMargin += 15;
                    BetPanel.Margin = new Thickness(0, 0, 0, betMargin);
                    BetPanelBorder.Margin = new Thickness(0, 0, 0, betMargin);
                    await Task.Delay(25);
                }
            }
        }
        /// <summary>
        /// Resizes the icons on screen.
        /// </summary>
        /// <param name="player">
        /// If true, PlayerIcon gets increased in size. Else, CpuIcon increases.
        /// <para>If null, both getting reset to default sizes.</para></param>
        private void IconResize(bool? player)
        {
            //If null, revert to default sizes.
            if (!player.HasValue)
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

            //Enlarge Player Icon
            if (player.Value == true)
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
            else //Enlarge Cpu Icon
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
        /// <summary>
        /// Updates/displays the total hand value.
        /// </summary>
        private void UpdateDisplayScore()
        {
            //Display total value of the active deck
            if (ActiveDeck == 1)
            {
                LblPlayerScore.Content = PlayerHandValue.Sum().ToString();
            }
            else
            {
                LblPlayerScore.Content = PlayerHandValue2.Sum().ToString();
            }

            //Cpu hand value
            LblCpuScore.Content = CpuHandValue.Sum().ToString();
        }
        /// <summary>
        /// Updates the big text in the middle of the screen.
        /// </summary>
        /// <param name="text">Text that is getting displayed.</param>
        /// <param name="textColor">Text color.
        /// <para>Can be: White, Red, Green, Orange or Gold.</para></param>
        private void UpdateDisplayText(string text, string textColor)
        {
            //Display Text
            TxtResults.Text = text;

            //Change TextColor
            switch (textColor)
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
        /// <summary>
        /// Highlights the active deck by making the opacity 100%.
        /// <para>Non-active deck's opacity gets lowered.</para>
        /// </summary>
        private void HighlightActiveDeck()
        {
            if (ActiveDeck == 1) //Highlight first deck
            {
                PlayerDeckPanel.Opacity = 1;
                PlayerDeckPanelBorder.Opacity = 1;
                PlayerDeckPanel2.Opacity = 0.4;
                PlayerDeckPanelBorder2.Opacity = 0.3;
            }
            else //Highlight second deck
            {
                PlayerDeckPanel.Opacity = 0.4;
                PlayerDeckPanelBorder.Opacity = 0.3;
                PlayerDeckPanel2.Opacity = 1;
                PlayerDeckPanelBorder2.Opacity = 1;
            }
        }
        /// <summary>
        /// Repositions and creates space for the 2nd panel in case of a split.
        /// <para>If not, restores default position.</para>
        /// </summary>
        /// <param name="split">Are we splitting?</param>
        private void RepositionDeckPanel(bool split)
        {
            if (split)
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
        /// <summary>
        /// Occurs if mouse hovers over the deck.
        /// Shows the total value of the deck that is getting hovered.
        /// <para>This is useful when dealing with splits.</para>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DisplayHandValue(object sender, System.Windows.Input.MouseEventArgs e)
        {
            //Alleen uitvoeren als er een 2de deck aanwezig is
            if (PlayerDeck2.Count == 0) { return; }

            //Increase font size
            LblPlayerScore.FontSize = 54;

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
        /// <summary>
        /// Occurs if the mouse leaves the deck.
        /// Resets the displayed handvalue to default.
        /// <para>This method works hand in hand with [ DisplayHandValue() ]</para>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RestoreHandValue(object sender, System.Windows.Input.MouseEventArgs e)
        {
            UpdateDisplayScore();
            LblPlayerScore.FontSize = 48;
        }
        /// <summary>
        /// Update the bet-text according to the slider in BetPanel.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SldAmount_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            TxtBetAmount.Text = SldAmount.Value.ToString();
        }
        #endregion

        #region Buttons
        //Main Buttons
        private async void BtnDeel_Click(object sender, RoutedEventArgs e)
        {
            //Table preparation
            Button_Enabling("");
            Reset_Table();
            LblPlayerScore.Content = "...";
            LblCpuScore.Content = "...";
            UpdateDisplayText("...", "White");
            Ronde++;

            //Verdeel 2 kaarten
            for (int i = 0; i < 2; i++)
            {
                AddCard(PullCard(), true, 1);

                //Avoid getting 22
                CheckOverflow(sender, e, true);
                UpdateDisplayScore();
                await Task.Delay(500);
            }

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
            //(!) 2de Ace waarde word als 1 opgeslagen dus split word niet herkent in de 2e if statement
            if (PlayerDeck.ElementAt(0).Contains("Ace") && PlayerDeck.ElementAt(1).Contains("Ace") && Money >= Bet)
            {
                Button_Enabling("Hit Stand Split Double");
            }
            else if (PlayerHandValue.ElementAt(0) == PlayerHandValue.ElementAt(1) && Money >= Bet)
            {
                Button_Enabling("Hit Stand Split Double");
            }
            else if (Money >= Bet)
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
            //Hide split/double button
            Button_Enabling("Hit Stand");

            //Add card and check for overflow
            AddCard(PullCard(), true, ActiveDeck);
            CheckOverflow(sender, e, true);

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
                //Start checking first deck if Cpu finishes its turn
                ActiveDeck = 1;

                //Table Prep
                UpdateDisplayScore();
                HighlightActiveDeck();

                Cpu_Turn(sender, e);
                return;
            }

            //Check wether we have an active second deck
            if (PlayerHandValue2.Sum() == 0 && ActiveDeck == 1) //If not
            {
                Cpu_Turn(sender, e);
            }
            else //if yes
            {
                ActiveDeck = 2;
                HighlightActiveDeck();
                UpdateDisplayScore();
            }
        }
        private void BtnChangeBet_Click(object sender, RoutedEventArgs e)
        {
            //Reset bet
            Bet = 0;

            //Table prep
            DisplayDeck(false);
            Button_Enabling("");
        }
        private void BtnContinue_Click(object sender, RoutedEventArgs e)
        {
            //Can we Apply the same bet again?
            if (Money < Bet) //if not
            {
                //Deny request
                UpdateDisplayText("NO", "White");
                BtnContinue.Visibility = Visibility.Hidden;
                return;
            }

            //Continue game with same bet
            Money -= Bet;
            MoneyRelatedActions();
            BtnDeel_Click(sender, e);
        }
        private async void BtnNewGame_Click(object sender, RoutedEventArgs e)
        {
            //Default waardes
            CardsInGame.Clear();
            TxtDeckCount.Text = (Deck.Count - CardsInGame.Count).ToString();
            Button_Enabling("");
            Money = 100;
            Bet = 0;
            Ronde = 0;
            DisplayDeck(false);
            Array.Clear(Historiek, 0, Historiek.Length);

            //Wait so cards are still visible while the panel is moving away.
            await Task.Delay(400);
            Reset_Table();
        }
        private async void BtnSplit_Click(object sender, RoutedEventArgs e)
        {
            //Does player have enough to split?
            if (Money < Bet)
            {
                UpdateDisplayText("Too broke to split >:D", "White");
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
            CheckOverflow(sender, e, true);
            UpdateDisplayScore();

            //Deck 2
            await Task.Delay(700);
            ActiveDeck = 2;
            HighlightActiveDeck();
            await Task.Delay(500);
            AddCard(PullCard(), true, 2);
            CheckOverflow(sender, e, true);
            UpdateDisplayScore();

            await Task.Delay(800);

            //Did Deck 1 get a blackjack?
            if (PlayerHandValue.Sum() != 21) //If not, make active deck 1
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
                ActiveDeck = 1;
                HighlightActiveDeck();
                Cpu_Turn(sender, e);
                return;
            }

            //Buttons
            Button_Enabling("Hit Stand");

            //Update Display Score
            UpdateDisplayScore();
        }
        private void BtnAllIn_Click(object sender, RoutedEventArgs e)
        {
            //Add all money as bet
            Bet = Money;
            Money = 0;
            MoneyRelatedActions();

            //Deel
            BtnDeel_Click(sender, e);
        }
        private void BtnDoubleDown_Click(object sender, RoutedEventArgs e)
        {
            //Money related
            Money -= Bet;
            Bet *= 2;
            MoneyRelatedActions();

            //Add Card
            AddCard(PullCard(), true, ActiveDeck);
            CheckOverflow(sender, e, true);

            //Show Score
            UpdateDisplayScore();

            //End Turn
            Cpu_Turn(sender, e);
        }

        //Betpanel Buttons
        private void BtnBet_Click(object sender, RoutedEventArgs e)
        {
            //Reset Bet Amount to 100% if given amount is higher than 100%
            bool BetParse = int.TryParse(TxtBetAmount.Text, out Bet);

            if (!BetParse)
            {
                UpdateDisplayText("Bet not accepted.", "White");
                return;
            }

            //Cannot BET €0
            if (Bet == 0)
            {
                UpdateDisplayText("We aint betting for free", "White");
                return;
            }

            //Place Bet
            Money -= Bet;

            //Hide BetPanel Show PlayerDeck
            DisplayDeck(true);

            //Display Bet acceptance
            UpdateDisplayText($"€{Bet} has been bet!", "White");

            MoneyRelatedActions();
        }
        private void BtnBetAllIn_Click(object sender, RoutedEventArgs e)
        {
            //Adjust slider and textbox accordingly
            SldAmount.Value = SldAmount.Maximum;
            TxtBetAmount.Text = SldAmount.Maximum.ToString();
        }

        //HEADER Buttons
        private void TxtHistoriek_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //Add all string from list to a stringbuilder.
            StringBuilder output = new StringBuilder();

            for (int i = 0; i < Historiek.Length; i++)
            {
                if (!string.IsNullOrWhiteSpace(Historiek[i]))
                {
                    output.AppendLine(Historiek[i]);
                }
            }

            //Display history as a messagebox
            MessageBox.Show(output.ToString(), "Historiek (Laatste 10 rondes)");
        }
        private void TxtPlaylist_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //Open playlist window
            Playlist_Window window = new Playlist_Window();
            window.ShowDialog();
        }
        #endregion

        #region Game Flow
        /// <summary>
        /// Pulls a card from the deck and returns the corresponding index number as an integer.
        /// </summary>
        /// <returns></returns>
        public int PullCard()
        {
            //Pull a random card that does not exist in the game yet
            int index = Random.Next(0, 52);

            while (CardsInGame.Contains(Deck.ElementAt(index)))
            {
                index = Random.Next(0, 52);
            }

            return index;
        }
        /// <summary>
        /// Adds the pulled card to the game.
        /// </summary>
        /// <param name="index">What is the index of the pulled card?</param>
        /// <param name="Player">Is this card ment for the player?</param>
        /// <param name="WhichDeck">For which deck (from the player) is this card for?</param>
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
            else //For CPU
            {
                CpuDeckPanel.Children.Add(Card(Player, 0));
                CpuDeck.Add(Deck.ElementAt(index));
                CpuHandValue.Add(CardValue());
            }

            //Resize 
            ResizeDeck();

            //How many cards are left?
            ShuffleDeck();
        }
        /// <summary>
        /// Here, the CPU start playing.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Cpu_Turn(object sender, RoutedEventArgs e)
        {
            //Disabling da buttons
            Button_Enabling("");
            //Update text
            UpdateDisplayText("CPU Turn.", "White");

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
                    await Task.Delay(700);
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
                await Task.Delay(700);
            }

            //Wait
            await Task.Delay(250);

            //Who won the game now?
            Match_Results();
        }
        /// <summary>
        /// Retrieves the match results and executes the correct actions accordingly.
        /// </summary>
        private async void Match_Results()
        {
            string result = CalculateResult(true);

            //Display Result
            if (result == "Player")
            {
                //Toon Text
                UpdateDisplayText("Won", "Green");
                //Icon Resizing
                TxtPlayerIcon.Text = "😎";
                LblPlayerScore.FontWeight = FontWeights.Bold;
                IconResize(true);
            }
            else if (result == "Cpu")
            {
                //Toon Text
                UpdateDisplayText("Lost", "Red");
                //Icon Resizing
                TxtPlayerIcon.Text = "😢";
                LblCpuScore.FontWeight = FontWeights.Bold;
                IconResize(false);
            }
            else
            {
                //Toon Text
                UpdateDisplayText("Push", "Orange");
                //Icon Resizing
                TxtPlayerIcon.Text = "😅";
                LblPlayerScore.FontWeight = FontWeights.Bold;
                LblCpuScore.FontWeight = FontWeights.Bold;
                IconResize(null);
            }

            //Hand out PAY
            BetHandling(result);

            //Check Second Deck ONLY IF NEEDED
            if (PlayerDeck2.Count == 0)
            {
                return;
            }

            //Disable buttons
            Button_Enabling("");
            await Task.Delay(1300); //Wait 
            LblPlayerScore.FontWeight = FontWeights.Normal;
            IconResize(null);

            UpdateDisplayText("...", "White");
            await Task.Delay(250);
            ActiveDeck = 2;
            UpdateDisplayScore();
            HighlightActiveDeck();

            await Task.Delay(1300);
            //Execute same steps as above but this time for second deck
            //Calculate Result
            result = CalculateResult(false);

            //Display Result
            if (result == "Player")
            {
                UpdateDisplayText("Won", "Green");
                TxtPlayerIcon.Text = "😎";
                LblPlayerScore.FontWeight = FontWeights.Bold;
                IconResize(true);
            }
            else if (result == "Cpu")
            {
                UpdateDisplayText("Lost", "Red");
                TxtPlayerIcon.Text = "😢";
                LblCpuScore.FontWeight = FontWeights.Bold;
                IconResize(false);
            }
            else
            {
                UpdateDisplayText("Push", "Orange");
                TxtPlayerIcon.Text = "😅";
                LblPlayerScore.FontWeight = FontWeights.Bold;
                LblCpuScore.FontWeight = FontWeights.Bold;
                IconResize(null);
            }

            //Hand out PAY
            BetHandling(result);
        }
        /// <summary>
        /// Gives the player its money.
        /// </summary>
        /// <param name="result"></param>
        private void BetHandling(string result)
        {
            if (result == "Player")
            {
                //BLACKJACK? [NIET MOGELIJK BIJ EEN SPLIT!]
                if (PlayerDeck.Count == 2 && PlayerHandValue.Sum() == 21 && PlayerHandValue2.Count == 0)
                {
                    float i = Bet * 2.5f;
                    Money += Convert.ToInt16(Math.Round(i));
                    UpdateDisplayText($"BLACKJACK! + €{i}", "Gold"); //Override default "Won x amount" text
                }
                else
                {
                    Money += Bet * 2;
                    TxtResults.Text += $" €{Bet * 2}";
                }
            }
            else if (result == "Draw")
            {
                Money += Bet;
            }
            else
            {
                TxtResults.Text += $" €{Bet}";
            }

            //Update historiek
            UpdateHistoriek(result);
            //Update Header
            MoneyRelatedActions();

            //Game Over?
            GameOver();
        }
        /// <summary>
        /// Checks if the player has lost and cannot continue anymore.
        /// </summary>
        private async void GameOver()
        {
            //Can we continue to play?
            if (Money > 0)
            {
                //Enabling da buttons
                Button_Enabling("ChangeBet Continue AllIn");
                return;
            }

            //Avoid a game over if player has a split and 2nd deck hasn't been checked yet
            if (PlayerHandValue2.Count > 0 && ActiveDeck == 1) { return; }

            //Disable buttons
            Button_Enabling("");

            //Play game over animation
            await Task.Delay(1500);
            while (TxtResults.Opacity > 0)
            {
                TxtResults.Opacity -= 0.05;
                await Task.Delay(50);
            }

            UpdateDisplayText("Game Over", "White");

            while (TxtResults.Opacity < 1)
            {
                TxtResults.Opacity += 0.02;
                await Task.Delay(25);
            }

            //Show new game button after x amount of seconds
            await Task.Delay(1500);
            Button_Enabling("NewGame");
        }
        #endregion

        #region Game Balance
        /// <summary>
        /// Returns a Card Image.
        /// </summary>
        /// <param name="player">Is the card for the player?</param>
        /// <param name="deck">For which deck is the card? <para>If for cpu, then this value is not important.</para></param>
        /// <returns></returns>
        public Image Card(bool player, int deck)
        {
            int leftMargin = 0; //Used as the margin

            //Match margin so they stack on top of each other. [1st card margin 0, all others -110]
            if (player)
            {
                if (PlayerDeckPanel.Children.Count != 0 && deck == 1)
                {
                    leftMargin = -110;
                }
                else if (PlayerDeckPanel2.Children.Count != 0 && deck == 2)
                {
                    leftMargin = -110;
                }
            }
            else
            {
                if (CpuDeckPanel.Children.Count != 0)
                {
                    leftMargin = -110;
                }
            }

            //Modify Cardname
            string ModifName = CardName;
            ModifName = string.Concat(ModifName.Replace("-", ""));

            //Create Card
            Image Card = new Image
            {
                Height = 190,
                Width = 140,
                Stretch = Stretch.UniformToFill,
                Source = new BitmapImage(new Uri($"/Cards/{CardName}.png", UriKind.Relative)),
                Margin = new Thickness(leftMargin, 0, 0, 0),
                Name = ModifName,
                Cursor = Cursors.Hand
            };

            //Add event handler to card
            Card.MouseDown += Card_MouseDown;

            return Card;
        }
        /// <summary>
        /// Opens a new window displaying the selected Card.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Card_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Image card = (Image)sender;

            //Modify the name again
            string name = card.Name;
            int hyphenIndex = name.IndexOfAny("ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray(), 1);
            name = name.Insert(hyphenIndex, "-");

            //Pass the name through and open a new window
            Properties.Settings.Default.CardName = name;
            CardViewWindow window = new CardViewWindow();
            window.ShowDialog();
        }
        /// <summary>
        /// Checks if there are any cards left in the deck.
        /// <para>
        /// If empty, reshuffles the deck and notifies the player that the deck got reshuffled.
        /// </para>
        /// </summary>
        private async void ShuffleDeck()
        {
            bool playAnimation = false;

            //Reshuffle AllCards if all cards have been played
            if (CardsInGame.Count == Deck.Count())
            {
                CardsInGame.Clear();
                playAnimation = true;
                TxtDeckCount.Text = (Deck.Count() - CardsInGame.Count()).ToString();
            }

            if (playAnimation)
            {
                int margin = 0;
                ShuffleNotifBorder.Visibility = Visibility.Visible;

                //Animation
                while (margin != -10)
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
        /// <summary>
        /// Checks if the total value exceeds 21.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <param name="player">Are we checking player's hand?</param>
        void CheckOverflow(object sender, RoutedEventArgs e, bool player)
        {
            if (player)
            {
                if (PlayerHandValue.Sum() > 21 && ActiveDeck == 1)
                {
                    FixOverflow(sender, e, player);
                }
                else if (PlayerHandValue2.Sum() > 21 && ActiveDeck == 2)
                {
                    FixOverflow(sender, e, player);
                }
            }
            else
            {
                if (CpuHandValue.Sum() > 21)
                {
                    FixOverflow(sender, e, player);
                }
            }
        }
        /// <summary>
        /// Looks for an 11 value and converts it into a 1.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <param name="player">Is it for the player?</param>
        private void FixOverflow(object sender, RoutedEventArgs e, bool player)
        {
            if (player)
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
        /// <summary>
        /// Looks at the card's name and returns the corresponding value as an integer.
        /// </summary>
        /// <returns></returns>
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
        /// <summary>
        /// Checks if CPU has an 11 value and converts that value into a 1.
        /// <para>
        /// (!) CPU cannot end with a soft 17+.
        /// </para>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
        /// <summary>
        /// Looks at the results and determines who has won.
        /// </summary>
        /// <param name="deck1">Are we checking the first deck?</param>
        /// <returns></returns>
        public string CalculateResult(bool deck1)
        {
            if (deck1)
            {
                if (CpuHandValue.Sum() == 21 && PlayerHandValue.Sum() != 21)
                {
                    return "Cpu";
                }
                else if (CpuHandValue.Sum() == 21 && PlayerHandValue.Sum() == 21)
                {
                    return "Draw";
                }
                else if (CpuHandValue.Sum() > 21 && PlayerHandValue.Sum() > 21)
                {
                    return "Draw";
                }
                else if (CpuHandValue.Sum() == PlayerHandValue.Sum())
                {
                    return "Draw";
                }
                else if (CpuHandValue.Sum() > 21)
                {
                    return "Player";
                }
                else if (CpuHandValue.Sum() > PlayerHandValue.Sum())
                {
                    return "Cpu";
                }
                else if (PlayerHandValue.Sum() > 21)
                {
                    return "Cpu";
                }
                else
                {
                    return "Player";
                }
            }
            else
            {
                if (CpuHandValue.Sum() == 21 && PlayerHandValue2.Sum() != 21)
                {
                    return "Cpu";
                }
                else if (CpuHandValue.Sum() == 21 && PlayerHandValue2.Sum() == 21)
                {
                    return "Draw";
                }
                else if (CpuHandValue.Sum() > 21 && PlayerHandValue2.Sum() > 21)
                {
                    return "Draw";
                }
                else if (CpuHandValue.Sum() == PlayerHandValue2.Sum())
                {
                    return "Draw";
                }
                else if (CpuHandValue.Sum() > 21)
                {
                    return "Player";
                }
                else if (CpuHandValue.Sum() > PlayerHandValue2.Sum())
                {
                    return "Cpu";
                }
                else if (PlayerHandValue2.Sum() > 21)
                {
                    return "Cpu";
                }
                else
                {
                    return "Player";
                }
            }
        }
        /// <summary>
        /// Updates the text on the HEADER.
        /// <para>
        /// Also edits the sliders on BetPanel.
        /// </para>
        /// </summary>
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
        #endregion

        #region Other
        /// <summary>
        /// Starts playing the background music
        /// (Properties.Settings.Default.Music)
        /// </summary>
        private void PlaySound()
        {
            //Toggle music
            switch (Properties.Settings.Default.Music)
            {
                case "The_Holy_Queen":
                    {
                        MusicPlayer = new SoundPlayer(Properties.Resources.The_Holy_Queen);
                        break;
                    }
                case "Goldenvengeance":
                    {
                        MusicPlayer = new SoundPlayer(Properties.Resources.Goldenvengeance);
                        break;
                    }
                case "Wandering_Rose":
                    {
                        MusicPlayer = new SoundPlayer(Properties.Resources.Wandering_Rose);
                        break;
                    }
                case "Jazz":
                    {
                        MusicPlayer = new SoundPlayer(Properties.Resources.Jazz);
                        break;
                    }
                case "Jazz2":
                    {
                        MusicPlayer = new SoundPlayer(Properties.Resources.Jazz2);
                        break;
                    }
            }

            //Play Sound
            MusicPlayer.PlayLooping();
        }
        /// <summary>
        /// Starts an unending loop to track IRL time.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            ///<summary>
            ///Timer is avoided because it's inaccurate and unreliable :|
            /// </summary>
            /// dont @ me

            //Display time
            bool active = true;
            while (active)
            {
                TxtTime.Text = DateTime.Now.ToString($"HH:mm:ss");
                await Task.Delay(1000);
            }
        }
        /// <summary>
        /// Keeps track of match results and adds it to string[] Historiek
        /// </summary>
        /// <param name="result"></param>
        private void UpdateHistoriek(string result)
        {
            string Resultaat = "";

            if (result == "Player")
            {
                Resultaat = "Gewonnen";
            }
            else if (result == "Cpu")
            {
                Resultaat = "Verloren";
            }
            else
            {
                Resultaat = "Push";
            }

            string historiekLog = $"[Ronde {Ronde}]: €{Bet} {Resultaat} || [Geld= €{Money}]";

            //Voeg waarde toe aan eerst lege positie
            int index = Array.IndexOf(Historiek, Historiek.FirstOrDefault(x => string.IsNullOrWhiteSpace(x)));

            if (index >= 0)
            {
                Historiek[index] = historiekLog;
            }
            else //Als er geen meer ruimte is
            {
                //Shuif alle waardes op
                for (int i = 0; i < Historiek.Length - 1; i++)
                {
                    Historiek[i] = Historiek[i + 1];
                }

                //Voeg nieuwe waarde toe aan eerste positie
                Historiek[Historiek.Length - 1] = historiekLog;
            }

        }
        /// <summary>
        /// Checks if the background music has been modified.
        /// <para>
        /// No need to restart a song if it hasn't changed.
        /// </para>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Activated(object sender, EventArgs e)
        {
            //Is de muziek verandert?
            if (Music != Properties.Settings.Default.Music)
            {
                Music = Properties.Settings.Default.Music;
                PlaySound();
            }
        }
        #endregion
    }
}
