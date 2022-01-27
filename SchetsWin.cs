using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;
using System.Resources;
using System.IO;

namespace SchetsEditor
{
    public class SchetsWin : Form
    {
        MenuStrip menuStrip;
        SchetsControl schetscontrol;
        ISchetsTool huidigeTool;
        Panel paneel;
        TextBox rood;
        TextBox groen;
        TextBox blauw;
        Button Ok;
        bool vast;
        ResourceManager resourcemanager
            = new ResourceManager("SchetsEditor.Properties.Resources"
                                 , Assembly.GetExecutingAssembly()
                                 );

        private void veranderAfmeting(object o, EventArgs ea)
        {
            schetscontrol.Size = new Size(this.ClientSize.Width - 70
                                          , this.ClientSize.Height - 60);
            paneel.Location = new Point(64, this.ClientSize.Height - 40);
        }

        private void klikToolMenu(object obj, EventArgs ea)
        {
            schetscontrol.huidigeTool = huidigeTool;
            this.huidigeTool = (ISchetsTool)((ToolStripMenuItem)obj).Tag;
        }

        private void klikToolButton(object obj, EventArgs ea)
        {
            schetscontrol.huidigeTool = huidigeTool;
            this.huidigeTool = (ISchetsTool)((RadioButton)obj).Tag;
        }

        private void afsluiten(object obj, EventArgs ea)
        {
            this.Close();
        }

        //nieuw
        private void save(object obj, EventArgs ea)
        {
            if (this.Text == "")
                save_as(obj, ea);
            else schrijfNaarFile();
        }
        //nieuw
        private void save_as(object obj, EventArgs ea)
        {
            SaveFileDialog dialoog = new SaveFileDialog();
            dialoog.Filter = "Tekstfiles|.txt|Alle files|.*";
            dialoog.Title = "Tekst opslaan als...";
            if (dialoog.ShowDialog() == DialogResult.OK)
            {
                this.Text = dialoog.FileName;
                this.schrijfNaarFile();
            }
        }
        //nieuw
        private void schrijfNaarFile()
        {
            StreamWriter writer = new StreamWriter(this.Text);
            foreach (Element e in schetscontrol.Schets.elements_list)
            {
                SolidBrush k = (SolidBrush)e.kleur;
                string kleur = k.Color.R + " " + k.Color.G + " " + k.Color.B;
                writer.WriteLine(e.tool + " " + e.p1.X + " " + e.p1.Y + " " + e.p2.X + " " + e.p2.Y + " " + kleur + " " + e.tekst + " " + e.dikte);
            }

            writer.Close();
        }

        public void LeesVanFile(string naam)
        {
            StreamReader reader = new StreamReader(naam);
            // string k = reader.ReadToEnd();
            string m = reader.ReadLine();
            //this.invoer.Text = reader.ReadToEnd();
            while (m != null)
            {
                Element e = new Element(m);
                schetscontrol.Schets.elements_list.Add(e);
                m = reader.ReadLine();
            }
            reader.Close();
            this.Text = naam;
            Graphics g = schetscontrol.MaakBitmapGraphics();
            schetscontrol.Schets.reconstrueer(g);
        }

