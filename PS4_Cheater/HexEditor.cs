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
    public partial class HexEditor : Form
    {
        private MappedSection section;
        private MemoryHelper memoryHelper;

        private int page;
        private int page_count;
        private long line;
        private int column;

        const int page_size = 8 * 1024 * 1024;

        public HexEditor(MemoryHelper memoryHelper, int offset, MappedSection section)
        {
            InitializeComponent();

            this.memoryHelper = memoryHelper;
            this.section = section;
            this.page = offset / page_size;
            this.line = (offset - page * page_size) / hexBox.BytesPerLine;
            this.column = (offset - page * page_size) % hexBox.BytesPerLine;

            this.page_count = divup((int)section.Length, page_size);

            for (int i = 0; i < page_count; ++i)
            {
                ulong start = section.Start + (ulong)i * page_size;
                ulong end = section.Start + (ulong)(i + 1) * page_size;
                page_list.Items.Add((i + 1).ToString() + String.Format(" {0:X}-{1:X}", start, end));
            }
        }

        private void update_ui(int page, long line)
        {
            hexBox.LineInfoOffset = (uint)((ulong)section.Start + (ulong)(page_size * page));

            int mem_size = page_size;

            if (section.Length - page_size * page < mem_size)
            {
                mem_size = section.Length - page_size * page;
            }

            byte[] dst = MemoryHelper.ReadMemory(section.Start + (ulong)page * page_size, (int)mem_size);
            hexBox.ByteProvider = new MemoryViewByteProvider(dst);

            if (line != 0)
            {
                hexBox.SelectionStart = line * hexBox.BytesPerLine + column;
                hexBox.SelectionLength = 4;
                hexBox.ScrollByteIntoView((line + hexBox.Height / (int)hexBox.CharSize.Height - 1) * hexBox.BytesPerLine + column);
            }
        }

        private void HexEdit_Load(object sender, EventArgs e)
        {
            page_list.SelectedIndex = page;
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
            line = 0;
            column = 0;

            page_list.SelectedIndex = page;
        }

        private void previous_btn_Click(object sender, EventArgs e)
        {
            if (page <= 0)
            {
                return;
            }

            page--;
            line = 0;
            column = 0;
            page_list.SelectedIndex = page;
        }

        private void page_list_SelectedIndexChanged(object sender, EventArgs e)
        {
            page = page_list.SelectedIndex;

            update_ui(page, line);
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
                    MemoryHelper.WriteMemory(section.Start + (ulong)(page * page_size + change_list[i]), b);
                }
                mvbp.change_list.Clear();
            }
        }

        private void refresh_btn_Click(object sender, EventArgs e)
        {
            page_list.SelectedIndex = page;
            line = hexBox.CurrentLine - 1;
            column = 0;
            update_ui(page, line);
        }

        private void find_Click(object sender, EventArgs e)
        {
            FindOptions findOptions = new FindOptions();
            findOptions.Type = FindType.Hex;
            findOptions.Hex = MemoryHelper.string_to_hex_bytes(input_box.Text);
            hexBox.Find(findOptions);
        }
    }
}
