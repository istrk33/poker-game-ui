using System;
using System.Collections.Generic;
using System.Linq;

namespace PokerProject
{
    /// <summary>
    /// Rules - different types of rounds
    /// </summary>
    public enum Rule
    {
        DealCardDown,
        DealCardUp,
        DealCardCommon,
        MayDrawDown,
        MayDrawUp,
        Ante,
        Bet,
        Showdown
    }
    /// <summary>
    /// Faces of playing cards
    /// </summary>
    public enum Face
    {
        Two,
        Three,
        Four,
        Five,
        Six,
        Seven,
        Eight,
        Nine,
        Ten,
        Jack,
        Queen,
        King,
        Ace
    }
    /// <summary>
    /// Suits of playing cards
    /// </summary>
    public enum Suit
    {
        Clubs,
        Diamonds,
        Hearts,
        Spades
    }
    /// <summary>
    /// The possible "scores" in Poker
    /// </summary>
    public enum Hand
    {
        HighCard,
        OnePair,
        TwoPairs,
        ThreeOfAKind,
        Straight,
        Flush,
        FullHouse,
        FourOfAKind,
        StraightFlush
    }
    /// <summary>
    /// Class Card. Objects of this class are playing cards, static methods allow to check for specific combinations.
    /// </summary>
    public class Card : IComparable
    {
        #region private attributes
        private readonly Face face;
        private readonly Suit suit;
        #endregion
        #region public properties
        /// <summary>
        /// The face of one card (2-Ace).
        /// </summary>
        public Face Face { get { return face; } }
        /// <summary>
        /// The suit of one card (Clubs, Diamonds, Hearts, Spades).
        /// </summary>
        public Suit Suit { get { return suit; } }
        /// <summary>
        /// The fact that the card is dealt down (visible to the player) : true, or
        /// up (visible to all) : false.
        /// </summary>
        public bool Down { get; internal set; } = false;
        /// <summary>
        /// The fact that the card has presently been discarded by the player.
        /// </summary>
        public bool Discard { get; internal set; } = false;

