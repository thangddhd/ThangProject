using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Forms;

namespace coms
{
    public partial class MyForm : Form
    {
        public coms.COMMON.ui.ColumnChooserPopup _chooserPopup;
        //static int countForm = 0;
        public MyForm()
        {
            Load += MyForm_Load;
            Shown += new EventHandler(MyForm_Shown);//canh 20140604
            //FormClosing += MyForm_FormClosing;
            //FormClosed += MyForm_FormClosed;
        }

        #region canh 20140604
        void MyForm_Shown(object sender, EventArgs e)
        {
            if (((System.Windows.Forms.Form)(this)).Text != "P100007030ドキュメント管理" && ((System.Windows.Forms.Form)(this)).Text != "B100008900ドキュメント管理")
            {
                this.TopMost = true;
                System.Threading.Thread.Sleep(500);
                this.TopMost = false;
            }
        }
        #endregion canh 20140604

        void MyForm_Load(object sender, EventArgs e)
        {
            //this.Icon = ((System.Drawing.Icon)(global::GridviewEx.Properties.Resources.coms));
            //countForm++;
        }

        void MyForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            //countForm--;            
        }

        void MyForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            //if (countForm < 3)
            //{
            //    Application.Exit();
            //}
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // MyForm
            // 
            this.ClientSize = new System.Drawing.Size(292, 266);
            this.Name = "MyForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.ResumeLayout(false);

        }
    }
}