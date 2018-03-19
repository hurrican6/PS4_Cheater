namespace PS4_Cheater
{
    using librpc;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Globalization;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;
    using System.Linq;

    public partial class main : Form
    {
        private ProcessManager processManager = new ProcessManager();
        private MemoryHelper memoryHelper = new MemoryHelper();
        private CheatList cheatList = new CheatList();

        private const int CHEAT_LIST_ENABLED = 0;
        private const int CHEAT_LIST_ADDRESS = 1;
        private const int CHEAT_LIST_TYPE = 2;
        private const int CHEAT_LIST_VALUE = 3;
        private const int CHEAT_LIST_SECTION = 4;
        private const int CHEAT_LIST_LOCK = 5;
        private const int CHEAT_LIST_DESC = 6;

        private const int RESULT_LIST_ADDRESS = 0;
        private const int RESULT_LIST_TYPE = 1;
        private const int RESULT_LIST_VALUE = 2;
        private const int RESULT_LIST_SECTION = 4;

        private const int VERSION_LIST_405 = 0;
        private const int VERSION_LIST_455 = 1;

        private const int VERSION_LIST_DEFAULT = VERSION_LIST_405;

        private string[] SEARCH_BY_FLOAT_FIRST = new string[]
        {
             CONSTANT.EXACT_VALUE,
             CONSTANT.FUZZY_VALUE,
             CONSTANT.BIGGER_THAN,
             CONSTANT.SMALLER_THAN,
             CONSTANT.BETWEEN_VALUE,
             CONSTANT.UNKNOWN_INITIAL_VALUE
        };

        private string[] SEARCH_BY_BYTES_FIRST = new string[]
        {
            CONSTANT.EXACT_VALUE,
            CONSTANT.BIGGER_THAN,
            CONSTANT.SMALLER_THAN,
            CONSTANT.BETWEEN_VALUE,
            CONSTANT.UNKNOWN_INITIAL_VALUE
        };

        private string[] SEARCH_BY_FLOAT_NEXT = new string[]
        {
             CONSTANT.EXACT_VALUE,
             CONSTANT.FUZZY_VALUE,
             CONSTANT.INCREASED_VALUE,
             CONSTANT.INCREASED_VALUE_BY,
             CONSTANT.DECREASED_VALUE,
             CONSTANT.DECREASED_VALUE_BY,
             CONSTANT.BIGGER_THAN,
             CONSTANT.SMALLER_THAN,
             CONSTANT.CHANGED_VALUE,
             CONSTANT.UNCHANGED_VALUE,
             CONSTANT.BETWEEN_VALUE,
        };

        private string[] SEARCH_BY_BYTES_NEXT = new string[]
        {
            CONSTANT.EXACT_VALUE,
            CONSTANT.INCREASED_VALUE,
            CONSTANT.INCREASED_VALUE_BY,
            CONSTANT.DECREASED_VALUE,
            CONSTANT.DECREASED_VALUE_BY,
            CONSTANT.BIGGER_THAN,
            CONSTANT.SMALLER_THAN,
            CONSTANT.CHANGED_VALUE,
            CONSTANT.UNCHANGED_VALUE,
            CONSTANT.BETWEEN_VALUE,
        };

        private string[] SEARCH_BY_HEX = new string[]
        {
            CONSTANT.EXACT_VALUE,
        };

        public main()
        {
            this.InitializeComponent();
        }

        private void main_Load(object sender, EventArgs e)
        {
            valueTypeList.Items.AddRange(CONSTANT.SEARCH_VALUE_TYPE);
            valueTypeList.SelectedIndex = 2;

            string version = Config.getSetting("ps4 version");
            string ip = Config.getSetting("ip");

            if (version == "4.05")
            {
                version_list.SelectedIndex = VERSION_LIST_405;
            }
            else if (version == "4.55")
            {
                version_list.SelectedIndex = VERSION_LIST_455;
            }
            else
            {
                version_list.SelectedIndex = VERSION_LIST_DEFAULT;
            }

            if (!string.IsNullOrEmpty(ip))
            {
                ip_box.Text = ip;
            }

            this.next_scan_btn.Text = CONSTANT.NEXT_SCAN;
            this.new_scan_btn.Text = CONSTANT.FIRST_SCAN;
            this.refresh_btn.Text = CONSTANT.REFRESH;
            this.Text += " " + CONSTANT.MAJOR_VERSION + "." + CONSTANT.SECONDARY_VERSION + "." + CONSTANT.THIRD_VERSION;
        }

        private void main_FormClosing(object sender, FormClosingEventArgs e)
        {

            string ip = Config.getSetting("ip");
            string version = "";
            switch (version_list.SelectedIndex)
            {
                case VERSION_LIST_405:
                    version = "4.05";
                    break;
                case VERSION_LIST_455:
                    version = "4.55";
                    break;
                default:
                    break;
            }

            if (!string.IsNullOrWhiteSpace(version))
            {
                Config.updateSeeting("ps4 version", version);
            }

            if (!string.IsNullOrWhiteSpace(ip_box.Text))
            {
                Config.updateSeeting("ip", ip_box.Text);
            }
        }

        class WorkerReturn
        {
            public List<ListViewItem> ListViewItems { get; set; }
            public bool[] MappedSectionCheckeSet { get; set; }
            public ulong Results { get; set; }
        }

        private void update_result_list_view(BackgroundWorker worker, bool refresh, int start, float percent)
        {
            worker.ReportProgress(start);

            List<ListViewItem> listViewItems = new List<ListViewItem>();
            bool[] mappedSectionCheckeSet = new bool[processManager.MappedSectionList.Count];

            ulong totalResultCount = processManager.TotalResultCount();
            ulong curResultCount = 0;
            string value_type = MemoryHelper.GetStringOfValueType(memoryHelper.ValueType);

            const int MAX_RESULTS_NUM = 0x10000;

            for (int idx = 0; idx < processManager.MappedSectionList.Count; ++idx)
            {
                MappedSection mapped_section = processManager.MappedSectionList[idx];
                ResultList result_list = mapped_section.ResultList;
                if (result_list == null)
                {
                    continue;
                }
                if (!mapped_section.Check)
                {
                    continue;
                }

                mappedSectionCheckeSet[idx] = result_list.Count > 0;

                for (result_list.Begin(); !result_list.End(); result_list.Next())
                {
                    if (curResultCount >= MAX_RESULTS_NUM)
                    {
                        break;
                    }

                    uint memory_address_offset = 0;
                    byte[] memory_value = null;

                    result_list.Get(ref memory_address_offset, ref memory_value);

                    curResultCount++;
                    ListViewItem lvi = new ListViewItem();

                    lvi.Text = String.Format("{0:X}", memory_address_offset + mapped_section.Start);

                    if (refresh && !worker.CancellationPending)
                    {
                        memory_value = memoryHelper.GetBytesByType(memory_address_offset + mapped_section.Start);
                        result_list.Set(memory_value);
                        worker.ReportProgress(start + (int)(100.0f * curResultCount / MAX_RESULTS_NUM));
                    }

                    lvi.SubItems.Add(value_type);
                    lvi.SubItems.Add(memoryHelper.BytesToString(memory_value));
                    lvi.SubItems.Add(memoryHelper.BytesToHexString(memory_value));
                    lvi.SubItems.Add(processManager.GetSectionName(idx));

                    listViewItems.Add(lvi);
                }
            }

            WorkerReturn workerReturn = new WorkerReturn();
            workerReturn.ListViewItems = listViewItems;
            workerReturn.MappedSectionCheckeSet = mappedSectionCheckeSet;
            workerReturn.Results = totalResultCount;

            worker.ReportProgress(start + (int)(100 * percent), workerReturn);
        }


        void setButtons(bool enabled)
        {
            new_scan_btn.Enabled = enabled;
            refresh_btn.Enabled = enabled;
            next_scan_btn.Enabled = enabled;
            processes_comboBox.Enabled = enabled;
            get_processes_btn.Enabled = enabled;
            section_list_menu.Enabled = enabled;
            compareTypeList.Enabled = enabled;
        }

        private void new_scan_btn_Click(object sender, EventArgs e)
        {
            try
            {
                if (new_scan_btn.Text == CONSTANT.FIRST_SCAN)
                {
                    if (MessageBox.Show("search size:" + (processManager.TotalMemorySize / 1024).ToString() + "KB") != DialogResult.OK)
                    {
                        return;
                    }

                    memoryHelper.InitMemoryHandler((string)valueTypeList.SelectedItem,
                        (string)compareTypeList.SelectedItem, alignment_box.Checked, value_box.Text.Length);

                    setButtons(false);
                    new_scan_btn.Enabled = true;
                    valueTypeList.Enabled = false;

                    new_scan_worker.RunWorkerAsync();

                    new_scan_btn.Text = CONSTANT.STOP;
                    InitCompareTypeListOfNextScan();
                }
                else if (new_scan_btn.Text == CONSTANT.NEW_SCAN)
                {
                    valueTypeList.Enabled = true;
                    refresh_btn.Enabled = false;
                    next_scan_btn.Enabled = false;
                    new_scan_btn.Text = CONSTANT.FIRST_SCAN;

                    result_list_view.Items.Clear();
                    processManager.ClearResultList();
                    InitCompareTypeListOfFirstScan();
                }
                else if (new_scan_btn.Text == CONSTANT.STOP)
                {
                    new_scan_worker.CancelAsync();
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void refresh_Click(object sender, EventArgs e)
        {
            try
            {
                if (refresh_btn.Text == "Refresh")
                {
                    setButtons(false);
                    refresh_btn.Enabled = true;
                    refresh_btn.Text = CONSTANT.STOP;
                    update_result_list_worker.RunWorkerAsync();
                }
                else if (refresh_btn.Text == CONSTANT.STOP)
                {
                    update_result_list_worker.CancelAsync();
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void next_scan_btn_Click(object sender, EventArgs e)
        {
            try
            {
                if (next_scan_btn.Text == "Next Scan")
                {
                    memoryHelper.InitNextScanMemoryHandler((string)compareTypeList.SelectedItem);
                    setButtons(false);
                    next_scan_btn.Enabled = true;
                    next_scan_btn.Text = CONSTANT.STOP;
                    next_scan_worker.RunWorkerAsync();
                }
                else if (next_scan_btn.Text == CONSTANT.STOP)
                {
                    next_scan_worker.CancelAsync();
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void set_section_list_box(bool check)
        {
            for (int i = 0; i < section_list_box.Items.Count; ++i)
            {
                section_list_box.SetItemChecked(i, check);
            }
        }

        private void selectAll_CheckBox_Click(object sender, EventArgs e)
        {
            bool check = select_all.Checked;
            set_section_list_box(check);
        }

        private void update_result_list_view_ui(WorkerReturn ret)
        {
            result_list_view.Items.Clear();
            result_list_view.BeginUpdate();
            result_list_view.Items.AddRange(ret.ListViewItems.ToArray());
            result_list_view.EndUpdate();

            for (int i = 0; i < section_list_box.Items.Count; ++i)
            {
                section_list_box.SetItemChecked(i, ret.MappedSectionCheckeSet[i]);
            }
            msg.Text = ret.Results + " results";
        }

        private void next_scan_worker_DoWork(object sender, DoWorkEventArgs e)
        {
            long processed_memory_len = 0;
            ulong total_memory_size = processManager.TotalMemorySize + 1;
            string value_0 = value_box.Text;
            string value_1 = value_1_box.Text;
            next_scan_worker.ReportProgress(0);
            for (int section_idx = 0; section_idx < processManager.MappedSectionList.Count; ++section_idx)
            {
                if (next_scan_worker.CancellationPending) break;
                MappedSection mappedSection = processManager.MappedSectionList[section_idx];
                mappedSection.UpdateResultList(processManager, memoryHelper, value_0, value_1, false);
                if (mappedSection.Check) processed_memory_len += mappedSection.Length;
                next_scan_worker.ReportProgress((int)(((float)processed_memory_len / total_memory_size) * 80));
            }
            next_scan_worker.ReportProgress(80);
            
            update_result_list_view(next_scan_worker, false, 80, 0.2f);
        }

        private void update_result_list_worker_DoWork(object sender, DoWorkEventArgs e)
        {
            update_result_list_view(update_result_list_worker, true, 0, 1.0f);
        }

        private void new_scan_worker_DoWork(object sender, DoWorkEventArgs e)
        {
            long processed_memory_len = 0;
            ulong total_memory_size = processManager.TotalMemorySize + 1;
            string value_0 = value_box.Text;
            string value_1 = value_1_box.Text;
            new_scan_worker.ReportProgress(0);
            for (int section_idx = 0; section_idx < processManager.MappedSectionList.Count; ++section_idx)
            {
                if (new_scan_worker.CancellationPending) break;
                MappedSection mappedSection = processManager.MappedSectionList[section_idx];
                mappedSection.UpdateResultList(processManager, memoryHelper, value_0, value_1, true);
                if (mappedSection.Check) processed_memory_len += mappedSection.Length;
                new_scan_worker.ReportProgress((int)(((float)processed_memory_len / total_memory_size) * 80));
            }
            new_scan_worker.ReportProgress(80);
            update_result_list_view(new_scan_worker, false, 80, 0.2f);
        }

        private void new_scan_worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage == 0)
            {
                msg.Text = "Peeking memory...";
            }

            if (e.ProgressPercentage == 50)
            {
                msg.Text = "Analysing memory...";
            }

            if (e.ProgressPercentage == 100)
            {
                if (e.UserState is WorkerReturn)
                {
                    update_result_list_view_ui((WorkerReturn)e.UserState);
                }
            }

            progressBar.Value = e.ProgressPercentage;
        }

        private void update_result_list_worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage == 0)
            {
                msg.Text = "Processing memory...";
            }

            if (e.ProgressPercentage == 100 && e.UserState is WorkerReturn)
            {
                update_result_list_view_ui((WorkerReturn)e.UserState);
            }

            progressBar.Value = e.ProgressPercentage;
        }

        private void dump_dialog(int sectionID)
        {
            if (sectionID >= 0)
            {
                MappedSection section = processManager.MappedSectionList[sectionID];

                save_file_dialog.Filter = "Binary files (*.bin)|*.bin|All files (*.*)|*.*";
                save_file_dialog.FilterIndex = 1;
                save_file_dialog.RestoreDirectory = true;
                save_file_dialog.FileName = (string)section_list_box.Items[section_list_box.SelectedIndex];

                if (save_file_dialog.ShowDialog() == DialogResult.OK)
                {
                    byte[] buffer = MemoryHelper.ReadMemory(section.Start, (int)section.Length);

                    FileStream myStream = new FileStream(save_file_dialog.FileName, FileMode.OpenOrCreate);
                    myStream.Write(buffer, 0, buffer.Length);
                    myStream.Close();
                }
            }
        }

        private void view_item_Click(object sender, EventArgs e)
        {
            if (result_list_view.SelectedItems.Count == 1)
            {
                ListView.SelectedListViewItemCollection items = result_list_view.SelectedItems;

                ulong address = ulong.Parse(result_list_view.SelectedItems[0].
                    SubItems[RESULT_LIST_ADDRESS].Text, NumberStyles.HexNumber);
                int sectionID = processManager.GetMappedSectionID(address);

                if (sectionID >= 0)
                {
                    int offset = 0;

                    MappedSection section = processManager.MappedSectionList[sectionID];

                    offset = (int)(address - section.Start);
                    HexEditor hexEdit = new HexEditor(memoryHelper, offset, section);
                    hexEdit.Show(this);
                }
            }
        }


        private void dump_item_Click(object sender, EventArgs e)
        {
            if (result_list_view.SelectedItems.Count == 1)
            {
                ulong address = ulong.Parse(result_list_view.SelectedItems[0].
                    SubItems[RESULT_LIST_ADDRESS].Text, NumberStyles.HexNumber);

                int sectionID = processManager.GetMappedSectionID(address);

                dump_dialog(sectionID);
            }
        }

        private void sectionView_Click(object sender, EventArgs e)
        {
            int sectionID = -1;
            int offset = 0;

            sectionID = section_list_box.SelectedIndex;

            if (sectionID >= 0)
            {
                MappedSection section = processManager.MappedSectionList[sectionID];
                HexEditor hexEdit = new HexEditor(memoryHelper, offset, section);
                hexEdit.Show(this);
            }
        }

        private void sectionDump_Click(object sender, EventArgs e)
        {
            if (section_list_box.SelectedIndex >= 0)
            {
                dump_dialog(section_list_box.SelectedIndex);
            }
        }

        private void section_list_box_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            processManager.SectionCheck(e.Index, e.NewValue == CheckState.Checked);
        }

        void add_new_row_of_cheat_list_view(DataCheat cheat, int sectionID)
        {
            int index = this.cheat_list_view.Rows.Add();

            DataGridViewRow cheat_list_view_item = cheat_list_view.Rows[index];
            cheat_list_view_item.Cells[CHEAT_LIST_ADDRESS].Value = cheat.Address;
            cheat_list_view_item.Cells[CHEAT_LIST_TYPE].Value = MemoryHelper.GetStringOfValueType(cheat.Type);
            cheat_list_view_item.Cells[CHEAT_LIST_VALUE].Value = cheat.Value;
            cheat_list_view_item.Cells[CHEAT_LIST_SECTION].Value = processManager.GetSectionName(sectionID);
            cheat_list_view_item.Cells[CHEAT_LIST_LOCK].Value = cheat.Lock;
            cheat_list_view_item.Cells[CHEAT_LIST_DESC].Value = cheat.Description;
        }

        void new_data_cheat(string address_str, string type, string value, string section, string flag, string description)
        {
            try
            {
                ulong address = ulong.Parse(address_str, NumberStyles.HexNumber);
                int sectionID = processManager.GetMappedSectionID(address);

                if (sectionID == -1)
                {
                    MessageBox.Show("Address is out of range!");
                    return;
                }

                for (int i = 0; i < cheatList.Count; ++i)
                {
                    if (cheatList[i].Address == address_str)
                    {
                        return;
                    }
                }

                ulong flag_u = ulong.Parse(flag, NumberStyles.HexNumber);
                bool lock_ = (flag_u & CONSTANT.SAVE_FLAG_LOCK) == CONSTANT.SAVE_FLAG_LOCK ? true : false;

                ValueType valueType = MemoryHelper.GetValueTypeByString(type);
                CheatType cheatType = Cheat.GetCheatTypeByValueType(valueType);
                if (cheatType == CheatType.DATA_TYPE)
                {
                    DataCheat dataCheat = new DataCheat(processManager, address_str, sectionID, value, lock_, valueType, description);
                    add_new_row_of_cheat_list_view(dataCheat, sectionID);
                    cheatList.Add(dataCheat);
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, exception.Source, MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
        }

        private void result_list_view_DoubleClick(object sender, EventArgs e)
        {
            if (result_list_view.SelectedItems.Count == 1)
            {
                ListView.SelectedListViewItemCollection items = result_list_view.SelectedItems;

                ListViewItem lvItem = items[0];
                string address = lvItem.SubItems[RESULT_LIST_ADDRESS].Text;
                string type = lvItem.SubItems[RESULT_LIST_TYPE].Text;
                string value = lvItem.SubItems[RESULT_LIST_VALUE].Text;
                string section = lvItem.SubItems[RESULT_LIST_SECTION].Text;
                string flag = "0";
                string description = "";

                new_data_cheat(address, type, value, section, flag, description);
            }

        }

        private void cheat_list_view_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                DataGridViewRow edited_row = cheat_list_view.Rows[e.RowIndex];
                object edited_col = edited_row.Cells[e.ColumnIndex].Value;

                switch (e.ColumnIndex)
                {
                    case CHEAT_LIST_VALUE:
                        cheatList[e.RowIndex].Value = (string)edited_col;
                        break;
                    case CHEAT_LIST_DESC:
                        cheatList[e.RowIndex].Description = (string)edited_col;
                        break;
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void cheat_list_view_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                DataGridViewRow edited_row = cheat_list_view.Rows[e.RowIndex];

                object edited_col = null;

                switch (e.ColumnIndex)
                {
                    case CHEAT_LIST_ENABLED:
                        cheat_list_view.EndEdit();
                        cheatList[e.RowIndex].Value = cheatList[e.RowIndex].Value;
                        break;
                    case CHEAT_LIST_LOCK:
                        cheat_list_view.EndEdit();
                        edited_col = edited_row.Cells[e.ColumnIndex].Value;
                        cheatList[e.RowIndex].Lock = (bool)edited_col;
                        break;
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void add_address_btn_Click(object sender, EventArgs e)
        {
            NewAddress newAddress = new NewAddress();
            newAddress.ShowDialog();

            if (newAddress.succed)
            {
                ulong address = newAddress.address;
                string type = newAddress.type;
                string value = newAddress.value;
                string lock_ = newAddress.lock_;
                string description = newAddress.descriptioin.ToString();

                int sectionID = processManager.GetMappedSectionID(address);

                if (sectionID < 0)
                {
                    MessageBox.Show("Invalid Address!!");
                    return;
                }

                string sectionName = processManager.GetSectionName(sectionID);
                new_data_cheat(String.Format("{0:X}", address), type, value, sectionName, lock_, description);
            }
        }
        private void cheat_list_view_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                cheatList.RemoveAt(e.RowIndex);
            }
        }

        private void save_address_btn_Click(object sender, EventArgs e)
        {
            try
            {
                save_file_dialog.Filter = "Cheat files (*.cht)|*.cht";
                save_file_dialog.FilterIndex = 1;
                save_file_dialog.RestoreDirectory = true;

                if (save_file_dialog.ShowDialog() == DialogResult.OK)
                {
                    cheatList.SaveFile(save_file_dialog.FileName, (string)processes_comboBox.SelectedItem, processManager);
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void load_address_btn_Click(object sender, EventArgs e)
        {
            open_file_dialog.Filter = "Cheat files (*.cht)|*.cht";
            open_file_dialog.FilterIndex = 1;
            open_file_dialog.RestoreDirectory = true;

            if (open_file_dialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            cheat_list_view.Rows.Clear();
            cheatList.LoadFile(open_file_dialog.FileName, (string)processes_comboBox.SelectedItem, processManager);


            for (int i = 0; i < cheatList.Count; ++i)
            {
                Cheat cheat = cheatList[i];
                if (cheat.CheatType == CheatType.DATA_TYPE)
                {
                    add_new_row_of_cheat_list_view((DataCheat)cheat, ((DataCheat)cheat).SectionID);
                }
            }
        }

        private void refresh_cheat_list_Click(object sender, EventArgs e)
        {
            try
            {
                for (int i = 0; i < cheatList.Count; ++i)
                {
                    DataGridViewRow row = cheat_list_view.Rows[i];
                    cheatList[i].Refresh();

                    row.Cells[CHEAT_LIST_VALUE].Value = cheatList[i].Value;
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void refresh_lock_Tick(object sender, EventArgs e)
        {
            for (int i = 0; i < cheatList.Count; ++i)
            {
                try
                {
                    if (!cheatList[i].Lock)
                    {
                        continue;
                    }

                    cheatList[i].Value = cheatList[i].Value;
                }
                catch
                {

                }
            }
        }
        private void processes_comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                section_list_box.Items.Clear();
                result_list_view.Items.Clear();
                new_scan_btn.Enabled = true;
                valueTypeList.Enabled = true;
                compareTypeList.Enabled = true;
                section_list_box.Enabled = true;

                ProcessInfo processInfo = processManager.GetProcessInfo(processes_comboBox.Text);
                processManager.InitMemorySectionList(processInfo);

                section_list_box.BeginUpdate();
                for (int i = 0; i < processManager.MappedSectionList.Count; ++i)
                {
                    section_list_box.Items.Add(processManager.GetSectionName(i), false);
                }
                section_list_box.EndUpdate();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void get_processes_btn_Click(object sender, EventArgs e)
        {
            try
            {
                MemoryHelper.Connect(ip_box.Text);

                this.processes_comboBox.Items.Clear();
                ProcessList pl = MemoryHelper.GetProcessList();
                foreach (Process process in pl.processes)
                {
                    this.processes_comboBox.Items.Add(process.name);
                }
                this.processes_comboBox.SelectedIndex = 0;
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, exception.Source, MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
        }

        private void send_pay_load(string IP, string payloadPath, int port)
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            socket.Connect(new IPEndPoint(IPAddress.Parse(IP), port));
            socket.SendFile(payloadPath);
            socket.Close();
        }

        private void send_payload_btn_Click(object sender, EventArgs e)
        {
            try
            {
                string patch_path = Application.StartupPath;
                switch (version_list.SelectedIndex)
                {
                    case VERSION_LIST_405:
                        patch_path += @"\4.05\";
                        break;
                    case VERSION_LIST_455:
                        patch_path += @"\4.55\";
                        break;
                    default:
                        throw new System.ArgumentException("Unknown version.");
                }

                this.send_pay_load(this.ip_box.Text, patch_path + @"payload.bin", Convert.ToInt32(this.port_box.Text));
                System.Threading.Thread.Sleep(1000);
                this.msg.Text = "Injecting kpayload.elf...";
                this.send_pay_load(this.ip_box.Text, patch_path + @"kpayload.elf", 9023);
                System.Threading.Thread.Sleep(1000);
                this.msg.ForeColor = Color.Green;
                this.msg.Text = "Payload injected successfully!";
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, exception.Source, MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
        }

        private void InitCompareTypeListOfFirstScan()
        {
            compareTypeList.Items.Clear();
            switch (MemoryHelper.GetValueTypeByString((string)valueTypeList.SelectedItem))
            {
                case ValueType.ULONG_TYPE:
                case ValueType.UINT_TYPE:
                case ValueType.USHORT_TYPE:
                case ValueType.BYTE_TYPE:
                    compareTypeList.Items.AddRange(SEARCH_BY_BYTES_FIRST);
                    break;
                case ValueType.DOUBLE_TYPE:
                case ValueType.FLOAT_TYPE:
                    compareTypeList.Items.AddRange(SEARCH_BY_FLOAT_FIRST);
                    break;
                case ValueType.HEX_TYPE:
                case ValueType.STRING_TYPE:
                    compareTypeList.Items.AddRange(SEARCH_BY_HEX);
                    break;
                default:
                    throw new Exception("GetStringOfValueType!!!");
            }
            compareTypeList.SelectedIndex = 0;
        }

        private void InitCompareTypeListOfNextScan()
        {
            compareTypeList.Items.Clear();
            switch (MemoryHelper.GetValueTypeByString((string)valueTypeList.SelectedItem))
            {
                case ValueType.ULONG_TYPE:
                case ValueType.UINT_TYPE:
                case ValueType.USHORT_TYPE:
                case ValueType.BYTE_TYPE:
                    compareTypeList.Items.AddRange(SEARCH_BY_BYTES_NEXT);
                    break;
                case ValueType.DOUBLE_TYPE:
                case ValueType.FLOAT_TYPE:
                    compareTypeList.Items.AddRange(SEARCH_BY_FLOAT_NEXT);
                    break;
                case ValueType.HEX_TYPE:
                case ValueType.STRING_TYPE:
                    compareTypeList.Items.AddRange(SEARCH_BY_HEX);
                    break;
                default:
                    throw new Exception("GetStringOfValueType!!!");
            }
            compareTypeList.SelectedIndex = 0;
        }

        private void valueTypeList_SelectedIndexChanged(object sender, EventArgs e)
        {
            InitCompareTypeListOfFirstScan();
        }

        private void compareList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((string)compareTypeList.SelectedItem == CONSTANT.BETWEEN_VALUE)
            {
                value_1_box.Visible = true;
                value_label.Visible = true;
                and_label.Visible = true;
                value_box.Width = 112;
            }
            else
            {
                value_1_box.Visible = false;
                value_label.Visible = false;
                and_label.Visible = false;
                value_box.Width = 274;
            }
        }

        private void new_scan_worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            new_scan_btn.Text = CONSTANT.FIRST_SCAN;
            if (e.Error != null)
            {
                msg.Text = e.Error.Message;
            }
            setButtons(true);
        }

        private void update_result_list_worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            refresh_btn.Text = CONSTANT.REFRESH;
            if (e.Error != null)
            {
                msg.Text = e.Error.Message;
            }
            setButtons(true);
        }

        private void next_scan_worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            next_scan_btn.Text = CONSTANT.NEXT_SCAN;
            if (e.Error != null)
            {
                msg.Text = e.Error.Message;
            }
            setButtons(true);
        }
    }
}

