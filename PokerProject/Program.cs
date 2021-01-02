using System;
using System.Windows.Forms;

namespace PokerProject
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            RuleSet theRules ;
            string fileName="";

            QuestionCustom qs = new QuestionCustom();
            if (qs.ShowDialog() == DialogResult.Yes)
            {
                OpenFileDialog changeRules = new OpenFileDialog
                {
                    Filter = "Fichier xml|*.xml",
                    Title = "Choose a File",
                    InitialDirectory = "../"
                };
                if (changeRules.ShowDialog() == DialogResult.OK)
                {
                    fileName = changeRules.FileName;
                }
                else
                {
                    fileName = "../../rules.xml";
                }
            }
            else
            {
                fileName = "../../rules.xml";
            }


            theRules = new RuleSet(fileName);

            // 2 - SetupView

            ExampleSetupView setupView = new ExampleSetupView(theRules);
            setupView.TopMost=true;
            if(setupView.ShowDialog() != DialogResult.OK)
            {
                return;
            }
             
            GameView theView = new GameView(theRules);
            GameControler.Initialize(theRules, theView);
 
            Application.Run(theView);
        }
    }
}
