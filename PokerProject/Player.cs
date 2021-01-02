using System.Collections.Generic;

namespace PokerProject
{
    /// <summary>
    /// Types of player: human / AI.
    /// </summary>
    public enum PlayerType
    {
        Human,
        AI
    }
    /// <summary>
    /// Types of player status: in the deal, out for this deal, out of the game.
    /// </summary>
    public enum PlayerStatus
    {
        In,
        OutDeal,
        OutGame
    }
    /// <summary>
    /// Class player: represent a player, their hand, status, money.
    /// </summary>
    public class Player
    {
        #region attributes and encapsulated properties
        private string name;
        private readonly List<Card> cards=new List<Card>();
        private int money;
        private readonly PlayerType playerType;
        /// <summary>
        /// The name of the player.
        /// </summary>
        public string Name => name;
        /// <summary>
        /// The amount of money (chips) left.
        /// </summary>
        public int Money => money;
        /// <summary>
        /// AI or Human.
        /// </summary>
        public PlayerType PlayerType => playerType;
        /// <summary>
        /// In, Out of the deal, or Out of the game.
        /// </summary>
        public PlayerStatus PlayerStatus { get; internal set; } = PlayerStatus.In;
        /// <summary>
        /// The current bet placed on this deal by this player.
        /// </summary>
        public int CurrentBet { get; internal set; } = 0;
        /// <summary>
        /// The list of cards presently received by this player.
        /// </summary>
        public List<Card> Hand => cards;
        #endregion
        #region constructor
        /// <summary>
        /// Standard parameter'd constructor.
        /// </summar
        /// <param name="playerType">AI or Human.</param>
        /// <param name="money">The number of chips the player starts with.</param>
        public Player(PlayerType playerType, int money)
        {
            this.playerType = playerType;
            this.money = money;
        }
        #endregion
        #region public methods
        /// <summary>
        /// Sets the name of the player after creation.
        /// </summary>
        /// <param name="name">The new name of the player.</param>
        public void SetName(string name) => this.name = name;
        /// <summary>
        /// Spends a positive amount of chips and indicates whether the player is still in the game.
        /// </summary>
        /// <param name="amount">The amount to spend (must be positive).</param>
        /// <returns>True iff the player is still in the game (has still some money left).</returns>
        public bool Spend(int amount)
        {
            if (amount>0)
            {
                money -= amount;
            }
            return money > 0;
        }
        /// <summary>
        /// Gains a positive amount of chips/
        /// </summary>
        /// <param name="amount">The amount to gain (must be positive).</param>
        public void Gain(int amount)
        {
            if (amount>0)
            {
                money += amount;
            }
        }
        /// <summary>
        /// Adds a new card to the hand of the player.
        /// </summary>
        /// <param name="c">The card to receive.</param>
        public void AddCard(Card c)
        {
            cards.Add(c);
        }
        /// <summary>
        /// Scans the hand of the player for any discarded card, and remove them from the hand.
        /// </summary>
        public void RemoveDiscards()
        {
            cards.RemoveAll(c => c.Discard);
        }
        #endregion
    }
}