using System.ComponentModel;
using System.Windows.Forms;

namespace PS4_Cheater
{
    partial class main
    {
        private ColumnHeader result_list_view_value;
        private ColumnHeader result_list_view_section;
        private Button refresh_cheat_list_btn;
        private Button load_cheat_list_btn;
        private ColumnHeader result_list_view_hex;
        private Button save_cheat_list_btn;
        private Button new_cheat_list_btn;
        private DataGridView cheat_list_view;
        private CheckedListBox section_list_box;
        private ContextMenuStrip section_list_menu;
        private ToolStripMenuItem section_view_menu;
        private ToolStripMenuItem section_dump_menu;
        private TextBox value_box;
        private Timer refresh_lock;
        private SaveFileDialog save_file_dialog;
        private SplitContainer splitContainer1;
        private SplitContainer splitContainer4;
        private ListView result_list_view;
        private ColumnHeader result_list_view_address;
        private ColumnHeader result_list_view_type;
        private SplitContainer splitContainer2;
        private ComboBox processes_comboBox;
        private TextBox ip_box;
        private TextBox port_box;
        private Button send_payload_btn;
        private Button get_processes_btn;
        private Label msg;
        private CheckBox select_all;
        private ProgressBar progressBar;
        private ComboBox compareTypeList;
        private ComboBox valueTypeList;
        private Label label4;
        private Button next_scan_btn;
        private Button new_scan_btn;
        private Button refresh_btn;
        private BackgroundWorker new_scan_worker;
        private BackgroundWorker next_scan_worker;
        private OpenFileDialog open_file_dialog;
        private Panel parent;
        private IContainer components = null;
        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.parent = new System.Windows.Forms.Panel();
            this.result_list_view_value = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.result_list_view_section = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.refresh_cheat_list_btn = new System.Windows.Forms.Button();
            this.load_cheat_list_btn = new System.Windows.Forms.Button();
            this.result_list_view_hex = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.save_cheat_list_btn = new System.Windows.Forms.Button();
            this.new_cheat_list_btn = new System.Windows.Forms.Button();
            this.cheat_list_view = new System.Windows.Forms.DataGridView();
            this.section_list_box = new System.Windows.Forms.CheckedListBox();
            this.section_list_menu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.section_view_menu = new System.Windows.Forms.ToolStripMenuItem();
            this.section_dump_menu = new System.Windows.Forms.ToolStripMenuItem();
            this.value_box = new System.Windows.Forms.TextBox();
            this.refresh_lock = new System.Windows.Forms.Timer(this.components);
            this.save_file_dialog = new System.Windows.Forms.SaveFileDialog();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer4 = new System.Windows.Forms.SplitContainer();
            this.result_list_view = new System.Windows.Forms.ListView();
            this.result_list_view_address = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.result_list_view_type = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.result_list_menu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.view_item = new System.Windows.Forms.ToolStripMenuItem();
            this.dump_item = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.version_list = new System.Windows.Forms.ComboBox();
            this.processes_comboBox = new System.Windows.Forms.ComboBox();
            this.ip_box = new System.Windows.Forms.TextBox();
            this.port_box = new System.Windows.Forms.TextBox();
            this.send_payload_btn = new System.Windows.Forms.Button();
            this.get_processes_btn = new System.Windows.Forms.Button();
            this.alignment_box = new System.Windows.Forms.CheckBox();
            this.value_1_box = new System.Windows.Forms.TextBox();
            this.value_label = new System.Windows.Forms.Label();
            this.and_label = new System.Windows.Forms.Label();
            this.msg = new System.Windows.Forms.Label();
            this.select_all = new System.Windows.Forms.CheckBox();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.compareTypeList = new System.Windows.Forms.ComboBox();
            this.valueTypeList = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.next_scan_btn = new System.Windows.Forms.Button();
            this.new_scan_btn = new System.Windows.Forms.Button();
            this.refresh_btn = new System.Windows.Forms.Button();
            this.new_scan_worker = new System.ComponentModel.BackgroundWorker();
            this.next_scan_worker = new System.ComponentModel.BackgroundWorker();
            this.open_file_dialog = new System.Windows.Forms.OpenFileDialog();
            this.update_result_list_worker = new System.ComponentModel.BackgroundWorker();
            this.cheat_list_view_active = new System.Windows.Forms.DataGridViewButtonColumn();
            this.cheat_list_view_address = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cheat_list_view_type = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cheat_list_view_value = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cheat_list_view_section = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cheat_list_view_lock = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.cheat_list_view_description = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.cheat_list_view)).BeginInit();
            this.section_list_menu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer4)).BeginInit();
            this.splitContainer4.Panel1.SuspendLayout();
            this.splitContainer4.Panel2.SuspendLayout();
            this.splitContainer4.SuspendLayout();
            this.result_list_menu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.SuspendLayout();
            // 
            // parent
            // 
            this.parent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.parent.Location = new System.Drawing.Point(0, 0);
            this.parent.Name = "parent";
            this.parent.Size = new System.Drawing.Size(1011, 619);
            this.parent.TabIndex = 11;
            // 
            // result_list_view_value
            // 
            this.result_list_view_value.Text = "Value";
            this.result_list_view_value.Width = 120;
            // 
            // result_list_view_section
            // 
            this.result_list_view_section.Text = "Section";
            this.result_list_view_section.Width = 180;
            // 
            // refresh_cheat_list_btn
            // 
            this.refresh_cheat_list_btn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(37)))), ((int)(((byte)(38)))));
            this.refresh_cheat_list_btn.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.refresh_cheat_list_btn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.refresh_cheat_list_btn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(84)))), ((int)(((byte)(84)))), ((int)(((byte)(92)))));
            this.refresh_cheat_list_btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.refresh_cheat_list_btn.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.refresh_cheat_list_btn.ForeColor = System.Drawing.Color.White;
            this.refresh_cheat_list_btn.Location = new System.Drawing.Point(179, 2);
            this.refresh_cheat_list_btn.Name = "refresh_cheat_list_btn";
            this.refresh_cheat_list_btn.Size = new System.Drawing.Size(165, 25);
            this.refresh_cheat_list_btn.TabIndex = 31;
            this.refresh_cheat_list_btn.TabStop = false;
            this.refresh_cheat_list_btn.Text = "Refresh";
            this.refresh_cheat_list_btn.UseMnemonic = false;
            this.refresh_cheat_list_btn.UseVisualStyleBackColor = false;
            this.refresh_cheat_list_btn.Click += new System.EventHandler(this.refresh_cheat_list_Click);
            // 
            // load_cheat_list_btn
            // 
            this.load_cheat_list_btn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.load_cheat_list_btn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(37)))), ((int)(((byte)(38)))));
            this.load_cheat_list_btn.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.load_cheat_list_btn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.load_cheat_list_btn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(84)))), ((int)(((byte)(84)))), ((int)(((byte)(92)))));
            this.load_cheat_list_btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.load_cheat_list_btn.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.load_cheat_list_btn.ForeColor = System.Drawing.Color.White;
            this.load_cheat_list_btn.Location = new System.Drawing.Point(534, 2);
            this.load_cheat_list_btn.Name = "load_cheat_list_btn";
            this.load_cheat_list_btn.Size = new System.Drawing.Size(165, 25);
            this.load_cheat_list_btn.TabIndex = 33;
            this.load_cheat_list_btn.TabStop = false;
            this.load_cheat_list_btn.Text = "Load";
            this.load_cheat_list_btn.UseMnemonic = false;
            this.load_cheat_list_btn.UseVisualStyleBackColor = false;
            this.load_cheat_list_btn.Click += new System.EventHandler(this.load_address_btn_Click);
            // 
            // result_list_view_hex
            // 
            this.result_list_view_hex.Text = "Hex";
            this.result_list_view_hex.Width = 120;
            // 
            // save_cheat_list_btn
            // 
            this.save_cheat_list_btn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.save_cheat_list_btn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(37)))), ((int)(((byte)(38)))));
            this.save_cheat_list_btn.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.save_cheat_list_btn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.save_cheat_list_btn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(84)))), ((int)(((byte)(84)))), ((int)(((byte)(92)))));
            this.save_cheat_list_btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.save_cheat_list_btn.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.save_cheat_list_btn.ForeColor = System.Drawing.Color.White;
            this.save_cheat_list_btn.Location = new System.Drawing.Point(350, 2);
            this.save_cheat_list_btn.Name = "save_cheat_list_btn";
            this.save_cheat_list_btn.Size = new System.Drawing.Size(165, 25);
            this.save_cheat_list_btn.TabIndex = 32;
            this.save_cheat_list_btn.TabStop = false;
            this.save_cheat_list_btn.Text = "Save";
            this.save_cheat_list_btn.UseMnemonic = false;
            this.save_cheat_list_btn.UseVisualStyleBackColor = false;
            this.save_cheat_list_btn.Click += new System.EventHandler(this.save_address_btn_Click);
            // 
            // new_cheat_list_btn
            // 
            this.new_cheat_list_btn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(37)))), ((int)(((byte)(38)))));
            this.new_cheat_list_btn.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.new_cheat_list_btn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.new_cheat_list_btn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(84)))), ((int)(((byte)(84)))), ((int)(((byte)(92)))));
            this.new_cheat_list_btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.new_cheat_list_btn.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.new_cheat_list_btn.ForeColor = System.Drawing.Color.White;
            this.new_cheat_list_btn.Location = new System.Drawing.Point(0, 2);
            this.new_cheat_list_btn.Name = "new_cheat_list_btn";
            this.new_cheat_list_btn.Size = new System.Drawing.Size(158, 25);
            this.new_cheat_list_btn.TabIndex = 30;
            this.new_cheat_list_btn.TabStop = false;
            this.new_cheat_list_btn.Text = "New";
            this.new_cheat_list_btn.UseMnemonic = false;
            this.new_cheat_list_btn.UseVisualStyleBackColor = false;
            this.new_cheat_list_btn.Click += new System.EventHandler(this.add_address_btn_Click);
            // 
            // cheat_list_view
            // 
            this.cheat_list_view.AllowUserToAddRows = false;
            this.cheat_list_view.AllowUserToResizeRows = false;
            this.cheat_list_view.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.cheat_list_view.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.cheat_list_view_active,
            this.cheat_list_view_address,
            this.cheat_list_view_type,
            this.cheat_list_view_value,
            this.cheat_list_view_section,
            this.cheat_list_view_lock,
            this.cheat_list_view_description});
            this.cheat_list_view.Location = new System.Drawing.Point(0, 36);
            this.cheat_list_view.Name = "cheat_list_view";
            this.cheat_list_view.RowHeadersVisible = false;
            this.cheat_list_view.RowTemplate.Height = 23;
            this.cheat_list_view.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.cheat_list_view.Size = new System.Drawing.Size(703, 255);
            this.cheat_list_view.TabIndex = 50;
            this.cheat_list_view.TabStop = false;
            this.cheat_list_view.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.cheat_list_view_CellClick);
            this.cheat_list_view.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.cheat_list_view_CellEndEdit);
            this.cheat_list_view.RowsRemoved += new System.Windows.Forms.DataGridViewRowsRemovedEventHandler(this.cheat_list_view_RowsRemoved);
            // 
            // section_list_box
            // 
            this.section_list_box.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.section_list_box.ContextMenuStrip = this.section_list_menu;
            this.section_list_box.FormattingEnabled = true;
            this.section_list_box.HorizontalScrollbar = true;
            this.section_list_box.Location = new System.Drawing.Point(10, 3);
            this.section_list_box.Name = "section_list_box";
            this.section_list_box.Size = new System.Drawing.Size(284, 212);
            this.section_list_box.TabIndex = 7;
            this.section_list_box.TabStop = false;
            this.section_list_box.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.section_list_box_ItemCheck);
            // 
            // section_list_menu
            // 
            this.section_list_menu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.section_view_menu,
            this.section_dump_menu});
            this.section_list_menu.Name = "section_list_boxMenu";
            this.section_list_menu.Size = new System.Drawing.Size(129, 48);
            // 
            // section_view_menu
            // 
            this.section_view_menu.Name = "section_view_menu";
            this.section_view_menu.Size = new System.Drawing.Size(128, 22);
            this.section_view_menu.Text = "Hex Editor";
            this.section_view_menu.Click += new System.EventHandler(this.sectionView_Click);
            // 
            // section_dump_menu
            // 
            this.section_dump_menu.Name = "section_dump_menu";
            this.section_dump_menu.Size = new System.Drawing.Size(128, 22);
            this.section_dump_menu.Text = "Dump";
            this.section_dump_menu.Click += new System.EventHandler(this.sectionDump_Click);
            // 
            // value_box
            // 
            this.value_box.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.value_box.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(37)))), ((int)(((byte)(38)))));
            this.value_box.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.value_box.ForeColor = System.Drawing.Color.White;
            this.value_box.Location = new System.Drawing.Point(12, 269);
            this.value_box.MaxLength = 500;
            this.value_box.Name = "value_box";
            this.value_box.Size = new System.Drawing.Size(112, 21);
            this.value_box.TabIndex = 10;
            this.value_box.Text = "0";
            // 
            // refresh_lock
            // 
            this.refresh_lock.Enabled = true;
            this.refresh_lock.Interval = 500;
            this.refresh_lock.Tick += new System.EventHandler(this.refresh_lock_Tick);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.splitContainer4);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(1011, 619);
            this.splitContainer1.SplitterDistance = 703;
            this.splitContainer1.TabIndex = 47;
            // 
            // splitContainer4
            // 
            this.splitContainer4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer4.Location = new System.Drawing.Point(0, 0);
            this.splitContainer4.Name = "splitContainer4";
            this.splitContainer4.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer4.Panel1
            // 
            this.splitContainer4.Panel1.Controls.Add(this.result_list_view);
            // 
            // splitContainer4.Panel2
            // 
            this.splitContainer4.Panel2.Controls.Add(this.refresh_cheat_list_btn);
            this.splitContainer4.Panel2.Controls.Add(this.load_cheat_list_btn);
            this.splitContainer4.Panel2.Controls.Add(this.save_cheat_list_btn);
            this.splitContainer4.Panel2.Controls.Add(this.new_cheat_list_btn);
            this.splitContainer4.Panel2.Controls.Add(this.cheat_list_view);
            this.splitContainer4.Size = new System.Drawing.Size(703, 619);
            this.splitContainer4.SplitterDistance = 324;
            this.splitContainer4.TabIndex = 0;
            this.splitContainer4.TabStop = false;
            // 
            // result_list_view
            // 
            this.result_list_view.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.result_list_view_address,
            this.result_list_view_type,
            this.result_list_view_value,
            this.result_list_view_hex,
            this.result_list_view_section});
            this.result_list_view.ContextMenuStrip = this.result_list_menu;
            this.result_list_view.Dock = System.Windows.Forms.DockStyle.Fill;
            this.result_list_view.FullRowSelect = true;
            this.result_list_view.GridLines = true;
            this.result_list_view.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.result_list_view.Location = new System.Drawing.Point(0, 0);
            this.result_list_view.MultiSelect = false;
            this.result_list_view.Name = "result_list_view";
            this.result_list_view.Size = new System.Drawing.Size(703, 324);
            this.result_list_view.TabIndex = 16;
            this.result_list_view.TabStop = false;
            this.result_list_view.UseCompatibleStateImageBehavior = false;
            this.result_list_view.View = System.Windows.Forms.View.Details;
            this.result_list_view.DoubleClick += new System.EventHandler(this.result_list_view_DoubleClick);
            // 
            // result_list_view_address
            // 
            this.result_list_view_address.Text = "Address";
            this.result_list_view_address.Width = 120;
            // 
            // result_list_view_type
            // 
            this.result_list_view_type.Text = "Type";
            this.result_list_view_type.Width = 120;
            // 
            // result_list_menu
            // 
            this.result_list_menu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.view_item,
            this.dump_item});
            this.result_list_menu.Name = "result_list_menu";
            this.result_list_menu.Size = new System.Drawing.Size(129, 48);
            // 
            // view_item
            // 
            this.view_item.Name = "view_item";
            this.view_item.Size = new System.Drawing.Size(128, 22);
            this.view_item.Text = "Hex Editor";
            this.view_item.Click += new System.EventHandler(this.view_item_Click);
            // 
            // dump_item
            // 
            this.dump_item.Name = "dump_item";
            this.dump_item.Size = new System.Drawing.Size(128, 22);
            this.dump_item.Text = "Dump";
            this.dump_item.Click += new System.EventHandler(this.dump_item_Click);
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.version_list);
            this.splitContainer2.Panel1.Controls.Add(this.processes_comboBox);
            this.splitContainer2.Panel1.Controls.Add(this.ip_box);
            this.splitContainer2.Panel1.Controls.Add(this.port_box);
            this.splitContainer2.Panel1.Controls.Add(this.send_payload_btn);
            this.splitContainer2.Panel1.Controls.Add(this.get_processes_btn);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.alignment_box);
            this.splitContainer2.Panel2.Controls.Add(this.value_1_box);
            this.splitContainer2.Panel2.Controls.Add(this.value_label);
            this.splitContainer2.Panel2.Controls.Add(this.and_label);
            this.splitContainer2.Panel2.Controls.Add(this.section_list_box);
            this.splitContainer2.Panel2.Controls.Add(this.value_box);
            this.splitContainer2.Panel2.Controls.Add(this.msg);
            this.splitContainer2.Panel2.Controls.Add(this.select_all);
            this.splitContainer2.Panel2.Controls.Add(this.progressBar);
            this.splitContainer2.Panel2.Controls.Add(this.compareTypeList);
            this.splitContainer2.Panel2.Controls.Add(this.valueTypeList);
            this.splitContainer2.Panel2.Controls.Add(this.label4);
            this.splitContainer2.Panel2.Controls.Add(this.next_scan_btn);
            this.splitContainer2.Panel2.Controls.Add(this.new_scan_btn);
            this.splitContainer2.Panel2.Controls.Add(this.refresh_btn);
            this.splitContainer2.Size = new System.Drawing.Size(304, 619);
            this.splitContainer2.SplitterDistance = 113;
            this.splitContainer2.TabIndex = 47;
            this.splitContainer2.TabStop = false;
            // 
            // version_list
            // 
            this.version_list.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(37)))), ((int)(((byte)(38)))));
            this.version_list.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.version_list.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.version_list.ForeColor = System.Drawing.Color.White;
            this.version_list.FormattingEnabled = true;
            this.version_list.Items.AddRange(new object[] {
            "4.05",
            "4.55"});
            this.version_list.Location = new System.Drawing.Point(10, 4);
            this.version_list.Name = "version_list";
            this.version_list.Size = new System.Drawing.Size(80, 20);
            this.version_list.TabIndex = 0;
            // 
            // processes_comboBox
            // 
            this.processes_comboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.processes_comboBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(37)))), ((int)(((byte)(38)))));
            this.processes_comboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.processes_comboBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.processes_comboBox.ForeColor = System.Drawing.Color.White;
            this.processes_comboBox.FormattingEnabled = true;
            this.processes_comboBox.Location = new System.Drawing.Point(10, 57);
            this.processes_comboBox.Name = "processes_comboBox";
            this.processes_comboBox.Size = new System.Drawing.Size(275, 20);
            this.processes_comboBox.TabIndex = 6;
            this.processes_comboBox.TabStop = false;
            this.processes_comboBox.SelectedIndexChanged += new System.EventHandler(this.processes_comboBox_SelectedIndexChanged);
            // 
            // ip_box
            // 
            this.ip_box.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ip_box.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(37)))), ((int)(((byte)(38)))));
            this.ip_box.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ip_box.ForeColor = System.Drawing.Color.White;
            this.ip_box.Location = new System.Drawing.Point(94, 3);
            this.ip_box.MaxLength = 15;
            this.ip_box.Name = "ip_box";
            this.ip_box.Size = new System.Drawing.Size(142, 21);
            this.ip_box.TabIndex = 1;
            this.ip_box.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // port_box
            // 
            this.port_box.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.port_box.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(37)))), ((int)(((byte)(38)))));
            this.port_box.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.port_box.ForeColor = System.Drawing.Color.White;
            this.port_box.Location = new System.Drawing.Point(242, 3);
            this.port_box.MaxLength = 4;
            this.port_box.Name = "port_box";
            this.port_box.Size = new System.Drawing.Size(43, 21);
            this.port_box.TabIndex = 2;
            this.port_box.TabStop = false;
            this.port_box.Text = "9020";
            this.port_box.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // send_payload_btn
            // 
            this.send_payload_btn.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.send_payload_btn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(37)))), ((int)(((byte)(38)))));
            this.send_payload_btn.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.send_payload_btn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.send_payload_btn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(84)))), ((int)(((byte)(84)))), ((int)(((byte)(92)))));
            this.send_payload_btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.send_payload_btn.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.send_payload_btn.ForeColor = System.Drawing.Color.White;
            this.send_payload_btn.Location = new System.Drawing.Point(10, 30);
            this.send_payload_btn.Name = "send_payload_btn";
            this.send_payload_btn.Size = new System.Drawing.Size(275, 24);
            this.send_payload_btn.TabIndex = 3;
            this.send_payload_btn.TabStop = false;
            this.send_payload_btn.Text = "Send Payload";
            this.send_payload_btn.UseMnemonic = false;
            this.send_payload_btn.UseVisualStyleBackColor = false;
            this.send_payload_btn.Click += new System.EventHandler(this.send_payload_btn_Click);
            // 
            // get_processes_btn
            // 
            this.get_processes_btn.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.get_processes_btn.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.get_processes_btn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(37)))), ((int)(((byte)(38)))));
            this.get_processes_btn.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.get_processes_btn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.get_processes_btn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(84)))), ((int)(((byte)(84)))), ((int)(((byte)(92)))));
            this.get_processes_btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.get_processes_btn.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.get_processes_btn.ForeColor = System.Drawing.Color.White;
            this.get_processes_btn.Location = new System.Drawing.Point(10, 84);
            this.get_processes_btn.Name = "get_processes_btn";
            this.get_processes_btn.Size = new System.Drawing.Size(275, 24);
            this.get_processes_btn.TabIndex = 5;
            this.get_processes_btn.TabStop = false;
            this.get_processes_btn.Text = "Refresh Processes";
            this.get_processes_btn.UseMnemonic = false;
            this.get_processes_btn.UseVisualStyleBackColor = false;
            this.get_processes_btn.Click += new System.EventHandler(this.get_processes_btn_Click);
            // 
            // alignment_box
            // 
            this.alignment_box.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.alignment_box.AutoSize = true;
            this.alignment_box.Checked = true;
            this.alignment_box.CheckState = System.Windows.Forms.CheckState.Checked;
            this.alignment_box.Location = new System.Drawing.Point(167, 221);
            this.alignment_box.Name = "alignment_box";
            this.alignment_box.Size = new System.Drawing.Size(78, 16);
            this.alignment_box.TabIndex = 9;
            this.alignment_box.Text = "Alignment";
            this.alignment_box.UseVisualStyleBackColor = true;
            // 
            // value_1_box
            // 
            this.value_1_box.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.value_1_box.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(37)))), ((int)(((byte)(38)))));
            this.value_1_box.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.value_1_box.ForeColor = System.Drawing.Color.White;
            this.value_1_box.Location = new System.Drawing.Point(159, 269);
            this.value_1_box.MaxLength = 31;
            this.value_1_box.Name = "value_1_box";
            this.value_1_box.Size = new System.Drawing.Size(124, 21);
            this.value_1_box.TabIndex = 30;
            this.value_1_box.Text = "0";
            // 
            // value_label
            // 
            this.value_label.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.value_label.AutoSize = true;
            this.value_label.ForeColor = System.Drawing.Color.White;
            this.value_label.Location = new System.Drawing.Point(157, 247);
            this.value_label.Name = "value_label";
            this.value_label.Size = new System.Drawing.Size(41, 12);
            this.value_label.TabIndex = 31;
            this.value_label.Text = "Value:";
            // 
            // and_label
            // 
            this.and_label.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.and_label.AutoSize = true;
            this.and_label.Location = new System.Drawing.Point(130, 278);
            this.and_label.Name = "and_label";
            this.and_label.Size = new System.Drawing.Size(23, 12);
            this.and_label.TabIndex = 29;
            this.and_label.Text = "and";
            // 
            // msg
            // 
            this.msg.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.msg.AutoSize = true;
            this.msg.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.msg.ForeColor = System.Drawing.Color.Red;
            this.msg.Location = new System.Drawing.Point(16, 448);
            this.msg.Name = "msg";
            this.msg.Size = new System.Drawing.Size(0, 16);
            this.msg.TabIndex = 21;
            // 
            // select_all
            // 
            this.select_all.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.select_all.AutoSize = true;
            this.select_all.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.select_all.Location = new System.Drawing.Point(19, 222);
            this.select_all.Name = "select_all";
            this.select_all.Size = new System.Drawing.Size(84, 16);
            this.select_all.TabIndex = 8;
            this.select_all.Text = "Select All";
            this.select_all.UseVisualStyleBackColor = true;
            this.select_all.Click += new System.EventHandler(this.selectAll_CheckBox_Click);
            // 
            // progressBar
            // 
            this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar.Location = new System.Drawing.Point(12, 467);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(275, 23);
            this.progressBar.TabIndex = 22;
            // 
            // compareTypeList
            // 
            this.compareTypeList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.compareTypeList.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(37)))), ((int)(((byte)(38)))));
            this.compareTypeList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.compareTypeList.Enabled = false;
            this.compareTypeList.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.compareTypeList.ForeColor = System.Drawing.Color.White;
            this.compareTypeList.FormattingEnabled = true;
            this.compareTypeList.Location = new System.Drawing.Point(12, 328);
            this.compareTypeList.Name = "compareTypeList";
            this.compareTypeList.Size = new System.Drawing.Size(274, 20);
            this.compareTypeList.TabIndex = 12;
            this.compareTypeList.SelectedIndexChanged += new System.EventHandler(this.compareList_SelectedIndexChanged);
            // 
            // valueTypeList
            // 
            this.valueTypeList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.valueTypeList.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(37)))), ((int)(((byte)(38)))));
            this.valueTypeList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.valueTypeList.Enabled = false;
            this.valueTypeList.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.valueTypeList.ForeColor = System.Drawing.Color.White;
            this.valueTypeList.FormattingEnabled = true;
            this.valueTypeList.Location = new System.Drawing.Point(12, 302);
            this.valueTypeList.Name = "valueTypeList";
            this.valueTypeList.Size = new System.Drawing.Size(274, 20);
            this.valueTypeList.TabIndex = 11;
            this.valueTypeList.SelectedIndexChanged += new System.EventHandler(this.valueTypeList_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label4.AutoSize = true;
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(10, 247);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(41, 12);
            this.label4.TabIndex = 28;
            this.label4.Text = "Value:";
            // 
            // next_scan_btn
            // 
            this.next_scan_btn.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.next_scan_btn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(37)))), ((int)(((byte)(38)))));
            this.next_scan_btn.Enabled = false;
            this.next_scan_btn.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.next_scan_btn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.next_scan_btn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(84)))), ((int)(((byte)(84)))), ((int)(((byte)(92)))));
            this.next_scan_btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.next_scan_btn.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.next_scan_btn.ForeColor = System.Drawing.Color.White;
            this.next_scan_btn.Location = new System.Drawing.Point(12, 414);
            this.next_scan_btn.Name = "next_scan_btn";
            this.next_scan_btn.Size = new System.Drawing.Size(275, 25);
            this.next_scan_btn.TabIndex = 15;
            this.next_scan_btn.UseMnemonic = false;
            this.next_scan_btn.UseVisualStyleBackColor = false;
            this.next_scan_btn.Click += new System.EventHandler(this.next_scan_btn_Click);
            // 
            // new_scan_btn
            // 
            this.new_scan_btn.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.new_scan_btn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(37)))), ((int)(((byte)(38)))));
            this.new_scan_btn.Enabled = false;
            this.new_scan_btn.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.new_scan_btn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.new_scan_btn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(84)))), ((int)(((byte)(84)))), ((int)(((byte)(92)))));
            this.new_scan_btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.new_scan_btn.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.new_scan_btn.ForeColor = System.Drawing.Color.White;
            this.new_scan_btn.Location = new System.Drawing.Point(12, 352);
            this.new_scan_btn.Name = "new_scan_btn";
            this.new_scan_btn.Size = new System.Drawing.Size(274, 25);
            this.new_scan_btn.TabIndex = 13;
            this.new_scan_btn.UseMnemonic = false;
            this.new_scan_btn.UseVisualStyleBackColor = false;
            this.new_scan_btn.Click += new System.EventHandler(this.new_scan_btn_Click);
            // 
            // refresh_btn
            // 
            this.refresh_btn.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.refresh_btn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(37)))), ((int)(((byte)(38)))));
            this.refresh_btn.Enabled = false;
            this.refresh_btn.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.refresh_btn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.refresh_btn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(84)))), ((int)(((byte)(84)))), ((int)(((byte)(92)))));
            this.refresh_btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.refresh_btn.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.refresh_btn.ForeColor = System.Drawing.Color.White;
            this.refresh_btn.Location = new System.Drawing.Point(12, 383);
            this.refresh_btn.Name = "refresh_btn";
            this.refresh_btn.Size = new System.Drawing.Size(275, 25);
            this.refresh_btn.TabIndex = 14;
            this.refresh_btn.UseMnemonic = false;
            this.refresh_btn.UseVisualStyleBackColor = false;
            this.refresh_btn.Click += new System.EventHandler(this.refresh_Click);
            // 
            // new_scan_worker
            // 
            this.new_scan_worker.WorkerReportsProgress = true;
            this.new_scan_worker.WorkerSupportsCancellation = true;
            this.new_scan_worker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.new_scan_worker_DoWork);
            this.new_scan_worker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.new_scan_worker_ProgressChanged);
            this.new_scan_worker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.new_scan_worker_RunWorkerCompleted);
            // 
            // next_scan_worker
            // 
            this.next_scan_worker.WorkerReportsProgress = true;
            this.next_scan_worker.WorkerSupportsCancellation = true;
            this.next_scan_worker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.next_scan_worker_DoWork);
            this.next_scan_worker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.update_result_list_worker_ProgressChanged);
            this.next_scan_worker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.next_scan_worker_RunWorkerCompleted);
            // 
            // update_result_list_worker
            // 
            this.update_result_list_worker.WorkerReportsProgress = true;
            this.update_result_list_worker.WorkerSupportsCancellation = true;
            this.update_result_list_worker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.update_result_list_worker_DoWork);
            this.update_result_list_worker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.update_result_list_worker_ProgressChanged);
            this.update_result_list_worker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.update_result_list_worker_RunWorkerCompleted);
            // 
            // cheat_list_view_active
            // 
            this.cheat_list_view_active.Frozen = true;
            this.cheat_list_view_active.HeaderText = "X";
            this.cheat_list_view_active.Name = "cheat_list_view_active";
            this.cheat_list_view_active.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.cheat_list_view_active.Text = "X";
            this.cheat_list_view_active.Width = 25;
            // 
            // cheat_list_view_address
            // 
            this.cheat_list_view_address.HeaderText = "Address";
            this.cheat_list_view_address.Name = "cheat_list_view_address";
            this.cheat_list_view_address.ReadOnly = true;
            this.cheat_list_view_address.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // cheat_list_view_type
            // 
            this.cheat_list_view_type.HeaderText = "Type";
            this.cheat_list_view_type.Name = "cheat_list_view_type";
            this.cheat_list_view_type.ReadOnly = true;
            this.cheat_list_view_type.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // cheat_list_view_value
            // 
            this.cheat_list_view_value.HeaderText = "Value";
            this.cheat_list_view_value.Name = "cheat_list_view_value";
            this.cheat_list_view_value.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // cheat_list_view_section
            // 
            this.cheat_list_view_section.HeaderText = "Section";
            this.cheat_list_view_section.Name = "cheat_list_view_section";
            this.cheat_list_view_section.ReadOnly = true;
            this.cheat_list_view_section.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // cheat_list_view_lock
            // 
            this.cheat_list_view_lock.HeaderText = "Lock";
            this.cheat_list_view_lock.Name = "cheat_list_view_lock";
            this.cheat_list_view_lock.Width = 35;
            // 
            // cheat_list_view_description
            // 
            this.cheat_list_view_description.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.cheat_list_view_description.HeaderText = "Description";
            this.cheat_list_view_description.Name = "cheat_list_view_description";
            this.cheat_list_view_description.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(1011, 619);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.parent);
            this.DoubleBuffered = true;
            this.ForeColor = System.Drawing.Color.Silver;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Name = "main";
            this.Opacity = 0.99D;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "PS4 Cheater";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.main_FormClosing);
            this.Load += new System.EventHandler(this.main_Load);
            ((System.ComponentModel.ISupportInitialize)(this.cheat_list_view)).EndInit();
            this.section_list_menu.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer4.Panel1.ResumeLayout(false);
            this.splitContainer4.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer4)).EndInit();
            this.splitContainer4.ResumeLayout(false);
            this.result_list_menu.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel1.PerformLayout();
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        private ComboBox version_list;
        private ContextMenuStrip result_list_menu;
        private ToolStripMenuItem view_item;
        private ToolStripMenuItem dump_item;
        private TextBox value_1_box;
        private Label value_label;
        private Label and_label;
        private CheckBox alignment_box;
        private BackgroundWorker update_result_list_worker;
        private DataGridViewButtonColumn cheat_list_view_active;
        private DataGridViewTextBoxColumn cheat_list_view_address;
        private DataGridViewTextBoxColumn cheat_list_view_type;
        private DataGridViewTextBoxColumn cheat_list_view_value;
        private DataGridViewTextBoxColumn cheat_list_view_section;
        private DataGridViewCheckBoxColumn cheat_list_view_lock;
        private DataGridViewTextBoxColumn cheat_list_view_description;
    }
}
