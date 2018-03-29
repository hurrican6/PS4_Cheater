namespace PS4_Cheater
{
    partial class NewAddress
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
            this.components = new System.ComponentModel.Container();
            this.label1 = new System.Windows.Forms.Label();
            this.address_box = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.type_box = new System.Windows.Forms.ComboBox();
            this.value_box = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.save_btn = new System.Windows.Forms.Button();
            this.description_box = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cancel_btn = new System.Windows.Forms.Button();
            this.lock_box = new System.Windows.Forms.CheckBox();
            this.pointer_box = new System.Windows.Forms.CheckBox();
            this.PointerCheckerPointer = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(2, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "Address";
            // 
            // address_box
            // 
            this.address_box.Location = new System.Drawing.Point(78, 12);
            this.address_box.Name = "address_box";
            this.address_box.Size = new System.Drawing.Size(121, 21);
            this.address_box.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(2, 42);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "Type";
            // 
            // type_box
            // 
            this.type_box.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.type_box.FormattingEnabled = true;
            this.type_box.Location = new System.Drawing.Point(78, 39);
            this.type_box.Name = "type_box";
            this.type_box.Size = new System.Drawing.Size(121, 20);
            this.type_box.TabIndex = 3;
            // 
            // value_box
            // 
            this.value_box.Location = new System.Drawing.Point(264, 12);
            this.value_box.Name = "value_box";
            this.value_box.Size = new System.Drawing.Size(121, 21);
            this.value_box.TabIndex = 7;
            this.value_box.Text = "0";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(223, 15);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(35, 12);
            this.label4.TabIndex = 6;
            this.label4.Text = "Value";
            // 
            // save_btn
            // 
            this.save_btn.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.save_btn.Location = new System.Drawing.Point(40, 110);
            this.save_btn.Name = "save_btn";
            this.save_btn.Size = new System.Drawing.Size(121, 25);
            this.save_btn.TabIndex = 10;
            this.save_btn.Text = "Save";
            this.save_btn.UseVisualStyleBackColor = true;
            this.save_btn.Click += new System.EventHandler(this.save_btn_Click);
            // 
            // description_box
            // 
            this.description_box.Location = new System.Drawing.Point(78, 65);
            this.description_box.Name = "description_box";
            this.description_box.Size = new System.Drawing.Size(307, 21);
            this.description_box.TabIndex = 12;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(2, 68);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(71, 12);
            this.label3.TabIndex = 11;
            this.label3.Text = "Description";
            // 
            // cancel_btn
            // 
            this.cancel_btn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancel_btn.Location = new System.Drawing.Point(240, 110);
            this.cancel_btn.Name = "cancel_btn";
            this.cancel_btn.Size = new System.Drawing.Size(123, 25);
            this.cancel_btn.TabIndex = 13;
            this.cancel_btn.Text = "Close";
            this.cancel_btn.UseVisualStyleBackColor = true;
            this.cancel_btn.Click += new System.EventHandler(this.cancel_btn_Click);
            // 
            // lock_box
            // 
            this.lock_box.AutoSize = true;
            this.lock_box.Location = new System.Drawing.Point(264, 38);
            this.lock_box.Name = "lock_box";
            this.lock_box.Size = new System.Drawing.Size(48, 16);
            this.lock_box.TabIndex = 15;
            this.lock_box.Text = "Lock";
            this.lock_box.UseVisualStyleBackColor = true;
            // 
            // pointer_box
            // 
            this.pointer_box.AutoSize = true;
            this.pointer_box.Location = new System.Drawing.Point(78, 92);
            this.pointer_box.Name = "pointer_box";
            this.pointer_box.Size = new System.Drawing.Size(66, 16);
            this.pointer_box.TabIndex = 16;
            this.pointer_box.Text = "Pointer";
            this.pointer_box.UseVisualStyleBackColor = true;
            this.pointer_box.CheckedChanged += new System.EventHandler(this.pointer_box_CheckedChanged);
            // 
            // PointerCheckerPointer
            // 
            this.PointerCheckerPointer.Enabled = true;
            this.PointerCheckerPointer.Interval = 500;
            this.PointerCheckerPointer.Tick += new System.EventHandler(this.PointerCheckerPointer_Tick);
            // 
            // NewAddress
            // 
            this.AcceptButton = this.save_btn;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.CancelButton = this.cancel_btn;
            this.ClientSize = new System.Drawing.Size(398, 140);
            this.ControlBox = false;
            this.Controls.Add(this.pointer_box);
            this.Controls.Add(this.lock_box);
            this.Controls.Add(this.cancel_btn);
            this.Controls.Add(this.description_box);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.save_btn);
            this.Controls.Add(this.value_box);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.type_box);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.address_box);
            this.Controls.Add(this.label1);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "NewAddress";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "NewAddress";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.NewAddress_Load);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.NewAddress_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.NewAddress_MouseMove);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox address_box;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox type_box;
        private System.Windows.Forms.TextBox value_box;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button save_btn;
        private System.Windows.Forms.TextBox description_box;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button cancel_btn;
        private System.Windows.Forms.CheckBox lock_box;
        private System.Windows.Forms.CheckBox pointer_box;
        private System.Windows.Forms.Timer PointerCheckerPointer;
    }
}