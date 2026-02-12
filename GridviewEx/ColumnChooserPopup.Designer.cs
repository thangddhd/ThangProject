
namespace coms.COMMON.ui
{
    partial class ColumnChooserPopup
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
        /// Initializes designer components.
        /// This implementation is intentionally minimal because the runtime UI is created
        /// in BuildUI() inside the main class. Keeping InitializeComponent avoids designer errors.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.SuspendLayout();
            // 
            // ColumnChooserPopup
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            // default size — BuildUI may override; kept small for designer safety
            this.ClientSize = new System.Drawing.Size(260, 400);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "ColumnChooserPopup";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Column Chooser";
            this.ResumeLayout(false);
        }

        #endregion
    }
}