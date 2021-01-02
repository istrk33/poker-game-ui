using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace PokerProject
{
    /// <summary>
    /// Stage type for game setup steps. Playing : no longer in setup
    /// </summary>
    public enum Stage
    {
        Playing = -1,
        GameChoice,
        NumberOfPlayers,
        PlayerTypes,
        PlayerNames,
        Ante,
        AnteDoubles
    }
    /// <summary>
    /// Controler constructed for routing player interaction to the view.
    /// The current view used here is the Console.
    /// Singletonized.
    /// </summary>
    public static class GameControler
    {
        #region attributes
        private static Stage setupStage;
        private static RuleSet theRules;
        private static readonly Dictionary<string, Action<string>> commands = new Dictionary<string, Action<string>>();
        private static GameView theView;
        #endregion
        #region initialization methods
        /// <summary>
        /// Initialize after creation, combining model and view references.
        /// </summary>
        /// <param name="theRules">The RuleSet object reference.</param>
        /// <param name="theView">The GameView object reference.</param>
        public static void Initialize(RuleSet theRules, GameView theView)
        {
            MakeCommands();
            GameControler.theRules = theRules;
            GameControler.theView = theView;
            // setupStage = Stage.GameChoice; // 2
            // DisplayAvailableGames(); // 2
            setupStage = Stage.Playing;
            DisplayGameStart();
        }
        private static void MakeCommands()
        {
            commands.Add("help", DisplayHelp);
            commands.Add("ante", _ => PlaceNecessaryAnte());
            commands.Add("out", _ => QuitGame());
            commands.Add("status", _ => DisplayStatus());
            commands.Add("bet", PlaceBet);
            commands.Add("check", _ => CheckBet());
            commands.Add("fold", _ => FoldBet());
            commands.Add("next", _ => AcceptCurrentRound());
            commands.Add("cards", _ => DisplayCards());
            commands.Add("discard", DiscardCard);
            commands.Add("quit", _ => Environment.Exit(0));
        }
        #endregion
        #region public interaction methods callable by the game logic
        /// <summary>
        /// Method called when an AI player checks.
        /// </summary>
        /// <param name="name">The name of the player.</param>
        public static void DisplayAIChecks(string name)
        {
            theView.WriteLine(name + " checks.");
            theView.Refresh();
        }
        /// <summary>
        /// Method called when an AI player gets a card.
        /// </summary>
        /// <param name="name">The name of the player.</param>
        /// <param name="c">The received card.</param>
        public static void DisplayAIReceives(string name, Card c)
        {
            theView.WriteLine(name + " receives " + (c.Down ? " a card." : c.ToString()));
            theView.Refresh();
        }
        /// <summary>
        /// Method called when an AI player places their ante.
        /// </summary>
        /// <param name="name">The name of the player.</param>
        /// <param name="ante">The amount placed as ante.</param>
        public static void DisplayAIPlacesAnte(string name, int ante)
        {
            theView.WriteLine(name + " places " + ante + " as ante.");
            theView.Refresh();
        }
        /// <summary>
        /// Method called when an AI player folds.
        /// </summary>
        /// <param name="name">The name of the player.</param>
        public static void DisplayAIFolds(string name)
        {
            theView.WriteLine(name + " folds.");
            theView.Refresh();
        }
        /// <summary>
        /// Method called when an AI player places a bet.
        /// </summary>
        /// <param name="name">The name of the player.</param>
        /// <param name="amount">The amount of the bid.</param>
        public static void DisplayAIBets(string name, int amount)
        {
            theView.WriteLine(name + " bets " + amount);
            theView.Refresh();
        }

        /// <summary>
        /// Method called when an AI player bets all.
        /// </summary>
        /// <param name="name">The name of the player.</param>
        public static void DisplayAllIn(string name)
        {
            theView.WriteLine(name + " is all in.");
            theView.Refresh();
        }
        /// <summary>
        /// Method called when a player is out of the game because all their money is spent.
        /// </summary>
        /// <param name="name">The name of the player.</param>
        public static void DisplayLoss(string name)
        {
            theView.WriteLine("With no money left, " + name + " has lost the game.");
            theView.Refresh();
        }
        /// <summary>
        /// Method called when an human player needs to act.
        /// </summary>
        /// <param name="acts">The list of actions given as commands.</param>
        public static void DisplayPossibleActions(List<string> acts)
        {
            /*
             * 1 - Display bet dialog
             */
            if (theRules.Round == Rule.Bet)
            {
                ExampleBetView ebv = new ExampleBetView(theRules.CurrentMinBettable, theRules.PlayerMoney, theRules.CheckOnly);
                switch (ebv.ShowDialog())
                {
                    case DialogResult.Cancel: FoldBet(); break;
                    case DialogResult.OK: PlaceBet("all"); break;
                    case DialogResult.Yes: PlaceBet(ebv.BetAmount.ToString()); break;
                    case DialogResult.No: CheckBet(); break;
                }
                return;
            }
            string res = "Possible actions are: ";
            foreach (string action in acts)
            {
                res += action + " ";
            }
            theView.WriteLine(res);
            theView.Refresh();
        }
        /// <summary>
        /// Method called before the turn of each player.
        /// </summary>
        /// <param name="s"></param>
        public static void DisplayInitialPrompt(string s)
        {
            /* 1- Change the display style for the betting round
             **/
            if (theRules.Round == Rule.Bet && theRules.HumanPlayer)
            {
                theView.ChangeRoundLabel(s);
            }
            else
            {
                theView.ChangeRoundLabel("");
            }

            theView.WriteLine(s);
            theView.Refresh();
        }
        /// <summary>
        /// Method called when the game is over (one person not out / without money).
        /// </summary>
        public static void GameOver()
        {
            string str = $"The game is finished. Winner: {theRules.Winner?.Name} with {theRules.Winner?.Money} chips";
            theView.WriteLine(str);
            theView.Refresh();
            theRules.GameOver();
        }
        /// <summary>
        /// Method called when a player has one a deal.
        /// </summary>
        /// <param name="name">The name of the victor.</param>
        /// <param name="pot">The amount of chips won.</param>
        public static void DisplayDealWon(string name, int pot)
        {
            theView.WriteLine(name + " has won the deal with a pot of " + pot);
            theView.Refresh();
        }
        /// <summary>
        /// Method called when the deal is a draw.
        /// </summary>
        public static void DisplayDraw()
        {
            theView.WriteLine("Draw ! All remaining players split the pot.");
            theView.Refresh();
        }
        /// <summary>
        /// Method called after a showdown won by a player.
        /// </summary>
        /// <param name="hand">The hand that has won the deal.</param>
        public static void DisplayWinningHand(List<Card> hand)
        {
            theView.WriteLine("The winning hand is: " + Card.HandValue(Card.GetBestHand(hand).theBest) + " with :");
            hand.ForEach(c => theView.WriteLine("-" + c.ToString()));
            theView.Refresh();
        }
        /// <summary>
        /// Method called at the start of a new deal.
        /// </summary>
        /// <param name="list">The players still in the game.</param>
        public static void DisplayNewDeal(List<Player> list)
        {
            theView.WriteLine("New deal.");
            theView.WriteLine("Deal #" + theRules.Deal + ", players in:");
            list.ForEach(p => theView.WriteLine("- " + p.Name + " (" + p.Money + ")"));
            theView.Refresh();
        }
        #endregion
        #region public method callable by the view
        /// <summary>
        /// Method called by the view in order to interpret commands entered by the player.
        /// </summary>
        /// <param name="input">The player's input as text.</param>
        public static void Interpret(string input)
        {
            #region The setup stage

            if (setupStage != Stage.Playing)
            {
                switch (setupStage)
                {
                    case Stage.GameChoice:
                        if (theRules.SetGame(input))
                        {
                            theView.WriteLine("Enter the number of players, " + theRules.MinPlayers
                                + " minimum and " + theRules.MaxPlayers + " maximum");
                            setupStage = Stage.NumberOfPlayers;
                        }
                        else
                        {
                            DisplayIncorrectInput();
                        }
                        break;
                    case Stage.NumberOfPlayers:
                        if (theRules.SetNumberOfPlayers(input))
                        {
                            theView.WriteLine("Enter the types of players, H for human, A for AI (e. g., HAAA = one human player and three computer players).");
                            setupStage = Stage.PlayerTypes;
                        }
                        else
                        {
                            DisplayIncorrectInput();
                        }
                        break;
                    case Stage.PlayerTypes:
                        if (theRules.SetPlayerTypes(input))
                        {
                            theView.WriteLine("Optional: enter a name for each player separated by spaces (or return).");
                            setupStage = Stage.PlayerNames;
                        }
                        else
                        {
                            DisplayIncorrectInput();
                        }
                        break;
                    case Stage.PlayerNames:
                        if (theRules.SetPlayerNames(input))
                        {
                            theView.WriteLine("Enter the value for initial ante and minimum bets, default: " + theRules.Ante);
                            setupStage = Stage.Ante;
                        }
                        else
                        {
                            DisplayIncorrectInput();
                        }
                        break;
                    case Stage.Ante:
                        if (theRules.SetAnte(input))
                        {
                            theView.WriteLine("Enter yes/no for ante doubling at each deal, default: " + (theRules.AnteDoubles ? "yes" : "no"));
                            setupStage = Stage.AnteDoubles;
                        }
                        else
                        {
                            DisplayIncorrectInput();
                        }
                        break;
                    case Stage.AnteDoubles:
                        if (theRules.SetAnteDoubles(input))
                        {
                            setupStage = Stage.Playing;
                            DisplayGameStart();
                        }
                        else
                        {
                            DisplayIncorrectInput();
                        }
                        break;
                    default:
                        break;
                }
                return;
            }

            #endregion
            #region Generic method call mecanism
            string command = input.Split(' ')[0];
            string arguments = input.Contains(" ") ? input.Substring(input.IndexOf(' ') + 1) : "";
            if (commands.ContainsKey(command))
            {
                commands[command](arguments);
            }
            else
            {
                theView.WriteLine("Unknown command: " + input);
                DisplayHelp("");
            }
            #endregion
        }

        #endregion
        #region private methods for game flow / player actions
        private static void DisplayAvailableGames()
        {
            theView.WriteLine("Available games are:");
            theRules.AvailableGames.ForEach(s => theView.WriteLine(s));
            theView.WriteLine("Enter the full name of the chosen game.");
            theView.Refresh();
        }
        private static void DiscardCard(string card)
        {
            if (theRules.Round != Rule.MayDrawDown && theRules.Round != Rule.MayDrawUp)
            {
                theView.WriteLine("Cannot discard now.");
                theView.Refresh();
                return;
            }
            else if (card == null || card.Equals(""))
            {
                theView.WriteLine("Discard what?");
                theView.Refresh();
                return;
            }
            else if (theRules.DiscardCard(card))
            {
                theView.WriteLine("You discarded " + Card.ParseTwoLettersToString(card));
                theView.Refresh();
            }
            else
            {
                DisplayIncorrectInput();
            }
        }
        private static void DisplayCards()
        {
            List<Card> hand = theRules.GetHand();
            List<Card> common = theRules.CommonCards;
            List<(Player, Card)> up = theRules.GetUpCards();
            string str = "Your hand is: ";
            foreach (Card c in hand)
            {
                if (c.Down) str += c.ToString() + " ";
                else str += c.ToString() + " (up) ";
            }
            theView.WriteLine(str);
            str = "";
            if (common != null && common?.Count > 0)
            {
                theView.WriteLine(str + " - common cards are: ");
                foreach (Card c in common) str += c.ToString() + " ";
                theView.WriteLine(str);
            }
            if (up != null && up?.Count > 0)
            {
                theView.WriteLine(" - up card of all players are: ");
                foreach (var (p, c) in up) theView.WriteLine(c.ToString() + " for " + p.Name);
            }
            theView.Refresh();
        }
        private static void AcceptCurrentRound()
        {
            List<Card> cards;
            bool down = false;
            bool common = false;
            switch (theRules.Round)
            {
                case Rule.DealCardDown:
                    down = true;
                    cards = new List<Card> { theRules.DealCardDown() };
                    break;
                case Rule.DealCardUp:
                    cards = new List<Card> { theRules.DealCardUp() };
                    break;
                case Rule.DealCardCommon:
                    common = true;
                    cards = new List<Card> { theRules.DealCardCommon() };
                    break;
                case Rule.MayDrawDown:
                    down = true;
                    cards = theRules.DrawCardsDown();
                    break;
                case Rule.MayDrawUp:
                    cards = theRules.DrawCardsUp();
                    break;
                default:
                    theView.WriteLine("Cannot pass right now.");
                    return;
            }
            if (cards != null && cards?.Count > 0)
            {
                string str = common ? "Common cards received: " : "You receive: ";
                foreach (Card c in cards)
                {
                    str += c.ToString() + " " + (down ? "" : "face up ");
                }
                theView.WriteLine(str);
            }
            theView.Refresh();
            theRules.NextPlayerRoundOrDeal();
        }
        private static void CheckBet()
        {
            if (theRules.Round != Rule.Bet)
            {
                theView.WriteLine("Cannot check right now.");
                return;
            }
            int amount = theRules.CheckBet();
            if (amount > 0)
            {
                theView.WriteLine(theRules.PlayerName + " bets " + amount + " to check.");
            }
            else
            {
                theView.WriteLine(theRules.PlayerName + "checks.");
            }
            theView.Refresh();
            theRules.NextPlayerRoundOrDeal();
        }
        private static void FoldBet()
        {
            if (theRules.Round != Rule.Bet)
            {
                theView.WriteLine("Cannot fold right now.");
                return;
            }
            theRules.FoldBet();
            theView.WriteLine(theRules.PlayerName + " folds.");
            theView.Refresh();
            theRules.NextPlayerRoundOrDeal();
        }
        private static void PlaceBet(string s)
        {
            if (theRules.Round != Rule.Bet)
            {
                theView.WriteLine("Cannot bet right now.");
                theView.Refresh();
                return;
            }
            else if (theRules.CheckOnly)
            {
                theView.WriteLine("Everyone has already bet on this round, please check or fold.");
                theView.Refresh();
                return;
            }
            else if (s == null || s.Equals(""))
            {
                theView.WriteLine("Bet what?");
                theView.Refresh();
                return;
            }
            else
            {
                (bool success, int amount) = theRules.PlaceBet(s);
                if (success)
                {
                    theView.WriteLine(theRules.PlayerName + " bets " + amount);
                    theView.Refresh();
                    theRules.NextPlayerRoundOrDeal();
                }
                else
                {
                    DisplayIncorrectInput();
                }
            }
        }
        private static void DisplayStatus()
        {
            theView.WriteLine("<<<<--Current Status-->>>>");
            theView.WriteLine("The game is: " + theRules.GameName + ", you are :" + theRules.PlayerName);
            theView.WriteLine("You have " + theRules.PlayerMoney + " chips.");
            DisplayCards();
            List<Player> opi = theRules.OtherPlayers;
            theView.WriteLine("Other players are: ");
            foreach (Player p in opi)
            {
                theView.WriteLine("- " + p.Name + ": " + p.Money +
                    (p.PlayerStatus == PlayerStatus.In ? " (in, " : "(out, ") +
                    (p.PlayerType == PlayerType.AI ? "ai) " : "human"));
            }
            theView.Refresh();
        }
        private static void PlaceNecessaryAnte()
        {
            if (theRules.Round != Rule.Ante)
            {
                theView.WriteLine("Cannot place the ante right now.");
                return;
            }

            int ante = theRules.PlaceNecessaryAnte();
            if (ante > 0)
            {
                theView.WriteLine(theRules.PlayerName + " places " + ante + " as ante.");
            }
            theView.Refresh();
            theRules.NextPlayerRoundOrDeal();
        }
        private static void QuitGame()
        {
            if (theRules.Round != Rule.Bet && theRules.Round != Rule.Ante)
            {
                theView.WriteLine("Cannot bet right now.");
                return;
            }
            int gains = theRules.QuitGame();
            theView.WriteLine(theRules.PlayerName + " has quit the game with gains of " + gains);
            theView.Refresh();
            theRules.NextPlayerRoundOrDeal();
        }
        private static void DisplayGameStart()
        {
            theView.WriteLine("Game starting.");
            theView.Refresh();
            theRules.NewDeal();
        }
        private static void DisplayIncorrectInput()
        {
            theView.WriteLine("Incorrect input, please check.");
            theView.Refresh();
        }
        private static void DisplayHelp(string s)
        {
            switch (s)
            {
                case "":
                    string str = "Possible commands: ";
                    foreach (string k in commands.Keys)
                    {
                        str += k + " ";
                    }
                    theView.WriteLine(str);
                    break;
                case "help":
                    theView.WriteLine("Prints help. Without arguments: list available commands, with: prints help on a command.");
                    break;
                case "ante":
                    theView.WriteLine("Places the necessary ante, needed to start a deal. Only available in the ante round. No arguments.");
                    break;
                case "out":
                    theView.WriteLine("Quits the game with your current savings, minus the initial money. Available in ante and bet rounds. No arguments.");
                    break;
                case "status":
                    theView.WriteLine("Displays the current status: money, round, deal, game. Always available. No arguments.");
                    break;
                case "bet":
                    theView.WriteLine("Places a bet. Argument: the amount to bid (the minimum is either the ante or up to the current bet, the maximum is your remaining money). Only available in bet rounds, only once per player (check or fold after that).");
                    break;
                case "check":
                    theView.WriteLine("Checks the bet, automatically bidding either the amount to match the current bet or zero, if you have enough money. Only available during bet rounds. No arguments.");
                    break;
                case "fold":
                    theView.WriteLine("Folds the bet, putting you out of the deal. Only available during bet rounds. No arguments.");
                    break;
                case "next":
                    theView.WriteLine("Proceeds. In deal rounds, deals a card. In draw rounds, ends discarding and draw as many cards as were discarded. Only available during these rounds. No arguments.");
                    break;
                case "cards":
                    theView.WriteLine("Displays your cards, common cards, and cards of other players that are dealt up.");
                    break;
                case "discard":
                    theView.WriteLine("Discard one card to draw another later. Argument: the card to discard, one character for the face (23456789tjqa) and another for the suit (cdhs); e. g. th for the Ten of Hearts. Enter next when finished. Only available in draw rounds.");
                    break;
                case "quit":
                    theView.WriteLine("Ends the game (without saving)");
                    break;
                default:
                    theView.WriteLine("Unkown command: " + s);
                    break;
            }
            theView.Refresh();
        }
        #endregion
    }
}