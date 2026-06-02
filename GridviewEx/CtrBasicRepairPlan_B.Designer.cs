namespace coms.COMSK.ui.common
{
    partial class CtrBasicRepairPlan_B
    {
        /// <summary> 
        /// 必要なデザイナ変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region コンポーネント デザイナで生成されたコード

        /// <summary> 
        /// デザイナ サポートに必要なメソッドです。このメソッドの内容を 
        /// コード エディタで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            coms.COMMON.ui.GridViewStyle gridViewStyle1 = new coms.COMMON.ui.GridViewStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.btnAdd = new System.Windows.Forms.Button();
            this.gcRepairList_Building = new coms.COMMON.ui.DataGridViewEx();
            this.clDetail_B = new System.Windows.Forms.DataGridViewButtonColumn();
            this.clChild = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clUnit_B = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.clUpdateHistory_B = new System.Windows.Forms.DataGridViewButtonColumn();
            this.clDelete_B = new System.Windows.Forms.DataGridViewButtonColumn();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gcRepairList_Building)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.btnAdd);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.gcRepairList_Building);
            this.splitContainer1.Size = new System.Drawing.Size(726, 364);
            this.splitContainer1.SplitterDistance = 31;
            this.splitContainer1.TabIndex = 15;
            // 
            // btnAdd
            // 
            this.btnAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAdd.Location = new System.Drawing.Point(648, 3);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(75, 23);
            this.btnAdd.TabIndex = 0;
            this.btnAdd.Text = "新規登録";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // gcRepairList_Building
            // 
            this.gcRepairList_Building.AllowDrop = true;
            this.gcRepairList_Building.AllowUserToAddRows = false;
            this.gcRepairList_Building.AllowUserToOrderColumns = true;
            this.gcRepairList_Building.AutoGenerateColumns = false;
            this.gcRepairList_Building.BackgroundColor = System.Drawing.Color.White;
            this.gcRepairList_Building.ColumnHeadersHeight = 26;
            this.gcRepairList_Building.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.gcRepairList_Building.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.clDetail_B,
            this.clChild,
            this.clUnit_B,
            this.clUpdateHistory_B,
            this.clDelete_B});
            this.gcRepairList_Building.DisabledFilterAll = true;
            this.gcRepairList_Building.DisabledFilterColumns = null;
            this.gcRepairList_Building.DisabledSortAll = false;
            this.gcRepairList_Building.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gcRepairList_Building.EnableHeadersVisualStyles = false;
            this.gcRepairList_Building.FilterAndSortEnabled = true;
            this.gcRepairList_Building.FilterStringChangedInvokeBeforeDatasourceUpdate = true;
            this.gcRepairList_Building.FocusedRowHandle = -1;
            this.gcRepairList_Building.IgnoreAutoFormatColumns = null;
            this.gcRepairList_Building.KeepFilterAndSort = true;
            this.gcRepairList_Building.ListCellEditorColumnNames = null;
            this.gcRepairList_Building.Location = new System.Drawing.Point(0, 0);
            this.gcRepairList_Building.MaxFilterButtonImageHeight = 23;
            this.gcRepairList_Building.Name = "gcRepairList_Building";
            this.gcRepairList_Building.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.gcRepairList_Building.RowHeadersVisible = false;
            this.gcRepairList_Building.RowTemplate.Height = 21;
            this.gcRepairList_Building.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gcRepairList_Building.Size = new System.Drawing.Size(726, 329);
            this.gcRepairList_Building.SortAsNumberColumns = null;
            this.gcRepairList_Building.SortStringChangedInvokeBeforeDatasourceUpdate = true;
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
            this.gcRepairList_Building.StyleSettings = gridViewStyle1;
            this.gcRepairList_Building.TabIndex = 0;
            this.gcRepairList_Building.UsingRowSelectedStyle = true;
            // 
            // clDetail_B
            // 
            this.clDetail_B.HeaderText = "詳細";
            this.clDetail_B.MinimumWidth = 24;
            this.clDetail_B.Name = "clDetail_B";
            this.clDetail_B.ReadOnly = true;
            this.clDetail_B.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.clDetail_B.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.clDetail_B.Text = "詳細";
            this.clDetail_B.UseColumnTextForButtonValue = true;
            this.clDetail_B.Width = 45;
            // 
            // clChild
            // 
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.clChild.DefaultCellStyle = dataGridViewCellStyle1;
            this.clChild.HeaderText = "付随\r\n項目";
            this.clChild.MinimumWidth = 24;
            this.clChild.Name = "clChild";
            this.clChild.ReadOnly = true;
            this.clChild.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.clChild.Width = 50;
            // 
            // clUnit_B
            // 
            this.clUnit_B.DataPropertyName = "aaaa";
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.clUnit_B.DefaultCellStyle = dataGridViewCellStyle2;
            this.clUnit_B.HeaderText = "単位";
            this.clUnit_B.MinimumWidth = 24;
            this.clUnit_B.Name = "clUnit_B";
            this.clUnit_B.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.clUnit_B.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.clUnit_B.Width = 45;
            // 
            // clUpdateHistory_B
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.clUpdateHistory_B.DefaultCellStyle = dataGridViewCellStyle3;
            this.clUpdateHistory_B.HeaderText = "変更履歴";
            this.clUpdateHistory_B.MinimumWidth = 24;
            this.clUpdateHistory_B.Name = "clUpdateHistory_B";
            this.clUpdateHistory_B.ReadOnly = true;
            this.clUpdateHistory_B.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.clUpdateHistory_B.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.clUpdateHistory_B.Text = "変更履歴";
            this.clUpdateHistory_B.UseColumnTextForButtonValue = true;
            this.clUpdateHistory_B.Width = 70;
            // 
            // clDelete_B
            // 
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.clDelete_B.DefaultCellStyle = dataGridViewCellStyle4;
            this.clDelete_B.HeaderText = "削除";
            this.clDelete_B.MinimumWidth = 24;
            this.clDelete_B.Name = "clDelete_B";
            this.clDelete_B.ReadOnly = true;
            this.clDelete_B.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.clDelete_B.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.clDelete_B.Text = "削除";
            this.clDelete_B.UseColumnTextForButtonValue = true;
            this.clDelete_B.Width = 63;
            // 
            // CtrBasicRepairPlan_B
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "CtrBasicRepairPlan_B";
            this.Size = new System.Drawing.Size(726, 364);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gcRepairList_Building)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button btnAdd;
        private COMMON.ui.DataGridViewEx gcRepairList_Building;
        private COMMON.ui.DataGridViewNumericColumn clDisplayOrder;
        private System.Windows.Forms.DataGridViewTextBoxColumn clConstructionItem_B;
        private System.Windows.Forms.DataGridViewTextBoxColumn clParts_B;
        private System.Windows.Forms.DataGridViewTextBoxColumn clSpecifications_B;
        private System.Windows.Forms.DataGridViewTextBoxColumn clRepairKind_B;
        private COMMON.ui.DataGridViewNumericColumn clRepairPeriod_B;
        private COMMON.ui.DataGridViewNumericColumn clUnitPrice_B;
        private System.Windows.Forms.DataGridViewTextBoxColumn clPosition_B;
        private System.Windows.Forms.DataGridViewTextBoxColumn clDivision_B;
        private System.Windows.Forms.DataGridViewTextBoxColumn clCurrSpecification_B;
        private System.Windows.Forms.DataGridViewTextBoxColumn clTemp_B;
        private System.Windows.Forms.DataGridViewTextBoxColumn clRemarks;
        private System.Windows.Forms.DataGridViewButtonColumn clDetail_B;
        private System.Windows.Forms.DataGridViewTextBoxColumn clChild;
        private System.Windows.Forms.DataGridViewComboBoxColumn clUnit_B;
        private System.Windows.Forms.DataGridViewButtonColumn clUpdateHistory_B;
        private System.Windows.Forms.DataGridViewButtonColumn clDelete_B;
    }
}
