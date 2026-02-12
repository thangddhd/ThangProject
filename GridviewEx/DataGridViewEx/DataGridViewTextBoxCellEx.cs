using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using System;

namespace coms.COMMON.ui
{
    public class DataGridViewTextBoxCellEx : DataGridViewTextBoxCell, ISpannedCell
    {
        #region Fields
        private int m_ColumnSpan = 1;
        private int m_RowSpan = 1;
        private DataGridViewTextBoxCellEx m_OwnerCell;
        
        #endregion

        #region Properties

        public int ColumnSpan
        {
            get { return m_ColumnSpan; }
            set
            {
                if (DataGridView == null || m_OwnerCell != null)
                    return;
                if (value < 1 || ColumnIndex + value - 1 >= DataGridView.ColumnCount)
                    throw new System.ArgumentOutOfRangeException("value");
                if (m_ColumnSpan != value)
                    SetSpan(value, m_RowSpan);
            }
        }

        public int RowSpan
        {
            get { return m_RowSpan; }
            set
            {
                if (DataGridView == null || m_OwnerCell != null)
                    return;
                if (value < 1 || RowIndex + value - 1 >= DataGridView.RowCount)
                    throw new System.ArgumentOutOfRangeException("value");
                if (m_RowSpan != value)
                    SetSpan(m_ColumnSpan, value);
            }
        }

        public DataGridViewCell OwnerCell
        {
            get { return m_OwnerCell; }
            private set { m_OwnerCell = value as DataGridViewTextBoxCellEx; }
        }

        public override bool ReadOnly
        {
            get
            {
                return base.ReadOnly;
            }
            set
            {
                base.ReadOnly = value;

                if (m_OwnerCell == null
                    && (m_ColumnSpan > 1 || m_RowSpan > 1)
                    && DataGridView != null)
                {
                    foreach (var col in Enumerable.Range(ColumnIndex, m_ColumnSpan))
                        foreach (var row in Enumerable.Range(RowIndex, m_RowSpan))
                            if (col != ColumnIndex || row != RowIndex)
                            {
                                DataGridView[col, row].ReadOnly = value;
                            }
                }
            }
        }
        #endregion

        #region Overridden Editing.
        protected override object GetValue(int rowIndex)
        {
            object myValue = null;
            if (m_OwnerCell != null)
                myValue = m_OwnerCell.GetValue(m_OwnerCell.RowIndex);
            myValue = base.GetValue(rowIndex);

            if (this.DataGridView is DataGridViewEx customGrid)
            {
                var args = new CustomColumnDataEventArgs(
                    rowIndex,
                    this.OwningColumn.Name,
                    myValue, 
                    isGetData: true,
                    isSetData: false
                );
                // call user function 
                customGrid.OnCustomColumnData(args);
                // user is inputed value
                if (args.Handled)
                {
                    return args.Value;
                }
            }

            return myValue;
        }

        protected override bool SetValue(int rowIndex, object value)
        {
            if (this.DataGridView is DataGridViewEx customGrid)
            {
                var args = new CustomColumnDataEventArgs(
                    rowIndex,
                    this.OwningColumn.Name,
                    value, 
                    isGetData: false,
                    isSetData: true
                );

                customGrid.OnCustomColumnData(args);

                if (args.Handled)
                {
                    return true; //finished with no error
                }
            }
            // if user not inputed value 
            if (m_OwnerCell != null)
                return m_OwnerCell.SetValue(m_OwnerCell.RowIndex, value);
            return base.SetValue(rowIndex, value);
        }

