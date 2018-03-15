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
        private ProcessManager processManager = null;
        private MemoryHelper memoryHelper = null;
        private CheatList cheatList = new CheatList();

        private const int CHEAT_LIST_ADDRESS = 0;
        private const int CHEAT_LIST_TYPE = 1;
        private const int CHEAT_LIST_VALUE = 2;
        private const int CHEAT_LIST_SECTION = 3;
        private const int CHEAT_LIST_LOCK = 4;
        private const int CHEAT_LIST_DESC = 5;

        private const int RESULT_LIST_ADDRESS = 0;
        private const int RESULT_LIST_TYPE = 1;
        private const int RESULT_LIST_VALUE = 2;
        private const int RESULT_LIST_SECTION = 4;

        private const int VERSION_LIST_405 = 0;
        private const int VERSION_LIST_455 = 1;

        private const int VERSION_LIST_DEFAULT = VERSION_LIST_405;

        public main()
        {
            this.InitializeComponent();
        }

        private void main_Load(object sender, EventArgs e)
        {
            valueTypeList.SelectedIndex = 0;
            compareList.SelectedIndex = 0;

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
            public HashSet<int> MappedSectionCheckeSet { get; set; }
            public ulong Results { get; set; }
        }

        class WorkerArgs
        {
            public string ValueType { get; set; }
        }

        private void update_result_list_view(BackgroundWorker worker, string value_type, bool refresh, int start, float percent)
        {
            worker.ReportProgress(start, 0);

            List<ListViewItem> listViewItems = new List<ListViewItem>();
            HashSet<int> mappedSectionCheckeSet = new HashSet<int>();

            ulong totalAddressCount = processManager.TotalAddressCount();
            ulong curAddressCount = 0;

            for (int idx = 0; idx < processManager.MappedSectionList.Count; ++idx)
            {
                MappedSection mapped_section = processManager.MappedSectionList[idx];
                AddressList address_list = mapped_section.AddressList;
                if (address_list.Count > 0)
                {
                    mappedSectionCheckeSet.Add(idx);
                }
                for (int i = 0; i < address_list.Count; i++)
                {
                    if (curAddressCount >= 0x10000)
                    {
                        break;
                    }

                    curAddressCount++;
                    ListViewItem lvi = new ListViewItem();

                    lvi.Text = String.Format("{0:X}", address_list[i].AddressOffset + mapped_section.Start);

                    byte[] match_bytes = BitConverter.GetBytes(address_list[i].MemoryValue);

                    if (refresh)
                    {
                        match_bytes = memoryHelper.GetBytesByType(address_list[i].AddressOffset + mapped_section.Start);
                        Address address_tmp = new Address();
                        address_tmp.AddressOffset = address_list[i].AddressOffset;
                        address_tmp.MemoryValue = memoryHelper.BytesToUlong(match_bytes);
                        address_list[i] = address_tmp;
                    }

                    string value_output = memoryHelper.BytesToString(match_bytes);

                    lvi.SubItems.Add(value_type);
                    lvi.SubItems.Add(value_output);
                    lvi.SubItems.Add(MemoryHelper.bytes_to_hex_string(match_bytes));
                    lvi.SubItems.Add(processManager.GetSectionName(idx));

                    listViewItems.Add(lvi);

                    if (i % 500 == 0)
                    {
                        worker.ReportProgress(start + (int)(i / (float)curAddressCount * 100 * percent));
                    }
                }
            }

            WorkerReturn workerReturn = new WorkerReturn();
            workerReturn.ListViewItems = listViewItems;
            workerReturn.MappedSectionCheckeSet = mappedSectionCheckeSet;
            workerReturn.Results = totalAddressCount;

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
        }

        private void new_scan_btn_Click(object sender, EventArgs e)
        {
            try
            {
                if (processManager == null)
                {
                    return;
                }

                if (MessageBox.Show("search size:" + (processManager.TotalMemorySize / 1024).ToString() + "KB") != DialogResult.OK)
                {
                    return;
                }

                memoryHelper.InitMemoryHandler((string)valueTypeList.SelectedItem, (CompareType)compareList.SelectedIndex);
                result_list_view.Items.Clear();
                processManager.ClearAddressList();

                WorkerArgs args = new WorkerArgs();
                args.ValueType = (string)valueTypeList.SelectedItem;
                setButtons(false);
                new_scan_worker.RunWorkerAsync(args);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void processes_comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                section_list_box.Items.Clear();
                result_list_view.Items.Clear();
                cheat_list_view.Rows.Clear();
                cheatList.Clear();

                ProcessInfo processInfo = processManager.GetProcessInfo(processes_comboBox.Text);
                processManager.InitMemorySectionList(processInfo);
                MemoryHelper.ProcessID = processManager.ProcessID;

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

        private void refresh_Click(object sender, EventArgs e)
        {
            try
            {
                if (processManager == null)
                {
                    return;
                }

                memoryHelper.InitMemoryHandler((string)valueTypeList.SelectedItem, (CompareType)compareList.SelectedIndex);
                WorkerArgs args = new WorkerArgs();
                args.ValueType = (string)valueTypeList.SelectedItem;
                setButtons(false);
                update_result_list_worker.RunWorkerAsync(args);
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
                if (processManager == null)
                {
                    return;
                }

                memoryHelper.InitMemoryHandler((string)valueTypeList.SelectedItem, (CompareType)compareList.SelectedIndex);
                WorkerArgs args = new WorkerArgs();
                args.ValueType = (string)valueTypeList.SelectedItem;
                setButtons(false);
                next_scan_worker.RunWorkerAsync(args);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void send_pay_load(string IP, string payloadPath, int port)
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            socket.Connect(new IPEndPoint(IPAddress.Parse(IP), port));
            socket.SendFile(payloadPath);
            socket.Close();
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
                if (ret.MappedSectionCheckeSet.Contains(i))
                {
                    section_list_box.SetItemChecked(i, true);
                }
                else
                {
                    section_list_box.SetItemChecked(i, false);
                }
            }
            msg.Text = ret.Results + " results";
        }

        private void update_result_list_worker_DoWork(object sender, DoWorkEventArgs e)
        {
            WorkerArgs args = (WorkerArgs)e.Argument;
            update_result_list_view(update_result_list_worker, args.ValueType, true, 0, 1.0f);
        }
        private void next_scan_worker_DoWork(object sender, DoWorkEventArgs e)
        {
            ulong percent_len = 0;

            for (int section_idx = 0; section_idx < processManager.MappedSectionList.Count; ++section_idx)
            {
                MappedSection mappedSection = processManager.MappedSectionList[section_idx];
                AddressList addressList = mappedSection.AddressList;

                AddressList filtered_list = mappedSection.getFilteredAddressList(processManager, memoryHelper, value_box.Text,
                    next_scan_worker, ref percent_len, 0, 0.5f);
                mappedSection.AddressList = addressList.Intersect(memoryHelper, filtered_list, value_box.Text);
            }

            WorkerArgs args = (WorkerArgs)e.Argument;
            update_result_list_view(next_scan_worker, args.ValueType, false, 50, 0.5f);
        }

        private void new_scan_worker_DoWork(object sender, DoWorkEventArgs e)
        {
            WorkerArgs args = (WorkerArgs)e.Argument;
            ulong lenPercent = 0;
            for (int section_idx = 0; section_idx < processManager.MappedSectionList.Count; ++section_idx)
            {
                MappedSection mappedSection = processManager.MappedSectionList[section_idx];
                mappedSection.AddressList = mappedSection.getFilteredAddressList(processManager, memoryHelper, value_box.Text,
                    new_scan_worker, ref lenPercent, 0, 0.5f);
            }
            update_result_list_view(new_scan_worker, args.ValueType, false, 50, 0.5f);
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

            if (e.ProgressPercentage == 100 && e.UserState is WorkerReturn)
            {
                update_result_list_view_ui((WorkerReturn)e.UserState);
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

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                msg.Text = e.Error.Message;
            }
            setButtons(true);
        }

        private void sectionView_Click(object sender, EventArgs e)
        {
            if (section_list_box.SelectedIndex >= 0)
            {
                MappedSection section = processManager.MappedSectionList[section_list_box.SelectedIndex];
                HexEdit hexEdit = new HexEdit(memoryHelper, 0, section);
                hexEdit.Show(this);
            }
        }
        private void sectionDump_Click(object sender, EventArgs e)
        {
            if (section_list_box.SelectedIndex >= 0)
            {
                MappedSection section = processManager.MappedSectionList[section_list_box.SelectedIndex];

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

        void add_new_row_of_cheat_list_view(HexCheat cheat, int sectionID)
        {
            int index = this.cheat_list_view.Rows.Add();

            DataGridViewRow cheat_list_view_item = cheat_list_view.Rows[index];
            cheat_list_view_item.Cells[CHEAT_LIST_ADDRESS].Value = cheat.Address;
            cheat_list_view_item.Cells[CHEAT_LIST_TYPE].Value = MemoryHelper.GetStringOfValueType(cheat.Type);
            cheat_list_view_item.Cells[CHEAT_LIST_VALUE].Value = cheat.Value;
            cheat_list_view_item.Cells[CHEAT_LIST_SECTION].Value = processManager.GetSectionName(sectionID);
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
                else if (cheatType == CheatType.HEX_TYPE)
                {
                    HexCheat hexCheat = new HexCheat(processManager, address_str, sectionID, value, lock_, valueType, description);
                    add_new_row_of_cheat_list_view(hexCheat, sectionID);
                    cheatList.Add(hexCheat);
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
                if (e.ColumnIndex == CHEAT_LIST_VALUE)
                {
                    DataGridViewRow row = cheat_list_view.Rows[e.RowIndex];
                    cheatList[e.RowIndex].Value = (string)row.Cells[CHEAT_LIST_VALUE].Value;
                }
                else if (e.ColumnIndex == CHEAT_LIST_LOCK)
                {
                    DataGridViewRow row = cheat_list_view.Rows[e.RowIndex];
                    cheatList[e.RowIndex].Lock = (bool)row.Cells[CHEAT_LIST_LOCK].Value;
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void add_address_btn_Click(object sender, EventArgs e)
        {
            if (processManager == null)
            {
                return;
            }

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
                if (processManager == null)
                {
                    return;
                }

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
            if (processManager == null)
            {
                return;
            }

            open_file_dialog.Filter = "Cheat files (*.cht)|*.cht";
            open_file_dialog.FilterIndex = 1;
            open_file_dialog.RestoreDirectory = true;

            if (open_file_dialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            cheatList.LoadFile(open_file_dialog.FileName, (string)processes_comboBox.SelectedItem, processManager);
            cheat_list_view.Rows.Clear();

            for (int i = 0; i < cheatList.Count; ++i)
            {
                Cheat cheat = cheatList[i];
                if (cheat.CheatType == CheatType.DATA_TYPE)
                {
                    add_new_row_of_cheat_list_view((DataCheat)cheat, ((DataCheat)cheat).SectionID);
                }
                else if (cheat.CheatType == CheatType.HEX_TYPE)
                {
                    add_new_row_of_cheat_list_view((HexCheat)cheat, ((HexCheat)cheat).SectionID);
                }
            }
        }

        private void refresh_cheat_list_Click(object sender, EventArgs e)
        {
            try
            {
                if (processManager == null)
                {
                    return;
                }

                for (int i = 0; i < cheatList.Count; ++i)
                {
                    DataGridViewRow row = cheat_list_view.Rows[i];
                    cheatList[i].Refresh();

                    row.Cells[CHEAT_LIST_VALUE].Value = cheatList[i].Value;
                    //memoryHelper.InitMemoryHandler(type, CompareType.NONE);

                    //row.Cells[CHEAT_LIST_VALUE].Value = memoryHelper.BytesToString(memoryHelper.GetBytesByType(address));
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

                    //memoryHelper.InitMemoryHandler(type, CompareType.NONE);
                    //memoryHelper.SetBytesByType(address, memoryHelper.StringToBytes(value));
                }
                catch
                {

                }
            }
        }

        private void get_processes_btn_Click(object sender, EventArgs e)
        {
            try
            {
                MemoryHelper.Connect(ip_box.Text);
                processManager = new ProcessManager();
                memoryHelper = new MemoryHelper();
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
    }
}

