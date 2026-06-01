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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CtrBasicRepairPlan_B));
            coms.COMMON.ui.GridViewStyle gridViewStyle1 = new coms.COMMON.ui.GridViewStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle11 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle12 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle13 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle14 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle15 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle16 = new System.Windows.Forms.DataGridViewCellStyle();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.btnAdd = new System.Windows.Forms.Button();
            this.gcRepairList_Building = new coms.COMMON.ui.DataGridViewEx();
            this.clDisplayOrder = new COMMON.ui.DataGridViewNumericColumn();
            this.clDetail_B = new System.Windows.Forms.DataGridViewButtonColumn();
            this.clConstructionItem_B = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clParts_B = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clSpecifications_B = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clRepairKind_B = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clChild = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clRepairPeriod_B = new COMMON.ui.DataGridViewNumericColumn();
            this.clUnitPrice_B = new COMMON.ui.DataGridViewNumericColumn();
            this.clUnit_B = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.clPosition_B = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clDivision_B = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clCurrSpecification_B = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clTemp_B = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clRemarks = new System.Windows.Forms.DataGridViewTextBoxColumn();
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
            this.gcRepairList_Building.BackgroundColor = System.Drawing.Color.White;
            this.gcRepairList_Building.ColumnHeadersHeight = 26;
            this.gcRepairList_Building.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.gcRepairList_Building.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.clDisplayOrder,
            this.clDetail_B,
            this.clConstructionItem_B,
            this.clParts_B,
            this.clSpecifications_B,
            this.clRepairKind_B,
            this.clChild,
            this.clRepairPeriod_B,
            this.clUnitPrice_B,
            this.clUnit_B,
            this.clPosition_B,
            this.clDivision_B,
            this.clCurrSpecification_B,
            this.clTemp_B,
            this.clRemarks,
            this.clUpdateHistory_B,
            this.clDelete_B});
            this.gcRepairList_Building.DisabledFilterAll = true;
            this.gcRepairList_Building.DisabledFilterColumns = ((System.Collections.Generic.HashSet<string>)(resources.GetObject("gcRepairList_Building.DisabledFilterColumns")));
            this.gcRepairList_Building.DisabledSortAll = false;
            this.gcRepairList_Building.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gcRepairList_Building.EnableHeadersVisualStyles = false;
            this.gcRepairList_Building.FilterAndSortEnabled = true;
            this.gcRepairList_Building.FilterStringChangedInvokeBeforeDatasourceUpdate = true;
            this.gcRepairList_Building.FocusedRowHandle = -1;
            this.gcRepairList_Building.IgnoreAutoFormatColumns = ((System.Collections.Generic.HashSet<string>)(resources.GetObject("gcRepairList_Building.IgnoreAutoFormatColumns")));
            this.gcRepairList_Building.KeepFilterAndSort = true;
            this.gcRepairList_Building.ListCellEditorColumnNames = ((System.Collections.Generic.List<string>)(resources.GetObject("gcRepairList_Building.ListCellEditorColumnNames")));
            this.gcRepairList_Building.Location = new System.Drawing.Point(0, 0);
            this.gcRepairList_Building.MaxFilterButtonImageHeight = 23;
            this.gcRepairList_Building.Name = "gcRepairList_Building";
            this.gcRepairList_Building.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.gcRepairList_Building.RowHeadersVisible = false;
            this.gcRepairList_Building.RowTemplate.Height = 21;
            this.gcRepairList_Building.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gcRepairList_Building.Size = new System.Drawing.Size(726, 329);
            this.gcRepairList_Building.SortAsNumberColumns = ((System.Collections.Generic.HashSet<string>)(resources.GetObject("gcRepairList_Building.SortAsNumberColumns")));
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
            // clDisplayOrder
            // 
            this.clDisplayOrder.DataPropertyName = "ViewSequenceDisplay";
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle1.Format = "#,0.0";
            this.clDisplayOrder.DefaultCellStyle = dataGridViewCellStyle1;
            this.clDisplayOrder.HeaderText = "表示\r\n順";
            this.clDisplayOrder.AllowDecimal = true;
            this.clDisplayOrder.MinimumWidth = 24;
            this.clDisplayOrder.Name = "clDisplayOrder";
            this.clDisplayOrder.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.clDisplayOrder.Width = 45;
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
            // clConstructionItem_B
            // 
            this.clConstructionItem_B.DataPropertyName = "ConstructionItemName";
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.clConstructionItem_B.DefaultCellStyle = dataGridViewCellStyle2;
            this.clConstructionItem_B.HeaderText = "工事項目";
            this.clConstructionItem_B.MinimumWidth = 24;
            this.clConstructionItem_B.Name = "clConstructionItem_B";
            this.clConstructionItem_B.ReadOnly = true;
            this.clConstructionItem_B.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            // 
            // clParts_B
            // 
            this.clParts_B.DataPropertyName = "ConstructionCategoryName";
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.clParts_B.DefaultCellStyle = dataGridViewCellStyle3;
            this.clParts_B.HeaderText = "工事種別";
            this.clParts_B.MinimumWidth = 24;
            this.clParts_B.Name = "clParts_B";
            this.clParts_B.ReadOnly = true;
            this.clParts_B.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            // 
            // clSpecifications_B
            // 
            this.clSpecifications_B.DataPropertyName = "ConstructionSpecificationName";
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.clSpecifications_B.DefaultCellStyle = dataGridViewCellStyle4;
            this.clSpecifications_B.HeaderText = "仕様";
            this.clSpecifications_B.MinimumWidth = 24;
            this.clSpecifications_B.Name = "clSpecifications_B";
            this.clSpecifications_B.ReadOnly = true;
            this.clSpecifications_B.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.clSpecifications_B.Width = 200;
            // 
            // clRepairKind_B
            // 
            this.clRepairKind_B.DataPropertyName = "ConstructionDivisionName";
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.clRepairKind_B.DefaultCellStyle = dataGridViewCellStyle5;
            this.clRepairKind_B.HeaderText = "工事\r\n区分";
            this.clRepairKind_B.MinimumWidth = 24;
            this.clRepairKind_B.Name = "clRepairKind_B";
            this.clRepairKind_B.ReadOnly = true;
            this.clRepairKind_B.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.clRepairKind_B.Width = 50;
            // 
            // clChild
            // 
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.clChild.DefaultCellStyle = dataGridViewCellStyle6;
            this.clChild.HeaderText = "付随\r\n項目";
            this.clChild.MinimumWidth = 24;
            this.clChild.Name = "clChild";
            this.clChild.ReadOnly = true;
            this.clChild.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.clChild.Width = 50;
            // 
            // clRepairPeriod_B
            // 
            this.clRepairPeriod_B.DataPropertyName = "Cycle";
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.clRepairPeriod_B.DefaultCellStyle = dataGridViewCellStyle7;
            this.clRepairPeriod_B.HeaderText = "周期\r\n(年)";
            this.clRepairPeriod_B.MinimumWidth = 24;
            this.clRepairPeriod_B.Name = "clRepairPeriod_B";
            this.clRepairPeriod_B.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.clRepairPeriod_B.Width = 45;
            // 
            // clUnitPrice_B
            // 
            this.clUnitPrice_B.DataPropertyName = "Cost";
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.clUnitPrice_B.DefaultCellStyle = dataGridViewCellStyle8;
            this.clUnitPrice_B.HeaderText = "単価";
            this.clUnitPrice_B.MinimumWidth = 24;
            this.clUnitPrice_B.Name = "clUnitPrice_B";
            this.clUnitPrice_B.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            // 
            // clUnit_B
            // 
            this.clUnit_B.DataPropertyName = "UnitCode";
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.clUnit_B.DefaultCellStyle = dataGridViewCellStyle9;
            this.clUnit_B.HeaderText = "単位";
            this.clUnit_B.MinimumWidth = 24;
            this.clUnit_B.Name = "clUnit_B";
            this.clUnit_B.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.clUnit_B.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.clUnit_B.Width = 45;
            // 
            // clPosition_B
            // 
            this.clPosition_B.DataPropertyName = "ConstructionPositionName";
            dataGridViewCellStyle10.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.clPosition_B.DefaultCellStyle = dataGridViewCellStyle10;
            this.clPosition_B.HeaderText = "位置";
            this.clPosition_B.MinimumWidth = 24;
            this.clPosition_B.Name = "clPosition_B";
            this.clPosition_B.ReadOnly = true;
            this.clPosition_B.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.clPosition_B.Width = 50;
            // 
            // clDivision_B
            // 
            this.clDivision_B.DataPropertyName = "ConstructionRegionName";
            dataGridViewCellStyle11.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.clDivision_B.DefaultCellStyle = dataGridViewCellStyle11;
            this.clDivision_B.HeaderText = "部位";
            this.clDivision_B.MinimumWidth = 24;
            this.clDivision_B.Name = "clDivision_B";
            this.clDivision_B.ReadOnly = true;
            this.clDivision_B.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            // 
            // clCurrSpecification_B
            // 
            this.clCurrSpecification_B.DataPropertyName = "CurrentSpecification";
            dataGridViewCellStyle12.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.clCurrSpecification_B.DefaultCellStyle = dataGridViewCellStyle12;
            this.clCurrSpecification_B.HeaderText = "現状仕様";
            this.clCurrSpecification_B.MinimumWidth = 24;
            this.clCurrSpecification_B.Name = "clCurrSpecification_B";
            this.clCurrSpecification_B.ReadOnly = true;
            this.clCurrSpecification_B.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            // 
            // clTemp_B
            // 
            this.clTemp_B.DataPropertyName = "RepairConstructionContentName";
            dataGridViewCellStyle13.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.clTemp_B.DefaultCellStyle = dataGridViewCellStyle13;
            this.clTemp_B.HeaderText = "(修繕工事内容)";
            this.clTemp_B.MinimumWidth = 24;
            this.clTemp_B.Name = "clTemp_B";
            this.clTemp_B.ReadOnly = true;
            this.clTemp_B.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.clTemp_B.Width = 200;
            // 
            // clRemarks
            // 
            this.clRemarks.DataPropertyName = "Memo";
            dataGridViewCellStyle14.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.clRemarks.DefaultCellStyle = dataGridViewCellStyle14;
            this.clRemarks.HeaderText = "メモ";
            this.clRemarks.MinimumWidth = 24;
            this.clRemarks.Name = "clRemarks";
            this.clRemarks.ReadOnly = true;
            this.clRemarks.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.clRemarks.Width = 200;
            // 
            // clUpdateHistory_B
            // 
            dataGridViewCellStyle15.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.clUpdateHistory_B.DefaultCellStyle = dataGridViewCellStyle15;
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
            dataGridViewCellStyle16.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.clDelete_B.DefaultCellStyle = dataGridViewCellStyle16;
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
        private System.Windows.Forms.DataGridViewButtonColumn clDetail_B;
        private System.Windows.Forms.DataGridViewTextBoxColumn clConstructionItem_B;
        private System.Windows.Forms.DataGridViewTextBoxColumn clParts_B;
        private System.Windows.Forms.DataGridViewTextBoxColumn clSpecifications_B;
        private System.Windows.Forms.DataGridViewTextBoxColumn clRepairKind_B;
        private System.Windows.Forms.DataGridViewTextBoxColumn clChild;
        private COMMON.ui.DataGridViewNumericColumn clRepairPeriod_B;
        private COMMON.ui.DataGridViewNumericColumn clUnitPrice_B;
        private System.Windows.Forms.DataGridViewComboBoxColumn clUnit_B;
        private System.Windows.Forms.DataGridViewTextBoxColumn clPosition_B;
        private System.Windows.Forms.DataGridViewTextBoxColumn clDivision_B;
        private System.Windows.Forms.DataGridViewTextBoxColumn clCurrSpecification_B;
        private System.Windows.Forms.DataGridViewTextBoxColumn clTemp_B;
        private System.Windows.Forms.DataGridViewTextBoxColumn clRemarks;
        private System.Windows.Forms.DataGridViewButtonColumn clUpdateHistory_B;
        private System.Windows.Forms.DataGridViewButtonColumn clDelete_B;
    }
}
