using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace Grid.Graphics.Controls
{
    /*
     * Ici, on créer une Structure. Notre objet Structure sera contenu dans notre cellule dans le but d'avoir un objet bloquant les lignes de vue.
     * La cellule sur laquelle se trouve la structure devient également inaccessible.
     * Plus tard, si l'on veut intégrer une image plutôt qu'un Polygone vectoriel, il faudra adapter cette classe au dessin d'image.
     */
    public partial class Structure : UserControl
    {
        //On stock son parent pour la manipulation
        private Cell parentCell;
        //Comme sur la cellule, on stock le chemin graphique pour optimiser le dessin du Control
        private GraphicsPath gPath = new GraphicsPath();
        private Color baseColor;

        public Structure(Cell parent, Color baseColor)
        {
            //On définit quelques valeurs
            this.baseColor = baseColor;
            this.parentCell = parent;
            //En faisant en sorte que la position Y du parent soit 20 px (soit la moitié de la hauteur d'une cellule) au dessus de son parent
            this.Top = parent.Top - 20;
            this.Left = parent.Left;

            this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            this.BackColor = Color.Transparent;

            //Création du polygone
            gPath.AddLine(30, 0, 60, 20);
            gPath.AddLine(60, 20, 60, 40);
            gPath.AddLine(60, 40, 30, 60);
            gPath.AddLine(30, 60, 0, 40);
            gPath.AddLine(0, 40, 0, 20);
            gPath.AddLine(0, 20, 30, 0);
            gPath.CloseFigure();

            //On envoie la structure au premier plan, sinon il se retrouve sous toutes les cellules
            this.BringToFront();

            //Comme sur la cellule, on définit la région propre au polygone de la structure
            this.Region = new Region(gPath);
        }

        //On hack Windows Forms pour dessiner le Control à sa place
        protected override void OnPaint(PaintEventArgs e)
        {
            SolidBrush brush = new SolidBrush(Color.FromArgb(90, this.baseColor));
            e.Graphics.FillPath(brush, gPath);
        }

        //Puis on annule la peinture de l'arrière-plan du Control pour éviter que toute autre structure qui viendra se dessiner au dessus de lui perde un morceau
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);
        }

    }
}
