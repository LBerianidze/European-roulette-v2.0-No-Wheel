using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace European_roulette_v2._0
{
    public partial class UserControl1 : DataGridView
    {
        public UserControl1()
        {
            InitializeComponent();
            this.HorizontalScrollBar.Visible = true;
            this.HorizontalScrollBar.VisibleChanged += this.VerticalScrollBar_VisibleChanged;
            this.HorizontalScrollBar.SetBounds(this.HorizontalScrollBar.Location.X, this.HorizontalScrollBar.Location.Y, this.Width, this.HorizontalScrollBar.Height);
        }

        private void VerticalScrollBar_VisibleChanged(object sender, EventArgs e)
        {
            this.HorizontalScrollBar.Visible = true;
        }
        public void UpdateHorizontalWidth()
        {
            this.HorizontalScrollBar.SetBounds(this.HorizontalScrollBar.Location.X, 112, 567, this.HorizontalScrollBar.Height);
        }

        }
    }
