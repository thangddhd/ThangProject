
namespace GridviewEx
{
    partial class Form1
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージド リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            coms.COMMON.ui.GridViewStyle gridViewStyle1 = new coms.COMMON.ui.GridViewStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            coms.COMMON.ui.GridViewStyle gridViewStyle2 = new coms.COMMON.ui.GridViewStyle();
            this.dataGridViewEx1 = new coms.COMMON.ui.DataGridViewEx();
            this.Column1 = new System.Windows.Forms.DataGridViewButtonColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Column3 = new coms.COMMON.ui.DataGridViewNumericColumn();
            this.Column4 = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.Column5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column7 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.button1 = new System.Windows.Forms.Button();
            this.dateTimePickerEx31 = new coms.COMMON.ui.DateTimePickerEx3();
            this.dataGridViewEx2 = new coms.COMMON.ui.DataGridViewEx();
            this.Column8 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column9 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column10 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column11 = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.Column12 = new System.Windows.Forms.DataGridViewComboBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewEx1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewEx2)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridViewEx1
            // 
            this.dataGridViewEx1.AllowDrop = true;
            this.dataGridViewEx1.AllowUserToAddRows = false;
            this.dataGridViewEx1.AllowUserToOrderColumns = true;
            this.dataGridViewEx1.AutoGenerateColumns = false;
            this.dataGridViewEx1.ColumnHeadersHeight = 26;
            this.dataGridViewEx1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dataGridViewEx1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2,
            this.Column3,
            this.Column4,
            this.Column5,
            this.Column6,
            this.Column7});
            this.dataGridViewEx1.DisabledFilterAll = false;
            this.dataGridViewEx1.DisabledFilterColumns = ((System.Collections.Generic.HashSet<string>)(resources.GetObject("dataGridViewEx1.DisabledFilterColumns")));
            this.dataGridViewEx1.DisabledSortAll = false;
            this.dataGridViewEx1.EnableHeadersVisualStyles = false;
            this.dataGridViewEx1.FilterAndSortEnabled = true;
            this.dataGridViewEx1.FilterStringChangedInvokeBeforeDatasourceUpdate = true;
            this.dataGridViewEx1.FocusedRowHandle = -1;
            this.dataGridViewEx1.IgnoreAutoFormatColumns = ((System.Collections.Generic.HashSet<string>)(resources.GetObject("dataGridViewEx1.IgnoreAutoFormatColumns")));
            this.dataGridViewEx1.KeepFilterAndSort = true;
            this.dataGridViewEx1.ListCellEditorColumnNames = ((System.Collections.Generic.List<string>)(resources.GetObject("dataGridViewEx1.ListCellEditorColumnNames")));
            this.dataGridViewEx1.Location = new System.Drawing.Point(73, 29);
            this.dataGridViewEx1.Margin = new System.Windows.Forms.Padding(2);
            this.dataGridViewEx1.MaxFilterButtonImageHeight = 23;
            this.dataGridViewEx1.Name = "dataGridViewEx1";
            this.dataGridViewEx1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.dataGridViewEx1.RowHeadersVisible = false;
            this.dataGridViewEx1.RowHeadersWidth = 62;
            this.dataGridViewEx1.RowTemplate.Height = 27;
            this.dataGridViewEx1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewEx1.Size = new System.Drawing.Size(769, 247);
            this.dataGridViewEx1.SortAsNumberColumns = ((System.Collections.Generic.HashSet<string>)(resources.GetObject("dataGridViewEx1.SortAsNumberColumns")));
            this.dataGridViewEx1.SortStringChangedInvokeBeforeDatasourceUpdate = true;
            gridViewStyle1.AlternatingRowBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(248)))), ((int)(((byte)(255)))));
            gridViewStyle1.CellBorderColor = System.Drawing.Color.LightGray;
            gridViewStyle1.FocusedCellBorderColor = System.Drawing.Color.DodgerBlue;
            gridViewStyle1.GroupRowBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
            gridViewStyle1.GroupRowTextColor = System.Drawing.Color.Black;
            gridViewStyle1.HoverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(245)))), ((int)(((byte)(255)))));
            gridViewStyle1.RowBackColor = System.Drawing.Color.White;
            gridViewStyle1.RowTextColor = System.Drawing.Color.Black;
            gridViewStyle1.SelectedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(49)))), ((int)(((byte)(106)))), ((int)(((byte)(197)))));
            gridViewStyle1.SelectedTextColor = System.Drawing.Color.White;
            this.dataGridViewEx1.StyleSettings = gridViewStyle1;
            this.dataGridViewEx1.TabIndex = 0;
            this.dataGridViewEx1.UsingRowSelectedStyle = true;
            this.dataGridViewEx1.UsingSeparateRowStyle = true;
            // 
            // Column1
            // 
            this.Column1.DataPropertyName = "Column1";
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle1.Format = "N0";
            this.Column1.DefaultCellStyle = dataGridViewCellStyle1;
            this.Column1.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.Column1.HeaderText = "Column1";
            this.Column1.MinimumWidth = 24;
            this.Column1.Name = "Column1";
            this.Column1.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Column1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.Column1.Width = 150;
            // 
            // Column2
            // 
            this.Column2.DataPropertyName = "Column2";
            this.Column2.HeaderText = "Column2";
            this.Column2.MinimumWidth = 24;
            this.Column2.Name = "Column2";
            this.Column2.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Column2.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.Column2.Width = 150;
            // 
            // Column3
            // 
            this.Column3.AllowDecimal = false;
            this.Column3.DataPropertyName = "Column3";
            dataGridViewCellStyle2.Format = "yyyy-MM-dd";
            this.Column3.DefaultCellStyle = dataGridViewCellStyle2;
            this.Column3.HeaderText = "Column3";
            this.Column3.IgnoreFormat = false;
            this.Column3.MinimumWidth = 24;
            this.Column3.Name = "Column3";
            this.Column3.ReadOnly = true;
            this.Column3.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Column3.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.Column3.Width = 150;
            // 
            // Column4
            // 
            this.Column4.DataPropertyName = "UnitCode";
            this.Column4.HeaderText = "Column4";
            this.Column4.MinimumWidth = 24;
            this.Column4.Name = "Column4";
            this.Column4.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Column4.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.Column4.Width = 150;
            // 
            // Column5
            // 
            this.Column5.DataPropertyName = "Column5";
            this.Column5.HeaderText = "Column5";
            this.Column5.MinimumWidth = 24;
            this.Column5.Name = "Column5";
            this.Column5.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.Column5.Width = 150;
            // 
            // Column6
            // 
            this.Column6.DataPropertyName = "Column6";
            this.Column6.HeaderText = "Column6";
            this.Column6.MinimumWidth = 24;
            this.Column6.Name = "Column6";
            this.Column6.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.Column6.Width = 150;
            // 
            // Column7
            // 
            this.Column7.DataPropertyName = "Column7";
            this.Column7.HeaderText = "Column7";
            this.Column7.MinimumWidth = 24;
            this.Column7.Name = "Column7";
            this.Column7.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.Column7.Width = 150;
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePicker1.Location = new System.Drawing.Point(867, 68);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(104, 19);
            this.dateTimePicker1.TabIndex = 1;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(996, 64);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // dateTimePickerEx31
            // 
            this.dateTimePickerEx31.DisplayBackColor = System.Drawing.SystemColors.Window;
            this.dateTimePickerEx31.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dateTimePickerEx31.Location = new System.Drawing.Point(867, 106);
            this.dateTimePickerEx31.Name = "dateTimePickerEx31";
            this.dateTimePickerEx31.Size = new System.Drawing.Size(95, 19);
            this.dateTimePickerEx31.TabIndex = 3;
            this.dateTimePickerEx31.Value = new System.DateTime(2026, 5, 22, 13, 40, 48, 884);
            this.dateTimePickerEx31.Value2 = new System.DateTime(2026, 5, 22, 13, 40, 48, 884);
            this.dateTimePickerEx31.ValueChanged += new System.EventHandler(this.dateTimePickerEx31_ValueChanged);
            // 
            // dataGridViewEx2
            // 
            this.dataGridViewEx2.AllowDrop = true;
            this.dataGridViewEx2.AllowUserToAddRows = false;
            this.dataGridViewEx2.AllowUserToOrderColumns = true;
            this.dataGridViewEx2.AutoGenerateColumns = false;
            this.dataGridViewEx2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewEx2.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column8,
            this.Column9,
            this.Column10,
            this.Column11,
            this.Column12});
            this.dataGridViewEx2.DisabledFilterAll = false;
            this.dataGridViewEx2.DisabledFilterColumns = ((System.Collections.Generic.HashSet<string>)(resources.GetObject("dataGridViewEx2.DisabledFilterColumns")));
            this.dataGridViewEx2.DisabledSortAll = false;
            this.dataGridViewEx2.EnableHeadersVisualStyles = false;
            this.dataGridViewEx2.FilterAndSortEnabled = true;
            this.dataGridViewEx2.FilterStringChangedInvokeBeforeDatasourceUpdate = true;
            this.dataGridViewEx2.FocusedRowHandle = -1;
            this.dataGridViewEx2.IgnoreAutoFormatColumns = ((System.Collections.Generic.HashSet<string>)(resources.GetObject("dataGridViewEx2.IgnoreAutoFormatColumns")));
            this.dataGridViewEx2.KeepFilterAndSort = true;
            this.dataGridViewEx2.ListCellEditorColumnNames = ((System.Collections.Generic.List<string>)(resources.GetObject("dataGridViewEx2.ListCellEditorColumnNames")));
            this.dataGridViewEx2.Location = new System.Drawing.Point(173, 408);
            this.dataGridViewEx2.MaxFilterButtonImageHeight = 23;
            this.dataGridViewEx2.Name = "dataGridViewEx2";
            this.dataGridViewEx2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.dataGridViewEx2.RowHeadersVisible = false;
            this.dataGridViewEx2.RowTemplate.Height = 21;
            this.dataGridViewEx2.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewEx2.Size = new System.Drawing.Size(775, 150);
            this.dataGridViewEx2.SortAsNumberColumns = ((System.Collections.Generic.HashSet<string>)(resources.GetObject("dataGridViewEx2.SortAsNumberColumns")));
            this.dataGridViewEx2.SortStringChangedInvokeBeforeDatasourceUpdate = true;
            gridViewStyle2.AlternatingRowBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            gridViewStyle2.CellBorderColor = System.Drawing.Color.LightGray;
            gridViewStyle2.FocusedCellBorderColor = System.Drawing.Color.DodgerBlue;
            gridViewStyle2.GroupRowBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
            gridViewStyle2.GroupRowTextColor = System.Drawing.Color.Black;
            gridViewStyle2.HoverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(245)))), ((int)(((byte)(255)))));
            gridViewStyle2.RowBackColor = System.Drawing.Color.White;
            gridViewStyle2.RowTextColor = System.Drawing.Color.Black;
            gridViewStyle2.SelectedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(153)))), ((int)(((byte)(255)))));
            gridViewStyle2.SelectedTextColor = System.Drawing.Color.White;
            this.dataGridViewEx2.StyleSettings = gridViewStyle2;
            this.dataGridViewEx2.TabIndex = 4;
            // 
            // Column8
            // 
            this.Column8.DataPropertyName = "Column8";
            this.Column8.HeaderText = "Column8";
            this.Column8.MinimumWidth = 24;
            this.Column8.Name = "Column8";
            this.Column8.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            // 
            // Column9
            // 
            this.Column9.DataPropertyName = "Column9";
            this.Column9.HeaderText = "Column9";
            this.Column9.MinimumWidth = 24;
            this.Column9.Name = "Column9";
            this.Column9.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            // 
            // Column10
            // 
            this.Column10.DataPropertyName = "Column10";
            this.Column10.HeaderText = "Column10";
            this.Column10.MinimumWidth = 24;
            this.Column10.Name = "Column10";
            this.Column10.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            // 
            // Column11
            // 
            this.Column11.DataPropertyName = "Column11";
            this.Column11.HeaderText = "Column11";
            this.Column11.MinimumWidth = 24;
            this.Column11.Name = "Column11";
            this.Column11.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Column11.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            // 
            // Column12
            // 
            this.Column12.DataPropertyName = "Column12";
            this.Column12.HeaderText = "Column12";
            this.Column12.MinimumWidth = 24;
            this.Column12.Name = "Column12";
            this.Column12.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Column12.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1154, 679);
            this.Controls.Add(this.dataGridViewEx2);
            this.Controls.Add(this.dateTimePickerEx31);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.dateTimePicker1);
            this.Controls.Add(this.dataGridViewEx1);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewEx1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewEx2)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private coms.COMMON.ui.DataGridViewEx dataGridViewEx1;
        private System.Windows.Forms.DateTimePicker dateTimePicker1;
        private System.Windows.Forms.Button button1;
        private coms.COMMON.ui.DateTimePickerEx3 dateTimePickerEx31;
        private System.Windows.Forms.DataGridViewButtonColumn Column1;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Column2;
        private coms.COMMON.ui.DataGridViewNumericColumn Column3;
        private System.Windows.Forms.DataGridViewComboBoxColumn Column4;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column5;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column6;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column7;
        private coms.COMMON.ui.DataGridViewEx dataGridViewEx2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column8;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column9;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column10;
        private System.Windows.Forms.DataGridViewComboBoxColumn Column11;
        private System.Windows.Forms.DataGridViewComboBoxColumn Column12;
    }
}

