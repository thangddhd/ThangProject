using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace coms.COMMON.ui
{
    public class CheckedComboBox : ComboBox
    {
        private DropDownWrapper dropdown;
        private string valueSeparator = ", ";
        private string emptyText = "";

        // Expose ItemCheck event like original
        public event ItemCheckEventHandler ItemCheck;

        // Maximum pixel height for dropdown (0 = unlimited). Default 200.
        public int MaxDropDownHeight { get; set; } = 200;

        public CheckedComboBox()
        {
            this.DropDownStyle = ComboBoxStyle.DropDown;
            this.DropDownHeight = 1;
            this.DrawMode = DrawMode.Normal;
            this.dropdown = new DropDownWrapper(this);
        }

        // ================= PROPERTIES (public API parity) =================
        public string ValueSeparator
        {
            get => valueSeparator;
            set => valueSeparator = value ?? ", ";
        }

        // CheckOnClick must map to inner CheckedListBox
        public bool CheckOnClick
        {
            get => dropdown.ListBox.CheckOnClick;
            set => dropdown.ListBox.CheckOnClick = value;
        }

        // DisplayMember should map to list
        public new string DisplayMember
        {
            get => dropdown.ListBox.DisplayMember;
            set => dropdown.ListBox.DisplayMember = value;
        }

        public CheckedListBox.ObjectCollection Items => dropdown.ListBox.Items;
        public CheckedListBox.CheckedItemCollection CheckedItems => dropdown.ListBox.CheckedItems;
        public CheckedListBox.CheckedIndexCollection CheckedIndices => dropdown.ListBox.CheckedIndices;
        public event Action<bool> DropDownClosed;

        public bool ValueChanged => dropdown.ValueChanged;

        // ================= METHODS (public API parity) =================
        /// <summary>
        /// '1', '2', '3'
        /// </summary>
        /// <returns></returns>
        public string GetSelectedValues() => dropdown.GetCheckedItemsValueForSql();
        /// <summary>
        /// 1, 2, 3
        /// </summary>
        /// <returns></returns>
        public string GetSelectedValuesNoQuote() => dropdown.GetCheckedItemsValue();
        /// <summary>
        /// 全選択の場合
        /// </summary>
        /// <returns></returns>
        public bool HasCheckedValue(string targetVal) => dropdown.HasCheckedValue(targetVal);
        /// <summary>
        /// a, b, c
        /// </summary>
        /// <returns></returns>
        public string GetCheckedItemsText() => dropdown.GetCheckedItemsText();

        public bool GetItemChecked(int index) => dropdown.ListBox.GetItemChecked(index);

        public CheckState GetItemCheckState(int index) => dropdown.ListBox.GetItemCheckState(index);

        public void SetItemChecked(int index, bool isChecked)
        {
            dropdown.ListBox.SetItemChecked(index, isChecked);
            UpdateText();
        }

        public void SetItemCheckState(int index, CheckState state)
        {
            dropdown.ListBox.SetItemCheckState(index, state);
            UpdateText();
        }

        public void Clear()
        {
            for (int i = 0; i < dropdown.ListBox.Items.Count; i++)
                dropdown.ListBox.SetItemChecked(i, false);
            UpdateText();
        }

        public void SetDefaultContent(string s)
        {
            emptyText = s ?? "";
            dropdown.EmptyText = emptyText;
            if (dropdown.ListBox.CheckedItems.Count == 0)
                this.Text = emptyText;
        }

        // ================= DROPDOWN OPEN/CLOSE =================
        protected override void OnDropDown(EventArgs e)
        {
            base.OnDropDown(e);
            dropdown.ShowDropdown();
        }

        // kept for compatibility (original used OnDropDownClosed with CCBoxEventArgs)
        internal void NotifyDropdownClosed(bool enacted)
        {
            DropDownClosed?.Invoke(enacted);
        }

        // ================= TEXT & KEY HANDLING =================
        private void UpdateText()
        {
            string s = dropdown.GetCheckedItemsText();
            this.Text = string.IsNullOrEmpty(s) ? emptyText : s;
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            // block typing like original
            e.Handled = true;
            base.OnKeyPress(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Down)
                OnDropDown(null);

            base.OnKeyDown(e);
        }

        public void SetAllItemChecked(bool isChecked)
        {
            for (int ii = 1; ii < Items.Count; ii++)
            {
                SetItemChecked(ii, isChecked);
            }
        }

        public void SetAllItemCheckedFromZero(bool isChecked)
        {
            for (int ii = 0; ii < Items.Count; ii++)
            {
                SetItemChecked(ii, isChecked);
            }
        }

        // ================= DROPDOWN WRAPPER (internal) =================
        // English comments only inside code.
        // P/Invoke for synthetic mouse messages
        private const int WM_RBUTTONDOWN = 0x0204;
        private const int WM_RBUTTONUP = 0x0205;
        private const int WM_LBUTTONUP = 0x0202;
        private const int WM_MOUSEMOVE = 0x0200;

        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);
        // English comment: bring the control to foreground to ensure it gets mouse events
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
        internal class DropDownWrapper
        {
            private readonly CheckedComboBox parent;
            private readonly ToolStripDropDown dropDown;
            private readonly ToolStripControlHost host;
            private readonly CustomCheckedListBox listBox;

            private bool dropdownClosed = true;
            private string oldText = "";
            private bool[] oldChecked;
            private string emptyText = "";

            public CheckedListBox ListBox => listBox;
            public string EmptyText { get => emptyText; set => emptyText = value; }

            public bool ValueChanged
            {
                get
                {
                    string now = parent.Text ?? "";
                    return now != oldText;
                }
            }

            public DropDownWrapper(CheckedComboBox parent)
            {
                this.parent = parent;

                listBox = new CustomCheckedListBox(this);
                listBox.BorderStyle = BorderStyle.None;
                listBox.CheckOnClick = true; // default like original
                listBox.SelectionMode = SelectionMode.One;
                listBox.HorizontalScrollbar = true;

                // Make the list dock to fill the ToolStripControlHost so host won't resize child unexpectedly.
                // Comments inside code are in English as requested.
                listBox.Dock = DockStyle.Fill;

                listBox.ItemCheck += (s, e) =>
                {
                    // forward event and update text after check change
                    parent.ItemCheck?.Invoke(s, e);
                    parent.BeginInvoke(new Action(() => parent.Text = GetCheckedItemsText()));
                };

                host = new ToolStripControlHost(listBox)
                {
                    AutoSize = false,
                    Margin = Padding.Empty,
                    Padding = Padding.Empty
                };

                dropDown = new ToolStripDropDown
                {
                    AutoClose = true
                };
                dropDown.Items.Add(host);
                dropDown.Closing += DropDown_Closing;
                dropDown.Closed += DropDown_Closed;
            }

            private void DropDown_Closing(object sender, ToolStripDropDownClosingEventArgs e)
            {
                // Case 1: clicking inside list    do NOT close
                if (e.CloseReason == ToolStripDropDownCloseReason.ItemClicked)
                {
                    try
                    {
                        var rect = listBox.RectangleToScreen(listBox.ClientRectangle);
                        if (rect.Contains(Cursor.Position))
                        {
                            e.Cancel = true;
                            return;
                        }
                    }
                    catch { }
                }

                // before setting dropdownClosed = true
                try { listBox.Capture = false; } catch { }
                // Case 2: dropdown is closing normally    mark as closed
                //dropdownClosed = true;
            }

            private void DropDown_Closed(object sender, ToolStripDropDownClosedEventArgs e)
            {
                if (dropdownClosed) return;

                dropdownClosed = true;

                parent.NotifyDropdownClosed(false);
            }

            public void ShowDropdown()
            {
                if (!dropdownClosed) return;
                dropdownClosed = false;

                // Backup text and checked states
                oldText = parent.Text ?? "";
                oldChecked = new bool[listBox.Items.Count];
                for (int i = 0; i < listBox.Items.Count; i++)
                    oldChecked[i] = listBox.GetItemChecked(i);

                // Determine an accurate item height (force handle creation)
                int itemH;
                try
                {
                    var dummy = listBox.Handle; // force handle creation
                    itemH = listBox.ItemHeight;
                    if (itemH <= 1)
                        itemH = TextRenderer.MeasureText("W", listBox.Font).Height + 4; // small padding
                }
                catch
                {
                    itemH = TextRenderer.MeasureText("W", listBox.Font).Height + 4;
                }

                // Compute count and desired height
                int count = listBox.Items.Count;
                if (count == 0) count = 1;
                if (count > parent.MaxDropDownItems) count = parent.MaxDropDownItems;

                int desiredHeight = itemH * count + 2;

                // Clamp by MaxDropDownHeight (public property on outer control)
                int finalHeight = Math.Max(150, desiredHeight);// fix min height = 150
                try
                {
                    int maxH = parent.MaxDropDownHeight;
                    if (maxH > 0 && finalHeight > maxH) finalHeight = maxH;
                }
                catch { }

                // Set sizes on host and make sure list fills the host (we docked the list)
                host.Size = new Size(parent.Width, finalHeight);
                try
                {
                    // ensure the hosted control has consistent client area
                    listBox.Size = host.Size;
                }
                catch { }

                // Show the dropdown under the parent control
                dropDown.Show(parent, new Point(0, parent.Height));

                // Allow the message loop to process layout/paint immediately
                try { Application.DoEvents(); } catch { }

                // Force repaint/layout
                try { listBox.Invalidate(); listBox.Update(); listBox.Refresh(); } catch { }

                // Focus the list so keyboard/hover works immediately
                try { listBox.Focus(); } catch { }

                // Select first item for immediate feedback (but do not toggle it automatically)
                if (listBox.Items.Count > 0)
                {
                    try
                    {
                        listBox.SelectedIndex = 0;
                        listBox.TopIndex = 0;
                    }
                    catch { }
                }

                // Capture mouse so the first mouse-up or click goes to the list control (fixes first-click issue)
                try
                {
                    // Attach a one-time MouseUp handler to release capture after first mouse-up
                    listBox.MouseUp += ListBox_MouseUp_ReleaseCaptureOnce;
                    listBox.Capture = true;
                }
                catch { }

                // If the left mouse button is held, optionally synthesize mouse-up as fallback.
                // This is kept as a fallback but capture above should be enough.
                try
                {
                    if ((Control.MouseButtons & MouseButtons.Left) == MouseButtons.Left)
                    {
                        // synthesize mouse move to update hover and mouse up to ensure item receives the event.
                        Point clientPt = listBox.PointToClient(Cursor.Position);
                        int lParam = (clientPt.Y << 16) | (clientPt.X & 0xFFFF);
                        SendMessage(listBox.Handle, WM_MOUSEMOVE, IntPtr.Zero, new IntPtr(lParam));
                        SendMessage(listBox.Handle, WM_LBUTTONUP, IntPtr.Zero, new IntPtr(lParam));
                    }
                }
                catch { }

                try
                {
                    if (listBox.Items.Count > 0)
                    {
                        // Select first item
                        listBox.SelectedIndex = 0;

                        // Compute a safe click point inside item 0
                        Point pt = new Point(5, listBox.ItemHeight / 2);
                        int lParam = (pt.Y << 16) | (pt.X & 0xFFFF);

                        // Send WM_RBUTTONDOWN + WM_RBUTTONUP
                        SendMessage(listBox.Handle, WM_RBUTTONDOWN, IntPtr.Zero, new IntPtr(lParam));
                        SendMessage(listBox.Handle, WM_RBUTTONUP, IntPtr.Zero, new IntPtr(lParam));
                    }
                }
                catch { /* ignore */ }
            }

            // English comment: one-time release capture handler attached on ShowDropdown
            private void ListBox_ReleaseCaptureOnFirstMouseDown(object sender, MouseEventArgs e)
            {
                try
                {
                    listBox.Capture = false;
                }
                catch { }

                try
                {
                    listBox.MouseDown -= ListBox_ReleaseCaptureOnFirstMouseDown;
                }
                catch { }
            }

            private void ListBox_MouseUp_ReleaseCaptureOnce(object sender, MouseEventArgs e)
            {
                try
                {
                    listBox.Capture = false;
                }
                catch { }
                try
                {
                    listBox.MouseUp -= ListBox_MouseUp_ReleaseCaptureOnce;
                }
                catch { }
            }

            public void CloseDropdown(bool accept)
            {
                // ensure we release any mouse capture we set earlier
                try { listBox.Capture = false; } catch { }

                if (dropdownClosed) return;
                dropdownClosed = true;

                if (!accept)
                {
                    // restore
                    for (int i = 0; i < listBox.Items.Count; i++)
                        listBox.SetItemChecked(i, oldChecked[i]);
                    parent.Text = oldText;
                }
                else
                {
                    // apply and update text
                    string s = GetCheckedItemsText();
                    parent.Text = string.IsNullOrEmpty(s) ? emptyText : s;
                }

                try { dropDown.Close(); } catch { }

                try
                {
                    var f = parent.FindForm();
                    if (f != null) f.Activate();
                }
                catch { }

                try { parent.Focus(); } catch { }

                parent.NotifyDropdownClosed(accept);
            }

            // Build display string from Name
            public string GetCheckedItemsText()
            {
                StringBuilder sb = new StringBuilder();
                foreach (var it in listBox.CheckedItems)
                {
                    var type = it.GetType();
                    var prop = type.GetProperty("Name");
                    string name = prop?.GetValue(it)?.ToString() ?? it.ToString();
                    sb.Append(name).Append(parent.ValueSeparator);
                }
                if (sb.Length == 0) return "";
                sb.Length -= parent.ValueSeparator.Length;
                return sb.ToString();
            }

            // Build SQL value string from Value property
            public string GetCheckedItemsValueForSql()
            {
                StringBuilder sb = new StringBuilder();
                foreach (var it in listBox.CheckedItems)
                {
                    var type = it.GetType();
                    var prop = type.GetProperty("Value");
                    string v = prop?.GetValue(it)?.ToString() ?? "";
                    if (!string.IsNullOrEmpty(v))
                        sb.Append("'").Append(v).Append("'").Append(parent.ValueSeparator);
                }
                if (sb.Length == 0) return "";
                sb.Length -= parent.ValueSeparator.Length;
                return sb.ToString();
            }

            public string GetCheckedItemsValue()
            {
                StringBuilder sb = new StringBuilder();
                foreach (var it in listBox.CheckedItems)
                {
                    var type = it.GetType();
                    var prop = type.GetProperty("Value");
                    string v = prop?.GetValue(it)?.ToString() ?? "";
                    if (!string.IsNullOrEmpty(v))
                        sb.Append(v).Append(parent.ValueSeparator);
                }
                if (sb.Length == 0) return "";
                sb.Length -= parent.ValueSeparator.Length;
                return sb.ToString();
            }

            public bool HasCheckedValue(string targetVal)
            {
                StringBuilder sb = new StringBuilder();
                foreach (var it in listBox.CheckedItems)
                {
                    var type = it.GetType();
                    var prop = type.GetProperty("Value");
                    string v = prop?.GetValue(it)?.ToString() ?? "";
                    // 全選択の場合　→　empty value
                    if ((string.IsNullOrEmpty(v) && string.IsNullOrEmpty(targetVal)) || v == targetVal)
                        return true;
                }
                return false;
            }

            // ============ inner CheckedListBox ============
            internal class CustomCheckedListBox : CheckedListBox
            {
                private readonly DropDownWrapper owner;
                private int hoverIndex = -1;

                public CustomCheckedListBox(DropDownWrapper owner)
                {
                    this.owner = owner;
                    this.SelectionMode = SelectionMode.One;
                    this.HorizontalScrollbar = true;
                }

                protected override void OnKeyDown(KeyEventArgs e)
                {
                    if (e.KeyCode == Keys.Enter)
                    {
                        owner.CloseDropdown(true);
                        e.Handled = true;
                    }
                    else if (e.KeyCode == Keys.Escape)
                    {
                        owner.CloseDropdown(false);
                        e.Handled = true;
                    }
                    else if (e.KeyCode == Keys.Delete)
                    {
                        bool check = e.Shift; // Shift+Delete = check all
                        for (int i = 0; i < Items.Count; i++)
                            SetItemChecked(i, check);
                        e.Handled = true;
                    }

                    base.OnKeyDown(e);
                }

                protected override void OnMouseMove(MouseEventArgs e)
                {
                    base.OnMouseMove(e);
                    int idx = IndexFromPoint(e.Location);
                    if (idx >= 0 && idx != hoverIndex)
                    {
                        hoverIndex = idx;
                        SetSelected(idx, true);
                    }
                }
            }
        }
    }
}