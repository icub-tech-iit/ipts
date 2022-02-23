using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace iCubProductionTestSuite.classes
{
    public partial class StartStop : UserControl
    {
        public event EventHandler ButtonStartClick;
        public event EventHandler ButtonStopClick;

        public StartStop()
        {
            InitializeComponent();
        }

        private void buttonRun_Click(object sender, EventArgs e)
        {
            buttonStop.Enabled = true;
            buttonRun.Enabled = false;

            //bubble the event up to the parent
            if (this.ButtonStartClick != null)
                this.ButtonStartClick(this, e);
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            buttonStop.Enabled = false;
            buttonRun.Enabled = true;

            //bubble the event up to the parent
            if (this.ButtonStopClick != null)
                this.ButtonStopClick(this, e);
        }

        public void setStartEnabled(bool b)
        {
            buttonRun.Enabled = b;
            buttonStop.Enabled = !b;
        }
    }
}
