namespace PS4_Cheater
{
    using System.Drawing;
    using System.Windows.Forms;
    partial class HexEdit
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
            this.hexBox = new Be.Windows.Forms.HexBox();
            this.previous_btn = new System.Windows.Forms.Button();
            this.next_btn = new System.Windows.Forms.Button();
            this.msg = new System.Windows.Forms.Label();
            this.page_list = new System.Windows.Forms.ComboBox();
            this.commit_btn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // hexBox
            // 
            this.hexBox.Dock = System.Windows.Forms.DockStyle.Left;
            this.hexBox.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.hexBox.LineInfoVisible = true;
            this.hexBox.Location = new System.Drawing.Point(0, 0);
            this.hexBox.Name = "hexBox";
            this.hexBox.ShadowSelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(60)))), ((int)(((byte)(188)))), ((int)(((byte)(255)))));
            this.hexBox.ShadowSelectionVisible = false;
            this.hexBox.Size = new System.Drawing.Size(652, 581);
            this.hexBox.StringViewVisible = true;
            this.hexBox.TabIndex = 0;
            this.hexBox.UseFixedBytesPerLine = true;
            this.hexBox.VScrollBarVisible = true;
            // 
            // previous_btn
            // 
            this.previous_btn.Location = new System.Drawing.Point(658, 45);
            this.previous_btn.Name = "previous_btn";
            this.previous_btn.Size = new System.Drawing.Size(113, 23);
            this.previous_btn.TabIndex = 1;
            this.previous_btn.Text = "Previous";
            this.previous_btn.UseVisualStyleBackColor = true;
            this.previous_btn.Click += new System.EventHandler(this.previous_btn_Click);
            // 
            // next_btn
            // 
            this.next_btn.Location = new System.Drawing.Point(658, 87);
            this.next_btn.Name = "next_btn";
            this.next_btn.Size = new System.Drawing.Size(113, 23);
            this.next_btn.TabIndex = 2;
            this.next_btn.Text = "Next";
            this.next_btn.UseVisualStyleBackColor = true;
            this.next_btn.Click += new System.EventHandler(this.next_btn_Click);
            // 
            // msg
            // 
            this.msg.AutoSize = true;
            this.msg.Location = new System.Drawing.Point(656, 531);
            this.msg.Name = "msg";
            this.msg.Size = new System.Drawing.Size(0, 12);
            this.msg.TabIndex = 3;
            // 
            // page_list
            // 
            this.page_list.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.page_list.FormattingEnabled = true;
            this.page_list.Location = new System.Drawing.Point(660, 133);
            this.page_list.Name = "page_list";
            this.page_list.Size = new System.Drawing.Size(111, 20);
            this.page_list.TabIndex = 4;
            this.page_list.SelectedIndexChanged += new System.EventHandler(this.page_list_SelectedIndexChanged);
            // 
            // commit_btn
            // 
            this.commit_btn.Location = new System.Drawing.Point(660, 199);
            this.commit_btn.Name = "commit_btn";
            this.commit_btn.Size = new System.Drawing.Size(111, 23);
            this.commit_btn.TabIndex = 9;
            this.commit_btn.Text = "Commit";
            this.commit_btn.UseVisualStyleBackColor = true;
            this.commit_btn.Click += new System.EventHandler(this.commit_btn_Click);
            // 
            // HexEdit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(777, 581);
            this.Controls.Add(this.commit_btn);
            this.Controls.Add(this.page_list);
            this.Controls.Add(this.msg);
            this.Controls.Add(this.next_btn);
            this.Controls.Add(this.previous_btn);
            this.Controls.Add(this.hexBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "HexEdit";
            this.Text = "HexEditor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.HexEdit_FormClosing);
            this.Load += new System.EventHandler(this.HexEdit_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Be.Windows.Forms.HexBox hexBox;
        private Button previous_btn;
        private Button next_btn;
        private Label msg;
        private ComboBox page_list;
        private Button commit_btn;
    }
}