using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerProject
{
    class CardView
    {
        public Card card { get; set; }
        public Point p { get; set; }
        public Size size { get; set; }

        public string code()
        {
            return card.ToString().Split(' ')[1].Replace("(", "").Replace(")", "") + card.ToString().Split(' ')[4].Replace("(", "").Replace(")", "");
        }

        public void draw(Graphics g, bool currentHumanPlayer, bool visibleCard)
        {
            if (currentHumanPlayer || visibleCard || !card.Down)
            {
                g.DrawImage((Image)Properties.Resources.ResourceManager.GetObject(card.Suit.ToString() + card.Face.ToString()), new Rectangle(p, size));
            }
            else
            {
                g.DrawImage(Properties.Resources.blue_back, new Rectangle(p, size));
            }
        }

        public bool contains(Point pt)
        {
            Rectangle r = new Rectangle(p, size);
            return r.Contains(pt);
        }
    }
}