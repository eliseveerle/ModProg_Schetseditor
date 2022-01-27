using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Drawing;

namespace SchetsEditor
{
    public class Schets
    {
        private Bitmap bitmap;
        public List<Element> elements_list;
        // public Element vorig;
        public List<List<Element>> elementen_geschiedenis;
        public Schets()
        {
            bitmap = new Bitmap(1, 1);
            elements_list = new List<Element>();
            elementen_geschiedenis = new List<List<Element>>();

        }
        public Graphics BitmapGraphics
        {
            get { return Graphics.FromImage(bitmap); }
        }
        public void VeranderAfmeting(Size sz)
        {
            if (sz.Width > bitmap.Size.Width || sz.Height > bitmap.Size.Height)
            {
                Bitmap nieuw = new Bitmap(Math.Max(sz.Width, bitmap.Size.Width)
                                         , Math.Max(sz.Height, bitmap.Size.Height)
                                         );
                Graphics gr = Graphics.FromImage(nieuw);
                gr.FillRectangle(Brushes.White, 0, 0, sz.Width, sz.Height);
                gr.DrawImage(bitmap, 0, 0);
                bitmap = nieuw;
            }
        }
        public void Teken(Graphics gr)
        {
            gr.DrawImage(bitmap, 0, 0);
        }
        public void Schoon()
        {
            Graphics gr = Graphics.FromImage(bitmap);
            gr.FillRectangle(Brushes.White, 0, 0, bitmap.Width, bitmap.Height);
        }
        public void Roteer()
        {
            this.Schoon();
            List<Element> l = new List<Element>();
            foreach (Element k in elements_list)
            {
                
                l.Add(k.kopie());
            }
            foreach (Element e in l)
            {
                e.p1 = gedraaid (e.p1);
                e.p2 = gedraaid (e.p2);
            }
            elements_list.Clear();
            foreach (Element m in l)
            {
                elements_list.Add(m);
            }
            Graphics g = BitmapGraphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            this.reconstrueer(g);            
            geschiedenisopslaan();
        }

        public void geschiedenisopslaan()
        {
            List<Element> l = new List<Element>();
            foreach (Element k in this.elements_list)
            {
                l.Add(k);
            }
            elementen_geschiedenis.Add(l);
        }
        public void reconstrueer(Graphics g)
        {
            foreach (Element elem in elements_list)
            {
                elem.teken(this, g, elem.p1, elem.p2);
            }
        }

        public Point gedraaid (Point oud)
        {
            Point nieuw = new Point();
            nieuw.X = ((oud.Y - (bitmap.Height / 2)) + (bitmap.Width / 2));
            nieuw.Y = -1 * (oud.X - (bitmap.Width) / 2) + (bitmap.Height / 2);
            return nieuw;
        }
    }
}