using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Be.Windows.Forms;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using librpc;

namespace PS4_Cheater
{
    public partial class HexEdit : Form
    {
        public byte[] buffer { get; set; }
        public bool changed { get; set; }

        private MappedSection section;
        private MemoryHelper memoryHelper;

        private int page;
        private int page_count;
        private int offset;

        const int page_size = 8 * 1024 * 1024;

        public HexEdit(MemoryHelper memoryHelper, int offset, MappedSection section)
        {
            InitializeComponent();

            this.memoryHelper = memoryHelper;
            this.section = section;
            this.changed = false;
            this.offset = offset;
            this.page = offset / page_size;

            this.page_count = divup((int)section.Length, page_size);

            for (int i = 0; i < page_count; ++i)
            {
                page_list.Items.Add((i + 1).ToString());
            }
        }

        private void update_ui(int page)
        {
            this.hexBox.LineInfoOffset = (uint)((ulong)section.Start + (ulong)(page_size * page));

            int mem_size = page_size;

            if (section.Length - page_size * page < mem_size)
            {
                mem_size = section.Length - page_size * page;
            }

            msg.Text = "page:" + (page + 1) + "/" + page_count;
            byte[] dst = new byte[mem_size];
            Buffer.BlockCopy(buffer, page * page_size, dst, 0, mem_size);
            hexBox.ByteProvider = new MemoryViewByteProvider(dst);
        }

        private void HexEdit_Load(object sender, EventArgs e)
        {
            this.hexBox.LineInfoOffset = (uint)section.Start;
            this.buffer = memoryHelper.ReadMemory(section.Start, (int)section.Length);
            update_ui(page);
        }

        private void HexEdit_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        int divup(int sum, int div)
        {
            return sum / div + ((sum % div != 0) ? 1 : 0);
        }

        private void next_btn_Click(object sender, EventArgs e)
        {
            if (page + 1 >= page_count)
            {
                return;
            }

            page++;

            update_ui(page);
        }

        private void previous_btn_Click(object sender, EventArgs e)
        {
            if (page <= 0)
            {
                return;
            }

            page--;

            update_ui(page);
        }

        private void page_list_SelectedIndexChanged(object sender, EventArgs e)
        {
            page = page_list.SelectedIndex;

            update_ui(page);
        }

        private void commit_btn_Click(object sender, EventArgs e)
        {
            
            MemoryViewByteProvider mvbp = (MemoryViewByteProvider)this.hexBox.ByteProvider;
            if (mvbp.HasChanges())
            {
                byte[] buffer = mvbp.Bytes.ToArray();
                List<int> change_list = mvbp.change_list;

                for (int i = 0; i < change_list.Count; ++i)
                {
                    byte[] b = { buffer[change_list[i]]  };
                    memoryHelper.WriteMemory(section.Start + (ulong)(page * page_size + change_list[i]), b);
                }
                mvbp.change_list.Clear();
            }
        }
    }
}
