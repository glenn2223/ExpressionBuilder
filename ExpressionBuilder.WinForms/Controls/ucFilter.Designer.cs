/*
 * Created by SharpDevelop.
 * User: dcbelmont
 * Date: 18/02/2016
 * Time: 14:51
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace ExpressionBuilder.WinForms.Controls
{
	partial class UcFilter
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		private System.Windows.Forms.ComboBox cbProperties;
		private System.Windows.Forms.ComboBox cbOperations;
		private System.Windows.Forms.ComboBox cbConector;
		private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.ContextMenuStrip cmAddRightClick;
        private System.Windows.Forms.ToolStripMenuItem miAddGroup;

        /// <summary>
        /// Disposes resources used by the control.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
		{
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            this.cbProperties = new System.Windows.Forms.ComboBox();
            this.cbOperations = new System.Windows.Forms.ComboBox();
            this.cbConector = new System.Windows.Forms.ComboBox();
            this.btnAdd = new System.Windows.Forms.Button();
            this.cmAddRightClick = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.miAddGroup = new System.Windows.Forms.ToolStripMenuItem();
            this.btnRemove = new System.Windows.Forms.Button();
            this.valuesContainer = new System.Windows.Forms.FlowLayoutPanel();
            this.cbMatchTypes = new System.Windows.Forms.ComboBox();
            this.cmAddRightClick.SuspendLayout();
            this.SuspendLayout();
            // 
            // cbProperties
            // 
            this.cbProperties.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbProperties.FormattingEnabled = true;
            this.cbProperties.Location = new System.Drawing.Point(4, 4);
            this.cbProperties.MaximumSize = new System.Drawing.Size(164, 0);
            this.cbProperties.Name = "cbProperties";
            this.cbProperties.Size = new System.Drawing.Size(164, 21);
            this.cbProperties.TabIndex = 0;
            // 
            // cbOperations
            // 
            this.cbOperations.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbOperations.FormattingEnabled = true;
            this.cbOperations.Location = new System.Drawing.Point(170, 4);
            this.cbOperations.MaximumSize = new System.Drawing.Size(145, 0);
            this.cbOperations.Name = "cbOperations";
            this.cbOperations.Size = new System.Drawing.Size(145, 21);
            this.cbOperations.TabIndex = 1;
            this.cbOperations.SelectedIndexChanged += new System.EventHandler(this.cbOperations_SelectedIndexChanged);
            // 
            // cbConector
            // 
            this.cbConector.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cbConector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbConector.FormattingEnabled = true;
            this.cbConector.Items.AddRange(new object[] {
            "And",
            "Or"});
            this.cbConector.Location = new System.Drawing.Point(779, 3);
            this.cbConector.Name = "cbConector";
            this.cbConector.Size = new System.Drawing.Size(40, 21);
            this.cbConector.TabIndex = 3;
            // 
            // btnAdd
            // 
            this.btnAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAdd.ContextMenuStrip = this.cmAddRightClick;
            this.btnAdd.Location = new System.Drawing.Point(825, 3);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(25, 20);
            this.btnAdd.TabIndex = 4;
            this.btnAdd.Text = "+";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.BtnAddClick);
            // 
            // cmAddRightClick
            // 
            this.cmAddRightClick.BackColor = System.Drawing.Color.Transparent;
            this.cmAddRightClick.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miAddGroup});
            this.cmAddRightClick.Name = "Add Group";
            this.cmAddRightClick.Size = new System.Drawing.Size(133, 26);
            // 
            // miAddGroup
            // 
            this.miAddGroup.Name = "miAddGroup";
            this.miAddGroup.Size = new System.Drawing.Size(132, 22);
            this.miAddGroup.Text = "Add Group";
            this.miAddGroup.Click += new System.EventHandler(this.MiAddGroup_Click);
            // 
            // btnRemove
            // 
            this.btnRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRemove.Location = new System.Drawing.Point(856, 3);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(25, 20);
            this.btnRemove.TabIndex = 5;
            this.btnRemove.Text = "-";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.BtnRemoveClick);
            // 
            // valuesContainer
            // 
            this.valuesContainer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.valuesContainer.AutoSize = true;
            this.valuesContainer.Location = new System.Drawing.Point(321, 4);
            this.valuesContainer.MinimumSize = new System.Drawing.Size(389, 21);
            this.valuesContainer.Name = "valuesContainer";
            this.valuesContainer.Size = new System.Drawing.Size(389, 21);
            this.valuesContainer.TabIndex = 0;
            // 
            // cbMatchTypes
            // 
            this.cbMatchTypes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cbMatchTypes.BackColor = System.Drawing.SystemColors.Window;
            this.cbMatchTypes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbMatchTypes.FormattingEnabled = true;
            this.cbMatchTypes.Location = new System.Drawing.Point(716, 3);
            this.cbMatchTypes.Name = "cbMatchTypes";
            this.cbMatchTypes.Size = new System.Drawing.Size(57, 21);
            this.cbMatchTypes.TabIndex = 6;
            // 
            // UcFilter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.cbMatchTypes);
            this.Controls.Add(this.valuesContainer);
            this.Controls.Add(this.btnRemove);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.cbConector);
            this.Controls.Add(this.cbOperations);
            this.Controls.Add(this.cbProperties);
            this.MinimumSize = new System.Drawing.Size(884, 29);
            this.Name = "UcFilter";
            this.Size = new System.Drawing.Size(884, 29);
            this.Load += new System.EventHandler(this.UcFilterLoad);
            this.cmAddRightClick.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

		}

        private System.Windows.Forms.ComboBox cbMatchTypes;
        internal System.Windows.Forms.Button btnRemove;
        internal System.Windows.Forms.FlowLayoutPanel valuesContainer;
    }
}
