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
        private ProcessManager processManager;

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

        private const int CHEAT_CODE_HEADER_VERSION = 0;
        private const int CHEAT_CODE_HEADER_PROCESS_NAME = 1;
        private const int CHEAT_CODE_HEADER_ELEMENT_COUNT = CHEAT_CODE_HEADER_PROCESS_NAME + 1;

        private const int CHEAT_CODE_TYPE = 0;
        private const int CHEAT_CODE_DATA_TYPE_SECTION_ID = 1;
        private const int CHEAT_CODE_DATA_TYPE_ADDRESS_OFFSET = 2;
        private const int CHEAT_CODE_DATA_TYPE_VALUE_TYPE = 3;
        private const int CHEAT_CODE_DATA_TYPE_VALUE = 4;
        private const int CHEAT_CODE_DATA_TYPE_FLAG = 5;
        private const int CHEAT_CODE_DATA_TYPE_DESCRIPTION = 6;

        private const int CHEAT_CODE_DATA_TYPE_ELEMENT_COUNT = CHEAT_CODE_DATA_TYPE_DESCRIPTION + 1;

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
        }

        private void main_FormClosing(object sender, FormClosingEventArgs e)
        {

            string ip = Config.getSetting("ip");
            string version = "";
            switch(version_list.SelectedIndex)
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

            for (int idx = 0; idx < processManager.mapped_section_list.Count; ++idx)
            {
                MappedSection mapped_section = processManager.mapped_section_list[idx];
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
                        match_bytes = processManager.MemoryHelper.GetBytesByType(address_list[i].AddressOffset + mapped_section.Start);
                        Address address_tmp = new Address();
                        address_tmp.AddressOffset = address_list[i].AddressOffset;
                        address_tmp.MemoryValue = processManager.MemoryHelper.BytesToUint(match_bytes);
                        address_list[i] = address_tmp;
                    }

                    string value_output = processManager.MemoryHelper.BytesToString(match_bytes);

                    lvi.SubItems.Add(value_type);
                    lvi.SubItems.Add(value_output);
                    lvi.SubItems.Add(processManager.MemoryHelper.PrintBytesByHex(match_bytes));
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
                if (MessageBox.Show("search size:" + (processManager.TotalMemorySize / 1024).ToString() + "KB") != DialogResult.OK)
                {
                    return;
                }

                processManager.MemoryHelper.InitMemoryHandler((ValueType)valueTypeList.SelectedIndex, (CompareType)compareList.SelectedIndex);
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
                ProcessInfo processInfo = processManager.GetProcessInfo(processes_comboBox.Text);
                processManager.InitMemorySectionList(processInfo);
                processManager.MemoryHelper.ProcessID = processManager.ProcessID;

                section_list_box.BeginUpdate();
                for (int i = 0; i < processManager.mapped_section_list.Count; ++i)
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
                processManager.MemoryHelper.InitMemoryHandler((ValueType)valueTypeList.SelectedIndex, (CompareType)compareList.SelectedIndex);
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
                processManager.MemoryHelper.InitMemoryHandler((ValueType)valueTypeList.SelectedIndex, (CompareType)compareList.SelectedIndex);
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
            for (int section_idx = 0; section_idx < processManager.mapped_section_list.Count; ++section_idx)
            {
                MappedSection mappedSection = processManager.mapped_section_list[section_idx];
                AddressList addressList = mappedSection.AddressList;

                if (mappedSection.AddressList.Count < 50)
                {
                    for (int address_idx = 0; address_idx < addressList.Count; ++address_idx)
                    {
                        Address address = addressList[address_idx];

                        byte[] compare_value = processManager.MemoryHelper.GetCompareBytes(address, value_box.Text);

                        byte[] value = processManager.MemoryHelper.GetBytesByType(address.AddressOffset + mappedSection.Start);
                        if (!processManager.MemoryHelper.Compare(compare_value, value))
                        {
                            addressList.RemoveAt(address_idx);
                            --address_idx;
                        }
                        else
                        {
                            Address address_tmp = new Address();
                            address_tmp.AddressOffset = addressList[address_idx].AddressOffset;
                            address_tmp.MemoryValue = processManager.MemoryHelper.BytesToUint(value);
                            addressList[address_idx] = address_tmp;
                        }
                    }
                }
                else
                {
                    AddressList filtered_list = mappedSection.getFilteredAddressList(processManager, value_box.Text,
                        next_scan_worker, ref percent_len, 0, 0.5f);
                    mappedSection.AddressList = addressList.Intersect(processManager.MemoryHelper, filtered_list, value_box.Text);
                }
            }

            WorkerArgs args = (WorkerArgs)e.Argument;
            update_result_list_view(next_scan_worker, args.ValueType, false, 50, 0.5f);
        }

        private void new_scan_worker_DoWork(object sender, DoWorkEventArgs e)
        {
            WorkerArgs args = (WorkerArgs)e.Argument;
            ulong lenPercent = 0;
            for (int section_idx = 0; section_idx < processManager.mapped_section_list.Count; ++section_idx)
            {
                MappedSection mappedSection = processManager.mapped_section_list[section_idx];
                mappedSection.AddressList = mappedSection.getFilteredAddressList(processManager, value_box.Text,
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

        private void section_list_box_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            processManager.SectionCheck(e.Index, e.NewValue == CheckState.Checked);
        }

        void add_to_cheat_list_view(string address_str, string type, string value, string section, string flag, string description)
        {
            try
            {
                ulong address = ulong.Parse(address_str, NumberStyles.HexNumber);
                int sectionID = processManager.GetSectionInfoIdx(address);

                if (sectionID == -1)
                {
                    MessageBox.Show("Address is out of range!");
                    return;
                }

                for (int i = 0; i < cheat_list_view.Rows.Count; ++i)
                {
                    DataGridViewRow item = cheat_list_view.Rows[i];
                    if ((string)item.Cells[CHEAT_LIST_ADDRESS].Value == address_str)
                    {
                        return;
                    }
                }

                int index = this.cheat_list_view.Rows.Add();
                ulong flag_u = ulong.Parse(flag, NumberStyles.HexNumber);

                DataGridViewRow cheat_list_view_item = cheat_list_view.Rows[index];
                cheat_list_view_item.Cells[CHEAT_LIST_ADDRESS].Value = (address_str);
                cheat_list_view_item.Cells[CHEAT_LIST_TYPE].Value = (type);
                cheat_list_view_item.Cells[CHEAT_LIST_VALUE].Value = (value);
                cheat_list_view_item.Cells[CHEAT_LIST_SECTION].Value = (section);
                cheat_list_view_item.Cells[CHEAT_LIST_LOCK].Value = (flag_u & CONSTANT.SAVE_FLAG_LOCK) == CONSTANT.SAVE_FLAG_LOCK ? true : false;
                cheat_list_view_item.Cells[CHEAT_LIST_DESC].Value = (description);

                processManager.MemoryHelper.InitMemoryHandler(type, CompareType.NONE);
                processManager.MemoryHelper.SetBytesByType(address, processManager.MemoryHelper.StringToBytes(value));
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

                add_to_cheat_list_view(address, type, value, section, flag, description);
            }

        }

        private void cheat_list_view_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != 2)
            {
                return;
            }

            try
            {
                string value_str = (string)cheat_list_view.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                ulong address = ulong.Parse((string)cheat_list_view.Rows[e.RowIndex].Cells[0].Value, NumberStyles.HexNumber);
                string type = (string)cheat_list_view.Rows[e.RowIndex].Cells[1].Value;

                processManager.MemoryHelper.InitMemoryHandler(type, CompareType.NONE);

                int section_idx = processManager.GetSectionInfoIdx(address);

                if (section_idx == -1)
                {
                    throw new System.ArgumentException("Invalid address");
                }

                processManager.MemoryHelper.SetBytesByType(address, processManager.MemoryHelper.StringToBytes(value_str));
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void sectionDump_Click(object sender, EventArgs e)
        {
            if (section_list_box.SelectedIndex >= 0)
            {
                MappedSection section = processManager.mapped_section_list[section_list_box.SelectedIndex];

                save_file_dialog.Filter = "Binary files (*.bin)|*.bin|All files (*.*)|*.*";
                save_file_dialog.FilterIndex = 1;
                save_file_dialog.RestoreDirectory = true;
                save_file_dialog.FileName = (string)section_list_box.Items[section_list_box.SelectedIndex];

                if (save_file_dialog.ShowDialog() == DialogResult.OK)
                {
                    byte[] buffer = processManager.MemoryHelper.ReadMemory(section.Start, (int)section.Length);

                    FileStream myStream = new FileStream(save_file_dialog.FileName, FileMode.OpenOrCreate);
                    myStream.Write(buffer, 0, buffer.Length);
                    myStream.Close();
                }
            }
        }

        private void sectionView_Click(object sender, EventArgs e)
        {
            if (section_list_box.SelectedIndex >= 0)
            {
                MappedSection section = processManager.mapped_section_list[section_list_box.SelectedIndex];
                HexEdit hexEdit = new HexEdit(processManager.MemoryHelper, 0, section);
                hexEdit.Show(this);
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

                int sectionID = processManager.GetSectionInfoIdx(address);

                if (sectionID < 0)
                {
                    MessageBox.Show("Invalid Address!!");
                    return;
                }

                string sectionName = processManager.GetSectionName(sectionID);
                add_to_cheat_list_view(String.Format("{0:X}", address), type, value, sectionName, lock_, description);
            }
        }

        private void save_address_btn_Click(object sender, EventArgs e)
        {
            try
            {
                string save_buf = "1.2|" + (string)processes_comboBox.SelectedItem + "\n";
                for (int i = 0; i < cheat_list_view.RowCount; ++i)
                {
                    DataGridViewRow row = cheat_list_view.Rows[i];
                    string address_str = (string)row.Cells[CHEAT_LIST_ADDRESS].Value;
                    ulong address = ulong.Parse(address_str, NumberStyles.HexNumber);
                    string type = (string)row.Cells[CHEAT_LIST_TYPE].Value;
                    string value = (string)row.Cells[CHEAT_LIST_VALUE].Value;
                    string lock_ = ((bool)row.Cells[CHEAT_LIST_LOCK].Value) ? "1" : "0";
                    string description = (string)row.Cells[CHEAT_LIST_DESC].Value;

                    int sectionID = processManager.GetSectionInfoIdx(address);

                    if (sectionID < 0)
                    {
                        continue;
                    }

                    MappedSection sectionInfo = processManager.mapped_section_list[sectionID];


                    save_buf += "data|";
                    save_buf += sectionID + "|";
                    save_buf += String.Format("{0:X}", (address - sectionInfo.Start)) + "|";
                    save_buf += type + "|";
                    save_buf += value + "|";
                    save_buf += lock_ + "|";
                    save_buf += description + "|\n";
                }

                save_file_dialog.Filter = "Cheat files (*.cht)|*.cht";
                save_file_dialog.FilterIndex = 1;
                save_file_dialog.RestoreDirectory = true;

                if (save_file_dialog.ShowDialog() == DialogResult.OK)
                {
                    StreamWriter myStream = new StreamWriter(save_file_dialog.FileName);
                    myStream.Write(save_buf);
                    myStream.Close();
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void load_1_2_cheat_code(string[] cheats)
        {
            for (int i = 1; i < cheats.Length; ++i)
            {
                string cheat = cheats[i];
                try
                {
                    string[] cheat_elements = cheat.Split('|');

                    if (cheat_elements.Length == 0)
                    {
                        continue;
                    }

                    if (cheat_elements[CHEAT_CODE_TYPE] == "data")
                    {
                        if (cheat_elements.Length < CHEAT_CODE_DATA_TYPE_ELEMENT_COUNT)
                        {
                            continue;
                        }

                        int idx = int.Parse(cheat_elements[CHEAT_CODE_DATA_TYPE_SECTION_ID]);
                        if (idx >= processManager.mapped_section_list.Count || idx < 0)
                        {
                            MessageBox.Show("Invalid address.");
                            continue;
                        }

                        ulong address = ulong.Parse(cheat_elements[CHEAT_CODE_DATA_TYPE_ADDRESS_OFFSET], NumberStyles.HexNumber) + processManager.mapped_section_list[idx].Start;
                        string type = cheat_elements[CHEAT_CODE_DATA_TYPE_VALUE_TYPE];
                        string value = cheat_elements[CHEAT_CODE_DATA_TYPE_VALUE];
                        string section = processManager.GetSectionName(idx);
                        ulong flag = ulong.Parse(cheat_elements[CHEAT_CODE_DATA_TYPE_FLAG], NumberStyles.HexNumber);
                        string description = cheat_elements[CHEAT_CODE_DATA_TYPE_DESCRIPTION];

                        add_to_cheat_list_view(String.Format("{0:X}", address), type, value, section, flag.ToString(), description);
                    }
                    else
                    {
                        continue;
                    }
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.Message);
                }
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
            string[] cheats = File.ReadAllLines(open_file_dialog.FileName);

            if (cheats.Length < 2)
            {
                return;
            }

            string header = cheats[0];
            string[] header_items = header.Split('|');

            if (header_items.Length < CHEAT_CODE_HEADER_ELEMENT_COUNT)
            {
                return;
            }

            string version = header_items[CHEAT_CODE_HEADER_VERSION];

            if (version == "1.2")
            {
                string process_name = header_items[CHEAT_CODE_HEADER_PROCESS_NAME];
                if (process_name != (string)processes_comboBox.SelectedItem)
                {
                    MessageBox.Show("Invalid process.");
                    return;
                }

                load_1_2_cheat_code(cheats);
            }
            else
            {
                MessageBox.Show("Invalid version.");
                return;
            }
        }

        private void refresh_cheat_list_Click(object sender, EventArgs e)
        {
            try
            {
                for (int i = 0; i < cheat_list_view.RowCount; ++i)
                {
                    DataGridViewRow row = cheat_list_view.Rows[i];
                    string address_str = (string)row.Cells[CHEAT_LIST_ADDRESS].Value;
                    string type = (string)row.Cells[CHEAT_LIST_TYPE].Value;

                    ulong address = ulong.Parse(address_str, NumberStyles.HexNumber);

                    if (-1 == processManager.GetSectionInfoIdx(address))
                    {
                        continue;
                    }

                    processManager.MemoryHelper.InitMemoryHandler(type, CompareType.NONE);

                    row.Cells[CHEAT_LIST_VALUE].Value = processManager.MemoryHelper.BytesToString(processManager.MemoryHelper.GetBytesByType(address));
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }


        private void refresh_lock_Tick(object sender, EventArgs e)
        {
            for (int i = 0; i < cheat_list_view.RowCount; ++i)
            {
                try
                {
                    DataGridViewRow row = cheat_list_view.Rows[i];

                    if (!(bool)row.Cells[CHEAT_LIST_LOCK].Value)
                    {
                        continue;
                    }

                    string address_str = (string)row.Cells[CHEAT_LIST_ADDRESS].Value;
                    ulong address = ulong.Parse(address_str, NumberStyles.HexNumber);
                    string type = (string)row.Cells[CHEAT_LIST_TYPE].Value;
                    string value = (string)row.Cells[CHEAT_LIST_VALUE].Value;

                    int sectionID = processManager.GetSectionInfoIdx(address);

                    if (sectionID < 0)
                    {
                        continue;
                    }

                    processManager.MemoryHelper.InitMemoryHandler(type, CompareType.NONE);
                    processManager.MemoryHelper.SetBytesByType(address, processManager.MemoryHelper.StringToBytes(value));
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
                PS4RPC ps4 = new PS4RPC(ip_box.Text);
                ps4.Connect();
                MemoryHelper memoryHelper = new MemoryHelper(ps4);

                this.processes_comboBox.Items.Clear();
                ProcessList pl = ps4.GetProcessList();
                foreach (Process process in pl.processes)
                {
                    this.processes_comboBox.Items.Add(process.name);
                }

                processManager = new ProcessManager();
                processManager.MemoryHelper = memoryHelper;

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

