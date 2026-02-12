
using System.Windows.Forms;

namespace gridview_opens
{
    partial class Form3
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
            this.customDataGridView1 = new gridview_opens.controls.CustomDataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.customDataGridView1)).BeginInit();
            this.SuspendLayout();
            this.truong = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lop = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ten = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.toan = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tienganh = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.rank = new System.Windows.Forms.DataGridViewTextBoxColumn();
            // 
            // customDataGridView1
            // 
            this.customDataGridView1.AllowUserToOrderColumns = true;
            this.customDataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.customDataGridView1.Location = new System.Drawing.Point(12, 41);
            this.customDataGridView1.Name = "customDataGridView1";
            this.customDataGridView1.RowHeadersWidth = 62;
            this.customDataGridView1.RowTemplate.Height = 27;
            this.customDataGridView1.Size = new System.Drawing.Size(645, 504);
            this.customDataGridView1.TabIndex = 0;
            this.customDataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.truong,
            this.lop,
            this.ten,
            this.toan,
            this.tienganh,
            this.rank});
            // 
            // truong
            // 
            this.truong.DataPropertyName = "SchoolName";
            this.truong.HeaderText = "truong";
            this.truong.MinimumWidth = 24;
            this.truong.Name = "truong";
            this.truong.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.truong.Width = 150;
            // 
            // lop
            // 
            this.lop.DataPropertyName = "ClassName";
            this.lop.HeaderText = "lop";
            this.lop.MinimumWidth = 24;
            this.lop.Name = "lop";
            this.lop.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.lop.Width = 150;
            // 
            // ten
            // 
            this.ten.DataPropertyName = "Name";
            this.ten.HeaderText = "ten";
            this.ten.MinimumWidth = 24;
            this.ten.Name = "ten";
            this.ten.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.ten.Width = 150;
            // 
            // toan
            // 
            this.toan.DataPropertyName = "MathScore";
            this.toan.HeaderText = "toan";
            this.toan.MinimumWidth = 24;
            this.toan.Name = "toan";
            this.toan.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.toan.Width = 150;
            // 
            // tienganh
            // 
            this.tienganh.DataPropertyName = "EnglishScore";
            this.tienganh.HeaderText = "tienganh";
            this.tienganh.MinimumWidth = 24;
            this.tienganh.Name = "tienganh";
            this.tienganh.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.tienganh.Width = 150;
            // 
            // rank
            // 
            this.rank.DataPropertyName = "Rank";
            this.rank.HeaderText = "rank";
            this.rank.MinimumWidth = 24;
            this.rank.Name = "rank";
            this.rank.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.rank.Width = 150;
            // 
            // Form3
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1110, 791);
            this.Controls.Add(this.customDataGridView1);
            this.Name = "Form3";
            this.Text = "Form3";
            ((System.ComponentModel.ISupportInitialize)(this.customDataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private controls.CustomDataGridView customDataGridView1;
        private DataGridViewTextBoxColumn truong;
        private DataGridViewTextBoxColumn lop;
        private DataGridViewTextBoxColumn ten;
        private DataGridViewTextBoxColumn toan;
        private DataGridViewTextBoxColumn tienganh;
        private DataGridViewTextBoxColumn rank;
    }
}