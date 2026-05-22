
namespace GridviewEx
{
    partial class Form4
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.panel1 = new System.Windows.Forms.Panel();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tabControl1 = new coms.COMMON.ui.TabControlEx();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.button1 = new System.Windows.Forms.Button();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.reserveGridView1 = new coms.COMMON.ui.ReserveGridView();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column7 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column8 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.checkedComboBox1 = new coms.COMMON.ui.CheckedComboBox();
            this.dateTimePickerEx31 = new coms.COMMON.ui.DateTimePickerEx3();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.reserveGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.dataGridView1);
            this.panel1.Location = new System.Drawing.Point(64, 75);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(873, 87);
            this.panel1.TabIndex = 0;
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToResizeRows = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.ColumnHeadersVisible = false;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1});
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Left;
            this.dataGridView1.Location = new System.Drawing.Point(0, 0);
            this.dataGridView1.MinimumSize = new System.Drawing.Size(1980, 0);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.RowTemplate.Height = 21;
            this.dataGridView1.Size = new System.Drawing.Size(1980, 87);
            this.dataGridView1.TabIndex = 0;
            // 
            // Column1
            // 
            this.Column1.HeaderText = "Column1";
            this.Column1.Name = "Column1";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.tabControl1.ItemSize = new System.Drawing.Size(60, 18);
            this.tabControl1.Location = new System.Drawing.Point(64, 365);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(342, 150);
            this.tabControl1.TabIndex = 2;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.button1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(334, 124);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(48, 46);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.checkBox1);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(334, 124);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(53, 48);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(80, 16);
            this.checkBox1.TabIndex = 0;
            this.checkBox1.Text = "checkBox1";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.HeaderText = "Column1";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            // 
            // reserveGridView1
            // 
            this.reserveGridView1.AllowUserToAddRows = false;
            this.reserveGridView1.AllowUserToDeleteRows = false;
            this.reserveGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.reserveGridView1.ColumnHeadersVisible = false;
            this.reserveGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column2,
            this.Column3,
            this.Column4,
            this.Column5,
            this.Column6,
            this.Column7,
            this.Column8});
            this.reserveGridView1.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.reserveGridView1.FocusedCellBackColor = null;
            this.reserveGridView1.FocusedCellForeColor = null;
            this.reserveGridView1.FocusedReadOnlyCellBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(244)))));
            this.reserveGridView1.FocusedReadOnlyCellForeColor = null;
            this.reserveGridView1.Location = new System.Drawing.Point(53, 197);
            this.reserveGridView1.MultiSelect = false;
            this.reserveGridView1.Name = "reserveGridView1";
            this.reserveGridView1.RowHeadersVisible = false;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.Black;
            this.reserveGridView1.RowsDefaultCellStyle = dataGridViewCellStyle3;
            this.reserveGridView1.RowTemplate.Height = 21;
            this.reserveGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.reserveGridView1.Size = new System.Drawing.Size(761, 150);
            this.reserveGridView1.TabIndex = 1;
            this.reserveGridView1.CellDisplayTextNeeded += new System.EventHandler<coms.COMMON.ui.ReserveCellDisplayTextNeededEventArgs>(this.reserveGridView1_CellDisplayTextNeeded);
            this.reserveGridView1.CellReadOnlyNeeded += new System.EventHandler<coms.COMMON.ui.ReserveCellReadOnlyNeededEventArgs>(this.reserveGridView1_CellReadOnlyNeeded);
            this.reserveGridView1.CellBeginEditRule += new System.EventHandler<coms.COMMON.ui.ReserveCellBeginEditEventArgs>(this.reserveGridView1_CellBeginEditRule);
            this.reserveGridView1.EditingControlRule += new System.EventHandler<coms.COMMON.ui.ReserveEditingControlShowingEventArgs>(this.reserveGridView1_EditingControlRule);
            this.reserveGridView1.CellStyleNeeded += new System.EventHandler<coms.COMMON.ui.ReserveCellStyleNeededEventArgs>(this.reserveGridView1_CellStyleNeeded);
            this.reserveGridView1.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.reserveGridView1_CellValueChanged);
            this.reserveGridView1.CellValueNeeded += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.reserveGridView1_CellValueNeeded);
            // 
            // Column2
            // 
            this.Column2.DataPropertyName = "Column2";
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.Column2.DefaultCellStyle = dataGridViewCellStyle1;
            this.Column2.HeaderText = "Column2";
            this.Column2.Name = "Column2";
            // 
            // Column3
            // 
            this.Column3.DataPropertyName = "Column3";
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.Column3.DefaultCellStyle = dataGridViewCellStyle2;
            this.Column3.HeaderText = "Column3";
            this.Column3.Name = "Column3";
            // 
            // Column4
            // 
            this.Column4.DataPropertyName = "Column4";
            this.Column4.HeaderText = "Column4";
            this.Column4.Name = "Column4";
            // 
            // Column5
            // 
            this.Column5.DataPropertyName = "Column5";
            this.Column5.HeaderText = "Column5";
            this.Column5.Name = "Column5";
            // 
            // Column6
            // 
            this.Column6.DataPropertyName = "Column6";
            this.Column6.HeaderText = "Column6";
            this.Column6.Name = "Column6";
            // 
            // Column7
            // 
            this.Column7.DataPropertyName = "Column7";
            this.Column7.HeaderText = "Column7";
            this.Column7.Name = "Column7";
            // 
            // Column8
            // 
            this.Column8.DataPropertyName = "Column8";
            this.Column8.HeaderText = "Column8";
            this.Column8.Name = "Column8";
            // 
            // checkedComboBox1
            // 
            this.checkedComboBox1.BackColor = System.Drawing.SystemColors.HotTrack;
            this.checkedComboBox1.CheckOnClick = true;
            this.checkedComboBox1.DropDownHeight = 1;
            this.checkedComboBox1.FormattingEnabled = true;
            this.checkedComboBox1.IntegralHeight = false;
            this.checkedComboBox1.Location = new System.Drawing.Point(476, 396);
            this.checkedComboBox1.MaxDropDownHeight = 200;
            this.checkedComboBox1.Name = "checkedComboBox1";
            this.checkedComboBox1.Size = new System.Drawing.Size(121, 20);
            this.checkedComboBox1.TabIndex = 3;
            this.checkedComboBox1.ValueSeparator = ", ";
            // 
            // dateTimePickerEx31
            // 
            this.dateTimePickerEx31.DisplayBackColor = System.Drawing.SystemColors.Window;
            this.dateTimePickerEx31.Format = System.Windows.Forms.DateTimePickerFormat.Long;
            this.dateTimePickerEx31.Location = new System.Drawing.Point(709, 396);
            this.dateTimePickerEx31.Name = "dateTimePickerEx31";
            this.dateTimePickerEx31.Size = new System.Drawing.Size(105, 19);
            this.dateTimePickerEx31.TabIndex = 4;
            this.dateTimePickerEx31.Value = new System.DateTime(2026, 5, 22, 13, 23, 43, 805);
            this.dateTimePickerEx31.Value2 = new System.DateTime(2026, 5, 22, 13, 23, 43, 805);
            // 
            // Form4
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1091, 518);
            this.Controls.Add(this.dateTimePickerEx31);
            this.Controls.Add(this.checkedComboBox1);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.reserveGridView1);
            this.Controls.Add(this.panel1);
            this.Name = "Form4";
            this.Text = "Form4";
            this.Load += new System.EventHandler(this.Form4_Load);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.reserveGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private coms.COMMON.ui.ReserveGridView reserveGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column4;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column5;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column6;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column7;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column8;
        private coms.COMMON.ui.TabControlEx tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.CheckBox checkBox1;
        private coms.COMMON.ui.CheckedComboBox checkedComboBox1;
        private coms.COMMON.ui.DateTimePickerEx3 dateTimePickerEx31;
    }
}