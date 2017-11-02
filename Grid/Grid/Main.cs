using Grid.Graphics.Controls;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Grid
{
    public partial class Main : Form
    {
        private List<Cell> cells = new List<Cell>();
        public readonly int MapWidth = 10;
        public readonly int MapHeight = 20;

        public Main()
        {
            //Les positions de départ
            int posX = -30;
            int posY = -20;
            bool pair = true;

            //Première boucle, sur chaque lignes
            for(int i = 0;i < MapHeight; i++)
            {
                //Deuxième boucle, sur chaque colonnes
                for(int j = 0;j < MapWidth; j++)
                {
                    Cell c = new Cell(cells.Count);
                    c.Top = posY;
                    c.Left = posX;
                    this.Controls.Add(c);

                    posX += 60;

                    //On ajoute les cellules dans une liste pour leur manipulation ultérieure
                    cells.Add(c);

                    switch (cells.Count)
                    {
                        case 105:
                            c.DrawStructure(Color.Red);
                            break;
                        case 125:
                            c.DrawStructure(Color.Green);
                            break;
                        case 73:
                            c.SetUnwalkable();
                            break;
                    }
                }
                pair = !pair;
                posY += 20;

                //Si pair, on redéfinit les position de départ, sinon on met à 0
                if (pair)
                    posX = -30;
                else
                    posX = 0;
            }
            //Génération du Player
            Entity playerEntity = new Entity();
            cells[122].AddEntity(playerEntity);

            InitializeComponent();
        }
        public Cell GetCell(int id)
        {
            return cells[id];
        }
        public void BringCellsToFront(Cell firstCell)
        {
            for(int i = firstCell.CellId + 1;i < cells.Count; i++)
            {
                Console.WriteLine(i + " Bringed");
                cells[i].BringToFront();
                cells[i].BringStructure();
            }
        }
    }
}
