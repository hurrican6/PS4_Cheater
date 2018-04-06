using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PS4_Cheater
{
    public partial class NewAddress : Form
    {
        private Button add_offset_btn = new Button();
        private Button del_offset_btn = new Button();

        private List<TextBox> offset_box_list = new List<TextBox>();
        private List<Label> offset_label_list = new List<Label>();

        private MemoryHelper MemoryHelper = null;

        private ProcessManager ProcessManager = null;

        public NewAddress(ProcessManager ProcessManager)
        {
            InitializeComponent();

            this.MemoryHelper = new MemoryHelper(true, 0);
            this.ProcessManager = ProcessManager;
        }

        public ulong Address { get; set; }
        public string Value { get; set; }
        public string ValueTypeStr { get; set; }
        public string Descriptioin { get; set; }
        public bool Pointer { get; set; }
        public bool Lock { get; set; }

        public List<long> OffsetList = new List<long>();

        private void save_btn_Click(object sender, EventArgs e)
        {
            try
            {
                string value = value_box.Text;
                string type = type_box.Text;
                string description = description_box.Text;

                if (!Pointer)
                {
                    this.Address = ulong.Parse(address_box.Text, System.Globalization.NumberStyles.HexNumber);
                }
                else
                {
                    this.Address = ulong.Parse(offset_box_list[0].Text, System.Globalization.NumberStyles.HexNumber);
                    for (int i = 1; i < offset_box_list.Count; ++i)
                    {
                        OffsetList.Add(long.Parse(offset_box_list[i].Text, System.Globalization.NumberStyles.HexNumber));
                    }
                }

                this.Value = value;
                this.ValueTypeStr = type;
                this.Descriptioin = description;
                this.Lock = lock_box.Checked;

                this.Close();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void NewAddress_Load(object sender, EventArgs e)
        {
            type_box.Items.AddRange(CONSTANT.SEARCH_VALUE_TYPE);
            type_box.SelectedIndex = 2;
        }

        private void pointer_box_CheckedChanged(object sender, EventArgs e)
        {
            if (pointer_box.Checked)
            {
                Pointer = true;
                Point save_position = save_btn.Location;
                save_position.Y = 140;

                Point cancel_position = cancel_btn.Location;
                cancel_position.Y = 140;

                add_offset_btn.Text = "Add Offset";
                add_offset_btn.Size = save_btn.Size;
                add_offset_btn.Location = save_btn.Location;
                add_offset_btn.Click += AddOffset_Click;
                this.Controls.Add(add_offset_btn);

                del_offset_btn.Text = "Del Offset";
                del_offset_btn.Size = cancel_btn.Size;
                del_offset_btn.Location = cancel_btn.Location;
                del_offset_btn.Click += DelOffset_Click;
                this.Controls.Add(del_offset_btn);

                cancel_btn.Location = cancel_position;
                save_btn.Location = save_position;

                address_box.Enabled = false;
                this.Height = 170;
            }
            else
            {
                Pointer = false;

                Point save_position = save_btn.Location;
                save_position.Y = 110;

                Point cancel_position = cancel_btn.Location;
                cancel_position.Y = 110;

                cancel_btn.Location = cancel_position;
                save_btn.Location = save_position;

                this.Controls.Remove(del_offset_btn);
                this.Controls.Remove(add_offset_btn);

                for (int i = 0; i < offset_box_list.Count; ++i)
                {
                    this.Controls.Remove(offset_box_list[i]);
                    this.Controls.Remove(offset_label_list[i]);
                }

                offset_label_list.Clear();
                offset_box_list.Clear();

                this.Height = 140;

                address_box.Enabled = true;
            }
        }

        private void DelOffset_Click(object sender, EventArgs e)
        {
            if (offset_label_list.Count == 0) return;

            TextBox textBox = offset_box_list[offset_label_list.Count - 1];
            this.Controls.Remove(textBox);
            offset_box_list.RemoveAt(offset_label_list.Count - 1);

            Label label = offset_label_list[offset_label_list.Count - 1];
            this.Controls.Remove(label);
            offset_label_list.RemoveAt(offset_label_list.Count - 1);

            Point del_offset_position = del_offset_btn.Location;
            del_offset_position.Y -= 30;
            del_offset_btn.Location = del_offset_position;

            Point add_offset_position = add_offset_btn.Location;
            add_offset_position.Y -= 30;
            add_offset_btn.Location = add_offset_position;

            Point save_position = save_btn.Location;
            save_position.Y -= 30;
            save_btn.Location = save_position;

            Point cancel_position = cancel_btn.Location;
            cancel_position.Y -= 30;
            cancel_btn.Location = cancel_position;

            this.Height -= 30;
        }

        private void AddOffset_Click(object sender, EventArgs e)
        {
            TextBox textBox = new TextBox();
            textBox.Text = "0";
            textBox.Size = add_offset_btn.Size;
            textBox.Location = add_offset_btn.Location;
            this.Controls.Add(textBox);
            offset_box_list.Add(textBox);

            Label label = new Label();
            label.Text = "";
            label.Size = del_offset_btn.Size;
            label.Location = del_offset_btn.Location;
            this.Controls.Add(label);
            offset_label_list.Add(label);

            Point del_offset_position = del_offset_btn.Location;
            del_offset_position.Y += 30;
            del_offset_btn.Location = del_offset_position;

            Point add_offset_position = add_offset_btn.Location;
            add_offset_position.Y += 30;
            add_offset_btn.Location = add_offset_position;

            Point save_position = save_btn.Location;
            save_position.Y += 30;
            save_btn.Location = save_position;

            Point cancel_position = cancel_btn.Location;
            cancel_position.Y += 30;
            cancel_btn.Location = cancel_position;

            this.Height += 30;
        }

        private void PointerCheckerPointer_Tick(object sender, EventArgs e)
        {
            if (!Pointer)
                return;

            try
            {
                ValueType valueType = MemoryHelper.GetValueTypeByString(type_box.Text);

                long base_address = 0;
                for (int i = 0; i < offset_box_list.Count; ++i)
                {
                    long address = long.Parse(offset_box_list[i].Text, System.Globalization.NumberStyles.HexNumber);

                    if (i != offset_box_list.Count - 1)
                    {
                        byte[] next_address = MemoryHelper.ReadMemory((ulong)(address + base_address), 8);
                        base_address = BitConverter.ToInt64(next_address, 0);
                        offset_label_list[i].Text = base_address.ToString("X");
                    }
                    else
                    {
                        MemoryHelper.InitMemoryHandler(valueType, CompareType.NONE, true);
                        byte[] data = MemoryHelper.ReadMemory((ulong)(address + base_address), MemoryHelper.Length);
                        offset_label_list[i].Text = MemoryHelper.BytesToString(data);
                    }
                }
            }
            catch
            {

            }
        }

        private void cancel_btn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private Point offset;

        private void NewAddress_MouseDown(object sender, MouseEventArgs e)
        {
            if (MouseButtons.Left != e.Button) return;

            Point cur = this.PointToScreen(e.Location);
            offset = new Point(cur.X - this.Left, cur.Y - this.Top);
        }

        private void NewAddress_MouseMove(object sender, MouseEventArgs e)
        {
            if (MouseButtons.Left != e.Button) return;

            Point cur = MousePosition;
            this.Location = new Point(cur.X - offset.X, cur.Y - offset.Y);
        }
    }
}

