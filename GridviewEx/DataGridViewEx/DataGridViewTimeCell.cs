using System;
using System.Windows.Forms;

namespace coms.COMMON.ui
{
    public class DataGridViewTimeCell : DataGridViewTextBoxCell
    {
        public override void InitializeEditingControl(int rowIndex, object initialFormattedValue,
            DataGridViewCellStyle dataGridViewCellStyle)
        {
            base.InitializeEditingControl(rowIndex, initialFormattedValue, dataGridViewCellStyle);

            var ctl = DataGridView.EditingControl as TimeEditingControl;

            if (this.Value == null || string.IsNullOrEmpty(this.Value.ToString()))
            {
                ctl.Text = "";
                ctl.ShowPlaceholder();
            }
            else
            {
                ctl.Text = this.Value.ToString();
                ctl.HidePlaceholder();
            }
        }

        public override Type EditType => typeof(TimeEditingControl);

        public override Type ValueType => typeof(string);

        public override object DefaultNewRowValue => "";

        public override void DetachEditingControl()
        {
            var ctl = DataGridView.EditingControl as TimeEditingControl;
            if (ctl != null)
            {
                string input = ctl.Text;

                if (!IsValidTime(input))
                {
                    this.Value = "";
                }
                else
                {
                    this.Value = input;
                }
            }

            base.DetachEditingControl();
        }

        private bool IsValidTime(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return false;
            if (text.Length != 5) return false;
            if (text[2] != ':') return false;

            string hh = text.Substring(0, 2);
            string mm = text.Substring(3, 2);

            if (!int.TryParse(hh, out int hour)) return false;
            if (!int.TryParse(mm, out int minute)) return false;

            if (hour < 0 || hour > 23) return false;
            if (minute < 0 || minute > 59) return false;

            return true;
        }
    }
}
