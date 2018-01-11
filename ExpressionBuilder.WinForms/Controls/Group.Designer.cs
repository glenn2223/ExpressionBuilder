namespace ExpressionBuilder.WinForms.Controls
{
    partial class UcGroup
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.OpeningParenthasies = new System.Windows.Forms.Label();
            this.groupOfFilters = new System.Windows.Forms.FlowLayoutPanel();
            this.ClosingParenthasies = new System.Windows.Forms.Label();
            this.btnRemove = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // OpeningParenthasies
            // 
            this.OpeningParenthasies.AutoSize = true;
            this.OpeningParenthasies.Location = new System.Drawing.Point(3, 5);
            this.OpeningParenthasies.Name = "OpeningParenthasies";
            this.OpeningParenthasies.Size = new System.Drawing.Size(10, 13);
            this.OpeningParenthasies.TabIndex = 0;
            this.OpeningParenthasies.Text = "(";
            // 
            // groupOfFilters
            // 
            this.groupOfFilters.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupOfFilters.AutoSize = true;
            this.groupOfFilters.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.groupOfFilters.Location = new System.Drawing.Point(12, 21);
            this.groupOfFilters.MinimumSize = new System.Drawing.Size(865, 30);
            this.groupOfFilters.Name = "groupOfFilters";
            this.groupOfFilters.Size = new System.Drawing.Size(865, 30);
            this.groupOfFilters.TabIndex = 1;
            this.groupOfFilters.WrapContents = false;
            // 
            // ClosingParenthasies
            // 
            this.ClosingParenthasies.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ClosingParenthasies.AutoSize = true;
            this.ClosingParenthasies.Location = new System.Drawing.Point(3, 54);
            this.ClosingParenthasies.Name = "ClosingParenthasies";
            this.ClosingParenthasies.Size = new System.Drawing.Size(10, 13);
            this.ClosingParenthasies.TabIndex = 2;
            this.ClosingParenthasies.Text = ")";
            // 
            // btnRemove
            // 
            this.btnRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRemove.Location = new System.Drawing.Point(857, 0);
            this.btnRemove.Margin = new System.Windows.Forms.Padding(1);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(27, 20);
            this.btnRemove.TabIndex = 3;
            this.btnRemove.Text = "-";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.BtnRemoveClick);
            // 
            // UcGroup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.btnRemove);
            this.Controls.Add(this.ClosingParenthasies);
            this.Controls.Add(this.groupOfFilters);
            this.Controls.Add(this.OpeningParenthasies);
            this.MinimumSize = new System.Drawing.Size(884, 65);
            this.Name = "UcGroup";
            this.Size = new System.Drawing.Size(884, 74);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label OpeningParenthasies;
        private System.Windows.Forms.Label ClosingParenthasies;
        public System.Windows.Forms.FlowLayoutPanel groupOfFilters;
        internal System.Windows.Forms.Button btnRemove;
    }
}
