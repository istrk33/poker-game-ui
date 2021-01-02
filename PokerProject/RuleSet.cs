using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using System.Linq;

namespace PokerProject
{
    /// <summary>
    /// This class contains the status of a single game.
    /// Methods are invoked from, or make use of, the GameController
    /// static methods.
    /// This is the center of the game logic.
    /// </summary>
    public class RuleSet
    {
        #region game-status variables, attributes and properties for a single game
        private bool gameOver = false;
        private int deal = 0;
        private int pot = 0;
        private readonly List<Card> commonCards = new List<Card>();
        private readonly Dictionary<string, XElement> availableGames = new Dictionary<string, XElement>();
        private readonly List<Rule> theRounds = new List<Rule>();
        private int currentRound = 0;
        private readonly List<Player> players = new List<Player>();
        private int minPlayers=0;
        private int baseMoney=0;
        private int maxPlayers=0;
        private int numberOfPlayers=0;
        private int ante=0;
        private bool anteDoubles=false;
        private string gameName="";
        private Player currentPlayer=null;
        private readonly Stack<Card> deck = new Stack<Card>(52);
        private readonly Random rng = new Random();
        /// <summary>
        /// Indicates whether this is a betting round and all players have bet once.
        /// The only possible actions remaining are check and fold.
        /// </summary>
        public bool CheckOnly { get; private set; } = false;
        /// <summary>
        /// Computes the maximum bet placed by all players at this stage.
        /// </summary>
        public int CurrentMaxBet => players.Max(p => p.CurrentBet);
        /// <summary>
        /// Computes the minimum bet placeable at this point.
        /// </summary>
        public int CurrentMinBettable => Math.Max(0, Math.Max(ante, CurrentMaxBet));
        /// <summary>
        /// The minimum number of players for the current game.
        /// </summary>
        public int MinPlayers => minPlayers;
        /// <summary>
        /// The maximum number of players for the current game.
        /// </summary>
        public int MaxPlayers => maxPlayers;
        /// <summary>
        /// The maximum amount for base ante/bets for this game.
        /// </summary>
        public int MaxMoney => baseMoney/2;
        /// <summary>
        /// The current amount of the ante to be placed next round, also used as the minimum possible bet amount.
        /// </summary>
        public int Ante => ante;
        /// <summary>
        /// Whether the ante double at each new deal in this game.
        /// </summary>
        public bool AnteDoubles => anteDoubles;
        /// <summary>
        /// The cards common to all players for this game (can be empty).
        /// </summary>
        public List<Card> CommonCards => commonCards;
        /// <summary>
        /// Computes a list of the string representation of the available games.
        /// </summary>
        public List<string> AvailableGames
        {
            get
            {
                string[] names = new string[availableGames.Count];
                availableGames.Keys.CopyTo(names, 0);
                return new List<string>(names);
            }
        }
        /// <summary>
        /// The name of the current player as a string.
        /// </summary>
        public string PlayerName => currentPlayer.Name;
        /// <summary>
        /// Whether the current player is human or not.
        /// </summary>
        public bool HumanPlayer => currentPlayer.PlayerType == PlayerType.Human;
        /// <summary>
        /// The string representation of the current game.
        /// </summary>
        public string GameName => gameName;
        /// <summary>
        /// The rule used in the current round.
        /// </summary>
        public Rule Round => theRounds[currentRound];
        /// <summary>
        /// The amount of chips in the pot.
        /// </summary>
        public int Pot => pot;
        /// <summary>
        /// The number of the deal in this game.
        /// </summary>
        public int Deal => deal;
        /// <summary>
        /// The amount of chips of the current player.
        /// </summary>
        public int PlayerMoney => currentPlayer.Money;
        /// <summary>
        /// The list of all players. Use carefully, do not modify.
        /// </summary>
        public List<Player> AllPlayers => new List<Player>(players);
        /// <summary>
        /// The list of players that are not the current player. Do not modify.
        /// </summary>
        public List<Player> OtherPlayers => players.FindAll(p => p != currentPlayer);
        /// <summary>
        /// The player that has won the game if any (null until the game is over).
        /// </summary>
        public Player Winner { get; private set; } = null;
        #endregion
        #region constructor and initialisation methods
        /// <summary>
        /// Standard parameter'd constructor.
        /// </summary>
        /// <param name="file">The path of the XML file containing the rules for possible poker games.</param>
        public RuleSet(string file)
        {
            try
            {
                XDocument doc = XDocument.Load(file);
                foreach (XElement el in doc.Root.Nodes())
                {
                    if (el.Name.ToString().Equals("game"))
                    {
                        string name = el.Attribute("name").Value.Trim().ToLower();
                        availableGames.Add(name, el);
                    }
                }
            }
            catch (IOException e)
            {
                Console.Write("File error: " + file + " => " + e);
                Environment.Exit(-1);
            }
            catch (Exception e)
            {
                Console.Write("Unknown error: " + file + " => " + e);
                Environment.Exit(-1);
            }
        }
        private bool LoadGame(XElement el)
        {
            bool res = false;
            try
            {
                foreach (XElement xe in el.Nodes())
                {
                    switch (xe.Name.ToString())
                    {
                        case "players":
                            minPlayers = int.Parse(xe.Attribute("min").Value);
                            maxPlayers = int.Parse(xe.Attribute("max").Value);
                            break;
                        case "money":
                            baseMoney = int.Parse(xe.Attribute("start").Value);
                            ante = int.Parse(xe.Attribute("ante").Value);
                            bool doubles = false;
                            bool.TryParse(xe.Attribute("anteDoubles").Value, out doubles);
                            anteDoubles = doubles;
                            break;
                        case "play":
                            foreach (XElement round in xe.Nodes())
                            {
                                if (round.Name.ToString().Equals("round"))
                                {
                                    Rule roundRule = Rule.Showdown;
                                    switch (round.Value)
                                    {
                                        case "ante": roundRule = Rule.Ante; break;
                                        case "bet": roundRule = Rule.Bet; break;
                                        case "dealDown": roundRule = Rule.DealCardDown; break;
                                        case "dealCommon": roundRule = Rule.DealCardCommon; break;
                                        case "dealUp": roundRule = Rule.DealCardUp; break;
                                        case "mayDrawDown": roundRule = Rule.MayDrawDown; break;
                                        case "mayDrawUp": roundRule = Rule.MayDrawUp; break;
                                        case "showdown": roundRule = Rule.Showdown; break;
                                    }
                                    theRounds.Add(roundRule);
                                }
                            }
                            break;
                    }
                }
                res = true;
            }
            catch (Exception) { }
            return res;
        }
        /// <summary>
        /// Method called to choose a game, loads the appropriate rules if the game is available.
        /// </summary>
        /// <param name="input">The name of the game to load.</param>
        /// <returns>True iff the game has been successfully loaded.</returns>
        public bool SetGame(string input)
        {
            theRounds.Clear();
            bool res = false;
            string normalized = input.Trim().ToLower();
            if (availableGames.ContainsKey(normalized))
            {
                res = LoadGame(availableGames[normalized]);
                if (res) gameName = normalized;
            }
            return res;
        }
        /// <summary>
        /// Method called to set the number of players, sets this attribute if the input is correct.
        /// </summary>
        /// <param name="input">A string that should contain an appropriate number for the players.</param>
        /// <returns>True iff the number of players has been set.</returns>
        public bool SetNumberOfPlayers(string input)
        {
            bool res = false;
            _ = int.TryParse(input, out int num);
            if (num >= minPlayers && num <= maxPlayers)
            {
                numberOfPlayers = num;
                res = true;
            }
            return res;
        }
        /// <summary>
        /// Method called to set the types of players, creates the list of players if the input is correct.
        /// </summary>
        /// <param name="input">A string that should contains a character indicating the type of each player.</param>
        /// <returns>True iff the list of players has been successfully created.</returns>
        public bool SetPlayerTypes(string input)
        {
            bool res = false;
            string normalized = input.Trim().ToLower();
            if (normalized.Length == numberOfPlayers)
            {
                foreach (char c in normalized.ToCharArray())
                {
                    PlayerType pt;
                    if (c == 'a') pt = PlayerType.AI;
                    else if (c == 'h') pt = PlayerType.Human;
                    else break;
                    players.Add(new Player(pt, baseMoney));
                }
                res = players.Count == numberOfPlayers;
                if (!res) players.Clear();
                if (res) currentPlayer = players[0];
            }
            return res;
        }
        /// <summary>
        /// Method called to name each player.
        /// </summary>
        /// <param name="input">Either an empty string or a list of names separated by spaces.</param>
        /// <returns>True iff the names of the players have been successfully set.</returns>
        public bool SetPlayerNames(string input)
        {
            bool res = "".Equals(input);
            string normalized = input.Trim();
            var pNames = normalized.Split(' ');
            foreach (Player p in players)
            {
                if (res) p.SetName("Player " + players.IndexOf(p));
                else if (pNames.Length == players.Count) p.SetName(pNames[players.IndexOf(p)]);
                else break;
            }
            res = res ? true : (pNames.Length == players.Count);
            return res;
        }
        /// <summary>
        /// Method called to set or confirm the ante.
        /// </summary>
        /// <param name="input">Either an empty string or a number.</param>
        /// <returns>True iff the ante has been set or confirmed.</returns>
        public bool SetAnte(string input)
        {
            bool res = "".Equals(input);
            string normalized = input.Trim();
            if (!res)
            {
                _ = int.TryParse(normalized, out int theAnte);
                if (theAnte > 0 && theAnte < baseMoney / 2)
                {
                    ante = theAnte;
                    res = true;
                }
            }
            return res;
        }
        /// <summary>
        /// Method called to set or confirm whether the ante doubles at each deal.
        /// </summary>
        /// <param name="input">Either an empty string, "yes" or "no".</param>
        /// <returns>True iff the doubling of the ante has been set or confirmed.</returns>
        public bool SetAnteDoubles(string input)
        {
            bool res = "".Equals(input);
            string normalized = input.Trim();
            if (!res)
            {
                if (normalized.Equals("yes"))
                {
                    res = anteDoubles = true;
                }
                else if (normalized.Equals("no"))
                {
                    res = true;
                    anteDoubles = false;
                }
            }
            return res;
        }
        #endregion
        #region public methods for player actions callable by GameControler
        /// <summary>
        /// Gets the hand of the current player.
        /// </summary>
        /// <returns>The hand as a list of cards.</returns>
        public List<Card> GetHand()
        {
            return currentPlayer.Hand;
        }
        /// <summary>
        /// Get the cards that are visible to all (face up). 
        /// </summary>
        /// <returns>A list of (P, C) where P is the player that has C has an up-facing card.</returns>
        public List<(Player, Card)> GetUpCards()
        {
            return OtherPlayers.SelectMany(player => player.Hand.Where(card => !card.Down)
                                                                .Select(card => (player, card)))
                                                                .ToList();
        }
        /// <summary>
        /// Marks a card of the current player as discarded.
        /// </summary>
        /// <param name="card">The two-letter string representation of the card to discard.</param>
        /// <returns>True if the string is correct and represents a card of the player's hand that has been marked as discarded.</returns>
        public bool DiscardCard(string card)
        {
            bool res = false;
            if (card != null && card?.Length > 1)
            {
                Face? maybeF = Card.ParseFace(card.Substring(0, 1));
                Suit? maybeS = Card.ParseSuit(card.Substring(1, 1));
                if (maybeF.HasValue && maybeS.HasValue)
                {
                    bool select(Card c) => c.Face == maybeF.Value && c.Suit == maybeS.Value; // inline predicate
                    res = currentPlayer.Hand.Exists(select);
                    if (res)
                    {
                        currentPlayer.Hand.Find(select).Discard = true;
                    }
                }
            }
            return res;
        }
        /// <summary>
        /// Checks the bet for the current player. The action is always available for players that are still in.
        /// </summary>
        /// <returns>The amount spent by the player and added to the pot (can be 0 when the player is all in).</returns>
        public int CheckBet()
        {
            int res = Math.Min(CurrentMaxBet-currentPlayer.CurrentBet, currentPlayer.Money);
            currentPlayer.Spend(res);
            currentPlayer.CurrentBet = CurrentMaxBet;
            pot += res;
            if (currentPlayer.PlayerType==PlayerType.AI)
            {
                GameControler.DisplayAIChecks(currentPlayer.Name);
            }
            return res;
        }
        /// <summary>
        /// Draw some cards, facing up, depending on how many cards where discarded, and remove the discards.
        /// </summary>
        /// <returns>The list of drawn cards, null if there are no discards.</returns>
        public List<Card> DrawCardsUp()
        {
            int cardsToDraw = currentPlayer.Hand.FindAll(c => c.Discard).Count;
            if (cardsToDraw <= 0) return null;
            List<Card> drawn = new List<Card>();
            for (int i = 0; i < cardsToDraw; i++)
            {
                Card c = deck.Pop();
                currentPlayer.AddCard(c);
                drawn.Add(c);
                if (currentPlayer.PlayerType == PlayerType.AI)
                {
                    GameControler.DisplayAIReceives(currentPlayer.Name, c);
                }
            }
            currentPlayer.RemoveDiscards();
            return drawn;
        }
        /// <summary>
        /// Deals one card common for every player.
        /// </summary>
        /// <returns>The card that has been dealt from the deck.</returns>
        public Card DealCardCommon()
        {
            Card c = deck.Pop();
            commonCards.Add(c);
            return c;
        }
        /// <summary>
        /// Draw some cards, facing down, depending on how many cards where discarded, and remove the discards.
        /// </summary>
        /// <returns>The list of drawn cards, null if there are no discards.</returns>
        public List<Card> DrawCardsDown()
        {
            int cardsToDraw = currentPlayer.Hand.FindAll(c => c.Discard).Count;
            if (cardsToDraw <= 0) return null;
            List<Card> drawn = new List<Card>();
            for (int i = 0; i < cardsToDraw; i++)
            {
                Card c = deck.Pop();
                c.Down = true;
                currentPlayer.AddCard(c);
                drawn.Add(c);
                if (currentPlayer.PlayerType == PlayerType.AI)
                {
                    GameControler.DisplayAIReceives(currentPlayer.Name, c);
                }
            }
            currentPlayer.RemoveDiscards();
            return drawn;
        }
        /// <summary>
        /// Deals one card face up.
        /// </summary>
        /// <returns>The card that has been dealt from the deck.</returns>
        public Card DealCardUp()
        {
            Card c = deck.Pop();
            currentPlayer.AddCard(c);
            if (currentPlayer.PlayerType == PlayerType.AI)
            {
                GameControler.DisplayAIReceives(currentPlayer.Name, c);
            }
            return c;
        }
        /// <summary>
        /// Deals one card face down.
        /// </summary>
        /// <returns>The card that has been dealt from the deck.</returns>
        public Card DealCardDown()
        {
            Card c = deck.Pop();
            c.Down = true;
            currentPlayer.AddCard(c);
            if (currentPlayer.PlayerType == PlayerType.AI)
            {
                GameControler.DisplayAIReceives(currentPlayer.Name, c);
            }
            return c;
        }
        /// <summary>
        /// Places a bet. Several possibilities depending on the round, argument and player money.
        /// </summary>
        /// <param name="s">The amount to be bet, a number or "all".</param>
        /// <returns>(S, A) with S indicating if the bet can be correctly placed and A the amount bet if S is true.</returns>
        public (bool success, int amount) PlaceBet(string s)
        {
            (bool success, int amount) res = (false, 0);
            int min = Math.Max(CurrentMaxBet, ante);
            string normalized = s.Trim().ToLower();
            if (normalized.Equals("all"))
            {
                res.success = true;
                res.amount = currentPlayer.CurrentBet+currentPlayer.Money;
                GameControler.DisplayAllIn(currentPlayer.Name);
            }
            else
            {
                bool ok = int.TryParse(normalized, out int bet);
                if (ok && bet <= currentPlayer.CurrentBet+currentPlayer.Money && bet >= min)
                {
                    res = (true, bet);
                }
            }
            if (res.success)
            {
                int match = Math.Min(res.amount - currentPlayer.CurrentBet, currentPlayer.Money);
                currentPlayer.Spend(match);
                pot += match;
                currentPlayer.CurrentBet = res.amount;
            }
            if (currentPlayer.PlayerType==PlayerType.AI)
            {
                GameControler.DisplayAIBets(currentPlayer.Name, res.amount);
            }
            return res;
        }
        /// <summary>
        /// Folds. The current player becomes out of the deal.
        /// </summary>
        public void FoldBet()
        {
            currentPlayer.PlayerStatus = PlayerStatus.OutDeal;
            if (currentPlayer.PlayerType == PlayerType.AI)
            {
                GameControler.DisplayAIFolds(currentPlayer.Name);
            }
        }
        /// <summary>
        /// Quits with the current money. The current player becomes out of the game. AI players cannot do this.
        /// </summary>
        /// <returns>The money left, positive if there are gains, negative if there are losses.</returns>
        public int QuitGame()
        {
            currentPlayer.PlayerStatus = PlayerStatus.OutGame;
            currentPlayer.CurrentBet = 0;
            return currentPlayer.Money - baseMoney;
        }
        /// <summary>
        /// Places the amount of the ante to start the deal, or all money if the player has not enough.
        /// The current player may lose if they do not have money after the ante is placed.
        /// </summary>
        /// <returns>The actual amount payed as ante by the player.</returns>
        public int PlaceNecessaryAnte()
        {
            int ante = Math.Min(this.ante, currentPlayer.Money);
            currentPlayer.Spend(ante);
            pot += ante;
            if (currentPlayer.PlayerType==PlayerType.AI)
            {
                GameControler.DisplayAIPlacesAnte(currentPlayer.Name, ante);
            }
            if (currentPlayer.Money<=0)
            {
                Loss(currentPlayer);
                return -1;
            }
            return ante;
        }
        /// <summary>
        /// Method called to make the player act.
        /// If the player is human, the controller is used to display the possible actions and to wait on
        /// the input of the player.
        /// If the player is an AI, automatic play is engaged.
        /// </summary>
        public void Prompt()
        {
            if (gameOver) return;
            if (currentPlayer == null) GameControler.GameOver();
            GameControler.DisplayInitialPrompt("Game: " + gameName +
                ", Player: " + currentPlayer.Name + ", round= " + theRounds[currentRound]);
            if (currentPlayer.PlayerType == PlayerType.AI)
            {
                AutoPlay(currentPlayer);
            }
            else
            {
                GameControler.DisplayPossibleActions(PossibleActions(theRounds[currentRound]));
            }
        }
        #endregion
        #region private methods for game logic and AI
        private void Loss(Player currentPlayer)
        {
            if (gameOver) return;
            currentPlayer.PlayerStatus = PlayerStatus.OutGame;
            GameControler.DisplayLoss(currentPlayer.Name);
            if (players.FindAll(p=>p.PlayerStatus!=PlayerStatus.OutGame).Count<=1)
            {
                Winner = players.Find(p => p.PlayerStatus != PlayerStatus.OutGame);
                GameControler.GameOver();
            }
        }
        private List<string> PossibleActions(Rule rule)
        {
            List<string> res = null;
            switch(rule)
            {
                case Rule.Ante: res = new List<string> { "ante", "out", "status" }; break;
                case Rule.Bet: res = new List<string> { "bet", "check", "fold", "out", "cards", "status" }; break;
                case Rule.Showdown:
                case Rule.DealCardCommon:
                case Rule.DealCardDown: 
                case Rule.DealCardUp: res = new List<string> { "next", "status", "cards" }; break;
                case Rule.MayDrawDown:
                case Rule.MayDrawUp: res = new List<string> { "discard", "next", "status", "cards" }; break;
            }
            return res;
        }
        private void AutoPlay(Player thePlayer)
        {
            if (gameOver) return;
            if (thePlayer.PlayerType==PlayerType.AI && thePlayer.PlayerStatus==PlayerStatus.In)
            {
                switch(Round)
                {
                    case Rule.Ante: PlaceNecessaryAnte(); break;
                    case Rule.DealCardDown: DealCardDown(); break;
                    case Rule.DealCardCommon: DealCardCommon(); break;
                    case Rule.DealCardUp: DealCardUp(); break;
                    case Rule.MayDrawUp:
                    case Rule.MayDrawDown:
                        List<Card> discardable = currentPlayer.Hand.FindAll(c => !c.Discard);
                        int discarded = rng.Next(discardable.Count);
                        for (int i=0; i<discarded; i++)
                        {
                            Card c = discardable[rng.Next(discardable.Count)];
                            string cStr = c.ToString();
                            string twoLetters = cStr.Split('(')[1].Substring(0, 1) +
                                 cStr.Split('(')[2].Substring(0, 1);
                            discardable.Remove(c);
                            _ = DiscardCard(twoLetters);
                        }
                        if (Round == Rule.MayDrawDown) DrawCardsDown();
                        else DrawCardsUp();
                        break;
                    case Rule.Bet:
                        int min = Math.Max(CurrentMaxBet, ante);
                        if (PlayerMoney<=min)
                        {
                            CheckBet();
                            break;
                        }
                        int boldness = rng.Next(100);
                        if (boldness < 25) FoldBet();
                        else if (CheckOnly) CheckBet();
                        else if (boldness < 70) CheckBet();
                        else if (boldness > 95) _ = PlaceBet("all");
                        else
                        {
                            int max = currentPlayer.Money;
                            if (min >= max) CheckBet();
                            else _ = PlaceBet((rng.Next(max - min) + min).ToString());
                        }
                        break;
                    default: break;
                }
                NextPlayerRoundOrDeal();
            }
        }
        #endregion
        #region control flow methods
        /// <summary>
        /// public method setting the gameOver attribute to true, ending the game.
        /// </summary>
        public void GameOver() => gameOver = true;
        /// <summary>
        /// Method called to start a new deal.
        /// </summary>
        public void NewDeal()
        {
            if (gameOver) return;
            foreach (Player p in players)
            {
                if (p.Money<=0 && p.PlayerStatus != PlayerStatus.OutGame)
                {
                    Loss(p);
                }
            }
            if (players.FindAll(p=>p.PlayerStatus==PlayerStatus.OutGame).Count>=players.Count-1)
            {
                Winner = players.Find(p => p.PlayerStatus != PlayerStatus.OutGame);
                GameControler.GameOver();
                return;
            }
            pot = 0;
            if (deal>0  && anteDoubles)
            {
                ante *= 2;
            }
            currentRound = 0;
            deck.Clear();
            commonCards.Clear();
            List<Card> randomOrderCards = Card.Deck.OrderBy(c => rng.Next(int.MaxValue)).ToList();
            for (int i=Card.Deck.Count; i>0; i--)
            {
                deck.Push(randomOrderCards[rng.Next(0, i)]);
                randomOrderCards.Remove(deck.Peek());
            }
            foreach (Player p in players)
            {
                if (p.PlayerStatus==PlayerStatus.OutDeal)
                {
                    p.PlayerStatus = PlayerStatus.In;
                }
                p.Hand.Clear();
                p.CurrentBet = 0;
                if (p.Money <= 0 && p.PlayerStatus != PlayerStatus.OutGame) Loss(p);
            }
            currentPlayer = players.Find(p => p.PlayerStatus != PlayerStatus.OutGame);
            deal++;
            GameControler.DisplayNewDeal(players.FindAll(p => p.PlayerStatus == PlayerStatus.In));
            Prompt();
        }
        /// <summary>
        /// Method called when a player's actions are finished for the current round, passing to the
        /// next player (if there are players left in this round), the next round (if there are still
        /// rounds left in this deal), the next deal (if there are still players in the game with money),
        /// or possibly ending the game.
        /// </summary>
        public void NextPlayerRoundOrDeal()
        {
            if (gameOver) return;
            if (players.FindAll(p=>p.PlayerStatus==PlayerStatus.In).Count<=1) // Last standing
            {
                Victory(players.Find(p => p.PlayerStatus == PlayerStatus.In));
                CheckOnly = false;
                if (!gameOver) NewDeal();
                return;
            }
            if (Round==Rule.DealCardCommon) // Common - only first player then next round
            {
                CheckOnly = false;
                currentRound++;
                currentPlayer = players.Find(p => p.PlayerStatus == PlayerStatus.In);
                Prompt();
                return;
            }
            Player np = players.Find(p => p.PlayerStatus == PlayerStatus.In && players.IndexOf(p) > players.IndexOf(currentPlayer));
            if (np!=null) // Still a player in this round
            {
                currentPlayer = np;
                Prompt();
                return;
            }
            List<Player> remainingPlayers = players.FindAll(p => p.PlayerStatus == PlayerStatus.In);
            if (Round == Rule.Bet && !remainingPlayers.All(i=>i.CurrentBet==CurrentMaxBet)) // Betting incomplete
            {
                CheckOnly = true;
                currentPlayer = players.Find(p => p.PlayerStatus == PlayerStatus.In);
                Prompt();
                return;
            }
            if (currentRound<theRounds.Count-1 && theRounds[currentRound+1] != Rule.Showdown) // Still rounds to perform
            {
                CheckOnly = false;
                currentRound++;
                currentPlayer = players.Find(p => p.PlayerStatus == PlayerStatus.In);
                Prompt();
                return;
            }
            if (theRounds[currentRound + 1] == Rule.Showdown || Round== Rule.Showdown) // This is it.
            {
                List<(List<Card> cards, Player player)> remainingHands = new List<(List<Card>, Player)>();
                foreach (Player p in remainingPlayers)
                {
                    List<Card> hand = new List<Card>(p.Hand);
                    if (commonCards!=null && commonCards?.Count>0)
                    {
                        hand.AddRange(commonCards);
                    }
                    remainingHands.Add((hand, p));
                }
                Player winner = Card.Showdown(remainingHands);
                if (winner == null)
                {
                    remainingPlayers.ForEach(o => o.Gain(pot / remainingPlayers.Count));
                    GameControler.DisplayDraw();
                }
                else
                {
                    GameControler.DisplayWinningHand(remainingHands.Find(cp=>cp.player==winner).cards);
                    Victory(winner);
                }
            }
            CheckOnly = false;
            if (!gameOver) NewDeal();
        }
        private void Victory(Player player)
        {
            if (gameOver) return;
            player.Gain(pot);
            GameControler.DisplayDealWon(player.Name, pot);
            if (OtherPlayers.All(p=>p.Money<=0 || p.PlayerStatus==PlayerStatus.OutGame))
            {
                Winner = player;
                GameControler.GameOver();
            }
        }
        #endregion
    }
}