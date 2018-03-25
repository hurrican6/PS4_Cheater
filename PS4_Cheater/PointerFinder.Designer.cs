namespace PS4_Cheater
{
    partial class PointerFinder
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PointerFinder));
            this.pointer_list_view = new System.Windows.Forms.ListView();
            this.status_strip = new System.Windows.Forms.StatusStrip();
            this.msg = new System.Windows.Forms.ToolStripStatusLabel();
            this.splitor_0 = new System.Windows.Forms.ToolStripSplitButton();
            this.progress_bar = new System.Windows.Forms.ToolStripProgressBar();
            this.address_box = new System.Windows.Forms.TextBox();
            this.find_btn = new System.Windows.Forms.Button();
            this.init_btn = new System.Windows.Forms.Button();
            this.level_updown = new System.Windows.Forms.DomainUpDown();
            this.peek_worker = new System.ComponentModel.BackgroundWorker();
            this.pointer_finder_worker = new System.ComponentModel.BackgroundWorker();
            this.status_strip.SuspendLayout();
            this.SuspendLayout();
            // 
            // pointer_list_view
            // 
            this.pointer_list_view.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pointer_list_view.FullRowSelect = true;
            this.pointer_list_view.GridLines = true;
            this.pointer_list_view.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.pointer_list_view.Location = new System.Drawing.Point(0, 39);
            this.pointer_list_view.Name = "pointer_list_view";
            this.pointer_list_view.Size = new System.Drawing.Size(851, 360);
            this.pointer_list_view.TabIndex = 1;
            this.pointer_list_view.UseCompatibleStateImageBehavior = false;
            this.pointer_list_view.View = System.Windows.Forms.View.Details;
            // 
            // status_strip
            // 
            this.status_strip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.msg,
            this.splitor_0,
            this.progress_bar});
            this.status_strip.Location = new System.Drawing.Point(0, 399);
            this.status_strip.Name = "status_strip";
            this.status_strip.Size = new System.Drawing.Size(851, 22);
            this.status_strip.TabIndex = 2;
            this.status_strip.Text = "statusStrip1";
            // 
            // msg
            // 
            this.msg.Name = "msg";
            this.msg.Size = new System.Drawing.Size(0, 17);
            // 
            // splitor_0
            // 
            this.splitor_0.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.splitor_0.Image = ((System.Drawing.Image)(resources.GetObject("splitor_0.Image")));
            this.splitor_0.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.splitor_0.Name = "splitor_0";
            this.splitor_0.Size = new System.Drawing.Size(32, 20);
            this.splitor_0.Text = "toolStripSplitButton1";
            // 
            // progress_bar
            // 
            this.progress_bar.Name = "progress_bar";
            this.progress_bar.Size = new System.Drawing.Size(200, 16);
            // 
            // address_box
            // 
            this.address_box.Location = new System.Drawing.Point(305, 8);
            this.address_box.Name = "address_box";
            this.address_box.Size = new System.Drawing.Size(133, 21);
            this.address_box.TabIndex = 3;
            // 
            // find_btn
            // 
            this.find_btn.Location = new System.Drawing.Point(454, 6);
            this.find_btn.Name = "find_btn";
            this.find_btn.Size = new System.Drawing.Size(96, 23);
            this.find_btn.TabIndex = 4;
            this.find_btn.Text = "Find";
            this.find_btn.UseVisualStyleBackColor = true;
            this.find_btn.Click += new System.EventHandler(this.find_btn_Click);
            // 
            // init_btn
            // 
            this.init_btn.Location = new System.Drawing.Point(24, 8);
            this.init_btn.Name = "init_btn";
            this.init_btn.Size = new System.Drawing.Size(99, 23);
            this.init_btn.TabIndex = 5;
            this.init_btn.Text = "Init";
            this.init_btn.UseVisualStyleBackColor = true;
            this.init_btn.Click += new System.EventHandler(this.init_btn_Click);
            // 
            // level_updown
            // 
            this.level_updown.Location = new System.Drawing.Point(162, 9);
            this.level_updown.Name = "level_updown";
            this.level_updown.Size = new System.Drawing.Size(120, 21);
            this.level_updown.TabIndex = 6;
            // 
            // peek_worker
            // 
            this.peek_worker.WorkerReportsProgress = true;
            this.peek_worker.WorkerSupportsCancellation = true;
            this.peek_worker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.peek_worker_DoWork);
            this.peek_worker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.peek_worker_ProgressChanged);
            this.peek_worker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.peek_worker_RunWorkerCompleted);
            // 
            // pointer_finder_worker
            // 
            this.pointer_finder_worker.WorkerReportsProgress = true;
            this.pointer_finder_worker.WorkerSupportsCancellation = true;
            this.pointer_finder_worker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.pointer_finder_worker_DoWork);
            this.pointer_finder_worker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.pointer_finder_worker_ProgressChanged);
            this.pointer_finder_worker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.pointer_finder_worker_RunWorkerCompleted);
            // 
            // PointerFinder
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(851, 421);
            this.Controls.Add(this.level_updown);
            this.Controls.Add(this.init_btn);
            this.Controls.Add(this.find_btn);
            this.Controls.Add(this.address_box);
            this.Controls.Add(this.pointer_list_view);
            this.Controls.Add(this.status_strip);
            this.Name = "PointerFinder";
            this.Text = "Pointer Finder";
            this.Load += new System.EventHandler(this.PointerFinder_Load);
            this.status_strip.ResumeLayout(false);
            this.status_strip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView pointer_list_view;
        private System.Windows.Forms.StatusStrip status_strip;
        private System.Windows.Forms.TextBox address_box;
        private System.Windows.Forms.Button find_btn;
        private System.Windows.Forms.Button init_btn;
        private System.Windows.Forms.ToolStripStatusLabel msg;
        private System.Windows.Forms.ToolStripSplitButton splitor_0;
        private System.Windows.Forms.ToolStripProgressBar progress_bar;
        private System.Windows.Forms.DomainUpDown level_updown;
        private System.ComponentModel.BackgroundWorker peek_worker;
        private System.ComponentModel.BackgroundWorker pointer_finder_worker;
    }
}