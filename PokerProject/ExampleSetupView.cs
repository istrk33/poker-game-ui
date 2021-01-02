using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace PokerProject
{
    /// <summary>
    /// Example of a possible dialog for setup.
    /// </summary>
    public partial class ExampleSetupView : Form
    {
        #region attributes 
        private static List<CheckBox> theCheckBoxes;
        private static List<TextBox> theTextBoxes;
        private readonly RuleSet theRules;
        #endregion
        #region Constructor
        /// <summary>
        /// Standard parameter'd constructor.
        /// </summary>
        /// <param name="theRules">Reference to the RuleSet object to use and initialize.</param>
        public ExampleSetupView(RuleSet theRules)
        {
            InitializeComponent();
            theCheckBoxes = new List<CheckBox> { checkBoxP1, checkBoxP2, checkBoxP3, checkBoxP4, checkBoxP5, checkBoxP6, checkBoxP7, checkBoxP8 };
            theTextBoxes = new List<TextBox> { textBox1, textBox2, textBox3, textBox4, textBox5, textBox6, textBox7, textBox8 };
            this.theRules = theRules;
            theRules.AvailableGames.ForEach(r => listBoxChoice.Items.Add(r));
            int nb = (int)numericUpDownPlayers.Value;
            for (int i = 0; i < theCheckBoxes.Count; i++)
            {
                theCheckBoxes[i].Enabled = i < nb; // True for players selected, false for all others.
                theTextBoxes[i].Enabled = i < nb;
                theTextBoxes[i].BackColor = SystemColors.WindowFrame;
            }
        }
        #endregion
        #region Event handling private methods
        private void ListBoxChoice_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxChoice.SelectedItem!=null)
            {
                string choice = listBoxChoice.SelectedItem.ToString();
                theRules.SetGame(choice); // Necessary before the other information is inferred.

                numericUpDownPlayers.Minimum = theRules.MinPlayers;
                numericUpDownPlayers.Maximum = Math.Min(theRules.MaxPlayers, theCheckBoxes.Count);
                // Maximum cap'd by the actual number of controls.
            }
        }
        private void NumericUpDownPlayers_ValueChanged(object sender, EventArgs e)
        {
            if (listBoxChoice.SelectedItem.Equals("texas hold'em"))
            {
                numericUpDown1.Maximum = 8;
            }
            int nb = (int)numericUpDownPlayers.Value;

            for (int i = 0; i < theCheckBoxes.Count; i++)
            {
                theCheckBoxes[i].Enabled = i < nb; // True for players selected, false for all others.
                theTextBoxes[i].Enabled = i < nb;
                if (i >= nb)
                {
                    theTextBoxes[i].BackColor = SystemColors.WindowFrame;
                }
                else
                {
                    theTextBoxes[i].BackColor = Color.White;
                }
            }
        }
        private void ButtonQuit_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private bool GoodPlayerName()
        {
            bool goodPN = true;
            foreach(TextBox t in theTextBoxes )
            {
                if(t.Text.Length>10|| t.Text.Length == 0)
                {
                    goodPN = false;
                    return goodPN;
                }
            }
            return goodPN;
        }
        private void ButtonValidate_Click(object sender, EventArgs e)
        {
            if (GoodPlayerName()) {
                // nb players
                int nb = (int)numericUpDownPlayers.Value;
                // types of players
                string types = "";
                // names of players
                string names = "";
                // ante and doubles
                string ante = "";
                string doubles = "";
                for (int i = 0; i < nb; i++)
                {
                    types += theCheckBoxes[i].Checked ? "H" : "A";
                    names += theTextBoxes[i].Text.Replace(" ","") + " ";
                }

                ante += numericUpDown1.Value;
                doubles += checkBox1.Checked ? "yes" : "no";

                // validate
                if (theRules.GameName == null || theRules.GameName.Equals(""))
                {
                    MessageBox.Show("Issue: Game Choice");
                }
                else if (!theRules.SetNumberOfPlayers(nb.ToString()))
                {
                    MessageBox.Show("Issue: Nb Players");
                }
                else if (!theRules.SetPlayerTypes(types.Trim()))
                {
                    MessageBox.Show("Issue: type of Players");
                }
                else if (!theRules.SetPlayerNames(names.Trim()))
                {
                    MessageBox.Show("Issue: names of Players");
                }
                else if (!theRules.SetAnte(ante))
                {
                    MessageBox.Show("Issue: ante");
                }
                else if (!theRules.SetAnteDoubles(doubles))
                {
                    MessageBox.Show("Issue: ante doubles");
                }
                else
                {
                    DialogResult = DialogResult.OK;
                }
            }
            else
            {
                label5.ForeColor = Color.LightCoral;
                label5.Text = "Player name lenght must be\ngreater than 0 and less or equal to 10";
            }
        }
        #endregion

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                checkBox1.Text = "Enabled";
                checkBox1.ForeColor = Color.PaleGreen;
            }
            else
            {
                checkBox1.Text = "Disabled";
                checkBox1.ForeColor = Color.LightCoral;
            }
        }
    }
}
