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
            this.status_strip = new System.Windows.Forms.StatusStrip();
            this.progress_bar = new System.Windows.Forms.ToolStripProgressBar();
            this.msg = new System.Windows.Forms.ToolStripStatusLabel();
            this.address_box = new System.Windows.Forms.TextBox();
            this.find_btn = new System.Windows.Forms.Button();
            this.level_updown = new System.Windows.Forms.DomainUpDown();
            this.pointer_finder_worker = new System.ComponentModel.BackgroundWorker();
            this.pointer_list_view = new System.Windows.Forms.DataGridView();
            this.next_btn = new System.Windows.Forms.Button();
            this.next_pointer_finder_worker = new System.ComponentModel.BackgroundWorker();
            this.label1 = new System.Windows.Forms.Label();
            this.fast_scan_box = new System.Windows.Forms.CheckBox();
            this.status_strip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pointer_list_view)).BeginInit();
            this.SuspendLayout();
            // 
            // status_strip
            // 
            this.status_strip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.progress_bar,
            this.msg});
            this.status_strip.Location = new System.Drawing.Point(0, 399);
            this.status_strip.Name = "status_strip";
            this.status_strip.Size = new System.Drawing.Size(851, 22);
            this.status_strip.TabIndex = 2;
            this.status_strip.Text = "statusStrip1";
            // 
            // progress_bar
            // 
            this.progress_bar.Name = "progress_bar";
            this.progress_bar.Size = new System.Drawing.Size(600, 16);
            // 
            // msg
            // 
            this.msg.Name = "msg";
            this.msg.Size = new System.Drawing.Size(0, 17);
            // 
            // address_box
            // 
            this.address_box.Location = new System.Drawing.Point(202, 10);
            this.address_box.Name = "address_box";
            this.address_box.Size = new System.Drawing.Size(133, 21);
            this.address_box.TabIndex = 3;
            // 
            // find_btn
            // 
            this.find_btn.Location = new System.Drawing.Point(351, 8);
            this.find_btn.Name = "find_btn";
            this.find_btn.Size = new System.Drawing.Size(96, 23);
            this.find_btn.TabIndex = 4;
            this.find_btn.Text = "First Scan";
            this.find_btn.UseVisualStyleBackColor = true;
            this.find_btn.Click += new System.EventHandler(this.find_btn_Click);
            // 
            // level_updown
            // 
            this.level_updown.Location = new System.Drawing.Point(11, 11);
            this.level_updown.Name = "level_updown";
            this.level_updown.ReadOnly = true;
            this.level_updown.Size = new System.Drawing.Size(120, 21);
            this.level_updown.TabIndex = 6;
            // 
            // pointer_finder_worker
            // 
            this.pointer_finder_worker.WorkerReportsProgress = true;
            this.pointer_finder_worker.WorkerSupportsCancellation = true;
            this.pointer_finder_worker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.pointer_finder_worker_DoWork);
            this.pointer_finder_worker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.pointer_finder_worker_ProgressChanged);
            this.pointer_finder_worker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.pointer_finder_worker_RunWorkerCompleted);
            // 
            // pointer_list_view
            // 
            this.pointer_list_view.AllowUserToAddRows = false;
            this.pointer_list_view.AllowUserToDeleteRows = false;
            this.pointer_list_view.AllowUserToResizeRows = false;
            this.pointer_list_view.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pointer_list_view.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.pointer_list_view.Location = new System.Drawing.Point(0, 37);
            this.pointer_list_view.Name = "pointer_list_view";
            this.pointer_list_view.ReadOnly = true;
            this.pointer_list_view.RowTemplate.Height = 23;
            this.pointer_list_view.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.pointer_list_view.Size = new System.Drawing.Size(851, 362);
            this.pointer_list_view.TabIndex = 7;
            this.pointer_list_view.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.pointer_list_view_CellDoubleClick);
            // 
            // next_btn
            // 
            this.next_btn.Location = new System.Drawing.Point(471, 8);
            this.next_btn.Name = "next_btn";
            this.next_btn.Size = new System.Drawing.Size(96, 23);
            this.next_btn.TabIndex = 8;
            this.next_btn.Text = "Next Scan";
            this.next_btn.UseVisualStyleBackColor = true;
            this.next_btn.Click += new System.EventHandler(this.next_btn_Click);
            // 
            // next_pointer_finder_worker
            // 
            this.next_pointer_finder_worker.WorkerReportsProgress = true;
            this.next_pointer_finder_worker.WorkerSupportsCancellation = true;
            this.next_pointer_finder_worker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.next_pointer_finder_worker_DoWork);
            this.next_pointer_finder_worker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.next_pointer_finder_worker_ProgressChanged);
            this.next_pointer_finder_worker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.next_pointer_finder_worker_RunWorkerCompleted);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(143, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 9;
            this.label1.Text = "Address:";
            // 
            // fast_scan_box
            // 
            this.fast_scan_box.AutoSize = true;
            this.fast_scan_box.Checked = true;
            this.fast_scan_box.CheckState = System.Windows.Forms.CheckState.Checked;
            this.fast_scan_box.Location = new System.Drawing.Point(706, 12);
            this.fast_scan_box.Name = "fast_scan_box";
            this.fast_scan_box.Size = new System.Drawing.Size(72, 16);
            this.fast_scan_box.TabIndex = 10;
            this.fast_scan_box.Text = "FastScan";
            this.fast_scan_box.UseVisualStyleBackColor = true;
            this.fast_scan_box.CheckedChanged += new System.EventHandler(this.fast_scan_box_CheckedChanged);
            // 
            // PointerFinder
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(851, 421);
            this.Controls.Add(this.fast_scan_box);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.next_btn);
            this.Controls.Add(this.pointer_list_view);
            this.Controls.Add(this.level_updown);
            this.Controls.Add(this.find_btn);
            this.Controls.Add(this.address_box);
            this.Controls.Add(this.status_strip);
            this.Name = "PointerFinder";
            this.Text = "Pointer Finder";
            this.Load += new System.EventHandler(this.PointerFinder_Load);
            this.status_strip.ResumeLayout(false);
            this.status_strip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pointer_list_view)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.StatusStrip status_strip;
        private System.Windows.Forms.TextBox address_box;
        private System.Windows.Forms.Button find_btn;
        private System.Windows.Forms.ToolStripProgressBar progress_bar;
        private System.Windows.Forms.DomainUpDown level_updown;
        private System.ComponentModel.BackgroundWorker pointer_finder_worker;
        private System.Windows.Forms.DataGridView pointer_list_view;
        private System.Windows.Forms.Button next_btn;
        private System.ComponentModel.BackgroundWorker next_pointer_finder_worker;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ToolStripStatusLabel msg;
        private System.Windows.Forms.CheckBox fast_scan_box;
    }
}