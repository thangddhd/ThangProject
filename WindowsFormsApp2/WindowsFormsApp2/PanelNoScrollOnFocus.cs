using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApp2
{
    class PanelNoScrollOnFocus : Panel
    {
        protected override System.Drawing.Point ScrollToControl(Control activeControl)
        {
            return DisplayRectangle.Location;
        }
    }
}