        protected override object GetFormattedValue(object value,
                                                int rowIndex,
                                                ref DataGridViewCellStyle cellStyle,
                                                System.ComponentModel.TypeConverter valueTypeConverter,
                                                System.ComponentModel.TypeConverter formattedValueTypeConverter,
                                                DataGridViewDataErrorContexts context)
        {
            object baseFormattedValue = base.GetFormattedValue(value, rowIndex,
                                                                ref cellStyle,
                                                                valueTypeConverter,
                                                                formattedValueTypeConverter,
                                                                context);

            if (this.DataGridView is DataGridViewEx customGrid)
            {
                var args = new CustomColumnDisplayTextEventArgs(rowIndex, value)
                {
                    ColumnName = this.OwningColumn.Name,
                    Column = this.OwningColumn,
                    DisplayText = baseFormattedValue?.ToString() ?? string.Empty
                };
                //表示テキストする際にカスタイマイズが必要ならば関数を指定する
                //gvKumiai_CustomColumnDisplayText関数に参考
                customGrid.OnCustomColumnDisplayText(args);

                if (args.Handled)
                {
                    return args.DisplayText;
                }
            }

            return baseFormattedValue;
        }

        #endregion

        #region Private Methods
        
        #endregion

        #region Merge Cells
        private void SetSpan(int columnSpan, int rowSpan)
        {
            int prevColumnSpan = m_ColumnSpan;
            int prevRowSpan = m_RowSpan;
            m_ColumnSpan = columnSpan;
            m_RowSpan = rowSpan;

            if (DataGridView != null)
            {
                // clear.
                foreach (int rowIndex in Enumerable.Range(RowIndex, prevRowSpan))
                    foreach (int columnIndex in Enumerable.Range(ColumnIndex, prevColumnSpan))
                    {
                        var cell = DataGridView[columnIndex, rowIndex] as DataGridViewTextBoxCellEx;
                        if (cell != null)
                            cell.OwnerCell = null;
                    }

                // set.
                foreach (int rowIndex in Enumerable.Range(RowIndex, m_RowSpan))
                    foreach (int columnIndex in Enumerable.Range(ColumnIndex, m_ColumnSpan))
                    {
                        var cell = DataGridView[columnIndex, rowIndex] as DataGridViewTextBoxCellEx;
                        if (cell != null && cell != this)
                        {
                            if (cell.ColumnSpan > 1) cell.ColumnSpan = 1;
                            if (cell.RowSpan > 1) cell.RowSpan = 1;
                            cell.OwnerCell = this;
                        }
                    }

                OwnerCell = null;
                //DataGridView.Invalidate();
            }
        }
        private bool CellsRegionContainsSelectedCell(int columnIndex, int rowIndex, int columnSpan, int rowSpan)
        {
            if (DataGridView == null)
                return false;

            return (from col in Enumerable.Range(columnIndex, columnSpan)
                    from row in Enumerable.Range(rowIndex, rowSpan)
                    where DataGridView[col, row].Selected
                    select col).Any();
        }

        #endregion
    }

    public class DataGridViewTextBoxColumnEx : DataGridViewTextBoxColumn
    {
        #region property
        private bool m_IsRowSpan = false;
        private bool m_IsColSpan = false;

        [Category("Custom Span")]
        [Description("行結合可")]
        [DefaultValue(false)]
        public bool IsRowSpan
        {
            get { return m_IsRowSpan; }
            set { m_IsRowSpan = value; }
        }
        [Category("Custom Span")]
        [Description("列結合可")]
        [DefaultValue(false)]
        public bool IsColSpan
        {
            get { return m_IsColSpan; }
            set { m_IsColSpan = value; }
        }
        #endregion

        #region ctor
        public DataGridViewTextBoxColumnEx() : base()
        {
            this.CellTemplate = new DataGridViewTextBoxCellEx();
        }
        #endregion

        public Action<DataGridViewColumn> OnRunSomething;

        [Category("Custom Action")]
        [Description("IsRunSomeThingプロパティがTRUEに設定された際に、コールバック関数をトリガーします。")]
        public bool IsRunSomeThing
        {
            get { return false; }
            set
            {
                if (value == true)
                {
                    OnRunSomething?.Invoke(this);
                }
            }
        }

        
    }
}
