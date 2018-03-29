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
    public partial class PointerFinder : Form
    {
        private PointerList pointerList = new PointerList();
        private ProcessManager processManager = null;
        private MemoryHelper memoryHelper = null;
        private ulong address = 0;
        private DataGridView cheatList;

        public PointerFinder(ulong address, ProcessManager processManager, DataGridView cheat_list_view)
        {
            this.address = address;
            this.cheatList = cheat_list_view;
            this.processManager = processManager;
            this.memoryHelper = new MemoryHelper();

            InitializeComponent();
        }

        private void PointerFinder_Load(object sender, EventArgs e)
        {
            address_box.Text = address.ToString("X");
            for (int i = 1; i <= 30; ++i)
            {
                level_updown.Items.Add(i);
            }
            level_updown.SelectedIndex = 10;
            pointerList.NewPathGeneratedEvent += PointerList_NewPathGeneratedEvent;
            set_controls(false);
        }

        private void PointerList_NewPathGeneratedEvent(PointerList pointerList, List<ulong> path_offset, List<Pointer> path_address)
        {
            PointerFinderWorkerListViewUpdate view_info = new PointerFinderWorkerListViewUpdate(pointerList, path_offset, path_address);
            pointer_finder_worker.ReportProgress(0, view_info);
        }

        void set_controls(bool enable)
        {
            level_updown.Enabled = enable;
            address_box.Enabled = enable;
            find_btn.Enabled = enable;
        }

        private void init_btn_Click(object sender, EventArgs e)
        {
            peek_worker.RunWorkerAsync();
        }

        class PointerFinderWorkerArgs
        {
            public ulong Address { get; set; }
            public List<int> Range { get; set; }

            public PointerFinderWorkerArgs(ulong address, List<int> range)
            {
                this.Address = address;
                this.Range = range;
            }
        }

        class PointerFinderWorkerListViewUpdate
        {
            public PointerList PointerList { get; set; }
            public List<ulong> PathOffset { get; set; }
            public List<Pointer> PathAddress { get; set; }

            public PointerFinderWorkerListViewUpdate(PointerList pointerList, List<ulong> path_offset, List<Pointer> path_address)
            {
                this.PointerList = pointerList;
                this.PathOffset = path_offset;
                this.PathAddress = path_address;
            }
        }

        static int counter = 0;

        private void find_btn_Click(object sender, EventArgs e)
        {
            if (find_btn.Text == "Find")
            {
                ulong address = ulong.Parse(address_box.Text, System.Globalization.NumberStyles.HexNumber);
                int level = level_updown.SelectedIndex + 1;
                counter = 0;
                pointerList.Stop = false;
                List<int> range = new List<int>();
                for (int i = 0; i < level; ++i)
                {
                    range.Add(4 * 1024);
                }

                pointer_list_view.Clear();
                for (int i = 0; i < level; ++i)
                {
                    pointer_list_view.Columns.Add("Offset_" + i);
                    pointer_list_view.Columns.Add("Address_" + i);
                    pointer_list_view.Columns.Add("Section_" + i);
                }
                find_btn.Text = "Stop";
                pointer_finder_worker.RunWorkerAsync(new PointerFinderWorkerArgs(address, range));
            }
            else
            {
                pointerList.Stop = true;
                find_btn.Text = "Find";
            }
        }

        private void peek_worker_DoWork(object sender, DoWorkEventArgs e)
        {
            peek_worker.ReportProgress(0);
            for (int section_idx = 0; section_idx < processManager.MappedSectionList.Count; ++section_idx)
            {
                if (peek_worker.CancellationPending) break;
                MappedSection mappedSection = processManager.MappedSectionList[section_idx];
                mappedSection.PointerSearchInit(processManager, memoryHelper, pointerList);
                peek_worker.ReportProgress((int)(((float)section_idx / processManager.MappedSectionList.Count) * 80));
            }
            peek_worker.ReportProgress(80);
            pointerList.Init();
            //pointerList.Save();
            peek_worker.ReportProgress(100);
            
            //pointerList.Load();
        }

        private void peek_worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progress_bar.Value = e.ProgressPercentage;
        }

        private void peek_worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            set_controls(true);
        }

        private void pointer_finder_worker_DoWork(object sender, DoWorkEventArgs e)
        {
            PointerFinderWorkerArgs pointerFinderWorkerArgs = (PointerFinderWorkerArgs)e.Argument;
            pointerList.FindPointerList(pointerFinderWorkerArgs.Address, pointerFinderWorkerArgs.Range);
        }

        private void pointer_finder_worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            PointerFinderWorkerListViewUpdate pointerFinderWorkerListViewUpdate = (PointerFinderWorkerListViewUpdate)e.UserState;

            List<ulong> path_offset = pointerFinderWorkerListViewUpdate.PathOffset;
            List<Pointer> path_address = pointerFinderWorkerListViewUpdate.PathAddress;

            ListViewItem lvi = new ListViewItem();
            int level = level_updown.SelectedIndex + 1;

            for (int i = 0; i < path_offset.Count; ++i)
            {
                lvi.SubItems.Add(path_offset[i].ToString("X"));                           //offset
                lvi.SubItems.Add(path_address[i].Address.ToString("X"));                  //address
                int sectionID = processManager.GetMappedSectionID(path_address[i].Address);
                lvi.SubItems.Add(processManager.GetSectionName(sectionID));               //section
            }

            pointer_list_view.Items.Add(lvi);

            ++counter;
            msg.Text = counter.ToString();
            //progress_bar.Value = e.ProgressPercentage;
        }

        private void pointer_finder_worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            find_btn.Text = "Find";
        }
    }
}
