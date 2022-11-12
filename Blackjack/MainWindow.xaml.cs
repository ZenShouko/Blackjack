using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Media;

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
        //LOGS
        StringBuilder CalculateValueCpuLogs = new StringBuilder();
        StringBuilder CpuTurnLogs = new StringBuilder();
        //Lijsten
        List<string> Deck = new List<string>(); //ALL 52 CARDS IN THE GAME
        List<string> CardsInGame = new List<string>(); //ALL CARDS IN USE (player and cpu combined)
        List<int> PlayerHandValue = new List<int>(); //ALL CARD VALUES FROM PLAYER
        List<int> CpuHandValue = new List<int>(); //ALL CARD VALUES FROM CPU
        //Andere Variabelen
        bool AutoStand = false;
        bool Soft17Clear = false;
        int CpuValue;

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

        private void BtnDeel_Click(object sender, RoutedEventArgs e)
        {
            //Enablind Buttons
            BtnHit.IsEnabled = true;
            BtnStand.IsEnabled = true;
            BtnDeel.IsEnabled = false;
            //Maak lijst leeg
            CardsInGame.Clear();
            PlayerDeck.Items.Clear();
            CpuDeck.Items.Clear();
            PlayerHandValue.Clear();
            CpuHandValue.Clear();
            CpuTurnLogs.Clear();
            AutoStand = false;
            Soft17Clear = false;
            LblResults.Visibility = Visibility.Hidden;

            //Verdeel 2 kaarten
            for (int i = 0; i < 2; i++)
            {
                Give_Player_Card(sender, e);
            }

            ///[ADDING SPECIFIC CARDS]
            //PlayerDeck.Items.Add(Deck.ElementAt(49));
            //CalculateValueCONCEPT(sender, e);
            //PlayerDeck.Items.Add(Deck.ElementAt(50));
            //CalculateValueCONCEPT(sender, e);

            //Voor CPU
            GiveCpu_Card(sender, e);
            if (!AutoStand) //Add secret card only when player does not have a blackjack
            {
                CpuDeck.Items.Add("Secret");
                LblCpuScore.Text += "+";
            }
        }

        private void BtnStand_Click(object sender, RoutedEventArgs e)
        {
            //Enabling da buttons
            BtnHit.IsEnabled = false;
            BtnStand.IsEnabled = false;
            BtnDeel.IsEnabled = true;

            //Remove Secret Card
            CpuDeck.Items.Remove("Secret");
            //Start CPU's turn
            Cpu_Hit(sender, e);
        }

        private void Give_Player_Card(object sender, RoutedEventArgs e)
        {
            Random random = new Random();
            int index = random.Next(0, 52);

            while (CardsInGame.Contains(Deck.ElementAt(index)))
            {
                index = random.Next(0, 52);
            }

            CardsInGame.Add(Deck.ElementAt(index));
            PlayerDeck.Items.Add(Deck.ElementAt(index));

            CalculateValuePlayer(sender, e);
        }

        private void GiveCpu_Card(object sender, RoutedEventArgs e)
        {
            Random random = new Random();
            int index = random.Next(0, 52);

            while (CardsInGame.Contains(Deck.ElementAt(index)))
            {
                index = random.Next(0, 52);
            }

            CardsInGame.Add(Deck.ElementAt(index));
            CpuDeck.Items.Add(Deck.ElementAt(index));

            CalculateValueCpu(sender, e);
        }

        void CalculateValuePlayer(object sender, RoutedEventArgs e)
        {
            //Vars
            int Total = 0;
            StringBuilder LOG = new StringBuilder(); //Logs om ace te volgen
            /*------------------------------------------------------------------*/
            LOG.AppendLine("Activated CalculateValueCONCEPT() >>>");

            //On what Index is the last card?
            int LastIndex = PlayerDeck.Items.Count - 1;
            LOG.AppendLine($"Set Last Index Number to {LastIndex}.");
            //Put the value into a string
            string card = PlayerDeck.Items[LastIndex].ToString();
            LOG.AppendLine($"Set last card to {card}.");

            if (card.Contains("Ace"))
            {
                PlayerHandValue.Add(11);
            }
            else if (card.Contains("Two"))
            {
                PlayerHandValue.Add(2);
            }
            else if (card.Contains("Three"))
            {
                PlayerHandValue.Add(3);
            }
            else if (card.Contains("Four"))
            {
                PlayerHandValue.Add(4);
            }
            else if (card.Contains("Five"))
            {
                PlayerHandValue.Add(5);
            }
            else if (card.Contains("Six"))
            {
                PlayerHandValue.Add(6);
            }
            else if (card.Contains("Seven"))
            {
                PlayerHandValue.Add(7);
            }
            else if (card.Contains("Eight"))
            {
                PlayerHandValue.Add(8);
            }
            else if (card.Contains("Nine"))
            {
                PlayerHandValue.Add(9);
            }
            else if (card.Contains("Ten") || card.Contains("Jack") || card.Contains("Queen") || card.Contains("King"))
            {
                PlayerHandValue.Add(10);
            }

            //Reset Total value and count 
            Total = 0;
            foreach (var value in PlayerHandValue)
            {
                Total += value;
            }

            //Correct Aces if we exceed 21
            if (Total > 21)
            {
                LOG.AppendLine("Total Exceeds 21 after adding the values.");
                if (PlayerHandValue.Contains(11)) //can we switch 11 with 1?
                {
                    LOG.AppendLine("Replaced [11] with [1].");
                    PlayerHandValue.Remove(11);
                    PlayerHandValue.Add(1);
                }
                else //Auto Stand if there is no 11 to replace
                {
                    LOG.AppendLine("No [11] value found to replace. Activating AutoStand.");
                    AutoStand = true;
                    BtnStand_Click(sender, e);
                }
            }

            //Reset Total value and count again
            Total = 0;
            foreach (var value in PlayerHandValue)
            {
                Total += value;
            }

            //Did we get a 21?
            if (Total == 21)
            {
                LOG.AppendLine("We have 21! Activating AutoStand:)");
                AutoStand = true;
                BtnStand_Click(sender, e);
            }

            //Reset Total value and count again
            Total = 0;
            foreach (var value in PlayerHandValue)
            {
                Total += value;
            }

            //Display
            LblSpelerScore.Text = Total.ToString();
        }

        private void CalculateValueCpu(object sender, RoutedEventArgs e)
        {
            CalculateValueCpuLogs.AppendLine("CalculateValueCpu() got activated >>");
            //On what Index is the last card?
            int LastIndex = CpuDeck.Items.Count - 1;
            //Put the value into a string
            string card = CpuDeck.Items[LastIndex].ToString();

            if (card.Contains("Ace"))
            {
                CpuHandValue.Add(11);
            }
            else if (card.Contains("Two"))
            {
                CpuHandValue.Add(2);
            }
            else if (card.Contains("Three"))
            {
                CpuHandValue.Add(3);
            }
            else if (card.Contains("Four"))
            {
                CpuHandValue.Add(4);
            }
            else if (card.Contains("Five"))
            {
                CpuHandValue.Add(5);
            }
            else if (card.Contains("Six"))
            {
                CpuHandValue.Add(6);
            }
            else if (card.Contains("Seven"))
            {
                CpuHandValue.Add(7);
            }
            else if (card.Contains("Eight"))
            {
                CpuHandValue.Add(8);
            }
            else if (card.Contains("Nine"))
            {
                CpuHandValue.Add(9);
            }
            else if (card.Contains("Ten") || card.Contains("Jack") || card.Contains("Queen") || card.Contains("King"))
            {
                CpuHandValue.Add(10);
            }

            //Count Total Value
            int totalCPU = 0;
            foreach (int value in CpuHandValue)
            {
                CalculateValueCpuLogs.AppendLine($"Found a value in CpuHandValue! Adding [{value}] to Total[{totalCPU}]");
                totalCPU += value;
            }

            //Display Total Value
            CalculateValueCpuLogs.AppendLine($"Now Displaying Total[{totalCPU}]");
            LblCpuScore.Text = totalCPU.ToString();
            //TxtLogs.Text = CalculateValueCpuLogs.ToString();
        }

        private void Count_CpuValue(object sender, RoutedEventArgs e)
        {
            CpuValue = 0;

            foreach (int value in CpuHandValue)
            {
                CpuValue += value;
            }
        }
        private void Cpu_Hit(object sender, RoutedEventArgs e)
        {
            CpuTurnLogs.AppendLine("Cpu_Hit() got activated >>");
            //Give CPU It's second card
            if (CpuDeck.Items.Count == 1)
            {
                CpuTurnLogs.AppendLine("Giving CPU a second card.");
                GiveCpu_Card(sender, e);
            }

            //Count Total Value CPU
            Count_CpuValue(sender, e);

            //Count total value PLAYER
            int totalPlayer = 0;
            foreach (int value in PlayerHandValue)
            {
                totalPlayer += value;
            }
            CpuTurnLogs.AppendLine($"Counted totalvalues. CPU [{CpuValue}] Player [{totalPlayer}].");

            //Does CPU have a blackjack?
            if (CpuValue == 21)
            {
                if (totalPlayer != 21)
                {
                    CpuTurnLogs.AppendLine($"Cpu got a blackjack");
                    Match_Results("Cpu");
                }
                else
                {
                    CpuTurnLogs.AppendLine($"Bot player and cpu got 21.");
                    Match_Results("Draw");
                }

                TxtLogs.Text = CpuTurnLogs.ToString();
                LblCpuScore.Text = CpuValue.ToString();
                return;
            }

            //Keep pulling cards until we reach 17+
            CpuTurnLogs.AppendLine($"Not won yet, pulling cards ... Cpu:[{CpuValue}]");

            while (CpuValue < 17)
            {
                //Calculate CpuHandValue
                Count_CpuValue(sender, e);

                CpuTurnLogs.AppendLine($"- Pulled a card.");
                GiveCpu_Card(sender, e);
                Count_CpuValue(sender, e);
                CpuTurnLogs.Append($" CPU value=[{CpuValue}]");
            }


            //Do we have a soft 17?
            CpuTurnLogs.AppendLine($"Checking for soft 17 ...");
            if (!Soft17Clear) //only clear if we haven't already
            {
                CpuTurnLogs.AppendLine($"We haven't removed an ace yet.");
                CheckSoft17(sender, e);
            }
            else
            {
                CpuTurnLogs.AppendLine($"Soft Ace already cleared.");
            }

            //Recount Value
            Count_CpuValue(sender, e);
            //Keep pulling cards until we reach 17+
            CpuTurnLogs.AppendLine($"Prompt to pull cards untill we reach 17+");
            while (CpuValue < 17)
            {
                Count_CpuValue(sender, e);

                CpuTurnLogs.AppendLine($"- Pulled a card.");
                GiveCpu_Card(sender, e);
                Count_CpuValue(sender, e);
                CpuTurnLogs.Append($" CPU value=[{CpuValue}]");
            }

            //Recount Cpu Value
            Count_CpuValue(sender, e);

            //Who won the game now?
            if(CpuValue > 21 && totalPlayer > 21)
            {
                CpuTurnLogs.AppendLine($"Both scored higher than 21.");
                Match_Results("Draw");
            }
            else if (CpuValue == totalPlayer)
            {
                CpuTurnLogs.AppendLine($"Cpu and Player got same amount.");
                Match_Results("Draw");
            }
            else if (CpuValue > 21)
            {
                CpuTurnLogs.AppendLine($"Cpu pulled higher than 21");
                Match_Results("Player");
            }
            else if (CpuValue > totalPlayer)
            {
                CpuTurnLogs.AppendLine($"Cpu scored higher than player. CPU[{CpuValue}] > Player[{totalPlayer}]");
                Match_Results("Cpu");
            }
            else if(totalPlayer > 21)
            {
                CpuTurnLogs.AppendLine($"Player scored above 21. Cpu below.");
                Match_Results("Cpu");
            }
            else
            {
                CpuTurnLogs.AppendLine($"Player won (No arguments given).");
                Match_Results("Player");
            }

            //Recount cpuvalue
            Count_CpuValue(sender, e);
            //Display
            CpuTurnLogs.AppendLine($"Displaying Value [{CpuValue}]");
            TxtLogs.Text = CpuTurnLogs.ToString();
            LblCpuScore.Text = CpuValue.ToString();
        }

        private void CheckSoft17(object sender, RoutedEventArgs e)
        {
            CpuTurnLogs.AppendLine($"Checking for an ACE.");
            //Is there an ace?
            if (CpuHandValue.Contains(11))
            {
                CpuTurnLogs.AppendLine($"Ace found. Replacing [11] with [1].");
                CpuHandValue.Remove(11);
                CpuHandValue.Add(1);
                Soft17Clear = true;
            }
            else
            {
                CpuTurnLogs.AppendLine($"Ace not found ...");
            }
        }

        private void Match_Results(String Result)
        {
            //Maak result label visible
            LblResults.Visibility = Visibility.Visible;

            //Tell player wether they won or lost
            if (Result == "Player")
            {
                LblResults.Content = "Win";
                LblResults.Foreground = Brushes.Green;
            }
            else if (Result == "Cpu")
            {
                LblResults.Content = "Lose";
                LblResults.Foreground = Brushes.Red;
            }
            else
            {
                LblResults.Content = "Draw";
                LblResults.Foreground = Brushes.DarkOrange;
            }
        }
    }
}
