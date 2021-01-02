using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerProject
{
    class PlayerView
    {
        public static string currentPlayer{get;set;}
        public bool drawAll { get; set; } = false;
        public Player player { get; set; }
        public Point coordinate { get; set; }
        public Color color { get; set; }
        public bool displayCards { get; set; }
        public Size surface { get; set; }
        private List<bool> tmp { get; set; } = new List<bool>();
        public List<CardView> myCards { get; set; } = new List<CardView>();
        public override string ToString()
        {
            string pt = (player.PlayerType == PlayerType.Human) ? "Human " : "AI ";
            string st = (player.PlayerStatus == PlayerStatus.In) ? "In " : (player.PlayerStatus == PlayerStatus.OutDeal) ? "OutDeal " : "OutGame ";
            return player.Name + ", " + pt + "\n" +
                st + ", " + player.Money + " $\n" +
                "Bet " + player.CurrentBet + " $";
        }

        public void draw(Graphics g,PlayerView pv)
        {
            Brush b;
            Pen p = new Pen(color, 1);
            b = (color == Color.Red) ? Brushes.LightCoral : Brushes.Gray;
            switch (player.PlayerStatus)
            {
                case PlayerStatus.In:
                    Rectangle r = new Rectangle(coordinate, surface);
                    g.FillRectangle(b, r);
                    g.DrawRectangle(p, r);
                    g.DrawString(ToString(), new Font("Times New Roman", 12, FontStyle.Regular), Brushes.Black, new Point(coordinate.X, coordinate.Y + 20));
                    int coordX = coordinate.X + surface.Width / 2 - (70 * (player.Hand.Count) / 2);
                    myCards.Clear();
                    foreach (Card crd in player.Hand)
                    {
                        myCards.Add(new CardView
                        {
                            card = crd,
                            size = new Size(60, 90),
                            p = new Point(coordX, coordinate.Y + surface.Height + 5)
                        });
                        coordX += 70;
                    }
                    if (pv.player.Name != currentPlayer)
                    {
                        drawAll = false;
                    }
                    foreach(CardView cd in myCards)
                    {
                        if ((pv.player.PlayerType == PlayerType.Human && player.Name.Equals(pv.player.Name))||drawAll)
                        {
                            cd.draw(g,true,true);
                        }
                        else
                        {
                            cd.draw(g,false,drawAll);
                        }
                    }
                    break;
                case PlayerStatus.OutDeal:
                    Rectangle e = new Rectangle(coordinate, surface);
                    g.FillEllipse(b, e);
                    g.DrawEllipse(p, e);
                    g.DrawString(ToString(), new Font("Times New Roman", 12, FontStyle.Regular), Brushes.Black, new Point(coordinate.X + 25, coordinate.Y + 20));
                    break;
                case PlayerStatus.OutGame:
                    Point[] coordinates = { new Point(coordinate.X + surface.Width / 2, coordinate.Y), new Point(coordinate.X + surface.Width, coordinate.Y + 25), new Point(coordinate.X + surface.Width - 25, coordinate.Y + 100), new Point(coordinate.X + 25, coordinate.Y + 100), new Point(coordinate.X, coordinate.Y + 25) };
                    g.FillPolygon(b, coordinates);
                    g.DrawPolygon(p, coordinates);
                    g.DrawString(ToString(), new Font("Times New Roman", 12, FontStyle.Regular), Brushes.Black, new Point(coordinate.X + 25, coordinate.Y + 25));
                    break;
            }
        }

        public bool contains(Point p)
        {
            Rectangle r = new Rectangle(coordinate, surface);
            return r.Contains(p);
        }

        public void move(Point p)
        {
            this.coordinate = new Point(p.X - surface.Width / 2, p.Y - surface.Height / 2);
        }
    }
}
