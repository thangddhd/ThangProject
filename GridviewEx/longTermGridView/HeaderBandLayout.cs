using System.Collections.Generic;
using System.Drawing;

namespace coms.COMSK.ui.common
{
    public sealed class HeaderBandLayout
    {
        public int HeaderRowCount { get; set; }
        public int HeaderRowHeight { get; set; }
        public List<HeaderBandCellByName> Cells { get; private set; }

        public HeaderBandLayout()
        {
            HeaderRowCount = 1;
            HeaderRowHeight = 22;
            Cells = new List<HeaderBandCellByName>();
        }
    }

    public sealed class HeaderBandCellByName
    {
        public int BandRow { get; set; }
        public int BandRowSpan { get; set; }
        public List<string> ColumnNames { get; private set; }

        public string Text { get; set; }
        public Color BackColor { get; set; }
        public Color ForeColor { get; set; }
        public Font Font { get; set; }

        public Color BorderColor { get; set; }
        public int BorderThickness { get; set; }

        public HeaderBandCellByName()
        {
            BandRow = 0;
            BandRowSpan = 1;
            ColumnNames = new List<string>();

            Text = "";
            BackColor = SystemColors.Control;
            ForeColor = SystemColors.ControlText;
            Font = null;

            BorderColor = Color.DarkGray;
            BorderThickness = 1;
        }
    }
}