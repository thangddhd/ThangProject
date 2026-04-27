using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace coms.COMMON.ui
{
    public class GridViewStyle
    {
        public Color RowBackColor { get; set; } = Color.White;
        public Color AlternatingRowBackColor { get; set; } = Color.FromArgb(245, 245, 245);
        public Color RowTextColor { get; set; } = Color.Black;

        public Color SelectedBackColor { get; set; } = Color.FromArgb(51, 153, 255);
        public Color SelectedTextColor { get; set; } = Color.White;

        public Color GroupRowBackColor { get; set; } = Color.FromArgb(230, 230, 230);
        public Color GroupRowTextColor { get; set; } = Color.Black;

        public Color HoverBackColor { get; set; } = Color.FromArgb(235, 245, 255);

        public Color CellBorderColor { get; set; } = Color.LightGray;
        public Color FocusedCellBorderColor { get; set; } = Color.DodgerBlue;
    }

    public class GroupRowInfo
    {
        public string GroupKey { get; set; }
        public int GroupHeaderRowIndex { get; set; }
        public List<int> MemberRowIndexes { get; set; } = new List<int>();
        public bool Collapsed { get; set; } = false;
    }

    /// <summary>
    /// 1. Event Arguments
    /// </summary>
    public class CellMergeEventArgs : EventArgs
    {
        public DataGridView Grid { get; private set; }
        public DataGridViewColumn Column { get; private set; }
        public int ColumnIndex { get; private set; }
        public int RowIndex1 { get; private set; }  // RowHandle1
        public int RowIndex2 { get; private set; }  // RowHandle2

        public object CellValue1 { get; private set; }
        public object CellValue2 { get; private set; }

        /// <summary> true/false: merge cell or no. </summary>
        public bool Merge { get; set; }

        /// <summary> true: Merge value will be used </summary>
        public bool Handled { get; set; }

        public CellMergeEventArgs(DataGridViewColumn column, int columnIndex, int rowIndex1, int rowIndex2)
        {
            Column = column;
            ColumnIndex = columnIndex;
            RowIndex1 = rowIndex1;
            RowIndex2 = rowIndex2;
        }

        public CellMergeEventArgs(int rowIndex1, int rowIndex2, int columnIndex, object value1, object value2)
        {
            this.RowIndex1 = rowIndex1;
            this.RowIndex2 = rowIndex2;
            this.ColumnIndex = columnIndex;
            this.CellValue1 = value1;
            this.CellValue2 = value2;
            this.Merge = false;   //default: do not merge
            this.Handled = false; //not yet checked
        }

        public CellMergeEventArgs(DataGridView grid, int colIndex, int rowIndex1, int rowIndex2)
        {
            Grid = grid;
            ColumnIndex = colIndex;
            Column = grid.Columns[colIndex];

            RowIndex1 = rowIndex1;
            RowIndex2 = rowIndex2;

            CellValue1 = grid.Rows[rowIndex1].Cells[colIndex].Value;
            CellValue2 = grid.Rows[rowIndex2].Cells[colIndex].Value;

            Merge = false;
            Handled = false;
        }

        public CellMergeEventArgs(DataGridView grid, DataGridViewColumn column, int rowIndex1, int rowIndex2)
        {
            this.Grid = grid;
            this.ColumnIndex = column.Index;
            this.Column = column;

            this.RowIndex1 = rowIndex1;
            this.RowIndex2 = rowIndex2;

            this.CellValue1 = grid.Rows[rowIndex1].Cells[ColumnIndex].Value;
            this.CellValue2 = grid.Rows[rowIndex2].Cells[ColumnIndex].Value;

            this.Merge = false;      //default: do not merge
            this.Handled = false;    //not yet checked
        }
    }

    public class ButtonIconNeededEventArgs : EventArgs
    {
        public DataGridViewColumn Column { get; }
        public DataGridViewRow Row { get; }
        public object DataBoundItem { get; }
        public Image Icon { get; set; }
        public string Text { get; set; }
        public bool Enabled { get; set; } = true;
        public Color BackColor { get; set; } = Color.Empty;
        public Color ForeColor { get; set; } = Color.Empty;
        public int ColumnIndex { get; private set; }
        public int RowIndex { get; set; }
        public ButtonIconNeededEventArgs(DataGridViewColumn column, DataGridViewRow row, object dataBoundItem)
        {
            Column = column;
            Row = row;
            DataBoundItem = dataBoundItem;
            ColumnIndex = column.Index;
            RowIndex = row.Index;
        }
    }

    public class CellBackColorNeededEventArgs : EventArgs
    {
        public DataGridViewColumn Column { get; }
        public DataGridViewRow Row { get; }
        public object DataBoundItem { get; }
        public Color BackColor { get; set; }

        public CellBackColorNeededEventArgs(DataGridViewColumn column, DataGridViewRow row, object dataBoundItem, Color currentColor)
        {
            Column = column;
            Row = row;
            DataBoundItem = dataBoundItem;
            BackColor = currentColor;
        }
    }

    public class RowBackColorNeededEventArgs : EventArgs
    {
        public DataGridViewRow Row { get; }
        public object DataBoundItem { get; }
        public Color BackColor { get; set; }

        public RowBackColorNeededEventArgs(DataGridViewRow row, object dataBoundItem, Color currentColor)
        {
            Row = row;
            DataBoundItem = dataBoundItem;
            BackColor = currentColor;
        }
    }

    public class TextBoxIconNeededEventArgs : EventArgs
    {
        public DataGridViewColumn Column { get; }
        public DataGridViewRow Row { get; }
        public object DataBoundItem { get; }

        public Image Icon { get; set; }
        public int IconSize { get; set; } = 16;
        /// <summary>
        /// クリアしたい場合はstring.empty
        /// null の場合処理無し
        /// </summary>
        public string TextValue { get; set; } = null;
        // 変更ない場合NULL
        public bool? ReadOnly { get; set; } = null;

        public TextBoxIconNeededEventArgs(DataGridViewColumn column, DataGridViewRow row, object dataBoundItem)
        {
            Column = column;
            Row = row;
            DataBoundItem = dataBoundItem;
        }
    }

    public class ColumnEditEventArgs : EventArgs
    {
        public DataGridViewColumn Column { get; }
        public DataGridViewRow Row { get; }
        public object DataBoundItem { get; }
        /// <summary>
        /// 元々はリンクですがリンクではなく別のカラムタイプとして表示する場合
        /// </summary>
        public GridColumnType EditType { get; set; } = GridColumnType.None;
        public int ColumnIndex { get; private set; }
        public int RowIndex { get; set; }
        public ColumnEditEventArgs(DataGridViewColumn column, DataGridViewRow row, object dataBoundItem)
        {
            Column = column;
            Row = row;
            DataBoundItem = dataBoundItem;
            ColumnIndex = column.Index;
            RowIndex = row.Index;
        }
    }

    public class DisplayRow
    {
        public bool IsGroup { get; set; }
        public bool Collapsed { get; set; }
        public string GroupKey { get; set; }
        public object DataItem { get; set; } // DataRow, DataRowView hoặc object T
    }

    public class CollapsedGroupCache
    {
        public string GroupKey { get; set; }
        public List<DisplayRow> CachedRows { get; set; } = new List<DisplayRow>();
    }

    public enum GridColumnType
    {
        None = 0,
        Button = 1,
        CheckBox = 2,
        ComboBox = 3,
        Link = 4,
        TextBox = 5
    }

    public class ComboboxColumnEditEventArgs : EventArgs
    {
        public DataGridViewColumn Column { get; }
        public DataGridViewRow Row { get; }
        public object DataBoundItem { get; }

        // NEW:
        public bool Enabled { get; set; } = true;
        public Color BackColor { get; set; } = Color.Empty;

        // データソース
        public object DataSource { get; set; } = null;
        public string DisplayMember { get; set; } = null;
        public string ValueMember { get; set; } = null;

        // テキスト変えたい場合
        public string TextValue { get; set; } = null;

        public GridColumnType EditType { get; set; } = GridColumnType.None;

        public ComboboxColumnEditEventArgs(DataGridViewColumn column, DataGridViewRow row, object dataBoundItem)
        {
            Column = column;
            Row = row;
            DataBoundItem = dataBoundItem;
        }
    }












    //----------------------------------------------
    public class CellGroup
    {
        public int GroupId { get; set; }
        public string ColumnName { get; set; }
        public int StartRowIndex { get; set; }
        public int EndRowIndex { get; set; }
        public object Value { get; set; }

        public bool IsMergeable => EndRowIndex > StartRowIndex;
        public int CellCount
        {
            get
            {
                int c = EndRowIndex - StartRowIndex + 1;
                return c < 0 ? 0 : c;
            }
        }
        public string KeyColumnName { get; set; }
        public object KeyValue { get; set; }

    }

    public class CustomRowCellEventArgs : EventArgs
    {
        public DataGridView Grid { get; private set; }
        //(dexExpress: RepositoryItem) Current Column 
        public DataGridViewColumn Column { get; set; }
        // (dexExpress: RowHandle)
        public int RowIndex { get; set; }
        public int ColumnIndex { get; set; }
        //(DevExpress: CellValue)
        public object Value { get; set; }
        public string ColumnName { get; set; }
        public string DisplayText { get; set; }
        //Finished Flag
        public bool Handled { get; set; }
        //public CustomRowCellEventArgs(int rowIndex, DataGridViewColumn column)
        //{
        //    RowIndex = rowIndex;
        //    Column = column;
        //}
    }

    public class CustomColumnDisplayTextEventArgs : CustomRowCellEventArgs
    {
        public CustomColumnDisplayTextEventArgs(int rowIndex, object value)
        {
            RowIndex = rowIndex;
            Value = value;
            DisplayText = value?.ToString() ?? string.Empty;
        }

        public CustomColumnDisplayTextEventArgs(int col, int row, object val, DataGridViewColumn column)
        {
            ColumnIndex = col;
            RowIndex = row;
            Value = val;
            Column = column;
            DisplayText = val?.ToString();
        }
    }

    // Editor Template (DevExpress: RepositoryItemButtonEdit)
    public class CustomButtonEditorTemplate
    {
        // Icon/Image button
        public Image ButtonIcon { get; set; }

        // Button click
        public event EventHandler ButtonClick;

        public CustomButtonEditorTemplate(Image icon)
        {
            this.ButtonIcon = icon;
        }

        // Editor 
        public void Draw(Graphics g, Rectangle bounds)
        {
            // Button Icon
            using (var brush = new SolidBrush(Color.LightGray))
            {
                g.FillRectangle(brush, bounds);
            }
            if (ButtonIcon != null)
            {
                // Icon 
                g.DrawImage(ButtonIcon, bounds.Location);
            }
        }
    }

    public class CustomRowCellEditEventArgs : CustomRowCellEventArgs
    {
        //Xtragrid: RepositoryItem
        public DataGridViewCell CellEditor { get; set; }
        public object DataBoundItem { get; }
        public bool ReadOnly { get; set; } = false;
        //public bool Enabled { get; set; } = true;
        // Event
        public CustomRowCellEditEventArgs(int rowIndex, object value, DataGridViewColumn currentColumn)
        {
            this.RowIndex = rowIndex;
            this.ColumnName = currentColumn.Name;
            this.Value = value;
            this.Column = currentColumn;
            this.Handled = false;
        }

        public CustomRowCellEditEventArgs(int rowIndex, DataGridViewColumn currentColumn, DataGridViewCell currentCell, object dataBoundItem)
        {
            RowIndex = rowIndex;
            ColumnIndex = currentColumn.Index;
            ColumnName = currentColumn.Name;
            Column = currentColumn;
            CellEditor = currentCell;
            DataBoundItem = dataBoundItem;
            ReadOnly = currentCell.ReadOnly;
            this.Handled = false;
        }

    }

    public class CustomColumnDataEventArgs : EventArgs
    {
        public DataGridView Grid { get; private set; }
        //(dexExpress: RepositoryItem) Current Column 
        public DataGridViewColumn Column { get; set; }
        // (dexExpress: RowHandle)
        public int RowIndex { get; set; }
        public int ColumnIndex { get; set; }
        //(DevExpress: CellValue)
        public object Value { get; set; }
        public string ColumnName { get; set; }
        public string DisplayText { get; set; }
        //Finished Flag
        public bool Handled { get; set; }
        public bool IsGetData { get; private set; } // Read: True
        public bool IsSetData { get; private set; } // Write: True

        public CustomColumnDataEventArgs(int rowIndex, string columnName, object value, bool isGetData, bool isSetData)
        {
            this.RowIndex = rowIndex;
            this.ColumnName = columnName;
            this.Value = value;
            this.IsGetData = isGetData;
            this.IsSetData = isSetData;
        }

        public CustomColumnDataEventArgs(DataGridView grid, int colIndex, int rowIndex, bool isGetData)
        {
            this.Grid = grid;
            this.Column = grid.Columns[colIndex];
            this.RowIndex = rowIndex;
            this.IsGetData = isGetData;
            this.Handled = false;
        }
    }

    // DevExpress: _RowCellClick
    public class CustomRowCellClickEventArgs : CustomRowCellEventArgs
    {
        public MouseButtons Button { get; private set; }
        public Point Location { get; private set; } // (X, Y)
        public int X { get { return Location.X; } }
        public int Y { get { return Location.Y; } }

        // Thông tin chi tiết về vùng chạm (từ CalcHitInfo)
        public GridHitInfo HitInfo { get; private set; }

        public CustomRowCellClickEventArgs(DataGridViewCellMouseEventArgs e, GridHitInfo hitInfo, DataGridViewColumn column)
        {
            this.RowIndex = e.RowIndex;
            this.ColumnIndex = e.ColumnIndex;
            this.Button = e.Button;
            // e.Location 位置にMouseの位置をマッピング
            this.Location = new Point(e.X, e.Y);
            this.HitInfo = hitInfo;

            this.Column = column;
            this.ColumnName = column.Name;
        }
    }

    // DevExpress: _InitNewRow
    public class CustomInitNewRowEventArgs : CustomRowCellEventArgs
    {
        public CustomInitNewRowEventArgs(int rowIndex)
        {
            this.RowIndex = rowIndex;
        }
    }

    public class CustomCellValueChangedEventArgs : CustomRowCellEventArgs
    {
        public object NewValue { get; private set; }

        public object OldValue { get; private set; }

        public CustomCellValueChangedEventArgs(int rowIndex, int colIndex, object oldValue, object newValue, DataGridViewColumn column)
        {
            this.RowIndex = rowIndex;
            this.ColumnIndex = colIndex;
            this.OldValue = oldValue;
            this.NewValue = newValue;
            this.Column = column;
        }
    }

    //----------------------------------------------

    public enum GridHitTest
    {
        None = 0,
        Column = 1,
        ColumnEdge = 2,
        ColumnButton = 3,
        ColumnFilterButton = 4,
        ColumnPanel = 5,
        RowCell = 6,
        //RowIndicator = 7,
        //RowGroupButton = 8,
        //Row = 9,
        //RowPreview = 10,
        //RowDetail = 11,
        //RowDetailEdge = 12,
        //RowDetailIndicator = 13,
        EmptyRow = 14,
        //GroupPanel = 15,
        //GroupPanelColumn = 16,
        //GroupPanelColumnFilterButton = 17,
        //Footer = 18,
        CellButton = 19,
        //CustomizationForm = 20,
        //FilterPanel = 21,
        //FilterPanelCloseButton = 22,
        //RowFooter = 23,
        //RowEdge = 24,
        //FixedLeftDiv = 25,
        //FixedRightDiv = 26,
        //VScrollBar = 27,
        //HScrollBar = 28,
        //FilterPanelActiveButton = 29,
        //FilterPanelText = 30,
        //FilterPanelMRUButton = 31,
        //FilterPanelCustomizeButton = 32,
        //ViewCaption = 33,
        //MasterTabPageHeader = 34,
        RowHeader = 35,
        ColumnHeader = 36,
        TopLeftHeader = 37,
        Scrollbar = 38
    }

    public class GridHitInfo
    {
        public GridHitTest HitType { get; set; }
        public int RowIndex { get; set; }
        public int ColumnIndex { get; set; }
        public string Description { get; set; } // Mô tả chi tiết

        public GridHitInfo()
        {
            HitType = GridHitTest.None;
            RowIndex = -1;
            ColumnIndex = -1;
        }
    }
}
