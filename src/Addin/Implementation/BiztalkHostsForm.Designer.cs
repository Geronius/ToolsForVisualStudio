using System.Windows.Forms;

namespace DeploymentFrameworkForBizTalk.Addin.Implementation
{
    partial class BiztalkHostsForm
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BiztalkHostsForm));
            this.lstHosts = new System.Windows.Forms.CheckedListBox();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.btnCanl = new System.Windows.Forms.Button();
            this.btnAttach = new System.Windows.Forms.Button();
            this.cbBounceHost = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // lstHosts
            // 
            this.lstHosts.CheckOnClick = true;
            this.lstHosts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstHosts.FormattingEnabled = true;
            this.lstHosts.Location = new System.Drawing.Point(0, 0);
            this.lstHosts.Name = "lstHosts";
            this.lstHosts.Size = new System.Drawing.Size(284, 235);
            this.lstHosts.Sorted = true;
            this.lstHosts.TabIndex = 0;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Margin = new System.Windows.Forms.Padding(0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.lstHosts);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.flowLayoutPanel2);
            this.splitContainer2.Size = new System.Drawing.Size(284, 261);
            this.splitContainer2.SplitterDistance = 235;
            this.splitContainer2.SplitterWidth = 1;
            this.splitContainer2.TabIndex = 1;
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.flowLayoutPanel2.Controls.Add(this.btnCanl);
            this.flowLayoutPanel2.Controls.Add(this.btnAttach);
            this.flowLayoutPanel2.Controls.Add(this.cbBounceHost);
            this.flowLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel2.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel2.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.flowLayoutPanel2.Size = new System.Drawing.Size(284, 25);
            this.flowLayoutPanel2.TabIndex = 0;
            // 
            // btnCanl
            // 
            this.btnCanl.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCanl.Location = new System.Drawing.Point(209, 0);
            this.btnCanl.Margin = new System.Windows.Forms.Padding(0);
            this.btnCanl.Name = "btnCanl";
            this.btnCanl.Size = new System.Drawing.Size(75, 23);
            this.btnCanl.TabIndex = 0;
            this.btnCanl.Text = "&Cancel";
            this.btnCanl.UseVisualStyleBackColor = true;
            // 
            // btnAttach
            // 
            this.btnAttach.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnAttach.Location = new System.Drawing.Point(134, 0);
            this.btnAttach.Margin = new System.Windows.Forms.Padding(0);
            this.btnAttach.Name = "btnAttach";
            this.btnAttach.Size = new System.Drawing.Size(75, 23);
            this.btnAttach.TabIndex = 1;
            this.btnAttach.Text = "&Attach";
            this.btnAttach.UseVisualStyleBackColor = true;
            this.btnAttach.Click += new System.EventHandler(this.btnAttach_Click);
            // 
            // cbBounceHost
            // 
            this.cbBounceHost.AutoSize = true;
            this.cbBounceHost.Checked = global::DeploymentFrameworkForBizTalk.Addin.Properties.Settings.Default.BounceHost;
            this.cbBounceHost.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbBounceHost.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::DeploymentFrameworkForBizTalk.Addin.Properties.Settings.Default, "BounceHost", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cbBounceHost.Location = new System.Drawing.Point(46, 3);
            this.cbBounceHost.Name = "cbBounceHost";
            this.cbBounceHost.Size = new System.Drawing.Size(85, 17);
            this.cbBounceHost.TabIndex = 2;
            this.cbBounceHost.Text = "&BounceHost";
            this.cbBounceHost.UseVisualStyleBackColor = true;
            // 
            // BiztalkHostsForm
            // 
            this.AcceptButton = this.btnAttach;
            this.CancelButton = this.btnCanl;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.splitContainer2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimizeBox = false;
            this.Name = "BiztalkHostsForm";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.BiztalkHostsForm_FormClosed);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.flowLayoutPanel2.ResumeLayout(false);
            this.flowLayoutPanel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private SplitContainer splitContainer1;
        private StatusStrip statusStrip;
        private Button btnExecute;
        private Button btnToggleAll;
        private CheckBox cbRecycleAppPools;
        private CheckBox cbStopHosts;
        private CheckBox cbStartHosts;
        private Button btnCancel;
        private ToolStripStatusLabel lblStatus;
        private FlowLayoutPanel flowLayoutPanel1;
        private CheckedListBox lstHosts;
        private SplitContainer splitContainer2;
        private FlowLayoutPanel flowLayoutPanel2;
        private Button btnCanl;
        private Button btnAttach;
        private CheckBox cbBounceHost;
    }
}