        #endregion
        #region Constructor
        /// <summary>
        /// Creates a new card. Simple parameter'd constructor.
        /// </summary>
        /// <param name="face">The face.</param>
        /// <param name="suit">The suit.</param>
        public Card(Face face, Suit suit)
        {
            this.face = face;
            this.suit = suit;
        }
        #endregion
        #region Overriden public methods
        /// <summary>
        /// Used explicitly or when converting this object to text.
        /// Inherited from object.
        /// The string representation is based on the face and suit.
        /// </summary>
        /// <returns>The string representation of the card.</returns>
        public override string ToString()
        {
            string name = "";
            switch (face)
            {
                case Face.Two: name += "Two (2)"; break;
                case Face.Three: name += "Three (3)"; break;
                case Face.Four: name += "Four (4)"; break;
                case Face.Five: name += "Five (5)"; break;
                case Face.Six: name += "Six (6)"; break;
                case Face.Seven: name += "Seven (7)"; break;
                case Face.Eight: name += "Eight (8)"; break;
                case Face.Nine: name += "Nine (9)"; break;
                case Face.Ten: name += "Ten (t)"; break;
                case Face.Jack: name += "Jack (j)"; break;
                case Face.Queen: name += "Queen (q)"; break;
                case Face.King: name += "King (k)"; break;
                case Face.Ace: name += "Ace (a)"; break;
            }
            name += " of ";
            switch (suit)
            {
                case Suit.Clubs: name += "Clubs (c)"; break;
                case Suit.Diamonds: name += "Diamonds (d)"; break;
                case Suit.Hearts: name += "Hearts (h)"; break;
                case Suit.Spades: name += "Spades (s)"; break;
            }
            return name;
        }
        /// <summary>
        /// Equality method.
        /// Inherited from object.
        /// This override indicates that different copies of a card with the
        /// same face and suit are identical.
        /// </summary>
        /// <param name="o">The object to compare the card to.</param>
        /// <returns>True iff o is the same card as this.</returns>
        public override bool Equals(object o)
        {
            if (o == null || (!(o is Card))) return false;
            return (o as Card).Suit == Suit && (o as Card).Face == Face;
        }
        /// <summary>
        /// Necessary in order to override Equals.
        /// Inherited from object, unchanged.
        /// </summary>
        /// <returns>The hash for the base Card object.</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        /// <summary>
        /// Comparison method.
        /// Implementing the IComparable interface.
        /// Necessary in order to know which card beats which.
        /// Faces are compared, suits are not relevant.
        /// A return value of 0 does *not* entail that the cards are equal.
        /// </summary>
        /// <param name="o">The object to compare to.</param>
        /// <returns>Negative if weaker, 0 if same face value, positive if stronger or incomparable.</returns>
        public int CompareTo(object o)
        {
            if (o == null) return 1;
            if (!(o is Card)) return 1;
            return face - (o as Card).face;
        }
        #endregion
        #region static methods for detecting combinations
        /// <summary>
        /// Detects whether a given hand contains a straight flush.
        /// </summary>
        /// <param name="hand">The list of cards to test.</param>
        /// <returns>(true, list) if the hand contains list as a straight flush, (false, null) otherwise.</returns>
        public static (bool res, List<Card> straightFlush) ContainsStraightFlush(List<Card> hand)
        {
            List<Card> straight = ContainsStraight(hand).straight;
            List<Card> flush = ContainsFlush(hand).flush;
            if (straight != null && flush != null  && straight.All(c => flush.Contains(c)))
            {
                return (true, straight);
            }
            else
            {
                return (false, null);
            }
        }
        /// <summary>
        /// Detects whether a given hand contains four cards of a kind.
        /// </summary>
        /// <param name="groupedHand">The list of cards to test, grouped by kind.</param>
        /// <returns>(true, list) if the hand contains list as four cards of a kind, (false, null) otherwise.</returns>
        public static (bool res, List<Card> fourOfAKind) ContainsFourOfAKind(List<List<Card>> groupedHand)
        {
            List<Card> four = groupedHand.Find(l => l.Count == 4);
            return (four != null, four);
        }
        /// <summary>
        /// Detects whether a given hand contains a full house.
        /// </summary>
        /// <param name="groupedHand">The list of cards to test, grouped by kind.</param>
        /// <returns>(true, list) if the hand contains a full house with list as the three cards of a kind, (false, null) otherwise.</returns>
        public static (bool res, List<Card> threeInFull) ContainsFullHouse(List<List<Card>> groupedHand)
        {
            List<Card> three = ContainsThreeOfAKind(groupedHand).threeOfAKind;
            List<Card> two = groupedHand.Find(l => l.Count == 2);
            return (three != null && two != null, three);
        }
        /// <summary>
        /// Detects whether a given hand contains a flush.
        /// </summary>
        /// <param name="hand">The list of cards to test.</param>
        /// <returns>(true, list) if the hand contains list as a flush, (false, null) otherwise.</returns>
        public static (bool res, List<Card> flush) ContainsFlush(List<Card> hand)
        {
            var handBySuits = hand.GroupBy(c => c.Suit);
            List<List<Card>> suitedHand = new List<List<Card>>();
            handBySuits.ToList().ForEach(g => suitedHand.Add(g.ToList()));
            return (suitedHand.Exists(g => g.Count >= 5), suitedHand.Find(g => g.Count > 5));
        }
        /// <summary>
        /// Detects whether a given hand contains a straight.
        /// </summary>
        /// <param name="hand">The list of cards to test.</param>
        /// <returns>(true, list) if the hand contains list as a straight, (false, null) otherwise.</returns>
        public static (bool res, List<Card> straight) ContainsStraight(List<Card> hand)
        {
            var orderedHand = new List<Card>(hand);
            orderedHand.Sort();
            int i = 1;
            int start = 0;
            while (i + start < orderedHand.Count)
            {
                if (orderedHand[start + i].face == orderedHand[start + i - 1].face + 1)
                {
                    i++;
                }
                else
                {
                    start++;
                }
            }
            if (i >= 5)
            {
                return (true, orderedHand.GetRange(start, 5));
            }
            else
            {
                return (false, null);
            }
        }
        /// <summary>
        /// Detects whether a given hand contains three cards of a kind.
        /// </summary>
        /// <param name="groupedHand">The list of cards to test, grouped by kind.</param>
        /// <returns>(true, list) if the hand contains list as three cards of a kind, (false, null) otherwise.</returns>
        public static (bool res, List<Card> threeOfAKind) ContainsThreeOfAKind(List<List<Card>> groupedHand)
        {
            List<Card> three = groupedHand.Find(l => l.Count == 3);
            return (three != null, three);
        }
        /// <summary>
        /// Detects whether a given hand contains two pairs.
        /// </summary>
        /// <param name="groupedHand">The list of cards to test, grouped by kind.</param>
        /// <returns>(true, list) if the hand contains list the two pairs (grouped by kind), (false, null) otherwise.</returns>
        public static (bool res, List<List<Card>> twoPairs) ContainsTwoPairs(List<List<Card>> groupedHand)
        {
            List<Card> onePair = ContainsPair(groupedHand).pair;
            if (onePair != null)
            {
                var pairRemoved = groupedHand.FindAll(l => !l.All(c => onePair.Contains(c)));
                List<Card> otherPair = ContainsPair(pairRemoved).pair;
                return (otherPair != null, otherPair != null ?new List<List<Card>>{ onePair, otherPair}:null);
            }
            return (false, null);
        }
        /// <summary>
        /// Detects whether a given hand contains one pair.
        /// </summary>
        /// <param name="groupedHand">The list of cards to test, grouped by kind.</param>
        /// <returns>(true, list) if the hand contains list as a pair, (false, null) otherwise.</returns>
        public static (bool res, List<Card> pair) ContainsPair(List<List<Card>> groupedHand)
        {
            List<Card> pair = groupedHand.Find(l => l.Count == 2);
            return (pair != null, pair);
        }
        /// <summary>
        /// Gets the highest card in a hand of cards.
        /// </summary>
        /// <param name="hand">The list of cards to test, must not be null or empty.</param>
        /// <returns>The highest card in the list.</returns>
        public static Card GetHighCard(List<Card> hand)
        {
            var sortedHand = new List<Card>(hand);
            sortedHand.Sort();
            return sortedHand[hand.Count - 1];
        }
        /// <summary>
        /// Tests all possible combinations to get the best possible one in a list of card.
        /// </summary>
        /// <param name="hand">The list of cards to check.</param>
        /// <returns>(H, C) where H (label : theBest) is the best score possible and C (label : hand) is the list of cards that made that combination.</returns>
        public static (Hand theBest, List<Card> hand) GetBestHand(List<Card> hand)
        {
            var handByFaces = hand.GroupBy(c => c.Face);
            List<List<Card>> groupedHand = new List<List<Card>>();
            handByFaces.ToList().ForEach(g => groupedHand.Add(g.ToList()));
            var tentativeStraightFlush = ContainsStraightFlush(hand);
            if (tentativeStraightFlush.res) return (Hand.StraightFlush, tentativeStraightFlush.straightFlush);
            var tentativeFourOfAKind = ContainsFourOfAKind(groupedHand);
            if (tentativeFourOfAKind.res) return (Hand.FourOfAKind, tentativeFourOfAKind.fourOfAKind);
            var tentativeFullHouse = ContainsFullHouse(groupedHand);
            if (tentativeFullHouse.res) return (Hand.FullHouse, tentativeFullHouse.threeInFull);
            var tentativeFlush = ContainsFlush(hand);
            if (tentativeFlush.res) return (Hand.Flush, tentativeFlush.flush);
            var tentativeStraight = ContainsStraight(hand);
            if (tentativeStraight.res) return (Hand.Straight, tentativeStraight.straight);
            var tentativeThreeOfAKind = ContainsThreeOfAKind(groupedHand);
            if (tentativeThreeOfAKind.res) return (Hand.ThreeOfAKind, tentativeThreeOfAKind.threeOfAKind);
            var tentativeTwoPairs = ContainsTwoPairs(groupedHand);
            List<Card> theTwoPairs = new List<Card>();
            tentativeTwoPairs.twoPairs?.ForEach(l => theTwoPairs.AddRange(l));
            if (tentativeTwoPairs.res) return (Hand.TwoPairs, theTwoPairs);
            var tentativePair = ContainsPair(groupedHand);
            if (tentativePair.res) return (Hand.OnePair, tentativePair.pair);
            if (hand.Count > 0) return (Hand.HighCard, new List<Card>{GetHighCard(hand)});
            return (Hand.HighCard, null);
        }
        /// <summary>
        /// Computes which player has won a showdown.
        /// </summary>
        /// <param name="theHands">List of poker hands to compare paired with the associated players.</param>
        /// <returns>The winning player, null if draw (improbable but possible with identical flushes or straights, common in Texas Hold'em.).</returns>
        public static Player Showdown(List<(List<Card> cards, Player p)> theHands)
        {
            if (theHands == null) return null;
            if (theHands.Count == 0) return null;
            if (theHands.Count == 1) return theHands.First().p;
            // After this: more than one player is in.
            List<(Hand hand, List<Card> cards)> bestHands = theHands.Select(h => GetBestHand(h.cards)).ToList();
            List<(Player p, Hand hand, List<Card> cards)> winners = new List<(Player, Hand, List<Card>)>
                { (theHands.First().p, GetBestHand(theHands.First().cards).theBest, theHands.First().cards) };
            Hand best = bestHands[0].hand;
            for (int i=1; i<bestHands.Count; i++)
            {
                if (bestHands[i].hand>best)
                {
                    best = bestHands[i].hand;
                    winners.Clear();
                    winners.Add((theHands.Find(cp => cp.cards.All(c => bestHands[i].cards.Contains(c))).p,
                        best, bestHands[i].cards));
                }
                else if (bestHands[i].hand==best)
                {
                    winners.Add((theHands.Find(cp => cp.cards.All(c => bestHands[i].cards.Contains(c))).p,
                        best, bestHands[i].cards));
                }
            }
            if (winners.Count == 1) return winners[0].p;
            // After this: more than one player has the same type of winning hand.
            List<(Player p, List<Card> cards)> tieBreakers = new List<(Player, List<Card>)>();
            winners.ForEach(w => tieBreakers.Add((w.p, w.cards)));
            do
            {
                Card bestCard = GetHighCard(tieBreakers[0].cards);
                foreach ((Player _, List<Card> cards) in tieBreakers)
                {
                    if (GetHighCard(cards).CompareTo(bestCard) > 0)
                    {
                        bestCard = GetHighCard(cards);
                    }
                }
                if (tieBreakers.FindAll(tb => GetHighCard(tb.cards) == bestCard).Count == 1) return tieBreakers[0].p;
                else
                {
                    winners.RemoveAll(w => tieBreakers.Exists(tb => tb.p == w.p && GetHighCard(tb.cards) != bestCard));
                    tieBreakers.RemoveAll(tb => GetHighCard(tb.cards) != bestCard);
                    tieBreakers.ForEach(tb => tb.cards.RemoveAll(c => c == bestCard));
                    tieBreakers.RemoveAll(tb => tb.cards.Count == 0);
                }
            } while (tieBreakers.FindAll(tb => tb.cards.Count > 0).Count > 0);
            // After this: could not find a tiebreaker in the best hands.
            tieBreakers.Clear();
            winners.ForEach(w => tieBreakers.Add((w.p, w.p.Hand)));
            winners.ForEach(w => tieBreakers.Find(tb => tb.p == w.p).cards.RemoveAll(c => w.cards.Contains(c)));
            do
            {
                Card bestCard = GetHighCard(tieBreakers[0].cards);
                foreach ((Player _, List<Card> cards) in tieBreakers)
                {
                    if (GetHighCard(cards).CompareTo(bestCard) > 0)
                    {
                        bestCard = GetHighCard(cards);
                    }
                }
                if (tieBreakers.FindAll(tb => GetHighCard(tb.cards) == bestCard).Count == 1) return tieBreakers[0].p;
                else
                {
                    winners.RemoveAll(w => tieBreakers.Exists(tb => tb.p == w.p && GetHighCard(tb.cards) != bestCard));
                    tieBreakers.RemoveAll(tb => GetHighCard(tb.cards) != bestCard);
                    tieBreakers.ForEach(tb => tb.cards.RemoveAll(c => c == bestCard));
                    tieBreakers.RemoveAll(tb => tb.cards.Count == 0);
                }
            } while (tieBreakers.FindAll(tb => tb.cards.Count > 0).Count > 0);
            // After this: no tiebreakers.
            return null;
        }
        #endregion
        #region public static methods for card handling
        /// <summary>
        /// Returns a face type from a string, null if not computable
        /// </summary>
        /// <param name="str">The string to parse.</param>
        /// <returns>The face or null.</returns>
        public static Face? ParseFace(string str)
        {
            switch(str.Trim().ToLower())
            {
                case "2": return Face.Two;
                case "3": return Face.Three;
                case "4": return Face.Four;
                case "5": return Face.Five;
                case "6": return Face.Six;
                case "7": return Face.Seven;
                case "8": return Face.Eight;
                case "9": return Face.Nine;
                case "t": return Face.Ten;
                case "j": return Face.Jack;
                case "q": return Face.Queen;
                case "k": return Face.King;
                case "a": return Face.Ace;
                default: return null;
            }
        }
        /// <summary>
        /// Returns a suit type from a string, null if not computable
        /// </summary>
        /// <param name="str">The string to parse.</param>
        /// <returns>The suit or null.</returns>
        public static Suit? ParseSuit(string str)
        {
            switch(str.Trim().ToLower())
            {
                case "c": return Suit.Clubs;
                case "d": return Suit.Diamonds;
                case "h": return Suit.Hearts;
                case "s": return Suit.Spades;
                default: return null;
            }
        }
        /// <summary>
        /// Gets the string representation of a card from a two-character string abbreviation.
        /// </summary>
        /// <param name="twoLetters">The string to parse.</param>
        /// <returns>The string or "" if not computable.</returns>
        public static string ParseTwoLettersToString(string twoLetters)
        {
            string res = "";
            Face? maybeF = ParseFace(twoLetters.ElementAtOrDefault(0).ToString());
            Suit? maybeS = ParseSuit(twoLetters.ElementAtOrDefault(1).ToString());
            if (maybeF.HasValue && maybeS.HasValue)
            {
                res = new Card(maybeF.Value, maybeS.Value).ToString();
            }
            return res;
        }
        /// <summary>
        /// A ToString method for the Hand type (poker scores).
        /// </summary>
        /// <param name="hand">The hand type.</param>
        /// <returns>The string representation.</returns>
        public static string HandValue(Hand hand)
        {
            switch (hand)
            {
                case Hand.StraightFlush: return "straight flush";
                case Hand.FourOfAKind: return "four of a kind";
                case Hand.FullHouse: return "full house";
                case Hand.Flush: return "flush";
                case Hand.Straight: return "straight";
                case Hand.ThreeOfAKind: return "three of a kind";
                case Hand.TwoPairs: return "two pairs";
                case Hand.OnePair: return "one pair";
                case Hand.HighCard: return "high card";
            }
            return "";
        }
        /// <summary>
        /// A method to build a full (ordered) deck of 52 cards.
        /// </summary>
        public static List<Card> Deck { get
            {
                return new List<Card>
                {
                    new Card(Face.Two, Suit.Clubs),
                    new Card(Face.Three, Suit.Clubs),
                    new Card(Face.Four, Suit.Clubs),
                    new Card(Face.Five, Suit.Clubs),
                    new Card(Face.Six, Suit.Clubs),
                    new Card(Face.Seven, Suit.Clubs),
                    new Card(Face.Eight, Suit.Clubs),
                    new Card(Face.Nine, Suit.Clubs),
                    new Card(Face.Ten, Suit.Clubs),
                    new Card(Face.Jack, Suit.Clubs),
                    new Card(Face.Queen, Suit.Clubs),
                    new Card(Face.King, Suit.Clubs),
                    new Card(Face.Ace, Suit.Clubs),
                    new Card(Face.Two, Suit.Diamonds),
                    new Card(Face.Three, Suit.Diamonds),
                    new Card(Face.Four, Suit.Diamonds),
                    new Card(Face.Five, Suit.Diamonds),
                    new Card(Face.Six, Suit.Diamonds),
                    new Card(Face.Seven, Suit.Diamonds),
                    new Card(Face.Eight, Suit.Diamonds),
                    new Card(Face.Nine, Suit.Diamonds),
                    new Card(Face.Ten, Suit.Diamonds),
                    new Card(Face.Jack, Suit.Diamonds),
                    new Card(Face.Queen, Suit.Diamonds),
                    new Card(Face.King, Suit.Diamonds),
                    new Card(Face.Ace, Suit.Diamonds),
                    new Card(Face.Two, Suit.Hearts),
                    new Card(Face.Three, Suit.Hearts),
                    new Card(Face.Four, Suit.Hearts),
                    new Card(Face.Five, Suit.Hearts),
                    new Card(Face.Six, Suit.Hearts),
                    new Card(Face.Seven, Suit.Hearts),
                    new Card(Face.Eight, Suit.Hearts),
                    new Card(Face.Nine, Suit.Hearts),
                    new Card(Face.Ten, Suit.Hearts),
                    new Card(Face.Jack, Suit.Hearts),
                    new Card(Face.Queen, Suit.Hearts),
                    new Card(Face.King, Suit.Hearts),
                    new Card(Face.Ace, Suit.Hearts),
                    new Card(Face.Two, Suit.Spades),
                    new Card(Face.Three, Suit.Spades),
                    new Card(Face.Four, Suit.Spades),
                    new Card(Face.Five, Suit.Spades),
                    new Card(Face.Six, Suit.Spades),
                    new Card(Face.Seven, Suit.Spades),
                    new Card(Face.Eight, Suit.Spades),
                    new Card(Face.Nine, Suit.Spades),
                    new Card(Face.Ten, Suit.Spades),
                    new Card(Face.Jack, Suit.Spades),
                    new Card(Face.Queen, Suit.Spades),
                    new Card(Face.King, Suit.Spades),
                    new Card(Face.Ace, Suit.Spades)
                };
            } }
        #endregion
    }
}