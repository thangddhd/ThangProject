using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace gridview_opens.controls
{
    public partial class ColumnChooserPopup : Form
    {
        private GroupableDataGridView _ownerGrid;
        private FlowLayoutPanel pnlHiddenColumns;
        public event Action<DataGridViewColumn> ColumnReAddRequested;
        private Point _mouseDownPoint = Point.Empty;
        //private bool _isDraggingFromButton = false;

        public ColumnChooserPopup(GroupableDataGridView grid)
        {
            _ownerGrid = grid;
            InitializeComponent();
            BuildUI();
            PopulateHiddenColumns();

            this.SizeChanged += (s, e) => ResizeAllButtons();
        }

        private void BuildUI()
        {
            this.Text = "Column Chooser";
            this.Size = new Size(260, 400);
            this.FormBorderStyle = FormBorderStyle.SizableToolWindow;
            this.BackColor = Color.WhiteSmoke;

            pnlHiddenColumns = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoScroll = true,
                Padding = new Padding(2),
                Margin = new Padding(0)
            };

            // button's margin
            pnlHiddenColumns.ControlAdded += (s, e) =>
            {
                if (e.Control is Button b) b.Margin = new Padding(1);
            };

            // NOTE: VerticalScroll."force" show; 
            // but we'll account for scrollbar width when sizing buttons to avoid layout jump.
            pnlHiddenColumns.AllowDrop = true;
            pnlHiddenColumns.DragEnter += PnlHiddenColumns_DragEnter;
            pnlHiddenColumns.DragOver += PnlHiddenColumns_DragOver;
            pnlHiddenColumns.DragDrop += PnlHiddenColumns_DragDrop;

            this.Controls.Add(pnlHiddenColumns);
        }

        public void PopulateHiddenColumns()
        {
            pnlHiddenColumns.Controls.Clear();
            foreach (DataGridViewColumn col in _ownerGrid.Columns)
            {
                if (!col.Visible)
                    AddButtonForColumn(col);
            }
            ResizeAllButtons();
        }

        private void AddButtonForColumn(DataGridViewColumn col)
        {
            int buttonWidth = CalculateButtonWidth();

            var btn = new Button
            {
                Text = col.HeaderText,
                Tag = col.Name,
                Width = buttonWidth,
                Height = 30,
                FlatStyle = FlatStyle.Standard,
                BackColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("MS UI Gothic", 9F),
                Cursor = Cursors.Default,
                Margin = new Padding(2),
                ForeColor = SystemColors.ControlText
            };

            //btn.FlatAppearance.BorderColor = Color.Silver;
            //btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(230, 240, 255);
            btn.MouseDown += (s, e) =>
            {
                if (e.Button != MouseButtons.Left) return;

                if (e.Clicks >= 2)
                {
                    var b = s as Button;
                    string colName = b?.Tag as string;
                    if (!string.IsNullOrEmpty(colName) && _ownerGrid.Columns.Contains(colName))
                    {
                        var gridCol = _ownerGrid.Columns[colName];
                        gridCol.Visible = true;

                        _ownerGrid.Columns.Remove(gridCol);
                        _ownerGrid.Columns.Insert(_ownerGrid.Columns.Count, gridCol);

                        ColumnReAddRequested?.Invoke(gridCol);
                        RemoveColumnByName(colName);
                    }
                    // reset mouseDown to avoid starting drag
                    _mouseDownPoint = Point.Empty;
                    //_isDraggingFromButton = false;
                    return;
                }

                _mouseDownPoint = e.Location;
                //_isDraggingFromButton = false;
            };

            btn.MouseMove += (s, e) =>
            {
                if (e.Button != MouseButtons.Left) return;
                if (_mouseDownPoint == Point.Empty) return;

                var dx = Math.Abs(e.X - _mouseDownPoint.X);
                var dy = Math.Abs(e.Y - _mouseDownPoint.Y);
                var dragThreshold = SystemInformation.DragSize;

                if (dx >= dragThreshold.Width || dy >= dragThreshold.Height)
                {
                    var b = s as Button;
                    var colName = b?.Tag as string;
                    if (string.IsNullOrEmpty(colName)) return;

                    var data = new System.Windows.Forms.DataObject();
                    data.SetData(typeof(string), colName);

                    //_isDraggingFromButton = true;

                    var effect = b.DoDragDrop(data, DragDropEffects.Move | DragDropEffects.None);

                    if ((effect & DragDropEffects.Move) == DragDropEffects.Move)
                    {
                        RemoveColumnByName(colName);
                    }

                    // reset flag
                    _mouseDownPoint = Point.Empty;
                    //_isDraggingFromButton = false;
                }
            };

            // MouseUp: reset
            btn.MouseUp += (s, e) =>
            {
                _mouseDownPoint = Point.Empty;
                //_isDraggingFromButton = false;
            };

            // hover styling
            btn.MouseEnter += (s, e) => btn.BackColor = Color.FromArgb(240, 248, 255);
            btn.MouseLeave += (s, e) => btn.BackColor = Color.White;

            pnlHiddenColumns.Controls.Add(btn);
        }

        private int CalculateButtonWidth()
        {
            int sbWidth = SystemInformation.VerticalScrollBarWidth;
            int extra = 10;
            int w = this.ClientSize.Width - sbWidth - extra;
            if (w < 60) w = 60;
            return w;
        }

        private void ResizeAllButtons()
        {
            var width = CalculateButtonWidth();
            foreach (Control c in pnlHiddenColumns.Controls.OfType<Button>())
            {
                c.Width = width;
            }
        }

        private void PnlHiddenColumns_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(DataGridViewColumn)) || e.Data.GetDataPresent(typeof(string)))
                e.Effect = DragDropEffects.Move;
            else
                e.Effect = DragDropEffects.None;
        }

        private void PnlHiddenColumns_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        private void PnlHiddenColumns_DragDrop(object sender, DragEventArgs e)
        {
            string colName = null;
            if (e.Data.GetDataPresent(typeof(string)))
                colName = e.Data.GetData(typeof(string)) as string;
            else if (e.Data.GetDataPresent(typeof(DataGridViewColumn)))
                colName = ((DataGridViewColumn)e.Data.GetData(typeof(DataGridViewColumn))).Name;

            if (string.IsNullOrEmpty(colName)) return;
            if (!_ownerGrid.Columns.Contains(colName)) return;

            var col = _ownerGrid.Columns[colName];

            if (col.Visible)
                col.Visible = false;

            if (!pnlHiddenColumns.Controls.OfType<Button>().Any(b => (string)b.Tag == colName))
                AddButtonForColumn(col);
        }

        public void HandleColumnDroppedToGrid(string columnName)
        {
            if (_ownerGrid.Columns.Contains(columnName))
                _ownerGrid.Columns[columnName].Visible = true;

            RemoveColumnByName(columnName);
        }

        public void RemoveColumnByName(string columnName)
        {
            var btn = pnlHiddenColumns.Controls
                .OfType<Button>()
                .FirstOrDefault(b => (string)b.Tag == columnName);

            if (btn != null)
                pnlHiddenColumns.Controls.Remove(btn);
        }
    }
}