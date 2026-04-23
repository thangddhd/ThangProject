namespace coms.COMSK.ui.common
{
    partial class CtrRepairPlan_B
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CtrRepairPlan_B));
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
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnDeselectAll = new System.Windows.Forms.Button();
            this.gcRepairList_Building = new coms.COMMON.ui.DataGridViewEx();
            this.clDisplayOrder_B = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clDetail_B = new System.Windows.Forms.DataGridViewButtonColumn();
            this.clSelect_B = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.clConstructionItem_B = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clRepairType_B = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clSpec_B = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clDivision_B = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clChild_B = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clRepairPeriod_B = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clPrice_B = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clAmount_B = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clUnit_B = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.clTempBox_B = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clPosition_B = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clPart_B = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clCurrSpec_B = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clTemp_B = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clRemarks_B = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clUpdateHistory_B = new System.Windows.Forms.DataGridViewButtonColumn();
            ((System.ComponentModel.ISupportInitialize)(this.gcRepairList_Building)).BeginInit();
            this.SuspendLayout();
            // 
            // btnAdd
            // 
            this.btnAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAdd.Location = new System.Drawing.Point(693, 3);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(75, 23);
            this.btnAdd.TabIndex = 2;
            this.btnAdd.Text = "新規登録";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnDeselectAll
            // 
            this.btnDeselectAll.Location = new System.Drawing.Point(80, 3);
            this.btnDeselectAll.Name = "btnDeselectAll";
            this.btnDeselectAll.Size = new System.Drawing.Size(95, 23);
            this.btnDeselectAll.TabIndex = 1;
            this.btnDeselectAll.Text = "一括選択取消";
            this.btnDeselectAll.UseVisualStyleBackColor = true;
            this.btnDeselectAll.Click += new System.EventHandler(this.btnDeselectAll_Click);
            // 
            // gcRepairList_Building
            // 
            this.gcRepairList_Building.AllowDrop = true;
            this.gcRepairList_Building.AllowUserToAddRows = false;
            this.gcRepairList_Building.AllowUserToOrderColumns = true;
            this.gcRepairList_Building.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gcRepairList_Building.ColumnHeadersHeight = 40;
            this.gcRepairList_Building.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.gcRepairList_Building.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.clDisplayOrder_B,
            this.clDetail_B,
            this.clSelect_B,
            this.clConstructionItem_B,
            this.clRepairType_B,
            this.clSpec_B,
            this.clDivision_B,
            this.clChild_B,
            this.clRepairPeriod_B,
            this.clPrice_B,
            this.clAmount_B,
            this.clUnit_B,
            this.clTempBox_B,
            this.clPosition_B,
            this.clPart_B,
            this.clCurrSpec_B,
            this.clTemp_B,
            this.clRemarks_B,
            this.clUpdateHistory_B});
            this.gcRepairList_Building.DisabledFilterAll = false;
            this.gcRepairList_Building.DisabledFilterColumns = ((System.Collections.Generic.HashSet<string>)(resources.GetObject("gcRepairList_Building.DisabledFilterColumns")));
            this.gcRepairList_Building.DisabledSortAll = false;
            this.gcRepairList_Building.EnableHeadersVisualStyles = false;
            this.gcRepairList_Building.FilterAndSortEnabled = true;
            this.gcRepairList_Building.FilterStringChangedInvokeBeforeDatasourceUpdate = true;
            this.gcRepairList_Building.FocusedRowHandle = -1;
            this.gcRepairList_Building.IgnoreAutoFormatColumns = ((System.Collections.Generic.HashSet<string>)(resources.GetObject("gcRepairList_Building.IgnoreAutoFormatColumns")));
            this.gcRepairList_Building.KeepFilterAndSort = true;
            this.gcRepairList_Building.ListCellEditorColumnNames = ((System.Collections.Generic.List<string>)(resources.GetObject("gcRepairList_Building.ListCellEditorColumnNames")));
            this.gcRepairList_Building.Location = new System.Drawing.Point(0, 32);
            this.gcRepairList_Building.MaxFilterButtonImageHeight = 23;
            this.gcRepairList_Building.Name = "gcRepairList_Building";
            this.gcRepairList_Building.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.gcRepairList_Building.RowHeadersVisible = false;
            this.gcRepairList_Building.RowTemplate.Height = 21;
            this.gcRepairList_Building.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gcRepairList_Building.Size = new System.Drawing.Size(768, 262);
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
            this.gcRepairList_Building.TabIndex = 3;
            this.gcRepairList_Building.CustomColumnDisplayText += new System.EventHandler<coms.COMMON.ui.CustomColumnDisplayTextEventArgs>(this.gcRepairList_Building_CustomColumnDisplayText);
            this.gcRepairList_Building.CellMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.gcRepairList_Building_CellMouseClick);
            this.gcRepairList_Building.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.gcRepairList_Building_CellValueChanged);
            this.gcRepairList_Building.RowBackColorNeeded += new System.EventHandler<coms.COMMON.ui.RowBackColorNeededEventArgs>(this.gcRepairList_Building_RowBackColorNeeded);
            this.gcRepairList_Building.ButtonIconNeeded += new System.EventHandler<coms.COMMON.ui.ButtonIconNeededEventArgs>(this.gcRepairList_Building_ButtonIconNeeded);
            this.gcRepairList_Building.UsingRowSelectedStyle = true;
            // 
            // clDisplayOrder_B
            // 
            this.clDisplayOrder_B.DataPropertyName = "ViewSequenceDisplay";
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.clDisplayOrder_B.DefaultCellStyle = dataGridViewCellStyle1;
            this.clDisplayOrder_B.HeaderText = "表示\n順";
            this.clDisplayOrder_B.MinimumWidth = 24;
            this.clDisplayOrder_B.Name = "clDisplayOrder_B";
            this.clDisplayOrder_B.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.clDisplayOrder_B.Width = 45;
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
            // clSelect_B
            // 
            this.clSelect_B.DataPropertyName = "Select";
            this.clSelect_B.HeaderText = "選択";
            this.clSelect_B.MinimumWidth = 24;
            this.clSelect_B.Name = "clSelect_B";
            this.clSelect_B.ReadOnly = true;
            this.clSelect_B.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.clSelect_B.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.clSelect_B.Width = 45;
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
            this.clConstructionItem_B.Width = 80;
            // 
            // clRepairType_B
            // 
            this.clRepairType_B.DataPropertyName = "ConstructionCategoryName";
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.clRepairType_B.DefaultCellStyle = dataGridViewCellStyle3;
            this.clRepairType_B.HeaderText = "工事種別";
            this.clRepairType_B.MinimumWidth = 24;
            this.clRepairType_B.Name = "clRepairType_B";
            this.clRepairType_B.ReadOnly = true;
            this.clRepairType_B.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.clRepairType_B.Width = 80;
            // 
            // clSpec_B
            // 
            this.clSpec_B.DataPropertyName = "ConstructionSpecificationName";
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.clSpec_B.DefaultCellStyle = dataGridViewCellStyle4;
            this.clSpec_B.HeaderText = "仕様";
            this.clSpec_B.MinimumWidth = 24;
            this.clSpec_B.Name = "clSpec_B";
            this.clSpec_B.ReadOnly = true;
            this.clSpec_B.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.clSpec_B.Width = 180;
            // 
            // clDivision_B
            // 
            this.clDivision_B.DataPropertyName = "ConstructionDivisionName";
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.clDivision_B.DefaultCellStyle = dataGridViewCellStyle5;
            this.clDivision_B.HeaderText = "工事\n区分";
            this.clDivision_B.MinimumWidth = 24;
            this.clDivision_B.Name = "clDivision_B";
            this.clDivision_B.ReadOnly = true;
            this.clDivision_B.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.clDivision_B.Width = 65;
            // 
            // clChild_B
            // 
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.clChild_B.DefaultCellStyle = dataGridViewCellStyle6;
            this.clChild_B.HeaderText = "付随\n項目";
            this.clChild_B.MinimumWidth = 24;
            this.clChild_B.Name = "clChild_B";
            this.clChild_B.ReadOnly = true;
            this.clChild_B.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.clChild_B.Width = 50;
            // 
            // clRepairPeriod_B
            // 
            this.clRepairPeriod_B.DataPropertyName = "Cycle";
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopRight;
            this.clRepairPeriod_B.DefaultCellStyle = dataGridViewCellStyle7;
            this.clRepairPeriod_B.HeaderText = "周期\n(年)";
            this.clRepairPeriod_B.MinimumWidth = 24;
            this.clRepairPeriod_B.Name = "clRepairPeriod_B";
            this.clRepairPeriod_B.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.clRepairPeriod_B.Width = 45;
            // 
            // clPrice_B
            // 
            this.clPrice_B.DataPropertyName = "Cost";
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.clPrice_B.DefaultCellStyle = dataGridViewCellStyle8;
            this.clPrice_B.HeaderText = "単価";
            this.clPrice_B.MinimumWidth = 24;
            this.clPrice_B.Name = "clPrice_B";
            this.clPrice_B.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.clPrice_B.Width = 110;
            // 
            // clAmount_B
            // 
            this.clAmount_B.DataPropertyName = "Amount";
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.clAmount_B.DefaultCellStyle = dataGridViewCellStyle9;
            this.clAmount_B.HeaderText = "数量";
            this.clAmount_B.MinimumWidth = 24;
            this.clAmount_B.Name = "clAmount_B";
            this.clAmount_B.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.clAmount_B.Width = 45;
            // 
            // clUnit_B
            // 
            this.clUnit_B.DataPropertyName = "UnitCode";
            dataGridViewCellStyle10.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.clUnit_B.DefaultCellStyle = dataGridViewCellStyle10;
            this.clUnit_B.HeaderText = "単位";
            this.clUnit_B.MinimumWidth = 24;
            this.clUnit_B.Name = "clUnit_B";
            this.clUnit_B.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.clUnit_B.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.clUnit_B.Width = 45;
            // 
            // clTempBox_B
            // 
            dataGridViewCellStyle11.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.clTempBox_B.DefaultCellStyle = dataGridViewCellStyle11;
            this.clTempBox_B.HeaderText = "参考\n基準";
            this.clTempBox_B.MinimumWidth = 24;
            this.clTempBox_B.Name = "clTempBox_B";
            this.clTempBox_B.ReadOnly = true;
            this.clTempBox_B.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.clTempBox_B.Width = 45;
            // 
            // clPosition_B
            // 
            this.clPosition_B.DataPropertyName = "ConstructionPositionName";
            dataGridViewCellStyle12.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.clPosition_B.DefaultCellStyle = dataGridViewCellStyle12;
            this.clPosition_B.HeaderText = "位置";
            this.clPosition_B.MinimumWidth = 24;
            this.clPosition_B.Name = "clPosition_B";
            this.clPosition_B.ReadOnly = true;
            this.clPosition_B.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.clPosition_B.Width = 50;
            // 
            // clPart_B
            // 
            this.clPart_B.DataPropertyName = "ConstructionRegionName";
            dataGridViewCellStyle13.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.clPart_B.DefaultCellStyle = dataGridViewCellStyle13;
            this.clPart_B.HeaderText = "部位";
            this.clPart_B.MinimumWidth = 24;
            this.clPart_B.Name = "clPart_B";
            this.clPart_B.ReadOnly = true;
            this.clPart_B.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.clPart_B.Width = 90;
            // 
            // clCurrSpec_B
            // 
            this.clCurrSpec_B.DataPropertyName = "CurrentSpecification";
            dataGridViewCellStyle14.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.clCurrSpec_B.DefaultCellStyle = dataGridViewCellStyle14;
            this.clCurrSpec_B.HeaderText = "現状仕様";
            this.clCurrSpec_B.MinimumWidth = 24;
            this.clCurrSpec_B.Name = "clCurrSpec_B";
            this.clCurrSpec_B.ReadOnly = true;
            this.clCurrSpec_B.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            // 
            // clTemp_B
            // 
            this.clTemp_B.DataPropertyName = "RepairConstructionContentName";
            dataGridViewCellStyle15.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.clTemp_B.DefaultCellStyle = dataGridViewCellStyle15;
            this.clTemp_B.HeaderText = "(修繕工事内容)";
            this.clTemp_B.MinimumWidth = 24;
            this.clTemp_B.Name = "clTemp_B";
            this.clTemp_B.ReadOnly = true;
            this.clTemp_B.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.clTemp_B.Width = 190;
            // 
            // clRemarks_B
            // 
            this.clRemarks_B.DataPropertyName = "Memo";
            dataGridViewCellStyle16.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.clRemarks_B.DefaultCellStyle = dataGridViewCellStyle16;
            this.clRemarks_B.HeaderText = "メモ";
            this.clRemarks_B.MinimumWidth = 24;
            this.clRemarks_B.Name = "clRemarks_B";
            this.clRemarks_B.ReadOnly = true;
            this.clRemarks_B.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.clRemarks_B.Width = 185;
            // 
            // clUpdateHistory_B
            // 
            this.clUpdateHistory_B.HeaderText = "変更履歴";
            this.clUpdateHistory_B.MinimumWidth = 24;
            this.clUpdateHistory_B.Name = "clUpdateHistory_B";
            this.clUpdateHistory_B.ReadOnly = true;
            this.clUpdateHistory_B.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.clUpdateHistory_B.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.clUpdateHistory_B.Text = "変更履歴";
            this.clUpdateHistory_B.UseColumnTextForButtonValue = true;
            this.clUpdateHistory_B.Width = 80;
            // 
            // CtrRepairPlan_B
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.gcRepairList_Building);
            this.Controls.Add(this.btnDeselectAll);
            this.Controls.Add(this.btnAdd);
            this.Name = "CtrRepairPlan_B";
            this.Size = new System.Drawing.Size(771, 294);
            ((System.ComponentModel.ISupportInitialize)(this.gcRepairList_Building)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
		private System.Windows.Forms.Button btnAdd;
		private System.Windows.Forms.Button btnDeselectAll;
        private COMMON.ui.DataGridViewEx gcRepairList_Building;
        private System.Windows.Forms.DataGridViewTextBoxColumn clDisplayOrder_B;
        private System.Windows.Forms.DataGridViewButtonColumn clDetail_B;
        private System.Windows.Forms.DataGridViewCheckBoxColumn clSelect_B;
        private System.Windows.Forms.DataGridViewTextBoxColumn clConstructionItem_B;
        private System.Windows.Forms.DataGridViewTextBoxColumn clRepairType_B;
        private System.Windows.Forms.DataGridViewTextBoxColumn clSpec_B;
        private System.Windows.Forms.DataGridViewTextBoxColumn clDivision_B;
        private System.Windows.Forms.DataGridViewTextBoxColumn clChild_B;
        private System.Windows.Forms.DataGridViewTextBoxColumn clRepairPeriod_B;
        private System.Windows.Forms.DataGridViewTextBoxColumn clPrice_B;
        private System.Windows.Forms.DataGridViewTextBoxColumn clAmount_B;
        private System.Windows.Forms.DataGridViewComboBoxColumn clUnit_B;
        private System.Windows.Forms.DataGridViewTextBoxColumn clTempBox_B;
        private System.Windows.Forms.DataGridViewTextBoxColumn clPosition_B;
        private System.Windows.Forms.DataGridViewTextBoxColumn clPart_B;
        private System.Windows.Forms.DataGridViewTextBoxColumn clCurrSpec_B;
        private System.Windows.Forms.DataGridViewTextBoxColumn clTemp_B;
        private System.Windows.Forms.DataGridViewTextBoxColumn clRemarks_B;
        private System.Windows.Forms.DataGridViewButtonColumn clUpdateHistory_B;
    }
}
