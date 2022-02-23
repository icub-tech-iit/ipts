using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iCubProductionTestSuite.classes
{
    class GroupedControls
    {
        private List<System.Windows.Forms.Control> controls = new List<System.Windows.Forms.Control>();

        public List<System.Windows.Forms.Control> Controls
        {
            get
            {
                return controls;
            }
        }

        public void addControl(System.Windows.Forms.Control c)
        {
            Controls.Add(c);
        }

        public void setEnable(bool en)
        {
            foreach (System.Windows.Forms.Control c in Controls)
            {
                if (c.Name.Contains("tic"))
                {
                    TestItemControl t = (TestItemControl)c;
                    t.setCheckboxEnabled(en);
                }
                else c.Enabled = en;
            }
        }
    }
}
