namespace coms.COMSK.ui.common
{
    partial class CtrRepairPlan_E
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CtrRepairPlan_E));
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle17 = new System.Windows.Forms.DataGridViewCellStyle();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnDeselectAll = new System.Windows.Forms.Button();
            this.gridcRepairList_Equipment = new coms.COMMON.ui.DataGridViewEx();
            this.clDisplayOrder_E = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clDetail_E = new System.Windows.Forms.DataGridViewButtonColumn();
            this.clSelect_E = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.clSystem_E = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clRepairType_E = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clSpec_E = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clDivision_E = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clChild = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clRepairPeriod_E = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clPrice_E = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clAmount_E = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clUnit_E = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.clTempBox_E = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clPositionUnit = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clPosition_E = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clPartNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clParts_E = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clCurrSpec_E = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clTemp_E = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clRemarks_E = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clUpdateHistory_E = new System.Windows.Forms.DataGridViewButtonColumn();
            ((System.ComponentModel.ISupportInitialize)(this.gridcRepairList_Equipment)).BeginInit();
            this.SuspendLayout();
            // 
            // btnAdd
            // 
            this.btnAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAdd.Location = new System.Drawing.Point(724, 3);
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
            // gridcRepairList_Equipment
            // 
            this.gridcRepairList_Equipment.AllowDrop = true;
            this.gridcRepairList_Equipment.AllowUserToAddRows = false;
            this.gridcRepairList_Equipment.AllowUserToOrderColumns = true;
            this.gridcRepairList_Equipment.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gridcRepairList_Equipment.ColumnHeadersHeight = 40;
            this.gridcRepairList_Equipment.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.gridcRepairList_Equipment.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.clDisplayOrder_E,
            this.clDetail_E,
            this.clSelect_E,
            this.clSystem_E,
            this.clRepairType_E,
            this.clSpec_E,
            this.clDivision_E,
            this.clChild,
            this.clRepairPeriod_E,
            this.clPrice_E,
            this.clAmount_E,
            this.clUnit_E,
            this.clTempBox_E,
            this.clPositionUnit,
            this.clPosition_E,
            this.clPartNo,
            this.clParts_E,
            this.clCurrSpec_E,
            this.clTemp_E,
            this.clRemarks_E,
            this.clUpdateHistory_E});
            this.gridcRepairList_Equipment.DisabledFilterAll = false;
            this.gridcRepairList_Equipment.DisabledFilterColumns = ((System.Collections.Generic.HashSet<string>)(resources.GetObject("gridcRepairList_Equipment.DisabledFilterColumns")));
            this.gridcRepairList_Equipment.DisabledSortAll = false;
            this.gridcRepairList_Equipment.EnableHeadersVisualStyles = false;
            this.gridcRepairList_Equipment.FilterAndSortEnabled = true;
            this.gridcRepairList_Equipment.FilterStringChangedInvokeBeforeDatasourceUpdate = true;
            this.gridcRepairList_Equipment.FocusedRowHandle = -1;
            this.gridcRepairList_Equipment.IgnoreAutoFormatColumns = ((System.Collections.Generic.HashSet<string>)(resources.GetObject("gridcRepairList_Equipment.IgnoreAutoFormatColumns")));
            this.gridcRepairList_Equipment.KeepFilterAndSort = true;
            this.gridcRepairList_Equipment.ListCellEditorColumnNames = ((System.Collections.Generic.List<string>)(resources.GetObject("gridcRepairList_Equipment.ListCellEditorColumnNames")));
            this.gridcRepairList_Equipment.Location = new System.Drawing.Point(3, 32);
            this.gridcRepairList_Equipment.MaxFilterButtonImageHeight = 23;
            this.gridcRepairList_Equipment.Name = "gridcRepairList_Equipment";
            this.gridcRepairList_Equipment.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.gridcRepairList_Equipment.RowHeadersVisible = false;
            this.gridcRepairList_Equipment.RowTemplate.Height = 21;
            this.gridcRepairList_Equipment.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridcRepairList_Equipment.Size = new System.Drawing.Size(796, 348);
            this.gridcRepairList_Equipment.SortAsNumberColumns = ((System.Collections.Generic.HashSet<string>)(resources.GetObject("gridcRepairList_Equipment.SortAsNumberColumns")));
            this.gridcRepairList_Equipment.SortStringChangedInvokeBeforeDatasourceUpdate = true;
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
            this.gridcRepairList_Equipment.StyleSettings = gridViewStyle1;
            this.gridcRepairList_Equipment.TabIndex = 3;
            this.gridcRepairList_Equipment.CustomColumnDisplayText += new System.EventHandler<coms.COMMON.ui.CustomColumnDisplayTextEventArgs>(this.gridcRepairList_Equipment_CustomColumnDisplayText);
            this.gridcRepairList_Equipment.CellMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.gridcRepairList_Equipment_CellMouseClick);
            this.gridcRepairList_Equipment.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridcRepairList_Equipment_CellValueChanged);
            this.gridcRepairList_Equipment.RowBackColorNeeded += new System.EventHandler<coms.COMMON.ui.RowBackColorNeededEventArgs>(this.gridcRepairList_Equipment_RowBackColorNeeded);
            this.gridcRepairList_Equipment.ButtonIconNeeded += new System.EventHandler<coms.COMMON.ui.ButtonIconNeededEventArgs>(this.gridcRepairList_Equipment_ButtonIconNeeded);
            this.gridcRepairList_Equipment.UsingRowSelectedStyle = true;
            // 
            // clDisplayOrder_E
            // 
            this.clDisplayOrder_E.DataPropertyName = "ViewSequenceDisplay";
            this.clDisplayOrder_E.HeaderText = "表示\n順";
            this.clDisplayOrder_E.MinimumWidth = 24;
            this.clDisplayOrder_E.Name = "clDisplayOrder_E";
            this.clDisplayOrder_E.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.clDisplayOrder_E.Width = 45;
            // 
            // clDetail_E
            // 
            this.clDetail_E.HeaderText = "詳細";
            this.clDetail_E.MinimumWidth = 24;
            this.clDetail_E.Name = "clDetail_E";
            this.clDetail_E.ReadOnly = true;
            this.clDetail_E.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.clDetail_E.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.clDetail_E.Text = "詳細";
            this.clDetail_E.UseColumnTextForButtonValue = true;
            this.clDetail_E.Width = 45;
            // 
            // clSelect_E
            // 
            this.clSelect_E.DataPropertyName = "Select";
            this.clSelect_E.HeaderText = "選択";
            this.clSelect_E.MinimumWidth = 24;
            this.clSelect_E.Name = "clSelect_E";
            this.clSelect_E.ReadOnly = true;
            this.clSelect_E.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.clSelect_E.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.clSelect_E.Width = 45;
            // 
            // clSystem_E
            // 
            this.clSystem_E.DataPropertyName = "ConstructionItemName";
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.clSystem_E.DefaultCellStyle = dataGridViewCellStyle1;
            this.clSystem_E.HeaderText = "工事項目";
            this.clSystem_E.MinimumWidth = 24;
            this.clSystem_E.Name = "clSystem_E";
            this.clSystem_E.ReadOnly = true;
            this.clSystem_E.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.clSystem_E.Width = 70;
            // 
            // clRepairType_E
            // 
            this.clRepairType_E.DataPropertyName = "ConstructionCategoryName";
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.clRepairType_E.DefaultCellStyle = dataGridViewCellStyle2;
            this.clRepairType_E.HeaderText = "工事種別";
            this.clRepairType_E.MinimumWidth = 24;
            this.clRepairType_E.Name = "clRepairType_E";
            this.clRepairType_E.ReadOnly = true;
            this.clRepairType_E.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.clRepairType_E.Width = 70;
            // 
            // clSpec_E
            // 
            this.clSpec_E.DataPropertyName = "ConstructionSpecificationName";
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.clSpec_E.DefaultCellStyle = dataGridViewCellStyle3;
            this.clSpec_E.HeaderText = "仕様";
            this.clSpec_E.MinimumWidth = 24;
            this.clSpec_E.Name = "clSpec_E";
            this.clSpec_E.ReadOnly = true;
            this.clSpec_E.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.clSpec_E.Width = 125;
            // 
            // clDivision_E
            // 
            this.clDivision_E.DataPropertyName = "ConstructionDivisionName";
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.clDivision_E.DefaultCellStyle = dataGridViewCellStyle4;
            this.clDivision_E.HeaderText = "工事\n区分";
            this.clDivision_E.MinimumWidth = 24;
            this.clDivision_E.Name = "clDivision_E";
            this.clDivision_E.ReadOnly = true;
            this.clDivision_E.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.clDivision_E.Width = 65;
            // 
            // clChild
            // 
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.clChild.DefaultCellStyle = dataGridViewCellStyle5;
            this.clChild.HeaderText = "付随\n項目";
            this.clChild.MinimumWidth = 24;
            this.clChild.Name = "clChild";
            this.clChild.ReadOnly = true;
            this.clChild.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.clChild.Width = 50;
            // 
            // clRepairPeriod_E
            // 
            this.clRepairPeriod_E.DataPropertyName = "Cycle";
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.clRepairPeriod_E.DefaultCellStyle = dataGridViewCellStyle6;
            this.clRepairPeriod_E.HeaderText = "周期\n(年)";
            this.clRepairPeriod_E.MinimumWidth = 24;
            this.clRepairPeriod_E.Name = "clRepairPeriod_E";
            this.clRepairPeriod_E.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.clRepairPeriod_E.Width = 45;
            // 
            // clPrice_E
            // 
            this.clPrice_E.DataPropertyName = "Cost";
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.clPrice_E.DefaultCellStyle = dataGridViewCellStyle7;
            this.clPrice_E.HeaderText = "単価";
            this.clPrice_E.MinimumWidth = 24;
            this.clPrice_E.Name = "clPrice_E";
            this.clPrice_E.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            // 
            // clAmount_E
            // 
            this.clAmount_E.DataPropertyName = "Amount";
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.clAmount_E.DefaultCellStyle = dataGridViewCellStyle8;
            this.clAmount_E.HeaderText = "数量";
            this.clAmount_E.MinimumWidth = 24;
            this.clAmount_E.Name = "clAmount_E";
            this.clAmount_E.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.clAmount_E.Width = 45;
            // 
            // clUnit_E
            // 
            this.clUnit_E.DataPropertyName = "UnitCode";
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.clUnit_E.DefaultCellStyle = dataGridViewCellStyle9;
            this.clUnit_E.HeaderText = "単位";
            this.clUnit_E.MinimumWidth = 24;
            this.clUnit_E.Name = "clUnit_E";
            this.clUnit_E.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.clUnit_E.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.clUnit_E.Width = 45;
            // 
            // clTempBox_E
            // 
            dataGridViewCellStyle10.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.clTempBox_E.DefaultCellStyle = dataGridViewCellStyle10;
            this.clTempBox_E.HeaderText = "参考\n基準";
            this.clTempBox_E.MinimumWidth = 24;
            this.clTempBox_E.Name = "clTempBox_E";
            this.clTempBox_E.ReadOnly = true;
            this.clTempBox_E.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.clTempBox_E.Width = 45;
            // 
            // clPositionUnit
            // 
            this.clPositionUnit.DataPropertyName = "ConstructionPositionUnit";
            dataGridViewCellStyle11.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.clPositionUnit.DefaultCellStyle = dataGridViewCellStyle11;
            this.clPositionUnit.HeaderText = "部位\nユニット";
            this.clPositionUnit.MinimumWidth = 24;
            this.clPositionUnit.Name = "clPositionUnit";
            this.clPositionUnit.ReadOnly = true;
            this.clPositionUnit.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.clPositionUnit.Width = 55;
            // 
            // clPosition_E
            // 
            this.clPosition_E.DataPropertyName = "ConstructionPositionName";
            dataGridViewCellStyle12.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.clPosition_E.DefaultCellStyle = dataGridViewCellStyle12;
            this.clPosition_E.HeaderText = "部位";
            this.clPosition_E.MinimumWidth = 24;
            this.clPosition_E.Name = "clPosition_E";
            this.clPosition_E.ReadOnly = true;
            this.clPosition_E.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.clPosition_E.Width = 50;
            // 
            // clPartNo
            // 
            this.clPartNo.DataPropertyName = "ConstructionPartsNo";
            dataGridViewCellStyle13.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.clPartNo.DefaultCellStyle = dataGridViewCellStyle13;
            this.clPartNo.HeaderText = "部品\nNo";
            this.clPartNo.MinimumWidth = 24;
            this.clPartNo.Name = "clPartNo";
            this.clPartNo.ReadOnly = true;
            this.clPartNo.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.clPartNo.Width = 50;
            // 
            // clParts_E
            // 
            this.clParts_E.DataPropertyName = "ConstructionRegionName";
            dataGridViewCellStyle14.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.clParts_E.DefaultCellStyle = dataGridViewCellStyle14;
            this.clParts_E.HeaderText = "部品";
            this.clParts_E.MinimumWidth = 24;
            this.clParts_E.Name = "clParts_E";
            this.clParts_E.ReadOnly = true;
            this.clParts_E.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.clParts_E.Width = 70;
            // 
            // clCurrSpec_E
            // 
            this.clCurrSpec_E.DataPropertyName = "CurrentSpecification";
            dataGridViewCellStyle15.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.clCurrSpec_E.DefaultCellStyle = dataGridViewCellStyle15;
            this.clCurrSpec_E.HeaderText = "現状仕様";
            this.clCurrSpec_E.MinimumWidth = 24;
            this.clCurrSpec_E.Name = "clCurrSpec_E";
            this.clCurrSpec_E.ReadOnly = true;
            this.clCurrSpec_E.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.clCurrSpec_E.Width = 90;
            // 
            // clTemp_E
            // 
            this.clTemp_E.DataPropertyName = "RepairConstructionContentName";
            dataGridViewCellStyle16.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.clTemp_E.DefaultCellStyle = dataGridViewCellStyle16;
            this.clTemp_E.HeaderText = "(修繕工事内容)";
            this.clTemp_E.MinimumWidth = 24;
            this.clTemp_E.Name = "clTemp_E";
            this.clTemp_E.ReadOnly = true;
            this.clTemp_E.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.clTemp_E.Width = 180;
            // 
            // clRemarks_E
            // 
            this.clRemarks_E.DataPropertyName = "Memo";
            dataGridViewCellStyle17.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.clRemarks_E.DefaultCellStyle = dataGridViewCellStyle17;
            this.clRemarks_E.HeaderText = "メモ";
            this.clRemarks_E.MinimumWidth = 24;
            this.clRemarks_E.Name = "clRemarks_E";
            this.clRemarks_E.ReadOnly = true;
            this.clRemarks_E.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.clRemarks_E.Width = 180;
            // 
            // clUpdateHistory_E
            // 
            this.clUpdateHistory_E.HeaderText = "変更履歴";
            this.clUpdateHistory_E.MinimumWidth = 24;
            this.clUpdateHistory_E.Name = "clUpdateHistory_E";
            this.clUpdateHistory_E.ReadOnly = true;
            this.clUpdateHistory_E.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.clUpdateHistory_E.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.clUpdateHistory_E.Text = "変更履歴";
            this.clUpdateHistory_E.UseColumnTextForButtonValue = true;
            this.clUpdateHistory_E.Width = 80;
            // 
            // CtrRepairPlan_E
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gridcRepairList_Equipment);
            this.Controls.Add(this.btnDeselectAll);
            this.Controls.Add(this.btnAdd);
            this.Name = "CtrRepairPlan_E";
            this.Size = new System.Drawing.Size(802, 383);
            ((System.ComponentModel.ISupportInitialize)(this.gridcRepairList_Equipment)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
		private System.Windows.Forms.Button btnAdd;
		private System.Windows.Forms.Button btnDeselectAll;
        private COMMON.ui.DataGridViewEx gridcRepairList_Equipment;
        private System.Windows.Forms.DataGridViewTextBoxColumn clDisplayOrder_E;
        private System.Windows.Forms.DataGridViewButtonColumn clDetail_E;
        private System.Windows.Forms.DataGridViewCheckBoxColumn clSelect_E;
        private System.Windows.Forms.DataGridViewTextBoxColumn clSystem_E;
        private System.Windows.Forms.DataGridViewTextBoxColumn clRepairType_E;
        private System.Windows.Forms.DataGridViewTextBoxColumn clSpec_E;
        private System.Windows.Forms.DataGridViewTextBoxColumn clDivision_E;
        private System.Windows.Forms.DataGridViewTextBoxColumn clChild;
        private System.Windows.Forms.DataGridViewTextBoxColumn clRepairPeriod_E;
        private System.Windows.Forms.DataGridViewTextBoxColumn clPrice_E;
        private System.Windows.Forms.DataGridViewTextBoxColumn clAmount_E;
        private System.Windows.Forms.DataGridViewComboBoxColumn clUnit_E;
        private System.Windows.Forms.DataGridViewTextBoxColumn clTempBox_E;
        private System.Windows.Forms.DataGridViewTextBoxColumn clPositionUnit;
        private System.Windows.Forms.DataGridViewTextBoxColumn clPosition_E;
        private System.Windows.Forms.DataGridViewTextBoxColumn clPartNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn clParts_E;
        private System.Windows.Forms.DataGridViewTextBoxColumn clCurrSpec_E;
        private System.Windows.Forms.DataGridViewTextBoxColumn clTemp_E;
        private System.Windows.Forms.DataGridViewTextBoxColumn clRemarks_E;
        private System.Windows.Forms.DataGridViewButtonColumn clUpdateHistory_E;
    }
}