            public SchetsWin()
        {
            ISchetsTool[] deTools = { new PenTool()
                                    , new LijnTool()
                                    , new RechthoekTool()
                                    , new VolRechthoekTool()
                                    , new TekstTool()
                                    , new GumTool()
                                    , new Bring_forwardTool()
                                    , new ToTheBackTool()

                                    };
            String[] deKleuren = { "Black", "Red", "Green", "Blue"
                                 , "Yellow", "Magenta", "Cyan", "White"
                                 };
            int[] deLijnDiktes = { 2,3,5,10,15,20,30,50
                                 };
            //ik heb hier white toegevoegd, verder niks bij SchetsWin voor het gummen
            this.ClientSize = new Size(700, 500);
            huidigeTool = deTools[0];
            //nu voeg ik toe:
            schetscontrol = new SchetsControl();
            schetscontrol.Location = new Point(64, 10);
            schetscontrol.MouseDown += (object o, MouseEventArgs mea) =>
            {
                vast = true;
                huidigeTool.MuisVast(schetscontrol, mea.Location);
            };
            schetscontrol.MouseMove += (object o, MouseEventArgs mea) =>
            {
                if (vast)
                    huidigeTool.MuisDrag(schetscontrol, mea.Location);
            };
            schetscontrol.MouseUp += (object o, MouseEventArgs mea) =>
            {
                if (vast)
                    huidigeTool.MuisLos(schetscontrol, mea.Location);
                vast = false;
            };
            schetscontrol.KeyPress += (object o, KeyPressEventArgs kpea) =>
            {
                huidigeTool.Letter(schetscontrol, kpea.KeyChar);
            };
            this.Controls.Add(schetscontrol);

            menuStrip = new MenuStrip();
            menuStrip.Visible = false;
            this.Controls.Add(menuStrip);
            this.maakFileMenu();
            this.maakToolMenu(deTools);
            this.maakAktieMenu(deKleuren);
            this.maakToolButtons(deTools);
            this.maakAktieButtons(deKleuren, deLijnDiktes);
            this.Resize += this.veranderAfmeting;
            this.veranderAfmeting(null, null);
        }

        private void maakFileMenu()
        {
            ToolStripMenuItem menu = new ToolStripMenuItem("File");
            menu.MergeAction = MergeAction.MatchOnly;
            menu.DropDownItems.Add("Sluiten", null, this.afsluiten);
                menu.DropDownItems.Add("Save", null, this.save);
                menu.DropDownItems.Add("Save as", null, this.save_as);
                menuStrip.Items.Add(menu);
        }

        private void maakToolMenu(ICollection<ISchetsTool> tools)
        {
            ToolStripMenuItem menu = new ToolStripMenuItem("Tool");
            foreach (ISchetsTool tool in tools)
            {
                ToolStripItem item = new ToolStripMenuItem();
                item.Tag = tool;
                item.Text = tool.ToString();
                item.Image = (Image)resourcemanager.GetObject(tool.ToString());
                item.Click += this.klikToolMenu;
                menu.DropDownItems.Add(item);
            }
            menuStrip.Items.Add(menu);
        }

        private void maakAktieMenu(String[] kleuren)
        {
            ToolStripMenuItem menu = new ToolStripMenuItem("Aktie");
            menu.DropDownItems.Add("Clear", null, schetscontrol.leeg);
            menu.DropDownItems.Add("Roteer", null, schetscontrol.Roteer);
            ToolStripMenuItem submenu = new ToolStripMenuItem("Kies kleur");
            foreach (string k in kleuren)
                submenu.DropDownItems.Add(k, null, schetscontrol.VeranderKleurViaMenu);
            menu.DropDownItems.Add(submenu);
            menuStrip.Items.Add(menu);
        }

        private void maakToolButtons(ICollection<ISchetsTool> tools)
        {
            int t = 0;
            foreach (ISchetsTool tool in tools)
            {
                RadioButton b = new RadioButton();
                b.Appearance = Appearance.Button;
                b.Size = new Size(45, 62);
                b.Location = new Point(10, 10 + t * 62);
                b.Tag = tool;
                b.Text = tool.ToString();
                b.Image = (Image)resourcemanager.GetObject(tool.ToString());
                b.TextAlign = ContentAlignment.TopCenter;
                b.ImageAlign = ContentAlignment.BottomCenter;
                b.Click += this.klikToolButton;
                this.Controls.Add(b);
                if (t == 0) b.Select();
                t++;
            }
        }

