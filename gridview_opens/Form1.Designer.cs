using System;
using System.Drawing;
using System.Windows.Forms;
using gridview_opens.controls;

namespace gridview_opens
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
            gridview_opens.controls.GridViewStyle gridViewStyle1 = new gridview_opens.controls.GridViewStyle();
            this.groupableGrid1 = new gridview_opens.controls.GroupableDataGridView();
            this.truong = new System.Windows.Forms.DataGridViewButtonColumn();
            this.lop = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ten = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.toan = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tienganh = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.rank = new System.Windows.Forms.DataGridViewLinkColumn();
            this.btnGroupByDept = new System.Windows.Forms.Button();
            this.btnClearGroup = new System.Windows.Forms.Button();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.button1 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.groupableGrid1)).BeginInit();
            this.SuspendLayout();
            // 
            // groupableGrid1
            // 
            this.groupableGrid1.AllowDrop = true;
            this.groupableGrid1.AllowUserToAddRows = false;
            this.groupableGrid1.AllowUserToOrderColumns = true;
            this.groupableGrid1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupableGrid1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.groupableGrid1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.truong,
            this.lop,
            this.ten,
            this.toan,
            this.tienganh,
            this.rank});
            this.groupableGrid1.EnableHeadersVisualStyles = false;
            this.groupableGrid1.FilterAndSortEnabled = true;
            this.groupableGrid1.FilterStringChangedInvokeBeforeDatasourceUpdate = true;
            this.groupableGrid1.Location = new System.Drawing.Point(12, 100);
            this.groupableGrid1.MaxFilterButtonImageHeight = 23;
            this.groupableGrid1.Name = "groupableGrid1";
            this.groupableGrid1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.groupableGrid1.RowHeadersVisible = false;
            this.groupableGrid1.RowHeadersWidth = 62;
            this.groupableGrid1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.groupableGrid1.Size = new System.Drawing.Size(1273, 715);
            this.groupableGrid1.SortStringChangedInvokeBeforeDatasourceUpdate = true;
            gridViewStyle1.AlternatingRowBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            gridViewStyle1.CellBorderColor = System.Drawing.Color.LightGray;
            gridViewStyle1.FocusedCellBorderColor = System.Drawing.Color.DodgerBlue;
            gridViewStyle1.GroupRowBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
            gridViewStyle1.GroupRowTextColor = System.Drawing.Color.Black;
            gridViewStyle1.HoverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(245)))), ((int)(((byte)(255)))));
            gridViewStyle1.RowBackColor = System.Drawing.Color.White;
            gridViewStyle1.RowTextColor = System.Drawing.Color.Black;
            gridViewStyle1.SelectedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(153)))), ((int)(((byte)(255)))));
            gridViewStyle1.SelectedTextColor = System.Drawing.Color.White;
            this.groupableGrid1.StyleSettings = gridViewStyle1;
            this.groupableGrid1.TabIndex = 0;
            //this.groupableGrid1.CellMerge += new System.EventHandler<gridview_opens.controls.CellMergeEventArgs>(this.groupableGrid1_CellMerge);
            //this.groupableGrid1.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.groupableGrid1_CellClick);
            // 
            // truong
            // 
            this.truong.DataPropertyName = "SchoolName";
            this.truong.HeaderText = "truong";
            this.truong.MinimumWidth = 24;
            this.truong.Name = "truong";
            this.truong.Resizable = System.Windows.Forms.DataGridViewTriState.True;
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
            this.rank.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.rank.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.rank.Width = 150;
            // 
            // btnGroupByDept
            // 
            this.btnGroupByDept.Location = new System.Drawing.Point(12, 12);
            this.btnGroupByDept.Name = "btnGroupByDept";
            this.btnGroupByDept.Size = new System.Drawing.Size(140, 30);
            this.btnGroupByDept.TabIndex = 1;
            this.btnGroupByDept.Text = "Group by Department";
            this.btnGroupByDept.Click += new System.EventHandler(this.btnGroupByDept_Click);
            // 
            // btnClearGroup
            // 
            this.btnClearGroup.Location = new System.Drawing.Point(160, 12);
            this.btnClearGroup.Name = "btnClearGroup";
            this.btnClearGroup.Size = new System.Drawing.Size(120, 30);
            this.btnClearGroup.TabIndex = 2;
            this.btnClearGroup.Text = "Clear Grouping";
            this.btnClearGroup.Click += new System.EventHandler(this.btnClearGroup_Click);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(383, 44);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(115, 22);
            this.checkBox1.TabIndex = 3;
            this.checkBox1.Text = "checkBox1";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(816, 39);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(120, 30);
            this.button1.TabIndex = 4;
            this.button1.Text = "form 3";
            //this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1631, 1095);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.groupableGrid1);
            this.Controls.Add(this.btnGroupByDept);
            this.Controls.Add(this.btnClearGroup);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.groupableGrid1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private CheckBox checkBox1;
        private Button button1;
        private DataGridViewButtonColumn truong;
        private DataGridViewTextBoxColumn lop;
        private DataGridViewTextBoxColumn ten;
        private DataGridViewTextBoxColumn toan;
        private DataGridViewTextBoxColumn tienganh;
        private DataGridViewLinkColumn rank;
    }
}

