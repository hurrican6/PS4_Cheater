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
        public NewAddress()
        {
            InitializeComponent();
        }

        public bool succed = false;
        public ulong address { get; set; }
        public string value { get; set; }
        public string type { get; set; }
        public string descriptioin { get; set; }

        public string lock_ {get;set;}

        private void save_btn_Click(object sender, EventArgs e)
        {
            try
            {
                ulong address = ulong.Parse(address_box.Text, System.Globalization.NumberStyles.HexNumber);
                string value = value_box.Text;
                string type = type_box.Text;
                string description = description_box.Text;

                this.address = address;
                this.value = value;
                this.type = type;
                this.descriptioin = description;

                this.lock_ = lock_box.Checked ? "1" : "0";

                succed = true;
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

        private void cancell_btn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void type_box_SelectedIndexChanged(object sender, EventArgs e)
        {
            lock_box.Enabled = true;
            if ((string)type_box.SelectedItem == "hex")
            {
                lock_box.Checked = false;
                lock_box.Enabled = false;
            }
        }
    }
}
