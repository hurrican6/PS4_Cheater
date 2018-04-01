namespace PS4_Cheater
{
    using System.Drawing;
    using System.Windows.Forms;
    partial class HexEditor
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.hexBox = new Be.Windows.Forms.HexBox();
            this.refresh_btn = new System.Windows.Forms.Button();
            this.previous_btn = new System.Windows.Forms.Button();
            this.commit_btn = new System.Windows.Forms.Button();
            this.next_btn = new System.Windows.Forms.Button();
            this.page_list = new System.Windows.Forms.ComboBox();
            this.find = new System.Windows.Forms.Button();
            this.input_box = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.hexBox);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.input_box);
            this.splitContainer1.Panel2.Controls.Add(this.find);
            this.splitContainer1.Panel2.Controls.Add(this.refresh_btn);
            this.splitContainer1.Panel2.Controls.Add(this.previous_btn);
            this.splitContainer1.Panel2.Controls.Add(this.commit_btn);
            this.splitContainer1.Panel2.Controls.Add(this.next_btn);
            this.splitContainer1.Panel2.Controls.Add(this.page_list);
            this.splitContainer1.Size = new System.Drawing.Size(825, 581);
            this.splitContainer1.SplitterDistance = 623;
            this.splitContainer1.TabIndex = 4;
            // 
            // hexBox
            // 
            this.hexBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.hexBox.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.hexBox.LineInfoVisible = true;
            this.hexBox.Location = new System.Drawing.Point(0, 0);
            this.hexBox.Name = "hexBox";
            this.hexBox.ShadowSelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(60)))), ((int)(((byte)(188)))), ((int)(((byte)(255)))));
            this.hexBox.ShadowSelectionVisible = false;
            this.hexBox.Size = new System.Drawing.Size(623, 581);
            this.hexBox.StringViewVisible = true;
            this.hexBox.TabIndex = 11;
            this.hexBox.UseFixedBytesPerLine = true;
            this.hexBox.VScrollBarVisible = true;
            // 
            // refresh_btn
            // 
            this.refresh_btn.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.refresh_btn.Location = new System.Drawing.Point(15, 154);
            this.refresh_btn.Name = "refresh_btn";
            this.refresh_btn.Size = new System.Drawing.Size(160, 25);
            this.refresh_btn.TabIndex = 16;
            this.refresh_btn.Text = "Refresh";
            this.refresh_btn.UseVisualStyleBackColor = true;
            this.refresh_btn.Click += new System.EventHandler(this.refresh_btn_Click);
            // 
            // previous_btn
            // 
            this.previous_btn.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.previous_btn.Location = new System.Drawing.Point(15, 34);
            this.previous_btn.Name = "previous_btn";
            this.previous_btn.Size = new System.Drawing.Size(160, 25);
            this.previous_btn.TabIndex = 12;
            this.previous_btn.Text = "Previous";
            this.previous_btn.UseVisualStyleBackColor = true;
            this.previous_btn.Click += new System.EventHandler(this.previous_btn_Click);
            // 
            // commit_btn
            // 
            this.commit_btn.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.commit_btn.Location = new System.Drawing.Point(15, 194);
            this.commit_btn.Name = "commit_btn";
            this.commit_btn.Size = new System.Drawing.Size(160, 25);
            this.commit_btn.TabIndex = 15;
            this.commit_btn.Text = "Commit";
            this.commit_btn.UseVisualStyleBackColor = true;
            this.commit_btn.Click += new System.EventHandler(this.commit_btn_Click);
            // 
            // next_btn
            // 
            this.next_btn.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.next_btn.Location = new System.Drawing.Point(15, 74);
            this.next_btn.Name = "next_btn";
            this.next_btn.Size = new System.Drawing.Size(160, 25);
            this.next_btn.TabIndex = 13;
            this.next_btn.Text = "Next";
            this.next_btn.UseVisualStyleBackColor = true;
            this.next_btn.Click += new System.EventHandler(this.next_btn_Click);
            // 
            // page_list
            // 
            this.page_list.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.page_list.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.page_list.FormattingEnabled = true;
            this.page_list.Location = new System.Drawing.Point(15, 114);
            this.page_list.Name = "page_list";
            this.page_list.Size = new System.Drawing.Size(160, 20);
            this.page_list.TabIndex = 14;
            this.page_list.SelectedIndexChanged += new System.EventHandler(this.page_list_SelectedIndexChanged);
            // 
            // find
            // 
            this.find.Location = new System.Drawing.Point(15, 327);
            this.find.Name = "find";
            this.find.Size = new System.Drawing.Size(160, 23);
            this.find.TabIndex = 17;
            this.find.Text = "Find";
            this.find.UseVisualStyleBackColor = true;
            this.find.Click += new System.EventHandler(this.find_Click);
            // 
            // input_box
            // 
            this.input_box.Location = new System.Drawing.Point(15, 281);
            this.input_box.Name = "input_box";
            this.input_box.Size = new System.Drawing.Size(160, 21);
            this.input_box.TabIndex = 18;
            // 
            // HexEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(825, 581);
            this.Controls.Add(this.splitContainer1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "HexEditor";
            this.Text = "HexEditor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.HexEdit_FormClosing);
            this.Load += new System.EventHandler(this.HexEdit_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private SplitContainer splitContainer1;
        private Be.Windows.Forms.HexBox hexBox;
        private Button refresh_btn;
        private Button previous_btn;
        private Button commit_btn;
        private Button next_btn;
        private ComboBox page_list;
        private TextBox input_box;
        private Button find;
    }
}