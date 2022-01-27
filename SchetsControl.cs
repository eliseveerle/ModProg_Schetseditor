using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SchetsEditor
{
    public class SchetsControl : UserControl
    {
        private Schets schets;
        private Color penkleur;
        private int pendikte;
        public ISchetsTool huidigeTool;
        public Color PenKleur
        {
            get { return penkleur; }
        }
        public Schets Schets
        {
            get { return schets; }
        }
        // nieuw, weet nog niet waarom ik dit deed
        public int Pendikte
        {
            get { return pendikte; }
        }

        public SchetsControl()

        {   //nieuw:
            this.huidigeTool = null;
            this.BorderStyle = BorderStyle.Fixed3D;
            this.schets = new Schets();
            this.Paint += this.teken;
            this.Resize += this.veranderAfmeting;
            this.veranderAfmeting(null, null);

        }
        protected override void OnPaintBackground(PaintEventArgs e)
        {
        }
        private void teken(object o, PaintEventArgs pea)
        {
            schets.Teken(pea.Graphics);
        }
        private void veranderAfmeting(object o, EventArgs ea)
        {
            schets.VeranderAfmeting(this.ClientSize);

            this.Invalidate();
        }
        public Graphics MaakBitmapGraphics()
        {
            Graphics g = schets.BitmapGraphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            return g;
        }
        public void leeg(object o, EventArgs ea)
        {
            schets.Schoon();
            schets.geschiedenisopslaan();
            this.Invalidate();
        }
        public void Schoon(object o, EventArgs ea)
        {
            schets.Schoon();
            this.Invalidate();
        }
        public void Roteer(object o, EventArgs ea)
        {
            Graphics g = MaakBitmapGraphics();
            this.Schoon(o, ea);
            schets.Roteer();
            // foreach aanmaken en elk punt aanpassen in de rotate list
            //schets.elements_list
            //schets.VeranderAfmeting(new Size(this.ClientSize.Height, this.ClientSize.Width));
            this.Invalidate();
        }
        //deze methode klopt nog niet helemaal
        public void undo(object o, EventArgs ea)
        {
            if ((schets.elementen_geschiedenis.Count - 2) < 0)
                MessageBox.Show ("Kan niet verder terug, voor een leeg vel druk op clear");
            else
            {
                Graphics g = MaakBitmapGraphics();
                this.Schoon(o, ea);
                schets.elements_list.Clear();
                foreach (Element e in schets.elementen_geschiedenis[schets.elementen_geschiedenis.Count - 2])
                {
                    schets.elements_list.Add(e);
                }

                int a = (int)(schets.elementen_geschiedenis.Count - 1);
                schets.elementen_geschiedenis.RemoveAt(a);

                foreach (Element elem in Schets.elements_list)
                {
                    elem.teken(this, g, elem.p1, elem.p2);
                }

                this.Invalidate();
            }
            
        }
        //
        public void VeranderKleur(object obj, EventArgs ea)
        {
            string kleurNaam = ((ComboBox)obj).Text;
            penkleur = Color.FromName(kleurNaam);
        }
        //deze is ook nieuw
        public void VeranderDikte(object obj, EventArgs ea)
        {

            int dikte = int.Parse(((ComboBox)obj).Text);
            pendikte = dikte;
        }
        //deze is ook nieuw
        public void ok_klik(object obj, EventArgs ea, int red, int green, int blue)
        {
            penkleur = Color.FromArgb(red, green, blue);

        }
        public void VeranderKleurViaMenu(object obj, EventArgs ea)
        {
            string kleurNaam = ((ToolStripMenuItem)obj).Text;
            penkleur = Color.FromName(kleurNaam);
        }

        //nog veranderdikteviaMenu aanpassen
    }
}