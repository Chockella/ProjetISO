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
    public partial class Entity : UserControl
    {
        private GraphicsPath gPath;
        public Cell CurrentCell;

        public Entity()
        {
            this.BackColor = Color.Red;
            this.Width = 20;
            this.Height = 20;
            this.BringToFront();
        }
    }
}
