using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;
namespace SchetsEditor
{
    public class Element
    {
        public ISchetsTool tool;
        public Point p1;
        public Point p2;
        public Brush kleur;
        public string tekst;
        public SchetsControl s;
        public int dikte;
        //public List<Elem>

        public Element Get()
        {
            return this;
        }
        /* public Element(ISchetsTool tool, Point p1, Point p2, Brush kleur, bool tekenen)
         {
             this.tekenen = tekenen;
             this.tool = tool;
             this.p1 = p1;
             this.p2 = p2;
             this.kleur = kleur;
             //this.

         }*/
        public Element(ISchetsTool tool, Point p1, Point p2, Brush kleur, string tekst, int dikte)
        {

            this.tool = tool;
            this.p1 = p1;
            this.p2 = p2;
            this.kleur = kleur;
            this.dikte = dikte;
            if (tekst != null)
            {
                this.tekst = tekst;
            }
        }
        public static Rectangle Punten2Rechthoek(Point p1, Point p2)
        {
            return new Rectangle(new Point(Math.Min(p1.X, p2.X), Math.Min(p1.Y, p2.Y))
                                , new Size(Math.Abs(p1.X - p2.X), Math.Abs(p1.Y - p2.Y))
                                );
        }
        public static Pen MaakPen(Brush b, int dikte)
        {
            Pen pen = new Pen(b, dikte);
            pen.StartCap = LineCap.Round;
            pen.EndCap = LineCap.Round;
            return pen;
        }
        public void teken(Object obj, Graphics g, Point p1, Point p2)
        {
            if (tool.GetType() == typeof(PenTool))
            {
                g.DrawLine(MaakPen(kleur, dikte), p1, p2);
            }
            else if (tool.GetType() == typeof(LijnTool))
            {
                g.DrawLine(MaakPen(kleur, dikte), p1, p2);
            }
            else if (tool.GetType() == typeof(VolRechthoekTool))
            {
                g.FillRectangle(kleur, TweepuntTool.Punten2Rechthoek(p1, p2));
            }
            else if (tool.GetType() == typeof(RechthoekTool))
            {
                g.DrawRectangle(MaakPen(kleur, dikte), TweepuntTool.Punten2Rechthoek(p1, p2));
            }
            else if (tool.GetType() == typeof(TekstTool))
            {
                Font font = new Font("Tahoma", 40);
                SizeF sz =
                g.MeasureString(tekst, font, p1, StringFormat.GenericTypographic);
                g.DrawString(tekst, font, kleur,
                                              p1, StringFormat.GenericTypographic);
            }
        }
        public Element (string m)
        {
            
            string[] splits = m.Split(' ');
            
            if (splits[0] == "kader")
            {
                RechthoekTool tijdelijk = new RechthoekTool();
                this.tool = tijdelijk;
            }
            else if (splits[0] == "vlak")
            {
                VolRechthoekTool tijdelijk = new VolRechthoekTool();
                this.tool = tijdelijk;
            }
            else if (splits[0] == "lijn")
            {
                LijnTool tijdelijk = new LijnTool();
                this.tool = tijdelijk;
            }
            
            else if (splits[0] == "tekst")
            {
                TekstTool tijdelijk = new TekstTool();
                this.tool = tijdelijk;
                string tekst = splits[6];
            }
            else if (splits[0] == "pen")
            {
                PenTool tijdelijk = new PenTool();
                this.tool = tijdelijk;
            }

            this.p1 = new Point(int.Parse(splits[1]), int.Parse(splits[2]));
            this.p2 = new Point(int.Parse(splits[3]), int.Parse(splits[4]));
            Color k = new Color();
            k = Color.FromArgb(int.Parse(splits[5]), int.Parse(splits[6]), int.Parse(splits[7]));
            this.kleur = new SolidBrush(k);
            this.tekst = splits[8];
            this.dikte = int.Parse(splits[9]);

            //kleur = new SolidBrush(s.PenKleur);
        }

        public Element kopie ()
        {
            return new Element(tool, p1, p2, kleur, tekst, dikte);
        }
    }
}