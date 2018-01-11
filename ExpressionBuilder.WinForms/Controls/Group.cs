using System;
using System.Windows.Forms;

namespace ExpressionBuilder.WinForms.Controls
{
    public partial class UcGroup : UserControl
    {
        public event EventHandler OnRemove;

        public UcGroup()
        {
            InitializeComponent();
        }

        private void BtnRemoveClick(object sender, EventArgs e)
        {
            OnRemove(sender, e);
        }
    }
}
