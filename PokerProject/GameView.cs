using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.Remoting.Messaging;
using System.Windows.Forms;

namespace PokerProject
{
    /// <summary>
    /// A simple game view. WinForms, but Console-like :
    /// one textbox for command input,
    /// one listbox to provide text feedback.
    /// </summary>
    public partial class GameView : Form
    {
        private bool movePlayerView;
        private PlayerView selected;
        private PlayerView currentPlayer;
        private CardView selectedCard = null;
        List<PlayerView> playerViews = new List<PlayerView>();
        #region private attributes
        /// <summary>
        /// Not used at the start of the project, may be used for
        /// accessing public information ONLY.
        /// </summary>
        private readonly RuleSet theRules;
        #endregion
        #region constructor
        /// <summary>
        /// Standard parameter'd constructor.
        /// </summary>
        /// <param name="theRules">A reference to the RuleSet object used.</param>
        public GameView(RuleSet theRules)
        {
            InitializeComponent();
            roundLabel.Text = "";
            this.theRules = theRules;
            int translate = 0;
            foreach (Player p in theRules.AllPlayers)
            {
                PlayerView pv = new PlayerView
                {
                    player = p,
                    coordinate = new Point(5 + translate, 5),
                    color = (p.PlayerType == PlayerType.Human) ? Color.Red : Color.Black,
                    surface = new Size(200, 150)
                };
                playerViews.Add(pv);
                translate += pv.surface.Width + 5;
            }
            movePlayerView = false;
        }
        #endregion
        #region public methods callable by the controller
        /// <summary>
        /// 1 - Example of a different display
        /// </summary>
        /// <param name="text">To be displayed</param>
        public void ChangeRoundLabel(string text) => roundLabel.Text = text;
        /// <summary>
        /// Method used for the text-based interface in order to display information.
        /// </summary>
        /// <param name="line">The line of text to be displayed.</param>
        public void WriteLine(string line)
        {
            viewBox.Items.Add(line);
            viewBox.SelectedIndex = viewBox.Items.Count - 1;
        }
        #endregion

        private void drawingArea_Paint(object sender, PaintEventArgs e)
        {
            checkCurrentPlayer();
            playerViews.ForEach(pv => pv.draw(e.Graphics, currentPlayer));
            PlayerView.currentPlayer = theRules.PlayerName;
            roundLabel.Text = "Game : \n\n" + theRules.GameName;
            label1.Text = "Round : \n\n" + theRules.Deal;
            label2.Text = "State : \n\n" + theRules.Round.ToString();
            label3.Text = "Pot = \n\n" + theRules.Pot + " $";
            label4.ForeColor = (theRules.Winner != null) ? Color.DarkGreen : Color.DarkRed;
            label4.Text = (theRules.Winner != null) ? "Winner : \n" + theRules.Winner.Name : "Game not finished !";
        }

        private PlayerView selectedPv(Point p)
        {
            return playerViews.FirstOrDefault(pv => pv.contains(p));
        }

        private void checkCurrentPlayer()
        {

            foreach (PlayerView pv in playerViews)
            {
                if (pv.player.Name.Equals(theRules.PlayerName))
                {
                    currentPlayer = pv;
                    break;
                }
            }
        }

        private CardView selectedCDV(Point p)
        {
            CardView cardToDiscard = null;
            checkCurrentPlayer();
            bool playerFind = currentPlayer != null;
            if (playerFind)
            {
                foreach (CardView cv in currentPlayer.myCards)
                {
                    if (cv.contains(p))
                    {
                        selectedCard = cardToDiscard = cv;
                    }
                }
            }
            return cardToDiscard;
        }

        private void drawingArea_MouseDown(object sender, MouseEventArgs e)
        {
            if (selectedPv(e.Location) != null)
            {
                selected = selectedPv(e.Location);
                if (e.Button == MouseButtons.Left)
                {
                    movePlayerView = true;
                }
                else if (selected.player.Name != theRules.PlayerName)
                {
                    contextMenuStrip1.Show(new Point(e.Location.X, e.Location.Y + 147));
                }
            }
            else if (selectedCDV(e.Location) != null && theRules.GameName.Equals("five-card draw"))
            {
                contextMenuStrip2.Show(new Point(e.Location.X, e.Location.Y + 170));
            }
            else
            {
                automatic();
            }
        }

        private void automatic()
        {
            if (theRules.Round.ToString().Equals("Ante"))
            {
                GameControler.Interpret("ante");
            }
            else if (theRules.Round.ToString().Equals("DealCardDown") || theRules.Round.ToString().Equals("DealCardUp"))
            {
                GameControler.Interpret("next");
            }
        }
        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            selected.drawAll = !selected.drawAll;
            Refresh();
        }

        private void drawingArea_MouseMove(object sender, MouseEventArgs e)
        {
            if (movePlayerView)
            {
                selected.move(e.Location);
                Refresh();
            }
        }

        private void drawingArea_MouseUp(object sender, MouseEventArgs e)
        {
            if (movePlayerView)
            {
                movePlayerView = false;
                selected.move(e.Location);
                Refresh();
                selected = null;
            }
        }

        private void ante_Click(object sender, System.EventArgs e)
        {
            GameControler.Interpret("ante");
        }

        private void next_Click(object sender, System.EventArgs e)
        {
            GameControler.Interpret("next");
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            int coordX = 0;
            foreach (Card crd in theRules.CommonCards)
            {
                new CardView
                {
                    card = crd,
                    size = new Size(60, 90),
                    p = new Point(coordX, 5)
                }.draw(e.Graphics, true, true);
                coordX += 70;
            }
        }

        private void button1_Click(object sender, System.EventArgs e)
        {
            System.Environment.Exit(0);
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            foreach (PlayerView pv in playerViews)
            {
                if (pv.player.Name.Equals(theRules.PlayerName))
                {
                    stoleMoney(10);
                }
            }
        }

        private void stoleMoney(int moneyToStole)
        {
            foreach (PlayerView pv in playerViews)
            {
                if (pv.player.Name.Equals(theRules.PlayerName) && selected.player.Money - moneyToStole >= 0)
                {
                    selected.player.Spend(moneyToStole);
                    pv.player.Gain(moneyToStole);
                }
            }
            Refresh();
        }

        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            stoleMoney(50);
        }

        private void toolStripMenuItem5_Click(object sender, EventArgs e)
        {
            stoleMoney(100);
        }

        private void toolStripMenuItem6_Click(object sender, EventArgs e)
        {
            stoleMoney(1000);
        }

        private void toolStripMenuItem7_Click(object sender, EventArgs e)
        {
            stoleMoney(500);
        }

        private void toolStripMenuItem8_Click(object sender, EventArgs e)
        {
            stoleMoney(999);
        }

        private void discardCardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GameControler.Interpret("discard " + selectedCard.code());
            Refresh();
        }

        private void check_Click(object sender, EventArgs e)
        {
            GameControler.Interpret("check");
        }

        private void status_Click(object sender, EventArgs e)
        {
            GameControler.Interpret("status");
        }
    }
}