        private void maakAktieButtons(String[] kleuren, int[] diktes)
        {
            paneel = new Panel();
            paneel.Size = new Size(600, 50);
            this.Controls.Add(paneel);

            Button b; Label l; ComboBox cbb;
            b = new Button();
            b.Text = "Clear";
            b.Location = new Point(0, 0);
            b.Click += schetscontrol.leeg;
            paneel.Controls.Add(b);

            b = new Button();
            b.Text = "Rotate";
            b.Location = new Point(80, 0);
            b.Click += schetscontrol.Roteer;
            paneel.Controls.Add(b);

            l = new Label();
            l.Text = "Penkleur:";
            l.Location = new Point(180, 3);
            l.AutoSize = true;
            paneel.Controls.Add(l);
            //dit wordt ook nieuw
            Label red = new Label();
            Label green = new Label();
            Label blue = new Label();
            rood = new TextBox();
            groen = new TextBox();
            blauw = new TextBox();

            red.Text = "red ";
            red.Location = new Point(180, 20);
            rood.Size = new Size(50, 20);
            rood.Location = new Point(205, 20);
            rood.Text = "0";
            red.AutoSize = true;

            groen.Size = new Size(50, 20);
            groen.Location = new Point(320, 20);
            groen.Text = "0";
            green.Text = "green";
            green.Location = new Point(280, 20);
            green.AutoSize = true;

            blauw.Size = new Size(50, 20);
            blauw.Location = new Point(425, 20);
            blauw.Text = "0";
            blue.Text = "blue";
            blue.Location = new Point(400, 20);
            blue.AutoSize = true;

            paneel.Controls.Add(rood);
            paneel.Controls.Add(groen);
            paneel.Controls.Add(blauw);
            paneel.Controls.Add(red);
            paneel.Controls.Add(green);
            paneel.Controls.Add(blue);

            //ok button
            Ok = new Button();
            Ok.Location = new Point(480, 20);
            Ok.AutoSize = true;
            Ok.Text = "Ok";
            Ok.Click += klikken;

            paneel.Controls.Add(Ok);





            //dit is nieuw
            Button undo = new Button();
            undo.Text = "Undo";
            undo.Location = new Point(80, 20);
            EventArgs ea = new EventArgs();
            undo.Click += schetscontrol.undo;
            paneel.Controls.Add(undo);

            cbb = new ComboBox(); cbb.Location = new Point(240, 0);
            cbb.DropDownStyle = ComboBoxStyle.DropDownList;
            cbb.SelectedValueChanged += schetscontrol.VeranderKleur;
            cbb.Size = new Size(70, 20);
            foreach (string k in kleuren)
                cbb.Items.Add(k);
            cbb.SelectedIndex = 0;
            paneel.Controls.Add(cbb);
            //dit is nieuw
            cbb = new ComboBox(); cbb.Location = new Point(350, 0);
            cbb.DropDownStyle = ComboBoxStyle.DropDownList;
            cbb.SelectedValueChanged += schetscontrol.VeranderDikte;
            cbb.Size = new Size(50, 20);
            foreach (int n in diktes)
                cbb.Items.Add(n.ToString());
            cbb.SelectedIndex = 0;
            paneel.Controls.Add(cbb);
            //deze is ook nieuw
            l = new Label();
            l.Text = "Dikte:";
            l.Location = new Point(310, 3);
            l.AutoSize = true;
            paneel.Controls.Add(l);
        }
        public void klikken(object obj, EventArgs ea)
        {
            int Rood; 
            int Groen; 
            int Blauw; 
            if (rood.Text == "")
                Rood = 0;
            else
                Rood = int.Parse(rood.Text);
            if (groen.Text == "")
                Groen = 0;
            else
                Groen = int.Parse(groen.Text);
            if (blauw.Text == "")
                Blauw = 0;
            else
                Blauw = int.Parse(blauw.Text);
            if (Rood > 255)
                Rood = 255;
            if (Groen > 255)
                Groen = 255;
            if (Blauw > 255)
                Blauw = 255;

            rood.Text = Rood.ToString();
            groen.Text = Groen.ToString();
            blauw.Text = Blauw.ToString();
            schetscontrol.ok_klik(this, ea, Rood , Groen , Blauw);
        }
    }
}
