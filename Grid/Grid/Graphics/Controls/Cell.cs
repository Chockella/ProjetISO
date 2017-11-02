using System;
using System.Collections.Generic;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace Grid.Graphics.Controls
{
    public partial class Cell : UserControl
    {
        // On stocke les points dans un tableau pour s'en reservir plus tard
        private PointF[] points = new PointF[4];
        // On stocke le chemin graphique pour réduire la charge de travail du CPU lors de l'évènement OnPaint
        private GraphicsPath gPath = new GraphicsPath();
        private Structure structure;
        private bool isWalkable = true;

        // On attribue un Id aux cellules afin de reconnaître cette cellule pour définir des placements d'entités par exemple
        public readonly int CellId;
        private bool isHover = false;
        private bool firstPaint = false;

        /*
         * Lors de l'initialisation, on va affecter les valeurs aux points et dessiner le chemin graphique de la cellule
         * Rappel : le chemin graphique de la cellule est uniquement si on dessine la grille, chose qui n'est pas indispensable
         */
        public Cell(int cellId)
        {
            this.CellId = cellId;
            this.SetStyle(ControlStyles.UserMouse, true);
            this.SetStyle(ControlStyles.Opaque, false);
            this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            this.BackColor = Color.Transparent;
            this.Cursor = Cursors.Hand;
            this.Width = 60;
            this.Height = 40;

            this.points[0] = new PointF(0, 20);
            this.points[1] = new PointF(30, 0);
            this.points[2] = new PointF(60, 20);
            this.points[3] = new PointF(30, 40);

            gPath.AddLine(this.points[0], this.points[1]);
            gPath.AddLine(this.points[1], this.points[2]);
            gPath.AddLine(this.points[2], this.points[3]);
            gPath.AddLine(this.points[3], this.points[0]);
            gPath.CloseFigure();
            this.BringToFront();

            //On redéfinit la région du Control afin de considérer la zone de dessin du Control comme notre losange uniquement
            this.Region = new Region(gPath);

            this.MouseEnter += Cell_MouseEnter;
            this.MouseLeave += Cell_MouseLeave;
        }

        /// <summary>
        /// Retourne la cellule au dessus de la cellule courante
        /// </summary>
        /// <returns></returns>
        public Cell GetTopCell()
        {
            Main m = (Main)this.Parent;
            if (this.CellId - m.MapWidth * 2 < 0)
                return null;
            else return m.GetCell(this.CellId - m.MapWidth * 2);
        }
        /// <summary>
        /// Retourne la cellule à gauche de la cellule courante
        /// </summary>
        /// <returns></returns>
        public Cell GetLeftCell()
        {
            Main m = (Main)this.Parent;
            if (this.CellId > 0 && this.CellId % (m.MapWidth + 1) == 0)
                return m.GetCell(this.CellId - 1);
            else return null;
        }
        /// <summary>
        /// Retourne la cellule à droite de la cellule courante
        /// </summary>
        /// <returns></returns>
        public Cell GetRightCell()
        {
            Main m = (Main)this.Parent;
            if (this.CellId % m.MapWidth == 0)
                return m.GetCell(this.CellId + 1);
            else return null;
        }
        /// <summary>
        /// Retourne la cellule au dessous de la cellule courante
        /// </summary>
        /// <returns></returns>
        public Cell GetBottomCell()
        {
            Main m = (Main)this.Parent;
            if (this.CellId + m.MapWidth * 2 < m.MapHeight * m.MapWidth)
                return m.GetCell(this.CellId + m.MapWidth * 2);
            else return null;
        }

        /// <summary>
        /// Retourne la cellule en bas à droite de la cellule courante
        /// </summary>
        /// <returns></returns>
        public Cell GetBottomRightCell()
        {
            return null;
        }
        /// <summary>
        /// Retourne la cellule en bas à gauche de la cellule courante
        /// </summary>
        /// <returns></returns>
        public Cell GetBottomLeftCell()
        {
            return null;
        }
        /// <summary>
        /// Retourne la cellule en haut à droite de la cellule courante
        /// </summary>
        /// <returns></returns>
        public Cell GetTopRightCell()
        {
            return null;
        }
        /// <summary>
        /// Retourne la cellule en haut à gauche de la cellule courante
        /// </summary>
        /// <returns></returns>
        public Cell GetTopLeftCell()
        {
            return null;
        }

        /// <summary>
        /// Ajoute l'entité à la cellule
        /// </summary>
        public void AddEntity(Entity entity)
        {
            entity.Top = this.Top + 30;
            entity.Left = this.Left + 50;
            this.Parent.Controls.Add(entity);
            ((Main)this.Parent).BringCellsToFront(this);
            entity.BringToFront();
            entity.CurrentCell = this;
        }

        /// <summary>
        /// Balance la structure contenu dans la cellule au premier plan
        /// </summary>
        public void BringStructure()
        {
            this.firstPaint = false;
            if(this.structure != null)
                this.structure.BringToFront();
            this.Refresh();
        }

        /// <summary>
        /// Renvoie true si la cellule est marchable et si elle ne contient aucune structure
        /// </summary>
        public bool IsAccessible
        {
            get => (this.isWalkable && this.structure == null);
        }

        //Défini sur false la valeur isHover lorsque la souris n'est plus sur la région
        private void Cell_MouseLeave(object sender, EventArgs e)
        {
            this.isHover = false;
            this.Refresh();
        }

        //Défini sur true la valeur isHover lorsque la souris entre dans la région
        private void Cell_MouseEnter(object sender, EventArgs e)
        {
            this.isHover = true;
            this.Refresh();
        }


        /// <summary>
        /// Permet d'activer la fonction de dessin d'une structure avec une couleur de base pour cette structure, rendant la cellule inaccessible et bloquant la vue
        /// </summary>
        public void DrawStructure(Color baseColor)
        {
            structure = new Structure(this, baseColor);
            this.Parent.Controls.Add(structure);
            structure.BringToFront();

            //On annule l'écoute des évènements
            this.MouseEnter -= Cell_MouseEnter;
            this.MouseLeave -= Cell_MouseLeave;
        }

        /// <summary>
        /// Définit la valeur marchable sur false, la rendant inaccessible
        /// </summary>
        public void SetUnwalkable()
        {
            this.isWalkable = false;
            this.Cursor = Cursors.Default;

            //On annule l'écoute des évènements Hover
            this.MouseEnter -= Cell_MouseEnter;
            this.MouseLeave -= Cell_MouseLeave;
        }

        /// <summary>
        /// Renvoie true si le point est contenu dans la cellule
        /// </summary>
        public bool ContainsPoint(Point point)
        {
            List<float> coef = points.Skip(1).Select((p, i) =>
                                            (point.Y - points[i].Y) * (p.X - points[i].X)
                                          - (point.X - points[i].X) * (p.Y - points[i].Y)).ToList();
            if (coef.Any(p => p == 0))
                return true;
            for (int i = 1; i < coef.Count(); i++)
                if (coef[i] * coef[i - 1] < 0)
                    return false;

            return true;
        }

        //Dessinage de la forme, hack de Windows Forms pour dessiner notre propre forme
        //Si la fonction isHover est défini sur true, on dessine la cellule en rouge
        protected override void OnPaint(PaintEventArgs e)
        {
            if (isHover)
                e.Graphics.FillPath(new SolidBrush(Color.FromArgb(90, Color.Red)), gPath);
            else if (isWalkable) e.Graphics.FillPath(Brushes.White, gPath);
            else e.Graphics.FillPath(Brushes.Black, gPath);

            e.Graphics.DrawPath(new Pen(Color.FromArgb(50, 60, 60, 60), 1), gPath);
        }

        //On annule le dessin du background, sinon on dessinera par dessus les structures
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            if (!firstPaint)
            {
                firstPaint = true;
                base.OnPaintBackground(e);
            }
        }
    }
}
