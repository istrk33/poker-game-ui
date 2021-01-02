using System.Windows.Forms;

namespace PokerProject
{
    /// <summary>
    /// Example of a possible dialog for placing bets.
    /// </summary>
    public partial class ExampleBetView : Form
    {
        #region public property
        ///<summary>
        /// The amount selected in the NumericUpDown betAmount.
        /// </summary>
        public int BetAmount => (int)betAmount.Value;
        #endregion
        #region Constructor
        /// <summary>
        /// Standard parameter'd constructor.
        /// </summary>
        /// <param name="currentMinBettable">Minimum amount bettable.</param>
        /// <param name="playerMoney">Maximum amount bettable outside all and check.</param>
        /// <param name="checkOnly">True implies only check and fold are allowed.</param>
        public ExampleBetView(int currentMinBettable, int playerMoney, bool checkOnly)
        {
            InitializeComponent();
            if (checkOnly)
            {
                betAllButton.Enabled = betAmounButton.Enabled = false;
            }
            else
            {
                betAmount.Minimum = currentMinBettable;
                betAmount.Maximum = playerMoney;
            }
        }
        #endregion
    }
}
