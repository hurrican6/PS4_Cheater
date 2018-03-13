using Be.Windows.Forms;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Collections;
using System.Collections.Generic;

namespace PS4_Cheater
{
    public class MemoryViewByteProvider : IByteProvider
    {
        private bool _hasChanges;
        private ByteCollection _bytes;
        public List<int> change_list { get; set; }

        [field: CompilerGenerated, DebuggerBrowsable(0)]
        public event EventHandler Changed;

        [field: CompilerGenerated, DebuggerBrowsable(0)]
        public event EventHandler LengthChanged;

        public MemoryViewByteProvider(byte[] data) : this(new ByteCollection(data))
        {
            change_list = new List<int>();
        }

        public MemoryViewByteProvider(ByteCollection bytes)
        {
            this._bytes = bytes;
        }

        public void ApplyChanges()
        {
            this._hasChanges = false;
        }

        public void DeleteBytes(long index, long length)
        {

        }

        public bool HasChanges() =>
            this._hasChanges;

        public void InsertBytes(long index, byte[] bs)
        {

        }

        private void OnChanged(EventArgs e)
        {
            this._hasChanges = true;
            if (this.Changed != null)
            {
                this.Changed(this, e);
            }
        }

        private void OnLengthChanged(EventArgs e)
        {
            if (this.LengthChanged != null)
            {
                this.LengthChanged(this, e);
            }
        }

        public byte ReadByte(long index) =>
            this._bytes[(int)index];

        public bool SupportsDeleteBytes() =>
            false;

        public bool SupportsInsertBytes() =>
            false;

        public bool SupportsWriteByte() =>
            true;

        public void WriteByte(long index, byte value)
        {
            this._bytes[(int)index] = value;
            this.change_list.Add((int)index);
            this.OnChanged(EventArgs.Empty);
        }

        public ByteCollection Bytes =>
            this._bytes;

        public long Length =>
            ((long)this._bytes.Count);

        public long Offset =>
            0L;
    }
}
