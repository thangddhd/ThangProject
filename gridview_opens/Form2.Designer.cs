
namespace gridview_opens
{
    partial class Form2
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
            gridview_opens.controls.GridViewStyle gridViewStyle2 = new gridview_opens.controls.GridViewStyle();
            this.groupableDataGridView1 = new gridview_opens.controls.GroupableDataGridView();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.groupableDataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // groupableDataGridView1
            // 
            this.groupableDataGridView1.AllowDrop = true;
            this.groupableDataGridView1.AllowUserToAddRows = false;
            this.groupableDataGridView1.AllowUserToOrderColumns = true;
            this.groupableDataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.groupableDataGridView1.EnableHeadersVisualStyles = false;
            this.groupableDataGridView1.FilterAndSortEnabled = true;
            this.groupableDataGridView1.FilterStringChangedInvokeBeforeDatasourceUpdate = true;
            this.groupableDataGridView1.Location = new System.Drawing.Point(26, 124);
            this.groupableDataGridView1.MaxFilterButtonImageHeight = 23;
            this.groupableDataGridView1.Name = "groupableDataGridView1";
            this.groupableDataGridView1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.groupableDataGridView1.RowHeadersVisible = false;
            this.groupableDataGridView1.RowHeadersWidth = 62;
            this.groupableDataGridView1.RowTemplate.Height = 27;
            this.groupableDataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.groupableDataGridView1.Size = new System.Drawing.Size(1327, 877);
            this.groupableDataGridView1.SortStringChangedInvokeBeforeDatasourceUpdate = true;
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
            this.groupableDataGridView1.StyleSettings = gridViewStyle2;
            this.groupableDataGridView1.TabIndex = 0;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(548, 73);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(115, 22);
            this.checkBox1.TabIndex = 1;
            this.checkBox1.Text = "checkBox1";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1376, 1013);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.groupableDataGridView1);
            this.Name = "Form2";
            this.Text = "Form2";
            ((System.ComponentModel.ISupportInitialize)(this.groupableDataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private controls.GroupableDataGridView groupableDataGridView1;
        private System.Windows.Forms.CheckBox checkBox1;
    }
}